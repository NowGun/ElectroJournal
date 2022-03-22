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
using System.Net;
using MySql.Data.MySqlClient;
using System.Data;
using ElectroJournal.Classes;
using System.Windows.Media.Animation;
using System.Windows.Interop;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            //RectangleLoad.Visibility = Visibility.Hidden;
            TitleBar.CloseActionOverride = CloseActionOverride;
        }

        private async void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }

        private void MainWindow_Completed(object sender, EventArgs e)
        {
            (Resources["AnimLoad"] as Storyboard).Begin();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {/*
            if (!string.IsNullOrWhiteSpace(TextBoxResultA.Text) &&
                !string.IsNullOrWhiteSpace(TextBoxResultB.Text) &&
                !string.IsNullOrWhiteSpace(TextBoxTitle.Text) &&
                !string.IsNullOrWhiteSpace(TextBoxSteps.Text))
            {
                ButtonSend.IsEnabled = false;
                RectangleLoad.Visibility = Visibility.Visible;
                (Resources["AnimLoad"] as Storyboard).Completed += new EventHandler(MainWindow_Completed);

                var anim = (Storyboard)FindResource("AnimLoad");
                anim.Begin();

                var anim2 = (Storyboard)FindResource("AnimShowLoading");
                anim2.Begin();

                string Title = TextBoxTitle.Text;
                string Steps = TextBoxSteps.Text;
                string ResultA = TextBoxResultA.Text;
                string ResultB = TextBoxResultB.Text;
                SendMessage(Title, Steps, ResultA, ResultB);
            }
            else
            {
                Notifications("Заполните поле", "red");
            }*/
        }

        async private void SendMessage (string Title, string Steps, string ResultA, string ResultB)
        {/*
            var anim = (Storyboard)FindResource("AnimLoad");
            var anim2 = (Storyboard)FindResource("AnimNot");
            bool a = false;

            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("zhirowdaniil@gmail.com", "User");
            // кому отправляем
            MailAddress to = new MailAddress("nowgun98@gmail.com");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = Title;
            // текст письма
            m.Body = "Шаги воспроизведения:\n" + Steps + "\n\nПри Совершении действия А, происходит действие Б\n" + ResultA + "\n\nПри Совершении действия А, происходит действие В\n" + ResultB;
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
                Notifications("Сообщение отправлено", "green");
                ButtonSend.IsEnabled = true;
                anim.Begin();
                RectangleLoad.Visibility = Visibility.Hidden;
            }
            else if (a)
            {
                Notifications("Отсутствует подключение к интернету", "red");
                ButtonSend.IsEnabled = true;
                anim.Begin();
                RectangleLoad.Visibility = Visibility.Hidden;
            }*/
        }

        private void Notifications (string message, string color)
        {/*
            var anim2 = (Storyboard)FindResource("AnimNot");
            switch (color) {
                case "red":
                    LabelSend.Foreground = Brushes.Red;
                    LabelSend.Content = message;
                    break;

                case "green":
                    LabelSend.Foreground = Brushes.Green;
                    LabelSend.Content = message;
                    break;
            }
            anim2.Begin();*/
        }

        private bool _isDarkTheme = false;

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
    }
}
