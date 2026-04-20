USE master
GO

-- 1. Xóa Database nếu đã tồn tại để làm sạch môi trường
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'AtlasDB')
BEGIN
    ALTER DATABASE AtlasDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AtlasDB;
END
GO

-- 2. Tạo Database mới
CREATE DATABASE AtlasDB
GO

USE AtlasDB
GO

-- =============================================
-- 3. TẠO CÁC BẢNG (SCHEMA)
-- =============================================

-- General Information
CREATE TABLE dbo.Addresses(
    id int identity(1,1) primary key,
    AddressType nvarchar(50),
    Street nvarchar(255),
    City nvarchar(100),
    State nvarchar(100),
    Country nvarchar(100)
)
GO

CREATE TABLE dbo.Contacts(
	id int identity(1,1) primary key,
	Phone nvarchar(20),
	Email nvarchar(50) null
)
GO

CREATE TABLE dbo.Persons(
	id int identity(1,1) primary key,
	FirstName nvarchar(50) not null,
	LastName nvarchar(50) not null,
	DoB date not null,
    AddressId int not null,
    ContactId int not null,
	FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(id),
    FOREIGN KEY (ContactId) REFERENCES dbo.Contacts(id)
)
GO

-- Business Operations
CREATE TABLE dbo.Companies(
    id int identity(1,1) primary key,
    CompanyName nvarchar(100) not null,
    TaxId nvarchar(20) not null UNIQUE,
    AddressId int not null,
    ContactId int not null,
    FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(id),
    FOREIGN KEY (ContactId) REFERENCES dbo.Contacts(id)
)
GO

CREATE TABLE dbo.VendorsCompany(
    id int identity(1,1) primary key,
    CompanyId int not null UNIQUE,
    FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(id)
)
GO

CREATE TABLE dbo.VendorsPerson(
    id int identity(1,1) primary key,
    PersonId int not null UNIQUE,
    TaxId nvarchar(20) null,
    FOREIGN KEY (PersonId) REFERENCES dbo.Persons(id)
)
GO

CREATE TABLE dbo.CustomerCompany(
    id int identity(1,1) primary key,
    CompanyId int not null UNIQUE,
    FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(id)
)
GO

CREATE TABLE dbo.CustomerPerson(
    id int identity(1,1) primary key,
    PersonId int not null UNIQUE,
    TaxId nvarchar(20) null,
    FOREIGN KEY (PersonId) REFERENCES dbo.Persons(id)
)
GO

-- Employee Management
CREATE TABLE dbo.Employee(
    id int identity(1,1) primary key,
    EmployeeNumber nvarchar(20) not null UNIQUE,
    PersonId int not null UNIQUE,
    FOREIGN KEY (PersonId) REFERENCES dbo.Persons(id)
)
GO

CREATE TABLE dbo.EmployeeAccounts(
    EmployeeId int primary key,
    Username nvarchar(50) not null UNIQUE,
    PasswordHash nvarchar(255) not null,
    IsActive bit not null default 1,
    LastLogin datetime null,
    canProduct bit not null default 0,
    canSale bit not null default 0,
    canEmployee bit not null default 0,
    canInventory bit not null default 0,
    canAdministration bit not null default 0,
    canHR bit not null default 0,
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id)
)
GO

CREATE TABLE dbo.Departments(
    id int identity(1,1) primary key,
    DepartmentName nvarchar(100) not null UNIQUE,
    Description nvarchar(255) null,
    ParentDepartmentId int null,
    FOREIGN KEY (ParentDepartmentId) REFERENCES dbo.Departments(id)
)
GO

CREATE TABLE dbo.EmployeeDepartments(
    EmployeeId int not null,
    DepartmentId int not null,
    PRIMARY KEY (EmployeeId, DepartmentId),
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id),
    FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments(id)
)
GO

-- Logging
CREATE TABLE dbo.Logs(
    id int identity(1,1) primary key,
    EmployeeId int null,
    Action nvarchar(255) not null,
    Timestamp datetime not null default GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id)
)
GO

-- Inventory Management
CREATE TABLE dbo.Taxes(
    id int identity(1,1) primary key,
    TaxName nvarchar(50) not null UNIQUE,
    TaxRate decimal(5,2) not null,
    Description nvarchar(255) null,
    IsActive bit not null default 1,
    IsStackable bit not null default 0
)
GO

CREATE TABLE dbo.Products(
    id int identity(1,1) primary key,
    ProductName nvarchar(100) not null,
    ProductCode nvarchar(50) not null UNIQUE,
    ImageUrl nvarchar(255) null,
    SalePrice decimal(18,2) not null,
    CostPrice decimal(18,2) not null,
    Barcode nvarchar(50) null,
    isActive bit not null default 1,
    Onsale bit not null default 0,
    EmployeeId int not null,
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id)
)
GO

CREATE TABLE dbo.ProductTaxes(
    ProductId int not null,
    TaxId int not null,
    PRIMARY KEY (ProductId, TaxId),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id),
    FOREIGN KEY (TaxId) REFERENCES dbo.Taxes(id)
)
GO

CREATE TABLE dbo.ProductDetails(
    ProductId int primary key,
    ProductDescription nvarchar(255) null,
    Weight decimal(18,2) null,
    WarrantyPeriod int null,
    Dimensions nvarchar(50) null,
    Manufacturer nvarchar(100) null,
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id)
)
GO

CREATE TABLE dbo.Categories(
    id int identity(1,1) primary key,
    CategoryName nvarchar(100) not null UNIQUE,
    CategoryDesc nvarchar(255) null
)
GO

