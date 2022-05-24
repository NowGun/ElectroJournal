using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Smartcard
    {
        public Smartcard()
        {
            Presences = new HashSet<Presence>();
        }

        public uint Id { get; set; }
        public uint StudentId { get; set; }
        public string SmartcardIdentifier { get; set; }

        public virtual Student Student { get; set; }
        public virtual ICollection<Presence> Presences { get; set; }
    }
}
