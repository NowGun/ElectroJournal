using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using ElectroJournal.UControl;
using ElectroJournal.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>ButtonShowLogin
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
            (Application.Current.MainWindow as MainWindow).loginbool = true;
        }

        SettingsControl settingsControl = new();
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            (Resources["AnimLoadLogin"] as Storyboard).Completed += new EventHandler(MainWindow_Completed);
            (Resources["AnimLoadLogin"] as Storyboard).Begin();

            RectangleLoadLogin.Visibility = Visibility.Visible;

            var anim2 = (Storyboard)FindResource("AnimShowLoading");
            anim2.Begin();

            Login();
        }
        private void MainWindow_Completed(object? sender, EventArgs e) => (Resources["AnimLoadLogin"] as Storyboard).Begin();
        private async void Login()
        {
            try
            {
                ButtonLogin.IsEnabled = false;
                TextBoxLogin.IsEnabled = false;
                TextBoxPassword.IsEnabled = false;
                string pass = SettingsControl.Hash(TextBoxPassword.Password);
                Navigation nav = new();
                var anim = (Storyboard)FindResource("AnimLoadLogin");

                using zhirovContext db = new();
                bool isAvalaible = await db.Database.CanConnectAsync();
                string cond = @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)";

                if (isAvalaible)
                {
                    if (!string.IsNullOrWhiteSpace(TextBoxLogin.Text) && TextBoxPassword.Password != string.Empty)
                    {
                        if ((Regex.IsMatch(TextBoxLogin.Text, cond) && TextBoxLogin.Text.Contains("@")) || !TextBoxLogin.Text.Contains("@"))
                        {
                            Teacher? l = await db.Teachers.Where(p => (p.TeachersLogin == TextBoxLogin.Text.Trim() || p.TeachersMail == TextBoxLogin.Text.Trim()) && p.TeachersPassword == pass).FirstOrDefaultAsync();

                            if (l != null)
                            {
                                (Application.Current.MainWindow as MainWindow).UpdateUserInfo(l.TeachersImage, $"{l.TeachersName} {l.TeachersSurname}");

                                switch (l.TeachersAccesAdminPanel)
                                {
                                    case "True":
                                        (Application.Current.MainWindow as MainWindow).NavigationViewItemAdminPanel.Visibility = Visibility.Visible;
                                        break;

                                    case "False":
                                        (Application.Current.MainWindow as MainWindow).NavigationViewItemAdminPanel.Visibility = Visibility.Hidden;
                                        break;
                                }

                                Properties.Settings.Default.UserID = (int)l.Idteachers;
                                Properties.Settings.Default.Login = TextBoxLogin.Text;
                                Properties.Settings.Default.PassProfile = TextBoxPassword.Password;
                                Properties.Settings.Default.FirstName = l.TeachersName;
                                Properties.Settings.Default.Save();

                                l.TeachersStatus = 1;
                                await db.SaveChangesAsync();

                                (Application.Current.MainWindow as MainWindow).NavViewMenu.Visibility = Visibility.Visible;
                                (Application.Current.MainWindow as MainWindow).GridComboGroups.Visibility = Visibility.Visible;
                                (Application.Current.MainWindow as MainWindow).NavViewMenuAdmin.Visibility = Visibility.Hidden;
                                (Application.Current.MainWindow as MainWindow).RectangleBackToMenu.Visibility = Visibility.Hidden;
                                (Application.Current.MainWindow as MainWindow).isEntry = true;
                                (Application.Current.MainWindow as MainWindow).FillComboBoxGroups();
                                (Application.Current.MainWindow as MainWindow).StartCalls();
                                (Application.Current.MainWindow as MainWindow).OpenMenu();
                                (Application.Current.MainWindow as MainWindow).Calls.Visibility = Visibility.Visible;
                                (Application.Current.MainWindow as MainWindow).GridNLogin.Visibility = Visibility.Hidden;
                                (Application.Current.MainWindow as MainWindow).GridMenu.Visibility = Visibility.Visible;
                                (Application.Current.MainWindow as MainWindow).NavigationViewItemJournal.IsSelected = true;

                                anim.Stop();
                                RectangleLoadLogin.Visibility = Visibility.Hidden;
                                TextBoxLogin.IsEnabled = true;
                                TextBoxPassword.IsEnabled = true;
                                ButtonLogin.IsEnabled = true;

                                nav.NavigationPage("Journal");
                            }
                            else
                            {
                                (Application.Current.MainWindow as MainWindow)?.Notifications("Ошибка", "Логин или пароль введен неверно");
                                
                            }
                        }
                        else (Application.Current.MainWindow as MainWindow)?.Notifications("Ошибка", "Почта в неверном формате");

                    }
                    else
                    {
                        (Application.Current.MainWindow as MainWindow)?.Notifications("Ошибка", "Заполните поле");
                    }
                }
                else
                {
                    (Application.Current.MainWindow as MainWindow)?.Notifications("Ошибка", "База данных недоступна");
                }
                YesInput();
            }
            catch (Exception ex)
            {
                settingsControl.InputLog($"Login | {ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }
        private void IconForgotPassword_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.Login = TextBoxLogin.Text;
            Properties.Settings.Default.Save();
            
            new ResetPassword().ShowDialog();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var complete = settingsControl.CompletionLogin();

            TextBoxLogin.Text = complete.login;
            TextBoxPassword.Password = complete.pass;

            RectangleLoadLogin.Visibility = Visibility.Hidden;
        }
        private void IconChangeDB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new DBUser().ShowDialog();
        private void IconForgotPassword_MouseMove(object sender, MouseEventArgs e) => IconForgotPassword.Filled = true;
        private void IconForgotPassword_MouseLeave(object sender, MouseEventArgs e) => IconForgotPassword.Filled = false;
        private void IconChengeDB_MouseMove(object sender, MouseEventArgs e) => IconChengeDB.Filled = true;
        private void IconChengeDB_MouseLeave(object sender, MouseEventArgs e) => IconChengeDB.Filled = false;
        private void YesInput()
        {
            (Resources["AnimLoadLogin"] as Storyboard).Stop();
            TextBoxLogin.IsEnabled = true;
            TextBoxPassword.IsEnabled = true;
            ButtonLogin.IsEnabled = true;
            RectangleLoadLogin.Visibility = Visibility.Hidden;
        }
    }
}
