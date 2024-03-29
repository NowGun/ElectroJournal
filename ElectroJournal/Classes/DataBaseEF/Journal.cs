﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ElectroJournal.Classes.DataBaseEF
{
    public partial class Journal
    {
        public uint Idjournal { get; set; }
        public uint StudentsIdstudents { get; set; }
        public uint DisciplinesIddisciplines { get; set; }
        public uint TeachersIdteachers { get; set; }
        public string JournalScore { get; set; }
        public uint StudyperiodIdstudyperiod { get; set; }
        public string JournalYear { get; set; }
        public string JournalMonth { get; set; }
        public string JournalDay { get; set; }
        public uint ScheduleIdschedule { get; set; }

        public virtual Discipline DisciplinesIddisciplinesNavigation { get; set; }
        public virtual Schedule ScheduleIdscheduleNavigation { get; set; }
        public virtual Student StudentsIdstudentsNavigation { get; set; }
        public virtual Studyperiod StudyperiodIdstudyperiodNavigation { get; set; }
        public virtual Teacher TeachersIdteachersNavigation { get; set; }
    }
}
