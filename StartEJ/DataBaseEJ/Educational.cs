using System;
using System.Collections.Generic;

namespace ElectroJournal.Classes.DataBaseEJ
{
    public partial class Educational
    {
        public uint Ideducational { get; set; }
        public string Name { get; set; } = null!;
        public string? NameDb { get; set; }
    }
}
