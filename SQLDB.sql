USE master
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'AtlasDB')
BEGIN
    ALTER DATABASE AtlasDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AtlasDB;
END
GO

CREATE DATABASE AtlasDB
GO

USE AtlasDB
GO

-- =============================================
-- 1. DANH MỤC HỆ THỐNG & LOOKUP TABLES
-- =============================================

CREATE TABLE dbo.Roles(
    id int identity(1,1) primary key,
    RoleName nvarchar(50) not null UNIQUE,
    Description nvarchar(255) null
)
GO

CREATE TABLE dbo.Permissions(
    id int identity(1,1) primary key,
    PermissionKey nvarchar(100) not null UNIQUE,
    Description nvarchar(255) null
)
GO

CREATE TABLE dbo.RolePermissions(
    RoleId int not null,
    PermissionId int not null,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES dbo.Roles(id),
    FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(id)
)
GO

CREATE TABLE dbo.SalesOrderStatuses(
    id int identity(1,1) primary key,
    StatusName nvarchar(50) not null UNIQUE,
    Description nvarchar(255) null
)
GO

CREATE TABLE dbo.PurchaseOrderStatuses(
    id int identity(1,1) primary key,
    StatusName nvarchar(50) not null UNIQUE,
    Description nvarchar(255) null
)
GO

CREATE TABLE dbo.Units(
    id int identity(1,1) primary key,
    UnitName nvarchar(50) not null UNIQUE, 
    ShortName nvarchar(10) null
)
GO

CREATE TABLE dbo.PaymentMethods(
    id int identity(1,1) primary key,
    MethodName nvarchar(50) not null UNIQUE
)
GO

CREATE TABLE dbo.PaymentStatuses(
    id int identity(1,1) primary key,
    StatusName nvarchar(50) not null UNIQUE
)
GO

CREATE TABLE dbo.TransactionTypes(
    id int identity(1,1) primary key,
    TypeName nvarchar(50) not null UNIQUE -- 'IN', 'OUT', 'TRANSFER', 'ADJUST'
)
GO

CREATE TABLE dbo.Currencies (
    id int identity(1,1) primary key,
    CurrencyCode nvarchar(3) not null UNIQUE, -- USD, VND, EUR
    CurrencyName nvarchar(50) not null,
    ExchangeRate decimal(18,6) not null default 1.0, 
    IsBaseCurrency bit default 0,
    LastUpdated datetime default GETDATE()
)
GO

CREATE TABLE dbo.Images (
    id int identity(1,1) primary key,
    ImageUrl nvarchar(255) not null,
    CreatedAt datetime default GETDATE(),
)

-- =============================================
-- 1.1 Basic Information
-- =============================================

CREATE TABLE dbo.MyCompanyInfo(
    id int identity(1,1) primary key,
    CompanyName nvarchar(100) not null,
    TaxId nvarchar(20) null,
    Address nvarchar(255) null,
    Phone nvarchar(20) null,
    Email nvarchar(50) null,
    LogoId int null,
    FOREIGN KEY (LogoId) REFERENCES dbo.Images(id)
)


-- =============================================
-- 2. UNIFIED PARTY MODEL (QUẢN LÝ ĐỐI TÁC TINH GỌN)
-- =============================================

CREATE TABLE dbo.Addresses(
    id int identity(1,1) primary key,
    AddressType nvarchar(50),
    Street nvarchar(255),
    City nvarchar(100),
    State nvarchar(100),
    Country nvarchar(100),
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE()
)
GO

CREATE TABLE dbo.Contacts(
    id int identity(1,1) primary key,
    Phone nvarchar(20),
    Email nvarchar(50) null,
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE()
)
GO

