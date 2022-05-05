using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

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
                ((MainWindow)Application.Current.MainWindow).ThemeCheck();
                this.Close();
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Заполните все поля");
            }
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Classes.SettingsControl s = new();
            s.ChangeTheme();
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
