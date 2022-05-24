using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Schoolweek
    {
        public Schoolweek()
        {
            Schedules = new HashSet<Schedule>();
        }

        public uint Idschoolweek { get; set; }
        public DateTime SchoolweekStart { get; set; }
        public DateTime SchoolweekEnd { get; set; }
        public uint StudyperiodIdstudyperiod { get; set; }

        public virtual Studyperiod StudyperiodIdstudyperiodNavigation { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
