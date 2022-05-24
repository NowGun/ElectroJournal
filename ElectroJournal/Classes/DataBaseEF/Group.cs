using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Group
    {
        public Group()
        {
            Disciplinehours = new HashSet<Disciplinehour>();
            Journalthemes = new HashSet<Journaltheme>();
            Schedules = new HashSet<Schedule>();
            Students = new HashSet<Student>();
            TeachersHasGroups = new HashSet<TeachersHasGroup>();
        }

        public uint Idgroups { get; set; }
        public string GroupsName { get; set; }
        public string GroupsNameAbbreviated { get; set; }
        public string GroupsPrefix { get; set; }
        public uint TypelearningIdtypelearning { get; set; }
        public uint CourseIdcourse { get; set; }
        public uint TeachersIdteachers { get; set; }

        public virtual Course CourseIdcourseNavigation { get; set; }
        public virtual Teacher TeachersIdteachersNavigation { get; set; }
        public virtual Typelearning TypelearningIdtypelearningNavigation { get; set; }
        public virtual ICollection<Disciplinehour> Disciplinehours { get; set; }
        public virtual ICollection<Journaltheme> Journalthemes { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<TeachersHasGroup> TeachersHasGroups { get; set; }
    }
}
