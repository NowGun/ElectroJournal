#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Requestcabinet
    {
        public uint Idcabinet { get; set; }
        public string CabinetNumber { get; set; }
        public string CabinetFloor { get; set; }
        public string CabinetFeatures { get; set; }
        public int? CabinetNumberSeats { get; set; }
        public string CabinetName { get; set; }
        public string IfnullHousingName { get; set; }
    }
}
