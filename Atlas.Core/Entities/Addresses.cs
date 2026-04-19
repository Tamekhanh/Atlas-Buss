using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Core.Entities
{
    public class Addresses
    {
        public int Id { get; set; }
        public string AddressType {  get; set; } = string.Empty!;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
