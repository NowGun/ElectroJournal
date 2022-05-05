using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

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
            TextBoxLogin.Text = Properties.Settings.Default.Login;
        }

        private bool _isDarkTheme = false;

        DataBaseControls DbControls = new();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Classes.SettingsControl s = new();
            s.ChangeTheme();
        }
        private void Notifications(string message, string title)
        {
            RootSnackbar.Title = title;
            RootSnackbar.Content = message;
            RootSnackbar.Icon = WPFUI.Common.SymbolRegular.MailError16;
            RootSnackbar.Show();
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
                        else
                        {
                            Notifications("Логин или пароль введены неверно", "Уведомление");
                        }
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

            ProgressBar.Visibility = Visibility.Hidden;
        }
    }
}
