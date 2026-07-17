using Atlas.Core.Interfaces;
using Atlas.Services.Auth;
using Atlas.Services.HRM;
using Atlas.Services.Inventory;
using Atlas.Services.Category;
using Atlas.Services.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPartyService, PartyService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<ISalesOrderService, SalesOrderService>();
        services.AddScoped<IAttributeService, AttributeService>();
        services.AddScoped<IDocumentNumberService, DocumentNumberService>();

        return services;
    }
}