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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для DataBase.xaml
    /// </summary>
    public partial class DBUser : Window
    {
        public DBUser()
        {
            InitializeComponent();
            LoadData();
            TitleBar.CloseActionOverride = CloseActionOverride;
        }

        private bool _isDarkTheme = false;

        private void LoadData()
        {
            TextBoxServer.Text = Properties.Settings.Default.Server;
            TextBoxUser.Text = Properties.Settings.Default.UserName;
            TextBoxPassword.Password = Properties.Settings.Default.Password;
        }

        private void SaveData()
        {
            if (!String.IsNullOrWhiteSpace(TextBoxServer.Text) && !String.IsNullOrWhiteSpace(TextBoxUser.Text) && !String.IsNullOrWhiteSpace(TextBoxPassword.Password))
            {
                Properties.Settings.Default.Server = TextBoxServer.Text;
                Properties.Settings.Default.UserName = TextBoxUser.Text;
                Properties.Settings.Default.Password = TextBoxPassword.Password;

                Properties.Settings.Default.Save();

                ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данные изменены");
                this.Close();
            }
            else ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Заполните все поля");
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }

        public void ThemeCheck()
        {
            int theme = Properties.Settings.Default.Theme;

            _isDarkTheme = theme == 1;
            WPFUI.Theme.Manager.Switch(theme == 1 ? WPFUI.Theme.Style.Dark : WPFUI.Theme.Style.Light);

            ApplyBackgroundEffect();
        }

        private void ApplyBackgroundEffect()
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            WPFUI.Background.Manager.Remove(windowHandle);

            if (_isDarkTheme)
            {
                WPFUI.Background.Manager.ApplyDarkMode(windowHandle);
            }
            else
            {
                WPFUI.Background.Manager.RemoveDarkMode(windowHandle);
            }

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                this.Background = System.Windows.Media.Brushes.Transparent;
                WPFUI.Background.Manager.Apply(WPFUI.Background.BackgroundType.Mica, windowHandle);
            }
        }
        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }

        private void TextBoxServer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")))
            {
                e.Handled = true;
            }
        }
    }
}
