using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using unvell.ReoGrid;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Schedule.xaml
    /// </summary>
    public partial class Schedule
    {
        public Schedule()
        {
            InitializeComponent();
            SettingSheet();
            ReoGridSchedule.Visibility = System.Windows.Visibility.Collapsed;
        }

        private int gp;
        private int stuud;

        private void SettingSheet()
        {
            var worksheet = ReoGridSchedule.Worksheets[0];
            ReoGridSchedule.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);
            worksheet.SetSettings(WorksheetSettings.View_ShowHeaders, false);
            if (Properties.Settings.Default.Theme == 1)
            {
                ReoGridSchedule.ControlStyle = new ControlAppearanceStyle(Colors.Black, Colors.WhiteSmoke, false);
            }
            else
            {
                ReoGridSchedule.ControlStyle = new ControlAppearanceStyle(Colors.Gray, Colors.Black, false);
            }
        }
        private async void FillSchedule()
        {
            try
            {
                string[] w = ComboBoxSchoolWeek.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var worksheet = ReoGridSchedule.CurrentWorksheet;
                using zhirovContext db = new();

                var scheduleList = await db.Schedules
                    .Where(s => s.SchoolweekIdschoolweekNavigation.SchoolweekStart == DateOnly.Parse(w[0]) && s.SchoolweekIdschoolweekNavigation.SchoolweekEnd == DateOnly.Parse(w[1]))
                    .Include(s => s.GroupsIdgroupsNavigation)
                    .Include(s => s.PeriodclassesIdperiodclassesNavigation)
                    .Include(s => s.TeachersIdteachersNavigation)
                    .Include(s => s.CabinetIdcabinetNavigation)
                    .Include(s => s.TypeclassesIdtypeclassesNavigation)
                    .Include(s => s.DisciplinesIddisciplinesNavigation)
                    .Include(s => s.SchoolweekIdschoolweekNavigation)
                    .ToListAsync();

                await Task.Run(() =>
                {
                    foreach (var s in scheduleList)
                    {
                        for (int i = 1; i <= stuud - 1; i++)
                        {
                            for (int j = 1; j < gp; j++)
                            {
                                if (!String.IsNullOrWhiteSpace(ReoGridSchedule.CurrentWorksheet.Cells[i, 0].DisplayText))
                                {
                                    string gr = ReoGridSchedule.CurrentWorksheet.Cells[0, j].DisplayText;
                                    string call = ReoGridSchedule.CurrentWorksheet.Cells[i, 0].DisplayText;
                                    DateTime datt;
                                    if (!DateTime.TryParse(call, out datt))
                                    {
                                        string d = ReoGridSchedule.CurrentWorksheet.Cells[i - Int32.Parse(call), 0].DisplayText;
                                        if (s.GroupsIdgroupsNavigation.GroupsNameAbbreviated == gr
                                        && s.ScheduleDate == DateOnly.Parse(d)
                                        && s.PeriodclassesIdperiodclassesNavigation.PeriodclassesNumber == Int32.Parse(call))
                                        {
                                            Dispatcher.Invoke(() =>
                                            {
                                                if (s.DisciplinesIddisciplines != null
                                                && s.CabinetIdcabinet != null
                                                && s.TypeclassesIdtypeclasses != null
                                                && s.TeachersIdteachers != null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}" +
                                                    $"\\{s.CabinetIdcabinetNavigation.CabinetNumber}" +
                                                    $"\\{s.TypeclassesIdtypeclassesNavigation.TypeclassesName}" +
                                                    $"\\{s.TeachersIdteachersNavigation.TeachersSurname} {s.TeachersIdteachersNavigation.TeachersName}";
                                                }

                                                else if (s.DisciplinesIddisciplines != null
                                                && s.CabinetIdcabinet != null
                                                && s.TypeclassesIdtypeclasses != null
                                                && s.TeachersIdteachers == null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}" +
                                                    $"\\{s.CabinetIdcabinetNavigation.CabinetNumber}" +
                                                    $"\\{s.TypeclassesIdtypeclassesNavigation.TypeclassesName}";
                                                }

                                                else if (s.DisciplinesIddisciplines != null
                                                && s.CabinetIdcabinet != null
                                                && s.TypeclassesIdtypeclasses == null
                                                && s.TeachersIdteachers == null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}" +
                                                    $"\\{s.CabinetIdcabinetNavigation.CabinetNumber}";
                                                }

                                                else if (s.DisciplinesIddisciplines != null
                                                && s.CabinetIdcabinet == null
                                                && s.TypeclassesIdtypeclasses == null
                                                && s.TeachersIdteachers == null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}";
                                                }

                                                else if (s.DisciplinesIddisciplines != null
                                                && s.TypeclassesIdtypeclasses != null
                                                && s.TeachersIdteachers != null
                                                && s.CabinetIdcabinet == null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}" +
                                                    $"\\\\{s.TypeclassesIdtypeclassesNavigation.TypeclassesName}" +
                                                    $"\\{s.TeachersIdteachersNavigation.TeachersSurname} {s.TeachersIdteachersNavigation.TeachersName}";
                                                }

                                                else if (s.DisciplinesIddisciplines != null
                                                && s.TeachersIdteachers != null
                                                && s.TypeclassesIdtypeclasses == null
                                                && s.CabinetIdcabinet == null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}" +
                                                    $"\\\\\\{s.TeachersIdteachersNavigation.TeachersSurname} {s.TeachersIdteachersNavigation.TeachersName}";
                                                }

                                                else if (s.DisciplinesIddisciplines != null
                                                && s.CabinetIdcabinet != null
                                                && s.TeachersIdteachers != null
                                                && s.TypeclassesIdtypeclasses == null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}" +
                                                    $"\\{s.CabinetIdcabinetNavigation.CabinetNumber}" +
                                                    $"\\\\{s.TeachersIdteachersNavigation.TeachersSurname} {s.TeachersIdteachersNavigation.TeachersName}";
                                                }

                                                else if (s.DisciplinesIddisciplines != null
                                                && s.TypeclassesIdtypeclasses != null
                                                && s.CabinetIdcabinet == null
                                                && s.TeachersIdteachers == null)
                                                {
                                                    ReoGridSchedule.CurrentWorksheet[i, j] =
                                                    $"{s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated}" +
                                                    $"\\\\{s.TypeclassesIdtypeclassesNavigation.TypeclassesName}";
                                                }
                                                ReoGridSchedule.CurrentWorksheet.Cells[i, j].Style.TextWrap = TextWrapMode.WordBreak;
                                                Cell? cell = worksheet.Cells[i, j];
                                                cell.IsReadOnly = true;
                                            });

                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch
            {

            }
        }
        private async void FillGroups()
        {
            try
            {
                if (ComboBoxCourse.SelectedIndex != -1 && ComboBoxSchoolWeek.SelectedIndex != -1)
                {
                    var worksheet = ReoGridSchedule.CurrentWorksheet;
                    using zhirovContext db = new();
                    var g = ComboBoxCourse.SelectedIndex == 0 
                        ? await db.Groups.OrderBy(g => g.CourseIdcourseNavigation.CourseName).Select(g => g.GroupsNameAbbreviated).ToListAsync() 
                        : await db.Groups.OrderBy(g => g.GroupsNameAbbreviated).Where(g => g.CourseIdcourseNavigation.CourseName == ComboBoxCourse.SelectedItem.ToString()).Select(g => g.GroupsNameAbbreviated).ToListAsync();
                    gp = g.Count + 1;

                    for (int i = 1; i < gp; i++)
                    {
                        worksheet.SetCols(gp);
                        worksheet[0, i] = g[i - 1];
                        Cell? cell = worksheet.Cells[0, i];
                        cell.IsReadOnly = true;
                        ReoGridSchedule.CurrentWorksheet.ColumnHeaders[i].Width = 125;
                    }

                    FillCalls();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void FillCalls()
        {
            try
            {
                int x = 2;

                string[] d = ComboBoxSchoolWeek.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int countDays = (int)(DateTime.Parse(d[1]) - DateTime.Parse(d[0])).TotalDays + 1;
                DateTime[] dateDiap = new DateTime[countDays];
                for (int i = 0; i < countDays; i++)
                {
                    dateDiap[i] = DateTime.Parse(d[0]).Date.AddDays(i);
                }

                using zhirovContext db = new();
                var worksheet = ReoGridSchedule.CurrentWorksheet;
                var call = await db.Periodclasses.OrderBy(c => c.PeriodclassesNumber).Select(c => c.PeriodclassesNumber).ToListAsync();

                for (int j = 2; j - 2 < dateDiap.Length; j++)
                {
                    worksheet["A" + x] = dateDiap[j - 2].ToString("d");
                    ReoGridSchedule.CurrentWorksheet.MergeRange(new RangePosition(x - 1, 0, 1, gp));
                    ReoGridSchedule.CurrentWorksheet.Cells[x - 1, 0].Style.HAlign = ReoGridHorAlign.Center;
                    x++;
                    for (int i = 2; i - 2 < call.Count; i++)
                    {
                        worksheet["A" + x] = call[i - 2];
                        Cell? cell = worksheet.Cells["A" + x];
                        cell.IsReadOnly = true;
                        ReoGridSchedule.CurrentWorksheet.RowHeaders[x - 1].Height = 50;
                        x++;
                    }
                }
                stuud = x;
                worksheet.SetRows(stuud);
                FillSchedule();

                ((Storyboard)Resources["AnimCloseLoad"]).Begin();
            }
            catch (Exception ex)
            {
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => CheckComboBox();
        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            FillComboBoxSchoolWeek();
            FillComboBoxCourse();
        }
        private async void FillComboBoxCourse()
        {
            try
            {
                ComboBoxCourse.Items.Clear();
                ComboBoxCourse.Items.Add("Все курсы");
                using zhirovContext db = new();
                await db.Courses.OrderBy(t => t.CourseName).ForEachAsync(t =>
                {
                    ComboBoxCourse.Items.Add(t.CourseName);
                });
            }
            catch
            {

            }
        }
        private async void FillComboBoxSchoolWeek()
        {
            try
            {
                ComboBoxSchoolWeek.Items.Clear();
                using zhirovContext db = new();

                await db.Schoolweeks.OrderByDescending(g => g.SchoolweekStart).ForEachAsync(g =>
                {
                    ComboBoxSchoolWeek.Items.Add($"{g.SchoolweekStart} - {g.SchoolweekEnd}");
                });
            }
            catch
            {

            }
        }
        private void CheckComboBox()
        {
            if (ComboBoxCourse.SelectedIndex != -1 && ComboBoxSchoolWeek.SelectedIndex != -1)
            {
                ((Storyboard)Resources["AnimLoad"]).Begin();
                ReoGridSchedule.CurrentWorksheet.Reset();
                FillGroups();
            }
            else
            {
                GridPreLoad.Visibility = System.Windows.Visibility.Visible;
                ReoGridSchedule.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
