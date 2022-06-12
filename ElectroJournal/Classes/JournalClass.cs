using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElectroJournal.Classes
{
    public class JournalClass
    {
        public static async Task<string> CheckSchedule()
        {
            using zhirovContext db = new();

            TimeOnly nowTime = TimeOnly.FromDateTime(DateTime.Now);
            DateOnly nowDate = DateOnly.FromDateTime(DateTime.Now);
            int id = Properties.Settings.Default.UserID;

            var ps = await db.TeachersHasDisciplines.Where(p => p.TeachersIdteachers == id)
                .Include(p => p.DisciplinesIddisciplinesNavigation)
                .ToListAsync();
            var tg = await db.TeachersHasGroups.Where(t => t.TeachersIdteachers == id)
                .Include(t => t.GroupsIdgroupsNavigation)
                .ToListAsync();

            var s = await db.Schedules
                .Where(s => s.ScheduleDate == nowDate)
                .Include(s => s.GroupsIdgroupsNavigation)
                .Include(s => s.DisciplinesIddisciplinesNavigation)
                .Include(s => s.PeriodclassesIdperiodclassesNavigation)
                .ToListAsync();

            if (ps.Count != 0 && tg.Count != 0 && s.Count != 0)
            {
                foreach (var t in tg)
                {
                    foreach (var p in ps)
                    {
                        foreach (var sche in s)
                        {
                            if (sche.GroupsIdgroupsNavigation.GroupsNameAbbreviated == t.GroupsIdgroupsNavigation.GroupsNameAbbreviated
                                && sche.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == p.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated
                                && (sche.PeriodclassesIdperiodclassesNavigation.PeriodclassesStart < nowTime && sche.PeriodclassesIdperiodclassesNavigation.PeriodclassesEnd > nowTime))
                            {
                                ((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedItem = sche.GroupsIdgroupsNavigation.GroupsNameAbbreviated;
                                return sche.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
