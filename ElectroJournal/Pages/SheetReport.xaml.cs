using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для SheetReport.xaml
    /// </summary>
    public partial class SheetReport : Page
    {
        public SheetReport(string group2, DateOnly dateS, DateOnly dateE, string d)
        {
            InitializeComponent();
            SettingSheet();
            group = group2;
            dateStart = dateS;
            dateEnd = dateE;
            discipline = d;
            FillStudents(group2);
            FillDispADates();
        }

        List<int> idStud = new();
        List<string> disp = new();
        List<int> numberCall = new();
        private int stuud;
        private string group;
        private string discipline;
        private int daysTable;
        private DateOnly dateStart;
        private DateOnly dateEnd;

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sv = new();
            sv.Filter = "Excel 2007/2010 (*.xlsx)|*.xlsx";
            if (sv.ShowDialog() == true)
            {
                ReoGrid.Save(sv.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007, Encoding.UTF8);
            }
        }
        private void MenuItemSaveAsRGF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sv = new();
            sv.Filter = "ReoGrid Format (*.rgf)|*.rgf";
            if (sv.ShowDialog() == true)
            {
                ReoGrid.Save(sv.FileName, unvell.ReoGrid.IO.FileFormat.ReoGridFormat, Encoding.UTF8);
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
                    ReoGrid.CurrentWorksheet.ExportAsHTML(ss);
                }
            }
        }
        private void SettingSheet()
        {
            var worksheet = ReoGrid.Worksheets[0];
            ReoGrid.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);
            worksheet.SetSettings(WorksheetSettings.View_ShowHeaders, false);
            ReoGrid.ControlStyle = Properties.Settings.Default.Theme == 1 ? new ControlAppearanceStyle(Colors.Black, Colors.WhiteSmoke, false) : new ControlAppearanceStyle(Colors.Gray, Colors.Black, false);

            worksheet["A1"] = "Предметы";

            var cell = worksheet.Cells["A1"]; 
            cell.Style.HAlign = ReoGridHorAlign.Center;
        }
        private async void FillStudents(string group)
        {
            try
            {
                idStud.Clear();

                if (!string.IsNullOrWhiteSpace(group))
                {
                    var worksheet = ReoGrid.CurrentWorksheet;
                    using zhirovContext db = new();

                    var students = await db.Students.Where(p => p.GroupsIdgroupsNavigation.GroupsNameAbbreviated == group).OrderBy(p => p.StudentsSurname).ToListAsync();
                    stuud = students.Count;
                    for (int i = 2; i <= stuud + 2; i++)
                    {
                        worksheet.SetRows(i);
                        worksheet["A" + i] = $"{students[i-2].StudentsSurname} {students[i-2].StudentsName}";
                        worksheet.AutoFitColumnWidth(0, false);

                        idStud.Add((int)students[i - 2].Idstudents);

                        Cell? cell = worksheet.Cells["A" + i];
                        cell.IsReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillStudents (SheetReport) | {ex.Message}");
            }
        }
        private async void FillScore()
        {
            try
            {
                var worksheet = ReoGrid.CurrentWorksheet;
                using zhirovContext db = new();

                var scoreList = await db.Journals
                    .Where(s => s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == discipline &&
                    s.StudentsIdstudentsNavigation.GroupsIdgroupsNavigation.GroupsNameAbbreviated == group)
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
                                        if (score.JournalDay == ReoGrid.CurrentWorksheet.Cells[0, j].DisplayText && score.StudentsIdstudents == idStud[i - 1] && score.ScheduleIdscheduleNavigation.PeriodclassesIdperiodclassesNavigation.PeriodclassesNumber == numberCall[j - 1])
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
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillScore | {ex.Message}");
            }
        }
        private async void FillDispADates()
        {
            try
            {
                using zhirovContext db = new();
                var worksheet = ReoGrid.CurrentWorksheet;


                var s = await db.Schedules
                        .Where(s => s.GroupsIdgroupsNavigation.GroupsNameAbbreviated == group
                    && s.ScheduleDate.Year == dateEnd.Year
                    
                    && s.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == discipline)
                        .Include(s => s.PeriodclassesIdperiodclassesNavigation)
                        .ToListAsync();

                daysTable = s.Count + 1;

                if (s.Count == 0)
                {
                    
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
                    FillScore();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
