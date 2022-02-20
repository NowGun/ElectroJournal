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
using System.Threading;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;

namespace ElectroJournal.Pages.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для Teachers.xaml
    /// </summary>
    public partial class Teachers : Page
    {
        public Teachers()
        {
            InitializeComponent();

            ListViewTeachersRefresh();
            //TextBoxTeachersPassword.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(16);

        }

        

        List<int> idTeachers = new List<int>();


        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DataBaseConn DbUser = new DataBaseConn();
            DataBaseControls DbControls = new DataBaseControls();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            if (!string.IsNullOrWhiteSpace(TextBoxTeachersFIO.Text) && !string.IsNullOrWhiteSpace(TextBoxTeachersMail1.Text) && !string.IsNullOrWhiteSpace(TextBoxTeachersLogin.Text))
            {
                if (TextBoxTeachersFIO.Text.Split(new String[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                {
                    ProgressBarTeachers.Visibility = Visibility.Visible;
                    string[] FIO = TextBoxTeachersFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ListViewTeachers.SelectedItem != null)
                    {
                        MySqlCommand command = new MySqlCommand("UPDATE `teachers` SET `teachers_name` = @name, `teachers_surname` = @surname, `teachers_patronymic` = @patronymic, " +
                            "`teachers_acces_admin_panel` = @admin, `teachers_phone` = @phone, `teachers_mail` = @mail WHERE `idteachers` = @id", conn);


                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = FIO[1];
                        command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = FIO[0];
                        command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = FIO[2];
                        command.Parameters.Add("@admin", MySqlDbType.VarChar).Value = CheckBoxAdminAccess.IsChecked.ToString();
                        command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = TextBoxTeachersPhone1.Text;
                        command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = TextBoxTeachersMail1.Text;
                        command.Parameters.Add("@id", MySqlDbType.VarChar).Value = idTeachers[ListViewTeachers.SelectedIndex];

                        await conn.OpenAsync();

                        if (command.ExecuteNonQuery() == 1)
                        {
                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                        }
                        else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Ошибка", "Произошла ошибка");

                        conn.Close();
                        ListViewTeachersRefresh();
                    }
                    else
                    {

                        

                            ProgressBarTeachers.Visibility = Visibility.Visible;

                        MySqlCommand command = new MySqlCommand("INSERT INTO teachers (`teachers_login`, `teachers_password`, `teachers_name`, `teachers_surname`, `teachers_patronymic`," +
                            " `teachers_acces_admin_panel`, `teachers_phone`, `teachers_mail`) VALUES (@login, @password, @name, @surname, @patronymic, @admin, @phone, @mail)", conn);

                        MySqlCommand command2 = new MySqlCommand("CREATE USER @login @`%` IDENTIFIED BY @password", conn);
                        MySqlCommand command3 = new MySqlCommand("GRANT ALL PRIVILEGES ON zhirov. * TO @login @'%';", conn);

                        command2.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxTeachersLogin.Text;
                        command2.Parameters.Add("@password", MySqlDbType.VarChar).Value = PasswordBoxTeachers.Password;

                        command3.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxTeachersLogin.Text;

                        command.Parameters.Add("@login", MySqlDbType.VarChar).Value = TextBoxTeachersLogin.Text;
                        //command.Parameters.Add("@password", MySqlDbType.VarChar).Value = PBKDF2HashHelper.CreatePasswordHash(PasswordBoxTeachers.Password);
                        command.Parameters.Add("@password", MySqlDbType.VarChar).Value = DbControls.Hash(PasswordBoxTeachers.Password);
                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = FIO[1];
                        command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = FIO[0];
                        command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = FIO[2];
                        //command.Parameters.Add("@image", MySqlDbType.VarChar).Value = words[2];
                        command.Parameters.Add("@admin", MySqlDbType.VarChar).Value = CheckBoxAdminAccess.IsChecked.ToString();
                        command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = TextBoxTeachersPhone1.Text;
                        command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = TextBoxTeachersMail1.Text;



                        if (!DbControls.IsTeachersLoginExists(TextBoxTeachersLogin.Text) && !DbControls.IsUserExists(TextBoxTeachersLogin.Text))
                        {
                            await conn.OpenAsync();

                            if (command.ExecuteNonQuery() == 1 && command2.ExecuteNonQuery() == 0 && command3.ExecuteNonQuery() == 0)
                            {
                                //SendPasswordToUser();
                                //SendSMSToUser();
                                

                                TextBoxTeachersFIO.Clear();
                                TextBoxTeachersPhone1.Clear();
                                TextBoxTeachersMail1.Clear();
                                TextBoxTeachersLogin.Clear();
                                PasswordBoxTeachers.Clear();
                                CheckBoxAdminAccess.IsChecked = false;

                                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                                ProgressBarTeachers.Visibility = Visibility.Hidden;
                            }

                            conn.Close();
                            ListViewTeachersRefresh();
                        }
                        else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данный логин уже занят");
                    }
                    ProgressBarTeachers.Visibility=Visibility.Hidden;
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Поле ФИО должно быть в формате: Фамилия - Имя - Отчество");               
            }
            else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");

            


        }

        private async void ListViewTeachersRefresh()
        {
            ListViewTeachers.Items.Clear();
            idTeachers.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                switch (ComboBoxSortingTeacher.SelectedIndex)
                {
                    case 0:
                        await db.Teachers.OrderBy(t => t.TeachersSurname).ForEachAsync(t =>
                        {
                            ListViewTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                            idTeachers.Add((int)t.Idteachers);
                        });
                        break;
                    case 1:
                        await db.Teachers.OrderByDescending(t => t.TeachersSurname).ForEachAsync(t =>
                        {
                            ListViewTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                            idTeachers.Add((int)t.Idteachers);
                        });
                        break;
                }
            }


            /*
            idTeachers.Clear();

            MySqlCommand command = new MySqlCommand("", conn); //Команда выбора данных
            //command.CommandText = "SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers`";

            if (ComboBoxSortingTeacher.SelectedIndex == 0) command.CommandText = "SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers` ORDER BY `teachers_surname`";
            else command.CommandText = "SELECT `idteachers`, `teachers_name`, `teachers_surname`, `teachers_patronymic` FROM `teachers` ORDER BY `teachers_surname` DESC";

            await conn.OpenAsync(); //Открываем соединение
            MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync(); //Считываем и извлекаем данные
           
            for  (int i = 0; await read.ReadAsync(); i++)
            {
                if (read.GetValue(1).ToString() != "" && read.GetValue(2).ToString() != "" && read.GetValue(3).ToString() != "")                
                    ListViewTeachers.Items.Add(read.GetValue(2).ToString() + " " + read.GetValue(1).ToString() + " " + read.GetValue(3).ToString()); //Добавляем данные в лист итем

                idTeachers.Add(read.GetInt32(0));
            }
            conn.Close(); //Закрываем соединение
                          //ButtonSaveTeacher.IsEnabled = false;
            */
        }


        private void TextBoxTeachersPhone2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void TextBoxTeachersFIO_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (ListViewTeachers.SelectedItem == null)
            {
                PasswordBoxTeachers.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(8);

                TextBoxTeachersLogin.Text = Transliteration.CyrillicToLatin(TextBoxTeachersFIO.Text.Split().First(), NickBuhro.Translit.Language.Russian);
            }
        }

        private async void SendPasswordToUser()
        {
            try
            {
            string user = (string)((MainWindow)System.Windows.Application.Current.MainWindow).TextBlockTeacher.Content;
            bool a = true;
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("zhirowdaniil@gmail.com", user);
            // кому отправляем

                MailAddress to = new MailAddress(TextBoxTeachersMail1.Text);

            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = Title;
            // текст письма
            m.Body = "Добро пожаловать в систему Электронный журнал\n\nАдминистратор зарегистрировал Вас в системе электронного журнала, ниже написаны данные для входа в вашу учетную запись.\nМы рекомендуем при первой возможности поменять пароль на более удобный Вам, так как нынешний пароль является временным." +
                "\n\nЛогин: " + TextBoxTeachersLogin.Text + "\nПароль: " + PasswordBoxTeachers.Password + "\n\n\n\n";
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.elasticemail.com", 2525);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("zhirowdaniil@gmail.com", "E0E7027197724CDBDAFAD917FB914057C0CB");
            smtp.EnableSsl = true;

            await Task.Run(() =>
            {
                try
                {
                    smtp.Send(m);
                }
                catch (SmtpException)
                {
                    a = true;
                }
            });

            if (a)
            {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "письмо отправлено");
                //ButtonSend.IsEnabled = true;
                //anim.Begin();
                //RectangleLoad.Visibility = Visibility.Hidden;
            }
            else if (!a)
            {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "письмо не отправилось");
                //ButtonSend.IsEnabled = true;
                //anim.Begin();
                //RectangleLoad.Visibility = Visibility.Hidden;
            }
            }
            catch (System.FormatException)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Введен некорректный Email");
            }
        }

        private void SendSMSToUser() // СТОИТ ОЧЕНЬ ДОРОГО, ИСПОЛЬЗОВАТЬ ТОЛЬКО В КРАЙНИХ СЛУЧАЯХ!!!!
        {
            string myApiKey = "B0361252-C8BA-5438-E643-4651FCC4E55B"; //Ваш API ключ
            SmsRu.SmsRu sms = new SmsRu.SmsRu(myApiKey);
            var response = sms.Send(TextBoxTeachersPhone1.Text, "Добро пожаловать в Электронный журнал\n\nЛогин: " + TextBoxTeachersLogin.Text + "\nПароль: " + PasswordBoxTeachers.Password);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ListViewTeachers.SelectedItem = null;
            TextBoxTeachersFIO.Clear();
            TextBoxTeachersPhone1.Clear();
            TextBoxTeachersMail1.Clear();
            TextBoxTeachersLogin.Clear();
            PasswordBoxTeachers.Clear();
            CheckBoxAdminAccess.IsChecked = false;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteTeachers();
        }

        private async void ListViewTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonDelete.IsEnabled = true;

            if (ListViewTeachers != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
                    var teachers = await db.Teachers.Where(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex]).ToListAsync();

                    foreach (var t in teachers)
                    {
                        string FIO = t.TeachersSurname + " " + t.TeachersName + " " + t.TeachersPatronymic;

                        TextBoxTeachersFIO.Text = FIO;
                        TextBoxTeachersLogin.Text = t.TeachersLogin;
                        PasswordBoxTeachers.Password = t.TeachersPassword;
                        CheckBoxAdminAccess.IsChecked = bool.Parse(t.TeachersAccesAdminPanel);
                        TextBoxTeachersMail1.Text = t.TeachersMail;
                        TextBoxTeachersPhone1.Text = t.TeachersPhone;
                    }
                }
            }
        }

        private void ComboBoxSortingTeacher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewTeachersRefresh();
        }


        private void ListViewTeachers_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteTeachers();
            }
        }

        private void DeleteTeachers() 
        {
            DataBaseConn DbUser = new DataBaseConn();
            DataBaseControls DbControls = new DataBaseControls();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            if (ListViewTeachers.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListViewTeachers.SelectedItem != null)
            {
                DbControls.DeleteTeachers(idTeachers[ListViewTeachers.SelectedIndex]);
                ListViewTeachers.Items.Clear();

                ListViewTeachersRefresh();
                TextBoxTeachersFIO.Clear();
                TextBoxTeachersPhone1.Clear();
                TextBoxTeachersMail1.Clear();
                TextBoxTeachersLogin.Clear();
                PasswordBoxTeachers.Clear();
                CheckBoxAdminAccess.IsChecked = false;

                ButtonDelete.IsEnabled = false;
            }
        }
        private int lastFoundIndex = -1;
        private void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int i;
            for (i = lastFoundIndex + 1; i < ListViewTeachers.Items.Count; i++)
            {
                var currVal = ListViewTeachers.Items[i].ToString();
                if (currVal.ToLower().Contains(SearchBox.Text.ToLower()))
                {
                    ListViewTeachers.SelectedIndex = i;
                    lastFoundIndex = i;
                    break;//прерываем цикл
                }
            }
            if (lastFoundIndex > -1 && i == ListViewTeachers.Items.Count)
            {
                for (int s = 0; s <= lastFoundIndex; s++)
                {
                    var currVal = ListViewTeachers.Items[s].ToString();
                    if (currVal.ToLower().Contains(SearchBox.Text.ToLower()))
                    {
                        ListViewTeachers.SelectedIndex = s;
                        lastFoundIndex = s;
                        break;//прерываем цикл
                    }
                }
            }
        }

        private void PersonPicture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
        }
    }
}
