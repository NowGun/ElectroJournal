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

namespace ElectroJournal.UControl
{
    /// <summary>
    /// Логика взаимодействия для UserInfo.xaml
    /// </summary>
    public partial class UserInfo : UserControl
    {
        public UserInfo()
        {
            InitializeComponent();
        }
        Classes.Navigation nav = new();
        private void IconSetting_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            nav.NavigationPage("Setting");
            (Application.Current.MainWindow as MainWindow).NavDeselect();
        }
        private void RectangleUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            nav.NavigationPage("Users");
            (Application.Current.MainWindow as MainWindow).NavDeselect();
        }
        private void RectangleUser_MouseLeave(object sender, MouseEventArgs e) => this.Cursor = Cursors.Arrow;
        private void RectangleUser_MouseMove(object sender, MouseEventArgs e) => this.Cursor = Cursors.Hand;
        public void RefreshImage(string path)
        {
            if (!String.IsNullOrWhiteSpace(path))
            {
                var stringPath = $@"{path}";

                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.Default;
                bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                bitmapImage.DecodePixelHeight = 100;
                bitmapImage.EndInit();
                PersonPicture.ProfilePicture = bitmapImage;
            }
            else
            {
                PersonPicture.ProfilePicture = null;
                PersonPicture.DisplayName = (string)TextBlockTeacher.Content;
            }
        }
    }
}
