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
using System.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using ElectroJournal;
using System.Xml;

namespace startupEJ
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GridSetting.Visibility = Visibility.Hidden;
            GridDataBase.Visibility = Visibility.Hidden;
            //(Resources["AnimRectangle"] as Storyboard).Completed += new EventHandler(MainWindow_Completed);
            var animation = (Storyboard)FindResource("AnimRectangle");
            animation.Begin();
        }

        bool checkexit = false;

        XmlDocument xmlDocument = new XmlDocument();

        void MainWindow_Completed(object sender, EventArgs e)
        {
            (Resources["AnimRectangle"] as Storyboard).Begin();
        }

        void LanguageImage_Completed(object sender, EventArgs e)
        {
            (Resources["AnimImage"] as Storyboard).Begin();
        }

        private void ButtonAcceptLicense_Click(object sender, RoutedEventArgs e)
        {
            GridLicenses.Visibility = Visibility.Hidden;
            GridDataBase.Visibility = Visibility.Visible;
            ButtonBackAllUser.Visibility = Visibility.Hidden;
            GridAdministrator.Visibility = Visibility.Hidden;
            var animation = (Storyboard)FindResource("AnimOpenDataBase");
            animation.Begin();
        }

        private void ButtonOpenTheme_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxIPAllUser.Text) && !string.IsNullOrWhiteSpace(TextBoxLoginAllUser.Text) && !string.IsNullOrWhiteSpace(TextBoxPasswordAllUser.Text))
            {
                xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
                //xmlDocument.Load("C:/ElectroJournal/Settings.xml");
                XmlNode server = xmlDocument.GetElementsByTagName("server")[0];
                XmlNode username = xmlDocument.GetElementsByTagName("username")[0];
                XmlNode password = xmlDocument.GetElementsByTagName("password")[0];
                //string DB = "server=" + TextBoxIPAllUser.Text + "; username=" + TextBoxLoginAllUser.Text + "; password=" + TextBoxPasswordAllUser.Text + "; database=zhirov_cw";

                server.InnerText = TextBoxIPAllUser.Text;
                username.InnerText = TextBoxLoginAllUser.Text;
                password.InnerText = TextBoxPasswordAllUser.Text;
                //Properties.Settings.Default.DataBase = DB;
                //Properties.Settings.Default.Save();
                xmlDocument.Save("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");

                GridDataBase.Visibility = Visibility.Hidden;
                GridSetting.Visibility = Visibility.Visible;
                var animation = (Storyboard)FindResource("AnimOpenSetting");
                animation.Begin();
            }
            else
            {
                LabelError.Content = "Заполните поле";
                var animation = (Storyboard)FindResource("AnimError");
                animation.Begin();
            }

        }

        private void ButtonNotAcceptLicense_Click(object sender, RoutedEventArgs e)
        {
            checkexit = true;
            this.Close();
        }

        private void ButtonAdministrator_Click(object sender, RoutedEventArgs e)
        {
            ButtonAdministrator.IsEnabled = false;
            ButtonBackAllUser.Visibility = Visibility.Visible;
            GridAllUser.Visibility = Visibility.Hidden;
            GridAdministrator.Visibility = Visibility.Visible;
            var animation = (Storyboard)FindResource("AnimOpenGridAdministrator");
            animation.Begin();
        }

        private void ButtonBackAllUser_Click(object sender, RoutedEventArgs e)
        {
            ButtonAdministrator.IsEnabled = true;
            ButtonBackAllUser.Visibility = Visibility.Hidden;
            GridAdministrator.Visibility = Visibility.Hidden;
            GridAllUser.Visibility = Visibility.Visible;
            var animation = (Storyboard)FindResource("AnimOpenGridAllUser");
            animation.Begin();
        }

        bool buttoncheck = true;

        private void ImageBlackTheme_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (buttoncheck)
            {
                var animation = (Storyboard)FindResource("AnimToBlackTheme");
                animation.Begin();
                buttoncheck = false;
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!buttoncheck)
            {
                var animation = (Storyboard)FindResource("AnimToWhiteTheme");
                animation.Begin();
                buttoncheck = true;
            }
        }

        private void ButtonEnd_Click(object sender, RoutedEventArgs e)
        {
            xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //xmlDocument.Load("C:/ElectroJournal/Settings.xml");
            XmlNode xmlNode = xmlDocument.GetElementsByTagName("Theme")[0];

            if (buttoncheck == true)
            {
                xmlNode.InnerText = "White";
                //ElectroJournal.Properties.Settings.Default.Theme = "White";
                //Properties.Settings.Default.Theme = "White";
            }
            else if (buttoncheck == false)
                xmlNode.InnerText = "Black";

            //ElectroJournal.Properties.Settings.Default.Save();
            xmlDocument.Save("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            //xmlDocument.Save("C:/ElectroJournal/Settings.xml");

            checkexit = true;
            this.Close();
            Process.Start("C:/projects/ElectroJournalNetFramework/ElectroJournal/bin/Debug/ElectroJournal.exe"); //Запуск программы
            //Process.Start("C:/ElectroJournal/ElectroJournal.exe");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result;
            
            if (checkexit == false)
            {
            result = MessageBox.Show("Вы точно хотите прервать настройку?", "Закрытие StartUP", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            }
        }

        private void TextBoxIPAllUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")))
            {
                e.Handled = true;
            }
        }
    }
}
