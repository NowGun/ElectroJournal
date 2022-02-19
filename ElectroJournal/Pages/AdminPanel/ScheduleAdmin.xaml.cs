using ElectroJournal.Windows;
using ElectroJournal.Pages.AdminPanel.Schedule;
using System;
using System.Collections.Generic;
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
using MySql.Data.MySqlClient;
using ElectroJournal.Classes;
using System.Data;

namespace ElectroJournal.Pages.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для ScheduleAdmin.xaml
    /// </summary>
    public partial class ScheduleAdmin : Page
    {
        public ScheduleAdmin()
        {
            InitializeComponent();

            ListBoxScheduleRerfresh();
            LoadDataGridJournal();
        }

        DataBaseConn DbUser = new DataBaseConn();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBaseConn.GetDBConnection();

        int idSchedule = 0;

        private void IconAddScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
          new ScheduleCall().ShowDialog();
        }

        private void IconDeleteScheduleCall_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new ScheduleCall().ShowDialog();
        }

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
    }
}
