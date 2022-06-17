using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Presence
    {
        public uint Id { get; set; }
        public uint StudentId { get; set; }
        public DateTime PresenceDatetime { get; set; }
        public uint IdentificationTypeId { get; set; }
        public uint? SmartcardId { get; set; }
        public uint IdentificatedAt { get; set; }
        public uint ScheduleId { get; set; }
        public uint DisciplineId { get; set; }
        public uint ClassesPeriodId { get; set; }

        public virtual Cabinet IdentificatedAtNavigation { get; set; }
        public virtual IdentificationType IdentificationType { get; set; }
        public virtual Smartcard Smartcard { get; set; }
        public virtual Student Student { get; set; }
    }
}
