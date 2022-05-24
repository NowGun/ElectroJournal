using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        }

        private bool _isDarkTheme = false;
        private int tbc = 1;

        bool a = true;
        int secretCode = 0;

        private async void ButtonGridMailRepeatCode_Click(object sender, RoutedEventArgs e)
        {
            ButtonGridMailRepeatCode.IsEnabled = false;
            ButtonGridMailRepeatCode.Content = "Отправка";

            if (TextBoxGridMailMail.Text != string.Empty)
            {
                using zhirovContext db = new();
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.TeachersMail == TextBoxGridMailMail.Text);
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
                else
                {
                    Notifications("Логин или почта введены неверно", "Уведомление");
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
                    using zhirovContext db = new();
                    var teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == Properties.Settings.Default.UserID);

                    if (teacher != null)
                    {
                        teacher.TeachersPassword = SettingsControl.Hash(TextBoxVerifyNewPassword.Text);
                        await db.SaveChangesAsync();

                        ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Пароль успешно изменен");
                        ((MainWindow)Application.Current.MainWindow).ThemeCheck();
                        this.Close();
                    }
                    else
                    {
                        Notifications("Логин или пароль введены неверно", "Уведомление");
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
        private async void ButtonGridmailContinue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBoxGridMailMail.IsEnabled = false;
                ButtonGridmailContinue.IsEnabled = false;
                ButtonGridmailContinue.Content = "Проверка...";
                if (a)
                {
                    if (TextBoxGridMailMail.Text != string.Empty)
                    {
                        using zhirovContext db = new();
                        Teacher? teacher = await db.Teachers.FirstOrDefaultAsync(p => p.TeachersMail == TextBoxGridMailMail.Text);
                        if (teacher != null)
                        {
                            Properties.Settings.Default.UserID = (int)teacher.Idteachers;
                            Properties.Settings.Default.Save();
                            secretCode = await SendMail(TextBoxGridMailMail.Text);
                        }
                        else Notifications("Почта введена неверно", "Уведомление");
                    }
                    else Notifications("Заполните поле", "Уведомление");
                }
                TextBoxGridMailMail.IsEnabled = true;
                ButtonGridmailContinue.IsEnabled = true;
                ButtonGridmailContinue.Content = "Отправить код";
                TextBoxGridMailMail.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
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
                if (tbc == 6)
                {
                    tbc = 1;
                }

                if (tbc == 1)
                {
                    TextBoxCode2.Focus();
                }
                else if (tbc == 2)
                {
                    TextBoxCode3.Focus();
                }
                else if (tbc == 3)
                {
                    TextBoxCode4.Focus();
                }
                else if (tbc == 4)
                {
                    TextBoxCode5.Focus();
                }
                else if (tbc == 5)
                {
                    TextBoxCode6.Focus();
                }

                tbc++;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Classes.SettingsControl s = new();
            s.ChangeTheme();
        }
        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }
        private async Task<int> SendMail(string mail)
        {
            Random random = new();
            int secretCode = random.Next(100000, 999999);

            MailAddress from = new("mail@techno-review.ru", "Восстановление пароля");
            MailAddress to = new(mail);
            MailMessage m = new(from, to);
            m.Subject = Title;
            m.Body = $"Смена пароля в системе ElectroJournal\n Никому не сообщайте данный код: {secretCode} ";
            SmtpClient smtp = new("connect.smtp.bz", 25);
            smtp.Credentials = new NetworkCredential("zhirowdaniil@gmail.com", "CB1W3lAeBwQ6");
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
            }

            return secretCode;
        }
        private void Notifications(string message, string title)
        {
            RootSnackbar.Title = title;
            RootSnackbar.Message = message;
            RootSnackbar.Show();
        }
    }
}
