using ElectroJournal.DataBase;
using ElectroJournal.Pages.AdminPanel.Schedule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            //ListBoxScheduleRerfresh();
            //LoadDataGridJournal();
            FillDates();
            var ws = ReoGridSchedule.Worksheets[0];
            ws.CellDataChanged += rgrid_AfterCellEdit;
        }

        // DataBaseConn DbUser = new DataBaseConn();
        //DataBaseControls DbControls = new DataBaseControls();
        //MySqlConnection conn = DataBaseConn.GetDBConnection();

        int idSchedule = 0;

        private void IconAddScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new ScheduleCall().ShowDialog();
        }

        private async void IconDeleteScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxSchedule.SelectedIndex != -1)
            {
                using zhirovContext db = new();
                Periodclass? s = await db.Periodclasses.Where(s => s.PeriodclassesNumber == Int32.Parse(ListBoxSchedule.SelectedItem.ToString().Split().First())).FirstOrDefaultAsync();

                if (s != null)
                {
                    db.Periodclasses.Remove(s);
                    await db.SaveChangesAsync();

                    FillScheduleCall();
                }
            }
            
        }
        private async void FillScheduleCall()
        {
            ListBoxSchedule.Items.Clear();

            using zhirovContext db = new();

            var s = await db.Periodclasses.OrderBy(s => s.PeriodclassesNumber).ToListAsync();

            if (s != null)
            {
                foreach (var t in s)
                {
                    ListBoxSchedule.Items.Add($"{t.PeriodclassesNumber} | {t.PeriodclassesStart} - {t.PeriodclassesEnd}");
                }
            }
        }
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

        private void ReoGridSchedule_WorkbookLoaded(object sender, EventArgs e)
        {
            //SettingSheet();

        }
        void rgrid_AfterCellEdit(object sender, CellEventArgs e)
        {
            WorksheetRangeStyle MyStyle = new WorksheetRangeStyle();
            MyStyle.Flag = PlainStyleFlag.BackColor;
            string[] poz = ReoGridSchedule.CurrentWorksheet.SelectionRange.ToString().Split(new char[] { ':' });
            int poz5 = int.Parse(Regex.Match(poz[0], @"\d+").Value);

            var worksheet = ReoGridSchedule.CurrentWorksheet;
            if (ReoGridSchedule.CurrentWorksheet.Cells[poz5 - 1, 0].DisplayText == ReoGridSchedule.CurrentWorksheet.Cells[poz[0]].DisplayText)
            {
                worksheet.SetRangeStyles(poz[0], new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = Colors.IndianRed
                });
            }
            Test.Content = $"{poz[0]}";
        }
        private void FillDates()
        {
            ReoGridSchedule.CurrentWorksheet.SetRangeDataFormat(RangePosition.EntireRange, unvell.ReoGrid.DataFormat.CellDataFormatFlag.Text, null);
            ReoGridSchedule.CurrentWorksheet.MergeRange(new RangePosition(1, 1, 1, 5));
            ReoGridSchedule.CurrentWorksheet.MergeRange(new RangePosition(7, 1, 1, 5));
            ReoGridSchedule.CurrentWorksheet.MergeRange(new RangePosition(13, 1, 1, 5));
            var wk = ReoGridSchedule.CurrentWorksheet;
            ReoGridSchedule.CurrentWorksheet[1, 1] = "Понедельник 14.03.2022";
            ReoGridSchedule.CurrentWorksheet[7, 1] = "Вторник 15.03.2022";
            ReoGridSchedule.CurrentWorksheet[13, 1] = "Среда 16.03.2022";

            wk.SetRangeStyles("A1:ZZ150", new WorksheetRangeStyle
            {
                Flag = PlainStyleFlag.HorizontalAlign,
                HAlign = ReoGridHorAlign.Center
            });
        }
        private void ExpanderScheduleCall_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillScheduleCall();
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
            ListBoxDisciplines.Items.Clear();

            using zhirovContext db = new();
            await db.Disciplines.OrderBy(c => c.DisciplinesNameAbbreviated).ForEachAsync(c =>
            {
                ListBoxDisciplines.Items.Add(c.DisciplinesNameAbbreviated);
            });
        }
        private async void FillListBoxTeachers()
        {
            ListBoxTeachers.Items.Clear();

            using zhirovContext db = new();

            await db.Teachers.OrderBy(t => t.TeachersSurname).ForEachAsync(t =>
            {
                ListBoxTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName}");
            });
        }
        private void ExpanderCabinets_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillComboBoxHousing();
        private void ExpanderDisciplines_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillListBoxDisciplines();
        private void ExpanderTeachers_Expanded(object sender, System.Windows.RoutedEventArgs e) => FillListBoxTeachers();
    }
}
