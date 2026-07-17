using Atlas.Core.Interfaces;
using Atlas.Infrastructure;
using Atlas.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login/SignIn";
        options.AccessDeniedPath = "/Account/Login/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents
        {
                OnValidatePrincipal = async context =>
                {
                    var username = context.Principal?.FindFirstValue(ClaimTypes.Name);
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return;
                    }

                    // Avoid querying DB on every request: revalidate only if cookie was issued more than 15 minutes ago
                    var issuedUtc = context.Properties?.IssuedUtc;
                    if (issuedUtc.HasValue && DateTimeOffset.UtcNow - issuedUtc.Value < TimeSpan.FromMinutes(15))
                    {
                        return;
                    }

                    var authRepository = context.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();
                    var isActive = await authRepository.IsActiveByUsernameAsync(username);

                    if (!isActive)
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    else
                    {
                        // update issued time so we won't revalidate immediately again
                        context.Properties!.IssuedUtc = DateTimeOffset.UtcNow;
                    }
                }
        };
    });

    

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductManage", policy =>
        policy.RequireClaim("permission", "PRODUCT_MANAGE", "ADMIN"));
    options.AddPolicy("ProductView", policy =>
        policy.RequireClaim("permission", "PRODUCT_MANAGE", "PRODUCT_VIEW"));

    options.AddPolicy("HRManage", policy =>
        policy.RequireClaim("permission", "HR_MANAGE", "ADMIN"));
    options.AddPolicy("HRMView", policy =>
        policy.RequireClaim("permission", "HR_MANAGE", "HR_VIEW"));

    options.AddPolicy("PartyManage", policy =>
        policy.RequireClaim("permission", "PARTY_MANAGE", "ADMIN"));
    options.AddPolicy("PartyView", policy =>
        policy.RequireClaim("permission", "PARTY_MANAGE", "PARTY_VIEW"));

    options.AddPolicy("CategoryManage", policy =>
        policy.RequireClaim("permission", "CATEGORY_MANAGE", "ADMIN"));
    options.AddPolicy("CategoryView", policy =>
        policy.RequireClaim("permission", "CATEGORY_MANAGE", "CATEGORY_VIEW"));

    options.AddPolicy("Administration", policy =>
        policy.RequireClaim("permission", "ADMIN"));
    
    options.AddPolicy("PurchaseManage", policy =>
        policy.RequireClaim("permission", "PURCHASE_MANAGE", "ADMIN"));
    options.AddPolicy("PurchaseView", policy =>
        policy.RequireClaim("permission", "PURCHASE_MANAGE", "PURCHASE_VIEW"));
    
    options.AddPolicy("SaleManage", policy =>
        policy.RequireClaim("permission", "SALE_MANAGE", "ADMIN"));
    options.AddPolicy("SaleView", policy =>
        policy.RequireClaim("permission", "SALE_MANAGE", "SALE_VIEW"));

    options.AddPolicy("WarehouseManage", policy =>
        policy.RequireClaim("permission", "WAREHOUSE_MANAGE", "ADMIN"));
    options.AddPolicy("WarehouseView", policy =>
        policy.RequireClaim("permission", "WAREHOUSE_MANAGE", "WAREHOUSE_VIEW"));

    options.AddPolicy("AttributeManage", policy =>
        policy.RequireClaim("permission", "PRODUCT_MANAGE", "ADMIN"));
    options.AddPolicy("AttributeView", policy =>
        policy.RequireClaim("permission", "PRODUCT_MANAGE", "ADMIN"));

});

builder.Services.AddAtlasInfrastructure(builder.Configuration);
builder.Services.AddAtlasApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

var folderName = builder.Configuration.GetSection("StorageSettings:FolderName").Value ?? "AtlasStorage";
var currentDir = builder.Environment.ContentRootPath;

// Ép buộc lùi ra thư mục cha (ra ngoài project Atlas.Web)
var parentDir = Directory.GetParent(currentDir)?.FullName ?? currentDir;
var absoluteStoragePath = Path.Combine(parentDir, folderName);

// Đảm bảo thư mục tồn tại để tránh crash ứng dụng khi khởi động
if (!Directory.Exists(absoluteStoragePath))
{
    Directory.CreateDirectory(absoluteStoragePath);
}

// Cho phép truy cập qua đường dẫn ảo "/file-storage"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(absoluteStoragePath),
    RequestPath = "/file-storage" 
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Code định tuyến Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "logpage",
    areaName: "LogPage",
    pattern: "LogPage/{action=Index}",
    defaults: new { controller = "Log" });

app.MapAreaControllerRoute(
    name: "account",
    areaName: "Account",
    pattern: "Account/{controller=Login}/{action=SignIn}/{id?}",
    defaults: new { controller = "Login" });

app.MapAreaControllerRoute(
    name: "products",
    areaName: "Products",
    pattern: "Products/{action=Index}/{id?}",
    defaults: new { controller = "Product" });

app.MapAreaControllerRoute(
    name: "hrm",
    areaName: "HRM",
    pattern: "HRM/{action=Index}/{id?}",
    defaults: new { controller = "HRM" });

app.MapAreaControllerRoute(
    name: "party",
    areaName: "Party",
    pattern: "Party/{action=Index}/{id?}",
    defaults: new { controller = "Party" });

app.MapControllerRoute(
    name: "setting",
    pattern: "Setting",
    defaults: new { controller = "Setting", action = "Settings" });

app.MapAreaControllerRoute(
    name: "category",
    areaName: "Category",
    pattern: "Category/{action=Index}/{id?}",
    defaults: new { controller = "Category" });

app.MapAreaControllerRoute(
    name: "purchase",
    areaName: "Purchase",
    pattern: "Purchase/{action=Index}/{id?}",
    defaults: new { controller = "PurchaseOrder" });

app.MapAreaControllerRoute(
    name: "sale",
    areaName: "Sale",
    pattern: "Sale/{action=Index}/{id?}",
    defaults: new { controller = "SaleOrder" });

app.MapAreaControllerRoute(
    name: "warehouse",
    areaName: "Warehouse",
    pattern: "Warehouse/{action=Index}/{id?}",
    defaults: new { controller = "Warehouse" });

app.MapAreaControllerRoute(
    name: "attributes",
    areaName: "Attributes",
    pattern: "Attributes/{action=Index}/{id?}",
    defaults: new { controller = "Attribute" });

app.MapGet("/", (HttpContext context) =>
    context.User.Identity?.IsAuthenticated == true
        ? Results.Redirect("/Index")
        : Results.Redirect("/Account/Login/SignIn"));

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
