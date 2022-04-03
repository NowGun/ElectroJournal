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
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;

namespace ElectroJournal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //CheckVersionWindows();
            InitializeComponent();            
            
            GridMenu.Visibility = Visibility.Hidden;
            Frame.Visibility = Visibility.Hidden;
            NavViewMenuAdmin.Visibility = Visibility.Hidden;
            RectangleBackToMenu.Visibility = Visibility.Hidden;
            RectangleLoadLogin.Visibility = Visibility.Hidden;

            
            settingsControl.CheckAutoRun();
            settingsControl.CheckTray();

            GridNLogin.Visibility = Visibility.Visible;
            TitleBar.CloseActionOverride = CloseActionOverride;
            MenuBoard.Visibility = Visibility.Hidden;
        }

        SettingsControl settingsControl = new();
        SettingMigration SettingMig = new();
        
        private readonly NotificationManager _notificationManager = new();
        public System.Windows.Threading.DispatcherTimer timer2 = new();

        

        private async void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            bool a;
            a = settingsControl.CheckTray();

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

                ContentDialogExample dialog = new();
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
            RectangleLoadLogin.Visibility = Visibility.Visible;
            (Resources["AnimLoadLogin"] as Storyboard).Completed += new EventHandler(MainWindow_Completed);

            var anim = (Storyboard)FindResource("AnimLoadLogin");
            anim.Begin();

            var anim2 = (Storyboard)FindResource("AnimShowLoading");
            anim2.Begin();

            Login();           
        }

        bool checkFiilScheduleDB = true;
        public bool animLabel = true;

        List<string> ScheduleStart = new();
        List<string> ScheduleEnd = new();
        List<int> ScheduleNumber = new();

        private async void SheduleCall(object sender, EventArgs e)
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                LabelScheduleCall.Visibility = Visibility.Hidden;
            }
            else
            {
                if (checkFiilScheduleDB)
                {
                    using (zhirovContext db = new zhirovContext())
                    {
                        var time = await db.Periodclasses.ToListAsync();

                        foreach (var t in time)
                        {
                            ScheduleStart.Add(t.PeriodclassesStart.ToString("hh':'mm"));
                            ScheduleEnd.Add(t.PeriodclassesEnd.ToString("hh':'mm"));
                            ScheduleNumber.Add(t.PeriodclassesNumber);
                            checkFiilScheduleDB = false;
                        }
                    }                    
                }

                var anim = (Storyboard)FindResource("AnimLabelScheduleCall");

                for (int i = 0; i <= ScheduleStart.Count; i++)
                {
                    if (i != ScheduleStart.Count)
                    {
                        if (DateTime.Parse(ScheduleStart[i]) < DateTime.Now && DateTime.Now < DateTime.Parse(ScheduleEnd[i]))
                        {
                            if (animLabel) anim.Begin();
                            animLabel = false;
                                                        
                            LabelScheduleCall.Content = $"Урок: {ScheduleNumber[i]}    Период занятий: {ScheduleStart[i]} - {ScheduleEnd[i]}    До конца занятий: " +
                               (DateTime.Parse(ScheduleEnd[i]) - DateTime.Now).ToString("mm':'ss");
                            break;
                        }
                        else if (i != ScheduleStart.Count - 1 && DateTime.Parse(ScheduleStart[0]) < DateTime.Now && DateTime.Parse(ScheduleStart[i + 1]) > DateTime.Now && DateTime.Now < DateTime.Parse(ScheduleEnd[i]))
                        {
                            if (animLabel == false) anim.Begin();
                            animLabel = true;

                            LabelScheduleCall.Content = "До конца перемены: " + (DateTime.Parse(ScheduleStart[i]) - DateTime.Now).ToString("mm':'ss");
                            break;
                        }
                    }
                    else if (i == ScheduleEnd.Count)
                    {
                        LabelScheduleCall.Content = "";
                        //break;
                    }
                }
            }
        }

        private async void Login()
        {
            ButtonLogin.IsEnabled = false;
            TextBoxLogin.IsEnabled = false;
            TextBoxPassword.IsEnabled = false;
            DataBaseControls DbControls = new();
            string pass = DbControls.Hash(TextBoxPassword.Password);

            var anim = (Storyboard)FindResource("AnimLoadLogin");
            var anim3 = (Storyboard)FindResource("AnimOpenMenuStart");


            using (zhirovContext db = new())
            {
                bool isAvalaible = await db.Database.CanConnectAsync();
                if (isAvalaible)
                {
                    if (!string.IsNullOrWhiteSpace(TextBoxLogin.Text) && TextBoxPassword.Password != string.Empty)
                    {
                        var login = await db.Teachers.Where(p => p.TeachersLogin == TextBoxLogin.Text && p.TeachersPassword == pass).ToListAsync();

                        if (login.Count != 0)
                        {
                            foreach (var l in login)
                            {
                                TextBlockTeacher.Content = $"{l.TeachersName} {l.TeachersSurname}";
                                
                                RefreshImage(l.TeachersImage);

                                switch (l.TeachersAccesAdminPanel)
                                {
                                    case "True":
                                        NavigationViewItemAdminPanel.Visibility = Visibility.Visible;
                                        break;

                                    case "False":
                                        NavigationViewItemAdminPanel.Visibility = Visibility.Hidden;
                                        break;
                                }

                                Properties.Settings.Default.UserID = (int)l.Idteachers;
                                Properties.Settings.Default.Login = TextBoxLogin.Text;

                                Properties.Settings.Default.Save();

                                NavViewMenu.Visibility = Visibility.Visible;
                                NavViewMenuAdmin.Visibility = Visibility.Hidden;
                                RectangleBackToMenu.Visibility = Visibility.Hidden;

                                FillComboBoxGroups();
                                //ComboBoxGroup.SelectedIndex = 0;
                                Frame.Navigate(new Pages.Journal());
                                //Notifications("Оповещение", "Авторизация прошла успешно");

                                timer2.Tick += new EventHandler(SheduleCall);
                                timer2.Interval = new TimeSpan(0, 0, 1);
                                timer2.Start();
                                
                                GridNLogin.Visibility = Visibility.Hidden;
                                GridLogin.Visibility = Visibility.Hidden;
                                anim3.Begin();
                                GridMenu.Visibility = Visibility.Visible;
                                Frame.Visibility = Visibility.Visible;
                                NavigationViewItemJournal.IsSelected = true;

                                anim.Stop();
                                RectangleLoadLogin.Visibility = Visibility.Hidden;
                                TextBoxLogin.IsEnabled = true;
                                TextBoxPassword.IsEnabled = true;
                                ButtonLogin.IsEnabled = true;
                            }
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
                else
                {
                    anim.Stop();
                    RectangleLoadLogin.Visibility = Visibility.Hidden;
                    TextBoxLogin.IsEnabled = true;
                    TextBoxPassword.IsEnabled = true;
                    ButtonLogin.IsEnabled = true;
                    Notifications("Ошибка", "База данных недоступна");
                }
            }

            //command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = PBKDF2HashHelper.VerifyPassword(TextBoxPassword.Password, (string)command.ExecuteScalar());
                       
            
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

        bool loginbool = true;

        private void Frame_ContentRendered(object sender, EventArgs e)
        {
            
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
                PathMenu.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#222222");
            }
            else
            {
                WPFUI.Background.Manager.RemoveDarkMode(windowHandle);
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                PathMenu.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#EDEDED");
            }

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                this.Background = System.Windows.Media.Brushes.Transparent;
                WPFUI.Background.Manager.Apply(WPFUI.Background.BackgroundType.Mica, windowHandle);
            }
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
            a = settingsControl.CheckTray();
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
            try
            {
                Process.Start("Updater.exe");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Файл Updater.exe не найден, выполните проверку на целостность файлов");
            }
            (sender as WPFUI.Controls.MessageBox)?.Close();
        }

        private void MessageBox_RightButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //(sender as WPFUI.Controls.MessageBox)?.Close();
            (sender as WPFUI.Controls.MessageBox)?.Close();
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

        private void MenuItemEJSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.Setting());
        }

        private void MenuItemEJUpdate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Updater.exe"); //Запуск программы обновления
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
            Frame.Navigate(new Pages.AdminPanel.AcademicYears());
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
            SettingMig.CheckStart();
            //settingsControl.CompletionLogin();
            ThemeCheck();

        }

        private void ButtonShowLogin_Click(object sender, RoutedEventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimOpenLogin");
            anim.Begin();

            ButtonShowLogin.IsEnabled = false;
            Frame.Visibility = Visibility.Hidden;
            GridLogin.Visibility = Visibility.Visible;
        }


        private async void FillComboBoxGroups()
        {
            ComboBoxGroup.Items.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Groups.OrderBy(t => t.GroupsNameAbbreviated).ForEachAsync(t =>
                {
                    ComboBoxGroup.Items.Add(t.GroupsNameAbbreviated);
                });
            }
        }

        private void ComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(new Pages.Journal());
        }

        private void IconSetting_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.Setting());
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (!loginbool)
            {
                GridLogin.Visibility = Visibility.Hidden;
                ButtonShowLogin.IsEnabled = true;
            }
            loginbool = false;
            Frame.Visibility = Visibility.Visible;

            var anim = (Storyboard)FindResource("AnimOpenFrame");
            var anim2 = (Storyboard)FindResource("AnimCloseFrame");
            //anim2.Begin();
            anim.Begin();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            SettingsControl sControl = new SettingsControl();
            try
            {
                if (!await sControl.CheckVersionAsync(Properties.Settings.Default.Version))
                {
                    WPFUI.Controls.MessageBox messageBox = new WPFUI.Controls.MessageBox();

                    messageBox.ButtonLeftName = "Скачать";
                    messageBox.ButtonRightName = "Закрыть";
                    messageBox.ButtonLeftClick += MessageBox_LeftButtonClick;
                    messageBox.ButtonRightClick += MessageBox_RightButtonClick;
                    messageBox.Title = "Оповещение";
                    messageBox.Content = $"Доступно новое обновление ElectroJournal\n{Properties.Settings.Default.Version} -> {await sControl.VersionAsync()}";
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Notifications("Уведомление", "Невозможно подключиться к базе данных" + ex);
            }
               
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;
            }
        }

        public void RefreshImage(string path)
        {
            if (!String.IsNullOrWhiteSpace(path))
            {
                var stringPath = $@"{path}";

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.Default;
                bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                bitmapImage.EndInit();
                PersonPicture.ProfilePicture = bitmapImage;
            }
            else
            {
                PersonPicture.ProfilePicture = null;
                PersonPicture.DisplayName = (string)TextBlockTeacher.Content;
            }
        }
    }
}