-- Chuẩn hóa gộp Person và Company thành một thực thể Party duy nhất
CREATE TABLE dbo.Parties(
    id int identity(1,1) primary key,
    PartyType nvarchar(20) not null CHECK (PartyType IN ('Person', 'Company')),
    DisplayName nvarchar(200) not null,
    FirstName nvarchar(50) null,         -- Dùng nếu là Person
    LastName nvarchar(50) null,          -- Dùng nếu là Person
    DoB date null,                       -- Dùng nếu là Person
    TaxId nvarchar(20) null,      -- Dùng chung cho Company hoặc Cá nhân kinh doanh
    AddressId int not null,
    ContactId int not null,
    IsCustomer bit not null default 0,   -- Đóng vai trò Khách hàng
    IsVendor bit not null default 0,     -- Đóng vai trò Nhà cung cấp
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE(),
    ImageID int null,
    FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(id),
    FOREIGN KEY (ContactId) REFERENCES dbo.Contacts(id),
    FOREIGN KEY (ImageID) REFERENCES dbo.Images(id)
)
GO

-- =============================================
-- 3. QUẢN LÝ NHÂN VIÊN & TÀI KHOẢN
-- =============================================

CREATE TABLE dbo.Employee(
    id int identity(1,1) primary key,
    EmployeeNumber nvarchar(20) not null UNIQUE,
    FullName nvarchar(100) not null,
    DoB date not null,
    AddressId int not null,
    ContactId int not null,
    ImageID int null,
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE(),
    FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(id),
    FOREIGN KEY (ContactId) REFERENCES dbo.Contacts(id),
    FOREIGN KEY (ImageID) REFERENCES dbo.Images(id)
)
GO

CREATE TABLE dbo.EmployeeAccounts(
    EmployeeId int primary key,
    Username nvarchar(50) not null UNIQUE,
    PasswordHash varchar(255) not null,
    IsActive bit not null default 1,
    LastLogin datetime null,
    RoleId int null,
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id),
    FOREIGN KEY (RoleId) REFERENCES dbo.Roles(id)
)
GO

CREATE TABLE dbo.Departments(
    id int identity(1,1) primary key,
    DepartmentName nvarchar(100) not null UNIQUE,
    Description nvarchar(255) null,
    ParentDepartmentId int null,
    FOREIGN KEY (ParentDepartmentId) REFERENCES dbo.Departments(id),
    CreatedAt datetime default GETDATE()
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

CREATE TABLE dbo.Logs(
    id bigint identity(1,1) primary key,
    EmployeeId int null,
    Action nvarchar(255) not null,
    Timestamp datetime not null default GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id)
)
GO

CREATE TABLE dbo.LogsDetails(
    LogId bigint not null,
    JsonChangeUrl nvarchar(255) not null, -- Lưu chi tiết thay đổi ở dạng JSON
    PRIMARY KEY (LogId),
    FOREIGN KEY (LogId) REFERENCES dbo.Logs(id)
)
GO

-- =============================================
-- 4. QUẢN LÝ SẢN PHẨM & BIẾN THỂ (PRODUCT SKU)
-- =============================================

CREATE TABLE dbo.Taxes(
    id int identity(1,1) primary key,
    TaxName nvarchar(50) not null UNIQUE,
    TaxRate decimal(18,4) not null, -- Giữ nguyên rate chính xác
    Description nvarchar(255) null,
    IsActive bit not null default 1,
    IsStackable bit not null default 0,
    EffectiveDate datetime null, 
    ExpiryDate datetime null
)
GO

CREATE TABLE dbo.Products(
    id int identity(1,1) primary key,
    ProductName nvarchar(100) not null,
    ProductCode nvarchar(50) not null UNIQUE,
    UnitId int null, 
    BaseSalePrice decimal(19,4) not null, -- Chuẩn hóa kiểu tiền tệ kế toán
    BaseCostPrice decimal(19,4) not null, 
    Barcode nvarchar(50) null,
    IsActive bit not null default 1,
    OnSale bit not null default 0,
    EmployeeId int not null,
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE(),
    UpdatedAt datetime null,
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id),
    FOREIGN KEY (UnitId) REFERENCES dbo.Units(id)
)
GO

CREATE TABLE dbo.ProductImages(

    ProductId int not null,
    ImageId int not null,
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id),
    FOREIGN KEY (ImageId) REFERENCES dbo.Images(id)
)

CREATE TABLE dbo.AttributeTypes(
    id int identity(1,1) primary key,
    AttributeName nvarchar(50) not null UNIQUE,
    Description nvarchar(255) null
)
GO

