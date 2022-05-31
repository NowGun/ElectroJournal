using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace StartEJ.Pages
{
    /// <summary>
    /// Логика взаимодействия для ConnectDB.xaml
    /// </summary>
    public partial class ConnectDB : Page
    {
        public ConnectDB()
        {
            InitializeComponent();
        }

        XmlDocument xmlDocument = new XmlDocument();

        private void TextBoxIP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")))
            {
                e.Handled = true;
            }
        }
        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxIP.Text) && !string.IsNullOrWhiteSpace(TextBoxLogin.Text) && !string.IsNullOrWhiteSpace(TextBoxPassword.Text))
            {
                xmlDocument.Load("setting.xml");

                XmlNode server = xmlDocument.GetElementsByTagName("server")[0];
                XmlNode username = xmlDocument.GetElementsByTagName("username")[0];
                XmlNode password = xmlDocument.GetElementsByTagName("password")[0];

                server.InnerText = TextBoxIP.Text;
                username.InnerText = TextBoxLogin.Text;
                password.InnerText = TextBoxPassword.Text;

                xmlDocument.Save("setting.xml");

                ((MainWindow)Application.Current.MainWindow).FrameEJ.Navigate(new Theme());
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Заполните все поля");
            } 
        }
        private void ButtonAdmin_Click(object sender, RoutedEventArgs e) => ((MainWindow)Application.Current.MainWindow).FrameEJ.Navigate(new Admin());
    }
}
