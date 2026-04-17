CREATE DATABASE AtlasDB
GO

use AtlasDB
GO

-- General Information
-----------------------------------
CREATE TABLE AtlasDB.dbo.Addresses(
    id int identity(1,1) primary key,
    AddressType varchar(50),
    Street varchar(255),
    City varchar(100),
    State varchar(100),
    Country varchar(100)
)
GO

CREATE TABLE AtlasDB.dbo.Contacts(
	id int identity(1,1) primary key,
	Phone varchar(20),
	Email varchar(50) null
)
GO

CREATE TABLE AtlasDB.dbo.Persons(
	id int identity(1,1) primary key,
	FirstName varchar(50) not null,
	LastName varchar(50) not null,
	DoB date not null,
    AddressId int not null,
	FOREIGN KEY (AddressId) REFERENCES AtlasDB.dbo.Addresses(id),
    ContactId int not null,
    FOREIGN KEY (ContactId) REFERENCES AtlasDB.dbo.Contacts(id)
)
GO

-- expand business operations
-----------------------------------
CREATE TABLE AtlasDB.dbo.Companies(
    id int identity(1,1) primary key,
    CompanyName varchar(100) not null,
    TaxId varchar(20) not null UNIQUE,
    AddressId int not null,
    FOREIGN KEY (AddressId) REFERENCES AtlasDB.dbo.Addresses(id),
    ContactId int not null,
    FOREIGN KEY (ContactId) REFERENCES AtlasDB.dbo.Contacts(id)
)
GO

CREATE TABLE AtlasDB.dbo.VendorsCompany(
    id int identity(1,1) primary key,
    CompanyId int not null UNIQUE,
    FOREIGN KEY (CompanyId) REFERENCES AtlasDB.dbo.Companies(id)
)
GO

CREATE TABLE AtlasDB.dbo.VendorsPerson(
    id int identity(1,1) primary key,
    PersonId int not null UNIQUE,
    FOREIGN KEY (PersonId) REFERENCES AtlasDB.dbo.Persons(id)
)
GO

CREATE TABLE AtlasDB.dbo.CustomerCompany(
    id int identity(1,1) primary key,
    CompanyId int not null UNIQUE,
    FOREIGN KEY (CompanyId) REFERENCES AtlasDB.dbo.Companies(id)
)
GO

CREATE TABLE AtlasDB.dbo.CustomerPerson(
    id int identity(1,1) primary key,
    PersonId int not null UNIQUE,
    FOREIGN KEY (PersonId) REFERENCES AtlasDB.dbo.Persons(id)
)
GO

-- Employee Management
-----------------------------------
CREATE TABLE AtlasDB.dbo.Employee(
    id int identity(1,1) primary key,
    EmployeeNumber varchar(20) not null UNIQUE,
    PersonId int not null UNIQUE,
    FOREIGN KEY (PersonId) REFERENCES AtlasDB.dbo.Persons(id)
)
GO

CREATE TABLE AtlasDB.dbo.Auth(
    idEmp int primary key,
    iProduct bit not null default 0,
    iSale bit not null default 0,
    iEmployee bit not null default 0,
    iInventory bit not null default 0,
    iAdministration bit not null default 0,
    iHR bit not null default 0,
    FOREIGN KEY (idEmp) REFERENCES AtlasDB.dbo.Employee(id)
)
GO

CREATE TABLE AtlasDB.dbo.Accounts(
    id int identity(1,1) primary key,
    EmployeeId int not null UNIQUE,
    Username varchar(50) not null UNIQUE,
    PasswordHash varchar(255) not null,
    IsActive bit not null default 1,
    LastLogin datetime null,
    FOREIGN KEY (EmployeeId) REFERENCES AtlasDB.dbo.Employee(id)
)
GO

CREATE TABLE AtlasDB.dbo.Departments(
    id int identity(1,1) primary key,
    DepartmentName varchar(100) not null UNIQUE,
    Description varchar(255) null
)
GO

CREATE TABLE AtlasDB.dbo.EmployeeDepartments(
    EmployeeId int not null,
    DepartmentId int not null,
    PRIMARY KEY (EmployeeId, DepartmentId),
    FOREIGN KEY (EmployeeId) REFERENCES AtlasDB.dbo.Employee(id),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(id)
)
GO