using System.Collections.Generic;

namespace ElectroJournal.Classes.DataBaseEJ
{
    public partial class Status
    {
        public Status()
        {
            Bugreporters = new HashSet<Bugreporter>();
        }

        public uint Idstatus { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Bugreporter> Bugreporters { get; set; }
    }
}
