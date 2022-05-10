using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Student
    {
        public Student()
        {
            Journals = new HashSet<Journal>();
        }

        public uint Idstudents { get; set; }
        public string StudentsName { get; set; }
        public string StudentsSurname { get; set; }
        public string StudentsPatronymic { get; set; }
        public DateTime? StudentsBirthday { get; set; }
        public string StudentsResidence { get; set; }
        public string StudentsDormitory { get; set; }
        public string StudentsParent { get; set; }
        public string StudentsPhone { get; set; }
        public string StudentsParentPhone { get; set; }
        public uint GroupsIdgroups { get; set; }

        public virtual Group GroupsIdgroupsNavigation { get; set; }
        public virtual ICollection<Journal> Journals { get; set; }
    }
}
