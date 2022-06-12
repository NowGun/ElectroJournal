using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Events;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Journal.xaml
    /// </summary>
    public partial class Journal
    {
        public Journal()
        {
            InitializeComponent();

            CheckComboBox();
            var ws = ReoGrid.Worksheets[0];
            ws.CellDataChanged += rgrid_AfterCellEdit;
            ws.BeforeCellEdit += rgrid_BeforeCellEdit;
        }

        private int stuud;
        private int daysTable;
        List<int> idStud = new();
        List<int> numberCall = new();
        string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
        private bool isChange = false;
        private bool isEdit = false;

        private void FillTable()
        {
            isEdit = false;

            if (ComboBoxDisp.SelectedIndex == -1 || ComboBoxMonth.SelectedIndex == -1 || ComboBoxYears.SelectedIndex == -1)
            {
                LabelData.Visibility = Visibility.Visible;
                ReoGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                LabelData.Visibility = Visibility.Collapsed;
                ReoGrid.Visibility = Visibility.Visible;

                var anim = (Storyboard)FindResource("AnimOpenLoad");
                anim.Begin();

                SettingSheet();
                FillText();
                FillStudents();
                FillDates();

                
            }
        }
        private async void FillScore()
        {
            try
            {
                if (((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1 && ComboBoxDisp.SelectedIndex != -1)
                {
                    var worksheet = ReoGrid.CurrentWorksheet;
                    using zhirovContext db = new();
                    if (ComboBoxYears.SelectedItem != null)
                    {
                        var scoreList = await db.Journals
                            .Where(s => s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == ComboBoxDisp.SelectedItem.ToString() &&
                            s.StudentsIdstudentsNavigation.GroupsIdgroupsNavigation.GroupsNameAbbreviated == ((MainWindow)Application.Current.MainWindow).ComboBoxGroup .SelectedItem.ToString() 
                            && s.StudyperiodIdstudyperiodNavigation.StudyperiodStart == ComboBoxYears.SelectedItem.ToString() 
                            && s.JournalMonth == Convert.ToString(ComboBoxMonth.SelectedIndex + 1))
                            .Include(s => s.ScheduleIdscheduleNavigation.PeriodclassesIdperiodclassesNavigation)
                            .ToListAsync();

                        await Task.Run(() =>
                        {
                            foreach (var score in scoreList)
                            {
                                for (int i = 1; i <= stuud; i++)
                                {
                                    for (int j = 1; j < daysTable; j++)
                                    {
                                        if (!String.IsNullOrWhiteSpace(ReoGrid.CurrentWorksheet.Cells[i, 0].DisplayText))
                                        {
                                            if (score != null)
                                            {
                                                if (score.JournalDay == ReoGrid.CurrentWorksheet.Cells[0, j].DisplayText && score.StudentsIdstudents == idStud[i-1] && score.ScheduleIdscheduleNavigation.PeriodclassesIdperiodclassesNavigation.PeriodclassesNumber == numberCall[j-1])
                                                {
                                                    Dispatcher.Invoke(() =>
                                                    {
                                                        ReoGrid.CurrentWorksheet[i, j] = score.JournalScore;
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        isEdit = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillScore | {ex.Message}");
            }
            finally
            {
                var anim = (Storyboard)FindResource("AnimCloseLoad");
                anim.Begin();
            }
        }
        private async void FillStudents()
        {
            try
            {
                idStud.Clear();

                if (((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1)
                {
                    var worksheet = ReoGrid.CurrentWorksheet;
                    using zhirovContext db = new();

                    var days = await db.Groups.Where(p => p.GroupsNameAbbreviated == ((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedItem.ToString()).Select(p => p.Idgroups).ToListAsync();
                    var students = await db.Students.Where(p => p.GroupsIdgroups == days[0]).OrderBy(p => p.StudentsSurname).ToListAsync();
                    stuud = students.Count;
                    for (int i = 2; i - 2 < stuud; i++)
                    {
                        worksheet.SetRows(i);
                        worksheet["A" + i] = students[i - 2].StudentsSurname + " " + students[i - 2].StudentsName;
                        worksheet.AutoFitColumnWidth(0, false);

                        idStud.Add((int)students[i - 2].Idstudents);

                        Cell? cell = worksheet.Cells["A" + i];
                        cell.IsReadOnly = true;
                    }
                }
                FillScore();
            }
            catch (Exception ex) 
            {
                SettingsControl.InputLog($"FillStudents | {ex.Message}");
            }
        }
        private async void FillDates()
        {
            try
            {
                if (ComboBoxDisp.SelectedIndex != -1 && ComboBoxMonth.SelectedIndex != -1 && ((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1)
                {
                    var worksheet = ReoGrid.CurrentWorksheet;

                    using zhirovContext db = new();
                    var s = await db.Schedules
                        .Where(s => s.GroupsIdgroupsNavigation.GroupsNameAbbreviated == ((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedItem.ToString()
                    && s.ScheduleDate.Month == ComboBoxMonth.SelectedIndex + 1
                    && s.ScheduleDate.Year == int.Parse(CheckYear())
                    && s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == ComboBoxDisp.SelectedItem.ToString())
                        .Include(s => s.PeriodclassesIdperiodclassesNavigation)
                        .ToListAsync();

                    daysTable = s.Count + 1;

                    if (s.Count == 0)
                    {
                        ReoGrid.Visibility = Visibility.Collapsed;
                        LabelData.Content = "Занятия отсутствуют";
                        LabelData.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (s != null)
                        {
                            for (int i = 1; i < daysTable; i++)
                            {
                                worksheet.SetCols(daysTable);
                                worksheet[0, i] = s[i - 1].ScheduleDate.Day;
                                worksheet[1, i] = "";
                                ReoGrid.DoAction(new SetColumnsWidthAction(1, i, 30));

                                Cell? cell = worksheet.Cells[0, i];
                                cell.IsReadOnly = true;

                                numberCall.Add(s[i - 1].PeriodclassesIdperiodclassesNavigation.PeriodclassesNumber);
                            }

                            worksheet.SetRangeStyles("B1:BP150", new WorksheetRangeStyle
                            {
                                Flag = PlainStyleFlag.HorizontalAlign,
                                HAlign = ReoGridHorAlign.Center,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillDates | {ex.Message}");
            }
        }
        private void FillText()
        {
            var worksheet = ReoGrid.CurrentWorksheet;
            worksheet["A1"] = "ФИО\\День месяца";
        }
        private void SettingSheet()
        {
            var worksheet = ReoGrid.Worksheets[0];
            ReoGrid.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);
            worksheet.SetSettings(WorksheetSettings.View_ShowHeaders, false);
            ReoGrid.ControlStyle = Properties.Settings.Default.Theme == 1 ? new ControlAppearanceStyle(Colors.Black, Colors.WhiteSmoke, false) : new ControlAppearanceStyle(Colors.Gray, Colors.Black, false);
        }
        private void ReoGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!((e.Text == "н") || (e.Text == "Н") || (e.Text == "а") || (e.Text == "2") || (e.Text == "3") || (e.Text == "4") || (e.Text == "5") || (e.Text == "/") || (e.Text == "А")))
            {
                e.Handled = true;
            }
        }
        private async void FillComboBoxDisp()
        {
            ComboBoxDisp.Items.Clear();
            using zhirovContext db = new();
            await db.TeachersHasDisciplines.Where(g => g.TeachersIdteachers == Properties.Settings.Default.UserID).Include(g => g.DisciplinesIddisciplinesNavigation).ForEachAsync(g => ComboBoxDisp.Items.Add(g.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated));
        }
        private async void FillComboBoxYears()
        {
            ComboBoxYears.Items.Clear();
            using zhirovContext db = new();
            await db.Studyperiods.OrderByDescending(t => t.StudyperiodStart).ForEachAsync(t => ComboBoxYears.Items.Add(t.StudyperiodStart));
            ComboBoxYears.SelectedIndex = 0;
        }
        private void FillComoBoxMonth()
        {
            foreach (var month in monthNames)
            {
                ComboBoxMonth.Items.Add(month);
            }
        }
        private async void SaveJournal(int students, string disciplines, int teachers, string studyPeriod, string score, string time, int call)
        {
            using zhirovContext db = new();
            string[] time2 = time.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            var disp = await db.Disciplines.Where(t => t.DisciplinesNameAbbreviated == disciplines).FirstOrDefaultAsync();
            var period = await db.Studyperiods.Where(t => t.StudyperiodStart == studyPeriod).FirstOrDefaultAsync();
            var periodCall = await db.Schedules
                .Where(p => p.ScheduleDate == DateOnly.Parse($"{CheckYear()}-{ComboBoxMonth.SelectedIndex + 1}-{time2[2]}") 
                && p.PeriodclassesIdperiodclassesNavigation.PeriodclassesNumber == numberCall[call-1]
                && p.GroupsIdgroupsNavigation.GroupsNameAbbreviated == ((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedItem.ToString())
                .FirstOrDefaultAsync();

            if (!isChange)
            {
                if (disp != null & period != null && periodCall != null && !String.IsNullOrWhiteSpace(score))
                {
                    Classes.DataBaseEF.Journal journal = new Classes.DataBaseEF.Journal
                    {
                        StudentsIdstudents = (uint)students,
                        DisciplinesIddisciplines = disp.Iddisciplines,
                        TeachersIdteachers = (uint)teachers,
                        StudyperiodIdstudyperiod = period.Idstudyperiod,
                        JournalYear = time2[0],
                        JournalMonth = time2[1],
                        JournalDay = time2[2],
                        JournalScore = score,
                        ScheduleIdschedule = periodCall.Idschedule
                    };

                    await db.Journals.AddAsync(journal);
                    await db.SaveChangesAsync();
                }
            }
            else
            {
                if (disp != null & period != null)
                {
                    Classes.DataBaseEF.Journal? j = await db.Journals.FirstOrDefaultAsync(t => t.StudentsIdstudents == students && t.DisciplinesIddisciplines == disp.Iddisciplines && t.StudyperiodIdstudyperiod == period.Idstudyperiod && t.JournalDay == time2[2]);

                    if (j != null)
                    {
                        j.JournalScore = score;

                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        private void ReoGrid_WorkbookLoaded(object sender, EventArgs e) => FillTable();
        private void rgrid_AfterCellEdit(object sender, CellEventArgs e)
        {
            string[] poz = ReoGrid.CurrentWorksheet.SelectionRange.ToString().Split(new char[] { ':' });
            int pozDate = ReoGrid.CurrentWorksheet.SelectionRange.Col;
            string score = poz[0];
            int stud = int.Parse(Regex.Match(score, @"\d+").Value);
            string poz6 = Regex.Replace(score, @"[^A-Z]+", string.Empty);

            if (isEdit && ComboBoxDisp.SelectedItem != null && !String.IsNullOrWhiteSpace(ReoGrid.CurrentWorksheet.Cells[stud - 1, 0].DisplayText))
                SaveJournal(idStud[stud-2], ComboBoxDisp.SelectedItem.ToString(), Properties.Settings.Default.UserID, ComboBoxYears.SelectedItem.ToString(), ReoGrid.CurrentWorksheet.Cells[score].DisplayText, $"{CheckYear()}-{ComboBoxMonth.SelectedIndex + 1}-{ReoGrid.CurrentWorksheet.Cells[$"{poz6}1"].DisplayText}", pozDate);
        }
        private void rgrid_BeforeCellEdit(object sender, CellEventArgs e)
        {            
            string[] poz = ReoGrid.CurrentWorksheet.SelectionRange.ToString().Split(new char[] { ':' });
            string score = poz[0];
            isChange = String.IsNullOrWhiteSpace(ReoGrid.CurrentWorksheet.Cells[score].DisplayText) ? false : true;
        }
        private string CheckYear()
        {
            string[] year = ComboBoxYears.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (ComboBoxMonth.SelectedIndex <= 6)
                return year[1];
            else if (ComboBoxMonth.SelectedIndex >= 8)
                return year[0];

            return null;
        }
        private async void CheckGroup()
        {
            try
            {
                if (((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1)
                {
                    using zhirovContext db = new();
                    var g = await db.Groups.FirstOrDefaultAsync(g => g.GroupsNameAbbreviated == ((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedItem.ToString() && g.TeachersIdteachers == Properties.Settings.Default.UserID);

                    if (g != null)
                    {
                        GridYears.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        GridYears.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e) => CheckGroup();
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillTable();
        private async void CheckComboBox()
        {
            if (((MainWindow)Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1)
            {
                LabelGroup.Visibility = Visibility.Collapsed;
                ReoGrid.Visibility = Visibility.Visible;
                CardMenu.Visibility = Visibility.Visible;

                FillComoBoxMonth();
                ComboBoxMonth.SelectedIndex = DateTime.Now.Month - 1;
                FillComboBoxDisp();
                FillComboBoxYears();

                ComboBoxDisp.SelectedItem = await JournalClass.CheckSchedule();
            }
            else
            {
                LabelGroup.Visibility = Visibility.Visible;
                ReoGrid.Visibility = Visibility.Collapsed;
                CardMenu.Visibility = Visibility.Collapsed;
            }
        }
    }
}