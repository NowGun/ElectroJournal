using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class TeachersHasDiscipline
    {
        public uint TeachersIdteachers { get; set; }
        public uint DisciplinesIddisciplines { get; set; }

        public virtual Discipline DisciplinesIddisciplinesNavigation { get; set; }
        public virtual Teacher TeachersIdteachersNavigation { get; set; }
    }
}
