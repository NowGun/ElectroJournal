using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Periodclass
    {
        public Periodclass()
        {
            Schedules = new HashSet<Schedule>();
        }

        public uint Idperiodclasses { get; set; }
        public TimeSpan PeriodclassesStart { get; set; }
        public TimeSpan PeriodclassesEnd { get; set; }
        public int PeriodclassesNumber { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