CREATE TABLE dbo.AttributeValues(
    id int identity(1,1) primary key,
    AttributeTypeId int not null,
    AttributeValue nvarchar(50) not null,
    FOREIGN KEY (AttributeTypeId) REFERENCES dbo.AttributeTypes(id),
    CONSTRAINT UC_AttributeValue UNIQUE (AttributeTypeId, AttributeValue)
)
GO

CREATE TABLE dbo.ProductVariants(
    id int identity(1,1) primary key,
    ProductId int not null,
    SKU nvarchar(50) not null UNIQUE,
    VariantPrice decimal(19,4) null,
    VariantCost decimal(19,4) null,
    IsActive bit default 1,
    CreatedAt datetime default GETDATE(),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id)
)
GO

CREATE TABLE dbo.VariantAttributeMappings(
    VariantId int not null,
    AttributeValueId int not null,
    PRIMARY KEY (VariantId, AttributeValueId),
    FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(id),
    FOREIGN KEY (AttributeValueId) REFERENCES dbo.AttributeValues(id)
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
    ProductDescription nvarchar(max) null,
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

CREATE TABLE dbo.CategoryProducts(
    CategoryId int not null,
    ProductId int not null,
    PRIMARY KEY (CategoryId, ProductId),
    FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(id),
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(id)
)
GO

-- =============================================
-- 5. QUẢN LÝ BẢNG GIÁ (PRICELIST)
-- =============================================

CREATE TABLE dbo.Pricelist(
    id int identity(1,1) primary key,
    PricelistName nvarchar(100) not null,
    EffectiveDate date not null,
    ExpiryDate date null,
    VendorId int null, -- Trỏ thẳng về Parties có IsVendor = 1
    CurrencyId int not null default 1, -- Đồng bộ tiền tệ áp dụng
    FOREIGN KEY (VendorId) REFERENCES dbo.Parties(id),
    FOREIGN KEY (CurrencyId) REFERENCES dbo.Currencies(id)
)
GO

CREATE TABLE dbo.PricelistProductVariant(
    id int identity(1,1) primary key,
    PricelistId int not null,
    VariantId int not null,
    Price decimal(19,4) null,
    Discount decimal(5,2) null,
    FOREIGN KEY (PricelistId) REFERENCES dbo.Pricelist(id),
    FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(id)
)
GO

-- =============================================
-- 6. QUẢN LÝ KHO (INVENTORY)
-- =============================================

CREATE TABLE dbo.Warehouses(
    id int identity(1,1) primary key,
    WarehouseName nvarchar(100) not null UNIQUE,
    AddressId int not null,
    ManagerId int null,
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE(),
    FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(id),
    FOREIGN KEY (ManagerId) REFERENCES dbo.Employee(id)
)
GO

CREATE TABLE dbo.InventoryStock(
    WarehouseId int not null,
    VariantId int not null,
    Quantity int not null default 0,
    ReservedQuantity int not null default 0,
    LastUpdated datetime not null default GETDATE(),
    PRIMARY KEY (WarehouseId, VariantId),
    FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(id),
    FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(id)
)
GO

CREATE TABLE dbo.InventoryTransactions(
    id bigint identity(1,1) primary key,
    VariantId int not null,
    WarehouseId int not null,
    Quantity int not null,
    TransactionTypeId int not null, -- Đã chuẩn hóa qua Lookup Table
    ReferenceId nvarchar(50),
    EmployeeId int not null,
    TransactionDate datetime default GETDATE(),
    Note nvarchar(255),
    FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(id),
    FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(id),
    FOREIGN KEY (TransactionTypeId) REFERENCES dbo.TransactionTypes(id),
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id)
)
GO

-- =============================================
-- 7. QUẢN LÝ BÁN HÀNG (SALES)
-- =============================================

