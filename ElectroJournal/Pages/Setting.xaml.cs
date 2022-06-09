using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using ElectroJournal.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Setting.xaml
    /// </summary>
    public partial class Setting : Page
    {
        public Setting()
        {
            InitializeComponent();
            LoadApp();
        }

        private bool isLoaded = false;
        SettingsControl settingsControl = new();

        private void LoadApp()
        {
            ComboBoxTheme.SelectedIndex = Properties.Settings.Default.Theme;
            CheckBoxRememberData.IsChecked = Properties.Settings.Default.RememberData;
            CheckBoxAutoRun.IsChecked = Properties.Settings.Default.AutoRun;
            CheckBoxCollapseToTray.IsChecked = Properties.Settings.Default.Tray;
            CheckBoxUpdate.IsChecked = Properties.Settings.Default.CheckUpdate;
            LabelVersion.Content = $"Версия {Properties.Settings.Default.Version} от 06.06.2022";

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.Server)) LabelIpAddress.Content = Properties.Settings.Default.Server; 
            else LabelIpAddress.Content = "отсутствует";
        }
        private async void LoadSettingDB()
        {
            try
            {
                var anim2 = (Storyboard)FindResource("AnimCloseSyncPrivate");
                using (zhirovContext db = new())
                {
                    Classes.DataBaseEF.Setting? s = await db.Settings.FirstOrDefaultAsync(s => s.TeachersIdteachers == Properties.Settings.Default.UserID);

                    if (s != null)
                    {
                        if (s.SettingsPhone == 1)
                        {
                            Properties.Settings.Default.ShowPhone = false;
                        }
                        else
                        {
                            Properties.Settings.Default.ShowPhone = true;
                        }

                        if (s.SettingsEmail == 1)
                        {
                            Properties.Settings.Default.ShowEmail = false;
                        }
                        else
                        {
                            Properties.Settings.Default.ShowEmail = true;
                        }

                        Properties.Settings.Default.Save();
                    }
                }

                bool showPhone = Properties.Settings.Default.ShowPhone;
                bool showEmail = Properties.Settings.Default.ShowEmail;

                CheckBoxShowPhone.IsChecked = showPhone;
                CheckBoxShowEmail.IsChecked = showEmail;

                anim2.Begin();
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"LoadSettingDB | {ex.Message}");
                MessageBox.Show(ex.Message, "LoadSettingDB");
            }
        }
        private void SaveApp()
        {
            Properties.Settings.Default.Theme = ComboBoxTheme.SelectedIndex;
            Properties.Settings.Default.AutoRun = CheckBoxAutoRun.IsChecked ?? false;
            Properties.Settings.Default.Tray = CheckBoxCollapseToTray.IsChecked ?? false;
            Properties.Settings.Default.RememberData = CheckBoxRememberData.IsChecked ?? false;
            Properties.Settings.Default.ShowPhone = CheckBoxShowPhone.IsChecked ?? false;
            Properties.Settings.Default.ShowEmail = CheckBoxShowEmail.IsChecked ?? false;
            Properties.Settings.Default.CheckUpdate = CheckBoxUpdate.IsChecked ?? false;

            Properties.Settings.Default.Save();

            SettingsControl settingsControl = new();
            settingsControl.CheckAutoRun();
            //settingsControl.CompletionLogin();
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
        }
        private void ButtonChangeBD_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).AnimLogout(true);
            new DBUser().ShowDialog();
        }
        private async void ButtonOpenUpdater_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonOpenUpdater.Content != "Скачать")
                {
                    var anim = (Storyboard)FindResource("AnimOpenCheckUpdate");
                    var anim2 = (Storyboard)FindResource("AnimYNewUpdate");
                    var anim3 = (Storyboard)FindResource("AnimCloseCheckUpdate");
                    anim.Begin();
                    ButtonOpenUpdater.IsEnabled = false;

                    if (!await SettingsControl.CheckVersionAsync(Properties.Settings.Default.Version))
                    {
                        LabelNewVersion.Content = $"Доступно новое обновление {Properties.Settings.Default.Version} -> {await SettingsControl.VersionAsync()}";
                        ButtonOpenUpdater.Content = "Скачать";
                        ButtonOpenUpdater.IsEnabled = true;
                        anim2.Begin();
                    }
                    else
                    {
                        anim3.Begin();
                        ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Обновлений не найдено");
                        ButtonOpenUpdater.IsEnabled = true;
                    }
                }
                else if (ButtonOpenUpdater.Content == "Скачать")
                {
                    try
                    {
                        Process.Start("Updater.exe");
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Файл Updater.exe не найден, переустановите приложение");
                    }
                }
            }
            catch (Exception ex)
            {
                var anim3 = (Storyboard)FindResource("AnimCloseCheckUpdate");
                anim3.Begin();
                ButtonOpenUpdater.IsEnabled = true;
                SettingsControl.InputLog($"ButtonOpenUpdater_Click | {ex.Message}");
                ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Не удалось проверить обновление");
            }
        }
        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                SaveApp();
                ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            }
            else
            {
                isLoaded = true;
            }
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) => SaveApp();
        private void Page_Unloaded(object sender, RoutedEventArgs e) => isLoaded = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (((MainWindow)Application.Current.MainWindow).isEntry) CardExpanderPrivate.Visibility = Visibility.Visible;
            else CardExpanderPrivate.Visibility = Visibility.Hidden;
        }
        private void CheckBoxDB_Click(object sender, RoutedEventArgs e) => SaveDataDB();
        private async void SaveDataDB()
        {
            try
            {
                var anim = (Storyboard)FindResource("AnimOpenSave");
                var anim2 = (Storyboard)FindResource("AnimCloseSave");
                anim.Begin();
                Properties.Settings.Default.ShowPhone = CheckBoxShowPhone.IsChecked ?? false;
                Properties.Settings.Default.ShowEmail = CheckBoxShowEmail.IsChecked ?? false;
                Properties.Settings.Default.Save();

                using zhirovContext db = new();
                Classes.DataBaseEF.Setting? setting = await db.Settings.FirstOrDefaultAsync(s => s.TeachersIdteachers == Properties.Settings.Default.UserID);

                if (setting != null)
                {
                    setting.SettingsEmail = (sbyte)((bool)CheckBoxShowEmail.IsChecked ? 0 : 1);
                    setting.SettingsPhone = (sbyte)((bool)CheckBoxShowPhone.IsChecked ? 0 : 1);

                    await db.SaveChangesAsync();
                }
                anim2.Begin();
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"SaveDataDB | {ex.Message}");
            }
        }
        private void CardExpanderPrivate_Expanded(object sender, RoutedEventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimOpenSyncPrivate");
            anim.Begin();
            LoadSettingDB();
        }
    }
}
