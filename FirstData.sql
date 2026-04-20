USE AtlasDB
GO

-- =============================================
-- 1. General Information
-- =============================================
INSERT INTO AtlasDB.dbo.Addresses (AddressType, Street, City, State, Country) VALUES 
(N'Văn phòng', N'123 Đường Lê Lợi', N'Quận 1', N'TP. Hồ Chí Minh', N'Việt Nam'),
(N'Nhà riêng', N'456 Đường Nguyễn Huệ', N'Hoàn Kiếm', N'Hà Nội', N'Việt Nam'),
(N'Kho bãi', N'789 Khu Công Nghiệp Tân Bình', N'Tân Phú', N'TP. Hồ Chí Minh', N'Việt Nam'),
(N'Văn phòng', N'101 Wall Street', N'New York', N'NY', N'USA');
GO

INSERT INTO AtlasDB.dbo.Contacts (Phone, Email) VALUES 
(N'0901234567', N'nguyenvana@gmail.com'),
(N'0907654321', N'tranthib@gmail.com'),
(N'0912334455', N'levanc@gmail.com'),
(N'0988887777', N'contact@globalcorp.com');
GO

INSERT INTO AtlasDB.dbo.Persons (FirstName, LastName, DoB, AddressId, ContactId) VALUES 
(N'Văn A', N'Nguyễn', '1990-01-15', 1, 1), -- Nhân viên 1
(N'Thị B', N'Trần', '1992-05-20', 2, 2),   -- Nhân viên 2
(N'Văn C', N'Lê', '1985-11-10', 2, 3),     -- Khách hàng cá nhân
(N'John', N'Doe', '1980-03-25', 4, 4);    -- Đại diện công ty
GO

-- =============================================
-- 2. Business Operations
-- =============================================
INSERT INTO AtlasDB.dbo.Companies (CompanyName, TaxId, AddressId, ContactId) VALUES 
(N'Công ty Công nghệ Atlas', N'TAX123456', 1, 1),
(N'Global Trade Corp', N'TAX999888', 4, 4),
(N'Phân phối Linh kiện X', N'TAX777666', 3, 4);
GO

-- Gán công ty làm Nhà cung cấp
INSERT INTO AtlasDB.dbo.VendorsCompany (CompanyId) VALUES (2), (3);
GO

-- Gán cá nhân làm Nhà cung cấp (Freelancer)
INSERT INTO AtlasDB.dbo.VendorsPerson (PersonId, TaxId) VALUES (4, '08028400');
GO

-- Gán công ty làm Khách hàng
INSERT INTO AtlasDB.dbo.CustomerCompany (CompanyId) VALUES (2);
GO

-- Gán cá nhân làm Khách hàng
INSERT INTO AtlasDB.dbo.CustomerPerson (PersonId, TaxId) VALUES (3, '08028200');
GO

-- =============================================
-- 3. Employee Management
-- =============================================
INSERT INTO AtlasDB.dbo.Employee (EmployeeNumber, PersonId) VALUES 
('EMP001', 1), 
('EMP002', 2);
GO

INSERT INTO AtlasDB.dbo.EmployeeAccounts (EmployeeId, Username, PasswordHash, canProduct, canSale, canEmployee, canInventory, canAdministration, canHR) VALUES 
(1, 'admin_atlas', 'hash_password_1', 1, 1, 1, 1, 1, 1), -- Quyền toàn năng
(2, 'sales_staff', 'hash_password_2', 1, 1, 0, 1, 0, 0); -- Quyền bán hàng và kho
GO

INSERT INTO AtlasDB.dbo.Departments (DepartmentName, Description, ParentDepartmentId) VALUES 
(N'Ban Giám Đốc', N'Quản lý cấp cao', NULL),
(N'Phòng Kinh Doanh', N'Phụ trách bán hàng', 1),
(N'Phòng Kho vận', N'Quản lý kho và vận chuyển', 1),
(N'Phòng Nhân Sự', N'Quản lý con người', 1);
GO

INSERT INTO AtlasDB.dbo.EmployeeDepartments (EmployeeId, DepartmentId) VALUES 
(1, 1), (1, 4), -- Admin thuộc Ban giám đốc và Nhân sự
(2, 2);         -- Nhân viên sales thuộc phòng kinh doanh
GO

-- =============================================
-- 4. Inventory Management
-- =============================================
INSERT INTO AtlasDB.dbo.Taxes (TaxName, TaxRate, Description) VALUES 
(N'VAT 10%', 10.00, N'Thuế giá trị gia tăng tiêu chuẩn'),
(N'Thuế đặc biệt 5%', 5.00, N'Thuế cho hàng xa xỉ');
GO

