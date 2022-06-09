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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Classes.SettingsControl s = new();
            s.ChangeTheme();
        }
        private void Notifications(string message, string title)
        {
            RootSnackbar.Title = title;
            RootSnackbar.Message = message;
            RootSnackbar.Show();
        }
        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }
        private async void ButtonSaveGridReset_Click(object sender, RoutedEventArgs e)
        {
            SettingsControl sc = new();
            string error;

            ProgressBar.Visibility = Visibility.Visible;
            if (TextBoxLogin.Text != string.Empty && PasswordBoxNewAcceptPass.Password != string.Empty && PasswordBoxOldPass.Password != string.Empty && PasswordBoxNewPass.Password != string.Empty)
            {
                if (sc.ValidatePassword(PasswordBoxNewPass.Password, out error))
                {
                    if (PasswordBoxNewPass.Password == PasswordBoxNewAcceptPass.Password)
                    {
                        using zhirovContext db = new();
                        var teacher = await db.Teachers.FirstOrDefaultAsync(p => p.TeachersMail == TextBoxLogin.Text && p.TeachersPassword == SettingsControl.Hash(PasswordBoxOldPass.Password));

                        if (teacher != null)
                        {
                            teacher.TeachersPassword = SettingsControl.Hash(PasswordBoxNewPass.Password);
                            await db.SaveChangesAsync();

                            ((MainWindow)Application.Current.MainWindow).AnimLogout(true);
                            ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Пароль успешно изменен");
                            ProgressBar.Visibility = Visibility.Hidden;
                            this.Close();
                        }
                        else Notifications("Логин или пароль введены неверно", "Уведомление");
                    }
                    else Notifications("Пароли не совпадают", "Ошибка");
                }
                else Notifications(error, "Ошибка");
            }
            else Notifications("Заполните все поля", "Ошибка");

            ProgressBar.Visibility = Visibility.Hidden;
        }
    }
}
