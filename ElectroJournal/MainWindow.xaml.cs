﻿using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using ElectroJournal.Pages;
using ElectroJournal.Windows;
using Microsoft.EntityFrameworkCore;
using ModernWpf;
using ModernWpf.Controls;
using ModernWpf.SampleApp.ControlPages;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPFUI.Controls;

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
            GridComboGroups.Visibility = Visibility.Hidden;

            settingsControl.CheckAutoRun();
            settingsControl.CheckTray();

            GridNLogin.Visibility = Visibility.Visible;
        }

        SettingsControl settingsControl = new();
        SettingMigration SettingMig = new();

        private readonly NotificationManager _notificationManager = new();
        public System.Windows.Threading.DispatcherTimer timer2 = new();
        public bool isEntry = false;

        bool checkFiilScheduleDB = true;
        public bool animLabel = true;
        private bool _isDarkTheme = false;
        private bool loginbool = true;

        List<string> ScheduleStart = new();
        List<string> ScheduleEnd = new();
        List<int> ScheduleNumber = new();

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
                    using zhirovContext db = new();
                    var time = await db.Periodclasses.ToListAsync();

                    foreach (var t in time)
                    {
                        ScheduleStart.Add(t.PeriodclassesStart.ToString("hh':'mm"));
                        ScheduleEnd.Add(t.PeriodclassesEnd.ToString("hh':'mm"));
                        ScheduleNumber.Add(t.PeriodclassesNumber);
                        checkFiilScheduleDB = false;
                    }
                }

                var anim = (Storyboard)FindResource("AnimLabelScheduleCall");

                for (int i = 0; i <= ScheduleStart.Count; i++)
                {
                    if (i != ScheduleStart.Count)
                    {
                        if (DateTime.Parse(ScheduleStart[i]) < DateTime.Now && DateTime.Now < DateTime.Parse(ScheduleEnd[i]))
                        {
                            if (animLabel)
                            {
                                anim.Begin();
                            }

                            animLabel = false;

                            LabelScheduleCall.Content = $"Урок: {ScheduleNumber[i]}    Период занятий: {ScheduleStart[i]} - {ScheduleEnd[i]}    До конца занятий: " +
                               (DateTime.Parse(ScheduleEnd[i]) - DateTime.Now).ToString("mm':'ss");
                            break;
                        }
                        else if (i != ScheduleStart.Count - 1 && DateTime.Parse(ScheduleStart[0]) < DateTime.Now && DateTime.Parse(ScheduleStart[i + 1]) > DateTime.Now && DateTime.Now < DateTime.Parse(ScheduleEnd[i]))
                        {
                            if (animLabel == false)
                            {
                                anim.Begin();
                            }

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
            try
            {
                ButtonLogin.IsEnabled = false;
                TextBoxLogin.IsEnabled = false;
                TextBoxPassword.IsEnabled = false;
                DataBaseControls DbControls = new();
                string pass = DbControls.Hash(TextBoxPassword.Password);

                var anim = (Storyboard)FindResource("AnimLoadLogin");
                var anim3 = (Storyboard)FindResource("AnimOpenMenuStart");

                using zhirovContext db = new();
                bool isAvalaible = await db.Database.CanConnectAsync();
                if (isAvalaible)
                {
                    if (!string.IsNullOrWhiteSpace(TextBoxLogin.Text) && TextBoxPassword.Password != string.Empty)
                    {
                        var login = await db.Teachers.Where(p => p.TeachersLogin == TextBoxLogin.Text.Trim() && p.TeachersPassword == pass).ToListAsync();

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
                                Properties.Settings.Default.PassProfile = TextBoxPassword.Password;
                                Properties.Settings.Default.FirstName = l.TeachersName;

                                Properties.Settings.Default.Save();

                                Teacher? teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == Properties.Settings.Default.UserID);

                                if (teacher != null)
                                {
                                    teacher.TeachersStatus = 1;
                                }

                                await db.SaveChangesAsync();

                                NavViewMenu.Visibility = Visibility.Visible;
                                GridComboGroups.Visibility = Visibility.Visible;
                                NavViewMenuAdmin.Visibility = Visibility.Hidden;
                                RectangleBackToMenu.Visibility = Visibility.Hidden;

                                isEntry = true;
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

            catch (Exception ex)
            {
                settingsControl.InputLog($"Login | {ex.Message}");
                System.Windows.MessageBox.Show(ex.Message);
            }
            //command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = PBKDF2HashHelper.VerifyPassword(TextBoxPassword.Password, (string)command.ExecuteScalar());
        }
        private void RectangleBackToMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavViewMenu.Visibility = Visibility.Visible;
            var anim = (Storyboard)FindResource("AnimOpenMenu");
            anim.Begin();

            NavViewMenuAdmin.Visibility = Visibility.Hidden;
            RectangleBackToMenu.Visibility = Visibility.Hidden;

            NavigationViewItemJournal.IsSelected = true;
            Frame.Navigate(new Pages.Journal());
        }
        private void Frame_ContentRendered(object sender, EventArgs e)
        {

        }
        public void Notifications(string title, string text)
        {
            _notificationManager.Show(
                new NotificationContent { Title = title, Message = text, Type = NotificationType.Information },
                areaName: "WindowArea");
        }
        public void ThemeCheck()
        {
            int theme = Properties.Settings.Default.Theme;

            _isDarkTheme = theme == 1;

            var newTheme = theme == 0
            ? WPFUI.Appearance.ThemeType.Light
            : WPFUI.Appearance.ThemeType.Dark;

            if (_isDarkTheme)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                PathMenu.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#222222");
                GridWhiteTheme.Visibility = Visibility.Hidden;
                GridBlackTheme.Visibility = Visibility.Visible;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                PathMenu.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#EDEDED");
                GridBlackTheme.Visibility = Visibility.Hidden;
                GridWhiteTheme.Visibility = Visibility.Visible;
            }

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                GridBlackTheme.Visibility = Visibility.Hidden;
                GridWhiteTheme.Visibility = Visibility.Hidden;
                this.Background = System.Windows.Media.Brushes.Transparent;

                WPFUI.Appearance.Theme.Apply(
           themeType: newTheme,
           backgroundEffect: WPFUI.Appearance.BackgroundType.Mica,
           updateAccent: true,
           forceBackground: false);
            }
            else
            {
                WPFUI.Appearance.Theme.Apply(
           themeType: newTheme,
           updateAccent: true,
           forceBackground: false);
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
            e.Cancel = true;
            bool a;
            a = settingsControl.CheckTray();
            SettingsControl sc = new();
            var anim = (Storyboard)FindResource("AnimExitProgram");

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
                ContentDialogExample dialog = new();
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    anim.Begin();
                    bool b = await sc.ExitUser();
                    if (b || !b)
                    {
                        foreach (var process in Process.GetProcessesByName("ElectroJournal"))
                        {
                            process.Kill();
                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                }

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
            Board board = new();
            PrintDialog printDialog = new();
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

        #region События меню
        private void NavigationViewItemBoard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Board());
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
            var anim = (Storyboard)FindResource("AnimOpenMenuAdmin");
            anim.Begin();
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
        private void RectangleUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Users? user;
            Frame.Navigate(user = new());
            GC.SuppressFinalize(this);
            //GC.Collect();
            //user = null;
            //Frame.Navigate(new Pages.Users());
            NavigationViewItemJournal.IsSelected = false;
        }
        #endregion

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
            settingsControl.CompletionLogin();
            settingsControl.CreateDirLogs();
            settingsControl.LogFileCreate();
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

            using zhirovContext db = new();
            await db.Groups.OrderBy(t => t.GroupsNameAbbreviated).ForEachAsync(t =>
            {
                ComboBoxGroup.Items.Add(t.GroupsNameAbbreviated);
            });

            ComboBoxGroup.SelectedItem = "ПКС-4"; // удалить
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
            var anim2 = (Storyboard)FindResource("AnimCloseFrame");
            anim2.Begin();

            if (!loginbool)
            {
                GridLogin.Visibility = Visibility.Hidden;
                ButtonShowLogin.IsEnabled = true;
            }
            loginbool = false;
            Frame.Visibility = Visibility.Visible;

            var anim = (Storyboard)FindResource("AnimOpenFrame");

            anim.Begin();
        }
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            SettingsControl sControl = new();
            try
            {
                if (!await sControl.CheckVersionAsync(Properties.Settings.Default.Version))
                {
                    WPFUI.Controls.MessageBox messageBox = new();

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
                settingsControl.InputLog($"Window_ContentRendered | {ex.Message}");
                //System.Windows.MessageBox.Show(ex.Message);
                Notifications("Уведомление", "Проверка обновления неудачная");
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

                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.Default;
                bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                bitmapImage.DecodePixelHeight = 100;
                bitmapImage.EndInit();
                PersonPicture.ProfilePicture = bitmapImage;
            }
            else
            {
                PersonPicture.ProfilePicture = null;
                PersonPicture.DisplayName = (string)TextBlockTeacher.Content;
            }
        }
        public ImageSource? GetImageUser(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                var stringPath = $@"{path}";

                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                bitmapImage.EndInit();

                return bitmapImage;
            }
            return null;
        }
        private void NavigationViewItemAcademicYears_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Pages.AdminPanel.AcademicYears());
        }

        /*        private void NavViewMenu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
                {
                    var selectedItem = (ModernWpf.Controls.NavigationViewItem)args.SelectedItem;
                    string pageName = "ElectroJournal.Pages." + (string)selectedItem.Tag;
                    Type pageType = typeof(Pages.Journal).Assembly.GetType(pageName);
                    Frame.Navigate(pageType);
                }*/
    }

    public class BackgroundGrid
    {
        public string? color1 { get; set; }
        public string? color2 { get; set; }
        public string? color3 { get; set; }
        public string? color4 { get; set; }
        public string? color5 { get; set; }
    }
}
