using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Compression;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Interop;
using System.ComponentModel;

namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }

        private bool _isDarkTheme = false;

        public void ThemeCheck()
        {
            int theme = 0;

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

        private void ButtonDownloadUpdate_Click(object sender, RoutedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("ElectroJournal"))
            {
                process.Kill();
            }
            ButtonDownloadUpdate.IsEnabled = false;
            LabelInformation.Visibility = Visibility.Visible;
            DownloadFiles();
        }

        private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        private void DownloadFiles()
        {
            WebClient wc = new WebClient();

            string url = "http://techno-review.ru/download/net5.0-windows.zip";
            string name = "ElectroJournal.zip";

            //wc.DownloadFile(url, name);

            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            stopwatch.Start();
            wc.DownloadFileAsync(new Uri(url), name);

            
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //ЗАПУСК КОНСОЛЬНОГО ПРИЛОЖЕНИЯ ДЛЯ РАСПАКОВКИ
            ButtonDownloadUpdate.IsEnabled = true;
            LabelInformation.Visibility= Visibility.Hidden;
            ProgressBarLoad.Value = 0;
            try
            {
                Process.Start("UpdaterUnZip.exe");
                this.Close();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Программа для распаковки не найдена, переустановите приложение.", "Ошибка");                
            }
            
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            string downloadedMBs = Math.Round(e.BytesReceived / 1024.0 / 1024.0) + " MB";
            string totalMBs = Math.Round(e.TotalBytesToReceive / 1024.0 / 1024.0) + " MB";

            LabelInformation.Content ="Загружено: " + downloadedMBs + "/" + totalMBs + "\tСкорость: " + string.Format("{0} MB/s", (e.BytesReceived / 1024.0 / 1024.0 / stopwatch.Elapsed.TotalSeconds).ToString("0.00"));
            ProgressBarLoad.Maximum = (int)e.TotalBytesToReceive / 100;
            ProgressBarLoad.Value = (int)e.BytesReceived / 100;
        }
    }
}
