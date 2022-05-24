using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Disciplinehour
    {
        public uint Iddisciplinehours { get; set; }
        public uint GroupsIdgroups { get; set; }
        public uint DisciplinesIddisciplines { get; set; }
        public uint CourseIdcourse { get; set; }
        public int DisciplinehoursMadatory { get; set; }
        public int DisciplinehoursSeparate { get; set; }

        public virtual Course CourseIdcourseNavigation { get; set; }
        public virtual Discipline DisciplinesIddisciplinesNavigation { get; set; }
        public virtual Group GroupsIdgroupsNavigation { get; set; }
    }
}
