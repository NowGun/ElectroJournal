using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Cabinet
    {
        public Cabinet()
        {
            Schedules = new HashSet<Schedule>();
        }

        public uint Idcabinet { get; set; }
        public string CabinetNumber { get; set; }
        public string CabinetFloor { get; set; }
        public string CabinetFeatures { get; set; }
        public int? CabinetNumberSeats { get; set; }
        public string CabinetName { get; set; }
        public uint HousingIdhousing { get; set; }

        public virtual Housing HousingIdhousingNavigation { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
