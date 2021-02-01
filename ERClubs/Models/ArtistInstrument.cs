using System;
using System.Collections.Generic;

namespace ERClubs.Models
{
    public partial class ArtistInstrument
    {
        public int ArtistInstrumentId { get; set; }
        public int ArtistId { get; set; }
        public int InstrumentId { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual Instrument Instrument { get; set; }
    }
}
