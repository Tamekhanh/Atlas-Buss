using System.Security.Cryptography;
using System.Text;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;

namespace Atlas.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogService _logService;

        public AuthService(IAuthRepository authRepository, ILogService logService)
        {
            _authRepository = authRepository;
            _logService = logService;
        }

        public async Task<AuthenticatedUser?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var account = await _authRepository.GetByUsernameAsync(username.Trim());
            if (account is null || !account.IsActive)
            {
                return null;
            }

            if (!VerifyPassword(password, account.PasswordHash))
            {
                return null;
            }

            await _authRepository.UpdateLastLoginAsync(account.EmployeeId, DateTime.UtcNow);
            await _logService.AddLogAsync(account.EmployeeId, "User login");

            var permissionKeys = account.Role?.RolePermissions?
                .Where(rp => rp.Permission is not null && !string.IsNullOrWhiteSpace(rp.Permission.PermissionKey))
                .Select(rp => rp.Permission!.PermissionKey)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            bool hasPermission(string token) =>
                permissionKeys.Any(key => key.Contains(token, StringComparison.OrdinalIgnoreCase));

            return new AuthenticatedUser
            {
                EmployeeId = account.EmployeeId,
                Username = account.Username,
                FullName = account.Employee?.FullName ?? string.Empty,
                RoleId = account.RoleId,
                RoleName = account.Role?.RoleName,
                PermissionKeys = permissionKeys,
                CanProduct = hasPermission("PRODUCT"),
                CanSale = hasPermission("SALE"),
                CanEmployee = hasPermission("EMPLOYEE"),
                CanInventory = hasPermission("INVENTORY"),
                CanAdministration = hasPermission("ADMIN"),
                CanHR = hasPermission("HR")
            };
        }

        public async Task<string?> RegisterAsync(string employeeNumber, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(employeeNumber) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Vui long nhap day du thong tin.";
            }

            var normalizedEmployeeNumber = employeeNumber.Trim();
            var normalizedUsername = username.Trim();

            if (await _authRepository.UsernameExistsAsync(normalizedUsername))
            {
                return "Username da ton tai.";
            }

            var employee = await _authRepository.GetEmployeeByNumberAsync(normalizedEmployeeNumber);
            if (employee is null)
            {
                return "Khong tim thay nhan vien voi Employee Number nay.";
            }

            if (employee.Account is not null)
            {
                return "Nhan vien nay da co tai khoan.";
            }

            var account = new Atlas.Core.Entities.EmployeeAccount
            {
                EmployeeId = employee.Id,
                Username = normalizedUsername,
                PasswordHash = HashPassword(password),
                IsActive = true,
                LastLogin = null,
                RoleId = null
            };

            var created = await _authRepository.AddAccountAsync(account);
            if (created)
            {
                await _logService.AddLogAsync(employee.Id, $"Account registration: {normalizedUsername}");
            }

            return created ? null : "Khong the tao tai khoan.";
        }

        private static bool VerifyPassword(string plainTextPassword, string storedPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(storedPasswordHash))
            {
                return false;
            }

            if (string.Equals(storedPasswordHash, plainTextPassword, StringComparison.Ordinal))
            {
                return true;
            }

            var computedSha256 = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(plainTextPassword)));

            return string.Equals(storedPasswordHash, computedSha256, StringComparison.OrdinalIgnoreCase);
        }

        private static string HashPassword(string plainTextPassword)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(plainTextPassword)));
        }
    }
}
