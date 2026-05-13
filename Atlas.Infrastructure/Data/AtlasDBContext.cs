using Atlas.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure
{
    public class AtlasDBContext : DbContext
    {
        public AtlasDBContext(DbContextOptions<AtlasDBContext> options) : base(options) { }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Addresses> Addresses { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAccount> EmployeeAccounts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeeDepartment> EmployeeDepartments { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<VendorCompany> VendorCompanies { get; set; }
        public DbSet<CustomerCompany> CustomerCompanies { get; set; }
        public DbSet<VendorPerson> VendorPersons { get; set; }
        public DbSet<CustomerPerson> CustomerPersons { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<SalesOrderStatus> SalesOrderStatuses { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<PurchaseOrderStatus> PurchaseOrderStatuses { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<InventoryStock> InventoryStocks { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryPricelist> CategoryPricelists { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<Pricelist> Pricelists { get; set; }
        public DbSet<PricelistProduct> PricelistProducts { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<ProductTax> ProductTaxes { get; set; }
        public DbSet<Units> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Logs", "dbo");
                entity.HasKey(log => log.Id);
                entity.Property(log => log.Action).HasMaxLength(255).IsRequired();
                entity.Property(log => log.Timestamp).HasDefaultValueSql("GETDATE()");

                entity.HasOne(log => log.Employee)
                    .WithMany()
                    .HasForeignKey(log => log.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", "dbo");
                entity.HasKey(role => role.Id);
                entity.Property(role => role.RoleName).HasMaxLength(50).IsRequired();
                entity.Property(role => role.Description).HasMaxLength(255);
                entity.HasIndex(role => role.RoleName).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions", "dbo");
                entity.HasKey(permission => permission.Id);
                entity.Property(permission => permission.PermissionKey).HasMaxLength(100).IsRequired();
                entity.Property(permission => permission.Description).HasMaxLength(255);
                entity.HasIndex(permission => permission.PermissionKey).IsUnique();
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions", "dbo");
                entity.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });

                entity.HasOne(rolePermission => rolePermission.Role)
                    .WithMany(role => role.RolePermissions)
                    .HasForeignKey(rolePermission => rolePermission.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rolePermission => rolePermission.Permission)
                    .WithMany(permission => permission.RolePermissions)
                    .HasForeignKey(rolePermission => rolePermission.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Persons", "dbo");
                entity.HasKey(person => person.Id);
                entity.Property(person => person.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(person => person.LastName).HasMaxLength(50).IsRequired();
                entity.Property(person => person.IsDeleted).HasDefaultValue(false);

                entity.HasOne(person => person.Address)
                    .WithMany()
                    .HasForeignKey(person => person.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(person => person.Contact)
                    .WithMany()
                    .HasForeignKey(person => person.ContactId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Companies>(entity =>
            {
                entity.ToTable("Companies", "dbo");
                entity.HasKey(company => company.Id);
                entity.Property(company => company.CompanyName).HasMaxLength(100).IsRequired();
                entity.Property(company => company.TaxId).HasMaxLength(20).IsRequired();
                entity.HasOne(company => company.Address)
                    .WithMany()
                    .HasForeignKey(company => company.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(company => company.Contact)
                    .WithMany()
                    .HasForeignKey(company => company.ContactId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VendorCompany>(entity =>
            {
                entity.ToTable("VendorsCompany", "dbo");
                entity.HasKey(vendorCompany => vendorCompany.Id);
                entity.HasOne(vendorCompany => vendorCompany.Company)
                    .WithMany()
                    .HasForeignKey(vendorCompany => vendorCompany.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CustomerCompany>(entity =>
            {
                entity.ToTable("CustomerCompany", "dbo");
                entity.HasKey(customerCompany => customerCompany.Id);
                entity.HasOne(customerCompany => customerCompany.Company)
                    .WithMany()
                    .HasForeignKey(customerCompany => customerCompany.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VendorPerson>(entity =>
            {
                entity.ToTable("VendorsPerson", "dbo");
                entity.HasKey(vendorPerson => vendorPerson.Id);
                entity.Property(vendorPerson => vendorPerson.TaxId).HasMaxLength(20);
                entity.HasOne(vendorPerson => vendorPerson.Person)
                    .WithMany()
                    .HasForeignKey(vendorPerson => vendorPerson.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CustomerPerson>(entity =>
            {
                entity.ToTable("CustomerPerson", "dbo");
                entity.HasKey(customerPerson => customerPerson.Id);
                entity.Property(customerPerson => customerPerson.TaxId).HasMaxLength(20);
                entity.HasOne(customerPerson => customerPerson.Person)
                    .WithMany()
                    .HasForeignKey(customerPerson => customerPerson.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.ToTable("Contacts", "dbo");
                entity.HasKey(contact => contact.Id);
                entity.Property(contact => contact.Phone).HasMaxLength(20);
                entity.Property(contact => contact.Email).HasMaxLength(50);
                entity.Property(contact => contact.IsDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.ToTable("Addresses", "dbo");
                entity.HasKey(address => address.Id);
                entity.Property(address => address.Street).HasMaxLength(255);
                entity.Property(address => address.City).HasMaxLength(100);
                entity.Property(address => address.State).HasMaxLength(100);
                entity.Property(address => address.IsDeleted).HasDefaultValue(false);
                entity.Property(address => address.Country).HasMaxLength(100);
            });

            modelBuilder.Entity<Units>(entity =>
            {
                entity.ToTable("Units", "dbo");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.UnitName).HasMaxLength(50).IsRequired();
                entity.Property(u => u.ShortName).HasMaxLength(10);
                entity.HasIndex(u => u.UnitName).IsUnique();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee", "dbo");
                entity.HasKey(employee => employee.Id);
                entity.Property(employee => employee.IsDeleted).HasDefaultValue(false);
                entity.Property(employee => employee.EmployeeNumber).HasMaxLength(20).IsRequired();

                entity.HasOne(employee => employee.Person)
                    .WithOne(person => person.Employee)
                    .HasForeignKey<Employee>(employee => employee.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeeAccount>(entity =>
            {
                entity.ToTable("EmployeeAccounts", "dbo");
                entity.HasKey(employeeAccount => employeeAccount.EmployeeId);

                entity.Property(employeeAccount => employeeAccount.Username)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(employeeAccount => employeeAccount.PasswordHash)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired();

                entity.Property(employeeAccount => employeeAccount.IsActive)
                    .HasColumnName("IsActive")
                    .HasDefaultValue(true);

                entity.Property(employeeAccount => employeeAccount.LastLogin)
                    .HasColumnName("LastLogin");

                entity.Property(employeeAccount => employeeAccount.RoleId)
                    .HasColumnName("RoleId");

                entity.HasOne(employeeAccount => employeeAccount.Employee)
                    .WithOne(employee => employee.Account)
                    .HasForeignKey<EmployeeAccount>(employeeAccount => employeeAccount.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(employeeAccount => employeeAccount.Role)
                    .WithMany(role => role.EmployeeAccounts)
                    .HasForeignKey(employeeAccount => employeeAccount.RoleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.ToTable("Products", "dbo");
                entity.HasKey(product => product.Id);
                entity.Property(product => product.IsDeleted).HasDefaultValue(false);
                entity.Property(product => product.IsActive).HasColumnName("isActive");
                entity.Property(product => product.Onsale).HasColumnName("Onsale");

                entity.Property(product => product.UnitId).IsRequired();
                entity.Property(product => product.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(product => product.UpdatedAt).IsRequired(false);

                entity.HasOne(product => product.Employee)
                    .WithMany(employee => employee.Products)
                    .HasForeignKey(product => product.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(product => product.Unit)
                    .WithMany()
                    .HasForeignKey(product => product.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductDetails>(entity =>
            {
                entity.ToTable("ProductDetails", "dbo");
                entity.HasKey(productDetail => productDetail.ProductId);
                entity.Property(productDetail => productDetail.ProductDescription).HasColumnType("nvarchar(max)");
                entity.Property(productDetail => productDetail.Dimensions).HasMaxLength(50);
                entity.Property(productDetail => productDetail.Manufacturer).HasMaxLength(100);

                entity.HasOne(productDetail => productDetail.Product)
                    .WithOne(product => product.ProductDetail)
                    .HasForeignKey<ProductDetails>(productDetail => productDetail.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments", "dbo");
                entity.HasKey(department => department.Id);
                entity.Property(department => department.DepartmentName).HasMaxLength(100).IsRequired();
                entity.Property(department => department.Description).HasMaxLength(255);

                entity.HasOne(department => department.ParentDepartment)
                    .WithMany(department => department.ChildDepartments)
                    .HasForeignKey(department => department.ParentDepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeeDepartment>(entity =>
            {
                entity.ToTable("EmployeeDepartments", "dbo");
                entity.HasKey(ed => new { ed.EmployeeId, ed.DepartmentId });

                entity.HasOne(ed => ed.Employee)
                    .WithMany(employee => employee.EmployeeDepartments)
                    .HasForeignKey(ed => ed.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ed => ed.Department)
                    .WithMany(department => department.EmployeeDepartments)
                    .HasForeignKey(ed => ed.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.ToTable("SalesOrders", "dbo");
                entity.HasKey(order => order.Id);
                entity.Property(order => order.OrderNumber).HasMaxLength(50).IsRequired();
                entity.Property(order => order.OrderStatusId).HasDefaultValue(1);
                entity.Property(order => order.OrderDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(order => order.Employee)
                    .WithMany()
                    .HasForeignKey(order => order.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.CustomerCompany)
                    .WithMany()
                    .HasForeignKey(order => order.CustomerCompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.CustomerPerson)
                    .WithMany()
                    .HasForeignKey(order => order.CustomerPersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.OrderStatus)
                    .WithMany(status => status.SalesOrders)
                    .HasForeignKey(order => order.OrderStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SalesOrderStatus>(entity =>
            {
                entity.ToTable("SalesOrderStatuses", "dbo");
                entity.HasKey(status => status.Id);
                entity.Property(status => status.StatusName).HasMaxLength(50).IsRequired();
                entity.Property(status => status.Description).HasMaxLength(255);
                entity.HasIndex(status => status.StatusName).IsUnique();
            });

            modelBuilder.Entity<SalesOrderDetail>(entity =>
            {
                entity.ToTable("SalesOrderDetails", "dbo");
                entity.HasKey(detail => detail.Id);

                entity.HasOne(detail => detail.SalesOrder)
                    .WithMany(order => order.SalesOrderDetails)
                    .HasForeignKey(detail => detail.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(detail => detail.Product)
                    .WithMany()
                    .HasForeignKey(detail => detail.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(detail => detail.Warehouse)
                    .WithMany()
                    .HasForeignKey(detail => detail.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("PurchaseOrders", "dbo");
                entity.HasKey(order => order.Id);
                entity.Property(order => order.PONumber).HasMaxLength(50).IsRequired();
                entity.Property(order => order.OrderStatusId).HasDefaultValue(1);
                entity.Property(order => order.OrderDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(order => order.Employee)
                    .WithMany()
                    .HasForeignKey(order => order.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.VendorCompany)
                    .WithMany()
                    .HasForeignKey(order => order.VendorCompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.VendorPerson)
                    .WithMany()
                    .HasForeignKey(order => order.VendorPersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.OrderStatus)
                    .WithMany(status => status.PurchaseOrders)
                    .HasForeignKey(order => order.OrderStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseOrderStatus>(entity =>
            {
                entity.ToTable("PurchaseOrderStatuses", "dbo");
                entity.HasKey(status => status.Id);
                entity.Property(status => status.StatusName).HasMaxLength(50).IsRequired();
                entity.Property(status => status.Description).HasMaxLength(255);
                entity.HasIndex(status => status.StatusName).IsUnique();
            });

            modelBuilder.Entity<PurchaseOrderDetail>(entity =>
            {
                entity.ToTable("PurchaseOrderDetails", "dbo");
                entity.HasKey(detail => detail.Id);

                entity.HasOne(detail => detail.PurchaseOrder)
                    .WithMany(order => order.PurchaseOrderDetails)
                    .HasForeignKey(detail => detail.POId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(detail => detail.Product)
                    .WithMany()
                    .HasForeignKey(detail => detail.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(detail => detail.Warehouse)
                    .WithMany()
                    .HasForeignKey(detail => detail.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices", "dbo");
                entity.HasKey(invoice => invoice.Id);
                entity.Property(invoice => invoice.InvoiceNumber).HasMaxLength(50).IsRequired();
                entity.Property(invoice => invoice.InvoiceDate).HasDefaultValueSql("GETDATE()");
                entity.Property(invoice => invoice.DueDate).HasColumnType("date");
                entity.Property(invoice => invoice.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(invoice => invoice.IsPaid).HasDefaultValue(false);
                entity.HasIndex(invoice => invoice.InvoiceNumber).IsUnique();

                entity.HasOne(invoice => invoice.SalesOrder)
                    .WithMany()
                    .HasForeignKey(invoice => invoice.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments", "dbo");
                entity.HasKey(payment => payment.Id);
                entity.Property(payment => payment.PaymentDate).HasDefaultValueSql("GETDATE()");
                entity.Property(payment => payment.Amount).HasColumnType("decimal(18,2)");
                entity.Property(payment => payment.PaymentMethod).HasMaxLength(50);
                entity.Property(payment => payment.Note).HasMaxLength(255);

                entity.HasOne(payment => payment.Invoice)
                    .WithMany(invoice => invoice.Payments)
                    .HasForeignKey(payment => payment.InvoiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("Warehouses", "dbo");
                entity.HasKey(warehouse => warehouse.Id);
                entity.Property(warehouse => warehouse.WarehouseName).HasMaxLength(100).IsRequired();

                entity.HasOne(warehouse => warehouse.Address)
                    .WithMany()
                    .HasForeignKey(warehouse => warehouse.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(warehouse => warehouse.Manager)
                    .WithMany()
                    .HasForeignKey(warehouse => warehouse.ManagerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<InventoryStock>(entity =>
            {
                entity.ToTable("InventoryStock", "dbo");
                entity.HasKey(stock => new { stock.WarehouseId, stock.ProductId });

                entity.Property(stock => stock.ReservedQuantity).HasDefaultValue(0);
                entity.Property(stock => stock.LastUpdated).HasDefaultValueSql("GETDATE()");

                entity.HasOne(stock => stock.Warehouse)
                    .WithMany(warehouse => warehouse.InventoryStocks)
                    .HasForeignKey(stock => stock.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(stock => stock.Product)
                    .WithMany()
                    .HasForeignKey(stock => stock.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.ToTable("InventoryTransactions", "dbo");
                entity.HasKey(transaction => transaction.Id);
                entity.Property(transaction => transaction.Id).ValueGeneratedOnAdd();
                entity.Property(transaction => transaction.TransactionType).HasMaxLength(50);
                entity.Property(transaction => transaction.ReferenceId).HasMaxLength(50);
                entity.Property(transaction => transaction.Note).HasMaxLength(255);
                entity.Property(transaction => transaction.TransactionDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(transaction => transaction.Product)
                    .WithMany()
                    .HasForeignKey(transaction => transaction.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(transaction => transaction.Warehouse)
                    .WithMany()
                    .HasForeignKey(transaction => transaction.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(transaction => transaction.Employee)
                    .WithMany()
                    .HasForeignKey(transaction => transaction.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories", "dbo");
                entity.HasKey(category => category.Id);
                entity.Property(category => category.CategoryName).HasMaxLength(100).IsRequired();
                entity.Property(category => category.CategoryDesc).HasMaxLength(255);
            });

            modelBuilder.Entity<CategoryPricelist>(entity =>
            {
                entity.ToTable("CategoryPricelist", "dbo");
                entity.HasKey(cp => cp.Id);

                entity.HasOne(cp => cp.Category)
                    .WithMany(category => category.CategoryPricelists)
                    .HasForeignKey(cp => cp.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.Pricelist)
                    .WithMany(pricelist => pricelist.CategoryPricelists)
                    .HasForeignKey(cp => cp.PricelistId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.ToTable("CategoryProducts", "dbo");
                entity.HasKey(cp => new { cp.CategoryId, cp.ProductId });

                entity.HasOne(cp => cp.Category)
                    .WithMany(category => category.CategoryProducts)
                    .HasForeignKey(cp => cp.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.Product)
                    .WithMany()
                    .HasForeignKey(cp => cp.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Pricelist>(entity =>
            {
                entity.ToTable("Pricelist", "dbo");
                entity.HasKey(pricelist => pricelist.Id);
                entity.Property(pricelist => pricelist.EffectiveDate).HasColumnType("date");
                entity.Property(pricelist => pricelist.ExpiryDate).HasColumnType("date");

                entity.HasOne(pricelist => pricelist.VendorCompany)
                    .WithMany()
                    .HasForeignKey(pricelist => pricelist.VendorCompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pricelist => pricelist.VendorPerson)
                    .WithMany()
                    .HasForeignKey(pricelist => pricelist.VendorPersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PricelistProduct>(entity =>
            {
                entity.ToTable("PricelistProduct", "dbo");
                entity.HasKey(pp => pp.Id);

                entity.HasOne(pp => pp.Pricelist)
                    .WithMany(pricelist => pricelist.PricelistProducts)
                    .HasForeignKey(pp => pp.PricelistId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pp => pp.Product)
                    .WithMany()
                    .HasForeignKey(pp => pp.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tax>(entity =>
            {
                entity.ToTable("Taxes", "dbo");
                entity.HasKey(tax => tax.Id);
                entity.Property(tax => tax.TaxName).HasMaxLength(50).IsRequired();
                entity.Property(tax => tax.Description).HasMaxLength(255);
                entity.Property(tax => tax.TaxRate).HasColumnType("decimal(18,4)");
            });

            modelBuilder.Entity<ProductTax>(entity =>
            {
                entity.ToTable("ProductTaxes", "dbo");
                entity.HasKey(pt => new { pt.ProductId, pt.TaxId });

                entity.HasOne(pt => pt.Product)
                    .WithMany(product => product.ProductTaxes)
                    .HasForeignKey(pt => pt.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pt => pt.Tax)
                    .WithMany(tax => tax.ProductTaxes)
                    .HasForeignKey(pt => pt.TaxId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
