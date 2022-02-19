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
    /// Логика взаимодействия для Students.xaml
    /// </summary>
    public partial class Students : Page
    {
        public Students()
        {
            InitializeComponent();

            FillComboBoxStudents();

            ListBoxGroups.Visibility = Visibility.Hidden;
        }

        DataBase DbUser = new DataBase();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBase.GetDBConnection();

        List<int> idStudents = new List<int>();

        private void ListViewStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ButtonSaveTeacher.IsEnabled = true;
            if (ListBoxStudents.SelectedItem != null) //если строка выделена выполняется условие
            {
                MySqlCommand command = new MySqlCommand("SELECT `idstudents`, `students_name`, `students_surname`, `students_patronymic`, `students_birthday`, `students_residence`, `students_dormitory` FROM `students` WHERE idstudents = @id ", conn); //Команда выбора данных

                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Int32.Parse(ListBoxStudents.SelectedItem.ToString().Split().First());

                conn.Open(); //Открываем соединение
                MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные

                while (read.Read()) //Читаем пока есть данные
                {
                    string FIO = read.GetString(2) + " " + read.GetString(1) + " " + read.GetString(3);

                    TextBoxStudentsFIO.Text = FIO;
                    break;

                }
                conn.Close(); //Закрываем соединение
            }




        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string[] FIO = TextBoxStudentsFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrWhiteSpace(TextBoxStudentsFIO.Text))
            {
                if (TextBoxStudentsFIO.Text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                {
                    ProgressBar.Visibility = Visibility.Visible;

                    if (ListBoxStudents.SelectedItem != null)
                    {
                        MySqlCommand command = new MySqlCommand("UPDATE `students` SET `students_name` = @name, `students_surname` = @surname, `students_patronymic` = @patronymic, " +
                            "`students_birthday` = @birthday, `students_residence` = @residence, `students_dormitory` = @dormitory, `students_parent` = @parent, " +
                            "`students_phone` = @phone, `students_parent_phone` = @parentphone, `groups_idgroups` = @groups  WHERE `idstudents` = @id", conn);


                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = FIO[1];
                        command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = FIO[0];
                        command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = FIO[2];
                        command.Parameters.Add("@birthday", MySqlDbType.VarChar).Value = DatePickerDateBirthday.SelectedDate.Value.ToString("yyyy/MM/dd");
                        command.Parameters.Add("@residence", MySqlDbType.VarChar).Value = TextBoxStudentsResidence.Text;
                        command.Parameters.Add("@dormitory", MySqlDbType.VarChar).Value = CheckBoxStudentsDormitory.IsChecked.ToString();
                        command.Parameters.Add("@parent", MySqlDbType.VarChar).Value = TextBoxParentFIO.Text;
                        command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = TextBoxStudentsPhone.Text;
                        command.Parameters.Add("@parentphone", MySqlDbType.VarChar).Value = TextBoxParentPhone.Text;
                        command.Parameters.Add("@group", MySqlDbType.VarChar).Value = 1;
                        command.Parameters.Add("@id", MySqlDbType.VarChar).Value = idStudents[ListBoxStudents.SelectedIndex];

                        await conn.OpenAsync();

                        if (command.ExecuteNonQuery() == 1)
                        {
                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                        }
                        else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Ошибка", "Произошла ошибка");

                        conn.Close();
                        ListBoxStudentsRefresh();
                    }
                    else
                    {

                        ProgressBar.Visibility = Visibility.Visible;

                        MySqlCommand command = new MySqlCommand("INSERT INTO `students` (`students_name`, `students_surname`, `students_patronymic`, `students_birthday`, `students_residence`," +
                " `students_dormitory`, `students_parent`, `students_phone`, `students_parent_phone`, `groups_idgroups`) " +
                "VALUES (@name, @surname, @patronymic, @birthday, @residence, @dormitory, @parent, @phone, @parentphone, (SELECT idgroups FROM `groups` WHERE groups_name_abbreviated = @group))", conn);




                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = FIO[1];
                        command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = FIO[0];
                        command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = FIO[2];
                        command.Parameters.Add("@birthday", MySqlDbType.VarChar).Value = DatePickerDateBirthday.SelectedDate.Value.ToString("yyyy/MM/dd");
                        command.Parameters.Add("@residence", MySqlDbType.VarChar).Value = TextBoxStudentsResidence.Text;
                        command.Parameters.Add("@dormitory", MySqlDbType.VarChar).Value = CheckBoxStudentsDormitory.IsChecked.ToString();
                        command.Parameters.Add("@parent", MySqlDbType.VarChar).Value = TextBoxParentFIO.Text;
                        command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = TextBoxStudentsPhone.Text;
                        command.Parameters.Add("@parentphone", MySqlDbType.VarChar).Value = TextBoxParentPhone.Text;
                        command.Parameters.Add("@group", MySqlDbType.VarChar).Value = ListBoxGroups.SelectedItem.ToString();


                        await conn.OpenAsync();

                        if (command.ExecuteNonQuery() == 1)
                        {
                            TextBoxParentFIO.Clear();
                            TextBoxParentPhone.Clear();
                            TextBoxStudentsFIO.Clear();
                            TextBoxStudentsPhone.Clear();
                            TextBoxStudentsResidence.Clear();
                            DatePickerDateBirthday.Text = null;
                            CheckBoxStudentsDormitory.IsChecked = false;

                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                            ProgressBar.Visibility = Visibility.Hidden;
                        }
                        else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "НЕ РАБОТАЕТ");

                        conn.Close();
                        ListBoxStudentsRefresh();


                    }
                    ProgressBar.Visibility = Visibility.Hidden;
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Поле ФИО должно быть в формате: Фамилия - Имя - Отчество");
            }
            else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ListBoxStudents.SelectedItem = null;
            TextBoxParentFIO.Clear();
            TextBoxParentPhone.Clear();
            TextBoxStudentsFIO.Clear();
            TextBoxStudentsPhone.Clear();
            TextBoxStudentsResidence.Clear();
            DatePickerDateBirthday.Text = null;
            CheckBoxStudentsDormitory.IsChecked = false;


        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteStudent();
        }

        private async void ListBoxStudentsRefresh()
        {
            ListBoxStudents.Items.Clear();
            idStudents.Clear();

            MySqlCommand command = new MySqlCommand("", conn); //Команда выбора данных

            if (ComboBoxSorting.SelectedIndex == 0) command.CommandText = "SELECT `idstudents`, `students_name`, `students_surname`, `students_patronymic` FROM `students` ORDER BY `students_surname`";
            else command.CommandText = "SELECT `idstudents`, `students_name`, `students_surname`, `students_patronymic` FROM `students` ORDER BY `students_surname` DESC";

            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные

            for (int i = 0; await read.ReadAsync(); i++)
            {
                if (read.GetValue(1).ToString() != "" && read.GetValue(2).ToString() != "" && read.GetValue(3).ToString() != "")
                    ListBoxStudents.Items.Add(read.GetValue(2).ToString() + " " + read.GetValue(1).ToString() + " " + read.GetValue(3).ToString()); //Добавляем данные в лист итем

                idStudents.Add(read.GetInt32(0));
            }
            conn.Close(); //Закрываем соединение
                          //ButtonSaveTeacher.IsEnabled = false;

        }

        private void TextBoxStudentsPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void ListBoxStudents_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteStudent();
            }
        }

        private async void ListBoxStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonDelete.IsEnabled = true;

            if (ListBoxStudents.SelectedItem != null) //если строка выделена выполняется условие
            {
                MySqlCommand command = new MySqlCommand("SELECT `idstudents`, `students_name`, `students_surname`, `students_patronymic`, `students_birthday`, `students_residence`, " +
                            "`students_dormitory`, `students_parent`, `students_phone`, `students_parent_phone`, `groups_idgroups` FROM `students` WHERE idstudents = @id", conn); //Команда выбора данных

                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = idStudents[ListBoxStudents.SelectedIndex];

                conn.Open(); //Открываем соединение
                MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные

                if (await read.ReadAsync()) //Читаем пока есть данные
                {
                    string FIO = read.GetString(2) + " " + read.GetString(1) + " " + read.GetString(3);

                    TextBoxStudentsFIO.Text = FIO;
                    DatePickerDateBirthday.SelectedDate = read.GetDateTime(4);
                    TextBoxStudentsResidence.Text = read.GetString(5);
                    CheckBoxStudentsDormitory.IsChecked = bool.Parse(read.GetString(6));
                    TextBoxParentFIO.Text = read.GetString(7);
                    TextBoxStudentsPhone.Text = read.GetString(8);
                    TextBoxParentPhone.Text = read.GetString(9);
                }
                conn.Close(); //Закрываем соединение
            }
        }

        private void ComboBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxStudentsRefresh();
        }

        private void DeleteStudent()
        {
            if (ListBoxStudents.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListBoxStudents.SelectedItem != null)
            {
                DbControls.DeleteStudent(idStudents[ListBoxStudents.SelectedIndex]);
                ListBoxStudents.Items.Clear();

                ListBoxStudentsRefresh();
                TextBoxParentFIO.Clear();
                TextBoxParentPhone.Clear();
                TextBoxStudentsFIO.Clear();
                TextBoxStudentsPhone.Clear();
                TextBoxStudentsResidence.Clear();
                DatePickerDateBirthday.Text = null;
                CheckBoxStudentsDormitory.IsChecked = false;
            }
        }

        private async void FillComboBoxStudents()
        {
            ComboBoxGroups.Items.Clear();
            MySqlCommand command = new MySqlCommand("SELECT idcourse, course_name from course", conn); //Команда выбора данных
            conn.Open(); //Открываем соединение

            MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные
            while (await read.ReadAsync()) //Читаем пока есть данные
            {
                ComboBoxGroups.Items.Add(read.GetValue(1).ToString());
            }
            conn.Close(); //Закрываем соединение
        }

        List<int> idGroups = new List<int>();

        private async void FillListBox()
        {
            ListBoxGroups.Items.Clear();
            idGroups.Clear();

            MySqlCommand command = new MySqlCommand("select idgroups, groups_name_abbreviated from `groups` join course on course.idcourse = course_idcourse where course_name = @course", conn); //Команда выбора данных
            //command.CommandText = "SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers`";

            command.Parameters.Add("@course", MySqlDbType.VarChar).Value = ComboBoxGroups.SelectedItem.ToString();

            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные

            for (int i = 0; await read.ReadAsync(); i++)
            {
                ListBoxGroups.Items.Add(read.GetString(1));
                idGroups.Add(read.GetInt32(0));
            }
            conn.Close(); //Закрываем соединение
                          //ButtonSaveTeacher.IsEnabled = false;

        }

        private void ComboBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListBox();
            ListBoxGroups.Visibility = Visibility.Visible;
        }
    }
}