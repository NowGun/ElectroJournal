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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для UpdateInfo.xaml
    /// </summary>
    public partial class UpdateInfo : Window
    {
        public UpdateInfo()
        {
            InitializeComponent();
            TitleBar.CloseActionOverride = CloseActionOverride;
        }

        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }
        private bool _isDarkTheme = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }

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
            }
            else
            {
                WPFUI.Background.Manager.RemoveDarkMode(windowHandle);
            }

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                this.Background = System.Windows.Media.Brushes.Transparent;
                WPFUI.Background.Manager.Apply(WPFUI.Background.BackgroundType.Mica, windowHandle);
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("Updater.exe");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Файл Updater.exe не найден, выполните проверку на целостность файлов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Файл Updater.exe не найден, выполните проверку на целостность файлов в настройках приложения");
            }
            this.Close();
        }
    }
}
