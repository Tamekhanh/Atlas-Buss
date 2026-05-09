USE AtlasDB
GO

-- =============================================
-- 1. SEED LOOKUP TABLES (BẮT BUỘC PHẢI LÀM TRƯỚC)
-- =============================================

-- Seed Roles
INSERT INTO dbo.Roles (RoleName, Description) VALUES 
('Administrator', 'Full system access'),
('Sales Staff', 'Access to sales and inventory'),
('HR Manager', 'Access to personnel and administration');
GO

-- Seed Permissions
INSERT INTO dbo.Permissions (PermissionKey, Description) VALUES 
('PRODUCT_MANAGE', 'Manage products and categories'),
('SALE_CREATE', 'Create and edit sales orders'),
('EMPLOYEE_MANAGE', 'Manage employee records'),
('INVENTORY_MANAGE', 'Manage warehouse and stock'),
('ADMIN_ALL', 'Full administrative access'),
('HR_MANAGE', 'Manage HR and departments');
GO

-- Seed RolePermissions (Gán quyền cho Role)
INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES 
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), -- Admin có tất cả quyền
(2, 1), (2, 2), (2, 4),                         -- Sales có quyền Sản phẩm, Bán hàng, Kho
(3, 1), (3, 3), (3, 5), (3, 6);                 -- HR có quyền Sản phẩm, Nhân sự, Admin, HR
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

