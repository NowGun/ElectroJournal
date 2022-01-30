using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElectroJournal.Classes;
using ElectroJournal.Windows;
using ElectroJournal.Pages;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;
using Notifications.Wpf;
using System.Configuration;
using System.Xml;
using System.Drawing;
using Microsoft.Win32;
using WPFUI.Controls;
using System.Windows.Interop;
using ModernWpf;
using ModernWpf.SampleApp.ControlPages;
using ModernWpf.Controls;
using WPFUI.Background;

namespace ElectroJournal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CheckVersionWindows();
            InitializeComponent();
            GridMenu.Visibility = Visibility.Hidden;
            Frame.Visibility = Visibility.Hidden;
            NavViewMenuAdmin.Visibility = Visibility.Hidden;
            RectangleBackToMenu.Visibility = Visibility.Hidden;
            RectangleLoadLogin.Visibility = Visibility.Hidden;
            CheckAutoRun();
            CompletionLogin();

            TitleBar.CloseActionOverride = CloseActionOverride;
            //ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            MenuBoard.Visibility = Visibility.Hidden;


            
            ThemeCheck();
        }

        DataBase DbUser = new DataBase();
        DataBaseControls DbControls = new DataBaseControls();
        private readonly NotificationManager _notificationManager = new NotificationManager();
        XmlDocument xmlDocument = new XmlDocument();
        MySqlConnection conn = DataBase.GetDBConnection();
        DataTable table = new DataTable();
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        DispatcherTimer label_time_time;

        System.Windows.Threading.DispatcherTimer timer2 = new System.Windows.Threading.DispatcherTimer();


        const string name = "ElectroJournal";

        private async void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            bool a;
            a = CheckTray();

            if (a)
            {
                var content = new NotificationContent
                {
                    Title = "Программа была свернута",
                    Message = "Программа продолжит своё выполнение в фоне.",
                    Type = NotificationType.Information

                };
                _notificationManager.Show(content);
                
                this.Hide();
            }
            else if (!a)
            {

                ContentDialogExample dialog = new ContentDialogExample();
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    foreach (var process in Process.GetProcessesByName("ElectroJournal"))
                    {
                        process.Kill();
                    }
                }
                else { }                
            }
        }

        private void CheckVersionWindows()
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                WPFUI.Background.Manager.Apply(this);
            }
        }

        private void ImageUpdateProgram_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("C:/projects/ElectroJournalNetFramework/UpdateElectroJournal/bin/Debug/UpdateElectroJournal.exe"); //Запуск программы обновления
        }

        private void RectangleUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.Users());
            NavigationViewItemJournal.IsSelected = false;
        }

        private void RectangleUser_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void RectangleUser_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void MainWindow_Completed(object sender, EventArgs e)
        {
            (Resources["AnimLoadLogin"] as Storyboard).Begin();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            label_time_time = new DispatcherTimer();
            label_time_time.Tick += (o, t) => { LabelTime.Content = "Время: " + DateTime.Now.ToLongTimeString(); };
            label_time_time.Interval = new TimeSpan(0, 0, 1);
            label_time_time.Start();

            //timer2.Tick += new EventHandler(LoadLessonPeriod);
           //timer2.Interval = new TimeSpan(0, 0, 1);
            //timer2.Start();


            Login();

            RectangleLoadLogin.Visibility = Visibility.Visible;
            (Resources["AnimLoadLogin"] as Storyboard).Completed += new EventHandler(MainWindow_Completed);

            var anim = (Storyboard)FindResource("AnimLoadLogin");
            anim.Begin();

            var anim2 = (Storyboard)FindResource("AnimShowLoading");
            anim2.Begin();


        }

        async void Login()
        {
            bool a = false;
            var anim = (Storyboard)FindResource("AnimLoadLogin");
            var anim3 = (Storyboard)FindResource("AnimOpenMenuStart");

            MySqlCommand command = new MySqlCommand("SELECT * FROM `teachers` WHERE `teachers_login` = @log AND `teachers_password` = @pass", conn);
            //MySqlCommand command = new MySqlCommand("SELECT SHA(authentication_string) from mysql.user WHERE user = @log", conn);

            command.Parameters.Add("@log", MySqlDbType.VarChar).Value = TextBoxLogin.Text;
            command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = DbControls.Hash(TextBoxPassword.Password);
            //command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = PBKDF2HashHelper.VerifyPassword(TextBoxPassword.Password, (string)command.ExecuteScalar());

            ButtonLogin.IsEnabled = false;
            TextBoxLogin.IsEnabled = false;
            TextBoxPassword.IsEnabled = false;

            adapter.SelectCommand = command;

            await Task.Run(() =>
            {
                try
                {
                    adapter.Fill(table);
                }
                catch (MySqlException)
                {
                    Notifications("Ошибка", "База данных недоступна");

                    a = true;
                }
            });

            if (a)
            {
                anim.Stop();
                RectangleLoadLogin.Visibility = Visibility.Hidden;
                TextBoxLogin.IsEnabled = true;
                TextBoxPassword.IsEnabled = true;
                ButtonLogin.IsEnabled = true;
                return;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(TextBoxLogin.Text) && TextBoxPassword.Password != string.Empty)
                {
                    if (table.Rows.Count > 0)
                    {
                        MySqlCommand command2 = new MySqlCommand("SELECT `teachers_login`, `teachers_surname`, `teachers_name`, `teachers_patronymic`, `teachers_acces_admin_panel`, `idteachers` FROM `teachers` WHERE `teachers_login` = @login", conn); //Команда выбора данных

                        command2.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxLogin.Text;

                        conn.Open();

                        MySqlDataReader read = command2.ExecuteReader(); //Считываем и извлекаем данные

                        if (read.Read())
                        {
                            TextBlockTeacher.Text = read.GetString(1) + " " + read.GetString(2);

                            if (read.GetString(4) == "False") NavigationViewItemAdminPanel.Visibility = Visibility.Hidden;
                            else NavigationViewItemAdminPanel.Visibility = Visibility.Visible;

                            Properties.Settings.Default.UserID = read.GetInt32(5);
                            Properties.Settings.Default.Login = TextBoxLogin.Text;

                            Properties.Settings.Default.Save();

                            conn.Close();

                            GridLogin.Visibility = Visibility.Hidden;
                            GridMenu.Visibility = Visibility.Visible;
                            Frame.Visibility = Visibility.Visible;
                            NavigationViewItemJournal.IsSelected = true;
                            Frame.Navigate(new Pages.Journal());

                            //LoadLessonPeriod();
                            Notifications("Оповещение", "Авторизация прошла успешно");
                        }
                        conn.Close();
                        table.Clear();
                        anim.Stop();
                        RectangleLoadLogin.Visibility = Visibility.Hidden;
                        TextBoxLogin.IsEnabled = true;
                        TextBoxPassword.IsEnabled = true;
                        ButtonLogin.IsEnabled = true;
                    }
                    else
                    {
                        Notifications("Ошибка", "Логин или пароль введен неверно");
                        anim.Stop();
                        RectangleLoadLogin.Visibility = Visibility.Hidden;
                        TextBoxLogin.IsEnabled = true;
                        TextBoxPassword.IsEnabled = true;
                        ButtonLogin.IsEnabled = true;
                    }
                }
                else
                {
                    Notifications("Ошибка", "Заполните поле");
                    anim.Stop();
                    RectangleLoadLogin.Visibility = Visibility.Hidden;
                    TextBoxLogin.IsEnabled = true;
                    TextBoxPassword.IsEnabled = true;
                    ButtonLogin.IsEnabled = true;
                }
            }
        }


        private void RectangleBackToMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimOpenMenu");
            anim.Begin();

            NavViewMenuAdmin.Visibility = Visibility.Hidden;
            NavViewMenu.Visibility = Visibility.Visible;
            RectangleBackToMenu.Visibility = Visibility.Hidden;

            NavigationViewItemJournal.IsSelected = true;
            Frame.Navigate(new Pages.Journal());
        }

        private void Frame_ContentRendered(object sender, EventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimOpenFrame");
            anim.Begin();
        }

        public void Notifications(string title, string text)
        {
            _notificationManager.Show(
                new NotificationContent { Title = title, Message = text, Type = NotificationType.Information },
                areaName: "WindowArea");
        }

        private bool _isDarkTheme = false;

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
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
            else
            {
                WPFUI.Background.Manager.RemoveDarkMode(windowHandle);
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }

            this.Background = System.Windows.Media.Brushes.Transparent;
            WPFUI.Background.Manager.Apply(WPFUI.Background.BackgroundType.Mica, windowHandle);

        }

        private void CompletionLogin()
        {
            string username = Properties.Settings.Default.UserName;
            string password = Properties.Settings.Default.Password;

            //TextBoxLogin.Text = username;
            //TextBoxPassword.Password = password;
        }

        public void AnimLog(bool a)
        {
            if (a)
            {
                var anim = (Storyboard)FindResource("AnimLogoutUser");
                anim.Begin();
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool a;
            a = CheckTray();
            e.Cancel = true;

            if (a)
            {
                var content = new NotificationContent
                {
                    Title = "Программа была свернута",
                    Message = "Программа продолжит своё выполнение в фоне.",
                    Type = NotificationType.Information

                };
                _notificationManager.Show(content);
                e.Cancel = true;
                this.Hide();
            }
            else if (!a)
            {
                ContentDialogExample dialog = new ContentDialogExample();
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    foreach (var process in Process.GetProcessesByName("ElectroJournal"))
                    {
                        process.Kill();
                    }
                }
                else e.Cancel = true;

                /*
                WPFUI.Controls.MessageBox messageBox2 = new WPFUI.Controls.MessageBox();
                messageBox2.LeftButtonName = "Да";
                messageBox2.RightButtonName = "Нет";
                messageBox2.LeftButtonClick += MessageBox_LeftButtonClick;
                messageBox2.RightButtonClick += MessageBox_RightButtonClick;
                messageBox2.Show("Уведомление", "Вы точно хотите выйти из программы ElectroJournal?");
                */

                /*
                MessageBoxResult result;
                result = System.Windows.MessageBox.Show("Вы точно хотите выйти из программы?", "Выход из ElectroJournal", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    foreach (var process in Process.GetProcessesByName("ElectroJournal"))
                    {
                        process.Kill();
                    }
                }
                   */
            }
        }


        private void MessageBox_LeftButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {

            foreach (var process in Process.GetProcessesByName("ElectroJournal"))
            {
                process.Kill();
            }
        }

        private void MessageBox_RightButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as WPFUI.Controls.MessageBox)?.Close();
        }

        private bool CheckTray()
        {
            //bool a;
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //string collapsetotray = xmlDocument.GetElementsByTagName("collapsetotray")[0].InnerText;

            bool tray = Properties.Settings.Default.Tray;

            //a = bool.Parse(collapsetotray);

            return tray;
        }

        public void CheckAutoRun()
        {
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //string autorun = xmlDocument.GetElementsByTagName("autorun")[0].InnerText;

            bool autorun = Properties.Settings.Default.AutoRun;

            if (autorun)
            {
                SetAutorunValue(true);
            }
            else SetAutorunValue(false);
        }

        public bool SetAutorunValue(bool autorun)
        {
            string ExePath = System.Windows.Forms.Application.ExecutablePath;
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                if (autorun)
                    reg.SetValue(name, ExePath);
                else
                    reg.DeleteValue(name);

                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void ImageForgotPassword_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void ImageForgotPassword_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.Login = TextBoxLogin.Text;
            Properties.Settings.Default.Save();

            new Windows.ResetPassword().ShowDialog();
        }

        private void ImageForgotPassword_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void ImageChangeDB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new DBUser().ShowDialog();
        }

        private void MenuItemBoardPrint_Click(object sender, RoutedEventArgs e)
        {
            Board board = new Board();
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(board.InkCanvas, "Печать изображения");
            }

            ((Board)Frame.Content).GridInkCanvas.Visibility = Visibility.Visible;
        }

        private void MenuItemBoardFullScreen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemEJCalculator_Click(object sender, RoutedEventArgs e)
        {
            new Calculator().Show();
        }

        private void MenuItemEJSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.Setting());
        }

        private void MenuItemEJUpdate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("C:/projects/ElectroJournalNetFramework/UpdateElectroJournal/bin/Debug/UpdateElectroJournal.exe"); //Запуск программы обновления
        }

        private void MenuItemEJHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemEJProgInfo_Click(object sender, RoutedEventArgs e)
        {
            new ProgramInformation().ShowDialog();
        }

        private void MenuItemEJBug_Click(object sender, RoutedEventArgs e)
        {
            new Help().ShowDialog();
        }

        private void ButtonOpenNotLogin_Click(object sender, RoutedEventArgs e)
        {
            GridLogin.Visibility = Visibility.Hidden;
            GridMenu.Visibility = Visibility.Visible;
            Frame.Visibility = Visibility.Visible;
        }

        int aa = 0;

        private void LoadLessonPeriod(object sender, EventArgs e)
        {
            MySqlCommand command = new MySqlCommand("SELECT date_format(`periodclasses_start`, '%H:%i'), date_format(`periodclasses_end`, '%H:%i'), " +
                "periodclasses_number, date_format(SUBTIME(periodclasses_end, CURRENT_TIME()), '%i:%s') FROM periodclasses WHERE CURRENT_TIME() between periodclasses_start AND periodclasses_end", conn); //Команда выбора данных

            conn.Open(); //Открываем соединение

            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные

            if (read.Read())
            {
                LabelTime.Visibility = Visibility.Visible;
                LabelLesson.Visibility = Visibility.Visible;
                LabelPeriodLesson.Visibility = Visibility.Visible;
                LabelEndLesson.Visibility = Visibility.Visible;

                LabelPeriodLesson.Content = "Период занятий: " + read.GetValue(0).ToString() + " - " + read.GetValue(1).ToString();
                LabelLesson.Content = "Урок: " + read.GetValue(2).ToString();
                LabelEndLesson.Content = "Конец занятия через: " + read.GetValue(3).ToString();
            }
            else
            {
                LabelTime.Visibility = Visibility.Hidden;
                LabelLesson.Visibility = Visibility.Hidden;
                LabelPeriodLesson.Visibility = Visibility.Hidden;
                LabelEndLesson.Visibility = Visibility.Hidden;
            }
            conn.Close(); //Закрываем соединение
            asdasd.Content = aa++;
        }

        private void NavigationViewItemBoard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.Board());
        }

        private void NavigationViewItemJournal_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.Journal());
        }

        private void NavigationViewItemSchedule_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.Schedule());
        }

        private void NavigationViewItemWord_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.Word());
        }

        private void NavigationViewItemAdminPanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavViewMenu.Visibility = Visibility.Hidden;
            NavViewMenuAdmin.Visibility = Visibility.Visible;
            NavigationViewItemTeachers.IsSelected = true;
            RectangleBackToMenu.Visibility = Visibility.Visible;
            Frame.Navigate(new Pages.AdminPanel.Teachers());
        }

        private void NavigationViewItemDisciplines_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.AdminPanel.Disciplines());
        }

        private void NavigationViewItemTeachers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.AdminPanel.Teachers());
        }

        private void NavigationViewItemStudents_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.AdminPanel.Students());
        }

        private void NavigationViewItemGroups_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.AdminPanel.Groups());
        }

        private void NavigationViewItemCabinets_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.AdminPanel.Cabinet());
        }

        private void NavigationViewItemScheduleAdmin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.AdminPanel.ScheduleAdmin());
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("ElectroJournal"))
            {
                process.Kill();
            }
        }

        private void TitleBar_NotifyIconDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }
    }
}
