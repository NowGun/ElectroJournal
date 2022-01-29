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
            xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            string server = xmlDocument.GetElementsByTagName("server")[0].InnerText;
            string user = xmlDocument.GetElementsByTagName("username")[0].InnerText;
            string password = xmlDocument.GetElementsByTagName("password")[0].InnerText;

            TextBoxServer.Text = server;
            TextBoxUser.Text = user;
            TextBoxPassword.Text = password;
        }

        private void SaveData()
        {
            xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");

            XmlNode server = xmlDocument.GetElementsByTagName("server")[0];
            XmlNode user = xmlDocument.GetElementsByTagName("username")[0];
            XmlNode password = xmlDocument.GetElementsByTagName("password")[0];

            server.InnerText = TextBoxServer.Text;
            user.InnerText = TextBoxUser.Text;
            password.InnerText = TextBoxPassword.Text;

            xmlDocument.Save("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var anim = (Storyboard)FindResource("AnimLabelNotyfication");
            SaveData();
            anim.Begin();
        }
    }
}