-- =============================================
-- 2. General Information (Giữ nguyên logic cũ)
-- =============================================
INSERT INTO dbo.Addresses (AddressType, Street, City, State, Country) VALUES 
('Office', '123 Le Loi Street', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
('Residential', '456 Nguyen Hue Street', 'Hoan Kiem', 'Hanoi', 'Vietnam'),
('Warehouse', '789 Tan Binh Industrial Zone', 'Tan Phu', 'Ho Chi Minh City', 'Vietnam'),
('Office', '101 Wall Street', 'New York', 'NY', 'USA');
GO

INSERT INTO dbo.Contacts (Phone, Email) VALUES 
('0901234567', 'nguyenvana@gmail.com'),
('0907654321', 'tranthib@gmail.com'),
('0912334455', 'levanc@gmail.com'),
('0988887777', 'contact@globalcorp.com');
GO

INSERT INTO dbo.Persons (FirstName, LastName, DoB, AddressId, ContactId) VALUES 
('Anh', 'Nguyen', '1990-01-15', 1, 1), 
('Binh', 'Tran', '1992-05-20', 2, 2),   
('Cuong', 'Le', '1985-11-10', 2, 3),     
('John', 'Doe', '1980-03-25', 4, 4);    
GO

-- =============================================
-- 3. Business Operations (Giữ nguyên logic cũ)
-- =============================================
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
-- 4. Employee Management (Thay đổi can... thành RoleId)
-- =============================================
INSERT INTO dbo.Employee (EmployeeNumber, PersonId) VALUES 
('EMP001', 1), 
('EMP002', 2),
('EMP003', 3);
GO

INSERT INTO dbo.EmployeeAccounts (EmployeeId, Username, PasswordHash, RoleId) VALUES 
(1, 'admin_atlas', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 1), -- Admin Role
(2, 'sales_staff', 'hash_password_2', 2), -- Sales Role
(3, 'hr_admin', 'hash_password_3', 3); -- HR Role
GO

INSERT INTO dbo.Departments (DepartmentName, Description, ParentDepartmentId) VALUES 
('Executive Management', 'Senior management', NULL),
('Sales Department', 'Sales and business operations', 1),
('Warehouse and Logistics', 'Warehouse and inventory management', 1),
('Human Resources', 'Personnel management', 1);
GO

INSERT INTO dbo.EmployeeDepartments (EmployeeId, DepartmentId) VALUES 
(1, 1), (1, 4), 
(2, 2);         
GO

-- =============================================
-- 5. Inventory Management (Thêm ReservedQuantity)
-- =============================================
INSERT INTO dbo.Taxes (TaxName, TaxRate, Description) VALUES 
('VAT 10%', 10.00, 'Standard value-added tax'),
('Special Tax 5%', 5.00, 'Luxury goods tax');
GO

INSERT INTO dbo.Products (ProductName, ProductCode, SalePrice, CostPrice, EmployeeId) VALUES 
('Dell XPS 13 Laptop', 'LAP-DELL-01', 25000000, 20000000, 1),
('Logitech MX Master 3 Mouse', 'MOU-LOGI-01', 2500000, 1500000, 1),
('Keychron K2 Mechanical Keyboard', 'KBD-KEYC-01', 2000000, 1200000, 1);
GO

INSERT INTO dbo.ProductTaxes (ProductId, TaxId) VALUES (1, 1), (2, 1), (3, 1);
GO

INSERT INTO dbo.ProductDetails (ProductId, ProductDescription, Weight, WarrantyPeriod, Dimensions, Manufacturer) VALUES 
(1, 'Premium ultrabook laptop', 1.2, 12, '30x20x1.5cm', 'Dell'),
(2, 'Ergonomic mouse', 0.1, 24, '12x8x5cm', 'Logitech'),
(3, 'Wireless mechanical keyboard', 0.8, 12, '32x12x3cm', 'Keychron');
GO

INSERT INTO dbo.Categories (CategoryName, CategoryDesc) VALUES 
('Laptops', 'Laptop computers'),
('Accessories', 'Mouse, keyboards, headsets');
GO

INSERT INTO dbo.CategoryProducts (CategoryId, ProductId) VALUES (1, 1), (2, 2), (2, 3);
GO

INSERT INTO dbo.Warehouses (WarehouseName, AddressId, ManagerId) VALUES 
('Main Warehouse Ho Chi Minh', 3, 1),
('Hanoi Branch Warehouse', 2, 2);
GO

-- Thêm cột ReservedQuantity = 0
INSERT INTO dbo.InventoryStock (WarehouseId, ProductId, Quantity, ReservedQuantity) VALUES 
(1, 1, 50, 0), (1, 2, 100, 0), (1, 3, 80, 0),
(2, 1, 20, 0), (2, 2, 30, 0), (2, 3, 40, 0);
GO

-- =============================================
-- 6. Sales Management (Thay đổi OrderStatus -> OrderStatusId)
-- =============================================
-- Order 1: Completed (Id = 4)
INSERT INTO dbo.SalesOrders (OrderNumber, EmployeeId, CustomerCompanyId, CustomerPersonId, OrderStatusId) VALUES 
('SO-2024-001', 2, 1, NULL, 4);

-- Order 2: Pending (Id = 1)
INSERT INTO dbo.SalesOrders (OrderNumber, EmployeeId, CustomerCompanyId, CustomerPersonId, OrderStatusId) VALUES 
('SO-2024-002', 2, NULL, 1, 1);
GO

INSERT INTO dbo.SalesOrderDetails (OrderId, ProductId, WarehouseId, Quantity, UnitPrice, Discount, TaxRate) VALUES 
(1, 1, 1, 2, 25000000, 1000000, 10.00), 
(1, 2, 1, 5, 2500000, 0, 10.00),       
(2, 3, 2, 1, 2000000, 200000, 10.00);  
GO

-- BỔ SUNG DỮ LIỆU CHO HÓA ĐƠN (Để khớp với Schema mới)
INSERT INTO dbo.Invoices (InvoiceNumber, OrderId, TotalAmount, IsPaid) VALUES 
('INV-2024-001', 1, 61500000, 1); -- Giả định tổng tiền cho SO-001
GO

INSERT INTO dbo.Payments (InvoiceId, Amount, PaymentMethod) VALUES 
(1, 61500000, 'Bank Transfer');
GO

-- =============================================
-- 7. Purchase Management (Thay đổi OrderStatus -> OrderStatusId)
-- =============================================
-- PO 1: Received (Id = 3)
INSERT INTO dbo.PurchaseOrders (PONumber, EmployeeId, VendorCompanyId, VendorPersonId, OrderStatusId) VALUES 
('PO-2024-001', 1, 1, NULL, 3);

-- PO 2: Draft (Id = 1)
INSERT INTO dbo.PurchaseOrders (PONumber, EmployeeId, VendorCompanyId, VendorPersonId, OrderStatusId) VALUES 
('PO-2024-002', 1, NULL, 1, 1);
GO

INSERT INTO dbo.PurchaseOrderDetails (POId, ProductId, WarehouseId, Quantity, UnitPrice, TaxRate) VALUES 
(1, 1, 1, 10, 20000000, 10.00), 
(1, 2, 1, 50, 1500000, 10.00),
(2, 3, 2, 20, 1200000, 10.00);
GO

-- =============================================
-- 8. Logs
-- =============================================
INSERT INTO dbo.Logs (EmployeeId, Action) VALUES 
(1, 'Initialize database system'),
(2, 'Create sales order SO-2024-001'),
(1, 'Receive purchase order batch PO-2024-001');
GO