using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Update
    {
        public uint Idupdate { get; set; }
        public string UpdateNew { get; set; }
        public string UpdateVersion { get; set; }
    }
}
