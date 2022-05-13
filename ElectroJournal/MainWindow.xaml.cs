using ElectroJournal.Classes;
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
            InitializeComponent();

            GridMenu.Visibility = Visibility.Hidden;
            NavViewMenuAdmin.Visibility = Visibility.Hidden;
            RectangleBackToMenu.Visibility = Visibility.Hidden;
            GridComboGroups.Visibility = Visibility.Hidden;

            settingsControl.CheckAutoRun();
            settingsControl.CheckTray();
            GridNLogin.Visibility = Visibility.Visible;
            Frame.Navigate(new Authorization());
        }

        SettingsControl settingsControl = new();
        SettingMigration SettingMig = new();

        private readonly NotificationManager _notificationManager = new();
        public bool isEntry = false;
        
        private bool _isDarkTheme = false;
        public bool loginbool = true;

        public void StartCalls() => Calls.StartTimer();
        public void StopCalls() => Calls.StopTimer();
        public void OpenMenu() => (Resources["AnimOpenMenuStart"] as Storyboard).Begin();
        private void RectangleUser_MouseMove(object sender, MouseEventArgs e) => this.Cursor = Cursors.Hand;
        private void RectangleUser_MouseLeave(object sender, MouseEventArgs e) => this.Cursor = Cursors.Arrow;               
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
        public void AnimLogout(bool a)
        {
            if (a)
            {
                settingsControl.ExitUser();
                StopCalls();
                Calls.checkFillScheduleDB = true;
                Calls.startAnim = true;
                Calls.LabelScheduleCall.Content = "";
                isEntry = false;
                Calls.Visibility = Visibility.Hidden;
                GridComboGroups.Visibility = Visibility.Hidden;
                GridMenu.Visibility = Visibility.Hidden;
                GridNLogin.Visibility = Visibility.Visible;
                loginbool = true;
                ButtonShowLogin.IsEnabled = false;

                var anim = (Storyboard)FindResource("AnimLogoutUser");
                anim.Begin();

                Frame.Navigate(new Authorization());
            }
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            bool a = settingsControl.CheckTray();
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
                else e.Cancel = true;
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
        private void MenuItemEJHelp_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItemEJProgInfo_Click(object sender, RoutedEventArgs e) => new ProgramInformation().ShowDialog();
        private void MenuItemEJBug_Click(object sender, RoutedEventArgs e) => new Help().ShowDialog();

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
            NavigationViewItemJournal.IsSelected = false;
        }
        #endregion

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e) => Show();
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("ElectroJournal"))
            {
                process.Kill();
            }
        }
        private void TitleBar_NotifyIconDoubleClick(object sender, RoutedEventArgs e) => Show();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingMig.CheckStart();
            settingsControl.CreateDirLogs();
            settingsControl.LogFileCreate();
            ThemeCheck();           
        }
        private void ButtonShowLogin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Authorization());
            ButtonShowLogin.IsEnabled = false;
        }
        public async void FillComboBoxGroups()
        {
            ComboBoxGroup.Items.Clear();

            using zhirovContext db = new();
            await db.Groups.OrderBy(t => t.GroupsNameAbbreviated).ForEachAsync(t =>
            {
                ComboBoxGroup.Items.Add(t.GroupsNameAbbreviated);
            });

            ComboBoxGroup.SelectedItem = "ПКС-4"; // удалить
        }
        private void ComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e) => Frame.Navigate(new Pages.Journal());
        public void UpdateUserInfo(string path, string fio)
        {
            UserInfo.RefreshImage(path);
            UserInfo.TextBlockTeacher.Content = fio;
        }
        public void UpdateUserInfo(string path) => UserInfo.RefreshImage(path);
        private void IconSetting_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => Frame.Navigate(new Pages.Setting());
        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
           //(Resources["AnimCloseFrame"] as Storyboard).Begin();
            if (!loginbool) ButtonShowLogin.IsEnabled = true;
            loginbool = false;
            (Resources["AnimOpenFrame"] as Storyboard).Begin();
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
                Notifications("Уведомление", "Проверка обновления неудачная");
            }
        } // Проверка обновления
        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back) e.Cancel = true;
        }
        private void NavigationViewItemAcademicYears_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => Frame.Navigate(new Pages.AdminPanel.AcademicYears());

        /*        private void NavViewMenu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
                {
                    var selectedItem = (ModernWpf.Controls.NavigationViewItem)args.SelectedItem;
                    string pageName = "ElectroJournal.Pages." + (string)selectedItem.Tag;
                    Type pageType = typeof(Pages.Journal).Assembly.GetType(pageName);
                    Frame.Navigate(pageType);
                }*/
    }
}