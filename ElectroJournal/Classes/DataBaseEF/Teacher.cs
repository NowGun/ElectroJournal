using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Teacher
    {
        public Teacher()
        {
            Journals = new HashSet<Journal>();
            Schedules = new HashSet<Schedule>();
            TeachersHasGroups = new HashSet<TeachersHasGroup>();
        }

        public uint Idteachers { get; set; }
        public string TeachersLogin { get; set; }
        public string TeachersPassword { get; set; }
        public string TeachersName { get; set; }
        public string TeachersSurname { get; set; }
        public string TeachersPatronymic { get; set; }
        public byte[] TeachersImage { get; set; }
        public string TeachersAccesAdminPanel { get; set; }
        public string TeachersPhone { get; set; }
        public string TeachersMail { get; set; }

        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<TeachersHasGroup> TeachersHasGroups { get; set; }
    }
}
