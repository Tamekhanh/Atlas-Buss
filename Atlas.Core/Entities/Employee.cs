using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public int PersonId { get; set; }

        public Person? Person { get; set; }
        public EmployeeAccount? Account { get; set; }
        public ICollection<Products> Products { get; set; } = new List<Products>();

        [NotMapped]
        public string FullName => Person is null ? string.Empty : $"{Person.FirstName} {Person.LastName}".Trim();
    }

    public class EmployeeAccount
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }
        public bool CanProduct { get; set; }
        public bool CanSale { get; set; }
        public bool CanEmployee { get; set; }
        public bool CanInventory { get; set; }
        public bool CanAdministration { get; set; }
        public bool CanHR { get; set; }

        public Employee? Employee { get; set; }
    }
}
