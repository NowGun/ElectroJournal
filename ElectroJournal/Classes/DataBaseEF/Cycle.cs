using System;
using System.Collections.Generic;


namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Cycle
    {
        public Cycle()
        {
            Disciplines = new HashSet<Discipline>();
        }

        public uint Idcycle { get; set; }
        public string CyclуName { get; set; }
        public string CycleIndex { get; set; }

        public virtual ICollection<Discipline> Disciplines { get; set; }
    }
}
