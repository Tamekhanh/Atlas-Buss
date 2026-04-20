using Atlas.Core.Interfaces;
using Atlas.Infrastructure;
using Atlas.Infrastructure.Repositories;
using Atlas.Services.Auth;
using Atlas.Services.Customer;
using Atlas.Services.HRM;
using Atlas.Services.Inventory;
using Atlas.Services.Vendor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AtlasDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IVendorCompanyRepository, VendorCompanyRepository>();
builder.Services.AddScoped<IVendorCompanyService, VendorCompanyService>();
builder.Services.AddScoped<ICustomerCompanyRepository, CustomerCompanyRepository>();
builder.Services.AddScoped<ICustomerCompanyService, CustomerCompanyService>();
builder.Services.AddScoped<IVendorPersonRepository, VendorPersonRepository>();
builder.Services.AddScoped<IVendorPersonService, VendorPersonService>();
builder.Services.AddScoped<ICustomerPersonRepository, CustomerPersonRepository>();
builder.Services.AddScoped<ICustomerPersonService, CustomerPersonService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

// Code định tuyến Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

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
    name: "vendor",
    areaName: "Vendor",
    pattern: "Vendor/{action=Index}/{id?}",
    defaults: new { controller = "Vendor" });

app.MapAreaControllerRoute(
    name: "customer",
    areaName: "Customer",
    pattern: "Customer/{action=Index}/{id?}",
    defaults: new { controller = "Customer" });

app.MapGet("/", (HttpContext context) =>
    context.User.Identity?.IsAuthenticated == true
        ? Results.Redirect("/Index")
        : Results.Redirect("/Account/Login/SignIn"));

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
