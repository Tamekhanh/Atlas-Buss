using Atlas.Core.Models;

namespace Atlas.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticatedUser?> AuthenticateAsync(string username, string password);
        Task<string?> RegisterAsync(string employeeNumber, string username, string password);
    }
}
