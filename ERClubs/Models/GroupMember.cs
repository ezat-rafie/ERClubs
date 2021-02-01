using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class GroupMember
    {
        public int ArtistIdGroup { get; set; }
        public int ArtistIdMember { get; set; }
        public DateTime? DateJoined { get; set; }
        public DateTime? DateLeft { get; set; }

        public virtual Artist ArtistIdGroupNavigation { get; set; }
        public virtual Artist ArtistIdMemberNavigation { get; set; }
    }
}
