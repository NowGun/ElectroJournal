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
            GridResetOldPassword.Visibility = Visibility.Hidden;
            GridNewPassword.Visibility = Visibility.Hidden;
        }

        DataBaseConn DbUser = new DataBaseConn();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBaseConn.GetDBConnection();

        bool a = true;
        int secretCode = 0;

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

        private void LabelOldPassword_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GridSelectReset.Visibility = Visibility.Hidden;
            GridResetOldPassword.Visibility = Visibility.Visible;
            TextBoxGridResetLogin.Text = Properties.Settings.Default.Login;            
        }

        private void LabelLogin_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GridSelectReset.Visibility= Visibility.Hidden;
            GridNewPassword.Visibility= Visibility.Visible;
            TextBoxGridMailLogin.Text = Properties.Settings.Default.Login;
        }

        private void ButtonSaveGridReset_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxGridResetLogin.Text != string.Empty && PasswordBoxGridResetNewPassord.Password != string.Empty && PasswordBoxGridResetOldPassword.Password != string.Empty && PasswordBoxGridResetVerify.Password != string.Empty) 
            {
                if (PasswordBoxGridResetNewPassord.Password == PasswordBoxGridResetVerify.Password)
                {
                    MySqlCommand command = new MySqlCommand("UPDATE `teachers` SET `teachers_password` = @passwordnew WHERE `teachers_login` = @login AND `teachers_password` = @password", conn);

                    command.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxGridResetLogin.Text;
                    command.Parameters.Add("@password", MySqlDbType.VarChar).Value = DbControls.Hash(PasswordBoxGridResetOldPassword.Password);
                    command.Parameters.Add("@passwordnew", MySqlDbType.VarChar).Value = DbControls.Hash(PasswordBoxGridResetNewPassord.Password);

                    conn.Open();

                    if (command.ExecuteNonQuery() == 1)
                    {
                        LabelNotify.Content = "Пароль успешно изменен";
                    }
                    conn.Close();
                }
                else
                {
                    LabelNotify.Content = "Пароли не совпадают";
                }
            }
            else
            {
                LabelNotify.Content = "Заполните все поля";
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
                        GridVerifySecretCode.Visibility = Visibility.Visible;
                        ButtonGridmailContinue.Content = "Далее";
                        a = false;
                    }

                    conn.Close();
                }
            }
            else if (!a)
            {
                if (Convert.ToString(secretCode) == TextBoxGridMailCode.Text)
                {
                    GridLoginMail.Visibility = Visibility.Hidden;
                    GridMailNewPassword.Visibility = Visibility.Visible;
                }
                else
                {
                    LabelNotify.Content = "Введенный код неверен";
                }
            }
        }

        private int SendMail(string mail)
        {
            Random random = new Random();
            int secretCode = random.Next(100000, 999999);

            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("zhirowdaniil@gmail.com", "ElectroJournal");
            // кому отправляем
            MailAddress to = new MailAddress(mail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = Title;
            // текст письма
            m.Body = "Смена пароля в системе ElectroJournal\n Никому не сообщайте данный код: " + secretCode + "\n\n\n\n\n\n\n";
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.elasticemail.com", 2525);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("zhirowdaniil@gmail.com", "E0E7027197724CDBDAFAD917FB914057C0CB");
            smtp.EnableSsl = true;
                        
             try
             {
                 smtp.Send(m);
             }
             catch (SmtpException)
             {
                 LabelNotify.Content = "Произошла ошибка";
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
                        LabelNotify.Content = "Пароль успешно изменен";
                    }
                    conn.Close();
                }
                else
                {
                    LabelNotify.Content = "Пароли не совпадают";
                }
            }
            else
            {
                LabelNotify.Content = "Заполните все поля";
            }
        }
    }
}
