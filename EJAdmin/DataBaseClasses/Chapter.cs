using System;
using System.Collections.Generic;

namespace EJAdmin.DataBaseClasses
{
    public partial class Chapter
    {
        public Chapter()
        {
            Bugreporters = new HashSet<Bugreporter>();
        }

        public uint Idchapter { get; set; }
        public string ChapterName { get; set; } = null!;

        public virtual ICollection<Bugreporter> Bugreporters { get; set; }
    }
}
