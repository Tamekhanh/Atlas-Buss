using Atlas.Core.Interfaces;
using Atlas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextPool<AtlasDBContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        services.AddScoped<IPricelistRepository, PricelistRepository>();
        services.AddScoped<IVendorCompanyRepository, VendorCompanyRepository>();
        services.AddScoped<ICustomerCompanyRepository, CustomerCompanyRepository>();
        services.AddScoped<IVendorPersonRepository, VendorPersonRepository>();
        services.AddScoped<ICustomerPersonRepository, CustomerPersonRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ILogRepository, LogRepository>();

        return services;
    }
}