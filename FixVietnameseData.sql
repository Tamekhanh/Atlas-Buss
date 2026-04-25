USE AtlasDB
GO

-- Normalize legacy Vietnamese/mojibake seed data to English.
UPDATE dbo.Addresses SET AddressType='Office', Street='123 Le Loi Street', City='District 1', State='Ho Chi Minh City', Country='Vietnam' WHERE Id=1;
UPDATE dbo.Addresses SET AddressType='Residential', Street='456 Nguyen Hue Street', City='Hoan Kiem', State='Hanoi', Country='Vietnam' WHERE Id=2;
UPDATE dbo.Addresses SET AddressType='Warehouse', Street='789 Tan Binh Industrial Zone', City='Tan Phu', State='Ho Chi Minh City', Country='Vietnam' WHERE Id=3;
UPDATE dbo.Addresses SET AddressType='Office' WHERE Id=4;

UPDATE dbo.Persons SET FirstName='Anh', LastName='Nguyen' WHERE Id=1;
UPDATE dbo.Persons SET FirstName='Binh', LastName='Tran' WHERE Id=2;
UPDATE dbo.Persons SET FirstName='Cuong', LastName='Le' WHERE Id=3;

UPDATE dbo.Companies SET CompanyName='Atlas Technology Company' WHERE Id=1;
UPDATE dbo.Companies SET CompanyName='Component Distribution X' WHERE Id=3;

UPDATE dbo.Products SET ProductName='Dell XPS 13 Laptop' WHERE Id=1;
UPDATE dbo.Products SET ProductName='Logitech MX Master 3 Mouse' WHERE Id=2;
UPDATE dbo.Products SET ProductName='Keychron K2 Mechanical Keyboard' WHERE Id=3;

UPDATE dbo.ProductDetails SET ProductDescription='Premium ultrabook laptop' WHERE ProductId=1;
UPDATE dbo.ProductDetails SET ProductDescription='Ergonomic mouse' WHERE ProductId=2;
UPDATE dbo.ProductDetails SET ProductDescription='Wireless mechanical keyboard' WHERE ProductId=3;

UPDATE dbo.Categories SET CategoryName='Laptops', CategoryDesc='Laptop computers' WHERE Id=1;
UPDATE dbo.Categories SET CategoryName='Accessories', CategoryDesc='Mouse, keyboards, headsets' WHERE Id=2;

UPDATE dbo.Warehouses SET WarehouseName='Main Warehouse Ho Chi Minh' WHERE Id=1;
UPDATE dbo.Warehouses SET WarehouseName='Hanoi Branch Warehouse' WHERE Id=2;

UPDATE dbo.Departments SET DepartmentName='Executive Management', Description='Senior management' WHERE Id=1;
UPDATE dbo.Departments SET DepartmentName='Sales Department', Description='Sales and business operations' WHERE Id=2;
UPDATE dbo.Departments SET DepartmentName='Warehouse and Logistics', Description='Warehouse and inventory management' WHERE Id=3;
UPDATE dbo.Departments SET DepartmentName='Human Resources', Description='Personnel management' WHERE Id=4;

UPDATE dbo.Taxes SET TaxName='VAT 10%', Description='Standard value-added tax' WHERE Id=1;
UPDATE dbo.Taxes SET TaxName='Special Tax 5%', Description='Luxury goods tax' WHERE Id=2;

-- Keep employee permissions for EMP003 fully enabled.
UPDATE dbo.EmployeeAccounts
SET canProduct = 1,
    canSale = 1,
    canEmployee = 1,
    canInventory = 1,
    canAdministration = 1,
    canHR = 1
WHERE EmployeeId = 3;

-- Normalize existing log messages.
UPDATE dbo.Logs SET Action='Initialize database system' WHERE Id=1;
UPDATE dbo.Logs SET Action='Create sales order SO-2024-001' WHERE Id=2;
UPDATE dbo.Logs SET Action='Receive purchase order batch PO-2024-001' WHERE Id=3;
UPDATE dbo.Logs SET Action='User login' WHERE Action='Dang nhap he thong';
UPDATE dbo.Logs SET Action='User logout' WHERE Action='Dang xuat he thong';
UPDATE dbo.Logs SET Action=REPLACE(Action, 'Dang ky tai khoan:', 'Account registration:') WHERE Action LIKE 'Dang ky tai khoan:%';
GO
