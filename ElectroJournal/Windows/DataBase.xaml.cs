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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для DataBase.xaml
    /// </summary>
    public partial class DBUser : Window
    {
        public DBUser()
        {
            InitializeComponent();
            LoadData();
        }

        XmlDocument xmlDocument = new XmlDocument();

        private void LoadData()
        {
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //string server = xmlDocument.GetElementsByTagName("server")[0].InnerText;
            //string user = xmlDocument.GetElementsByTagName("username")[0].InnerText;
            //string password = xmlDocument.GetElementsByTagName("password")[0].InnerText;

            string server = Properties.Settings.Default.Server;
            string user = Properties.Settings.Default.UserName;
            string password = Properties.Settings.Default.Password;

            TextBoxServer.Text = server;
            TextBoxUser.Text = user;
            TextBoxPassword.Text = password;
        }

        private void SaveData()
        {
            //xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");

            // XmlNode server = xmlDocument.GetElementsByTagName("server")[0];
            // XmlNode user = xmlDocument.GetElementsByTagName("username")[0];
            // XmlNode password = xmlDocument.GetElementsByTagName("password")[0];



            Properties.Settings.Default.Server = TextBoxServer.Text;
            Properties.Settings.Default.UserName = TextBoxUser.Text;
            Properties.Settings.Default.Password = TextBoxPassword.Text;

            Properties.Settings.Default.Save();
            //xmlDocument.Save("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimLabelNotyfication");
            SaveData();
            anim.Begin();
        }
    }
}
