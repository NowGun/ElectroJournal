namespace ElectroJournal.Classes.DataBaseEJ
{
    public partial class Bugreporter
    {
        public uint Idbugreporter { get; set; }
        public string? BugreporterTitle { get; set; }
        public string? BugreporterMessage { get; set; }
        public uint StatusIdstatus { get; set; }
        public uint ChapterIdchapter { get; set; }

        public virtual Status? StatusIdstatusNavigation { get; set; }
        public virtual Chapter? ChapterIdchapterNavigation { get; set; }
    }
}
