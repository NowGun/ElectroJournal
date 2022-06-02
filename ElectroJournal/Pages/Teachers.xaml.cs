using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace ElectroJournal.Pages
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
        }

        List<int> idTeachers = new();
        private int lastFoundIndex = -1;
        private string pass;

        private void SearchBoxDiscipline_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (ListViewTeachers.SelectedItems != null)
                FillListBoxDiscipline();
        }

        #region Заполнение ListBox
        private void ComboBoxSortingTeacher_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillListBoxTeachers(); // Сортировка листа
        private async void FillListBoxTeachers()
        {
            try
            {
                ListViewTeachers.Items.Clear();
                idTeachers.Clear();

                using zhirovContext db = new();

                if (String.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    switch (ComboBoxSortingTeacher.SelectedIndex)
                    {
                        case 0:
                            await db.Teachers.OrderBy(t => t.TeachersSurname).Where(t => t.TeachersName != "CardReaderService").ForEachAsync(t =>
                            {
                                ListViewTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                                idTeachers.Add((int)t.Idteachers);
                            });
                            break;
                        case 1:
                            await db.Teachers.OrderByDescending(t => t.TeachersSurname).Where(t => t.TeachersName != "CardReaderService").ForEachAsync(t =>
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
                        .Where(t => (EF.Functions.Like(t.TeachersName, $"%{SearchBox.Text}%") ||
                        EF.Functions.Like(t.TeachersSurname, $"%{SearchBox.Text}%") ||
                        EF.Functions.Like(t.TeachersPatronymic, $"%{SearchBox.Text}%")) && t.TeachersName != "CardReaderService")
                        .ForEachAsync(t =>
                        {
                            ListViewTeachers.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                            idTeachers.Add((int)t.Idteachers);
                        });
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBoxTeachers | {ex.Message}");
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
                        var c3 = await db.TeachersHasDisciplines.Where(c3 => c3.TeachersIdteachers == idTeachers[ListViewTeachers.SelectedIndex] && c3.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == c.DisciplinesNameAbbreviated).FirstOrDefaultAsync();

                        if (c3 != null)
                        {
                            co1.Add(new DisciplineList
                            {
                                DisciplineName = c.DisciplinesNameAbbreviated,
                                DisciplineIsChecked = "True"
                            });
                        }
                        else
                        {
                            co1.Add(new DisciplineList
                            {
                                DisciplineName = c.DisciplinesNameAbbreviated
                            });
                        }
                    }
                }
                else
                {
                    var c2 = await db.Disciplines.ToListAsync();

                    foreach (var c in c2)
                    {
                        var c3 = await db.TeachersHasDisciplines.Where(c3 => c3.TeachersIdteachers == idTeachers[ListViewTeachers.SelectedIndex] && c3.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == c.DisciplinesNameAbbreviated).FirstOrDefaultAsync();

                        if (c3 != null)
                        {
                            co1.Add(new DisciplineList
                            {
                                DisciplineName = c.DisciplinesNameAbbreviated,
                                DisciplineIsChecked = "True"
                            });
                        }
                        else
                        {
                            co1.Add(new DisciplineList
                            {
                                DisciplineName = c.DisciplinesNameAbbreviated
                            });
                        }
                    }
                }

                ListBoxDiscipline.ItemsSource = co1;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBoxDiscipline | {ex.Message}");
            }
        }
        private async void FillListBoxGroups() // Заполнение листа с группами
        {
            try
            {
                ObservableCollection<GroupsList> co1 = new();
                using zhirovContext db = new();

                if (!String.IsNullOrWhiteSpace(SearchBoxDiscipline.Text))
                {
                    var c2 = await db.Groups.Where(c => EF.Functions.Like(c.GroupsNameAbbreviated, $"%{SearchBoxGroups.Text}%") || EF.Functions.Like(c.GroupsName, $"%{SearchBoxGroups.Text}%")).ToListAsync();

                    foreach (var c in c2)
                    {
                        var c3 = await db.TeachersHasGroups.Where(c3 => c3.TeachersIdteachers == idTeachers[ListViewTeachers.SelectedIndex] && c3.GroupsIdgroupsNavigation.GroupsNameAbbreviated == c.GroupsNameAbbreviated).FirstOrDefaultAsync();

                        if (c3 != null)
                        {
                            co1.Add(new GroupsList
                            {
                                GroupsName = c.GroupsNameAbbreviated,
                                GroupsIsChecked = "True"
                            });
                        }
                        else
                        {
                            co1.Add(new GroupsList
                            {
                                GroupsName = c.GroupsNameAbbreviated
                            });
                        }
                    }
                }
                else
                {
                    var c2 = await db.Groups.ToListAsync();

                    foreach (var c in c2)
                    {
                        var c3 = await db.TeachersHasGroups.Where(c3 => c3.TeachersIdteachers == idTeachers[ListViewTeachers.SelectedIndex] && c3.GroupsIdgroupsNavigation.GroupsNameAbbreviated == c.GroupsNameAbbreviated).FirstOrDefaultAsync();

                        if (c3 != null)
                        {
                            co1.Add(new GroupsList
                            {
                                GroupsName = c.GroupsNameAbbreviated,
                                GroupsIsChecked = "True"
                            });
                        }
                        else
                        {
                            co1.Add(new GroupsList
                            {
                                GroupsName = c.GroupsNameAbbreviated
                            });
                        }
                    }
                }

                ListBoxGroups.ItemsSource = co1;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBoxGroups | {ex.Message}");
            }
        }
        #endregion

        #region Взаимодействие с данными
        private async void ListViewTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e) // Подгрузка данных определенного преподавателя
        {
            try
            {
                ButtonDelete.IsEnabled = true;

                if (ListViewTeachers.SelectedItem != null)
                {
                    using zhirovContext db = new();
                    var t = await db.Teachers.Where(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex]).FirstOrDefaultAsync();

                    string FIO = t.TeachersSurname + " " + t.TeachersName + " " + t.TeachersPatronymic;

                    TextBoxTeachersFIO.Text = FIO;
                    CheckBoxAdminAccess.IsChecked = bool.Parse(t.TeachersAccesAdminPanel);
                    TextBoxTeachersMail.Text = t.TeachersMail;
                    TextBoxTeachersPhone.Text = t.TeachersPhone;

                    FillListBoxDiscipline();
                    FillListBoxGroups();
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListViewTeachers_SelectionChanged | {ex.Message}");
            }
        }
        private void TextBoxTeachersFIO_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) // Генерация логина и пароля
        {
            if (ListViewTeachers.SelectedItem == null)
            {
                pass = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(12);
            }
        }
        private void ListViewTeachers_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) // Удаление преподавателя
        {
            if (e.Key == Key.Delete)
            {
                DeleteTeachers();
            }
        }
        private void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) => FillListBoxTeachers(); // Поиск преподавателя
        private async void ButtonSave_Click(object sender, RoutedEventArgs e) // Редактирование/добавление преподавателя
        {
            try
            {
                ProgressBarTeachers.Visibility = Visibility.Visible;

                if (!string.IsNullOrWhiteSpace(TextBoxTeachersFIO.Text) && !string.IsNullOrWhiteSpace(TextBoxTeachersMail.Text))
                {
                    string cond = @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)";

                    if (Regex.IsMatch(TextBoxTeachersMail.Text, cond))
                    {
                        if (TextBoxTeachersFIO.Text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                        {
                            using zhirovContext db = new();

                            string[] FIO = TextBoxTeachersFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var mail = await db.Teachers.Where(p => p.TeachersMail == TextBoxTeachersMail.Text).ToListAsync();
                            var mail2 = await db.Teachers.Where(p => p.Idteachers == Properties.Settings.Default.UserID && p.TeachersMail == TextBoxTeachersMail.Text).FirstOrDefaultAsync();

                            if (ListViewTeachers.SelectedItem != null)
                            {
                                Teacher? teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex]);

                                if (teacher != null)
                                {
                                    if (mail.Count == 0 || mail2 != null)
                                    {
                                        teacher.TeachersName = FIO[1];
                                        teacher.TeachersSurname = FIO[0];
                                        teacher.TeachersPatronymic = FIO[2];
                                        teacher.TeachersAccesAdminPanel = CheckBoxAdminAccess.IsChecked.ToString();
                                        teacher.TeachersPhone = TextBoxTeachersPhone.Text;
                                        teacher.TeachersMail = TextBoxTeachersMail.Text;

                                        await db.SaveChangesAsync();
                                    }
                                    else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Почта занята");
                                }
                            }
                            else
                            {
                                Teacher teacher = new()
                                {
                                    TeachersName = FIO[1],
                                    TeachersSurname = FIO[0],
                                    TeachersPatronymic = FIO[2],
                                    TeachersPassword = SettingsControl.Hash(pass),
                                    TeachersMail = TextBoxTeachersMail.Text,
                                    TeachersPhone = TextBoxTeachersPhone.Text,
                                    TeachersAccesAdminPanel = CheckBoxAdminAccess.IsChecked.ToString()
                                };

                                if (mail.Count == 0)
                                {
                                    await db.Teachers.AddAsync(teacher);
                                    await db.SaveChangesAsync();

                                    SendPasswordToUser();
                                    //SendSMSToUser();
                                }
                                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данная почта занята");
                            }
                            ClearValue();
                            FillListBoxTeachers();
                        }
                        else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Поле ФИО должно быть в формате: Фамилия - Имя - Отчество");
                    }
                    else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Почта в неверном формате");
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");

                ProgressBarTeachers.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonSave_Click(teachers) | {ex.Message}");
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => ClearValue(); // Добавление преподавателя
        private void ClearValue()
        {
            ListViewTeachers.SelectedItem = null;
            TextBoxTeachersFIO.Clear();
            TextBoxTeachersPhone.Clear();
            TextBoxTeachersMail.Clear();
            CheckBoxAdminAccess.IsChecked = false;
            ObservableCollection<DisciplineList> co1 = new();
            ObservableCollection<GroupsList> co2 = new();
            ListBoxDiscipline.ItemsSource = co1;
            ListBoxGroups.ItemsSource = co2;
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e) => DeleteTeachers(); // Удаление преподавателя
        private async void DeleteTeachers() // Удаление преподавателя
        {
            try
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
                        TextBoxTeachersPhone.Clear();
                        TextBoxTeachersMail.Clear();
                        CheckBoxAdminAccess.IsChecked = false;
                        ButtonDelete.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"DeleteTeachers | {ex.Message}");
            }            
        }
        private async void CheckBoxDiscipline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = sender as System.Windows.Controls.CheckBox;
                using zhirovContext db = new();

                if ((bool)cb.IsChecked)
                {
                    var disp = await db.Disciplines.Where(d => d.DisciplinesNameAbbreviated == cb.Content.ToString()).FirstOrDefaultAsync();

                    TeachersHasDiscipline teachersHasDiscipline = new()
                    {
                        TeachersIdteachers = (uint)idTeachers[ListViewTeachers.SelectedIndex],
                        DisciplinesIddisciplines = disp.Iddisciplines
                    };

                    if (teachersHasDiscipline != null)
                    {
                        await db.TeachersHasDisciplines.AddAsync(teachersHasDiscipline);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    TeachersHasDiscipline td = await db.TeachersHasDisciplines.Where(td => td.TeachersIdteachers == idTeachers[ListViewTeachers.SelectedIndex] && td.DisciplinesIddisciplinesNavigation.DisciplinesNameAbbreviated == cb.Content.ToString()).FirstOrDefaultAsync();

                    if (td != null)
                    {
                        db.TeachersHasDisciplines.Remove(td);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"CheckBoxDiscipline_Click | {ex.Message}");
            }            
        }
        private void SearchBoxGroups_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) => FillListBoxGroups();
        private async void CheckBoxGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cb = sender as System.Windows.Controls.CheckBox;
                using zhirovContext db = new();

                if ((bool)cb.IsChecked)
                {
                    var disp = await db.Groups.Where(d => d.GroupsNameAbbreviated == cb.Content.ToString()).FirstOrDefaultAsync();

                    TeachersHasGroup teachersHasGroup = new()
                    {
                        TeachersIdteachers = (uint)idTeachers[ListViewTeachers.SelectedIndex],
                        GroupsIdgroups = disp.Idgroups
                    };

                    if (teachersHasGroup != null)
                    {
                        await db.TeachersHasGroups.AddAsync(teachersHasGroup);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    TeachersHasGroup td = await db.TeachersHasGroups.Where(td => td.TeachersIdteachers == idTeachers[ListViewTeachers.SelectedIndex] && td.GroupsIdgroupsNavigation.GroupsNameAbbreviated == cb.Content.ToString()).FirstOrDefaultAsync();

                    if (td != null)
                    {
                        db.TeachersHasGroups.Remove(td);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"CheckBoxGroups_Click | {ex.Message}");
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
                MailAddress to = new(TextBoxTeachersMail.Text);
                MailMessage m = new(from, to);

                m.Subject = "ElectroJournal";
                m.Body = "Добро пожаловать в систему Электронный журнал\n\nАдминистратор зарегистрировал Вас в системе электронного журнала, ниже написаны данные для входа в вашу учетную запись.\nМы рекомендуем при первой возможности поменять пароль на более удобный Вам, так как нынешний пароль является временным." +
                    "\n\nЛогин: " + TextBoxTeachersMail.Text + "\nПароль: " + pass + "        \n\n\n";

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
                }
                else if (!a)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "письмо не отправилось");
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"SendPasswordToUser | {ex.Message}");
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Введен некорректный Email");
            }
        }
        private void SendSMSToUser() // Отправка СМС сообщения (стоит дорого!!!)
        {
            string myApiKey = "B0361252-C8BA-5438-E643-4651FCC4E55B"; //Ваш API ключ
            SmsRu.SmsRu sms = new(myApiKey);
            sms.Send(TextBoxTeachersPhone.Text, "Добро пожаловать в Электронный журнал\n\nЛогин: " + TextBoxTeachersMail.Text + "\nПароль: " + pass);
        }
        #endregion
    }

    public class DisciplineList
    {
        public string? DisciplineName { get; set; }
        public string? DisciplineIsChecked { get; set; }
    }
    public class GroupsList
    {
        public string? GroupsName { get; set; }
        public string? GroupsIsChecked { get; set; }
    }
}
