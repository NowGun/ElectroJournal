using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using NickBuhro.Translit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

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

            FillListBoxDiscipline();
            ComboBoxSortingTeacher.SelectedIndex = 0;
            //FillListBoxTeachers();
            //TextBoxTeachersPassword.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(16);

        }

        List<int> idTeachers = new();
        private int lastFoundIndex = -1;       

        private void PersonPicture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.ShowDialog();
        }
        private void SearchBoxDiscipline_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            FillListBoxDiscipline();
        }

        #region Заполнение ListBox
        private void ComboBoxSortingTeacher_SelectionChanged(object sender, SelectionChangedEventArgs e) // Сортировка листа
        {
            FillListBoxTeachers();
        }
        private async void FillListBoxTeachers()
        {
            ListViewTeachers.Items.Clear();
            idTeachers.Clear();

            using zhirovContext db = new();

            if (String.IsNullOrWhiteSpace(SearchBox.Text))
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
            else
            {
                await db.Teachers
                    .OrderBy(t => t.TeachersSurname)
                    .Where(t => EF.Functions.Like(t.TeachersName, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.TeachersSurname, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.TeachersPatronymic, $"%{SearchBox.Text}%"))
                    .ForEachAsync(t =>
                    {
                        ListViewTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                        idTeachers.Add((int)t.Idteachers);
                    });
            }
        } // Заполнение листа с преподавателями
        private async void FillListBoxDiscipline() // Заполнение листа с предметами
        {
            try
            {
                ObservableCollection<DisciplineList> co1 = new();

                using zhirovContext db = new();

                if (!String.IsNullOrWhiteSpace(SearchBoxDiscipline.Text))
                {
                    var c2 = await db.Disciplines.Where(c => EF.Functions.Like(c.DisciplinesNameAbbreviated, $"%{SearchBoxDiscipline.Text}%") || EF.Functions.Like(c.DisciplinesName, $"%{SearchBoxDiscipline.Text}%")).ToListAsync();

                    foreach (var c in c2)
                    {

                        co1.Add(new DisciplineList
                        {
                            DisciplineName = c.DisciplinesNameAbbreviated
                        });

                    }
                }
                else
                {
                    var c2 = await db.Disciplines.ToListAsync();

                    foreach (var c in c2)
                    {

                        co1.Add(new DisciplineList
                        {
                            DisciplineName = c.DisciplinesNameAbbreviated
                        });
                    }
                }

                ListBoxDiscipline.ItemsSource = co1;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Взаимодействие с данными
        private async void ListViewTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e) // Подгрузка данных определенного преподавателя
        {
            ButtonDelete.IsEnabled = true;

            if (ListViewTeachers.SelectedItem != null)
            {
                using zhirovContext db = new();
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
        private void TextBoxTeachersFIO_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) // Генерация логина и пароля
        {
            if (ListViewTeachers.SelectedItem == null)
            {
                PasswordBoxTeachers.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(8);

                TextBoxTeachersLogin.Text = Transliteration.CyrillicToLatin(TextBoxTeachersFIO.Text.Split().First(), NickBuhro.Translit.Language.Russian);
            }
        }
        private void ListViewTeachers_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) // Удаление преподавателя
        {
            if (e.Key == Key.Delete)
            {
                DeleteTeachers();
            }
        }
        private void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) // Поиск преподавателя
        {
            FillListBoxTeachers();
        } 
        private async void ButtonSave_Click(object sender, RoutedEventArgs e) // Редактирование/удаление преподавателя
        {
            DataBaseControls DbControls = new();
            ProgressBarTeachers.Visibility = Visibility.Visible;

            if (!string.IsNullOrWhiteSpace(TextBoxTeachersFIO.Text) && !string.IsNullOrWhiteSpace(TextBoxTeachersMail1.Text) && !string.IsNullOrWhiteSpace(TextBoxTeachersLogin.Text))
            {
                if (TextBoxTeachersFIO.Text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                {
                    string[] FIO = TextBoxTeachersFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ListViewTeachers.SelectedItem != null)
                    {
                        using zhirovContext db = new();

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

                            FillListBoxTeachers();
                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                        }

                    }
                    else
                    {
                        using zhirovContext db = new();
                        var teac = await db.Teachers.Where(p => p.TeachersLogin == TextBoxTeachersLogin.Text).ToListAsync();

                        Teacher teacher = new()
                        {
                            TeachersName = FIO[1],
                            TeachersSurname = FIO[0],
                            TeachersPatronymic = FIO[2],
                            TeachersLogin = TextBoxTeachersLogin.Text,
                            TeachersPassword = DbControls.Hash(PasswordBoxTeachers.Password),
                            TeachersMail = TextBoxTeachersMail1.Text,
                            TeachersPhone = TextBoxTeachersPhone1.Text,
                            TeachersAccesAdminPanel = CheckBoxAdminAccess.IsChecked.ToString()
                        };

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
                            FillListBoxTeachers();
                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                        }
                        else
                        {
                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данный логин уже занят");
                        }
                    }
                }
                else
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Поле ФИО должно быть в формате: Фамилия - Имя - Отчество");
                }
            }
            else
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");
            }

            ProgressBarTeachers.Visibility = Visibility.Hidden;
        } 
        private void ButtonAdd_Click(object sender, RoutedEventArgs e) // Добавление преподавателя
        {
            ListViewTeachers.SelectedItem = null;
            TextBoxTeachersFIO.Clear();
            TextBoxTeachersPhone1.Clear();
            TextBoxTeachersMail1.Clear();
            TextBoxTeachersLogin.Clear();
            PasswordBoxTeachers.Clear();
            CheckBoxAdminAccess.IsChecked = false;
        } 
        private void ButtonDelete_Click(object sender, RoutedEventArgs e) // Удаление преподавателя
        {
            DeleteTeachers();
        }
        private async void DeleteTeachers() // Удаление преподавателя
        {
            if (ListViewTeachers.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListViewTeachers.SelectedItem != null)
            {
                using zhirovContext db = new();
                Teacher? teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex]);

                if (teacher != null)
                {
                    db.Teachers.Remove(teacher);
                    await db.SaveChangesAsync();

                    ListViewTeachers.Items.Clear();
                    FillListBoxTeachers();
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
        #endregion

        #region Отправка данных
        private async void SendPasswordToUser() // Отправка сообщения на почту
        {
            try
            {
                string user = TextBoxTeachersFIO.Text;
                bool a = true;

                MailAddress from = new("mail@techno-review.ru", user);
                MailAddress to = new(TextBoxTeachersMail1.Text);
                MailMessage m = new(from, to);

                m.Subject = Title;
                m.Body = "Добро пожаловать в систему Электронный журнал\n\nАдминистратор зарегистрировал Вас в системе электронного журнала, ниже написаны данные для входа в вашу учетную запись.\nМы рекомендуем при первой возможности поменять пароль на более удобный Вам, так как нынешний пароль является временным." +
                    "\n\nЛогин: " + TextBoxTeachersLogin.Text + "\nПароль: " + PasswordBoxTeachers.Password;

                SmtpClient smtp = new("connect.smtp.bz", 2525);

                smtp.Credentials = new NetworkCredential("zhirowdaniil@gmail.com", "CB1W3lAeBwQ6");
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
        private void SendSMSToUser() // Отправка СМС сообщения (стоит дорого!!!)
        {
            string myApiKey = "B0361252-C8BA-5438-E643-4651FCC4E55B"; //Ваш API ключ
            SmsRu.SmsRu sms = new(myApiKey);
            sms.Send(TextBoxTeachersPhone1.Text, "Добро пожаловать в Электронный журнал\n\nЛогин: " + TextBoxTeachersLogin.Text + "\nПароль: " + PasswordBoxTeachers.Password);
        }
        #endregion

        private void ListBoxDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }

    public class DisciplineList
    {
        public string? DisciplineName { get; set; }
        public string? DisciplineIsChecked { get; set; }
    }
}
