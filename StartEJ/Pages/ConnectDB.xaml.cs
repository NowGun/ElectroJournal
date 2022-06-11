using ElectroJournal.Classes.DataBaseEJ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Xml;

namespace StartEJ.Pages
{
    /// <summary>
    /// Логика взаимодействия для ConnectDB.xaml
    /// </summary>
    public partial class ConnectDB : Page
    {
        public ConnectDB()
        {
            InitializeComponent();
            FillComboBoxUniver();
        }

        XmlDocument xmlDocument = new XmlDocument();
        private void TextBoxIP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")))
            {
                e.Handled = true;
            }
        }
        private async void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                xmlDocument.Load("setting.xml");

                XmlNode server = xmlDocument.GetElementsByTagName("server")[0];
                XmlNode username = xmlDocument.GetElementsByTagName("username")[0];
                XmlNode password = xmlDocument.GetElementsByTagName("password")[0];
                XmlNode database = xmlDocument.GetElementsByTagName("database")[0];
                XmlNode typeserver = xmlDocument.GetElementsByTagName("TypeServer")[0];

                if (RadioButtonRent.IsChecked == true)
                {
                    if (ComboBoxServer.SelectedIndex != -1)
                    {
                        using ejContext db = new();
                        Educational? d = await db.Educationals.FirstOrDefaultAsync(d => d.Name == ComboBoxServer.SelectedItem.ToString());

                        if (d != null)
                        {
                            server.InnerText = "193.33.230.80";
                            username.InnerText = "Zhirov";
                            password.InnerText = "64580082";
                            database.InnerText = d.NameDb;
                            if ((bool)RadioButtonMine.IsChecked) typeserver.InnerText = "true";
                            else if ((bool)RadioButtonRent.IsChecked) typeserver.InnerText = "false";

                            ((MainWindow)Application.Current.MainWindow).FrameEJ.Navigate(new Theme());
                        }
                    }
                    else ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Выберите учебное заведение");
                }
                else if (RadioButtonMine.IsChecked == true)
                {
                    if (!String.IsNullOrWhiteSpace(TextBoxDB.Text) && !string.IsNullOrWhiteSpace(TextBoxIP.Text) && !string.IsNullOrWhiteSpace(TextBoxLogin.Text) && !string.IsNullOrWhiteSpace(TextBoxPassword.Text))
                    {
                        server.InnerText = TextBoxIP.Text;
                        username.InnerText = TextBoxLogin.Text;
                        password.InnerText = TextBoxPassword.Text;
                        database.InnerText = TextBoxDB.Text;

                        if ((bool)RadioButtonMine.IsChecked) typeserver.InnerText = "true";
                        else if ((bool)RadioButtonRent.IsChecked) typeserver.InnerText = "false";

                        ((MainWindow)Application.Current.MainWindow).FrameEJ.Navigate(new Theme());
                    }
                    else ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Заполните все поля");
                }
                xmlDocument.Save("setting.xml");
            }
            catch
            {

            }
        }
        private async void FillComboBoxUniver()
        {
            ComboBoxServer.Items.Clear();
            using ejContext db = new();
            if (await db.Database.CanConnectAsync()) 
                await db.Educationals.OrderBy(d => d.Name).ForEachAsync(d => ComboBoxServer.Items.Add(d.Name));
            else
            {
                ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Аренда сервера в данный момент недоступна");
                RadioButtonMine.IsChecked = true;
            }
        }
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var c = sender as RadioButton;
            switch (c.Content.ToString())
            {
                case "Пользовательский сервер":
                    TextBoxPassword.IsEnabled = true;
                    TextBoxIP.IsEnabled = true;
                    TextBoxLogin.IsEnabled = true;
                    TextBoxDB.IsEnabled = true;
                    ComboBoxServer.IsEnabled = false;
                    ComboBoxServer.SelectedIndex = -1;
                    break;

                case "Арендованный сервер":
                    TextBoxPassword.IsEnabled = false;
                    TextBoxDB.IsEnabled = false;
                    TextBoxLogin.IsEnabled = false;
                    TextBoxIP.IsEnabled = false;
                    ComboBoxServer.IsEnabled = true;
                    TextBoxDB.Clear();
                    TextBoxPassword.Clear();
                    TextBoxLogin.Clear();
                    TextBoxIP.Clear();
                    break;
            }
        }
        private void ButtonAdmin_Click(object sender, RoutedEventArgs e) => ((MainWindow)Application.Current.MainWindow).FrameEJ.Navigate(new Admin());
    }
}
