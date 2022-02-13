using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
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
using System.Xml;

namespace StartEJ.Pages
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        public Admin()
        {
            InitializeComponent();
        }

        XmlDocument xmlDocument = new XmlDocument();
        

        private void TextBoxIP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")))
            {
                e.Handled = true;
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxIP.Text) && !string.IsNullOrWhiteSpace(TextBoxLogin.Text) && !string.IsNullOrWhiteSpace(TextBoxPassword.Text) 
                && !string.IsNullOrWhiteSpace(TextBoxFIO.Text) && !string.IsNullOrWhiteSpace(TextBoxMail.Text))
            {
                xmlDocument.Load("setting.xml");

                XmlNode server = xmlDocument.GetElementsByTagName("server")[0];
                XmlNode username = xmlDocument.GetElementsByTagName("username")[0];
                XmlNode password = xmlDocument.GetElementsByTagName("password")[0];

                server.InnerText = TextBoxIP.Text;
                username.InnerText = TextBoxLogin.Text;
                password.InnerText = TextBoxPassword.Text;

                xmlDocument.Save("setting.xml");

                AddUser();

                
            }
            else            
            {
                
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните все поля");
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).FrameEJ.Navigate(new Pages.ConnectDB());
        }

        private async void AddUser()
        {
            MySqlConnection conn = GetDBConnection();

            string[] FIO = TextBoxFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            MySqlCommand command = new MySqlCommand("INSERT INTO teachers (`teachers_login`, `teachers_password`, `teachers_name`, `teachers_surname`, `teachers_patronymic`," +
                             " `teachers_acces_admin_panel`, `teachers_phone`, `teachers_mail`) VALUES (@login, @password, @name, @surname, @patronymic, @admin, @phone, @mail)", conn);

            //MySqlCommand command2 = new MySqlCommand("CREATE USER @login @`%` IDENTIFIED BY @password", conn);

            //command2.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxLogin.Text;
            //command2.Parameters.Add("@password", MySqlDbType.VarChar).Value = Hash(TextBoxPassword.Text);
            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxLogin.Text;
            command.Parameters.Add("@password", MySqlDbType.VarChar).Value = Hash(TextBoxPassword.Text);
            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = FIO[1];
            command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = FIO[0];
            command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = FIO[2];
            command.Parameters.Add("@admin", MySqlDbType.VarChar).Value = "True";
            command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = "";
            command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = TextBoxMail.Text;

            if (!IsTeachersLoginExists(TextBoxLogin.Text))
            {
                await conn.OpenAsync();

                if (command.ExecuteNonQuery() == 1)
                {
                    SendPasswordToUser();
                    ((MainWindow)System.Windows.Application.Current.MainWindow).FrameEJ.Navigate(new Pages.Theme());
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                }
                conn.Close();
            }
            else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данный логин уже занят");

        }


        public static MySqlConnection GetDBConnection()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("setting.xml");

            string server = xmlDocument.GetElementsByTagName("server")[0].InnerText;
            string username = xmlDocument.GetElementsByTagName("username")[0].InnerText;
            string password = xmlDocument.GetElementsByTagName("password")[0].InnerText;
            string database = "zhirov";

            String connString = "server=" + server + "; username=" + username + "; password=" + password + "; database=" + database;

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }

        public string Hash(string password)
        {
            MD5 md5hasher = MD5.Create();

            var data = md5hasher.ComputeHash(Encoding.Default.GetBytes(password));

            return Convert.ToBase64String(data);
        }

        public bool IsTeachersLoginExists(string login)
        {
            MySqlConnection conn = GetDBConnection();

            MySqlCommand command = new MySqlCommand("SELECT count(`teachers_login`) FROM `teachers` WHERE `teachers_login` = @login", conn);

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = login;

            conn.Open();
            MySqlDataReader read = command.ExecuteReader();
            
            if (read.Read())
            {
                int a = read.GetInt32(0);
                conn.Close();
                if (a >= 1)
                {
                    return true;
                }
                else return false;
            }
            else return true;
        }

        public bool IsUserExists(string login)
        {
            MySqlConnection conn = GetDBConnection();

            MySqlCommand command = new MySqlCommand("SELECT count(User) FROM mysql.user where User = @login", conn);

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = login;

            conn.Open();
            MySqlDataReader read = command.ExecuteReader();

            if (read.Read())
            {
                int a = read.GetInt32(0);
                conn.Close();
                if (a >= 1)
                {
                    return true;
                }
                else return false;
            }
            else return true;
        }

        private async void SendPasswordToUser()
        {
            try
            {
                string user = "ElectroJournal";
                bool a = true;
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress("zhirowdaniil@gmail.com", user);
                // кому отправляем

                MailAddress to = new MailAddress(TextBoxMail.Text);

                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = Title;
                // текст письма
                m.Body = "Добро пожаловать в систему Электронный журнал\n\nАдминистратор зарегистрироал Вас в системе электронного журнала, ниже написаны данные для входа в вашу учетную запись.\nМы рекомендуем при первой возможности поменять пароль на более удобный Вам, так как нынешний пароль является временным." +
                    "\n\nЛогин: " + TextBoxLogin.Text + "\nПароль: " + TextBoxPassword.Text + "\n\n\n\n";
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

                if (a)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "письмо отправлено");
                    //ButtonSend.IsEnabled = true;
                    //anim.Begin();
                    //RectangleLoad.Visibility = Visibility.Hidden;
                }
                else if (!a)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "письмо не отправилось");
                    //ButtonSend.IsEnabled = true;
                    //anim.Begin();
                    //RectangleLoad.Visibility = Visibility.Hidden;
                }
            }
            catch (System.FormatException)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Введен некорректный Email");
            }
        }
    }
}
