using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using ElectroJournal.Pages.AdminPanel.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
        private bool changeCall = false;

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
        private async void FillListBoxTeachers()
        {
            ListBoxTeachers.Items.Clear();
            using zhirovContext db = new();

            if (String.IsNullOrWhiteSpace(SearchBoxTeachers.Text))
            {
                await db.Teachers.OrderBy(t => t.TeachersSurname).ForEachAsync(t => ListBoxTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName}"));
            }
            else
            {
                await db.Teachers
                    .OrderBy(t => t.TeachersSurname)
                    .Where(t => EF.Functions.Like(t.TeachersName, $"%{SearchBoxTeachers.Text}%") ||
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
        private void RootDialogNewSchedule_ButtonLeftClick(object sender, System.Windows.RoutedEventArgs e)
        {
            RootDialogNewSchedule.Hide();
        }
        private void RootDialogNewSchedule_ButtonRightClick(object sender, System.Windows.RoutedEventArgs e) => RootDialogNewSchedule.Hide();
        private void ButtonNewSchedule_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RootDialogNewSchedule.Show();
        }
        private void RootDialogScheduleCall_ButtonRightClick(object sender, System.Windows.RoutedEventArgs e) => RootDialogScheduleCall.Hide();
        private async void RootDialogScheduleCall_ButtonLeftClick(object sender, System.Windows.RoutedEventArgs e)
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
        private void MenuItemSaveSchedule_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog sv = new();
            sv.Filter = "Excel 2007/2010 (*.xlsx)|*.xlsx";
            if (sv.ShowDialog() == true)
            {
                ReoGridSchedule.Save(sv.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007, Encoding.UTF8);
            }
        }
        private void MenuItemSaveAsRGF_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog sv = new();
            sv.Filter = "ReoGrid Format (*.rgf)|*.rgf";
            if (sv.ShowDialog() == true)
            {
                ReoGridSchedule.Save(sv.FileName, unvell.ReoGrid.IO.FileFormat.ReoGridFormat, Encoding.UTF8);
            }
        }
        private void MenuItemSaveAsHTML_Click(object sender, System.Windows.RoutedEventArgs e)
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
        private void MenuItemPrintSchedule_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            /*var session = ReoGridSchedule.CurrentWorksheet.CreatePrintSession();
            session.Print();*/
        }
    }
}
