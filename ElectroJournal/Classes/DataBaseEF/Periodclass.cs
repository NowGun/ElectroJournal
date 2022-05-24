using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Periodclass
    {
        public Periodclass()
        {
            Schedules = new HashSet<Schedule>();
        }

        public uint Idperiodclasses { get; set; }
        public TimeOnly? PeriodclassesStart { get; set; }
        public TimeOnly? PeriodclassesEnd { get; set; }
        public int PeriodclassesNumber { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
