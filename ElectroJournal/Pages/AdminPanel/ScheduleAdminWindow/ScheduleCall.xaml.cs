using ElectroJournal.Classes;
using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;

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
                //SendPasswordToUser();
                //SendSMSToUser();
                //MainWindow.Notifications("Сообщение", "Данные сохранены");
            }
        }
    }
}
