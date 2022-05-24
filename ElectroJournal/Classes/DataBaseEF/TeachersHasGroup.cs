using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class TeachersHasGroup
    {
        public uint TeachersIdteachers { get; set; }
        public uint GroupsIdgroups { get; set; }

        public virtual Group GroupsIdgroupsNavigation { get; set; }
        public virtual Teacher TeachersIdteachersNavigation { get; set; }
    }
}
