using ElectroJournal.Pages.AdminPanel.Schedule;
using System;
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

        private void IconDeleteScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new ScheduleCall().ShowDialog();
        }
        /*
        private void ListBoxScheduleRerfresh()
        {
            ListBoxSchedule.Items.Clear();

            MySqlCommand command = new MySqlCommand("SELECT `idperiodclasses`, date_format(`periodclasses_start`, '%H:%i'), date_format(`periodclasses_end`, '%H:%i'), `periodclasses_number` FROM `periodclasses`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                ListBoxSchedule.Items.Add(read.GetValue(3).ToString() + " | " + read.GetValue(1).ToString() + " - " + read.GetValue(2).ToString());
            }
            conn.Close(); //Закрываем соединение
                          //ButtonSaveTeacher.IsEnabled = false;
        }
        private async void LoadDataGridJournal()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM schedule", conn);

            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            await Task.Run(() =>
            {
                da.Fill(dt);

            });
            DataGridShedule.ItemsSource = dt.AsDataView();
        }

        */

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
    }
}
