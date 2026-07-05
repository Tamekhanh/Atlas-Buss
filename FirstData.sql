USE AtlasDB
GO

-- =============================================
-- 1. SEED LOOKUP TABLES (DANH MỤC HỆ THỐNG)
-- =============================================

-- Seed Roles & Permissions
-- 1. Insert Roles
INSERT INTO dbo.Roles (RoleName, Description) VALUES 
('Administrator', 'Full system access'),
('Sales Staff', 'Access to sales and inventory'),
('HR Manager', 'Access to personnel and administration'),
('Product Manager', 'Can create and manage products'),
('Viewer', 'Read-only access to view data');

-- 2. Insert Permissions (Aligned exactly with C# Policies)
INSERT INTO dbo.Permissions (PermissionKey, Description) VALUES 
('ADMIN', 'Full administrative access'),                             -- Id: 1
('PRODUCT_MANAGE', 'Create, edit, and delete products'),             -- Id: 2
('PRODUCT_VIEW', 'View products'),                                   -- Id: 3
('HR_MANAGE', 'Manage HR and departments'),                          -- Id: 4
('HR_VIEW', 'View HR records'),                                      -- Id: 5
('PARTY_MANAGE', 'Manage parties/entities'),                         -- Id: 6
('PARTY_VIEW', 'View parties/entities'),                             -- Id: 7
('CATEGORY_MANAGE', 'Manage product categories'),                    -- Id: 8
('CATEGORY_VIEW', 'View product categories'),                        -- Id: 9
('PURCHASE_MANAGE', 'Manage purchase orders'),                       -- Id: 10
('PURCHASE_VIEW', 'View purchase orders'),                           -- Id: 11
('SALE_MANAGE', 'Manage sales orders'),                              -- Id: 12
('SALE_VIEW', 'View sales orders');                                  -- Id: 13

-- 3. Map Roles to Permissions Logically
INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES 
-- Administrator (Role 1) gets ADMIN, which usually bypasses everything, 
-- but you can explicitly grant all if your architecture requires it.
(1, 1), (1, 2), (1, 4), (1, 6), (1, 8), (1, 10), (1, 12), -- Full access

-- Sales Staff (Role 2) gets Sales and Product View permissions
(2, 12), (2, 13), -- SALE_MANAGE, SALE_VIEW
(2, 3),           -- PRODUCT_VIEW
(2, 9),           -- CATEGORY_VIEW

-- HR Manager (Role 3) gets HR permissions
(3, 4), (3, 5),   -- HR_MANAGE, HR_VIEW

-- Product Manager (Role 4) gets Product and Category manage permissions
(4, 2), (4, 3),   -- PRODUCT_MANAGE, PRODUCT_VIEW
(4, 8), (4, 9),   -- CATEGORY_MANAGE, CATEGORY_VIEW

(5, 3), (5, 5), (5, 7), (5, 9), (5, 11), (5, 13);   -- Viewer role gets PARTY_VIEW and SALE_VIEW

-- Seed Statuses & Lookups
INSERT INTO dbo.SalesOrderStatuses (StatusName, Description) VALUES 
('Pending', 'Order has been created but not yet processed'),
('Confirmed', 'Order has been verified'),
('Shipped', 'Order is on its way to customer'),
('Completed', 'Order delivered and paid'),
('Cancelled', 'Order has been cancelled');

INSERT INTO dbo.PurchaseOrderStatuses (StatusName, Description) VALUES 
('Draft', 'Purchase order is being prepared'),
('Ordered', 'PO has been sent to vendor'),
('Received', 'Goods have been received in warehouse'),
('Cancelled', 'PO has been cancelled');

INSERT INTO dbo.PaymentMethods (MethodName) VALUES 
('Cash'), ('Bank Transfer'), ('Credit Card');

INSERT INTO dbo.PaymentStatuses (StatusName) VALUES 
('Pending'), ('Completed'), ('Failed'), ('Refunded');

INSERT INTO dbo.TransactionTypes (TypeName) VALUES 
('IN'), ('OUT'), ('TRANSFER'), ('ADJUST');

-- Seed Currencies
INSERT INTO dbo.Currencies (CurrencyCode, CurrencyName, ExchangeRate, IsBaseCurrency) VALUES 
('VND', 'Vietnamese Dong', 1.0, 1),
('USD', 'US Dollar', 24500.0, 0);

-- Seed Units
INSERT INTO dbo.Units (UnitName, ShortName) VALUES 
(N'Chiếc', N'Cái'), (N'Bộ', N'Bộ'), (N'Kilogram', N'Kg');
GO

-- =============================================
-- 2. THÔNG TIN ĐỐI TÁC (UNIFIED PARTY MODEL)
-- =============================================
INSERT INTO dbo.Addresses (AddressType, Street, City, State, Country) VALUES 
('Office', '123 Le Loi Street', 'District 1', 'Ho Chi Minh City', 'Vietnam'),          -- 1
('Residential', '456 Nguyen Hue Street', 'Hoan Kiem', 'Hanoi', 'Vietnam'),             -- 2
('Warehouse', '789 Tan Binh Industrial Zone', 'Tan Phu', 'Ho Chi Minh City', 'Vietnam'),-- 3
('Office', '101 Wall Street', 'New York', 'NY', 'USA'),                                -- 4
('Office', '222 Nguyen Van Linh Street', 'District 7', 'Ho Chi Minh City', 'Vietnam'); -- 5

INSERT INTO dbo.Contacts (Phone, Email) VALUES 
('0901234567', 'nguyenvana@gmail.com'),          -- 1
('0907654321', 'tranthib@gmail.com'),            -- 2
('0912334455', 'levanc@gmail.com'),              -- 3
('0988887777', 'contact@globalcorp.com'),        -- 4
('0922333444', 'product.manager@atlas.com');     -- 5

-- Gộp Person và Company vào 1 bảng Parties duy nhất
INSERT INTO dbo.Parties (PartyType, DisplayName, FirstName, LastName, DoB, TaxId, AddressId, ContactId, IsCustomer, IsVendor) VALUES 
('Person', 'Nguyen Anh', 'Anh', 'Nguyen', '1990-01-15', NULL, 1, 1, 0, 0),                       -- 1 (Employee only)
('Person', 'Tran Binh', 'Binh', 'Tran', '1992-05-20', NULL, 2, 2, 0, 0),                         -- 2 (Employee only)
('Person', 'Le Cuong', 'Cuong', 'Le', '1985-11-10', '08028200', 2, 3, 1, 0),                     -- 3 (Customer & Employee)
('Person', 'John Doe', 'John', 'Doe', '1980-03-25', '08028400', 4, 4, 0, 1),                     -- 4 (Vendor)
('Person', 'Pham Minh', 'Minh', 'Pham', '1993-08-12', NULL, 5, 5, 0, 0),                         -- 5 (Employee only)
('Company', 'Atlas Technology Company', NULL, NULL, NULL, 'TAX123456', 1, 1, 0, 0),              -- 6 (Own Company)
('Company', 'Global Trade Corp', NULL, NULL, NULL, 'TAX999888', 4, 4, 1, 1),                     -- 7 (Customer & Vendor)
('Company', 'Component Distribution X', NULL, NULL, NULL, 'TAX777666', 3, 4, 0, 1);              -- 8 (Vendor)
GO

-- =============================================
-- 3. NHÂN VIÊN & TÀI KHOẢN (Đã cập nhật cấu trúc)
-- =============================================
INSERT INTO dbo.Employee (EmployeeNumber, FullName, DoB, AddressId, ContactId) VALUES 
('EMP001', 'Nguyen Anh', '1990-01-15', 1, 1), 
('EMP002', 'Tran Binh', '1992-05-20', 2, 2), 
('EMP003', 'Le Cuong', '1985-11-10', 2, 3), 
('EMP004', 'Pham Minh', '1993-08-12', 5, 5),
('EMP005', 'John Doe', '1980-03-25', 4, 4);

INSERT INTO dbo.EmployeeAccounts (EmployeeId, Username, PasswordHash, RoleId) VALUES 
(1, 'admin_atlas', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 1),
(2, 'sales_staff', 'hash_2', 2),
(3, 'hr_admin', 'hash_3', 3),
(4, 'product_manager', 'hash_4', 4),
(5, 'user_viewer', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 5);

INSERT INTO dbo.Departments (DepartmentName, Description, ParentDepartmentId) VALUES 
('Executive Management', 'Senior management', NULL),
('Sales Department', 'Sales and business operations', 1),
('Warehouse and Logistics', 'Warehouse and inventory management', 1),
('Human Resources', 'Personnel management', 1);

INSERT INTO dbo.EmployeeDepartments (EmployeeId, DepartmentId) VALUES 
(1, 1), (1, 4), (2, 2);         
GO

-- =============================================
-- 4. SẢN PHẨM, THUỘC TÍNH & BIẾN THỂ
-- =============================================

INSERT INTO dbo.Taxes (TaxName, TaxRate, Description, IsStackable) VALUES 
('VAT 10%', 10.00, 'Standard value-added tax', 0),
('Special Tax 5%', 5.00, 'Luxury goods tax', 1);

INSERT INTO dbo.AttributeTypes (AttributeName, Description) VALUES 
('Color', 'Product color'), ('Size', 'Product size'), ('RAM', 'Memory capacity'), ('Storage', 'Disk capacity');

INSERT INTO dbo.AttributeValues (AttributeTypeId, AttributeValue) VALUES 
(1, 'Space Gray'), (1, 'Silver'), (1, 'Midnight'), -- Color (1-3)
(2, 'S'), (2, 'M'), (2, 'L'), (2, 'XL'),           -- Size (4-7)
(3, '8GB'), (3, '16GB'), (3, '32GB'),              -- RAM (8-10)
(4, '256GB'), (4, '512GB'), (4, '1TB');            -- Storage (11-13)

INSERT INTO dbo.Products (ProductName, ProductCode, UnitId, BaseSalePrice, BaseCostPrice, EmployeeId) VALUES 
('MacBook Pro 14', 'LAP-MAC-01', 1, 45000000, 40000000, 1),
('MacBook Air M2', 'LAP-MAC-02', 1, 28000000, 24000000, 1),
('ThinkPad X1 Carbon', 'LAP-LEN-01', 1, 35000000, 30000000, 1),
('AirPods Pro 2', 'AUD-APP-01', 1, 6000000, 4500000, 1),
('Secretlab Titan Evo', 'CHR-SEC-01', 1, 12000000, 9000000, 1);

-- Product Variants
INSERT INTO dbo.ProductVariants (ProductId, SKU, VariantPrice, VariantCost) VALUES 
(1, 'MAC-PRO-14-SGR-16-512', 45000000, 40000000), 
(1, 'MAC-PRO-14-SLV-16-512', 45000000, 40000000), 
(2, 'MAC-AIR-M2-MID-8-256', 28000000, 24000000),  
(2, 'MAC-AIR-M2-SLV-16-512', 32000000, 28000000), 
(3, 'TPI-X1-BLK-16-512', 35000000, 30000000),    
(4, 'APP-PODS-PRO-2', 6000000, 4500000),        
(5, 'SEC-TITAN-BLK-L', 12000000, 9000000),      
(5, 'SEC-TITAN-WHT-L', 13000000, 9500000);      

-- Mappings
INSERT INTO dbo.VariantAttributeMappings (VariantId, AttributeValueId) VALUES 
(1, 1), (1, 9), (1, 12), -- MAC PRO 14 SGR 16GB 512GB
(2, 2), (2, 9), (2, 12), -- MAC PRO 14 SLV 16GB 512GB
(3, 3), (3, 8), (3, 11), -- MAC AIR MID 8GB 256GB
(7, 1), (7, 6),          -- Chair BLK L
(8, 2), (8, 6);          -- Chair WHT L

INSERT INTO dbo.ProductDetails (ProductId, ProductDescription, Weight, WarrantyPeriod, Dimensions, Manufacturer) VALUES 
(1, 'Apple M2 Pro chip, 16GB RAM, 512GB SSD', 1.6, 12, '31x22x1.5cm', 'Apple'),
(5, 'Ergonomic premium gaming chair', 34.0, 60, '85x70x37cm', 'Secretlab');

INSERT INTO dbo.Categories (CategoryName, CategoryDesc) VALUES 
('Laptops', 'Laptop computers'), ('Accessories', 'Mouse, keyboards, headsets');

INSERT INTO dbo.CategoryProducts (CategoryId, ProductId) VALUES (1, 1), (1, 2), (1, 3), (2, 4);
GO

-- =============================================
-- 5. QUẢN LÝ KHO (INVENTORY)
-- =============================================
INSERT INTO dbo.Warehouses (WarehouseName, AddressId, ManagerId) VALUES 
('Main Warehouse Ho Chi Minh', 3, 1), ('Hanoi Branch Warehouse', 2, 2);

INSERT INTO dbo.InventoryStock (WarehouseId, VariantId, Quantity, ReservedQuantity) VALUES 
(1, 1, 50, 0), (1, 2, 30, 0), (1, 3, 100, 0), (1, 4, 40, 0),
(2, 1, 20, 0), (2, 3, 50, 0);
GO

-- =============================================
-- 6. QUẢN LÝ BÁN HÀNG (SALES)
-- =============================================
-- Khách hàng: Global Trade Corp (PartyId = 7), Le Cuong (PartyId = 3)
INSERT INTO dbo.SalesOrders (OrderNumber, EmployeeId, CustomerId, OrderStatusId, CurrencyId, ExchangeRate) VALUES 
('SO-2024-001', 2, 7, 4, 1, 1.0),
('SO-2024-002', 2, 3, 1, 1, 1.0);

-- Thêm chi tiết đơn hàng (Cột TaxRate được bỏ đi, tính toán thủ công nhét vào TaxAmount cho đơn giản)
INSERT INTO dbo.SalesOrderDetails (OrderId, VariantId, WarehouseId, Quantity, UnitPrice, Discount, TaxAmount) VALUES 
(1, 1, 1, 2, 45000000, 1000000, 8900000), -- Subtotal = 89m, 10% Tax = 8.9m
(1, 3, 1, 5, 28000000, 0, 14000000),      -- Subtotal = 140m, 10% Tax = 14m
(2, 5, 2, 1, 35000000, 200000, 3480000);  -- Subtotal = 34.8m, 10% Tax = 3.48m

-- Áp dụng bảng trung gian cho Stackable Taxes
INSERT INTO dbo.SalesOrderDetailTaxes (OrderDetailId, TaxId) VALUES 
(1, 1), (2, 1), (3, 1);

INSERT INTO dbo.Invoices (InvoiceNumber, OrderId, TotalAmount, IsPaid) VALUES 
('INV-2024-001', 1, 251900000, 1); -- (89m + 8.9m) + (140m + 14m)

INSERT INTO dbo.SalesOrderPayments (OrderId, Amount, PaymentMethodId, PaymentStatusId) VALUES 
(1, 251900000, 2, 2); -- 2: Bank Transfer, 2: Completed
GO

-- =============================================
-- 7. QUẢN LÝ NHẬP HÀNG (PURCHASE)
-- =============================================
-- Nhà cung cấp: Component Dist X (PartyId = 8), John Doe (PartyId = 4)
INSERT INTO dbo.PurchaseOrders (PONumber, EmployeeId, VendorId, OrderStatusId, CurrencyId, ExchangeRate) VALUES 
('PO-2024-001', 1, 8, 3, 1, 1.0),
('PO-2024-002', 1, 4, 1, 2, 24500.0); -- Mua bằng USD

INSERT INTO dbo.PurchaseOrderDetails (POId, VariantId, WarehouseId, Quantity, UnitPrice, TaxAmount) VALUES 
(1, 1, 1, 10, 40000000, 40000000), -- 400m total, 10% Tax = 40m
(1, 3, 1, 50, 24000000, 120000000),
(2, 5, 2, 20, 1200, 0); -- Giá 1200 USD

INSERT INTO dbo.PurchaseOrderDetailTaxes (OrderDetailId, TaxId) VALUES 
(1, 1), (2, 1);
GO

-- =============================================
-- 8. LOGS (Đã hỗ trợ chuẩn Audit Trail)
-- =============================================
INSERT INTO dbo.Logs (EmployeeId, Action) VALUES 
(1, 'Initialize database'),
(2, 'Create sales order SO-2024-001'),
(2, 'Update sales order SO-2024-001');
GO


-- Thêm Thuế
INSERT INTO dbo.Taxes (TaxName, TaxRate, Description, IsStackable) VALUES
('Import Tax 3%', 3.00, 'Thuế nhập khẩu hàng điện tử', 1),
('Eco Tax 1%', 1.00, 'Thuế bảo vệ môi trường', 1);

-- Thêm Loại thuộc tính mới
INSERT INTO dbo.AttributeTypes (AttributeName, Description) VALUES
('Material', 'Chất liệu sản phẩm'), 
('Processor', 'Chip xử lý'),
('Generation', 'Thế hệ sản phẩm');

-- Thêm Giá trị thuộc tính (Tiếp nối ID cũ)
INSERT INTO dbo.AttributeValues (AttributeTypeId, AttributeValue) VALUES
(5, 'Leather'), (5, 'Mesh'), (5, 'Fabric'),        -- Material (5)
(6, 'Intel i5'), (6, 'Intel i7'), (6, 'Apple M3'),  -- Processor (6)
(7, 'Gen 12'), (7, 'Gen 13'), (7, 'Gen 14');       -- Generation (7)


INSERT INTO dbo.Products (ProductName, ProductCode, UnitId, BaseSalePrice, BaseCostPrice, EmployeeId) VALUES
-- Laptops (Tiếp tục)
('Dell XPS 13', 'LAP-DEL-01', 1, 32000000, 27000000, 1),
('HP Spectre x360', 'LAP-HP-01', 1, 30000000, 25000000, 1),
('Asus ROG Zephyrus', 'LAP-ASU-01', 1, 42000000, 36000000, 1),
('Surface Laptop 5', 'LAP-MS-01', 1, 28000000, 23000000, 1),
('Gaming Laptop Acer', 'LAP-ACE-01', 1, 25000000, 20000000, 1),

-- Smartphones
('iPhone 15 Pro', 'PHO-APP-15P', 1, 28000000, 22000000, 1),
('iPhone 15', 'PHO-APP-15', 1, 20000000, 16000000, 1),
('Samsung S23 Ultra', 'PHO-SAM-S23U', 1, 25000000, 20000000, 1),
('Samsung S23', 'PHO-SAM-S23', 1, 18000000, 14000000, 1),
('Google Pixel 8', 'PHO-GOG-P8', 1, 17000000, 13000000, 1),

-- Monitors
('LG UltraFine 4K', 'MON-LG-4K', 1, 15000000, 12000000, 1),
('Samsung Odyssey G7', 'MON-SAM-G7', 1, 12000000, 9000000, 1),
('Dell UltraSharp 27', 'MON-DEL-27', 1, 11000000, 8000000, 1),
('Asus ProArt', 'MON-ASU-PA', 1, 18000000, 14000000, 1),
('ViewSonic VG24', 'MON-VWS-24', 1, 5000000, 3500000, 1),

-- Peripherals (Chuột/Bàn phím)
('Logitech MX Master 3S', 'MOU-LOG-MX3', 1, 2500000, 1500000, 1),
('Razer DeathAdder V3', 'MOU-RAZ-DV3', 1, 2000000, 1200000, 1),
('Keychron K2', 'KBD-KEY-K2', 1, 2200000, 1400000, 1),
('Logitech G Pro X', 'KBD-LOG-GPX', 1, 3500000, 2200000, 1),
('Corsair K70', 'KBD-COR-K70', 1, 4000000, 2800000, 1),

-- Furniture (Ghế/Bàn)
('Herman Miller Aeron', 'CHR-HM-AER', 1, 35000000, 25000000, 1),
('Sihoo M57', 'CHR-SIH-M57', 1, 5000000, 3500000, 1),
('Gaming Chair DX', 'CHR-GAM-DX', 1, 4000000, 2500000, 1),
('Standing Desk Pro', 'DSK-STD-PRO', 1, 8000000, 5000000, 1),
('Wooden Desk Minimal', 'DSK-WDN-MIN', 1, 3000000, 1800000, 1),

-- Accessories
('Magic Mouse 2', 'ACC-APP-MOU', 1, 2200000, 1500000, 1),
('Magic Keyboard', 'ACC-APP-KBD', 1, 3000000, 2000000, 1),
('AirPods Max', 'AUD-APP-MAX', 1, 12000000, 9000000, 1),
('Sony WH-1000XM5', 'AUD-SON-XM5', 1, 8000000, 6000000, 1),
('SteelSeries Arctis 7', 'AUD-STE-A7', 1, 5000000, 3500000, 1);


INSERT INTO dbo.ProductVariants (ProductId, SKU, VariantPrice, VariantCost) VALUES
-- Laptops
(6, 'DEL-XPS13-SLV-I5', 32000000, 27000000),
(6, 'DEL-XPS13-SLV-I7', 36000000, 31000000),
(7, 'HP-SPEC-WHT-I5', 30000000, 25000000),
(8, 'ASU-ROG-BLK-I9', 42000000, 36000000),
(9, 'MS-SURF-PLT-I5', 28000000, 23000000),
(10, 'ACE-GAM-BLK-I7', 25000000, 20000000),

-- Smartphones
(11, 'IPH-15P-NAT-128', 28000000, 22000000),
(11, 'IPH-15P-NAT-256', 31000000, 25000000),
(12, 'IPH-15-BLU-128', 20000000, 16000000),
(13, 'SAM-S23U-BLK-256', 25000000, 20000000),
(13, 'SAM-S23U-BLK-512', 28000000, 23000000),
(14, 'SAM-S23-WHT-128', 18000000, 14000000),
(15, 'GOG-P8-OBS-128', 17000000, 13000000),

-- Monitors
(16, 'LG-UF-4K-SILV', 15000000, 12000000),
(17, 'SAM-OD-G7-CURV', 12000000, 9000000),
(18, 'DEL-US-27-BLK', 11000000, 8000000),
(19, 'ASU-PA-27-BLK', 18000000, 14000000),
(20, 'VWS-VG-24-BLK', 5000000, 3500000),

-- Peripherals
(21, 'LOG-MX3-GRY', 2500000, 1500000),
(21, 'LOG-MX3-BLK', 2500000, 1500000),
(22, 'RAZ-DV3-WHT', 2000000, 1200000),
(23, 'KEY-K2-RGB', 2200000, 1400000),
(24, 'LOG-GPX-BLK', 3500000, 2200000),
(25, 'COR-K70-RGB', 4000000, 2800000),

-- Furniture
(26, 'HM-AER-GRY', 35000000, 25000000),
(27, 'SIH-M57-BLK', 5000000, 3500000),
(28, 'GAM-DX-RED', 4000000, 2500000),
(29, 'DSK-STD-WHT', 8000000, 5000000),
(30, 'DSK-WDN-OAK', 3000000, 1800000),

-- Accessories
(31, 'APP-MOU-SILV', 2200000, 1500000),
(32, 'APP-KBD-SILV', 3000000, 2000000),
(33, 'APP-MAX-SILV', 12000000, 9000000),
(34, 'SON-XM5-BLK', 8000000, 6000000),
(34, 'SON-XM5-SLV', 8000000, 6000000),
(35, 'STE-A7-BLK', 5000000, 3500000);