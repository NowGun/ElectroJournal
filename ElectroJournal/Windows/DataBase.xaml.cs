using ElectroJournal.Classes.DataBaseEJ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
            FillComboBoxUniver();
        }

        private bool _isDarkTheme = false;

        private async void LoadData()
        {
            using ejContext db = new();

            switch (Properties.Settings.Default.TypeServer)
            {
                case false:
                    RadioButtonRent.IsChecked = true;
                    TextBoxSchema.IsEnabled = false;
                    TextBoxServer.IsEnabled = false;
                    TextBoxPassword.IsEnabled = false;
                    TextBoxUser.IsEnabled = false;
                    
                    Educational? d = await db.Educationals.FirstOrDefaultAsync(d => d.NameDb == Properties.Settings.Default.DataBase);
                    if (d != null)
                    {
                        ComboBoxServer.SelectedItem = d.Name;
                    }

                    break;
                case true:
                    RadioButtonMine.IsChecked= true;
                    ComboBoxServer.IsEnabled = false;
                    TextBoxServer.Text = Properties.Settings.Default.Server;
                    TextBoxUser.Text = Properties.Settings.Default.UserName;
                    TextBoxPassword.Text = Properties.Settings.Default.Password;
                    TextBoxSchema.Text = Properties.Settings.Default.DataBase;
                    break;
            }
        }
        private async void FillComboBoxUniver()
        {
            ComboBoxServer.Items.Clear();
            using ejContext db = new();
            await db.Educationals.OrderBy(d => d.Name).ForEachAsync(d => ComboBoxServer.Items.Add(d.Name));
        }
        private async void SaveData()
        {
            try
            {
                if (RadioButtonRent.IsChecked == true)
                {
                    if (ComboBoxServer.SelectedIndex != -1)
                    {
                        using ejContext db = new();
                        Educational? d = await db.Educationals.FirstOrDefaultAsync(d => d.Name == ComboBoxServer.SelectedItem.ToString());

                        if (d != null)
                        {
                            Properties.Settings.Default.Server = "193.33.230.80";
                            Properties.Settings.Default.UserName = "Zhirov";
                            Properties.Settings.Default.Password = "64580082";
                            Properties.Settings.Default.DataBase = d.NameDb;
                            if ((bool)RadioButtonMine.IsChecked) Properties.Settings.Default.TypeServer = true;
                            else if ((bool)RadioButtonRent.IsChecked) Properties.Settings.Default.TypeServer = false;
                            ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данные изменены");
                            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
                            this.Close();
                        }
                    }
                    else ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Выберите учебное заведение");
                }
                else if (RadioButtonMine.IsChecked == true)
                {
                    if (!String.IsNullOrWhiteSpace(TextBoxSchema.Text) && !String.IsNullOrWhiteSpace(TextBoxServer.Text) && !String.IsNullOrWhiteSpace(TextBoxUser.Text) && !String.IsNullOrWhiteSpace(TextBoxPassword.Password))
                    {
                        Properties.Settings.Default.Server = TextBoxServer.Text;
                        Properties.Settings.Default.UserName = TextBoxUser.Text;
                        Properties.Settings.Default.Password = TextBoxPassword.Password;
                        Properties.Settings.Default.DataBase = TextBoxSchema.Text;
                        if ((bool)RadioButtonMine.IsChecked) Properties.Settings.Default.TypeServer = true;
                        else if ((bool)RadioButtonRent.IsChecked) Properties.Settings.Default.TypeServer = false;
                        ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данные изменены");
                        ((MainWindow)Application.Current.MainWindow).ThemeCheck();
                        this.Close();
                    }
                    else ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Заполните все поля");
                }
                Properties.Settings.Default.Save();
            }
            catch
            {

            }
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e) => SaveData();
        private void TextBoxServer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")))
            {
                e.Handled = true;
            }
        }
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var c = sender as System.Windows.Controls.RadioButton;
            switch (c.Content.ToString())
            {
                case "Пользовательский сервер":
                    TextBoxPassword.IsEnabled = true;
                    TextBoxServer.IsEnabled = true;
                    TextBoxUser.IsEnabled = true;
                    TextBoxSchema.IsEnabled = true;
                    ComboBoxServer.IsEnabled = false;
                    ComboBoxServer.SelectedIndex = -1;
                    break;

                case "Арендованный сервер":
                    TextBoxPassword.IsEnabled = false;
                    TextBoxServer.IsEnabled = false;
                    TextBoxUser.IsEnabled = false;
                    TextBoxSchema.IsEnabled = false;
                    ComboBoxServer.IsEnabled = true;
                    TextBoxSchema.Clear();
                    TextBoxPassword.Clear();
                    TextBoxServer.Clear();
                    TextBoxUser.Clear();
                    break;
            }
        }
    }
}
