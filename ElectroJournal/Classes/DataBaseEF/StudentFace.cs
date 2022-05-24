using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class StudentFace
    {
        public uint StudentId { get; set; }
        public string Face { get; set; }

        public virtual Student Student { get; set; }
    }
}
