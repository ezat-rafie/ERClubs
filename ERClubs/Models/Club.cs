using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class Club
    {
        public Club()
        {
            ClubStyle = new HashSet<ClubStyle>();
            Contract = new HashSet<Contract>();
        }

        public int ClubId { get; set; }

        public virtual NameAddress ClubNavigation { get; set; }
        public virtual ICollection<ClubStyle> ClubStyle { get; set; }
        public virtual ICollection<Contract> Contract { get; set; }
    }
}
