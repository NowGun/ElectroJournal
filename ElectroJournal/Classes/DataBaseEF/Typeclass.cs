using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Typeclass
    {
        public Typeclass()
        {
            Schedules = new HashSet<Schedule>();
        }

        public uint Idtypeclasses { get; set; }
        public string TypeclassesName { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