INSERT INTO AtlasDB.dbo.Products (ProductName, ProductCode, SalePrice, CostPrice, EmployeeId) VALUES 
(N'Laptop Dell XPS 13', 'LAP-DELL-01', 25000000, 20000000, 1),
(N'Chuột Logitech MX Master 3', 'MOU-LOGI-01', 2500000, 1500000, 1),
(N'Bàn phím Cơ Keychron K2', 'KBD-KEYC-01', 2000000, 1200000, 1);
GO

INSERT INTO AtlasDB.dbo.ProductTaxes (ProductId, TaxId) VALUES 
(1, 1), -- Laptop chịu thuế 10%
(2, 1), -- Chuột chịu thuế 10%
(3, 1); -- Phím chịu thuế 10%
GO

INSERT INTO AtlasDB.dbo.ProductDetails (ProductId, ProductDescription, Weight, WarrantyPeriod, Dimensions, Manufacturer) VALUES 
(1, N'Laptop cao cấp mỏng nhẹ', 1.2, 12, N'30x20x1.5cm', N'Dell'),
(2, N'Chuột công thái học', 0.1, 24, N'12x8x5cm', N'Logitech'),
(3, N'Bàn phím không dây', 0.8, 12, N'32x12x3cm', N'Keychron');
GO

INSERT INTO AtlasDB.dbo.Categories (CategoryName, CategoryDesc) VALUES 
(N'Máy tính xách tay', N'Các loại laptop'),
(N'Phụ kiện', N'Chuột, bàn phím, tai nghe');
GO

INSERT INTO AtlasDB.dbo.CategoryProducts (CategoryId, ProductId) VALUES 
(1, 1), 
(2, 2), 
(2, 3);
GO

INSERT INTO AtlasDB.dbo.Warehouses (WarehouseName, AddressId, ManagerId) VALUES 
(N'Kho Tổng HCM', 3, 1),
(N'Kho Chi Nhánh HN', 2, 2);
GO

INSERT INTO AtlasDB.dbo.InventoryStock (WarehouseId, ProductId, Quantity) VALUES 
(1, 1, 50), (1, 2, 100), (1, 3, 80),
(2, 1, 20), (2, 2, 30), (2, 3, 40);
GO

-- =============================================
-- 5. Sales Management
-- =============================================
-- Đơn hàng 1: Bán cho Công ty (CustomerCompanyId = 1)
INSERT INTO AtlasDB.dbo.SalesOrders (OrderNumber, EmployeeId, CustomerCompanyId, CustomerPersonId, OrderStatus) VALUES 
('SO-2024-001', 2, 1, NULL, 'Completed');

-- Đơn hàng 2: Bán cho Cá nhân (CustomerPersonId = 1)
INSERT INTO AtlasDB.dbo.SalesOrders (OrderNumber, EmployeeId, CustomerCompanyId, CustomerPersonId, OrderStatus) VALUES 
('SO-2024-002', 2, NULL, 1, 'Pending');
GO

INSERT INTO AtlasDB.dbo.SalesOrderDetails (OrderId, ProductId, WarehouseId, Quantity, UnitPrice, Discount, TaxRate) VALUES 
(1, 1, 1, 2, 25000000, 1000000, 10.00), -- Mua 2 laptop, giảm 1tr
(1, 2, 1, 5, 2500000, 0, 10.00),       -- Mua 5 chuột
(2, 3, 2, 1, 2000000, 200000, 10.00);  -- Mua 1 phím, giảm 200k
GO

-- =============================================
-- 6. Purchase Management
-- =============================================
-- Nhập hàng từ Nhà cung cấp Công ty
INSERT INTO AtlasDB.dbo.PurchaseOrders (PONumber, EmployeeId, VendorCompanyId, VendorPersonId, OrderStatus) VALUES 
('PO-2024-001', 1, 1, NULL, 'Received');

-- Nhập hàng từ Nhà cung cấp Cá nhân
INSERT INTO AtlasDB.dbo.PurchaseOrders (PONumber, EmployeeId, VendorCompanyId, VendorPersonId, OrderStatus) VALUES 
('PO-2024-002', 1, NULL, 1, 'Draft');
GO

INSERT INTO AtlasDB.dbo.PurchaseOrderDetails (POId, ProductId, WarehouseId, Quantity, UnitPrice, TaxRate) VALUES 
(1, 1, 1, 10, 20000000, 10.00), 
(1, 2, 1, 50, 1500000, 10.00),
(2, 3, 2, 20, 1200000, 10.00);
GO

-- =============================================
-- 7. Logs
-- =============================================
INSERT INTO AtlasDB.dbo.Logs (EmployeeId, Action) VALUES 
(1, N'Khởi tạo hệ thống database'),
(2, N'Tạo đơn hàng SO-2024-001'),
(1, N'Nhập kho lô hàng PO-2024-001');
GO