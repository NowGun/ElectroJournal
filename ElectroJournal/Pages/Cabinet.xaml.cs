using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElectroJournal.Pages
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
            FillComboBoxHousing();
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
        private void ButtonCabinetDelete_Click(object sender, RoutedEventArgs e) => DeleteCabinet();
        private async void DeleteCabinet()
        {
            try
            {
                if (ListBoxCabinet.Items.Count == 0) ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
                else if (ListBoxCabinet.SelectedItem != null)
                {
                    using zhirovContext db = new();

                    Classes.DataBaseEF.Cabinet? cab = await db.Cabinets.FirstOrDefaultAsync(c => c.Idcabinet == idCab[ListBoxCabinet.SelectedIndex]);

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
            catch (Exception ex)
            {
                SettingsControl.InputLog($"DeleteCabinet | {ex.Message}");
            }
        }
        private async void FillListBoxCabinet()
        {
            try
            {
                ListBoxCabinet.Items.Clear();
                idCab.Clear();

                using zhirovContext db = new();

                if (ComboBoxSortingCabinet.SelectedItem != null)
                {
                    if (String.IsNullOrWhiteSpace(SearchBoxCabinet.Text))
                    {
                        await db.Cabinets.OrderBy(c => c.CabinetNumber).Where(c => c.HousingIdhousingNavigation.HousingName == ComboBoxSortingCabinet.SelectedItem.ToString()).ForEachAsync(c =>
                        {
                            ListBoxCabinet.Items.Add(c.CabinetNumber);
                            idCab.Add((int)c.Idcabinet);
                        });
                    }
                    else
                    {
                        await db.Cabinets.OrderBy(c => c.CabinetNumber).Where(c => EF.Functions.Like(c.CabinetNumber, $"%{SearchBoxCabinet.Text}%") && c.HousingIdhousingNavigation.HousingName == ComboBoxSortingCabinet.SelectedItem.ToString()).ForEachAsync(c =>
                        {
                            ListBoxCabinet.Items.Add(c.CabinetNumber);
                            idCab.Add((int)c.Idcabinet);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBoxCabinet | {ex.Message}");
            }
        }
        private async void FillComboBoxHousing()
        {
            try
            {
                ComboBoxSortingCabinet.Items.Clear();

                using zhirovContext db = new();
                await db.Housings.OrderBy(t => t.HousingName).ForEachAsync(t =>
                {
                    ComboBoxSortingCabinet.Items.Add(t.HousingName);
                });
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBoxHousing | {ex.Message}");
            }
        }
        private async void ListBoxCabinet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxCabinet_SelectionChanged | {ex.Message}");
            }
        }
        private async void ButtonSaveCabinet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TextBoxCabinet.Text) && ListBoxHousing.SelectedItem != null)
                {
                    using zhirovContext db = new();
                    int a;
                    if (String.IsNullOrWhiteSpace(TextBoxNumSeats.Text)) a = 0;
                    else a = Int32.Parse(TextBoxNumSeats.Text);

                    if (ListBoxCabinet.SelectedItem != null)
                    {
                        Classes.DataBaseEF.Cabinet? cab = await db.Cabinets.Include(c => c.HousingIdhousingNavigation).FirstOrDefaultAsync(c => c.Idcabinet == idCab[ListBoxCabinet.SelectedIndex]);

                        if (cab != null)
                        {
                            cab.CabinetName = TextBoxName.Text;
                            cab.CabinetNumberSeats = a;
                            cab.CabinetFloor = TextBoxFloor.Text;
                            cab.CabinetNumber = TextBoxCabinet.Text;
                            cab.HousingIdhousing = (uint)idHousing[ListBoxHousing.SelectedIndex];

                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        Classes.DataBaseEF.Cabinet cab = new()
                        {
                            CabinetName = TextBoxName.Text,
                            CabinetNumberSeats = a,
                            CabinetFloor = TextBoxFloor.Text,
                            CabinetNumber = TextBoxCabinet.Text,
                            HousingIdhousing = (uint)idHousing[ListBoxHousing.SelectedIndex]
                        };

                        await db.Cabinets.AddAsync(cab);
                        await db.SaveChangesAsync();
                    }

                    TextBoxCabinet.Clear();
                    TextBoxFloor.Clear();
                    TextBoxName.Clear();
                    TextBoxNumSeats.Clear();
                    ListBoxHousing.SelectedIndex = -1;

                    FillListBoxCabinet();
                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonSaveCabinet_Click | {ex.Message}");
            }
        }
        private void SearchBoxCabinet_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) => FillListBoxCabinet();
        private void ComboBoxSortingCabinet_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillListBoxCabinet();
        #endregion

        #region Корпуса
        private void IconAddHousing_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            changeHousing = false;
            RootDialog.Show();
        }
        private void RootDialog_ButtonRightClick(object sender, RoutedEventArgs e) => RootDialog.Hide();
        private async void RootDialog_ButtonLeftClick(object sender, RoutedEventArgs e)
        {
            try
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
                FillComboBoxHousing();
                RootDialog.Hide();
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"RootDialog_ButtonLeftClick | {ex.Message}");
            }
        }
        private async void IconDeleteHousing_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (ListBoxHousing.Items.Count == 0)
                {
                    ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
                }
                else if (ListBoxHousing.SelectedItem != null)
                {
                    using zhirovContext db = new();
                    Housing? housing = await db.Housings.FirstOrDefaultAsync(h => h.Idhousing == idHousing[ListBoxHousing.SelectedIndex]);

                    if (housing != null)
                    {
                        db.Housings.Remove(housing);
                        await db.SaveChangesAsync();

                        FillListBoxHousing();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"IconDeleteHousing_PreviewMouseLeftButtonUp | {ex.Message}");
            }
        }
        private async void IconChangeHousing_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                SettingsControl.InputLog($"IconChangeHousing_PreviewMouseLeftButtonUp | {ex.Message}");
            }
        }
        private async void FillListBoxHousing()
        {
            try
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
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBoxHousing | {ex.Message}");
            }
        }
        #endregion
    }
}