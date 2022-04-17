using System;
using System.Collections.Generic;

namespace ElectroJournal.Classes.DataBaseEJ
{
    public partial class Chapter
    {
        public Chapter()
        {
            Bugreporters = new HashSet<Bugreporter>();
        }

        public uint Idchapter { get; set; }
        public string ChapterName { get; set; }

        public virtual ICollection<Bugreporter> Bugreporters { get; set; }
    }
}
