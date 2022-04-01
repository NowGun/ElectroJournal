using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroJournal.Classes.DataBaseEJ
{
    public partial class Version
    {
        public uint IdVersion { get; set; }
        public string VersionName { get; set; } = null!;
    }
}
