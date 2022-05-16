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
        public void NavigationPage(string page)
        {
            NavigationTransitionInfo _transitionInfo = new DrillInNavigationTransitionInfo();
            
            if (!(page == "Setting" && (Application.Current.MainWindow as MainWindow).IsOpenSetting) && !(page == "Users" && (Application.Current.MainWindow as MainWindow).IsOpenUsers))
            {
                (Application.Current.MainWindow as MainWindow).IsOpenSetting = false;
                (Application.Current.MainWindow as MainWindow).IsOpenUsers = false;
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
                if (page == "Setting") (Application.Current.MainWindow as MainWindow).IsOpenSetting = true;
                else if (page == "Users") (Application.Current.MainWindow as MainWindow).IsOpenUsers = true;
            }
        }
    }
}