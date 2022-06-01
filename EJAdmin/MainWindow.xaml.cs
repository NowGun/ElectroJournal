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
using WPFUI.Common;
using WPFUI.Controls.Interfaces;

namespace EJAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void RootNavigation_OnNavigated(INavigation sender, RoutedNavigationEventArgs e)
        {
            RootFrame.Margin = new Thickness(
                left: 0,
                top: e.CurrentPage.Tag as string == "Entry" ? -69 : 0,
                right: 0,
                bottom: 0);
        }

        private void NavigationButtonTheme_OnClick(object sender, RoutedEventArgs e)
        {
            // We check what theme is currently
            // active and choose its opposite.
            var newTheme = WPFUI.Appearance.Theme.GetAppTheme() == WPFUI.Appearance.ThemeType.Dark
                ? WPFUI.Appearance.ThemeType.Light
                : WPFUI.Appearance.ThemeType.Dark;

            // We apply the theme to the entire application.
            WPFUI.Appearance.Theme.Apply(
                themeType: newTheme,
                backgroundEffect: WPFUI.Appearance.BackgroundType.Mica,
                updateAccent: true,
                forceBackground: false);
        }
    }
}
