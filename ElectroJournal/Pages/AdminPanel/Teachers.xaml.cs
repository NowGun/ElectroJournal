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
using MySql.Data.MySqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NickBuhro.Translit;
using System.Net.Mail;
using System.Net;
using ElectroJournal.Classes;
using System.Windows.Forms;
using SmsRu;
using System.Drawing;

namespace ElectroJournal.Pages.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для Teachers.xaml
    /// </summary>
    public partial class Teachers : Page
    {
        public Teachers()
        {
            InitializeComponent();

            ListViewTeachersRefresh();
            //TextBoxTeachersPassword.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(16);

        }

        DataBase DbUser = new DataBase();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBase.GetDBConnection();


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string[] FIO = TextBoxTeachersFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            ProgressBarTeachers.Visibility = Visibility.Visible;

            MySqlCommand command = new MySqlCommand("INSERT INTO teachers (`teachers_login`, `teachers_password`, `teachers_name`, `teachers_surname`, `teachers_patronymic`," +
                " `teachers_acces_admin_panel`, `teachers_phone`, `teachers_mail`) VALUES (@login, @password, @name, @surname, @patronymic, @admin, @phone, @mail)", conn);

            //MySqlCommand command = new MySqlCommand("CREATE USER @login @`%` IDENTIFIED BY @pass", conn);

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxTeachersLogin.Text;
            //command.Parameters.Add("@password", MySqlDbType.VarChar).Value = PBKDF2HashHelper.CreatePasswordHash(PasswordBoxTeachers.Password);
            command.Parameters.Add("@password", MySqlDbType.VarChar).Value = DbControls.Hash(PasswordBoxTeachers.Password);
            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = FIO[1];
            command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = FIO[0];
            command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = FIO[2];
            //command.Parameters.Add("@image", MySqlDbType.VarChar).Value = words[2];
            command.Parameters.Add("@admin", MySqlDbType.VarChar).Value = CheckBoxAdminAccess.IsChecked.ToString();
            command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = TextBoxTeachersPhone1.Text;
            command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = TextBoxTeachersMail1.Text;


            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
                //SendPasswordToUser();
                //SendSMSToUser();
                ListViewTeachersRefresh();
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                ProgressBarTeachers.Visibility = Visibility.Hidden;
            }
        }

        private async void ListViewTeachersRefresh()
        {
            ListViewTeachers.Items.Clear();


            MySqlCommand command = new MySqlCommand("SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers`", conn); //Команда выбора данных
            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (await read.ReadAsync()) //Читаем пока есть данные
            {
                if (read.GetValue(1).ToString() != "" && read.GetValue(2).ToString() != "" && read.GetValue(3).ToString() != "")
                    ListViewTeachers.Items.Add(read.GetValue(0).ToString() + " | " + read.GetValue(2).ToString() + " " + read.GetValue(1).ToString() + " " + read.GetValue(3).ToString()); //Добавляем данные в лист итем
            }
            conn.Close(); //Закрываем соединение
                          //ButtonSaveTeacher.IsEnabled = false;

        }



        private void TextBoxTeachersPhone2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void RectangleTeachersPhoto_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
        }



        private void TextBoxTeachersFIO_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            PasswordBoxTeachers.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(16);

            TextBoxTeachersLogin.Text = Transliteration.CyrillicToLatin(TextBoxTeachersFIO.Text.Split().First(), NickBuhro.Translit.Language.Russian);
        }

        private async void SendPasswordToUser()
        {
            MainWindow mainWindow = new MainWindow();
            string user = mainWindow.TextBlockTeacher.Text;
            bool a = true;
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("zhirowdaniil@gmail.com", user);
            // кому отправляем
            MailAddress to = new MailAddress(TextBoxTeachersMail1.Text);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = Title;
            // текст письма
            m.Body = "Добро пожаловать в систему Электронный журнал\n\nАдминистратор зарегистрироал Вас в системе электронного журнала, ниже написаны данные для входа в вашу учетную запись.\nМы рекомендуем при первой возможности поменять пароль на более удобный Вам, так как нынешний пароль является временным." +
                "\n\nЛогин: " + TextBoxTeachersLogin.Text + "\nПароль: " + PasswordBoxTeachers.Password + "\n\n\n\n";
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.elasticemail.com", 2525);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("zhirowdaniil@gmail.com", "E0E7027197724CDBDAFAD917FB914057C0CB");
            smtp.EnableSsl = true;

            await Task.Run(() =>
            {
                try
                {
                    smtp.Send(m);
                }
                catch (SmtpException)
                {
                    a = true;
                }
            });

            if (!a)
            {
                mainWindow.Notifications("Сообщение", "письмо отправлено");
                //ButtonSend.IsEnabled = true;
                //anim.Begin();
                //RectangleLoad.Visibility = Visibility.Hidden;
            }
            else if (a)
            {
                mainWindow.Notifications("Сообщение", "письмо не отправилось");
                //ButtonSend.IsEnabled = true;
                //anim.Begin();
                //RectangleLoad.Visibility = Visibility.Hidden;
            }
        }

        private void SendSMSToUser() // СТОИТ ОЧЕНЬ ДОРОГО, ИСПОЛЬЗОВАТЬ ТОЛЬКО В КРАЙНИХ СЛУЧАЯХ!!!!
        {
            string myApiKey = "B0361252-C8BA-5438-E643-4651FCC4E55B"; //Ваш API ключ
            SmsRu.SmsRu sms = new SmsRu.SmsRu(myApiKey);
            var response = sms.Send(TextBoxTeachersPhone1.Text, "Добро пожаловать в систему Электронный журнал\n\nАдминистратор зарегистрироал Вас в системе электронного журнала, ниже написаны данные для входа в вашу учетную запись.\nМы рекомендуем при первой возможности поменять пароль на более удобный Вам, так как нынешний пароль является временным." +
                "\n\nЛогин: " + TextBoxTeachersLogin.Text + "\nПароль: " + PasswordBoxTeachers.Password);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            if (ListViewTeachers.Items.Count == 0)
            {
                mainwindow.Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListViewTeachers.SelectedItem != null)
            {
                string name_test = ListViewTeachers.SelectedItem.ToString().Split().First();
                DbControls.DeleteTeachers(name_test);
                ListViewTeachers.Items.Clear();
                ListViewTeachersRefresh();
            }
        }

        private void ListViewTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //ButtonSaveTeacher.IsEnabled = true;
            if (ListViewTeachers.SelectedItem != null) //если строка выделена выполняется условие
            {
                MySqlCommand command = new MySqlCommand("SELECT `idteachers`, `teachers_login`, `teachers_password`, `teachers_name`, `teachers_surname`, `teachers_patronymic`, `teachers_image`, `teachers_acces_admin_panel` FROM `teachers` WHERE idteachers = @id", conn); //Команда выбора данных

                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Int32.Parse(ListViewTeachers.SelectedItem.ToString().Split().First());

                conn.Open(); //Открываем соединение
                MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные

                while (read.Read()) //Читаем пока есть данные
                {
                    
                    string FIO = read.GetString(4) + " " + read.GetString(3) + " " + read.GetString(5);
                    
                        TextBoxTeachersFIO.Text = FIO;
                        TextBoxTeachersLogin.Text = read.GetString(1);
                        PasswordBoxTeachers.Password = read.GetString(2);
                        CheckBoxAdminAccess.IsChecked = bool.Parse(read.GetString(7));
                        break;
                    
                }
                conn.Close(); //Закрываем соединение
            }
        }
    }
}
