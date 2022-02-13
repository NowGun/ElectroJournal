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

namespace StartEJ.Pages
{
    /// <summary>
    /// Логика взаимодействия для License.xaml
    /// </summary>
    public partial class License : Page
    {
        public License()
        {
            InitializeComponent();
        }

        private void ButtonNAccept_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).Close();
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).FrameEJ.Navigate(new Pages.ConnectDB());
        }
    }
}