CREATE TABLE dbo.SalesOrders(
    id int identity(1,1) primary key,
    OrderNumber nvarchar(50) not null UNIQUE, 
    OrderDate datetime not null default GETDATE(),
    EmployeeId int not null,
    CustomerId int not null, -- Chỉ cần một cột trỏ tới Parties (IsCustomer = 1)
    OrderStatusId int not null default 1,
    CurrencyId int not null default 1, -- Tích hợp tiền tệ đơn hàng
    ExchangeRate decimal(18,6) not null default 1.0, -- Ghi nhận tỷ giá tại thời điểm tạo đơn
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id),
    FOREIGN KEY (CustomerId) REFERENCES dbo.Parties(id),
    FOREIGN KEY (OrderStatusId) REFERENCES dbo.SalesOrderStatuses(id),
    FOREIGN KEY (CurrencyId) REFERENCES dbo.Currencies(id)
)
GO

CREATE TABLE dbo.SalesOrderDetails(
    id int identity(1,1) primary key,
    OrderId int not null,
    VariantId int not null, 
    WarehouseId int not null,
    Quantity int not null CHECK (Quantity > 0),
    UnitPrice decimal(19,4) not null, -- Đơn giá chuẩn tài chính
    Discount decimal(19,4) not null default 0,
    -- Loại bỏ TaxId độc lập để tránh xung đột dữ liệu với bảng mapping đa thuế dưới
    TaxAmount decimal(19,4) not null default 0, -- Lưu trữ tổng tiền thuế sau khi tính toán các loại thuế áp dụng
    SubTotal AS ((Quantity * UnitPrice) - Discount), 
    LineTotal AS (((Quantity * UnitPrice) - Discount) + TaxAmount), 
    FOREIGN KEY (OrderId) REFERENCES dbo.SalesOrders(id),
    FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(id),
    FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(id)
)
GO

-- Quản lý Thuế chồng (Nhiều loại thuế áp dụng cho 1 dòng hóa đơn)
CREATE TABLE dbo.SalesOrderDetailTaxes (
    OrderDetailId int not null,
    TaxId int not null,
    PRIMARY KEY (OrderDetailId, TaxId),
    FOREIGN KEY (OrderDetailId) REFERENCES dbo.SalesOrderDetails(id),
    FOREIGN KEY (TaxId) REFERENCES dbo.Taxes(id)
)
GO

CREATE TABLE dbo.SalesOrderPayments(
    id int identity(1,1) primary key,
    OrderId int not null,
    PaymentDate datetime not null default GETDATE(),
    Amount decimal(19,4) not null,
    PaymentMethodId int not null, -- Đã chuẩn hóa qua Lookup Table
    Note nvarchar(255),
    PaymentStatusId int not null default 1, -- Đã chuẩn hóa qua Lookup Table
    FOREIGN KEY (OrderId) REFERENCES dbo.SalesOrders(id),
    FOREIGN KEY (PaymentMethodId) REFERENCES dbo.PaymentMethods(id),
    FOREIGN KEY (PaymentStatusId) REFERENCES dbo.PaymentStatuses(id)
)
GO

CREATE TABLE dbo.SalesOrderBills(
    id int identity(1,1) primary key,
    OrderId int not null,
    BillUrl nvarchar(255) not null,
    CreatedAt datetime default GETDATE(),
    FOREIGN KEY (OrderId) REFERENCES dbo.SalesOrders(id)
)
GO

CREATE TABLE dbo.Invoices(
    id int identity(1,1) primary key,
    InvoiceNumber nvarchar(50) not null UNIQUE,
    OrderId int not null,
    InvoiceDate datetime not null default GETDATE(),
    DueDate date null,
    TotalAmount decimal(19,4) not null,
    IsPaid bit default 0,
    CreatedAt datetime default GETDATE(),
    FOREIGN KEY (OrderId) REFERENCES dbo.SalesOrders(id)
)
GO

-- =============================================
-- 8. QUẢN LÝ NHẬP HÀNG (PURCHASE)
-- =============================================

CREATE TABLE dbo.PurchaseOrders(
    id int identity(1,1) primary key,
    PONumber nvarchar(50) not null UNIQUE, 
    OrderDate datetime not null default GETDATE(),
    EmployeeId int not null,
    VendorId int not null, -- Trỏ về Parties (IsVendor = 1)
    OrderStatusId int not null default 1,
    CurrencyId int not null default 1,
    ExchangeRate decimal(18,6) not null default 1.0,
    IsDeleted bit default 0,
    CreatedAt datetime default GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee(id),
    FOREIGN KEY (VendorId) REFERENCES dbo.Parties(id),
    FOREIGN KEY (OrderStatusId) REFERENCES dbo.PurchaseOrderStatuses(id),
    FOREIGN KEY (CurrencyId) REFERENCES dbo.Currencies(id)
)
GO

