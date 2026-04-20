namespace Atlas.Core.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DoB { get; set; }
        public int AddressId { get; set; }
        public int ContactId { get; set; }

        public Employee? Employee { get; set; }
        public Addresses? Address { get; set; }
        public Contacts? Contact { get; set; }
    }

    public class VendorPerson
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string? TaxId { get; set; }
        public Person? Person { get; set; }
    }

    public class CustomerPerson
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string? TaxId { get; set; }
        public Person? Person { get; set; }
    }
}