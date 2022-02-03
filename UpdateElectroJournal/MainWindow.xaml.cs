using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
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

namespace UpdateElectroJournal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SpeechRecognitionEngine listner = new SpeechRecognitionEngine();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void UnPackNewVersion()
        {
            // путь к архиву
            const string archivePath = @"ElectroJournal.zip";
            // путь к папке
            const string directoryPath = @"newVersion";

            // вызов метода для извлечения файлов из архива
            ZipFile.ExtractToDirectory(archivePath, directoryPath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateDir();
            DownloadFiles();
            UnPackNewVersion();
            MoveFiles();
            DeleteDir();
            Process.Start("ElectroJournal.exe");
            Close();
        }

        private void CreateDir()
        {
            string root = @"newVersion";
            DirectoryInfo directory = new DirectoryInfo(root);
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        private void DeleteDir()
        {
            string root = @"newVersion";
            try
            {
                DirectoryInfo directory = new DirectoryInfo(root);
                directory.Delete(true);
            }
            catch (Exception ex)
            {
            }



            System.IO.File.Delete(@"ElectroJournal.zip");
        }

        private void MoveFiles()
        {
            /*
            System.IO.DirectoryInfo dr = new System.IO.DirectoryInfo("newVersion");
            foreach (System.IO.FileInfo fi in dr.GetFiles())
            {

                fi.CopyTo(fi.Name, true);
            }


            */

            FileSystem.CopyDirectory(@"newVersion", Directory.GetCurrentDirectory(), true);
        }

        private void DownloadFiles()
        {
            WebClient wc = new WebClient();

            string url = "http://techno-review.ru/download/net5.0-windows.zip";
            string name = "ElectroJournal.zip";

            wc.DownloadFile(url, name);
        }
    }
}
