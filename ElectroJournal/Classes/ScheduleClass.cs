using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroJournal.Classes
{
    public class ScheduleClass
    {
        public async void SaveSchedule(string group, string call, string? teach, string? cab, string? type, string date, string? disp, string week, bool isChange)
        {
            using zhirovContext db = new();

            string?[] teach2 = teach.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] dates = week.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            var t = await db.Teachers.Where(t => t.TeachersName == teach2[1] && t.TeachersSurname == teach2[0]).FirstOrDefaultAsync();
            var g = await db.Groups.FirstOrDefaultAsync(g => g.GroupsNameAbbreviated == group);
            var c = await db.Periodclasses.FirstOrDefaultAsync(c => c.PeriodclassesNumber == Int32.Parse(call));
            var ca = await db.Cabinets.FirstOrDefaultAsync(c => c.CabinetNumber == cab);
            var ty = await db.Typeclasses.FirstOrDefaultAsync(t => t.TypeclassesName == type);
            var d = await db.Disciplines.FirstOrDefaultAsync(d => d.DisciplinesNameAbbreviated == disp || d.DisciplinesName == disp);
            var w = await db.Schoolweeks.FirstOrDefaultAsync(w => w.SchoolweekStart == DateOnly.Parse(dates[0]) && w.SchoolweekEnd == DateOnly.Parse(dates[1]));

            if (t != null && g != null && c != null && ca != null && ty != null && d != null && w != null)
            {
                if (isChange)
                {
                    DataBaseEF.Schedule? s = await db.Schedules
                        .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                       && s.ScheduleDate == DateOnly.Parse(date)
                       && s.SchoolweekIdschoolweek == w.Idschoolweek
                       && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                    if (s != null)
                    {
                        s.DisciplinesIddisciplines = d.Iddisciplines;
                        s.TeachersIdteachers = t.Idteachers;
                        s.CabinetIdcabinet = ca.Idcabinet;
                        s.TypeclassesIdtypeclasses = ty.Idtypeclasses;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    DataBaseEF.Schedule s = new DataBaseEF.Schedule
                    {
                        GroupsIdgroups = g.Idgroups,
                        PeriodclassesIdperiodclasses = c.Idperiodclasses,
                        TeachersIdteachers = t.Idteachers,
                        CabinetIdcabinet = ca.Idcabinet,
                        TypeclassesIdtypeclasses = ty.Idtypeclasses,
                        ScheduleDate = DateOnly.Parse(date),
                        DisciplinesIddisciplines = d.Iddisciplines,
                        SchoolweekIdschoolweek = w.Idschoolweek
                    };

                    await db.Schedules.AddAsync(s);
                    await db.SaveChangesAsync();
                }
            }
            else if (t != null && g != null && c != null && ca != null && d != null && w != null)
            {
                if (isChange)
                {
                    DataBaseEF.Schedule? s = await db.Schedules
                        .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                       && s.ScheduleDate == DateOnly.Parse(date)
                       && s.SchoolweekIdschoolweek == w.Idschoolweek
                       && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                    if (s != null)
                    {
                        s.DisciplinesIddisciplines = d.Iddisciplines;
                        s.TeachersIdteachers = t.Idteachers;
                        s.CabinetIdcabinet = ca.Idcabinet;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    DataBaseEF.Schedule s = new DataBaseEF.Schedule
                    {
                        GroupsIdgroups = g.Idgroups,
                        PeriodclassesIdperiodclasses = c.Idperiodclasses,
                        TeachersIdteachers = t.Idteachers,
                        CabinetIdcabinet = ca.Idcabinet,
                        ScheduleDate = DateOnly.Parse(date),
                        DisciplinesIddisciplines = d.Iddisciplines,
                        SchoolweekIdschoolweek = w.Idschoolweek
                    };

                    await db.Schedules.AddAsync(s);
                    await db.SaveChangesAsync();
                }
            }
            else if (t != null && g != null && c != null && ty != null && d != null && w != null)
            {
                if (isChange)
                {
                    DataBaseEF.Schedule? s = await db.Schedules
                        .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                       && s.ScheduleDate == DateOnly.Parse(date)
                       && s.SchoolweekIdschoolweek == w.Idschoolweek
                       && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                    if (s != null)
                    {
                        s.DisciplinesIddisciplines = d.Iddisciplines;
                        s.TeachersIdteachers = t.Idteachers;
                        s.TypeclassesIdtypeclasses = ty.Idtypeclasses;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    DataBaseEF.Schedule s = new DataBaseEF.Schedule
                    {
                        GroupsIdgroups = g.Idgroups,
                        PeriodclassesIdperiodclasses = c.Idperiodclasses,
                        TeachersIdteachers = t.Idteachers,
                        TypeclassesIdtypeclasses = ty.Idtypeclasses,
                        ScheduleDate = DateOnly.Parse(date),
                        DisciplinesIddisciplines = d.Iddisciplines,
                        SchoolweekIdschoolweek = w.Idschoolweek
                    };

                    await db.Schedules.AddAsync(s);
                    await db.SaveChangesAsync();
                }
            }
            else if (t != null && g != null && c != null && d != null && w != null)
            {
                if (isChange)
                {
                    DataBaseEF.Schedule? s = await db.Schedules
                        .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                       && s.ScheduleDate == DateOnly.Parse(date)
                       && s.SchoolweekIdschoolweek == w.Idschoolweek
                       && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                    if (s != null)
                    {
                        s.DisciplinesIddisciplines = d.Iddisciplines;
                        s.TeachersIdteachers = t.Idteachers;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    DataBaseEF.Schedule s = new DataBaseEF.Schedule
                    {
                        GroupsIdgroups = g.Idgroups,
                        PeriodclassesIdperiodclasses = c.Idperiodclasses,
                        TeachersIdteachers = t.Idteachers,
                        ScheduleDate = DateOnly.Parse(date),
                        DisciplinesIddisciplines = d.Iddisciplines,
                        SchoolweekIdschoolweek = w.Idschoolweek
                    };

                    await db.Schedules.AddAsync(s);
                    await db.SaveChangesAsync();
                }
            }
        }
        public async void SaveSchedule(string group, string call, string? cab, string? type, string date, string? disp, string week, bool isChange)
        {
            using zhirovContext db = new();

            string[] dates = week.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            var g = await db.Groups.FirstOrDefaultAsync(g => g.GroupsNameAbbreviated == group);
            var c = await db.Periodclasses.FirstOrDefaultAsync(c => c.PeriodclassesNumber == Int32.Parse(call));
            var ca = await db.Cabinets.FirstOrDefaultAsync(c => c.CabinetNumber == cab);
            var ty = await db.Typeclasses.FirstOrDefaultAsync(t => t.TypeclassesName == type);
            var d = await db.Disciplines.FirstOrDefaultAsync(d => d.DisciplinesNameAbbreviated == disp || d.DisciplinesName == disp);
            var w = await db.Schoolweeks.FirstOrDefaultAsync(w => w.SchoolweekStart == DateOnly.Parse(dates[0]) && w.SchoolweekEnd == DateOnly.Parse(dates[1]));

            if (g != null && c != null && ca != null && ty != null && d != null && w != null)
            {
                if (isChange)
                {
                    DataBaseEF.Schedule? s = await db.Schedules
                        .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                       && s.ScheduleDate == DateOnly.Parse(date)
                       && s.SchoolweekIdschoolweek == w.Idschoolweek
                       && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                    if (s != null)
                    {
                        s.DisciplinesIddisciplines = d.Iddisciplines;
                        s.CabinetIdcabinet = ca.Idcabinet;
                        s.TypeclassesIdtypeclasses = ty.Idtypeclasses;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    DataBaseEF.Schedule s = new DataBaseEF.Schedule
                    {
                        GroupsIdgroups = g.Idgroups,
                        PeriodclassesIdperiodclasses = c.Idperiodclasses,
                        CabinetIdcabinet = ca.Idcabinet,
                        TypeclassesIdtypeclasses = ty.Idtypeclasses,
                        ScheduleDate = DateOnly.Parse(date),
                        DisciplinesIddisciplines = d.Iddisciplines,
                        SchoolweekIdschoolweek = w.Idschoolweek
                    };

                    await db.Schedules.AddAsync(s);
                    await db.SaveChangesAsync();
                }
            }
            else if (g != null && c != null && ty != null && d != null && w != null)
            {
                if (isChange)
                {
                    DataBaseEF.Schedule? s = await db.Schedules
                        .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                       && s.ScheduleDate == DateOnly.Parse(date)
                       && s.SchoolweekIdschoolweek == w.Idschoolweek
                       && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                    if (s != null)
                    {
                        s.DisciplinesIddisciplines = d.Iddisciplines;
                        s.TypeclassesIdtypeclasses = ty.Idtypeclasses;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    DataBaseEF.Schedule s = new DataBaseEF.Schedule
                    {
                        GroupsIdgroups = g.Idgroups,
                        PeriodclassesIdperiodclasses = c.Idperiodclasses,
                        TypeclassesIdtypeclasses = ty.Idtypeclasses,
                        ScheduleDate = DateOnly.Parse(date),
                        DisciplinesIddisciplines = d.Iddisciplines,
                        SchoolweekIdschoolweek = w.Idschoolweek
                    };

                    await db.Schedules.AddAsync(s);
                    await db.SaveChangesAsync();
                }
            }
        }
        public async void SaveSchedule(string group, string call, string? cab, string date, string? disp, string week, bool isChange)
        {
            try
            {
                using zhirovContext db = new();

                string[] dates = week.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                var g = await db.Groups.FirstOrDefaultAsync(g => g.GroupsNameAbbreviated == group);
                var c = await db.Periodclasses.FirstOrDefaultAsync(c => c.PeriodclassesNumber == Int32.Parse(call));
                var ca = await db.Cabinets.FirstOrDefaultAsync(c => c.CabinetNumber == cab);
                var d = await db.Disciplines.FirstOrDefaultAsync(d => d.DisciplinesNameAbbreviated == disp || d.DisciplinesName == disp);
                var w = await db.Schoolweeks.FirstOrDefaultAsync(w => w.SchoolweekStart == DateOnly.Parse(dates[0]) && w.SchoolweekEnd == DateOnly.Parse(dates[1]));

                if (g != null && c != null && ca != null && d != null && w != null)
                {
                    if (isChange)
                    {
                        DataBaseEF.Schedule? s = await db.Schedules
                            .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                           && s.ScheduleDate == DateOnly.Parse(date)
                           && s.SchoolweekIdschoolweek == w.Idschoolweek
                           && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                        if (s != null)
                        {
                            s.DisciplinesIddisciplines = d.Iddisciplines;
                            s.CabinetIdcabinet = ca.Idcabinet;
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        DataBaseEF.Schedule s = new DataBaseEF.Schedule
                        {
                            GroupsIdgroups = g.Idgroups,
                            PeriodclassesIdperiodclasses = c.Idperiodclasses,
                            CabinetIdcabinet = ca.Idcabinet,
                            ScheduleDate = DateOnly.Parse(date),
                            DisciplinesIddisciplines = d.Iddisciplines,
                            SchoolweekIdschoolweek = w.Idschoolweek
                        };

                        await db.Schedules.AddAsync(s);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async void SaveSchedule(string group, string call, string date, string? disp, string week, bool isChange)
        {
            using zhirovContext db = new();

            string[] dates = week.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            var g = await db.Groups.FirstOrDefaultAsync(g => g.GroupsNameAbbreviated == group);
            var c = await db.Periodclasses.FirstOrDefaultAsync(c => c.PeriodclassesNumber == Int32.Parse(call));
            var d = await db.Disciplines.FirstOrDefaultAsync(d => d.DisciplinesNameAbbreviated == disp || d.DisciplinesName == disp);
            var w = await db.Schoolweeks.FirstOrDefaultAsync(w => w.SchoolweekStart == DateOnly.Parse(dates[0]) && w.SchoolweekEnd == DateOnly.Parse(dates[1]));

            if (g != null && c != null && d != null && w != null)
            {
                if (isChange)
                {
                    DataBaseEF.Schedule? s = await db.Schedules
                        .FirstOrDefaultAsync(s => s.GroupsIdgroups == g.Idgroups
                       && s.ScheduleDate == DateOnly.Parse(date)
                       && s.SchoolweekIdschoolweek == w.Idschoolweek
                       && s.PeriodclassesIdperiodclasses == c.Idperiodclasses);

                    if (s != null)
                    {
                        s.DisciplinesIddisciplines = d.Iddisciplines;
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    DataBaseEF.Schedule s = new DataBaseEF.Schedule
                    {
                        GroupsIdgroups = g.Idgroups,
                        PeriodclassesIdperiodclasses = c.Idperiodclasses,
                        ScheduleDate = DateOnly.Parse(date),
                        DisciplinesIddisciplines = d.Iddisciplines,
                        SchoolweekIdschoolweek = w.Idschoolweek
                    };

                    await db.Schedules.AddAsync(s);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
