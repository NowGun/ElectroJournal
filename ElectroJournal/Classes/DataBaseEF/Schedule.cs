using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Schedule
    {
        public uint Idschedule { get; set; }
        public uint GroupsIdgroups { get; set; }
        public uint? PeriodclassesIdperiodclasses { get; set; }
        public uint? TeachersIdteachers { get; set; }
        public uint? CabinetIdcabinet { get; set; }
        public uint? TypeclassesIdtypeclasses { get; set; }
        public DateOnly ScheduleDate { get; set; }
        public uint? DisciplinesIddisciplines { get; set; }
        public uint SchoolweekIdschoolweek { get; set; }

        public virtual Cabinet CabinetIdcabinetNavigation { get; set; }
        public virtual Discipline DisciplinesIddisciplinesNavigation { get; set; }
        public virtual Group GroupsIdgroupsNavigation { get; set; }
        public virtual Periodclass PeriodclassesIdperiodclassesNavigation { get; set; }
        public virtual Schoolweek SchoolweekIdschoolweekNavigation { get; set; }
        public virtual Teacher TeachersIdteachersNavigation { get; set; }
        public virtual Typeclass TypeclassesIdtypeclassesNavigation { get; set; }
    }
}
