using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class Instrument
    {
        public Instrument()
        {
            ArtistInstrument = new HashSet<ArtistInstrument>();
        }

        public int InstrumentId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ArtistInstrument> ArtistInstrument { get; set; }
    }
}
