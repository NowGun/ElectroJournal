using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Studyperiod
    {
        public Studyperiod()
        {
            Journals = new HashSet<Journal>();
            Schoolweeks = new HashSet<Schoolweek>();
        }

        public uint Idstudyperiod { get; set; }
        public string StudyperiodStart { get; set; }

        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<Schoolweek> Schoolweeks { get; set; }
    }
}
