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
        }

        DataBase DbUser = new DataBase();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBase.GetDBConnection();

        private void ListViewStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ButtonSaveTeacher.IsEnabled = true;
            if (ListViewStudents.SelectedItem != null) //если строка выделена выполняется условие
            {
                MySqlCommand command = new MySqlCommand("SELECT `idstudents`, `students_name`, `students_surname`, `students_patronymic`, `students_birthday`, `students_residence`, `students_dormitory` FROM `students` WHERE idstudents = @id ", conn); //Команда выбора данных

                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = Int32.Parse(ListViewStudents.SelectedItem.ToString().Split().First());

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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            string[] FIO = TextBoxStudentsFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            MySqlCommand command = new MySqlCommand("INSERT INTO students (`students_name`, `students_surname`, `students_patronymic`, `students_birthday`, `students_residence`," +
                " `students_dormitory`) VALUES (@name, @surname, @patronymic, @birthday, @residence, @dormitory)", conn);

            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = FIO[1];
            command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = FIO[0];
            command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = FIO[2];
            command.Parameters.Add("@birthday", MySqlDbType.VarChar).Value = DatePickerDateBirthday.SelectedDate.Value.ToString("yyyy/MM/dd");
            command.Parameters.Add("@residence", MySqlDbType.VarChar).Value = TextBoxStudentsResidence.Text;
            command.Parameters.Add("@dormitory", MySqlDbType.VarChar).Value = CheckBoxStudentsDormitory.IsChecked.ToString();


            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
                MainWindow.Notifications("Сообщение", "Данные сохранены");
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Notifications("", DatePickerDateBirthday.SelectedDate.Value.ToString("yyyy/MM/dd"));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            if (ListViewStudents.Items.Count == 0)
            {
                mainwindow.Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListViewStudents.SelectedItem != null)
            {
                string name_test = ListViewStudents.SelectedItem.ToString().Split().First();
                //DbControls.DeleteTeachers(name_test);
                ListViewStudents.Items.Clear();
                ListViewStudentsRefresh();
            }
        }

        private async void ListViewStudentsRefresh()
        {
            ListViewStudents.Items.Clear();


            MySqlCommand command = new MySqlCommand("SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers`", conn); //Команда выбора данных
            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = command.ExecuteReader(); //Считываем и извлекаем данные
            while (await read.ReadAsync()) //Читаем пока есть данные
            {
                if (read.GetValue(1).ToString() != "" && read.GetValue(2).ToString() != "" && read.GetValue(3).ToString() != "")
                    ListViewStudents.Items.Add(read.GetValue(0).ToString() + " | " + read.GetValue(2).ToString() + " " + read.GetValue(1).ToString() + " " + read.GetValue(3).ToString()); //Добавляем данные в лист итем
            }
            conn.Close(); //Закрываем соединение
                          //ButtonSaveTeacher.IsEnabled = false;

        }

        private void TextBoxStudentsPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }
    }
}
