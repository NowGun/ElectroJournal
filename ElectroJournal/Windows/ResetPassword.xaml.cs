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
using System.Net.Mail;
using ElectroJournal.Classes;
using ElectroJournal.Pages;
using MySql.Data.MySqlClient;
using System.Net;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для ResetPassword.xaml
    /// </summary>
    public partial class ResetPassword : Window
    {
        public ResetPassword()
        {
            InitializeComponent();
            GridVerifySecretCode.Visibility = Visibility.Hidden;
            TitleBar.CloseActionOverride = CloseActionOverride;
        }

        DataBaseConn DbUser = new DataBaseConn();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBaseConn.GetDBConnection();
        private bool _isDarkTheme = false;

        bool a = true;
        int secretCode = 0;

        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }

        private void FramePassword_Initialized(object sender, EventArgs e)
        {
            //FramePassword.Navigate(new Pages.ResetPassword.ConfirmPassword());
        }

        private void ButtonPassword_Click(object sender, RoutedEventArgs e)
        {
            Pages.ResetPassword.ConfirmPassword cp = new Pages.ResetPassword.ConfirmPassword();
            if (cp.TextBoxNewPassword.Text != null && cp.TextBoxReturnNewPassword.Text != null && cp.TextBoxOldPassword.Text != null)
            {
                //FramePassword.Navigate(new Pages.ResetPassword.ConfirmSMS());
            }
        }

        private void ButtonGridmailContinue_Click(object sender, RoutedEventArgs e)
        {
            if (a)
            {
                if (TextBoxGridMailLogin.Text != string.Empty && TextBoxGridMailMail.Text != string.Empty)
                {
                    MySqlCommand command = new MySqlCommand("SELECT `teachers_login`, `teachers_mail`, `idteachers` FROM `teachers` WHERE `teachers_login` = @login AND `teachers_mail` = @mail", conn);

                    command.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxGridMailLogin.Text;
                    command.Parameters.Add("@mail", MySqlDbType.Text).Value = TextBoxGridMailMail.Text;

                    conn.Open();
                    MySqlDataReader read = command.ExecuteReader();

                    if (read.Read())
                    {
                        Properties.Settings.Default.UserID = read.GetInt32(2);
                        Properties.Settings.Default.Save();
                        secretCode = SendMail(TextBoxGridMailMail.Text);
                        GridLoginMail.Visibility = Visibility.Hidden;
                        GridVerifySecretCode.Visibility = Visibility.Visible;
                        ButtonGridmailContinue.Visibility = Visibility.Hidden;
                        TextBoxCode1.Focus();
                        //ButtonGridmailContinue.Content = "Далее";
                        a = false;
                    }
                    else Notifications("Логин или почта введены неверно", "Уведомление");

                    conn.Close();
                }
                else Notifications("Заполните поля", "Уведомление");
            }            
        }

        private int SendMail(string mail)
        {
            Random random = new Random();
            int secretCode = random.Next(100000, 999999);

            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("help@techno-review.ru", "Восстановление пароля");
            // кому отправляем
            MailAddress to = new MailAddress(mail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = Title;
            // текст письма
            m.Body = "Смена пароля в системе ElectroJournal\n Никому не сообщайте данный код: " + secretCode + "\n\n\n\n\n\n\n";
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.beget.com", 2525);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("help@techno-review.ru", "64580082Now");
            smtp.EnableSsl = true;
                        
             try
             {
                 smtp.Send(m);
             }
             catch (SmtpException)
             {
                Notifications("Произошла ошибка", "Ошибка");
            }

            return secretCode;
        }

        private void ButtonGridMailRepeatCode_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxGridMailLogin.Text != string.Empty && TextBoxGridMailMail.Text != string.Empty)
            {
                MySqlCommand command = new MySqlCommand("SELECT `teachers_login`, `teachers_mail`, `idteachers` FROM `teachers` WHERE `teachers_login` = @login AND `teachers_mail` = @mail", conn);

                command.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxGridMailLogin.Text;
                command.Parameters.Add("@mail", MySqlDbType.Text).Value = TextBoxGridMailMail.Text;

                conn.Open();
                MySqlDataReader read = command.ExecuteReader();

                if (read.Read())
                {
                    TextBoxCode1.Clear();
                    TextBoxCode2.Clear();
                    TextBoxCode3.Clear();
                    TextBoxCode4.Clear();
                    TextBoxCode5.Clear();
                    TextBoxCode6.Clear();
                    secretCode = SendMail(TextBoxGridMailMail.Text);
                }
                conn.Close();
            }
        }

        private void ButtonSaveNewPassword_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxVerifyNewPassword.Text != string.Empty && TextBoxNewPassword.Text != string.Empty)
            {
                if (TextBoxVerifyNewPassword.Text == TextBoxNewPassword.Text)
                {
                    MySqlCommand command = new MySqlCommand("UPDATE `teachers` SET `teachers_password` = @passwordnew WHERE `idteachers` = @id", conn);

                    command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Properties.Settings.Default.UserID;
                    command.Parameters.Add("@passwordnew", MySqlDbType.VarChar).Value = DbControls.Hash(TextBoxVerifyNewPassword.Text);

                    conn.Open();

                    if (command.ExecuteNonQuery() == 1)
                    {
                        ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Пароль успешно изменен");
                        this.Close();
                    }
                    conn.Close();
                }
                else
                {
                    Notifications("Пароли не совпадают", "Ошибка");
                }
            }
            else
            {
                Notifications("Заполните все поля", "Ошибка");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }

        public void ThemeCheck()
        {
            int theme = Properties.Settings.Default.Theme;

            _isDarkTheme = theme == 1;
            WPFUI.Theme.Manager.Switch(theme == 1 ? WPFUI.Theme.Style.Dark : WPFUI.Theme.Style.Light);

            ApplyBackgroundEffect();
        }

        private void ApplyBackgroundEffect()
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            WPFUI.Background.Manager.Remove(windowHandle);

            if (_isDarkTheme)
            {
                WPFUI.Background.Manager.ApplyDarkMode(windowHandle);
            }
            else
            {
                WPFUI.Background.Manager.RemoveDarkMode(windowHandle);
            }

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                this.Background = System.Windows.Media.Brushes.Transparent;
                WPFUI.Background.Manager.Apply(WPFUI.Background.BackgroundType.Mica, windowHandle);
            }
        }

        private void Notifications(string message, string title)
        {
            RootSnackbar.Title = title;
            RootSnackbar.Content = message;
            //RootSnackbar.Icon = WPFUI.Common.Icon.MailError16;
            RootSnackbar.Expand();
        }

        private int tbc = 1;

        private void TextBoxCode1_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (TextBoxCode1.Text != String.Empty &&
                TextBoxCode2.Text != String.Empty &&
                TextBoxCode3.Text != String.Empty &&
                TextBoxCode4.Text != String.Empty &&
                TextBoxCode5.Text != String.Empty &&
                TextBoxCode6.Text != String.Empty)
            {
                if (Convert.ToString(secretCode) == $"{TextBoxCode1.Text}{TextBoxCode2.Text}{TextBoxCode3.Text}{TextBoxCode4.Text}{TextBoxCode5.Text}{TextBoxCode6.Text}")
                {
                    GridVerifySecretCode.Visibility = Visibility.Hidden;
                    GridMailNewPassword.Visibility = Visibility.Visible;
                }
                else
                {
                    var anim = (Storyboard)FindResource("AnimBadCode");
                    anim.Begin();
                    Notifications("Введенный код неверен", "Ошибка");
                }
            }
            else
            {
                if (tbc == 1) TextBoxCode2.Focus();
                else if (tbc == 2) TextBoxCode3.Focus();
                else if (tbc == 3) TextBoxCode4.Focus();
                else if (tbc == 4) TextBoxCode5.Focus();
                else if (tbc == 5) TextBoxCode6.Focus();

                tbc++;
            }            
        }
    }
}
