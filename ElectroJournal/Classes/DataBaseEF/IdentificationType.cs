using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class IdentificationType
    {
        public IdentificationType()
        {
            Presences = new HashSet<Presence>();
        }

        public uint Id { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Presence> Presences { get; set; }
    }
}
