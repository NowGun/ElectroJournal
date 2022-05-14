using ModernWpf.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElectroJournal.Classes
{
    public class Navigation
    {
        private NavigationTransitionInfo _transitionInfo = new DrillInNavigationTransitionInfo();
        public void NavigationPage(string page)
        {
            if (Environment.OSVersion.Version.Build < 1903)
            {
                string pageName = $"ElectroJournal.Pages.{page}";
                Type? pageType = typeof(Pages.Journal).Assembly.GetType(pageName);
                (Application.Current.MainWindow as MainWindow)?.Frame.Navigate(pageType);
            }
            else
            {
                string pageName = $"ElectroJournal.Pages.{page}";
                Type? pageType = typeof(Pages.Journal).Assembly.GetType(pageName);
                (Application.Current.MainWindow as MainWindow)?.Frame.Navigate(pageType, null, _transitionInfo);
            }
        }
        
    }
}
