using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Typeclass
    {
        public Typeclass()
        {
            Schedules = new HashSet<Schedule>();
        }

        public uint Idtypeclasses { get; set; }
        public string TypeclassesName { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
