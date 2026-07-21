# Atlas

A multi-tier ERP / inventory & order management web application built on **ASP.NET Core 10** with **Entity Framework Core 10** and **SQL Server**. Atlas covers products, parties (customers/suppliers), purchase orders, sales orders, warehouses, inventory transactions, billing, HRM, and role-based access control.

## Tech Stack

| Layer | Technology |
| --- | --- |
| Target framework | .NET 10 (`net10.0`) |
| Web / UI | ASP.NET Core MVC + Razor Pages, Areas-based structure |
| ORM | Entity Framework Core 10 (`SqlServer`, `Tools`) |
| Database | Microsoft SQL Server (default: `AtlasDB` on LocalDB) |
| AuthN | Cookie authentication (`CookieAuthenticationDefaults`) |
| AuthZ | Claim-based authorization policies (permission claims) |
| Architecture | Clean / layered (Core → Infrastructure → Services → Web) |

## Solution Structure

The solution lives in `Atlas.slnx` and is split into four projects, following a clean-layered architecture with dependency direction flowing inward:

```
Atlas
├── Atlas.Core            # Shared kernel: Entities, Enums, DTOs, Models, Interfaces
├── Atlas.Infrastructure  # EF Core DbContext, Repositories, StorageProvider; depends on Core
├── Atlas.Services         # Application/business services; depends on Core + Infrastructure
└── Atlas.Web             # MVC + Razor Pages host (Areas, controllers, views, wwwroot)
```

- **Atlas.Core** — No external dependencies (other than EF Core for entity attributes). Holds domain entities, enums, DTOs, and interface definitions.
- **Atlas.Infrastructure** — `AtlasDBContext`, repository implementations (one per aggregate), and `LocalStorageProvider` for file storage. Registered via `AddAtlasInfrastructure()`.
- **Atlas.Services** — Business services (`ProductService`, `PurchaseOrderService`, `SalesOrderService`, `PartyService`, `AttributeService`, `AuthService`, `LogService`, `DocumentNumberService`, etc.). Registered via `AddAtlasApplicationServices()`.
- **Atlas.Web** — The ASP.NET Core host. Composition root (`Program.cs`), Areas, controllers, views, and static assets.

### Feature Areas (Atlas.Web/Areas)

| Area | Purpose |
| --- | --- |
| `Account` | Login / sign-in, access denied, cookie auth |
| `Products` | Product, product details, variants, images |
| `Attributes` | Attribute types & values (product variant attributes) |
| `Category` | Product categories, category–product mappings |
| `Party` | Customers & suppliers (parties), addresses, contacts |
| `Purchase` | Purchase orders, purchase order bills |
| `Sale` | Sales orders, sales order bills, invoices, payments |
| `Warehouse` | Warehouses, inventory stock, inventory transactions |
| `HRM` | Employees, departments, employee accounts |
| `LogPage` | System activity logs |
| `Setting` | System settings |

## Domain Highlights (AtlasDBContext)

- **Identifiers & access:** `Roles`, `Permissions`, `RolePermissions`, `EmployeeAccounts`
- **People:** `Employees`, `Departments`, `EmployeeDepartments`, `Contacts`, `Addresses`
- **Catalog:** `Products`, `ProductDetails`, `ProductVariants`, `Units`, `Categories`, `CategoryProducts`, `Images`, `ProductImages`
- **Pricing & tax:** `Pricelists`, `PricelistProductVariant`, `CategoryPricelists`, `Taxes`, `ProductTaxes`
- **Attributes:** `AttributeTypes`, `AttributeValues`, `VariantAttributeMappings`
- **Purchasing:** `PurchaseOrders`, `PurchaseOrderDetails`, `PurchaseOrderBills`, `PurchaseOrderStatuses`
- **Selling:** `SalesOrders`, `SalesOrderDetails`, `SalesOrderBills`, `SalesOrderStatuses`, `Invoices`, `Payments`
- **Inventory:** `Warehouses`, `InventoryStocks`, `InventoryTransactions`
- **Parties:** `Parties` (customers/suppliers)
- **Auditing:** `Logs`

## Authentication & Authorization

- **Authentication:** Cookie-based with an 8-hour sliding expiration. The `OnValidatePrincipal` callback revalidates the user against the database (`IAuthRepository.IsActiveByUsernameAsync`) at most once every 15 minutes to avoid hitting the DB on every request, and signs out inactive users.
- **Authorization:** Claim-based policies keyed off a `permission` claim. Admin (`ADMIN`) satisfies every policy. Defined policies include:
  - `ProductManage` / `ProductView`
  - `HRManage` / `HRMView`
  - `PartyManage` / `PartyView`
  - `CategoryManage` / `CategoryView`
  - `AttributeManage` / `AttributeView`
  - `PurchaseManage` / `PurchaseView`
  - `SaleManage` / `SaleView`
  - `WarehouseManage` / `WarehouseView`
  - `Administration` (admin-only)

Unauthenticated users hitting `/` are redirected to `/Account/Login/SignIn`; authenticated users go to `/Index`.

## File Storage

User-uploaded files (e.g. product images) are served from a physical folder configured under `StorageSettings:FolderName` (default `AtlasStorage`). At startup, Atlas resolves this folder to the **parent of `Atlas.Web`** (the solution root) and exposes it at the virtual path `/file-storage` via `UseStaticFiles`. The directory is created on startup if missing.

## Database Setup

Atlas ships with raw SQL scripts at the solution root (development does not rely on EF migrations for schema):

- `SQLDB.sql` — drops/creates the `AtlasDB` database and all schema (tables, constraints).
- `FirstData.sql` — seeds lookup data, roles, permissions, and an initial admin account.
- `FixVietnameseData.sql` / `SelectDB.sql` — data-fix and helper query scripts.

### To initialize a fresh database

1. Configure the connection string in `Atlas.Web/appsettings.json` (`ConnectionStrings:DefaultConnection`). The default points to LocalDB:
   ```
   Server=(localdb)\MSSQLLocalDB;Database=AtlasDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
   ```
2. Execute `SQLDB.sql` to create the schema.
3. Execute `FirstData.sql` to seed initial roles, permissions, and reference data.

> The DbContext uses `AddDbContextPool` with `EnableRetryOnFailure()`, so the app resiliently reconnects to SQL Server on transient failures.

## Getting Started

Requirements:
- .NET SDK 10
- SQL Server (or LocalDB) accessible via the configured connection string

1. Restore and build:
   ```bash
   dotnet restore
   dotnet build
   ```
2. Set up the database (see **Database Setup** above).
3. Run the web project:
   ```bash
   dotnet run --project Atlas.Web
   ```
4. Navigate to the issued URL — you'll be redirected to the login page. Use the seeded admin account from `FirstData.sql`.

## Configuration (`Atlas.Web/appsettings.json`)

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AtlasDB;..."
  },
  "StorageSettings": { "BasePath": "AtlasStorage" },
  "Logging": { /* standard ASP.NET Core logging */ }
}
```

Environment-specific overrides live in `appsettings.Development.json`.

## Notes

- The solution uses the new `.slnx` solution format (see `Atlas.slnx`).
- `Areas/Log` and `Areas/Vendor` are excluded from compilation in `Atlas.Web.csproj`.
- Razor Pages is enabled alongside MVC (`AddRazorPages` / `MapRazorPages`).
