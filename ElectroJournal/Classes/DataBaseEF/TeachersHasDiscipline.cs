using ElectroJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
