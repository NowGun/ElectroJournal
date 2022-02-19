using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class Date
    {
        public int IdDate { get; set; }
        public DateTime Fulldate { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Quarter { get; set; }
        public int Week { get; set; }
        public int DayOfWeek { get; set; }
        public int Weekend { get; set; }
    }
}
