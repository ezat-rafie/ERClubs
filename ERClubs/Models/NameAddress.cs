using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class NameAddress
    {
        public NameAddress()
        {
            Artist = new HashSet<Artist>();
        }

        public int NameAddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string ProvinceCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public virtual Province ProvinceCodeNavigation { get; set; }
        public virtual Club Club { get; set; }
        public virtual ICollection<Artist> Artist { get; set; }
    }
}