CREATE TABLE dbo.PurchaseOrderDetails(
    id int identity(1,1) primary key,
    POId int not null,
    VariantId int not null, 
    WarehouseId int not null,
    Quantity int not null CHECK (Quantity > 0),
    UnitPrice decimal(19,4) not null, 
    Discount decimal(19,4) not null default 0,
    TaxAmount decimal(19,4) not null default 0,
    SubTotal AS ((Quantity * UnitPrice) - Discount),
    LineTotal AS (((Quantity * UnitPrice) - Discount) + TaxAmount),
    BillUrl nvarchar(255) null,
    FOREIGN KEY (POId) REFERENCES dbo.PurchaseOrders(id),
    FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(id),
    FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(id)
)
GO

-- ĐÃ SỬA LỖI: Sửa từ sao chép nhầm Sales sang bảng chuẩn của Purchase
CREATE TABLE dbo.PurchaseOrderDetailTaxes (
    OrderDetailId int not null,
    TaxId int not null,
    PRIMARY KEY (OrderDetailId, TaxId),
    FOREIGN KEY (OrderDetailId) REFERENCES dbo.PurchaseOrderDetails(id),
    FOREIGN KEY (TaxId) REFERENCES dbo.Taxes(id)
)
GO

CREATE TABLE dbo.PurchaseOrderPayments(
    id int identity(1,1) primary key,
    OrderId int not null,
    PaymentDate datetime not null default GETDATE(),
    Amount decimal(19,4) not null,
    PaymentMethodId int not null,
    Note nvarchar(255),
    PaymentStatusId int not null default 1,
    FOREIGN KEY (OrderId) REFERENCES dbo.PurchaseOrders(id),
    FOREIGN KEY (PaymentMethodId) REFERENCES dbo.PaymentMethods(id),
    FOREIGN KEY (PaymentStatusId) REFERENCES dbo.PaymentStatuses(id)
)
GO

-- =============================================
-- 9. TỐI ƯU HÓA CHỈ MỤC (FOREIGN KEY INDEXING)
-- =============================================

-- Các Index tìm kiếm nghiệp vụ
CREATE INDEX IX_Products_ProductName ON dbo.Products(ProductName);
CREATE INDEX IX_ProductVariants_SKU ON dbo.ProductVariants(SKU);
CREATE INDEX IX_SalesOrders_OrderNumber ON dbo.SalesOrders(OrderNumber);
CREATE INDEX IX_PurchaseOrders_PONumber ON dbo.PurchaseOrders(PONumber);
CREATE INDEX IX_Inventory_VariantWarehouse ON dbo.InventoryStock(VariantId, WarehouseId);

-- Bổ sung Index cho các Khóa ngoại thường xuyên JOIN dữ liệu lớn
CREATE INDEX IX_FK_ProductVariants_ProductId ON dbo.ProductVariants(ProductId);
CREATE INDEX IX_FK_SalesOrderDetails_OrderId ON dbo.SalesOrderDetails(OrderId);
CREATE INDEX IX_FK_SalesOrderDetails_VariantId ON dbo.SalesOrderDetails(VariantId);
CREATE INDEX IX_FK_SalesOrderDetails_WarehouseId ON dbo.SalesOrderDetails(WarehouseId);
CREATE INDEX IX_FK_PurchaseOrderDetails_POId ON dbo.PurchaseOrderDetails(POId);
CREATE INDEX IX_FK_PurchaseOrderDetails_VariantId ON dbo.PurchaseOrderDetails(VariantId);
CREATE INDEX IX_FK_InventoryTransactions_VariantId ON dbo.InventoryTransactions(VariantId);
CREATE INDEX IX_FK_Parties_AddressId ON dbo.Parties(AddressId);
CREATE INDEX IX_FK_Parties_ContactId ON dbo.Parties(ContactId);
GO