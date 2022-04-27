using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        public ChangePassword()
        {
            InitializeComponent();
            TitleBar.CloseActionOverride = CloseActionOverride;
            TextBoxLogin.Text = Properties.Settings.Default.Login;
        }

        private bool _isDarkTheme = false;

        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBaseConn.GetDBConnection();

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
        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }

        private async void ButtonSaveGridReset_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            if (TextBoxLogin.Text != string.Empty && PasswordBoxNewAcceptPass.Password != string.Empty && PasswordBoxOldPass.Password != string.Empty && PasswordBoxNewPass.Password != string.Empty)
            {
                if (PasswordBoxNewPass.Password == PasswordBoxNewAcceptPass.Password)
                {
                    using (zhirovContext db = new zhirovContext())
                    {
                        var teacher = await db.Teachers.FirstOrDefaultAsync(p => p.TeachersLogin == TextBoxLogin.Text && p.TeachersPassword == DbControls.Hash(PasswordBoxOldPass.Password));

                        if (teacher != null)
                        {
                            teacher.TeachersPassword = DbControls.Hash(PasswordBoxNewPass.Password);
                            await db.SaveChangesAsync();

                            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
                            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
                            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
                            ((MainWindow)Application.Current.MainWindow).GridNLogin.Visibility = Visibility.Visible;
                            ((MainWindow)Application.Current.MainWindow).LabelScheduleCall.Content = "";
                            ((MainWindow)Application.Current.MainWindow).timer2.Stop();
                            ((MainWindow)Application.Current.MainWindow).animLabel = true;
                            ((MainWindow)Application.Current.MainWindow).AnimLog(true);
                            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
                            ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Пароль успешно изменен");
                            ProgressBar.Visibility = Visibility.Hidden;
                            this.Close();
                        }
                        else Notifications("Логин или пароль введены неверно", "Уведомление");
                    }
                }
                else Notifications("Пароли не совпадают", "Ошибка");
            }
            else Notifications("Заполните все поля", "Ошибка");
            ProgressBar.Visibility = Visibility.Hidden;
        }
    }
}
