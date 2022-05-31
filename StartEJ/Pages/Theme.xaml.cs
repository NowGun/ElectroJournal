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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace StartEJ.Pages
{
    /// <summary>
    /// Логика взаимодействия для Theme.xaml
    /// </summary>
    public partial class Theme : Page
    {
        public Theme()
        {
            InitializeComponent();
        }

        XmlDocument xmlDocument = new XmlDocument();
        string themeSelect = "0";

        private void ButtonEnd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                xmlDocument.Load("setting.xml");
                XmlNode theme = xmlDocument.GetElementsByTagName("theme")[0];
                theme.InnerText = themeSelect;
                xmlDocument.Save("setting.xml");
                Process.Start("ElectroJournal.exe");
                ((MainWindow)System.Windows.Application.Current.MainWindow).Close();
            }
            catch 
            {
                ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Приложение для запуска не найдено, переустановите приложение");
            }            
        }
        private void GridBlack_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            themeSelect = "1";
            ((MainWindow)System.Windows.Application.Current.MainWindow).theme = 1;
            ((MainWindow)System.Windows.Application.Current.MainWindow).ThemeCheck();
        }
        private void GridLight_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            themeSelect = "0";
            ((MainWindow)System.Windows.Application.Current.MainWindow).theme = 0;
            ((MainWindow)System.Windows.Application.Current.MainWindow).ThemeCheck();
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e) => ((MainWindow)System.Windows.Application.Current.MainWindow).FrameEJ.Navigate(new Pages.ConnectDB());
    }
}
