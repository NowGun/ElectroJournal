using System;
using System.Collections.Generic;

namespace EJAdmin.DataBaseClasses
{
    public partial class Bugreporter
    {
        public uint Idbugreporter { get; set; }
        public string BugreporterTitle { get; set; } = null!;
        public string BugreporterMessage { get; set; } = null!;
        public uint StatusIdstatus { get; set; }
        public uint ChapterIdchapter { get; set; }

        public virtual Chapter ChapterIdchapterNavigation { get; set; } = null!;
        public virtual Status StatusIdstatusNavigation { get; set; } = null!;
    }
}
