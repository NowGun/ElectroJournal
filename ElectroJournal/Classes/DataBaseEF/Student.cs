﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Student
    {
        public Student()
        {
            Journals = new HashSet<Journal>();
            Presences = new HashSet<Presence>();
            Smartcards = new HashSet<Smartcard>();
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
        public uint? GroupsIdgroups { get; set; }
        public int? IdTicket { get; set; }

        public virtual Group GroupsIdgroupsNavigation { get; set; }
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<Presence> Presences { get; set; }
        public virtual ICollection<Smartcard> Smartcards { get; set; }
    }
}
