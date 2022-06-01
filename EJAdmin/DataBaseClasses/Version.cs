using System;
using System.Collections.Generic;

namespace EJAdmin.DataBaseClasses
{
    public partial class Version
    {
        public uint Idversion { get; set; }
        public string VersionName { get; set; } = null!;
    }
}
