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
            /*ButtonSaveCabinet.IsEnabled = false;
            ButtonSaveHousing.IsEnabled = false;
            ButtonDeleteCabinet.IsEnabled = false;
            ButtonDeleteHousing.IsEnabled = false;
            TextBoxCabinet.IsEnabled = false;
            TextBoxEntryFloor.IsEnabled = false;
            TextBoxNameHousing.IsEnabled = false;
            TextBoxAddressHousing.IsEnabled = false;
            TextBoxQuantityHousing.IsEnabled = false;
            ComboBoxSelectHousing.IsEnabled = false;
            ComboBoxSelectHousingRefresh();
            ListBoxCabinetRefresh();
            ListBoxHousingRefresh();*/
            FillListBoxCabinet();
        }

        List<int> idCab = new();

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

            await db.Cabinets.OrderBy(c => c.CabinetNumber).ForEachAsync(c =>
            {
                ListBoxCabinet.Items.Add(c.CabinetNumber);
                idCab.Add((int)c.Idcabinet);
            });
        }
        private async void FillListBoxHousing()
        {
            ListBoxHousing.Items.Clear();

            using zhirovContext db = new();

            await db.Housings.OrderBy(h => h.HousingName).ForEachAsync(h =>
            {
                ListBoxHousing.Items.Add(h.HousingName);

            });
        }
        private async void ListBoxCabinet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCabinetDelete.IsEnabled = true;

            if (ListBoxCabinet.SelectedItem != null)
            {
                FillListBoxHousing();

                using zhirovContext db = new();

                var cab = await db.Cabinets.FirstOrDefaultAsync(c => c.Idcabinet == idCab[ListBoxCabinet.SelectedIndex]);

                TextBoxCabinet.Text = cab.CabinetNumber;
                TextBoxFloor.Text = cab.CabinetFloor;
                TextBoxName.Text = cab.CabinetName;
                TextBoxNumSeats.Text = cab.CabinetNumberSeats.ToString();
            }
        }

        /* DataBaseConn DbUser = new DataBaseConn();
         DataBaseControls DbControls = new DataBaseControls();
         MainWindow Not = new MainWindow();
         MySqlConnection conn = DataBaseConn.GetDBConnection();

         private void ButtonSaveCabinet_Click(object sender, RoutedEventArgs e)
         {
             if (TextBoxCabinet.Text == string.Empty)
             {
                 Not.Notifications("Ошибка", "Напишите название");
             }
             else if (ListBoxCabinet.SelectedItem != null)
             {
                 MySqlCommand command = new MySqlCommand("UPDATE `cabinet` SET `cabinet_floor` = @floor, `cabinet_number` = @number, `cabinet_features` = @features, `cabinet_number_seats` = @seats," +
                     "`cabinet_name` = @name, `housing_idhousing` = (SELECT idhousing FROM housing WHERE `housing_name` = @housing) WHERE `idcabinet` = @id", conn);

                 command.Parameters.Add("@number", MySqlDbType.VarChar).Value = TextBoxCabinet.Text;
                 command.Parameters.Add("@floor", MySqlDbType.VarChar).Value = TextBoxEntryFloor.Text;
                 command.Parameters.Add("@features", MySqlDbType.VarChar).Value = TextBoxFeatures.Text;
                 command.Parameters.Add("@seats", MySqlDbType.VarChar).Value = TextBoxNumberSeats.Text;
                 command.Parameters.Add("@name", MySqlDbType.VarChar).Value = TextBoxName.Text;
                 command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Properties.Settings.Default.IdCabinet;
                 command.Parameters.Add("@housing", MySqlDbType.VarChar).Value = ComboBoxSelectHousing.SelectedItem.ToString();

                 conn.Open();

                 if (command.ExecuteNonQuery() == 1)
                 {
                     conn.Close();
                     ListBoxCabinetRefresh();
                     Not.Notifications("Уведомление", "Сохранено");
                 }
                 else
                 {
                     Not.Notifications("Ошибка", "Произошла ошибка");
                 }
             }
             else
             {
                 MySqlCommand command = new MySqlCommand("INSERT INTO cabinet (cabinet_number, cabinet_floor, cabinet_features, cabinet_number_seats, cabinet_name, housing_idhousing) VALUES " +
                     "(@number, @floor, @features, @seats, @name, (SELECT idhousing FROM housing WHERE housing_name = @housing))", conn);

                 command.Parameters.Add("@number", MySqlDbType.VarChar).Value = TextBoxCabinet.Text;
                 command.Parameters.Add("@floor", MySqlDbType.VarChar).Value = TextBoxEntryFloor.Text;
                 command.Parameters.Add("@features", MySqlDbType.VarChar).Value = TextBoxFeatures.Text;
                 command.Parameters.Add("@seats", MySqlDbType.VarChar).Value = TextBoxNumberSeats.Text;
                 command.Parameters.Add("@name", MySqlDbType.VarChar).Value = TextBoxName.Text;
                 command.Parameters.Add("@housing", MySqlDbType.VarChar).Value = ComboBoxSelectHousing.SelectedItem.ToString();


                 bool a = DbControls.IsCabinetExists(TextBoxCabinet.Text);

                 if (a == false)
                 {
                     conn.Open();

                     if (command.ExecuteNonQuery() == 1)
                     {
                         conn.Close();
                         ListBoxCabinetRefresh();
                         Not.Notifications("Уведомление", "Сохранено");
                     }
                     else
                     {
                         Not.Notifications("Ошибка", "Произошла ошибка");
                     }
                 }
                 else
                 {
                     Not.Notifications("Ошибка", "Произошла ошибка");
                 }
             }

         }

         private void ButtonSaveHousing_Click(object sender, RoutedEventArgs e)
         {
             if (TextBoxNameHousing.Text == string.Empty)
             {
                 Not.Notifications("Ошибка", "Напишите название");
             }
             else if (ListBoxHousing.SelectedItem != null)
             {
                 MySqlCommand command = new MySqlCommand("UPDATE `housing` SET `housing_name` = @name, `housing_address` = @address, `housing_count_cabinet` = @numberAudiences " +
                     "WHERE `idhousing` = @id", conn);

                 command.Parameters.Add("@name", MySqlDbType.VarChar).Value = TextBoxNameHousing.Text;
                 command.Parameters.Add("@address", MySqlDbType.VarChar).Value = TextBoxAddressHousing.Text;
                 command.Parameters.Add("@numberAudiences", MySqlDbType.VarChar).Value = TextBoxQuantityHousing.Text;
                 command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Properties.Settings.Default.IdHousing;

                 conn.Open();
                 if (command.ExecuteNonQuery() == 1)
                 {
                     conn.Close();
                     ListBoxHousingRefresh();
                     ComboBoxSelectHousingRefresh();
                     Not.Notifications("Уведомление", "Сохранено");
                 }
                 else
                 {
                     Not.Notifications("Ошибка", "Произошла ошибка");
                 }
             }
             else
             {
                 MySqlCommand command = new MySqlCommand("INSERT INTO `housing` (`housing_name`, `housing_address`, `housing_count_cabinet`) VALUES (@name, @address, @numberAudiences);", conn);

                 command.Parameters.Add("@name", MySqlDbType.VarChar).Value = TextBoxNameHousing.Text;
                 command.Parameters.Add("@address", MySqlDbType.VarChar).Value = TextBoxAddressHousing.Text;
                 command.Parameters.Add("@numberAudiences", MySqlDbType.VarChar).Value = TextBoxQuantityHousing.Text;

                 bool a = DbControls.IsHousingExists(TextBoxNameHousing.Text);

                 if (a == false)
                 {
                     conn.Open();

                     if (command.ExecuteNonQuery() == 1)
                     {
                         conn.Close();
                         ListBoxHousingRefresh();
                         ComboBoxSelectHousingRefresh();
                         Not.Notifications("Уведомление", "Сохранено");
                     }
                     else
                     {
                         Not.Notifications("Ошибка", "Произошла ошибка");
                     }
                 }
                 //ButtonDeleteTest.IsEnabled = true;
                 //GridNotificationsAnim("Сохранение успешно завершено");
             }
         }

         private void ListBoxHousing_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             TextBoxNameHousing.IsEnabled = true;
             TextBoxAddressHousing.IsEnabled = true;
             TextBoxQuantityHousing.IsEnabled = true;
             ButtonSaveHousing.IsEnabled = true;
             ButtonDeleteHousing.IsEnabled = true;
             TextBoxNameHousing.IsEnabled = false;
             TextBoxAddressHousing.Clear();
             TextBoxQuantityHousing.Clear();
             if (ListBoxHousing.SelectedItem != null) //если строка выделена выполняется условие
             {
                 MySqlCommand command = new MySqlCommand("SELECT `housing_name`, `housing_address`, `housing_count_cabinet` FROM `housing`", conn); //Команда выбора данных
                 conn.Open(); //Открываем соединение
                 MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные

                 while (read.Read()) //Читаем пока есть данные
                 {
                     if (ListBoxHousing.SelectedItem.ToString() == read.GetString(0))
                     {
                         TextBoxNameHousing.Text = read.GetString(0);
                         TextBoxAddressHousing.Text = read.GetString(1);
                         TextBoxQuantityHousing.Text = read.GetString(2);
                         break;
                     }
                 }
                 conn.Close(); //Закрываем соединение
             }
         }

         private void ListBoxCabinet_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             Properties.Settings.Default.IdCabinet = Int32.Parse(ListBoxCabinet.SelectedItem.ToString().Split().First());
             Properties.Settings.Default.Save();

             TextBoxCabinet.IsEnabled = true;
             TextBoxEntryFloor.IsEnabled = true;
             ComboBoxSelectHousing.IsEnabled = true;
             ButtonSaveCabinet.IsEnabled = true;
             ButtonDeleteCabinet.IsEnabled = true;
             TextBoxCabinet.Clear();
             TextBoxEntryFloor.Clear();
             ComboBoxSelectHousing.SelectedItem = null;
             if (ListBoxCabinet.SelectedItem != null) //если строка выделена выполняется условие
             {
                 MySqlCommand command = new MySqlCommand("SELECT * FROM requestcabinet", conn); //Команда выбора данных

                 command.Parameters.Add("@id", MySqlDbType.Int32).Value = Properties.Settings.Default.IdCabinet;

                 conn.Open(); //Открываем соединение
                 MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные

                 while (read.Read()) //Читаем пока есть данные
                 {
                     if (Properties.Settings.Default.IdCabinet == read.GetInt32(1))
                     {
                         TextBoxCabinet.Text = read.GetString(1);
                         TextBoxEntryFloor.Text = read.GetString(2);
                         TextBoxFeatures.Text = read.GetString(3);
                         TextBoxNumberSeats.Text = read.GetString(4);
                         TextBoxName.Text = read.GetString(5);
                         ComboBoxSelectHousing.SelectedItem = read.GetString(6);
                         //Properties.Settings.Default.IdCabinet = read.GetInt32(0);
                         break;
                     }

                 }
                 Not.Notifications("Проверка", "Уведомления работают");
                 conn.Close(); //Закрываем соединение
             }
         }

         private void ButtonDeleteHousing_Click(object sender, RoutedEventArgs e)
         {
             if (ListBoxHousing.Items.Count == 0)
             {
                 Not.Notifications("Ошибка", "Произошла ошибка");
             }
             else if (ListBoxHousing.SelectedItem != null)
             {
                 string NameHousing = ListBoxHousing.SelectedItem.ToString();
                 DbControls.DeleteHousing(NameHousing);
                 ListBoxHousing.Items.Clear();
                 TextBoxNameHousing.Clear();
                 TextBoxAddressHousing.Clear();
                 TextBoxQuantityHousing.Clear();
                 TextBoxNameHousing.IsEnabled = true;
                 ListBoxHousingRefresh();
                 ButtonDeleteHousing.IsEnabled = false;
             }
         }

         private void ButtonCreateHousing_Click(object sender, RoutedEventArgs e)
         {
             ListBoxHousing.SelectedItem = null;
             TextBoxNameHousing.Clear();
             TextBoxAddressHousing.Clear();
             TextBoxQuantityHousing.Clear();
             ButtonSaveHousing.IsEnabled = true;
             TextBoxNameHousing.IsEnabled = true;
             TextBoxAddressHousing.IsEnabled = true;
             TextBoxQuantityHousing.IsEnabled = true;
         }

         private void ButtonCreateCabinet_Click(object sender, RoutedEventArgs e)
         {
             ListBoxCabinet.SelectedItem = null;
             TextBoxCabinet.Clear();
             TextBoxEntryFloor.Clear();
             ComboBoxSelectHousing.SelectedItem = null;
             ButtonSaveCabinet.IsEnabled = true;
             TextBoxCabinet.IsEnabled = true;
             TextBoxEntryFloor.IsEnabled = true;
             ComboBoxSelectHousing.IsEnabled = true;
         }

         private void ButtonDeleteCabinet_Click(object sender, RoutedEventArgs e)
         {
             if (ListBoxCabinet.Items.Count == 0)
             {
                 Not.Notifications("Ошибка", "Произошла ошибка");
             }
             else if (ListBoxCabinet.SelectedItem != null)
             {
                 string NameCabinet = ListBoxCabinet.SelectedItem.ToString();
                 DbControls.DeleteCabinet(NameCabinet);
                 ListBoxCabinet.Items.Clear();
                 TextBoxCabinet.Clear();
                 TextBoxEntryFloor.Clear();
                 ComboBoxSelectHousing.SelectedItem = 0;
                 TextBoxCabinet.IsEnabled = true;
                 ListBoxCabinetRefresh();
                 ButtonDeleteCabinet.IsEnabled = false;
             }
         }

         async private void ListBoxCabinetRefresh()
         {
             ListBoxCabinet.Items.Clear();
             MySqlCommand command = new MySqlCommand("SELECT `cabinet_number` , `cabinet_name` FROM `cabinet`", conn); //Команда выбора данных
             conn.Open(); //Открываем соединение


             MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные
             while (await read.ReadAsync()) //Читаем пока есть данные
             {
                 if (read.GetValue(0).ToString() != "")
                 {
                     ListBoxCabinet.Items.Add(read.GetString(0) + " - " + read.GetString(1)); //Добавляем данные в лист итем
                 }
             }

             conn.Close(); //Закрываем соединение
         }

         async private void ListBoxHousingRefresh()
         {
             ListBoxHousing.Items.Clear();
             MySqlCommand command = new MySqlCommand("SELECT `housing_name` FROM `housing`", conn); //Команда выбора данных
             conn.Open(); //Открываем соединение


             MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные
             while (await read.ReadAsync()) //Читаем пока есть данные
             {
                 if (read.GetValue(0).ToString() != "")
                 {
                     ListBoxHousing.Items.Add(read.GetValue(0).ToString()); //Добавляем данные в лист итем
                 }
             }
             conn.Close(); //Закрываем соединение
         }

         async private void ComboBoxSelectHousingRefresh()
         {
             ComboBoxSelectHousing.Items.Clear();
             MySqlCommand command = new MySqlCommand("SELECT `housing_name` FROM `housing`", conn); //Команда выбора данных
             conn.Open(); //Открываем соединение

             MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные
             while (await read.ReadAsync()) //Читаем пока есть данные
             {
                 ComboBoxSelectClassCabinet.Items.Add(read.GetValue(0).ToString());
             }
             conn.Close(); //Закрываем соединение
         }*/
    }
}
