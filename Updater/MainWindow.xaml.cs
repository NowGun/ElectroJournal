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
using WPFUI.Taskbar;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Windows.Media.Animation;

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
            LoadChange();
        }

        private readonly Stopwatch stopwatch = new();

        private async void LoadChange()
        {
            Ping ping = new();

            try
            {
                PingReply reply = ping.Send("193.33.230.80", 4000);
                if (reply.Status == IPStatus.Success)
                {
                    HttpClient hc = new();
                    RichTextBoxUpdate.Document.Blocks.Add(new Paragraph(new Run(await hc.GetStringAsync(new Uri("http://193.33.230.80/public_html/pages/ChangeLog.txt")))));
                }
                else
                {
                    RichTextBoxUpdate.Document.Blocks.Add(new Paragraph(new Run("Обновление программы в данный момент недоступно.")));
                    ButtonDownloadUpdate.IsEnabled = false;
                }
            }
            catch (PingException ex)
            {

            }            
        }
        private void ButtonDownloadUpdate_Click(object sender, RoutedEventArgs e)
        {
            var anim2 = (Storyboard)FindResource("AnimOpenLoad");
            Progress.SetState(ProgressState.None, false);
            foreach (var process in Process.GetProcessesByName("ElectroJournal"))
            {
                process.Kill();
            }
            ButtonDownloadUpdate.IsEnabled = false;
            LabelInformation.Visibility = Visibility.Visible;
            anim2.Begin();
            DownloadFiles();
        }
        private void DownloadFiles()
        {
            try
            {
                WebClient wc = new();

                string url = "http://193.33.230.80/public_html/download/net5.0-windows.zip";
                string name = "ElectroJournal.zip";

                Progress.SetState(ProgressState.Indeterminate, false);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                stopwatch.Start();
                wc.DownloadFileAsync(new Uri(url), name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) // запуск приложения для распаковки
        {
            var anim2 = (Storyboard)FindResource("AnimCloseLoad");
            ButtonDownloadUpdate.IsEnabled = true;
            try
            {
                Process.Start("UpdaterUnZip.exe");
                this.Close();
            }
            catch (Win32Exception)
            {
                Progress.SetValue(100, 100, false);
                Progress.SetState(ProgressState.Error, false);
                ProgressBarLoad.ShowError = true;
                MessageBox.Show("Программа для распаковки не найдена, переустановите приложение.", "Ошибка");
                ProgressBarLoad.ShowError = false;
                ProgressBarLoad.Value = 0;
                Progress.SetState(ProgressState.Normal, false);
            }
            finally
            {
                anim2.Begin();
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
