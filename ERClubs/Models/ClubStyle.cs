using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class ClubStyle
    {
        public int ClubId { get; set; }
        public string StyleName { get; set; }

        public virtual Club Club { get; set; }
        public virtual Style StyleNameNavigation { get; set; }
    }
}
