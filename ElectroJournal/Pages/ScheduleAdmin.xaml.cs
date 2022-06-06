using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using unvell.ReoGrid;
using unvell.ReoGrid.Events;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для ScheduleAdmin.xaml
    /// </summary>
    public partial class ScheduleAdmin : Page
    {
        public ScheduleAdmin()
        {
            InitializeComponent();
            SettingSheet();
            ReoGridSchedule.Visibility = System.Windows.Visibility.Collapsed;
            var ws = ReoGridSchedule.Worksheets[0];
            ws.CellDataChanged += rgrid_AfterCellEdit;
            ws.BeforeCellEdit += rgrid_BeforeCellEdit;
        }

        int idSchedule = 0;
        private bool changeCall = false;
        private int gp;
        private int stuud;
        private int daysTable;
        private bool isChange = false;
        ScheduleClass sc = new();

        private void rgrid_BeforeCellEdit(object sender, CellEventArgs e)
        {
            string[] poz = ReoGridSchedule.CurrentWorksheet.SelectionRange.ToString().Split(new char[] { ':' });
            string score = poz[0];

            if (String.IsNullOrWhiteSpace(ReoGridSchedule.CurrentWorksheet.Cells[score].DisplayText)) isChange = false;
            else isChange = true;
        }
        private void IconAddScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            changeCall = false;
            NumberBoxNumberCall.Clear();
            TextBoxStartCall.Clear();
            TextBoxEndCall.Clear();
            RootDialogScheduleCall.Show();
        }
        private async void IconDeleteScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxScheduleCall.SelectedIndex != -1)
            {
                using zhirovContext db = new();
                Periodclass? s = await db.Periodclasses.Where(s => s.PeriodclassesNumber == Int32.Parse(ListBoxScheduleCall.SelectedItem.ToString().Split().First())).FirstOrDefaultAsync();

                if (s != null)
                {
                    db.Periodclasses.Remove(s);
                    await db.SaveChangesAsync();

                    FillListBoxScheduleCall();
                }
            }
        }
        private async void IconChangeScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxScheduleCall.SelectedIndex != -1)
            {
                changeCall = true;
                NumberBoxNumberCall.Clear();
                TextBoxStartCall.Clear();
                TextBoxEndCall.Clear();

                using zhirovContext db = new();

                var c = await db.Periodclasses.FirstOrDefaultAsync(c => c.PeriodclassesNumber.ToString() == ListBoxScheduleCall.SelectedItem.ToString().Split().First());

                NumberBoxNumberCall.Text = c.PeriodclassesNumber.ToString();
                TextBoxStartCall.Text = c.PeriodclassesStart.ToString();
                TextBoxEndCall.Text = c.PeriodclassesEnd.ToString();

                RootDialogScheduleCall.Show();
            }
        }
        private void SettingSheet()
        {
            ReoGridSchedule.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);
            if (Properties.Settings.Default.Theme == 1)
            {
                ReoGridSchedule.ControlStyle = new ControlAppearanceStyle(Colors.Black, Colors.WhiteSmoke, false);
            }
            else
            {
                ReoGridSchedule.ControlStyle = new ControlAppearanceStyle(Colors.Gray, Colors.Black, false);
            }
        }
        private void ReoGridSchedule_WorkbookLoaded(object sender, EventArgs e)
        {
            //SettingSheet();

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
                    foreach (var s  in scheduleList)
                    {
                        for (int i = 1; i <= stuud-1; i++)
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
        async void rgrid_AfterCellEdit(object sender, CellEventArgs e)
        {
            string[] poz = ReoGridSchedule.CurrentWorksheet.SelectionRange.ToString().Split(new char[] { ':' });
            string score = poz[0];
            int stud = int.Parse(Regex.Match(score, @"\d+").Value);
            string poz6 = Regex.Replace(score, @"[^A-Z]+", string.Empty);

            using zhirovContext db = new();

            if (!String.IsNullOrWhiteSpace(ReoGridSchedule.CurrentWorksheet.Cells[stud - 1, 0].DisplayText))
            {
                string call = ReoGridSchedule.CurrentWorksheet.Cells[stud - 1, 0].DisplayText;
                string group = ReoGridSchedule.CurrentWorksheet.Cells[$"{poz6}1"].DisplayText;
                string date = ReoGridSchedule.CurrentWorksheet.Cells[stud - Int32.Parse(call) - 1, 0].DisplayText;
                string?[] info = ReoGridSchedule.CurrentWorksheet.Cells[score].DisplayText.Split(new char[] { '\\' });
                switch (info.Length)
                {
                    case 4:
                        sc.SaveSchedule(group, call, info[3], info[1], info[2], date, info[0], ComboBoxSchoolWeek.SelectedItem.ToString(), isChange);
                        break;
                    case 3:
                        sc.SaveSchedule(group, call, info[1], info[2], date, info[0], ComboBoxSchoolWeek.SelectedItem.ToString(), isChange);
                        break;
                    case 2:
                        sc.SaveSchedule(group, call, info[1], date, info[0], ComboBoxSchoolWeek.SelectedItem.ToString(), isChange);
                        break;
                    case 1:
                        if (info[0] == "")
                        {
                            var s = await db.Schedules.FirstOrDefaultAsync(s => s.GroupsIdgroupsNavigation.GroupsNameAbbreviated == group
                        && s.ScheduleDate == DateOnly.Parse(date)
                        && s.PeriodclassesIdperiodclassesNavigation.PeriodclassesNumber == int.Parse(call));

                            if (s != null)
                            {
                                db.Schedules.Remove(s);
                                await db.SaveChangesAsync();
                            }
                        }
                        else sc.SaveSchedule(group, call, date, info[0], ComboBoxSchoolWeek.SelectedItem.ToString(), isChange);
                        break;
                }
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
                        ReoGridSchedule.CurrentWorksheet.RowHeaders[x-1].Height = 50;
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
        private void ExpanderScheduleCall_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillListBoxScheduleCall();
        private void ComboBoxHousing_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillListBoxCabinets();
        private async void FillComboBoxHousing()
        {
            ComboBoxHousing.Items.Clear();

            using zhirovContext db = new();
            await db.Housings.ForEachAsync(g =>
            {
                ComboBoxHousing.Items.Add(g.HousingName);
            });
        }
        private async void FillComboBoxSchoolWeek()
        {
            ComboBoxSchoolWeek.Items.Clear();

            using zhirovContext db = new();
            await db.Schoolweeks.OrderByDescending(g => g.Idschoolweek).ForEachAsync(g =>
            {
                ComboBoxSchoolWeek.Items.Add($"{g.SchoolweekStart} - {g.SchoolweekEnd}");
            });
        }
        private async void FillListBoxScheduleCall()
        {
            ListBoxScheduleCall.Items.Clear();

            using zhirovContext db = new();

            var s = await db.Periodclasses.OrderBy(s => s.PeriodclassesNumber).ToListAsync();

            if (s != null)
            {
                foreach (var t in s)
                {
                    ListBoxScheduleCall.Items.Add($"{t.PeriodclassesNumber} | {t.PeriodclassesStart} - {t.PeriodclassesEnd}");
                }
            }
        }
        private async void FillListBoxCabinets()
        {
            ListBoxCabinets.Items.Clear();

            using zhirovContext db = new();

            if (ComboBoxHousing.SelectedItem != null)
            {
                await db.Cabinets.OrderBy(c => c.CabinetNumber).Where(c => c.HousingIdhousingNavigation.HousingName == ComboBoxHousing.SelectedItem.ToString()).ForEachAsync(c =>
                {
                    ListBoxCabinets.Items.Add(c.CabinetNumber);
                });
            }
        }
        private async void FillListBoxDisciplines()
        {
            try
            {
                ListBoxDisciplines.Items.Clear();
                using zhirovContext db = new();

                if (String.IsNullOrWhiteSpace(SearchBoxDiscipline.Text))
                {
                    await db.Disciplines.OrderBy(c => c.DisciplinesNameAbbreviated).ForEachAsync(c =>
                    {
                        ListBoxDisciplines.Items.Add(c.DisciplinesNameAbbreviated);
                    });
                }
                else
                {
                    await db.Disciplines
                        .OrderBy(c => c.DisciplinesNameAbbreviated)
                        .Where(c => EF.Functions.Like(c.DisciplinesNameAbbreviated, $"%{SearchBoxDiscipline.Text}%") ||
                        EF.Functions.Like(c.DisciplinesName, $"%{SearchBoxDiscipline.Text}%"))
                        .ForEachAsync(c =>
                        {
                            ListBoxDisciplines.Items.Add(c.DisciplinesNameAbbreviated);
                        });
                }
            }
            catch
            {

            }
        }
        private async void FillListBoxTeachers()
        {
            ListBoxTeachers.Items.Clear();
            using zhirovContext db = new();

            if (String.IsNullOrWhiteSpace(SearchBoxTeachers.Text))
            {
                await db.Teachers.Where(t => t.TeachersName != "CardReaderService").OrderBy(t => t.TeachersSurname).ForEachAsync(t => ListBoxTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName}"));
            }
            else
            {
                await db.Teachers
                    .OrderBy(t => t.TeachersSurname)
                    .Where(t => t.TeachersName != "CardReaderService" && EF.Functions.Like(t.TeachersName, $"%{SearchBoxTeachers.Text}%") ||
                    EF.Functions.Like(t.TeachersSurname, $"%{SearchBoxTeachers.Text}%") ||
                    EF.Functions.Like(t.TeachersPatronymic, $"%{SearchBoxTeachers.Text}%"))
                    .ForEachAsync(t => ListBoxTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}"));
            }            
        }
        private void ExpanderCabinets_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillComboBoxHousing();
        private void ExpanderDisciplines_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillListBoxDisciplines();
        private void ExpanderTeachers_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillListBoxTeachers();
        private void SearchBoxDiscipline_PreviewKeyUp(object sender, KeyEventArgs e) => FillListBoxDisciplines();
        private void SearchBoxTeachers_PreviewKeyUp(object sender, KeyEventArgs e) => FillListBoxTeachers();
        private async void RootDialogNewSchedule_ButtonLeftClick(object sender, System.Windows.RoutedEventArgs e)
        {
            using zhirovContext db = new();

            Schoolweek sc = new()
            {
                SchoolweekStart = DateOnly.FromDateTime((DateTime)DatePickerNewScheduleStart.SelectedDate),
                SchoolweekEnd = DateOnly.FromDateTime((DateTime)DatePickernewScheduleEnd.SelectedDate),
                StudyperiodIdstudyperiod = 1
            };

            await db.Schoolweeks.AddAsync(sc);
            await db.SaveChangesAsync();

            FillComboBoxSchoolWeek();

            ComboBoxSchoolWeek.SelectedIndex = 0;
            RootDialogNewSchedule.Hide();
        }
        private void RootDialogNewSchedule_ButtonRightClick(object sender, System.Windows.RoutedEventArgs e) => RootDialogNewSchedule.Hide();
        private void ButtonNewSchedule_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RootDialogNewSchedule.Show();
        }
        private void RootDialogScheduleCall_ButtonRightClick(object sender, System.Windows.RoutedEventArgs e) => RootDialogScheduleCall.Hide();
        private async void RootDialogScheduleCall_ButtonLeftClick(object sender, RoutedEventArgs e)
        {
            using zhirovContext db = new();

            if (!changeCall)
            {
                Periodclass periodclass = new()
                {
                    PeriodclassesNumber = Int32.Parse(NumberBoxNumberCall.Text),
                    PeriodclassesStart = TimeOnly.FromDateTime(DateTime.Parse(TextBoxStartCall.Text)),
                    PeriodclassesEnd = TimeOnly.FromDateTime(DateTime.Parse(TextBoxEndCall.Text)),
                };
                await db.Periodclasses.AddAsync(periodclass);
                await db.SaveChangesAsync();
            }
            else
            {
                Periodclass? pr = await db.Periodclasses.FirstOrDefaultAsync(p => p.PeriodclassesNumber.ToString() == ListBoxScheduleCall.SelectedItem.ToString().Split().First());
                
                if (pr != null)
                {
                    pr.PeriodclassesNumber = Int32.Parse(NumberBoxNumberCall.Text);
                    pr.PeriodclassesStart = TimeOnly.FromDateTime(DateTime.Parse(TextBoxStartCall.Text));
                    pr.PeriodclassesEnd = TimeOnly.FromDateTime(DateTime.Parse(TextBoxEndCall.Text));

                    await db.SaveChangesAsync();
                }
            }

            FillListBoxScheduleCall();
            RootDialogScheduleCall.Hide();
        }
        private void MenuItemSaveSchedule_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sv = new();
            sv.Filter = "Excel 2007/2010 (*.xlsx)|*.xlsx";
            if (sv.ShowDialog() == true)
            {
                ReoGridSchedule.Save(sv.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007, Encoding.UTF8);
            }
        }
        private void MenuItemSaveAsRGF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sv = new();
            sv.Filter = "ReoGrid Format (*.rgf)|*.rgf";
            if (sv.ShowDialog() == true)
            {
                ReoGridSchedule.Save(sv.FileName, unvell.ReoGrid.IO.FileFormat.ReoGridFormat, Encoding.UTF8);
            }
        }
        private void MenuItemSaveAsHTML_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sv = new();
            sv.Filter = "HTML (*.html)|*.html";
            if (sv.ShowDialog() == true)
            {
                using (FileStream ss = new(sv.FileName, FileMode.Create, FileAccess.Write))
                {
                    ReoGridSchedule.CurrentWorksheet.ExportAsHTML(ss);
                }
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboBoxSchoolWeek();
            FillComboBoxCourse();
        }
        private void ComboBoxSchoolWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReoGridSchedule.CurrentWorksheet.Reset();
            FillGroups();
        }
        private void ListBoxDisciplines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Clipboard.SetText(ListBoxDisciplines.SelectedItem.ToString());
        }
        private void ComboBoxCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReoGridSchedule.CurrentWorksheet.Reset();
            FillGroups();
        }
        private async void FillComboBoxCourse()
        {
            ComboBoxCourse.Items.Clear();
            ComboBoxCourse.Items.Add("Все курсы");
            using zhirovContext db = new();
            await db.Courses.OrderBy(t => t.CourseName).ForEachAsync(t =>
            {
                ComboBoxCourse.Items.Add(t.CourseName);
            });
        }
        private void ButtonDeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mb = new WPFUI.Controls.MessageBox();

                mb.ButtonLeftName = "Да";
                mb.ButtonRightName = "Нет";

                mb.ButtonLeftClick += MessageBox_LeftButtonClick;
                mb.ButtonRightClick += MessageBox_RightButtonClick;

                mb.Show("Оповещение", "Вы точно хотите удалить расписание?\nДанные будут удалены без возможности восстановления.");
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonDeleteSchedule_Click (ScheduleAdmin) | {ex.Message}");
            }
        }
        private async void MessageBox_LeftButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboBoxSchoolWeek.SelectedIndex != -1)
                {
                    using zhirovContext db = new();
                    string[] d = ComboBoxSchoolWeek.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    Schoolweek? s = await db.Schoolweeks.FirstOrDefaultAsync(s => s.SchoolweekStart == DateOnly.Parse(d[0]) && s.SchoolweekEnd == DateOnly.Parse(d[1]));
                    if (s != null)
                    {
                        db.Schoolweeks.Remove(s);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"MessageBox_LeftButtonClick (ScheduleAdmin) | {ex.Message}");
            }
        }
        private void MessageBox_RightButtonClick(object sender, RoutedEventArgs e) => (sender as WPFUI.Controls.MessageBox)?.Close();
        private void CheckComboBox()
        {
            if (ComboBoxCourse.SelectedIndex != -1 && ComboBoxSchoolWeek.SelectedIndex != -1)
            {
                ((Storyboard)Resources["AnimLoad"]).Begin();
                ReoGridSchedule.CurrentWorksheet.Reset();
                ButtonDeleteSchedule.IsEnabled = true;
                FillGroups();
            }
            else if (ComboBoxCourse.SelectedIndex == -1 && ComboBoxSchoolWeek.SelectedIndex != -1)
            {
                ButtonDeleteSchedule.IsEnabled = true;
            }
            else
            {
                GridPreLoad.Visibility = Visibility.Visible;
                ReoGridSchedule.Visibility = Visibility.Collapsed;
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => CheckComboBox();
    }
}
