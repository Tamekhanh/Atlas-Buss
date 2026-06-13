USE AtlasDB
GO

-- =============================================
-- 1. SEED LOOKUP TABLES (DANH MỤC)
-- =============================================

-- Seed Roles
INSERT INTO dbo.Roles (RoleName, Description) VALUES 
('Administrator', 'Full system access'),
('Sales Staff', 'Access to sales and inventory'),
('HR Manager', 'Access to personnel and administration'),
('Product Manager', 'Can create and manage products');
GO

-- Seed Permissions
INSERT INTO dbo.Permissions (PermissionKey, Description) VALUES 
('PRODUCT_MANAGE', 'Manage products and categories'),
('SALE_CREATE', 'Create and edit sales orders'),
('PURCHASE_CREATE', 'Create and edit purchase orders'),
('EMPLOYEE_MANAGE', 'Manage employee records'),
('INVENTORY_MANAGE', 'Manage warehouse and stock'),
('ADMIN_ALL', 'Full administrative access'),
('HR_MANAGE', 'Manage HR and departments'),
('PRODUCT_CREATE', 'Create products');
GO

-- Seed RolePermissions
INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES 
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8), -- Admin
(2, 1), (2, 2), (2, 4),                         -- Sales
(3, 1), (3, 3), (3, 5), (3, 6),                 -- HR
(4, 7);                                         -- Product Manager
GO

-- Seed Sales Order Statuses
INSERT INTO dbo.SalesOrderStatuses (StatusName, Description) VALUES 
('Pending', 'Order has been created but not yet processed'),
('Confirmed', 'Order has been verified'),
('Shipped', 'Order is on its way to customer'),
('Completed', 'Order delivered and paid'),
('Cancelled', 'Order has been cancelled');
GO

-- Seed Purchase Order Statuses
INSERT INTO dbo.PurchaseOrderStatuses (StatusName, Description) VALUES 
('Draft', 'Purchase order is being prepared'),
('Ordered', 'PO has been sent to vendor'),
('Received', 'Goods have been received in warehouse'),
('Cancelled', 'PO has been cancelled');
GO

-- Đơn vị tính
IF NOT EXISTS (SELECT 1 FROM dbo.Units WHERE UnitName = N'Chiếc')
	INSERT INTO dbo.Units (UnitName, ShortName) VALUES (N'Chiếc', N'Cái');
IF NOT EXISTS (SELECT 1 FROM dbo.Units WHERE UnitName = N'Bộ')
	INSERT INTO dbo.Units (UnitName, ShortName) VALUES (N'Bộ', N'Bộ');
IF NOT EXISTS (SELECT 1 FROM dbo.Units WHERE UnitName = N'Kilogram')
	INSERT INTO dbo.Units (UnitName, ShortName) VALUES (N'Kilogram', N'Kg');
GO

