using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Chat
    {
        public uint Idchat { get; set; }
        public uint TeachersFrom { get; set; }
        public uint TeachersTo { get; set; }
        public string ChatText { get; set; }
        public DateTime ChatDate { get; set; }

        public virtual Teacher TeachersFromNavigation { get; set; }
        public virtual Teacher TeachersToNavigation { get; set; }
    }
}
