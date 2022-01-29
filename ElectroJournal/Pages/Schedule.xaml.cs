using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ElectroJournal.Classes;
using ElectroJournal.Windows;
using ElectroJournal.Pages;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;
using Notifications.Wpf;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Schedule.xaml
    /// </summary>
    public partial class Schedule : Page
    {
        public Schedule()
        {
            InitializeComponent();
            
            //CompletionDataGridGroups();
           // ComboBoxCabinetRefresh();
            ComboBoxCabinet.SelectedIndex = 0;
            //ListBoxCabinetRefresh();
            //ComboBoxCabinetSorting();
            //ComboBoxDisciplineRefresh();
            ComboBoxDiscipline.SelectedIndex = 0;
            //ListBoxDisciplineRefresh();
           // ComboBoxDisciplineSorting();
            //ListBoxTeachersRefresh();
            //FillDataGrid();
        }

        DataBase DbUser = new DataBase();

        MySqlConnection conn = DataBase.GetDBConnection();


        private void FillDataGridStroke()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT `Group` FROM `groups`", conn);
            DataTable dt = new DataTable();
            conn.Open();
            MySqlDataAdapter sdr = new MySqlDataAdapter(cmd);
            sdr.Fill(dt);
            DataGridShedule.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        async private void FillDataGrid()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT `Group` FROM `groups`", conn);
            conn.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            await Task.Run(() =>
            {

            adapter.Fill(dt);
            });
            DataGridShedule.ItemsSource = dt.DefaultView;
            cmd.Dispose();
            conn.Close();
        }

        private void dataGridView2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }



        private void ComboBoxCabinetRefresh()
        {
            ComboBoxCabinet.Items.Clear();
            MySqlCommand command = new MySqlCommand("SELECT `namehousing` FROM `housing`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                ComboBoxCabinet.Items.Add(read.GetValue(0).ToString());
            }
            conn.Close(); //Закрываем соединение
        }

        private void ListBoxCabinetRefresh()
        {
            MySqlCommand command = new MySqlCommand();
            command = new MySqlCommand("SELECT `NumberCabinet` FROM `cabinet`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                if (read.GetValue(0).ToString() != "")
                    ListBoxCabinet.Items.Add(read.GetValue(0).ToString()); //Добавляем данные в лист итем
            }
            conn.Close(); //Закрываем соединение
        }

        private void ComboBoxCabinetSorting()
        {
            MySqlCommand command = new MySqlCommand();

            ListBoxCabinet.Items.Clear();

            command = new MySqlCommand("SELECT NumberCabinet FROM cabinet WHERE housing = (SELECT idhousing FROM housing WHERE namehousing = @name)", conn);
            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = ComboBoxCabinet.SelectedItem.ToString();

            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                if (read.GetValue(0).ToString() != "")
                    ListBoxCabinet.Items.Add(read.GetValue(0).ToString()); //Добавляем данные в лист итем
            }
            conn.Close(); //Закрываем соединение
        }

        private void ComboBoxDisciplineSorting()
        {
            MySqlCommand command = new MySqlCommand();

            ListBoxDiscipline.Items.Clear();

            command = new MySqlCommand("SELECT disciplineAbbreviated FROM discipline WHERE IndexDiscipline = (SELECT idIndexDiscipline FROM indexdiscipline WHERE IndexDisciplinecol = @name)", conn);
            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = ComboBoxDiscipline.SelectedItem.ToString();

            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                if (read.GetValue(0).ToString() != "")
                    ListBoxDiscipline.Items.Add(read.GetValue(0).ToString()); //Добавляем данные в лист итем
            }
            conn.Close(); //Закрываем соединение
        }

        private void ComboBoxDisciplineRefresh()
        {
            ComboBoxDiscipline.Items.Clear();
            MySqlCommand command = new MySqlCommand("SELECT `IndexDisciplinecol` FROM `IndexDiscipline`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                ComboBoxDiscipline.Items.Add(read.GetValue(0).ToString());
            }
            conn.Close(); //Закрываем соединение
        }

        private void ListBoxDisciplineRefresh()
        {
            ListBoxDiscipline.Items.Clear();
            MySqlCommand command = new MySqlCommand("SELECT `disciplineAbbreviated` FROM `discipline`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                if (read.GetValue(0).ToString() != "")
                    ListBoxDiscipline.Items.Add(read.GetValue(0).ToString()); //Добавляем данные в лист итем
            }
            conn.Close(); //Закрываем соединение
        }

        private void ListBoxTeachersRefresh()
        {
            ListBoxTeachers.Items.Clear();
            MySqlCommand command = new MySqlCommand("SELECT `LastName`, `FirstName`, `MiddleName` FROM `teachers`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (read.Read()) //Читаем пока есть данные
            {
                if (read.GetValue(0).ToString() != "" && read.GetValue(1).ToString() != "" && read.GetValue(2).ToString() != "")
                    ListBoxTeachers.Items.Add(read.GetValue(0).ToString() + " " + read.GetValue(1).ToString() + " " + read.GetValue(2).ToString()); //Добавляем данные в лист итем
            }
            conn.Close(); //Закрываем соединение
        }

        private void ListBoxCabinetInformation()
        {
            if (ListBoxCabinet.SelectedItem != null) //если строка выделена выполняется условие
            {
                MySqlCommand command = new MySqlCommand("SELECT NumberCabinet, IFNULL(NameCabinet, ''), IFNULL(NumberSeats, ''), IFNULL(Information, '') FROM cabinet;", conn); //Команда выбора данных
                conn.Open(); //Открываем соединение
                MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные

                while (read.Read()) //Читаем пока есть данные
                {
                    if (ListBoxCabinet.SelectedItem.ToString() == read.GetString(0))
                    {
                        TextBoxNameCabinet.Text = read.GetString(1);
                        TextBoxNumberPlaces.Text = read.GetString(2);
                        TextBoxInformationCabinet.Text = read.GetString(3);
                        break;
                    }
                }
                conn.Close(); //Закрываем соединение
            }
        }


        private void DataGridShedule_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            //ListBoxCabinetInformation();
        }

        private void DataGridShedule_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }


        private void ComboBoxDiscipline_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxDisciplineSorting();
        }

        private void ComboBoxCabinet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxNameCabinet.Clear();
            TextBoxInformationCabinet.Clear();
            TextBoxNumberPlaces.Clear();
            ComboBoxCabinetSorting();
        }

        private void ComboBoxDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxDisciplineSorting();
        }

        private void ListBoxCabinet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxCabinetInformation();
        }
    }
}
