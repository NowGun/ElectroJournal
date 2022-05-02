using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

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
            ComboBoxGroups.SelectedIndex = 0;
        }

        List<int> idStudents = new List<int>();
        List<int> idGroups = new List<int>();

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DataBaseControls DbControls = new DataBaseControls();
            ProgressBar.Visibility = Visibility.Visible;

            if (!string.IsNullOrWhiteSpace(TextBoxStudentsFIO.Text))
            {
                if (TextBoxStudentsFIO.Text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                {
                    string[] FIO = TextBoxStudentsFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ListBoxStudents.SelectedItem != null)
                    {
                        using (zhirovContext db = new zhirovContext())
                        {
                            Student? student = await db.Students.FirstOrDefaultAsync(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]);

                            if (student != null)
                            {
                                student.StudentsName = FIO[1];
                                student.StudentsSurname = FIO[0];
                                student.StudentsPatronymic = FIO[2];
                                student.StudentsParentPhone = TextBoxParentPhone.Text;
                                student.StudentsPhone = TextBoxStudentsPhone.Text;
                                student.StudentsParent = TextBoxParentFIO.Text;
                                student.StudentsDormitory = CheckBoxStudentsDormitory.IsChecked.ToString();
                                student.StudentsResidence = TextBoxStudentsResidence.Text;
                                student.StudentsBirthday = DatePickerDateBirthday.SelectedDate.Value;
                                student.GroupsIdgroups = (uint)idGroups[ListBoxGroups.SelectedIndex];

                                await db.SaveChangesAsync();

                                ListBoxStudentsRefresh();
                                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                            }
                        }
                    }
                    else
                    {
                        using (zhirovContext db = new zhirovContext())
                        {
                            Student students = new Student
                            {
                                StudentsName = FIO[1],
                                StudentsSurname = FIO[0],
                                StudentsPatronymic = FIO[2],
                                StudentsParentPhone = TextBoxParentPhone.Text,
                                StudentsPhone = TextBoxStudentsPhone.Text,
                                StudentsParent = TextBoxParentFIO.Text,
                                StudentsDormitory = CheckBoxStudentsDormitory.IsChecked.ToString(),
                                StudentsResidence = TextBoxStudentsResidence.Text,
                                StudentsBirthday = DatePickerDateBirthday.SelectedDate.Value,
                                GroupsIdgroups = (uint)idGroups[ListBoxGroups.SelectedIndex]
                            };

                            await db.Students.AddAsync(students);
                            await db.SaveChangesAsync();

                            ListBoxStudentsRefresh();
                            TextBoxParentFIO.Clear();
                            TextBoxParentPhone.Clear();
                            TextBoxStudentsFIO.Clear();
                            TextBoxStudentsPhone.Clear();
                            TextBoxStudentsResidence.Clear();
                            DatePickerDateBirthday.Text = null;
                            CheckBoxStudentsDormitory.IsChecked = false;

                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
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

            ProgressBar.Visibility = Visibility.Hidden;
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
            ProgressBarListBox.Visibility = Visibility.Visible;

            using (zhirovContext db = new zhirovContext())
            {
                switch (ComboBoxSorting.SelectedIndex)
                {
                    case 0:
                        await db.Students.OrderBy(t => t.StudentsSurname).ForEachAsync(t =>
                        {
                            ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                            idStudents.Add((int)t.Idstudents);
                        });
                        break;
                    case 1:
                        await db.Students.OrderByDescending(t => t.StudentsSurname).ForEachAsync(t =>
                        {
                            ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                            idStudents.Add((int)t.Idstudents);
                        });
                        break;
                }
            }
            ProgressBarListBox.Visibility = Visibility.Hidden;
        }
        private void TextBoxStudentsPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
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

            if (ListBoxStudents.SelectedItem != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
                    var students = await db.Students.Where(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]).ToListAsync();

                    foreach (var t in students)
                    {
                        string FIO = t.StudentsSurname + " " + t.StudentsName + " " + t.StudentsPatronymic;
                        int indexGroup = idGroups.IndexOf((int)t.GroupsIdgroups);

                        TextBoxStudentsFIO.Text = FIO;
                        DatePickerDateBirthday.SelectedDate = t.StudentsBirthday;
                        TextBoxStudentsResidence.Text = t.StudentsResidence;
                        CheckBoxStudentsDormitory.IsChecked = bool.Parse(t.StudentsDormitory);
                        TextBoxParentFIO.Text = t.StudentsParent;
                        TextBoxStudentsPhone.Text = t.StudentsPhone;
                        TextBoxParentPhone.Text = t.StudentsParentPhone;
                        ListBoxGroups.SelectedIndex = indexGroup;
                    }
                }
            }
        }
        private void ComboBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxStudentsRefresh();
        }
        private async void DeleteStudent()
        {
            if (ListBoxStudents.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListBoxStudents.SelectedItem != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
                    Student? student = await db.Students.FirstOrDefaultAsync(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]);

                    if (student != null)
                    {
                        db.Students.Remove(student);
                        await db.SaveChangesAsync();

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
            }
        }
        private async void FillComboBoxStudents()
        {
            ComboBoxGroups.Items.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Courses.OrderBy(t => t.CourseName).ForEachAsync(t =>
                {
                    ComboBoxGroups.Items.Add(t.CourseName);
                });
            }
        }
        private async void FillListBox()
        {
            ListBoxGroups.Items.Clear();
            idGroups.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Groups.Where(p => p.CourseIdcourseNavigation.CourseName == ComboBoxGroups.SelectedItem.ToString()).ForEachAsync(p =>
                {
                    ListBoxGroups.Items.Add(p.GroupsNameAbbreviated);
                    idGroups.Add((int)p.Idgroups);
                });
            }
        }
        private void ComboBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListBox();
            ListBoxGroups.Visibility = Visibility.Visible;
        }
    }
}