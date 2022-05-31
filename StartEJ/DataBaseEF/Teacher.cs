using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Teacher
    {
        public Teacher()
        {
        }

        public uint Idteachers { get; set; }
        public string TeachersLogin { get; set; }
        public string TeachersPassword { get; set; }
        public string TeachersName { get; set; }
        public string TeachersSurname { get; set; }
        public string TeachersPatronymic { get; set; }
        public string TeachersImage { get; set; }
        public string TeachersAccesAdminPanel { get; set; }
        public string TeachersPhone { get; set; }
        public string TeachersMail { get; set; }
        public sbyte? TeachersStatus { get; set; }

    }
}
