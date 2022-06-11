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
        public SheetReport(string group2, DateOnly dateS, DateOnly dateE)
        {
            InitializeComponent();
            SettingSheet();
            group = group2;
            dateStart = dateS;
            dateEnd = dateE;
            FillStudents(group2);
            FillDispADates();
        }

        List<int> idStud = new();
        private int stuud;
        private string group;
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
            worksheet["A2"] = "Студенты\\Даты";

            var cell = worksheet.Cells["A1"]; 
            var cell2 = worksheet.Cells["A2"];
            cell.Style.HAlign = ReoGridHorAlign.Center;
            cell2.Style.HAlign = ReoGridHorAlign.Center;
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
                    for (int i = 3; i <= stuud + 3; i++)
                    {
                        worksheet.SetRows(i);
                        worksheet["A" + i] = $"{students[i-3].StudentsSurname} {students[i-3].StudentsName}";
                        worksheet.AutoFitColumnWidth(0, false);

                        idStud.Add((int)students[i - 3].Idstudents);

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
        private async void FillDispADates()
        {
            try
            {
                using zhirovContext db = new();
                var worksheet = ReoGrid.CurrentWorksheet;

                var disp = await db.Schedules.Where(d => d.GroupsIdgroupsNavigation.GroupsNameAbbreviated == group
                && (dateStart < d.ScheduleDate && d.ScheduleDate < dateEnd)
                && d.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated != "Обед")
                    .Include(d => d.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated)
                    .ToListAsync();

                if (disp != null) 
                {
                    for (int i = 1; i < disp.Count + 1; i++)
                    {
                        //worksheet.SetCols(disp.Count);
                        worksheet[0, i] = disp[i - 1].DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated;
                        ReoGrid.DoAction(new SetColumnsWidthAction(1, i, 30));

                        Cell? cell = worksheet.Cells[0, i];
                        cell.IsReadOnly = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
