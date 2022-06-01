using System;
using System.Collections.Generic;

namespace EJAdmin.DataBaseClasses
{
    public partial class Educational
    {
        public uint Ideducational { get; set; }
        public string Name { get; set; } = null!;
        public string? NameDb { get; set; }
    }
}
