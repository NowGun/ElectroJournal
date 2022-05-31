using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;

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
        private string cond = @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)";

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
            else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните все поля");
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e) => ((MainWindow)System.Windows.Application.Current.MainWindow).FrameEJ.Navigate(new Pages.ConnectDB());
        private async void AddUser()
        {
            try
            {
                ButtonNext.IsEnabled = false;
                if (!String.IsNullOrWhiteSpace(TextBoxPass.Text) && !String.IsNullOrWhiteSpace(TextBoxFIO.Text) && !String.IsNullOrWhiteSpace(TextBoxMail.Text))
                {
                    if (Regex.IsMatch(TextBoxMail.Text, cond))
                    {
                        string[] FIO = TextBoxFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        using zhirovContext db = new();
                        var teac = await db.Teachers.Where(p => p.TeachersMail == TextBoxMail.Text).ToListAsync();

                        if (teac.Count == 0)
                        {
                            Teacher teacher = new()
                            {
                                TeachersName = FIO[1],
                                TeachersSurname = FIO[0],
                                TeachersPatronymic = FIO[2],
                                TeachersPassword = Hash(TextBoxPass.Text),
                                TeachersMail = TextBoxMail.Text,
                                TeachersAccesAdminPanel = "True"
                            };

                            await db.Teachers.AddAsync(teacher);
                            await db.SaveChangesAsync();

                            SendPasswordToUser();

                            ((MainWindow)Application.Current.MainWindow).FrameEJ.Navigate(new Theme());
                        }
                        else ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данная почта занята");
                    }
                    else ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Почта в неверном формате");
                }
                else ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Заполните все поля");

                ButtonNext.IsEnabled = true;
            }
            catch
            {

            }
        }
        public string Hash(string password)
        {
            MD5 md5hasher = MD5.Create();
            return Convert.ToBase64String(md5hasher.ComputeHash(Encoding.Default.GetBytes(password)));
        }
        private async void SendPasswordToUser() // Отправка сообщения на почту
        {
            try
            {
                string user = TextBoxFIO.Text;
                bool a = true;

                MailAddress from = new("mail@techno-review.ru", user);
                MailAddress to = new(TextBoxMail.Text);
                MailMessage m = new(from, to);

                m.Subject = "ElectroJournal";
                m.Body = "Добро пожаловать в систему Электронный журнал\n\nАдминистратор зарегистрировал Вас в системе электронного журнала, ниже написаны данные для входа в вашу учетную запись.\nМы рекомендуем при первой возможности поменять пароль на более удобный Вам, так как нынешний пароль является временным." +
                    "\n\nЛогин: " + TextBoxMail.Text + "\nПароль: " + TextBoxPass.Text + "        \n\n\n";

                SmtpClient smtp = new("connect.smtp.bz", 2525);

                smtp.Credentials = new NetworkCredential("zhirowdaniil@gmail.com", "CB1W3lAeBwQ6");
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
                }
                else if (!a)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "письмо не отправилось");
                }
            }
            catch (System.FormatException)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Введен некорректный Email");
            }
        }
    }
}
