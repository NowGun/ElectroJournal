using System;
using System.Windows;
using System.Windows.Interop;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProgramInformation.xaml
    /// </summary>
    public partial class ProgramInformation : Window
    {
        public ProgramInformation()
        {
            InitializeComponent();
        }

        private bool _isDarkTheme = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Classes.SettingsControl s = new();
            s.ChangeTheme();
        } 
        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }
    }
}
