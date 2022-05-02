using ElectroJournal.Classes;
using MySql.Data.MySqlClient;
using System.Windows;

namespace ElectroJournal.Pages.AdminPanel.Schedule
{
    /// <summary>
    /// Логика взаимодействия для ScheduleCall.xaml
    /// </summary>
    public partial class ScheduleCall : Window
    {
        public ScheduleCall()
        {
            InitializeComponent();
        }

        DataBaseConn DbUser = new DataBaseConn();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBaseConn.GetDBConnection();

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO periodclasses (`periodclasses_start`, `periodclasses_end`, `periodclasses_number`) " +
                "VALUES (@start, @end, @number)", conn);

            command.Parameters.Add("@start", MySqlDbType.VarChar).Value = TimePickerStart.Text;
            command.Parameters.Add("@end", MySqlDbType.VarChar).Value = TimePickerEnd.Text;
            command.Parameters.Add("@number", MySqlDbType.VarChar).Value = NumberBoxNumber.Text;

            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
                //MainWindow.Notifications("Сообщение", "Данные сохранены");
            }
        }
    }
}
