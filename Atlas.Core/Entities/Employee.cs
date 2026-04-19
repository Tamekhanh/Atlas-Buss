using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public int PersonId { get; set; }

        public Person? Person { get; set; }
        public ICollection<Products> Products { get; set; } = new List<Products>();

        [NotMapped]
        public string FullName => Person is null ? string.Empty : $"{Person.FirstName} {Person.LastName}".Trim();
    }
}
