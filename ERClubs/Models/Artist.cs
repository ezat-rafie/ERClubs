using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class Artist
    {
        public Artist()
        {
            ArtistInstrument = new HashSet<ArtistInstrument>();
            ArtistStyle = new HashSet<ArtistStyle>();
            Contract = new HashSet<Contract>();
            GroupMemberArtistIdGroupNavigation = new HashSet<GroupMember>();
            GroupMemberArtistIdMemberNavigation = new HashSet<GroupMember>();
        }

        public int ArtistId { get; set; }
        public double MinimumHourlyRate { get; set; }
        public int NameAddressid { get; set; }

        public virtual NameAddress NameAddress { get; set; }
        public virtual ICollection<ArtistInstrument> ArtistInstrument { get; set; }
        public virtual ICollection<ArtistStyle> ArtistStyle { get; set; }
        public virtual ICollection<Contract> Contract { get; set; }
        public virtual ICollection<GroupMember> GroupMemberArtistIdGroupNavigation { get; set; }
        public virtual ICollection<GroupMember> GroupMemberArtistIdMemberNavigation { get; set; }
    }
}
