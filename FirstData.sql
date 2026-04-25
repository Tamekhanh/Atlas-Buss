USE AtlasDB
GO

-- =============================================
-- 1. General Information
-- =============================================
INSERT INTO AtlasDB.dbo.Addresses (AddressType, Street, City, State, Country) VALUES 
('Office', '123 Le Loi Street', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
('Residential', '456 Nguyen Hue Street', 'Hoan Kiem', 'Hanoi', 'Vietnam'),
('Warehouse', '789 Tan Binh Industrial Zone', 'Tan Phu', 'Ho Chi Minh City', 'Vietnam'),
('Office', '101 Wall Street', 'New York', 'NY', 'USA');
GO

INSERT INTO AtlasDB.dbo.Contacts (Phone, Email) VALUES 
('0901234567', 'nguyenvana@gmail.com'),
('0907654321', 'tranthib@gmail.com'),
('0912334455', 'levanc@gmail.com'),
('0988887777', 'contact@globalcorp.com');
GO

INSERT INTO AtlasDB.dbo.Persons (FirstName, LastName, DoB, AddressId, ContactId) VALUES 
('Anh', 'Nguyen', '1990-01-15', 1, 1), -- Employee 1
('Binh', 'Tran', '1992-05-20', 2, 2),   -- Employee 2
('Cuong', 'Le', '1985-11-10', 2, 3),     -- Individual Customer
('John', 'Doe', '1980-03-25', 4, 4);    -- Company Representative
GO

-- =============================================
-- 2. Business Operations
-- =============================================
INSERT INTO AtlasDB.dbo.Companies (CompanyName, TaxId, AddressId, ContactId) VALUES 
('Atlas Technology Company', 'TAX123456', 1, 1),
('Global Trade Corp', 'TAX999888', 4, 4),
('Component Distribution X', 'TAX777666', 3, 4);
GO

-- Assign companies as vendors
INSERT INTO AtlasDB.dbo.VendorsCompany (CompanyId) VALUES (2), (3);
GO

-- Assign individual as vendor (Freelancer)
INSERT INTO AtlasDB.dbo.VendorsPerson (PersonId, TaxId) VALUES (4, '08028400');
GO

-- Assign company as customer
INSERT INTO AtlasDB.dbo.CustomerCompany (CompanyId) VALUES (2);
GO

-- Assign individual as customer
INSERT INTO AtlasDB.dbo.CustomerPerson (PersonId, TaxId) VALUES (3, '08028200');
GO

-- =============================================
-- 3. Employee Management
-- =============================================
INSERT INTO AtlasDB.dbo.Employee (EmployeeNumber, PersonId) VALUES 
('EMP001', 1), 
('EMP002', 2),
('EMP003', 3);
GO

INSERT INTO AtlasDB.dbo.EmployeeAccounts (EmployeeId, Username, PasswordHash, canProduct, canSale, canEmployee, canInventory, canAdministration, canHR) VALUES 
(1, 'admin_atlas', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 1, 1, 1, 1, 1, 1), -- Full permissions
(2, 'sales_staff', 'hash_password_2', 1, 1, 0, 1, 0, 0), -- Sales and inventory permissions
(3, 'hr_admin', 'hash_password_3', 1, 1, 1, 1, 1, 1); -- Full permissions
GO

INSERT INTO AtlasDB.dbo.Departments (DepartmentName, Description, ParentDepartmentId) VALUES 
('Executive Management', 'Senior management', NULL),
('Sales Department', 'Sales and business operations', 1),
('Warehouse and Logistics', 'Warehouse and inventory management', 1),
('Human Resources', 'Personnel management', 1);
GO

INSERT INTO AtlasDB.dbo.EmployeeDepartments (EmployeeId, DepartmentId) VALUES 
(1, 1), (1, 4), -- Admin belongs to Executive Management and HR
(2, 2);         -- Sales employee belongs to Sales Department
GO

-- =============================================
-- 4. Inventory Management
-- =============================================
INSERT INTO AtlasDB.dbo.Taxes (TaxName, TaxRate, Description) VALUES 
('VAT 10%', 10.00, 'Standard value-added tax'),
('Special Tax 5%', 5.00, 'Luxury goods tax');
GO

INSERT INTO AtlasDB.dbo.Products (ProductName, ProductCode, SalePrice, CostPrice, EmployeeId) VALUES 
('Dell XPS 13 Laptop', 'LAP-DELL-01', 25000000, 20000000, 1),
('Logitech MX Master 3 Mouse', 'MOU-LOGI-01', 2500000, 1500000, 1),
('Keychron K2 Mechanical Keyboard', 'KBD-KEYC-01', 2000000, 1200000, 1);
GO

INSERT INTO AtlasDB.dbo.ProductTaxes (ProductId, TaxId) VALUES 
(1, 1), -- Laptop chịu thuế 10%
(2, 1), -- Chuột chịu thuế 10%
(3, 1); -- Phím chịu thuế 10%
GO

INSERT INTO AtlasDB.dbo.ProductDetails (ProductId, ProductDescription, Weight, WarrantyPeriod, Dimensions, Manufacturer) VALUES 
(1, 'Premium ultrabook laptop', 1.2, 12, '30x20x1.5cm', 'Dell'),
(2, 'Ergonomic mouse', 0.1, 24, '12x8x5cm', 'Logitech'),
(3, 'Wireless mechanical keyboard', 0.8, 12, '32x12x3cm', 'Keychron');
GO

INSERT INTO AtlasDB.dbo.Categories (CategoryName, CategoryDesc) VALUES 
('Laptops', 'Laptop computers'),
('Accessories', 'Mouse, keyboards, headsets');
GO

INSERT INTO AtlasDB.dbo.CategoryProducts (CategoryId, ProductId) VALUES 
(1, 1), 
(2, 2), 
(2, 3);
GO

INSERT INTO AtlasDB.dbo.Warehouses (WarehouseName, AddressId, ManagerId) VALUES 
('Main Warehouse Ho Chi Minh', 3, 1),
('Hanoi Branch Warehouse', 2, 2);
GO

INSERT INTO AtlasDB.dbo.InventoryStock (WarehouseId, ProductId, Quantity) VALUES 
(1, 1, 50), (1, 2, 100), (1, 3, 80),
(2, 1, 20), (2, 2, 30), (2, 3, 40);
GO

-- =============================================
-- 5. Sales Management
-- =============================================
-- Sales Order 1: Sale to company (CustomerCompanyId = 1)
INSERT INTO AtlasDB.dbo.SalesOrders (OrderNumber, EmployeeId, CustomerCompanyId, CustomerPersonId, OrderStatus) VALUES 
('SO-2024-001', 2, 1, NULL, 'Completed');

-- Sales Order 2: Sale to individual (CustomerPersonId = 1)
INSERT INTO AtlasDB.dbo.SalesOrders (OrderNumber, EmployeeId, CustomerCompanyId, CustomerPersonId, OrderStatus) VALUES 
('SO-2024-002', 2, NULL, 1, 'Pending');
GO

INSERT INTO AtlasDB.dbo.SalesOrderDetails (OrderId, ProductId, WarehouseId, Quantity, UnitPrice, Discount, TaxRate) VALUES 
(1, 1, 1, 2, 25000000, 1000000, 10.00), -- Buy 2 laptops, 1M discount
(1, 2, 1, 5, 2500000, 0, 10.00),       -- Buy 5 mice
(2, 3, 2, 1, 2000000, 200000, 10.00);  -- Buy 1 keyboard, 200k discount
GO

-- =============================================
-- 6. Purchase Management
-- =============================================
-- Purchase from company vendor
INSERT INTO AtlasDB.dbo.PurchaseOrders (PONumber, EmployeeId, VendorCompanyId, VendorPersonId, OrderStatus) VALUES 
('PO-2024-001', 1, 1, NULL, 'Received');

-- Purchase from individual vendor
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
(1, 'Initialize database system'),
(2, 'Create sales order SO-2024-001'),
(1, 'Receive purchase order batch PO-2024-001');
GO

-- If these log rows were already imported with broken encoding, normalize them here.
UPDATE AtlasDB.dbo.Logs SET Action = 'Initialize database system' WHERE Id = 1;
UPDATE AtlasDB.dbo.Logs SET Action = 'Create sales order SO-2024-001' WHERE Id = 2;
UPDATE AtlasDB.dbo.Logs SET Action = 'Receive purchase order batch PO-2024-001' WHERE Id = 3;
GO