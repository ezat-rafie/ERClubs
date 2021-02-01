using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class Contract
    {
        public int Contract1 { get; set; }
        public DateTime StartDate { get; set; }
        public int ArtistId { get; set; }
        public int ClubId { get; set; }
        public double PricePerPerformance { get; set; }
        public int NumberPerformances { get; set; }
        public double TotalPrice { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual Club Club { get; set; }
    }
}
