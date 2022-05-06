using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElectroJournal.Pages.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для Cabinet.xaml
    /// </summary>
    public partial class Cabinet : Page
    {
        public Cabinet()
        {
            InitializeComponent();
            FillListBoxCabinet();
            FillListBoxHousing();
        }

        List<int> idCab = new();
        List<int> idHousing = new();

        private bool changeHousing = false;

        #region Кабинеты
        private void ButonCabinetAdd_Click(object sender, RoutedEventArgs e)
        {
            ListBoxCabinet.SelectedIndex = -1;
            ListBoxHousing.SelectedIndex = -1;
            TextBoxCabinet.Clear();
            TextBoxFloor.Clear();
            TextBoxName.Clear();
            TextBoxNumSeats.Clear();
            ButtonCabinetDelete.IsEnabled = false;
        }
        private void ButtonCabinetDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteCabinet();
        }
        private async void DeleteCabinet()
        {
            if (ListBoxCabinet.Items.Count == 0) ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            else if (ListBoxCabinet.SelectedItem != null)
            {
                using zhirovContext db = new();

                DataBase.Cabinet? cab = await db.Cabinets.FirstOrDefaultAsync(c => c.Idcabinet == idCab[ListBoxCabinet.SelectedIndex]);

                if (cab != null)
                {
                    db.Cabinets.Remove(cab);
                    await db.SaveChangesAsync();

                    FillListBoxCabinet();
                    TextBoxCabinet.Clear();
                    TextBoxFloor.Clear();
                    TextBoxName.Clear();
                    TextBoxNumSeats.Clear();
                    ButtonCabinetDelete.IsEnabled = false;
                }
            }
        }
        private async void FillListBoxCabinet()
        {
            ListBoxCabinet.Items.Clear();
            idCab.Clear();

            using zhirovContext db = new();

            if (String.IsNullOrWhiteSpace(SearchBoxCabinet.Text))
            {
                await db.Cabinets.OrderBy(c => c.CabinetNumber).ForEachAsync(c =>
                {
                    ListBoxCabinet.Items.Add(c.CabinetNumber);
                    idCab.Add((int)c.Idcabinet);
                });
            }
            else
            {
                await db.Cabinets.OrderBy(c => c.CabinetNumber).Where(c => EF.Functions.Like(c.CabinetNumber, $"%{SearchBoxCabinet.Text}%")).ForEachAsync(c =>
                {
                    ListBoxCabinet.Items.Add(c.CabinetNumber);
                    idCab.Add((int)c.Idcabinet);
                });
            }

            
        }
        private async void ListBoxCabinet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCabinetDelete.IsEnabled = true;

            if (ListBoxCabinet.SelectedItem != null)
            {
                FillListBoxHousing();

                using zhirovContext db = new();

                var cab = await db.Cabinets.Include(c => c.HousingIdhousingNavigation).FirstOrDefaultAsync(c => c.Idcabinet == idCab[ListBoxCabinet.SelectedIndex]);

                TextBoxCabinet.Text = cab.CabinetNumber;
                TextBoxFloor.Text = cab.CabinetFloor;
                TextBoxName.Text = cab.CabinetName;
                TextBoxNumSeats.Text = cab.CabinetNumberSeats.ToString();
                ListBoxHousing.SelectedItem = cab.HousingIdhousingNavigation.HousingName;
            }
        }
        private async void ButtonSaveCabinet_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxName.Text) && ListBoxHousing.SelectedItem != null)
            {
                using zhirovContext db = new();

                if (ListBoxCabinet.SelectedItem != null)
                {
                    DataBase.Cabinet? cab = await db.Cabinets.Include(c => c.HousingIdhousingNavigation).FirstOrDefaultAsync(c => c.Idcabinet == idCab[ListBoxCabinet.SelectedIndex]);

                    if (cab != null)
                    {
                        cab.CabinetName = TextBoxName.Text;
                        cab.CabinetNumberSeats = Int32.Parse(TextBoxNumSeats.Text);
                        cab.CabinetFloor = TextBoxFloor.Text;
                        cab.CabinetNumber = TextBoxCabinet.Text;
                        cab.HousingIdhousing = (uint)idHousing[ListBoxHousing.SelectedIndex];

                        await db.SaveChangesAsync();
                        
                        ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                    }
                }
                else
                {
                    DataBase.Cabinet cab = new()
                    {
                        CabinetName = TextBoxName.Text,
                        CabinetNumberSeats= Int32.Parse(TextBoxNumSeats.Text),
                        CabinetFloor= TextBoxFloor.Text,
                        CabinetNumber= TextBoxCabinet.Text,
                        HousingIdhousing = (uint)idHousing[ListBoxHousing.SelectedIndex]
                    };

                    await db.Cabinets.AddAsync(cab);
                    await db.SaveChangesAsync();

                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                }

                FillListBoxCabinet();
            }
            else
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");
            }
        }
        private void SearchBoxCabinet_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            FillListBoxCabinet();
        }
        #endregion

        #region Корпуса
        private void IconAddHousing_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            changeHousing = false;
            RootDialog.Show();
        }
        private void RootDialog_ButtonRightClick(object sender, RoutedEventArgs e)
        {
            RootDialog.Hide();
        }
        private async void RootDialog_ButtonLeftClick(object sender, RoutedEventArgs e)
        {
            using zhirovContext db = new();

            if (!changeHousing)
            {
                Housing housing = new()
                {
                    HousingName = TextBoxHousingName.Text,
                    HousingAddress = TextBoxHousingAddress.Text,
                    HousingCountCabinet = Int32.Parse(NumberBoxCountCabinet.Text)
                };
                await db.Housings.AddAsync(housing);
                await db.SaveChangesAsync();
            }
            else
            {
                Housing? housing = await db.Housings.FirstOrDefaultAsync(h => h.Idhousing == idHousing[ListBoxHousing.SelectedIndex]);

                if (housing != null)
                {
                    housing.HousingName = TextBoxHousingName.Text;
                    housing.HousingAddress = TextBoxHousingAddress.Text;
                    housing.HousingCountCabinet = Int32.Parse(NumberBoxCountCabinet.Text);

                    await db.SaveChangesAsync();
                }
            }
            FillListBoxHousing();
            RootDialog.Hide();
        }
        private async void IconDeleteHousing_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListBoxHousing.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListBoxHousing.SelectedItem != null)
            {
                using zhirovContext db = new();
                Housing? housing = await db.Housings.FirstOrDefaultAsync(h => h.Idhousing == idHousing[ListBoxHousing.SelectedIndex]);

                if (housing == null)
                {
                    db.Housings.Remove(housing);
                    await db.SaveChangesAsync();

                    FillListBoxHousing();
                }
            }
        }
        private async void IconChangeHousing_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListBoxHousing.SelectedIndex != -1)
            {
                changeHousing = true;

                using zhirovContext db = new();

                var h = await db.Housings.FirstOrDefaultAsync(h => h.Idhousing == idHousing[ListBoxHousing.SelectedIndex]);

                TextBoxHousingName.Text = h.HousingName;
                TextBoxHousingAddress.Text = h.HousingAddress;
                NumberBoxCountCabinet.Text = h.HousingCountCabinet.ToString();

                RootDialog.Show();
            }
        }
        private async void FillListBoxHousing()
        {
            ListBoxHousing.Items.Clear();
            idHousing.Clear();

            using zhirovContext db = new();

            await db.Housings.OrderBy(h => h.HousingName).ForEachAsync(h =>
            {
                ListBoxHousing.Items.Add(h.HousingName);
                idHousing.Add((int)h.Idhousing);
            });
        }
        #endregion 
    }
}