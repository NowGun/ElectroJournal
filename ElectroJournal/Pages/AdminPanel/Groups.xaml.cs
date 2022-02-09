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

        List<int> idGroups = new List<int>();
        List<int> idTeachers = new List<int>();
        List<int> idTypeLearning = new List<int>();

        private void ButtonGroupSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxGroupsName.Text) && ComboBoxClassTeacher.SelectedItem != null)
            {
                if (ListBoxGroups.SelectedItem != null)
                {
                    MySqlCommand command = new MySqlCommand("UPDATE `groups` SET `groups_name` = @name, `groups_name_abbreviated` = @nameabb, `groups_prefix` = @prefix, `groups_course` = @course," +
                        "`typelearning_idtypelearning` = (SELECT `idtypelearning` FROM `typelearning` WHERE `idtypelearning` = @typelearning), `teachers_idteachers` = (SELECT `idteachers` FROM `teachers` WHERE `idteachers` = @teachers)  WHERE `idgroups` = @id", conn);

                    command.Parameters.Add("@name", MySqlDbType.VarChar).Value = TextBoxGroupsName.Text;
                    command.Parameters.Add("@nameabb", MySqlDbType.VarChar).Value = TextBoxGroupsNameAbbreviated.Text;
                    command.Parameters.Add("@prefix", MySqlDbType.VarChar).Value = TextBoxGroupsPrefix.Text;
                    command.Parameters.Add("@course", MySqlDbType.VarChar).Value = NumberBoxCourse.Text;
                    command.Parameters.Add("@typelearning", MySqlDbType.VarChar).Value = idTypeLearning[ComboBoxTypeLearning.SelectedIndex];
                    command.Parameters.Add("@teachers", MySqlDbType.VarChar).Value = idTeachers[ComboBoxClassTeacher.SelectedIndex];
                    command.Parameters.Add("@id", MySqlDbType.VarChar).Value = idGroups[ListBoxGroups.SelectedIndex];

                    conn.Open();

                    if (command.ExecuteNonQuery() == 1)
                    {
                        conn.Close();
                        FillListBoxGroups();

                        TextBoxGroupsName.Clear();
                        TextBoxGroupsNameAbbreviated.Clear();
                        TextBoxGroupsPrefix.Clear();
                        ComboBoxTypeLearning.SelectedIndex = 0;
                        ComboBoxClassTeacher.SelectedItem = null;
                        NumberBoxCourse.Text = "1";

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
                    command.Parameters.Add("@typelearning", MySqlDbType.VarChar).Value = idTypeLearning[ComboBoxTypeLearning.SelectedIndex];
                    command.Parameters.Add("@teachers", MySqlDbType.VarChar).Value = idTeachers[ComboBoxClassTeacher.SelectedIndex];

                    conn.Open();

                    if (command.ExecuteNonQuery() == 1)
                    {
                        conn.Close();
                        FillListBoxGroups();

                        TextBoxGroupsName.Clear();
                        TextBoxGroupsNameAbbreviated.Clear();
                        TextBoxGroupsPrefix.Clear();
                        ComboBoxTypeLearning.SelectedIndex = 0;
                        ComboBoxClassTeacher.SelectedItem = null;
                        NumberBoxCourse.Text = "1";

                        ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                    }
                    else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Ошибка", "Произошла ошибка");
                }
            }
            else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Ошибка", "Заполните помеченные поля");            
        }

        private void ButtonGroupDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteGroup();
        }

        private void ButonGroupAdd_Click(object sender, RoutedEventArgs e)
        {
            ListBoxGroups.SelectedItem = null;

            FillComboBoxClassTeacher();
            FillComboBoxTypeLearning();

            TextBoxGroupsName.Clear();
            TextBoxGroupsNameAbbreviated.Clear();
            TextBoxGroupsPrefix.Clear();
            ComboBoxTypeLearning.SelectedIndex = 0;
            ComboBoxClassTeacher.SelectedItem = null;
            NumberBoxCourse.Text = "1";
        }

        private async void FillListBoxGroups()
        {
            ListBoxGroups.Items.Clear();
            idGroups.Clear();

            MySqlCommand command = new MySqlCommand("", conn); //Команда выбора данных

            if (ComboBoxGroupsSorting.SelectedIndex == 0) command.CommandText = ("SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` ORDER BY `groups_name`");
            else if (ComboBoxGroupsSorting.SelectedIndex == 1) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 1 ORDER BY `groups_name`";
            else if (ComboBoxGroupsSorting.SelectedIndex == 2) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 2 ORDER BY `groups_name`";
            else if (ComboBoxGroupsSorting.SelectedIndex == 3) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 3 ORDER BY `groups_name`";
            else if (ComboBoxGroupsSorting.SelectedIndex == 4) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 4 ORDER BY `groups_name`";

            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные

            for (int i = 0; await read.ReadAsync(); i++)
            {
                ListBoxGroups.Items.Add(read.GetValue(1));
                idGroups.Add(read.GetInt32(0));
            }
            conn.Close(); //Закрываем соединение
        }

        private async void FillComboBoxClassTeacher()
        {
            ComboBoxClassTeacher.Items.Clear();

            MySqlCommand command = new MySqlCommand("SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers`", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение

            MySqlDataReader read = (MySqlDataReader) await command.ExecuteReaderAsync(); //Считываем и извлекаем данные

            for (int i = 0; await read.ReadAsync(); i++)
            {
                ComboBoxClassTeacher.Items.Add(read.GetValue(2).ToString() + " " + read.GetValue(1).ToString() + " " + read.GetValue(3).ToString());
                idTeachers.Add(read.GetInt32(0));
            }
            
            conn.Close(); //Закрываем соединение
        }

        private async void FillComboBoxTypeLearning()
        {
            ComboBoxTypeLearning.Items.Clear();

            MySqlCommand command = new MySqlCommand("SELECT `idtypelearning`, `typelearning_name` FROM `typelearning`", conn); //Команда выбора данных
            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные

            for (int i = 0; await read.ReadAsync(); i++)
            {
                ComboBoxTypeLearning.Items.Add(read.GetValue(1));
                idTypeLearning.Add(read.GetInt32(0));
            }           
            conn.Close();

            ComboBoxTypeLearning.SelectedIndex = 0;
        }

        private async void ListBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonGroupDelete.IsEnabled = true;

            if (ListBoxGroups.SelectedItem != null) //если строка выделена выполняется условие
            {
                MySqlCommand command = new MySqlCommand("SELECT * FROM showgroups WHERE `idgroups` = @id", conn); //Команда выбора данных

                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = idGroups[ListBoxGroups.SelectedIndex];

                conn.Open(); //Открываем соединение
                MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные

                if (await read.ReadAsync()) //Читаем пока есть данные
                {
                    string FIO = read.GetString(6) + " " + read.GetString(5) + " " + read.GetString(7);

                    TextBoxGroupsName.Text = read.GetString(1);
                    TextBoxGroupsNameAbbreviated.Text = read.GetString(2);
                    TextBoxGroupsPrefix.Text = read.GetString(3);
                    ComboBoxTypeLearning.SelectedItem = read.GetString(4);
                    ComboBoxClassTeacher.SelectedItem = FIO;
                    NumberBoxCourse.Text = read.GetString(8);


                }
                conn.Close(); //Закрываем соединение
            }
        }

        private void ListBoxGroups_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteGroup();
            }
        }

        private void DeleteGroup()
        {
            if (ListBoxGroups.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListBoxGroups.SelectedItem != null)
            {
                DbControls.DeleteGroup(idGroups[ListBoxGroups.SelectedIndex]);
                ListBoxGroups.Items.Clear();

                TextBoxGroupsName.Clear();
                TextBoxGroupsNameAbbreviated.Clear();
                TextBoxGroupsPrefix.Clear();
                ComboBoxTypeLearning.SelectedIndex = 0;
                ComboBoxClassTeacher.SelectedItem = null;
                NumberBoxCourse.Text = "1";

                FillListBoxGroups();
            }
        }

        private void ComboBoxGroupsSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListBoxGroups();
        }
    }
}
