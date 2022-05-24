using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Typelearning
    {
        public Typelearning()
        {
            Groups = new HashSet<Group>();
        }

        public uint Idtypelearning { get; set; }
        public string TypelearningName { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
