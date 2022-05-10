using ElectroJournal.Classes.DataBaseEF;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Discipline
    {
        public Discipline()
        {
            Disciplinehours = new HashSet<Disciplinehour>();
            Journals = new HashSet<Journal>();
            Journalthemes = new HashSet<Journaltheme>();
            TeachersHasDisciplines = new HashSet<TeachersHasDiscipline>();
        }

        public uint Iddisciplines { get; set; }
        public string DisciplinesIndex { get; set; }
        public string DisciplinesName { get; set; }
        public string DisciplinesNameAbbreviated { get; set; }

        public virtual ICollection<Disciplinehour> Disciplinehours { get; set; }
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<Journaltheme> Journalthemes { get; set; }
        public virtual ICollection<TeachersHasDiscipline> TeachersHasDisciplines { get; set; }
    }
}
