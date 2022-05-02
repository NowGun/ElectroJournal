using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Studyperiod
    {
        public Studyperiod()
        {
            Journals = new HashSet<Journal>();
        }

        public uint Idstudyperiod { get; set; }
        public string StudyperiodStart { get; set; }

        public virtual ICollection<Journal> Journals { get; set; }
    }
}
