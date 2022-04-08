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

            ComboBoxSortingTeacher.SelectedIndex = 0;
            //ListViewTeachersRefresh();
            //TextBoxTeachersPassword.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(16);

        }        

        List<int> idTeachers = new List<int>();


        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DataBaseControls DbControls = new DataBaseControls();
            ProgressBarTeachers.Visibility = Visibility.Visible;

            if (!string.IsNullOrWhiteSpace(TextBoxTeachersFIO.Text) && !string.IsNullOrWhiteSpace(TextBoxTeachersMail1.Text) && !string.IsNullOrWhiteSpace(TextBoxTeachersLogin.Text))
            {
                if (TextBoxTeachersFIO.Text.Split(new String[] {" "}, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                {
                    string[] FIO = TextBoxTeachersFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ListViewTeachers.SelectedItem != null)
                    {
                        using (zhirovContext db = new zhirovContext())
                        {
                            Teacher? teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex]);

                            if (teacher != null)
                            {
                                teacher.TeachersName = FIO[1];
                                teacher.TeachersSurname = FIO[0];
                                teacher.TeachersPatronymic = FIO[2];
                                teacher.TeachersAccesAdminPanel = CheckBoxAdminAccess.IsChecked.ToString();
                                teacher.TeachersPhone = TextBoxTeachersPhone1.Text;
                                teacher.TeachersMail = TextBoxTeachersMail1.Text;

                                await db.SaveChangesAsync();

                                ListViewTeachersRefresh();
                                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                            }
                        }                        
                    }
                    else
                    {
                        using (zhirovContext db = new zhirovContext())
                        {
                            var teac = await db.Teachers.Where(p => p.TeachersLogin == TextBoxTeachersLogin.Text).ToListAsync();

                            Teacher teacher = new Teacher { TeachersName = FIO[1], TeachersSurname = FIO[0], TeachersPatronymic = FIO[2], TeachersLogin = TextBoxTeachersLogin.Text,
                            TeachersPassword = DbControls.Hash(PasswordBoxTeachers.Password), TeachersMail = TextBoxTeachersMail1.Text, TeachersPhone = TextBoxTeachersPhone1.Text, 
                                TeachersAccesAdminPanel = CheckBoxAdminAccess.IsChecked.ToString() };

                            if (teac.Count == 0)
                            {
                                await db.Teachers.AddAsync(teacher);
                                await db.SaveChangesAsync();

                                SendPasswordToUser();
                                //SendSMSToUser();

                                TextBoxTeachersFIO.Clear();
                                TextBoxTeachersPhone1.Clear();
                                TextBoxTeachersMail1.Clear();
                                TextBoxTeachersLogin.Clear();
                                PasswordBoxTeachers.Clear();
                                CheckBoxAdminAccess.IsChecked = false;
                                ListViewTeachersRefresh();
                                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                            }
                            else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данный логин уже занят");
                        }
                    }
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Поле ФИО должно быть в формате: Фамилия - Имя - Отчество");               
            }
            else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");

            ProgressBarTeachers.Visibility = Visibility.Hidden;
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

            if (ListViewTeachers.SelectedItem != null)
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

        private async void DeleteTeachers() 
        {
            if (ListViewTeachers.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListViewTeachers.SelectedItem != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
                    Teacher? teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex]);

                    if (teacher != null)
                    {
                        db.Teachers.Remove(teacher);
                        await db.SaveChangesAsync();

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
