-- Employee accounts overview (start from EmployeeAccounts so account rows are never hidden)
SELECT
    ea.EmployeeId,
    ea.Username,
    ea.IsActive,
    ea.LastLogin,
    e.EmployeeNumber,
    p.FirstName + ' ' + p.LastName AS FullName,
    c.Phone,
    a.City,
    ea.canProduct,
    ea.canSale,
    ea.canEmployee,
    ea.canInventory,
    ea.canAdministration,
    ea.canHR
FROM AtlasDB.dbo.EmployeeAccounts ea
LEFT JOIN AtlasDB.dbo.Employee e ON ea.EmployeeId = e.id
LEFT JOIN AtlasDB.dbo.Persons p ON e.PersonId = p.id
LEFT JOIN AtlasDB.dbo.Contacts c ON p.ContactId = c.id
LEFT JOIN AtlasDB.dbo.Addresses a ON p.AddressId = a.id
ORDER BY ea.EmployeeId;

-- Quick check by username (change value as needed)
SELECT
    ea.EmployeeId,
    ea.Username,
    ea.IsActive,
    ea.LastLogin
FROM AtlasDB.dbo.EmployeeAccounts ea
WHERE ea.Username = 'tamek';


SELECT 
    p.ProductName, 
    p.ProductCode, 
    cat.CategoryName, 
    pd.Manufacturer, 
    pd.WarrantyPeriod, 
    p.SalePrice
FROM AtlasDB.dbo.Products p
JOIN AtlasDB.dbo.ProductDetails pd ON p.id = pd.ProductId
JOIN AtlasDB.dbo.CategoryProducts cp ON p.id = cp.ProductId
JOIN AtlasDB.dbo.Categories cat ON cp.CategoryId = cat.id;

SELECT 
    so.OrderNumber, 
    so.OrderDate,
    COALESCE(com.CompanyName, per.FirstName + ' ' + per.LastName) AS CustomerName,
    p.ProductName, 
    sod.Quantity, 
    sod.UnitPrice, 
    sod.Discount, 
    sod.SubTotal, 
    sod.TaxAmount, 
    sod.LineTotal
FROM AtlasDB.dbo.SalesOrderDetails sod
JOIN AtlasDB.dbo.SalesOrders so ON sod.OrderId = so.id
JOIN AtlasDB.dbo.Products p ON sod.ProductId = p.id
LEFT JOIN AtlasDB.dbo.CustomerCompany cc ON so.CustomerCompanyId = cc.id
LEFT JOIN AtlasDB.dbo.Companies com ON cc.CompanyId = com.id
LEFT JOIN AtlasDB.dbo.CustomerPerson cp ON so.CustomerPersonId = cp.id
LEFT JOIN AtlasDB.dbo.Persons per ON cp.PersonId = per.id;

SELECT 
    po.PONumber, 
    po.OrderStatus,
    COALESCE(com.CompanyName, per.FirstName + ' ' + per.LastName) AS VendorName,
    p.ProductName, 
    pod.Quantity, 
    pod.UnitPrice, 
    pod.LineTotal
FROM AtlasDB.dbo.PurchaseOrderDetails pod
JOIN AtlasDB.dbo.PurchaseOrders po ON pod.POId = po.id
JOIN AtlasDB.dbo.Products p ON pod.ProductId = p.id
LEFT JOIN AtlasDB.dbo.VendorsCompany vc ON po.VendorCompanyId = vc.id
LEFT JOIN AtlasDB.dbo.Companies com ON vc.CompanyId = com.id
LEFT JOIN AtlasDB.dbo.VendorsPerson vp ON po.VendorPersonId = vp.id
LEFT JOIN AtlasDB.dbo.Persons per ON vp.PersonId = per.id;

SELECT 
    l.Timestamp, 
    e.EmployeeNumber, 
    p.FirstName + ' ' + p.LastName AS EmployeeName, 
    l.Action
FROM AtlasDB.dbo.Logs l
JOIN AtlasDB.dbo.Employee e ON l.EmployeeId = e.id
JOIN AtlasDB.dbo.Persons p ON e.PersonId = p.id
ORDER BY l.Timestamp DESC;



