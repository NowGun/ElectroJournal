using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using ElectroJournal.Pages;
using ElectroJournal.Windows;
using Microsoft.EntityFrameworkCore;
using ModernWpf;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using ModernWpf.SampleApp.ControlPages;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            InvokeSplashScreen();
        }

        SettingsControl settingsControl = new();
        SettingMigration SettingMig = new();
        Classes.Navigation nav = new();

        private readonly NotificationManager _notificationManager = new();
        public bool isEntry = false;
        
        private bool _isDarkTheme = false;
        public bool loginbool = true;
        public bool IsOpenSetting = false;
        public bool IsOpenUsers = false;

        public void StartCalls() => Calls.StartTimer();
        public void StopCalls() => Calls.StopTimer();
        public void OpenMenu() => ((Storyboard)Resources["AnimOpenMenuStart"]).Begin();
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
            nav.NavigationPage("Journal");
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

                nav.NavigationPage("Authorization");
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
        private void MessageBox_LeftButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("Updater.exe");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Файл Updater.exe не найден, выполните проверку на целостность файлов");
            }
            finally
            {
                (sender as WPFUI.Controls.MessageBox)?.Close();
            }
        }
        private void MessageBox_RightButtonClick(object sender, RoutedEventArgs e) => (sender as WPFUI.Controls.MessageBox)?.Close();
        private void MenuItemEJHelp_Click(object sender, RoutedEventArgs e) => new HelpProgram().Show();
        private void MenuItemEJProgInfo_Click(object sender, RoutedEventArgs e) => new ProgramInformation().ShowDialog();
        private void MenuItemEJBug_Click(object sender, RoutedEventArgs e) => new Help().ShowDialog(); 
        private void NavigationViewItemAdminPanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimOpenMenuAdmin");
            anim.Begin();
            NavViewMenu.Visibility = Visibility.Hidden;
            NavViewMenuAdmin.Visibility = Visibility.Visible;
            NavigationViewItemTeachers.IsSelected = true;
            RectangleBackToMenu.Visibility = Visibility.Visible;
        }
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e) => Show();
        private async void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            SettingsControl sc = new();
            var anim = (Storyboard)FindResource("AnimExitProgram");
            Show();
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
            nav.NavigationPage("Authorization");
            ButtonShowLogin.IsEnabled = false;
            NavDeselect();
        }
        public async void FillComboBoxGroups()
        {
            ComboBoxGroup.Items.Clear();
            using zhirovContext db = new();
            await db.TeachersHasGroups.Where(g => g.TeachersIdteachers == Properties.Settings.Default.UserID).Include(g => g.GroupsIdgroupsNavigation).ForEachAsync(g => ComboBoxGroup.Items.Add(g.GroupsIdgroupsNavigation.GroupsNameAbbreviated));
        }
        private void ComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e) => nav.NavigationPage("Journal");
        public void UpdateUserInfo(string path, string fio)
        {
            UserInfo.TextBlockTeacher.Content = fio;
            UserInfo.RefreshImage(path);
        }
        public void UpdateUserInfo(string path) => UserInfo.RefreshImage(path);
        private void IconSetting_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            nav.NavigationPage("Setting");
            NavDeselect();
        }
        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (!loginbool) ButtonShowLogin.IsEnabled = true;
            loginbool = false;
            
            if (Environment.OSVersion.Version.Build < 1903) ((Storyboard)Resources["AnimOpenFrame"]).Begin();
        }
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.CheckUpdate)
                {
                    if (!await SettingsControl.CheckVersionAsync(Properties.Settings.Default.Version))
                    {
                        WPFUI.Controls.MessageBox messageBox = new();

                        messageBox.ButtonLeftName = "Скачать";
                        messageBox.ButtonRightName = "Закрыть";
                        messageBox.ButtonLeftClick += MessageBox_LeftButtonClick;
                        messageBox.ButtonRightClick += MessageBox_RightButtonClick;
                        messageBox.Title = "Оповещение";
                        messageBox.Content = $"Доступно новое обновление ElectroJournal\n{Properties.Settings.Default.Version} -> {await SettingsControl.VersionAsync()}";
                        messageBox.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"Window_ContentRendered | {ex.Message}");
                Notifications("Уведомление", "Проверка обновления неудачная");
            }
        } // Проверка обновления
        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back) e.Cancel = true;
        }
        private void NavViewMenu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem != null)
                nav.NavigationPage((string)((NavigationViewItem)args.SelectedItem).Tag);
        }
        private void InvokeSplashScreen()
        {
            GridMain.Visibility = Visibility.Visible;
            GridMenu.Visibility = Visibility.Hidden;
            NavViewMenuAdmin.Visibility = Visibility.Hidden;
            RectangleBackToMenu.Visibility = Visibility.Hidden;
            GridComboGroups.Visibility = Visibility.Hidden;

            settingsControl.CheckAutoRun();
            settingsControl.CheckTray();
            GridNLogin.Visibility = Visibility.Visible;

            nav.NavigationPage("Authorization");
        }
        public void NavDeselect()
        {
            NavigationViewItemJournal.IsSelected = false;
            NavigationViewItemSchedule.IsSelected = false;
            NavigationViewItemAcademicYears.IsSelected = false;
            NavigationViewItemAdminPanel.IsSelected = false;
            NavigationViewItemCabinets.IsSelected = false;
            NavigationViewItemDisciplines.IsSelected = false;
            NavigationViewItemGroups.IsSelected = false;
            NavigationViewItemSchedule1.IsSelected = false;
            NavigationViewItemScheduleAdmin.IsSelected = false;
            NavigationViewItemStudents.IsSelected = false;
            NavigationViewItemTeachers.IsSelected = false;
            NavigationViewItemWord.IsSelected = false;
            NavigationViewItemWord1.IsSelected = false;
            NavigationViewItemGroup.IsSelected = false; 
        }
        private void MenuItemOpenJournal_Click(object sender, RoutedEventArgs e)
        {
            Show();
            if (isEntry)
                nav.NavigationPage("Journal");
            else
                nav.NavigationPage("Authorization");
        }
        private void MenuItemOpenSchedule_Click(object sender, RoutedEventArgs e)
        {
            Show();
            nav.NavigationPage("Schedule");
        }
    }
}