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
using System.Windows.Media;
using MySql.Data.MySqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NickBuhro.Translit;
using System.Net.Mail;
using System.Net;
using ElectroJournal.Classes;
using System.Windows.Forms;
using SmsRu;
using System.Drawing;

namespace ElectroJournal.Pages.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для Groups.xaml
    /// </summary>
    public partial class Groups : Page
    {
        public Groups()
        {
            InitializeComponent();

            FillListBoxGroups();
            FillComboBoxClassTeacher();
            FillComboBoxTypeLearning();
        }

        DataBase DbUser = new DataBase();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBase.GetDBConnection();

        int idTeachers = 0;
        int idTypeLearning = 0;
        int idGroups = 0;

        private void ButtonGroupSave_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxGroupsName.Text == string.Empty)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Ошибка", "Напишите название");
            }
            else if (ListBoxGroups.SelectedItem != null)
            {
                MySqlCommand command = new MySqlCommand("UPDATE `groups` SET `groups_name` = @name, `groups_name_abbreviated` = @nameabb, `groups_prefix` = @prefix, `groups_course` = @course," +
                    "`typelearning_idtypelearning` = (SELECT `idtypelearning` FROM `typelearning` WHERE `idtypelearning` = @typelearning), `teachers_idteachers` = (SELECT `idteachers` FROM `teachers` WHERE `idteachers` = @teachers)  WHERE `idgroups` = @id", conn);

                command.Parameters.Add("@name", MySqlDbType.VarChar).Value = TextBoxGroupsName.Text;
                command.Parameters.Add("@nameabb", MySqlDbType.VarChar).Value = TextBoxGroupsNameAbbreviated.Text;
                command.Parameters.Add("@prefix", MySqlDbType.VarChar).Value = TextBoxGroupsPrefix.Text;
                command.Parameters.Add("@course", MySqlDbType.VarChar).Value = NumberBoxCourse.Text;
                command.Parameters.Add("@typelearning", MySqlDbType.VarChar).Value = idTypeLearning;
                command.Parameters.Add("@teachers", MySqlDbType.VarChar).Value = idTeachers;
                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = idGroups;

                conn.Open();

                if (command.ExecuteNonQuery() == 1)
                {
                    conn.Close();
                    FillListBoxGroups();
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Ошибка", "Произошла ошибка");
            }
            else
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO `groups` (groups_name, groups_name_abbreviated, groups_prefix, typelearning_idtypelearning, teachers_idteachers, groups_course) VALUES " +
                    "(@name, @nameabb, @prefix, (SELECT idtypelearning FROM typelearning WHERE idtypelearning = @typelearning), (SELECT idteachers FROM teachers WHERE idteachers = @teachers), @course)", conn);

                command.Parameters.Add("@name", MySqlDbType.VarChar).Value = TextBoxGroupsName.Text;
                command.Parameters.Add("@nameabb", MySqlDbType.VarChar).Value = TextBoxGroupsNameAbbreviated.Text;
                command.Parameters.Add("@prefix", MySqlDbType.VarChar).Value = TextBoxGroupsPrefix.Text;
                command.Parameters.Add("@course", MySqlDbType.VarChar).Value = NumberBoxCourse.Text;
                command.Parameters.Add("@typelearning", MySqlDbType.VarChar).Value = idTypeLearning;
                command.Parameters.Add("@teachers", MySqlDbType.VarChar).Value = idTeachers;

                    conn.Open();

                    if (command.ExecuteNonQuery() == 1)
                    {
                        conn.Close();
                    FillListBoxGroups();
                        ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                    }
                    else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Ошибка", "Произошла ошибка");
            }
        }

        private void ButtonGroupDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButonGroupAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void FillListBoxGroups()
        {
            ListBoxGroups.Items.Clear();

            MySqlCommand command = new MySqlCommand("SELECT `idgroups`, `groups_name` FROM `groups`", conn); //Команда выбора данных
            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (await read.ReadAsync()) //Читаем пока есть данные
            {
                ListBoxGroups.Items.Add(read.GetValue(1));
                idGroups = read.GetInt32(0);
            }
            conn.Close();
        }

        private async void FillComboBoxClassTeacher()
        {
            ComboBoxClassTeacher.Items.Clear();

            MySqlCommand command = new MySqlCommand("SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение

            MySqlDataReader read = (MySqlDataReader) await command.ExecuteReaderAsync(); //Считываем и извлекаем данные
            while (await read.ReadAsync()) //Читаем пока есть данные
            {
                ComboBoxClassTeacher.Items.Add(read.GetValue(2).ToString() + " " + read.GetValue(1).ToString() + " " + read.GetValue(3).ToString());
                idTeachers = read.GetInt32(0);
            }
            conn.Close(); //Закрываем соединение
        }

        private async void FillComboBoxTypeLearning()
        {
            ComboBoxTypeLearning.Items.Clear();

            MySqlCommand command = new MySqlCommand("SELECT `idtypelearning`, `typelearning_name` FROM `typelearning`", conn); //Команда выбора данных
            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (await read.ReadAsync()) //Читаем пока есть данные
            {
                ComboBoxTypeLearning.Items.Add(read.GetValue(1));
                idTypeLearning = read.GetInt32(0);
            }
            conn.Close();
        }

        private void ListBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }


    }
}
