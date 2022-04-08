﻿using System;
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
using ElectroJournal.Classes;
using ElectroJournal.Pages;
using System.Net;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;

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

        DataBaseControls DbControls = new DataBaseControls();
        private bool _isDarkTheme = false;

        bool a = true;
        int secretCode = 0;

        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }

        private async void ButtonGridmailContinue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxGridMailMail.IsEnabled = false;
                TextBoxGridMailLogin.IsEnabled = false;
                ButtonGridmailContinue.IsEnabled = false;
                ButtonGridmailContinue.Content = "Проверка...";
                if (a)
                {
                    if (TextBoxGridMailLogin.Text != string.Empty && TextBoxGridMailMail.Text != string.Empty)
                    {
                        using (zhirovContext db = new())
                        {
                            Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.TeachersLogin == TextBoxGridMailLogin.Text && p.TeachersMail == TextBoxGridMailMail.Text);
                            if (teacher != null)
                            {
                                Properties.Settings.Default.UserID = (int)teacher.Idteachers;
                                Properties.Settings.Default.Save();
                                secretCode = await SendMail(TextBoxGridMailMail.Text);
                            }
                            else Notifications("Логин или почта введены неверно", "Уведомление");
                        }
                    }
                    else Notifications("Заполните поля", "Уведомление");
                }
                TextBoxGridMailMail.IsEnabled = true;
                TextBoxGridMailLogin.IsEnabled = true;
                ButtonGridmailContinue.IsEnabled = true;
                ButtonGridmailContinue.Content = "Отправить код";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private async Task<int> SendMail(string mail)
        {
            Random random = new Random();
            int secretCode = random.Next(100000, 999999);


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Joey Tribbiani", "code@electrojournal.ru"));
            message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", "zhirowdaniil@gmail.com"));
            message.Subject = "How you doin'?";

            message.Body = new TextPart("plain")
            {
                Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("electrojournal.ru", 25, false);

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("root", "");

                client.Send(message);
                client.Disconnect(true);
            }

            /*// отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("root@daniil-server.org", "Восстановление пароля");
            // кому отправляем
            MailAddress to = new MailAddress(mail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = Title;
            // текст письма
            m.Body = "Смена пароля в системе ElectroJournal\n Никому не сообщайте данный код: " + secretCode + "\n\n\n\n\n\n\n";
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("daniil-server", 25);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("root", "64580082");
            smtp.EnableSsl = true;

            try
            {
                await smtp.SendMailAsync(m);
                GridLoginMail.Visibility = Visibility.Hidden;
                GridVerifySecretCode.Visibility = Visibility.Visible;
                ButtonGridmailContinue.Visibility = Visibility.Hidden;
                TextBoxCode1.Focus();
                a = false;
            }
            catch (SmtpException)
            {
                Notifications("Почтовый сервис недоступен", "Ошибка");
            }*/

            return secretCode;
        }

        private async void ButtonGridMailRepeatCode_Click(object sender, RoutedEventArgs e)
        {
            ButtonGridMailRepeatCode.IsEnabled = false;
            ButtonGridMailRepeatCode.Content = "Отправка";

            if (TextBoxGridMailLogin.Text != string.Empty && TextBoxGridMailMail.Text != string.Empty)
            {
                using (zhirovContext db = new())
                {
                    Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.TeachersLogin == TextBoxGridMailLogin.Text && p.TeachersMail == TextBoxGridMailMail.Text);
                    if (teacher != null)
                    {
                        TextBoxCode1.Clear();
                        TextBoxCode2.Clear();
                        TextBoxCode3.Clear();
                        TextBoxCode4.Clear();
                        TextBoxCode5.Clear();
                        TextBoxCode6.Clear();
                        secretCode = await SendMail(TextBoxGridMailMail.Text);
                    }
                    else Notifications("Логин или почта введены неверно", "Уведомление");
                }
            }

            ButtonGridMailRepeatCode.IsEnabled = true;
            ButtonGridMailRepeatCode.Content = "Отправить заново";
        }

        private async void ButtonSaveNewPassword_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxVerifyNewPassword.Text != string.Empty && TextBoxNewPassword.Text != string.Empty)
            {
                if (TextBoxVerifyNewPassword.Text == TextBoxNewPassword.Text)
                {
                    using (zhirovContext db = new())
                    {
                        var teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == Properties.Settings.Default.UserID);

                        if (teacher != null)
                        {
                            teacher.TeachersPassword = DbControls.Hash(TextBoxVerifyNewPassword.Text);
                            await db.SaveChangesAsync();

                            ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Пароль успешно изменен");
                            this.Close();
                        }
                        else Notifications("Логин или пароль введены неверно", "Уведомление");
                    }
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

                    TextBoxCode1.Clear();
                    TextBoxCode2.Clear();
                    TextBoxCode3.Clear();
                    TextBoxCode4.Clear();
                    TextBoxCode5.Clear();
                    TextBoxCode6.Clear();

                    TextBoxCode1.Focus();
                }
            }
            else
            {
                if (tbc == 6) tbc = 1;

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
