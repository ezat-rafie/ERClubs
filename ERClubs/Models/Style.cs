using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class Style
    {
        public Style()
        {
            ArtistStyle = new HashSet<ArtistStyle>();
            ClubStyle = new HashSet<ClubStyle>();
        }

        public string StyleName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ArtistStyle> ArtistStyle { get; set; }
        public virtual ICollection<ClubStyle> ClubStyle { get; set; }
    }
}
