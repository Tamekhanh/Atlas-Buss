using Atlas.Core.Interfaces;
using Atlas.Services.Auth;
using Atlas.Services.Customer;
using Atlas.Services.HRM;
using Atlas.Services.Inventory;
using Atlas.Services.Vendor;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IVendorCompanyService, VendorCompanyService>();
        services.AddScoped<ICustomerCompanyService, CustomerCompanyService>();
        services.AddScoped<IVendorPersonService, VendorPersonService>();
        services.AddScoped<ICustomerPersonService, CustomerPersonService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILogService, LogService>();

        return services;
    }
}