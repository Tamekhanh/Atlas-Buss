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

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IPurchaseOrderBillRepository, PurchaseOrderBillRepository>();
        services.AddScoped<ISalesOrderBillRepository, SalesOrderBillRepository>();        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        services.AddScoped<IPricelistRepository, PricelistRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IStorageProvider, LocalStorageProvider>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IAttributeRepository, AttributeRepository>();

        return services;
    }
}