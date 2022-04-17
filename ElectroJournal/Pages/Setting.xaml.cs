using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using ElectroJournal.Windows;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {          
            
            
        }
        private void LoadApp()
        {
            //CheckBoxAnim.IsChecked = Properties.Settings.Default.Animation;
            ComboBoxTheme.SelectedIndex = Properties.Settings.Default.Theme;
            CheckBoxRememberData.IsChecked = Properties.Settings.Default.RememberData;
            CheckBoxAutoRun.IsChecked = Properties.Settings.Default.AutoRun;
            CheckBoxCollapseToTray.IsChecked = Properties.Settings.Default.Tray;           
            LabelIpAddress.Content = Properties.Settings.Default.Server;
            LabelVersion.Content = $"Версия {Properties.Settings.Default.Version} от 16.04.2022";
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
                        if (s.SettingsPhone == 1) Properties.Settings.Default.ShowPhone = false;
                        else Properties.Settings.Default.ShowPhone = true;
                        
                        if (s.SettingsEmail == 1) Properties.Settings.Default.ShowEmail = false;
                        else Properties.Settings.Default.ShowEmail = true;

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
                MessageBox.Show(ex.Message, "LoadSettingDB");
            }
        }
        private void SaveApp()
        {
            Properties.Settings.Default.Theme = ComboBoxTheme.SelectedIndex;
            //Properties.Settings.Default.Animation = CheckBoxAnim.IsChecked ?? false;
            Properties.Settings.Default.AutoRun = CheckBoxAutoRun.IsChecked ?? false;
            Properties.Settings.Default.Tray = CheckBoxCollapseToTray.IsChecked ?? false;
            Properties.Settings.Default.RememberData = CheckBoxRememberData.IsChecked ?? false;
            Properties.Settings.Default.ShowPhone = CheckBoxShowPhone.IsChecked ?? false;
            Properties.Settings.Default.ShowEmail = CheckBoxShowEmail.IsChecked ?? false;

            Properties.Settings.Default.Save();

            SettingsControl settingsControl = new();
            settingsControl.CheckAutoRun();
            //settingsControl.CompletionLogin();
            ((MainWindow)System.Windows.Application.Current.MainWindow).ThemeCheck();
            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные успешно сохранены");
        }
        private void ButtonChangeBD_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            new DBUser().ShowDialog();
        }
        private async void ButtonOpenUpdater_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonOpenUpdater.Content != "Скачать")
                {
                    SettingsControl sControl = new SettingsControl();

                    var anim = (Storyboard)FindResource("AnimOpenCheckUpdate");
                    var anim2 = (Storyboard)FindResource("AnimYNewUpdate");
                    var anim3 = (Storyboard)FindResource("AnimCloseCheckUpdate");
                    anim.Begin();
                    ButtonOpenUpdater.IsEnabled = false;

                    if (!await sControl.CheckVersionAsync(Properties.Settings.Default.Version))
                    {
                        anim2.Begin();
                        LabelNewVersion.Content = $"Доступно новое обновление {Properties.Settings.Default.Version} -> {await sControl.VersionAsync()}";
                        ButtonOpenUpdater.Content = "Скачать";
                        ButtonOpenUpdater.IsEnabled = true;
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
                        ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Файл Updater.exe не найден, выполните проверку на целостность файлов");
                    }
                }
            }
            catch (Exception ex)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Не удалось проверить");
            }
        }
        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                SaveApp();
                ((MainWindow)System.Windows.Application.Current.MainWindow).ThemeCheck();
            }
            else isLoaded = true;            
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            SaveApp();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            isLoaded = false;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (((MainWindow)System.Windows.Application.Current.MainWindow).isEntry)
            {
                CardExpanderPrivate.Visibility = Visibility.Visible;
            }
            else CardExpanderPrivate.Visibility = Visibility.Hidden;
        }
        private void CheckBoxDB_Click(object sender, RoutedEventArgs e)
        {
            SaveDataDB();
        }
        private async void SaveDataDB()
        {
            var anim = (Storyboard)FindResource("AnimOpenSave");
            var anim2 = (Storyboard)FindResource("AnimCloseSave");
            anim.Begin();
            Properties.Settings.Default.ShowPhone = CheckBoxShowPhone.IsChecked ?? false;
            Properties.Settings.Default.ShowEmail = CheckBoxShowEmail.IsChecked ?? false;

            Properties.Settings.Default.Save();

            using (zhirovContext db = new zhirovContext())
            {
                Classes.DataBaseEF.Setting? setting = await db.Settings.FirstOrDefaultAsync(s => s.TeachersIdteachers == Properties.Settings.Default.UserID);                

                if (setting != null)
                {
                    setting.SettingsEmail = (sbyte)((bool)CheckBoxShowEmail.IsChecked ? 0 : 1);
                    setting.SettingsPhone = (sbyte)((bool)CheckBoxShowPhone.IsChecked ? 0 : 1);

                    await db.SaveChangesAsync();

                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                }
            }
            anim2.Begin();
        }
        private void CardExpanderPrivate_Expanded(object sender, RoutedEventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimOpenSyncPrivate");            
            anim.Begin();
            LoadSettingDB();
        }
    }
}
