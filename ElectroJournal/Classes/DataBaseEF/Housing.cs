using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Housing
    {
        public Housing()
        {
            Cabinets = new HashSet<Cabinet>();
        }

        public uint Idhousing { get; set; }
        public string HousingName { get; set; }
        public string HousingAddress { get; set; }
        public int HousingCountCabinet { get; set; }

        public virtual ICollection<Cabinet> Cabinets { get; set; }
    }
}
