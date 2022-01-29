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
using System.Data;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        Classes.DataBase DbUser = new Classes.DataBase();
        Classes.DataBaseControls DbControls = new Classes.DataBaseControls();
        MySqlConnection conn = Classes.DataBase.GetDBConnection();

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `teachers` WHERE `login` = @log AND `password` = @pass", conn);

            command.Parameters.Add("@log", MySqlDbType.VarChar).Value = TextBoxLogin.Text;
            command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = DbControls.Hash(TextBoxPassword.Password);

            adapter.SelectCommand = command;
            adapter.Fill(table);

            string textTeacher;

            if (TextBoxLogin.Text != "" || TextBoxPassword.Password != "")
            {
                if (table.Rows.Count > 0)
                {
                    //LoadMenu();
                    //TextBlockJournalOpen.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                    MySqlCommand command2 = new MySqlCommand("SELECT `login`, `LastName`, `FirstName`, `MiddleName` FROM `teachers`", conn); //Команда выбора данных
                    conn.Open(); //Открываем соединение
                    MySqlDataReader read = command2.ExecuteReader(); //Считываем и извлекаем данные
                    while (read.Read()) //Читаем пока есть данные
                    {
                        if (TextBoxLogin.Text == read.GetString(0))
                        {
                            textTeacher = read.GetString(1) + " " + read.GetString(2) + " " + read.GetString(3);
                            break;
                        }
                    }

                    conn.Close(); //Закрываем соединение
                    //GridNotificationsAnim("Авторизация успешно завершена");
                }
                else { }
            }
            else { }
        }
    }
}
