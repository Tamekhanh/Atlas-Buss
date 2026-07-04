using Atlas.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure
{
    public class AtlasDBContext : DbContext
    {
        public AtlasDBContext(DbContextOptions<AtlasDBContext> options) : base(options) { }
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
        public DbSet<PricelistProductVariant> PricelistProductsVariants { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<ProductTax> ProductTaxes { get; set; }
        public DbSet<Units> Units { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public DbSet<AttributeType> AttributeTypes { get; set; } = null!;
        public DbSet<AttributeValue> AttributeValues { get; set; } = null!;
        public DbSet<VariantAttributeMapping> VariantAttributeMappings { get; set; } = null!;
        public DbSet<Party> Parties { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }


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

            modelBuilder.Entity<VariantAttributeMapping>(entity =>
            {
                entity.ToTable("VariantAttributeMappings", "dbo");

                // Thiết lập Khóa chính hỗn hợp (Composite Key)
                // Nếu thiếu dòng này, EF Core sẽ báo lỗi runtime
                entity.HasKey(vam => new { vam.VariantId, vam.AttributeValueId });

                entity.HasOne(vam => vam.ProductVariant)
                    .WithMany(v => v.AttributeMappings)
                    .HasForeignKey(vam => vam.VariantId);

                entity.HasOne(vam => vam.AttributeValue)
                    .WithMany(av => av.VariantMappings)
                    .HasForeignKey(vam => vam.AttributeValueId);
            });

            modelBuilder.Entity<ProductVariant>(entity =>
                {
                    entity.ToTable("ProductVariants", "dbo");
                    entity.HasKey(v => v.Id);
                    entity.Property(v => v.SKU).IsRequired().HasMaxLength(50);

                    // Quan hệ n-1 với Product
                    entity.HasOne(v => v.Product)
                        .WithMany(p => p.Variants)
                        .HasForeignKey(v => v.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity<AttributeType>(entity =>
                {
                    entity.ToTable("AttributeTypes", "dbo");
                    entity.HasKey(at => at.Id);
                    entity.Property(at => at.AttributeName).IsRequired().HasMaxLength(50);
                });

            modelBuilder.Entity<AttributeValue>(entity =>
            {
                entity.ToTable("AttributeValues", "dbo");
                entity.HasKey(av => av.Id);
                entity.Property(av => av.Value).IsRequired().HasMaxLength(50);

                entity.HasOne(av => av.AttributeType)
                    .WithMany(at => at.Values)
                    .HasForeignKey(av => av.AttributeTypeId);
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

            modelBuilder.Entity<Party>(entity =>
            {
                entity.ToTable("Parties", "dbo");
                entity.HasKey(party => party.Id);

                // Cấu hình các trường dữ liệu
                entity.Property(party => party.PartyType)
                      .HasMaxLength(20)
                      .IsRequired(); // Chứa giá trị "Person" hoặc "Company"

                entity.Property(party => party.DisplayName)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(party => party.FirstName).HasMaxLength(50);
                entity.Property(party => party.LastName).HasMaxLength(50);
                entity.Property(party => party.TaxId).HasMaxLength(20);

                // Đảm bảo TaxId là duy nhất (nếu có nhập)
                entity.HasIndex(party => party.TaxId)
                      .IsUnique()
                      .HasFilter("[TaxId] IS NOT NULL");

                // Cấu hình các giá trị mặc định cho Cờ (Flags)
                entity.Property(party => party.IsCustomer).HasDefaultValue(false);
                entity.Property(party => party.IsVendor).HasDefaultValue(false);
                entity.Property(party => party.IsDeleted).HasDefaultValue(false);
                entity.Property(party => party.CreatedAt).HasDefaultValueSql("GETDATE()");

                // Cấu hình Khóa ngoại
                entity.HasOne(party => party.Address)
                    .WithMany()
                    .HasForeignKey(party => party.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(party => party.Contact)
                    .WithMany()
                    .HasForeignKey(party => party.ContactId)
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
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EmployeeNumber).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.EmployeeNumber).IsUnique();

                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(e => e.Address)
                    .WithMany()
                    .HasForeignKey(e => e.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Contact)
                    .WithMany()
                    .HasForeignKey(e => e.ContactId)
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

            modelBuilder.Entity<Images>(entity =>
            {
                entity.ToTable("Images", "dbo");
                entity.HasKey(i => i.Id);
                entity.Property(i => i.ImageUrl).IsRequired().HasMaxLength(255);
                entity.Property(i => i.CreatedAt).HasDefaultValueSql("GETDATE()");
            });


            modelBuilder.Entity<ProductImages>(entity =>
            {
                entity.ToTable("ProductImages", "dbo");

                entity.HasKey(pi => new { pi.ProductId, pi.ImageId });

                entity.HasOne(pi => pi.Product)
                    .WithMany(p => p.ProductImages) // Phải khớp với ICollection<ProductImages> trong class Products
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(pi => pi.Image)
                    .WithMany(i => i.ProductImages) // Phải khớp với ICollection<ProductImages> trong class Images
                    .HasForeignKey(pi => pi.ImageId)
                    .OnDelete(DeleteBehavior.Cascade);
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

    // CHUẨN HÓA: Trỏ về Party
    entity.HasOne(order => order.Customer)
        .WithMany(party => party.SalesOrders)
        .HasForeignKey(order => order.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(order => order.OrderStatus)
        .WithMany(status => status.SalesOrders)
        .HasForeignKey(order => order.OrderStatusId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(order => order.Currency)
        .WithMany(c => c.SalesOrders)
        .HasForeignKey(order => order.CurrencyId)
        .OnDelete(DeleteBehavior.Restrict);
});

            modelBuilder.Entity<SalesOrderStatuses>(entity =>
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

                // CHUẨN HÓA: Trỏ về Variant
                entity.HasOne(detail => detail.Variant)
                    .WithMany()
                    .HasForeignKey(detail => detail.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(detail => detail.Warehouse)
                    .WithMany()
                    .HasForeignKey(detail => detail.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // 3. CẤU HÌNH PURCHASE ORDER
            // ==========================================

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

                // CHUẨN HÓA: Trỏ về Party
                entity.HasOne(order => order.Vendor)
                    .WithMany(party => party.PurchaseOrders)
                    .HasForeignKey(order => order.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.OrderStatus)
                    .WithMany(status => status.PurchaseOrders)
                    .HasForeignKey(order => order.OrderStatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(order => order.Currency)
                    .WithMany(c => c.PurchaseOrders)
                    .HasForeignKey(order => order.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseOrderStatuses>(entity =>
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

                // CHUẨN HÓA: Trỏ về Variant
                entity.HasOne(detail => detail.Variant)
                    .WithMany()
                    .HasForeignKey(detail => detail.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(detail => detail.Warehouse)
                    .WithMany()
                    .HasForeignKey(detail => detail.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LogDetail>(entity =>
            {
                entity.ToTable("LogsDetails", "dbo");

                // Khóa chính
                entity.HasKey(ld => ld.LogId);

                // Cấu hình quan hệ 1-1 với Logs
                entity.HasOne(ld => ld.Log)
                    .WithOne(l => l.LogDetail)
                    .HasForeignKey<LogDetail>(ld => ld.LogId)
                    .OnDelete(DeleteBehavior.Cascade); // Xóa Log thì tự động xóa LogDetail
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

                // 1. SỬA TẠI ĐÂY: Đổi ProductId thành VariantId cho Khóa chính phức hợp
                entity.HasKey(stock => new { stock.WarehouseId, stock.VariantId });

                entity.Property(stock => stock.ReservedQuantity).HasDefaultValue(0);
                entity.Property(stock => stock.LastUpdated).HasDefaultValueSql("GETDATE()");

                entity.HasOne(stock => stock.Warehouse)
                    .WithMany(warehouse => warehouse.InventoryStocks)
                    .HasForeignKey(stock => stock.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. SỬA TẠI ĐÂY: Trỏ Khóa ngoại về Variant thay vì Product
                entity.HasOne(stock => stock.Variant)
                    .WithMany(v => v.InventoryStocks) // Liên kết ngược lại danh sách trong ProductVariant
                    .HasForeignKey(stock => stock.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.ToTable("InventoryTransactions", "dbo");
                entity.HasKey(transaction => transaction.Id);
                entity.Property(transaction => transaction.Id).ValueGeneratedOnAdd();

                entity.Property(transaction => transaction.ReferenceId).HasMaxLength(50);
                entity.Property(transaction => transaction.Note).HasMaxLength(255);
                entity.Property(transaction => transaction.TransactionDate).HasDefaultValueSql("GETDATE()");

                // 1. SỬA ĐỔI: Trỏ tới Variant thay vì Product
                entity.HasOne(transaction => transaction.Variant)
                    .WithMany(v => v.InventoryTransactions)
                    .HasForeignKey(transaction => transaction.VariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(transaction => transaction.Warehouse)
                    .WithMany(w => w.InventoryTransactions)
                    .HasForeignKey(transaction => transaction.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(transaction => transaction.Employee)
                    .WithMany()
                    .HasForeignKey(transaction => transaction.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. BỔ SUNG: Khóa ngoại cho Lookup Table TransactionTypes
                entity.HasOne(transaction => transaction.TransactionType)
                    .WithMany(t => t.InventoryTransactions)
                    .HasForeignKey(transaction => transaction.TransactionTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories", "dbo");
                entity.HasKey(category => category.Id);
                entity.Property(category => category.CategoryName).HasMaxLength(100).IsRequired();
                entity.Property(category => category.CategoryDesc).HasMaxLength(255);
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
                    .WithMany(product => product.CategoryProducts)
                    .HasForeignKey(cp => cp.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Pricelist>(entity =>
            {
                entity.ToTable("Pricelist", "dbo");
                entity.HasKey(pricelist => pricelist.Id);
                entity.Property(pricelist => pricelist.EffectiveDate).HasColumnType("date");
                entity.Property(pricelist => pricelist.ExpiryDate).HasColumnType("date");

                // CHUẨN HÓA: Trỏ về Party thay vì VendorCompany/Person
                entity.HasOne(pricelist => pricelist.Vendor)
                    .WithMany()
                    .HasForeignKey(pricelist => pricelist.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pricelist => pricelist.Currency)
                    .WithMany(c => c.Pricelists)
                    .HasForeignKey(pricelist => pricelist.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PricelistProductVariant>(entity =>
            {
                entity.ToTable("PricelistProductVariant", "dbo"); // Đảm bảo tên bảng khớp SQL
                entity.HasKey(pp => pp.Id);

                entity.HasOne(pp => pp.Pricelist)
                    .WithMany(pricelist => pricelist.PricelistVariants)
                    .HasForeignKey(pp => pp.PricelistId)
                    .OnDelete(DeleteBehavior.Cascade);

                // CHUẨN HÓA: Trỏ về Variant
                entity.HasOne(pp => pp.Variant)
                    .WithMany()
                    .HasForeignKey(pp => pp.VariantId)
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
