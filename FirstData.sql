USE AtlasDB
GO

-- =============================================
-- 1. SEED LOOKUP TABLES (DANH MỤC HỆ THỐNG)
-- =============================================

-- Seed Roles & Permissions
INSERT INTO dbo.Roles (RoleName, Description) VALUES 
('Administrator', 'Full system access'),
('Sales Staff', 'Access to sales and inventory'),
('HR Manager', 'Access to personnel and administration'),
('Product Manager', 'Can create and manage products');

INSERT INTO dbo.Permissions (PermissionKey, Description) VALUES 
('PRODUCT_MANAGE', 'Manage products and categories'),
('SALE_CREATE', 'Create and edit sales orders'),
('PURCHASE_CREATE', 'Create and edit purchase orders'),
('EMPLOYEE_MANAGE', 'Manage employee records'),
('INVENTORY_MANAGE', 'Manage warehouse and stock'),
('ADMIN_ALL', 'Full administrative access'),
('HR_MANAGE', 'Manage HR and departments'),
('PRODUCT_CREATE', 'Create products');

INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES 
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8), -- Admin
(2, 1), (2, 2), (2, 4),                                         -- Sales
(3, 1), (3, 3), (3, 5), (3, 6),                                 -- HR
(4, 7);                                                         -- Product Manager

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
('EMP004', 'Pham Minh', '1993-08-12', 5, 5);

INSERT INTO dbo.EmployeeAccounts (EmployeeId, Username, PasswordHash, RoleId) VALUES 
(1, 'admin_atlas', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 1),
(2, 'sales_staff', 'hash_2', 2),
(3, 'hr_admin', 'hash_3', 3),
(4, 'product_manager', 'hash_4', 4);

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
INSERT INTO dbo.Logs (EmployeeId, Action, OldValue, NewValue) VALUES 
(1, 'Initialize database', NULL, '{"Status":"Success", "Version":"2.0"}'),
(2, 'Create sales order SO-2024-001', NULL, '{"OrderId": 1, "Status": "Pending"}'),
(2, 'Update sales order SO-2024-001', '{"Status": "Pending"}', '{"Status": "Completed"}');
GO