-- =============================================
-- 2. THÔNG TIN ĐỐI TÁC (PARTY MODEL)
-- =============================================
INSERT INTO dbo.Addresses (AddressType, Street, City, State, Country) VALUES 
('Office', '123 Le Loi Street', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
('Residential', '456 Nguyen Hue Street', 'Hoan Kiem', 'Hanoi', 'Vietnam'),
('Warehouse', '789 Tan Binh Industrial Zone', 'Tan Phu', 'Ho Chi Minh City', 'Vietnam'),
('Office', '101 Wall Street', 'New York', 'NY', 'USA'),
('Office', '222 Nguyen Van Linh Street', 'District 7', 'Ho Chi Minh City', 'Vietnam');
GO

INSERT INTO dbo.Contacts (Phone, Email) VALUES 
('0901234567', 'nguyenvana@gmail.com'),
('0907654321', 'tranthib@gmail.com'),
('0912334455', 'levanc@gmail.com'),
('0988887777', 'contact@globalcorp.com'),
('0922333444', 'product.manager@atlas.com');
GO

INSERT INTO dbo.Persons (FirstName, LastName, DoB, AddressId, ContactId) VALUES 
('Anh', 'Nguyen', '1990-01-15', 1, 1), 
('Binh', 'Tran', '1992-05-20', 2, 2),   
('Cuong', 'Le', '1985-11-10', 2, 3),     
('John', 'Doe', '1980-03-25', 4, 4),
('Minh', 'Pham', '1993-08-12', 5, 5);    
GO

INSERT INTO dbo.Companies (CompanyName, TaxId, AddressId, ContactId) VALUES 
('Atlas Technology Company', 'TAX123456', 1, 1),
('Global Trade Corp', 'TAX999888', 4, 4),
('Component Distribution X', 'TAX777666', 3, 4);
GO

INSERT INTO dbo.VendorsCompany (CompanyId) VALUES (2), (3);
INSERT INTO dbo.VendorsPerson (PersonId, TaxId) VALUES (4, '08028400');
INSERT INTO dbo.CustomerCompany (CompanyId) VALUES (2);
INSERT INTO dbo.CustomerPerson (PersonId, TaxId) VALUES (3, '08028200');
GO

-- =============================================
-- 3. NHÂN VIÊN & TÀI KHOẢN
-- =============================================
INSERT INTO dbo.Employee (EmployeeNumber, PersonId) VALUES 
('EMP001', 1), ('EMP002', 2), ('EMP003', 3), ('EMP004', 5);
GO

INSERT INTO dbo.EmployeeAccounts (EmployeeId, Username, PasswordHash, RoleId) VALUES 
(1, 'admin_atlas', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 1),
(2, 'sales_staff', 'hash_2', 2),
(3, 'hr_admin', 'hash_3', 3),
(4, 'product_manager', 'hash_4', 4);
GO

INSERT INTO dbo.Departments (DepartmentName, Description, ParentDepartmentId) VALUES 
('Executive Management', 'Senior management', NULL),
('Sales Department', 'Sales and business operations', 1),
('Warehouse and Logistics', 'Warehouse and inventory management', 1),
('Human Resources', 'Personnel management', 1);
GO

INSERT INTO dbo.EmployeeDepartments (EmployeeId, DepartmentId) VALUES 
(1, 1), (1, 4), (2, 2);         
GO

-- =============================================
-- 4. SẢN PHẨM, THUỘC TÍNH & BIẾN THỂ (Cập nhật mới)
-- =============================================

-- 4.1. Thuế
INSERT INTO dbo.Taxes (TaxName, TaxRate, Description) VALUES 
('VAT 10%', 10.00, 'Standard value-added tax'),
('Special Tax 5%', 5.00, 'Luxury goods tax');
GO

-- 4.2. Thuộc tính động (Dynamic Attributes)
INSERT INTO dbo.AttributeTypes (AttributeName, Description) VALUES 
('Color', 'Product color'),
('Size', 'Product size'),
('RAM', 'Memory capacity'),
('Storage', 'Disk capacity');
GO

INSERT INTO dbo.AttributeValues (AttributeTypeId, AttributeValue) VALUES 
(1, 'Space Gray'), (1, 'Silver'), (1, 'Midnight'), -- Color
(2, 'S'), (2, 'M'), (2, 'L'), (2, 'XL'),           -- Size
(3, '8GB'), (3, '16GB'), (3, '32GB'),               -- RAM
(4, '256GB'), (4, '512GB'), (4, '1TB');             -- Storage
GO

-- 4.3. Sản phẩm cha (Parent Products)
-- Cập nhật cột SalePrice -> BaseSalePrice, CostPrice -> BaseCostPrice
INSERT INTO dbo.Products (ProductName, ProductCode, UnitId, BaseSalePrice, BaseCostPrice, EmployeeId) 
VALUES 
('MacBook Pro 14', 'LAP-MAC-01', 1, 45000000, 40000000, 1),
('MacBook Air M2', 'LAP-MAC-02', 1, 28000000, 24000000, 1),
('ThinkPad X1 Carbon', 'LAP-LEN-01', 1, 35000000, 30000000, 1),
('Asus ROG Zephyrus G14', 'LAP-ASU-01', 1, 40000000, 35000000, 1),
('Razer Blade 15', 'LAP-RAZ-01', 1, 50000000, 45000000, 1),
('HP Spectre x360', 'LAP-HP-01', 1, 32000000, 27000000, 1),
('Acer Swift 3', 'LAP-ACE-01', 1, 18000000, 15000000, 1),
('LG Gram 16', 'LAP-LG-01', 1, 30000000, 25000000, 1),
('Surface Pro 9', 'LAP-MIC-01', 1, 29000000, 25000000, 1),
('Alienware m15 R7', 'LAP-DEL-02', 1, 55000000, 48000000, 1),
('iPad Pro 12.9', 'TAB-APP-01', 1, 25000000, 22000000, 1),
('Galaxy Tab S8 Ultra', 'TAB-SAM-01', 1, 22000000, 19000000, 1),
('AirPods Pro 2', 'AUD-APP-01', 1, 6000000, 4500000, 1),
('Sony WH-1000XM5', 'AUD-SON-01', 1, 8000000, 6500000, 1),
('Bose QuietComfort 45', 'AUD-BOS-01', 1, 7500000, 6000000, 1),
('Samsung Odyssey G7', 'MON-SAM-01', 1, 15000000, 12000000, 1),
('Dell UltraSharp 27', 'MON-DEL-01', 1, 12000000, 9500000, 1),
('LG UltraGear 27', 'MON-LG-02', 1, 10000000, 8000000, 1),
('Asus ProArt 27', 'MON-ASU-01', 1, 14000000, 11000000, 1),
('Apple Studio Display', 'MON-APP-01', 1, 40000000, 35000000, 1),
('Logitech G Pro X Superlight', 'MOU-LOG-02', 1, 3000000, 2200000, 1),
('Razer DeathAdder V3', 'MOU-RAZ-01', 1, 2800000, 2000000, 1),
('Corsair K70 RGB', 'KBD-COR-01', 1, 3500000, 2800000, 1),
('SteelSeries Apex Pro', 'KBD-STE-01', 1, 4500000, 3500000, 1),
('Akko 3098B', 'KBD-AKK-01', 1, 2000000, 1500000, 1),
('Wacom Intuos Pro', 'DRW-WAC-01', 1, 8000000, 6000000, 1),
('Elgato Stream Deck MK.2', 'ACC-ELG-01', 1, 4000000, 3000000, 1),
('Blue Yeti USB Microphone', 'MIC-BLU-01', 1, 3000000, 2200000, 1),
('Shure SM7B', 'MIC-SHU-01', 1, 10000000, 8500000, 1),
('Secretlab Titan Evo', 'CHR-SEC-01', 1, 12000000, 9000000, 1);
GO

-- 4.4. Biến thể sản phẩm (Product Variants)
-- Tạo biến thể cho một số sản phẩm mẫu (Laptops)
INSERT INTO dbo.ProductVariants (ProductId, SKU, VariantPrice, VariantCost) VALUES 
(1, 'MAC-PRO-14-SGR-16-512', 45000000, 40000000), -- MacBook Pro: Space Gray, 16GB, 512GB
(1, 'MAC-PRO-14-SLV-16-512', 45000000, 40000000), -- MacBook Pro: Silver, 16GB, 512GB
(2, 'MAC-AIR-M2-MID-8-256', 28000000, 24000000),  -- MacBook Air: Midnight, 8GB, 256GB
(2, 'MAC-AIR-M2-SLV-16-512', 32000000, 28000000), -- MacBook Air: Silver, 16GB, 512GB (Giá cao hơn)
(3, 'TPI-X1-BLK-16-512', 35000000, 30000000),    -- ThinkPad: Black, 16GB, 512GB
(13, 'APP-PODS-PRO-2', 6000000, 4500000),         -- AirPods (1 variant)
(30, 'SEC-TITAN-BLK-L', 12000000, 9000000),       -- Secretlab Chair: Black, L
(30, 'SEC-TITAN-WHT-L', 13000000, 9500000);       -- Secretlab Chair: White, L
GO

-- 4.5. Ánh xạ Biến thể với Thuộc tính
INSERT INTO dbo.VariantAttributeMappings (VariantId, AttributeValueId) VALUES 
(1, 1), (1, 7), (1, 10), -- MAC PRO 14: Space Gray, 16GB, 512GB
(2, 2), (2, 7), (2, 10), -- MAC PRO 14: Silver, 16GB, 512GB
(3, 3), (3, 7), (3, 9),  -- MAC AIR: Midnight, 8GB, 256GB
(4, 2), (4, 8), (4, 10), -- MAC AIR: Silver, 16GB, 512GB
(5, 1), (5, 8), (5, 10), -- THINKPAD: Black, 16GB, 512GB
(7, 1), (7, 6),             -- Chair: Black, L
(8, 2), (8, 6);             -- Chair: White, L
GO

-- 4.6. Chi tiết sản phẩm
INSERT INTO dbo.ProductDetails (ProductId, ProductDescription, Weight, WarrantyPeriod, Dimensions, Manufacturer) 
VALUES 
(1, 'Apple M2 Pro chip, 16GB RAM, 512GB SSD', 1.6, 12, '31x22x1.5cm', 'Apple'),
(2, 'Ultra-thin laptop with M2 chip', 1.2, 12, '30x21x1.1cm', 'Apple'),
(3, 'Premium business ultrabook', 1.1, 36, '31x22x1.4cm', 'Lenovo'),
(30, 'Ergonomic premium gaming chair', 34.0, 60, '85x70x37cm', 'Secretlab');
GO

INSERT INTO dbo.Categories (CategoryName, CategoryDesc) VALUES 
('Laptops', 'Laptop computers'),
('Accessories', 'Mouse, keyboards, headsets');
GO

INSERT INTO dbo.CategoryProducts (CategoryId, ProductId) VALUES (1, 1), (1, 2), (2, 13);
GO

-- =============================================
-- 5. QUẢN LÝ KHO (Cập nhật dùng VariantId)
-- =============================================
INSERT INTO dbo.Warehouses (WarehouseName, AddressId, ManagerId) VALUES 
('Main Warehouse Ho Chi Minh', 3, 1),
('Hanoi Branch Warehouse', 2, 2);
GO

-- Tồn kho theo Variant
INSERT INTO dbo.InventoryStock (WarehouseId, VariantId, Quantity, ReservedQuantity) VALUES 
(1, 1, 50, 0), (1, 2, 30, 0), (1, 3, 100, 0), (1, 4, 40, 0),
(2, 1, 20, 0), (2, 3, 50, 0);
GO

-- =============================================
-- 6. QUẢN LÝ BÁN HÀNG (Cập nhật dùng VariantId)
-- =============================================
INSERT INTO dbo.SalesOrders (OrderNumber, EmployeeId, CustomerCompanyId, CustomerPersonId, OrderStatusId) VALUES 
('SO-2024-001', 2, 1, NULL, 4),
('SO-2024-002', 2, NULL, 1, 1);
GO

-- Chi tiết đơn hàng tham chiếu đến VariantId
INSERT INTO dbo.SalesOrderDetails (OrderId, VariantId, WarehouseId, Quantity, UnitPrice, Discount, TaxRate) VALUES 
(1, 1, 1, 2, 45000000, 1000000, 10.00), 
(1, 3, 1, 5, 28000000, 0, 10.00),       
(2, 5, 2, 1, 35000000, 200000, 10.00);  
GO

INSERT INTO dbo.Invoices (InvoiceNumber, OrderId, TotalAmount, IsPaid) VALUES 
('INV-2024-001', 1, 238500000, 1); 
GO

INSERT INTO dbo.Payments (InvoiceId, Amount, PaymentMethod) VALUES 
(1, 238500000, 'Bank Transfer');
GO

-- =============================================
-- 7. QUẢN LÝ NHẬP HÀNG (Cập nhật dùng VariantId)
-- =============================================
INSERT INTO dbo.PurchaseOrders (PONumber, EmployeeId, VendorCompanyId, VendorPersonId, OrderStatusId) VALUES 
('PO-2024-001', 1, 1, NULL, 3),
('PO-2024-002', 1, NULL, 1, 1);
GO

INSERT INTO dbo.PurchaseOrderDetails (POId, VariantId, WarehouseId, Quantity, UnitPrice, TaxRate) VALUES 
(1, 1, 1, 10, 40000000, 10.00), 
(1, 3, 1, 50, 24000000, 10.00),
(2, 5, 2, 20, 30000000, 10.00);
GO

-- =============================================
-- 8. LOGS
-- =============================================
INSERT INTO dbo.Logs (EmployeeId, Action) VALUES 
(1, 'Initialize database system with Variants'),
(2, 'Create sales order SO-2024-001'),
(1, 'Receive purchase order batch PO-2024-001');
GO