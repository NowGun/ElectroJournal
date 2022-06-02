using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Notifications.Wpf;

namespace StartEJ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameEJ.Navigate(new Pages.License());
            CreateFile();
        }

        public int theme = 0;
        private bool _isDarkTheme = false;
        private readonly NotificationManager _notificationManager = new NotificationManager();

        public void ThemeCheck()
        {
            _isDarkTheme = theme == 1;

            var newTheme = theme == 0
            ? WPFUI.Appearance.ThemeType.Light
            : WPFUI.Appearance.ThemeType.Dark;

            WPFUI.Appearance.Theme.Apply(
           themeType: newTheme,
           backgroundEffect: WPFUI.Appearance.BackgroundType.Mica,
           updateAccent: true,
           forceBackground: false);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }
        private void CreateFile()
        {
            using (XmlWriter writer = XmlWriter.Create("setting.xml")) 
            {
                writer.WriteStartElement("root");
                writer.WriteElementString("server", "");
                writer.WriteElementString("username", "");
                writer.WriteElementString("password", "");
                writer.WriteElementString("database", "");
                writer.WriteElementString("TypeServer", "");
                writer.WriteElementString("theme", "");
                writer.WriteEndElement();
                writer.Flush();
            };
        }
        public void Notifications(string title, string text)
        {
            _notificationManager.Show(
                new NotificationContent { Title = title, Message = text, Type = NotificationType.Information },
                areaName: "WindowArea");
        }
    }
}
