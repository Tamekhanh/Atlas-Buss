using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Core.Entities
{
    public class Companies
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public int ContactId { get; set; }
        public Addresses? Address { get; set; }
        public Contacts? Contact { get; set; }

    }

    public class VendorCompany
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Companies? Company { get; set; }
    }

    public class CustomerCompany
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Companies? Company { get; set; }
    }
}
