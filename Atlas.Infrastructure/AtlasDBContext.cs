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
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAccount> EmployeeAccounts { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<VendorCompany> VendorCompanies { get; set; }
        public DbSet<CustomerCompany> CustomerCompanies { get; set; }
        public DbSet<VendorPerson> VendorPersons { get; set; }
        public DbSet<CustomerPerson> CustomerPersons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Persons", "dbo");
                entity.HasKey(person => person.Id);
                entity.Property(person => person.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(person => person.LastName).HasMaxLength(50).IsRequired();

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
            });

            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.ToTable("Addresses", "dbo");
                entity.HasKey(address => address.Id);
                entity.Property(address => address.Street).HasMaxLength(255).IsRequired();
                entity.Property(address => address.City).HasMaxLength(100).IsRequired();
                entity.Property(address => address.State).HasMaxLength(100).IsRequired();
                entity.Property(address => address.Country).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee", "dbo");
                entity.HasKey(employee => employee.Id);
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
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(employeeAccount => employeeAccount.IsActive)
                    .HasColumnName("IsActive")
                    .HasDefaultValue(true);

                entity.Property(employeeAccount => employeeAccount.LastLogin)
                    .HasColumnName("LastLogin");

                entity.Property(employeeAccount => employeeAccount.CanProduct).HasColumnName("canProduct");
                entity.Property(employeeAccount => employeeAccount.CanSale).HasColumnName("canSale");
                entity.Property(employeeAccount => employeeAccount.CanEmployee).HasColumnName("canEmployee");
                entity.Property(employeeAccount => employeeAccount.CanInventory).HasColumnName("canInventory");
                entity.Property(employeeAccount => employeeAccount.CanAdministration).HasColumnName("canAdministration");
                entity.Property(employeeAccount => employeeAccount.CanHR).HasColumnName("canHR");

                entity.HasOne(employeeAccount => employeeAccount.Employee)
                    .WithOne(employee => employee.Account)
                    .HasForeignKey<EmployeeAccount>(employeeAccount => employeeAccount.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.ToTable("Products", "dbo");
                entity.HasKey(product => product.Id);
                entity.Property(product => product.IsActive).HasColumnName("isActive");
                entity.Property(product => product.Onsale).HasColumnName("Onsale");

                entity.HasOne(product => product.Employee)
                    .WithMany(employee => employee.Products)
                    .HasForeignKey(product => product.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductDetails>(entity =>
            {
                entity.ToTable("ProductDetails", "dbo");
                entity.HasKey(productDetail => productDetail.ProductId);
                entity.Property(productDetail => productDetail.ProductDescription).HasMaxLength(255);
                entity.Property(productDetail => productDetail.Dimensions).HasMaxLength(50);
                entity.Property(productDetail => productDetail.Manufacturer).HasMaxLength(100);

                entity.HasOne(productDetail => productDetail.Product)
                    .WithOne(product => product.ProductDetail)
                    .HasForeignKey<ProductDetails>(productDetail => productDetail.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
