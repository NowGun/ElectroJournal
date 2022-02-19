using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Journaltheme
    {
        public uint Idjournaltheme { get; set; }
        public uint GroupsIdgroups { get; set; }
        public uint DisciplinesIddisciplines { get; set; }
        public DateTime JournalthemeDate { get; set; }
        public int JournalthemeHours { get; set; }
        public string JournalthemeName { get; set; }
        public string JournalthemeHomework { get; set; }

        public virtual Discipline DisciplinesIddisciplinesNavigation { get; set; }
        public virtual Group GroupsIdgroupsNavigation { get; set; }
    }
}
