using Atlas.Core.Interfaces;
using Atlas.Infrastructure;
using Atlas.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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

                var authRepository = context.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();
                var account = await authRepository.GetByUsernameAsync(username);

                if (account is null || !account.IsActive)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        };
    });

builder.Services.AddAuthorization();

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
    name: "vendor",
    areaName: "Vendor",
    pattern: "Vendor/{action=Index}/{id?}",
    defaults: new { controller = "Vendor" });

app.MapAreaControllerRoute(
    name: "customer",
    areaName: "Customer",
    pattern: "Customer/{action=Index}/{id?}",
    defaults: new { controller = "Customer" });

app.MapControllerRoute(
    name: "setting",
    pattern: "Setting",
    defaults: new { controller = "Setting", action = "Settings" });

app.MapGet("/", (HttpContext context) =>
    context.User.Identity?.IsAuthenticated == true
        ? Results.Redirect("/Index")
        : Results.Redirect("/Account/Login/SignIn"));

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
