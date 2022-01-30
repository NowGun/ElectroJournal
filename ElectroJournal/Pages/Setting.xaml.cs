using ElectroJournal.Windows;
using System;
using System.Collections.Generic;
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
            LoadSave();
        }

        XmlDocument xmlDocument = new XmlDocument();

        private void LoadSave ()
        {
            LoadRun();
            LoadBD();
            LoadApp();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {          
            SaveApp();
            SaveRun();
            ((MainWindow)System.Windows.Application.Current.MainWindow).CheckAutoRun();
            ((MainWindow)System.Windows.Application.Current.MainWindow).ThemeCheck();
            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные успешно сохранены");
        }

        private void LoadBD()
        {
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //string server = xmlDocument.GetElementsByTagName("server")[0].InnerText;

            string server = Properties.Settings.Default.Server;
            LabelIpAddress.Content = server;
        }

        private void LoadApp()
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                ComboBoxTheme.Visibility = Visibility.Hidden;
            }
            int theme = Properties.Settings.Default.Theme;
            bool animation = Properties.Settings.Default.Animation;

            ComboBoxTheme.SelectedIndex = theme;

            CheckBoxAnim.IsChecked = animation;
        }

        private void LoadRun()
        {
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //string autorun = xmlDocument.GetElementsByTagName("autorun")[0].InnerText;
            //string collapsetotray = xmlDocument.GetElementsByTagName("collapsetotray")[0].InnerText;
            
            bool autorun = Properties.Settings.Default.AutoRun;
            bool tray = Properties.Settings.Default.Tray;

            CheckBoxAutoRun.IsChecked = autorun;
            CheckBoxCollapseToTray.IsChecked = tray;
        }

        private void SaveApp()
        {
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //XmlNode theme = xmlDocument.GetElementsByTagName("Theme")[0];
            //XmlNode animation = xmlDocument.GetElementsByTagName("animation")[0];

            Properties.Settings.Default.Theme = ComboBoxTheme.SelectedIndex;


            Properties.Settings.Default.Animation = CheckBoxAnim.IsChecked ?? true;

            Properties.Settings.Default.Save();
            //xmlDocument.Save("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
        }

        private void SaveRun()
        {
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //XmlNode autorun = xmlDocument.GetElementsByTagName("autorun")[0];
            //XmlNode collapsetotray = xmlDocument.GetElementsByTagName("collapsetotray")[0];



            Properties.Settings.Default.AutoRun = CheckBoxAutoRun.IsChecked ?? false;
            Properties.Settings.Default.Tray = CheckBoxCollapseToTray.IsChecked ?? false;

            Properties.Settings.Default.Save();
            //xmlDocument.Save("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
        }

        private void ButtonChangeBD_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            new DBUser().ShowDialog();
        }
    }
}