CREATE TABLE dbo.Pricelist(
    id int identity(1,1) primary key,
    EffectiveDate date not null,
    ExpiryDate date null,
    VendorCompanyId int null,
    VendorPersonId int null,
    FOREIGN KEY (VendorCompanyId) REFERENCES dbo.VendorsCompany(id),
    FOREIGN KEY (VendorPersonId) REFERENCES dbo.VendorsPerson(id),
    CONSTRAINT CHK_Pricelist_Vendor CHECK (
        (VendorCompanyId IS NOT NULL AND VendorPersonId IS NULL) 
        OR (VendorCompanyId IS NULL AND VendorPersonId IS NOT NULL)
    )
)
GO

CREATE TABLE dbo.CategoryPricelist(
    id int identity(1,1) primary key,
    CategoryId int not null,
    PricelistId int not null,
    FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(id),
    FOREIGN KEY (PricelistId) REFERENCES dbo.Pricelist(id)
)
GO

CREATE TABLE dbo.PricelistProduct(
    id int identity(1,1) primary key,
    IsStackable bit not null default 0,
    PricelistId int not null,
    ProductId int not null,
    Price decimal(18,2) null,
    Discount decimal(5,2) null,
    FOREIGN KEY (PricelistId) REFERENCES dbo.Pricelist(id),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id)
)
GO

CREATE TABLE dbo.CategoryProducts(
    CategoryId int not null,
    ProductId int not null,
    PRIMARY KEY (CategoryId, ProductId),
    FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(id),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id)
)
GO

CREATE TABLE dbo.Warehouses(
    id int identity(1,1) primary key,
    WarehouseName nvarchar(100) not null UNIQUE,
    AddressId int not null,
    ManagerId int null,
    FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(id),
    FOREIGN KEY (ManagerId) REFERENCES dbo.Employee(id)
)
GO

CREATE TABLE dbo.InventoryStock(
    WarehouseId int not null,
    ProductId int not null,
    Quantity int not null default 0,
    LastUpdated datetime not null default GETDATE(),
    PRIMARY KEY (WarehouseId, ProductId),
    FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(id),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id)
)
GO

-- Sales Management
CREATE TABLE dbo.SalesOrders(
    id int identity(1,1) primary key,
    OrderNumber nvarchar(50) not null UNIQUE, 
    OrderDate datetime not null default GETDATE(),
    EmployeeId int not null,
    CustomerCompanyId int null,
    CustomerPersonId int null,
    OrderStatus nvarchar(50) not null default 'Pending', 
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id),
    FOREIGN KEY (CustomerCompanyId) REFERENCES dbo.CustomerCompany(id),
    FOREIGN KEY (CustomerPersonId) REFERENCES dbo.CustomerPerson(id),
    CONSTRAINT CHK_SalesOrder_Customer CHECK (
        (CustomerCompanyId IS NOT NULL AND CustomerPersonId IS NULL) 
        OR (CustomerCompanyId IS NULL AND CustomerPersonId IS NOT NULL)
    )
)
GO

CREATE TABLE dbo.SalesOrderDetails(
    id int identity(1,1) primary key,
    OrderId int not null,
    ProductId int not null,
    WarehouseId int not null,
    Quantity int not null CHECK (Quantity > 0),
    UnitPrice decimal(18,2) not null, 
    Discount decimal(18,2) not null default 0,
    TaxRate decimal(5,2) not null default 0, 
    SubTotal AS ((Quantity * UnitPrice) - Discount), 
    TaxAmount AS (((Quantity * UnitPrice) - Discount) * (TaxRate / 100.0)),
    LineTotal AS (((Quantity * UnitPrice) - Discount) * (1 + TaxRate / 100.0)), 
    FOREIGN KEY (OrderId) REFERENCES dbo.SalesOrders(id),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id),
    FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(id)
)
GO

-- Purchase Management
CREATE TABLE dbo.PurchaseOrders(
    id int identity(1,1) primary key,
    PONumber nvarchar(50) not null UNIQUE, 
    OrderDate datetime not null default GETDATE(),
    EmployeeId int not null,
    VendorCompanyId int null,
    VendorPersonId int null,
    OrderStatus nvarchar(50) not null default 'Draft', 
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id),
    FOREIGN KEY (VendorCompanyId) REFERENCES dbo.VendorsCompany(id),
    FOREIGN KEY (VendorPersonId) REFERENCES dbo.VendorsPerson(id),
    CONSTRAINT CHK_PurchaseOrder_Vendor CHECK (
        (VendorCompanyId IS NOT NULL AND VendorPersonId IS NULL) 
        OR (VendorCompanyId IS NULL AND VendorPersonId IS NOT NULL)
    )
)
GO

CREATE TABLE dbo.PurchaseOrderDetails(
    id int identity(1,1) primary key,
    POId int not null,
    ProductId int not null,
    WarehouseId int not null,
    Quantity int not null CHECK (Quantity > 0),
    UnitPrice decimal(18,2) not null, 
    TaxRate decimal(5,2) not null default 0,
    SubTotal AS (Quantity * UnitPrice),
    TaxAmount AS ((Quantity * UnitPrice) * (TaxRate / 100.0)),
    LineTotal AS ((Quantity * UnitPrice) * (1 + TaxRate / 100.0)),
    FOREIGN KEY (POId) REFERENCES dbo.PurchaseOrders(id),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id),
    FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(id)
)
GO