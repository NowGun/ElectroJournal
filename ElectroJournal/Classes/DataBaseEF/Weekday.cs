using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Weekday
    {
        public Weekday()
        {
            Schedules = new HashSet<Schedule>();
        }

        public uint Idweekday { get; set; }
        public string WeekdayName { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
