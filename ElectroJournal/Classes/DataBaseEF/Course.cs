using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Course
    {
        public Course()
        {
            Disciplinehours = new HashSet<Disciplinehour>();
            Groups = new HashSet<Group>();
        }

        public uint Idcourse { get; set; }
        public string CourseName { get; set; }

        public virtual ICollection<Disciplinehour> Disciplinehours { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
