using Atlas.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure
{
    public class AtlasDBContext : DbContext
    {
        public AtlasDBContext(DbContextOptions<AtlasDBContext> options) : base(options) { }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Contracts> Contacts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }

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

            modelBuilder.Entity<Contracts>(entity =>
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
