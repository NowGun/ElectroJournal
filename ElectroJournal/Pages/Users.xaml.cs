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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElectroJournal.Windows;
using MySql.Data.MySqlClient;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using ElectroJournal.Classes;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        public Users()
        {
            InitializeComponent();
            LoadData();
        }

        DataBase DbUser = new DataBase();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBase.GetDBConnection();

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            MySqlCommand command = new MySqlCommand("UPDATE `teachers` SET `teachers_phone` = @phone, `teachers_mail` = @mail WHERE `idteachers` = @id", conn);

            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Properties.Settings.Default.UserID.ToString();
            command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = TextBoxPhone.Text;
            command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = TextBoxMail.Text;
            
            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данные обновлены");
            }
            conn.Close();
        }

        private void ButtonResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            //((MainWindow)Application.Current.MainWindow).AnimLog(true);

            new Windows.ResetPassword().ShowDialog();
        }

        private void LabelChangePhoto_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void LabelChangePhoto_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void LabelChangePhoto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
        }

        public void ButtonLogoutUser_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).GridNLogin.Visibility = Visibility.Visible;
            //((MainWindow)Application.Current.MainWindow).AnimLog(true);
        }

        private void LoadData()
        {
            MySqlCommand command = new MySqlCommand("SELECT `teachers_password`, `teachers_surname`, `teachers_name`, `teachers_patronymic`, `idteachers`, `teachers_phone`, `teachers_mail` FROM `teachers` WHERE `idteachers` = @id", conn); //Команда выбора данных

            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Properties.Settings.Default.UserID;
            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            
            if (read.Read())
            {
                TextBlockFIO.Content = read.GetString(1) + " " + read.GetString(2) + " " + read.GetString(3);
                PasswordBoxPassword.Password = read.GetString(0);
                LabelIDUser.Content = "Id: " + read.GetString(4);
                TextBoxPhone.Text = read.GetString(5);
                TextBoxMail.Text = read.GetString(6);

                ButtonSave.IsEnabled = false;
            }
            conn.Close(); //Закрываем соединение
        }

        private void TextBoxPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ButtonSave.IsEnabled = true;
        }

        private void TextBoxMail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ButtonSave.IsEnabled = true;
        }
    }
}
