using ElectroJournal.DataBase;

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Setting
    {
        public uint Idsettings { get; set; }
        public uint TeachersIdteachers { get; set; }
        public sbyte SettingsPhone { get; set; }
        public sbyte SettingsEmail { get; set; }

        public virtual Teacher TeachersIdteachersNavigation { get; set; }
    }
}
