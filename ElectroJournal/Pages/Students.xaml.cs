using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace ElectroJournal.Pages
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
            try
            {
                ProgressBar.Visibility = Visibility.Visible;

                if (!string.IsNullOrWhiteSpace(TextBoxStudentsFIO.Text) && ListBoxGroups.SelectedIndex != -1)
                {
                    if (TextBoxStudentsFIO.Text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                    {
                        using zhirovContext db = new();
                        string[] FIO = TextBoxStudentsFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        DateTime? date = !String.IsNullOrEmpty(DatePickerDateBirthday.Text) ? DateTime.Parse(DatePickerDateBirthday.Text) : null;

                        if (ListBoxStudents.SelectedItem != null)
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
                                student.StudentsResidence = TextBoxStudentsResidence.Text;
                                student.StudentsBirthday = DatePickerDateBirthday.Text != null ? date : null;
                                student.GroupsIdgroups = (uint)idGroups[ListBoxGroups.SelectedIndex];

                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            Student students = new Student
                            {
                                StudentsName = FIO[1],
                                StudentsSurname = FIO[0],
                                StudentsPatronymic = FIO[2],
                                StudentsParentPhone = TextBoxParentPhone.Text,
                                StudentsPhone = TextBoxStudentsPhone.Text,
                                StudentsParent = TextBoxParentFIO.Text,
                                StudentsResidence = TextBoxStudentsResidence.Text,
                                StudentsBirthday = DatePickerDateBirthday.Text != null ? date : null,
                                GroupsIdgroups = (uint)idGroups[ListBoxGroups.SelectedIndex]
                            };

                            await db.Students.AddAsync(students);
                            await db.SaveChangesAsync();
                        }
                        ClearValue();
                    }
                    else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Поле ФИО должно быть в формате: Фамилия - Имя - Отчество");
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");

                ProgressBar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonSave_Click(students) | {ex.Message}");
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e) 
        {
            ListBoxStudents.Items.Clear();
            TextBoxParentFIO.Clear();
            TextBoxParentPhone.Clear();
            TextBoxStudentsFIO.Clear();
            TextBoxStudentsPhone.Clear();
            TextBoxStudentsResidence.Clear();
            DatePickerDateBirthday.Text = null;
            ListBoxStudents.SelectedIndex = -1;
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e) => DeleteStudent();
        private async void ListBoxStudentsRefresh()
        {
            try
            {
                ListBoxStudents.Items.Clear();
                idStudents.Clear();
                ProgressBarListBox.Visibility = Visibility.Visible;

                using zhirovContext db = new();

                if (String.IsNullOrWhiteSpace(SearchBox.Text))
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
                else
                {
                    await db.Students
                        .OrderBy(t => t.StudentsSurname)
                        .Where(t => EF.Functions.Like(t.StudentsSurname, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.StudentsName, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.StudentsPatronymic, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.GroupsIdgroupsNavigation.GroupsName, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.GroupsIdgroupsNavigation.GroupsNameAbbreviated, $"%{SearchBox.Text}%"))
                        .ForEachAsync(t =>
                        {
                            ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                            idStudents.Add((int)t.Idstudents);
                        });
                }

                ProgressBarListBox.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxStudentsRefresh | {ex.Message}");
            }
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
            try
            {
                ButtonDelete.IsEnabled = true;

                if (ListBoxStudents.SelectedItem != null)
                {
                    using zhirovContext db = new();

                    var t = await db.Students.Where(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]).Include(p => p.GroupsIdgroupsNavigation.CourseIdcourseNavigation).FirstOrDefaultAsync();

                    if (t != null)
                    {
                        TextBoxStudentsFIO.Text = t.StudentsSurname + " " + t.StudentsName + " " + t.StudentsPatronymic;
                        DatePickerDateBirthday.SelectedDate = t.StudentsBirthday;
                        TextBoxStudentsResidence.Text = t.StudentsResidence;
                        TextBoxParentFIO.Text = t.StudentsParent;
                        TextBoxStudentsPhone.Text = t.StudentsPhone;
                        TextBoxParentPhone.Text = t.StudentsParentPhone;
                        ComboBoxGroups.SelectedItem = t.GroupsIdgroups == null ? null : t.GroupsIdgroupsNavigation.CourseIdcourseNavigation.CourseName;
                        ListBoxGroups.SelectedItem = t.GroupsIdgroups == null ? null : t.GroupsIdgroupsNavigation.GroupsNameAbbreviated;
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxStudents_SelectionChanged | {ex.Message}");
            }
        }
        private void ComboBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e) => ListBoxStudentsRefresh();
        private async void DeleteStudent()
        {
            try
            {
                if (ListBoxStudents.Items.Count == 0)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
                }
                else if (ListBoxStudents.SelectedItem != null)
                {
                    using zhirovContext db = new();

                    Student? student = await db.Students.FirstOrDefaultAsync(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]);

                    if (student != null)
                    {
                        db.Students.Remove(student);
                        await db.SaveChangesAsync();
                        ClearValue();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"DeleteStudent | {ex.Message}");
            }
        }
        private async void FillComboBoxStudents()
        {
            try
            {
                ComboBoxGroups.Items.Clear();

                using zhirovContext db = new();
                await db.Courses.OrderBy(t => t.CourseName).ForEachAsync(t =>
                {
                    ComboBoxGroups.Items.Add(t.CourseName);
                });
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBoxStudents | {ex.Message}");
            }
        }
        private async void FillListBox()
        {
            try
            {
                ListBoxGroups.Items.Clear();
                idGroups.Clear();

                using zhirovContext db = new();
                await db.Groups.Where(p => p.CourseIdcourseNavigation.CourseName == ComboBoxGroups.SelectedItem.ToString()).ForEachAsync(p =>
                {
                    ListBoxGroups.Items.Add(p.GroupsNameAbbreviated);
                    idGroups.Add((int)p.Idgroups);
                });
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBox | {ex.Message}");
            }
        }
        private void ComboBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListBox();
            ListBoxGroups.Visibility = Visibility.Visible;
        }
        private void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) => ListBoxStudentsRefresh();
        private void ButtonChangeNumber_Click(object sender, RoutedEventArgs e) => RootDialog.Show();
        private void RootDialog_ButtonRightClick(object sender, RoutedEventArgs e) => RootDialog.Hide();
        private void Page_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.D)
            {
                ClearValue();
                TextBoxStudentsFIO.Focus();
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SearchBox.Clear();
                SearchBox.Focus();
            }
        }
        private void ClearValue()
        {
            ListBoxStudents.Items.Clear();
            ListBoxStudentsRefresh();
            TextBoxParentFIO.Clear();
            TextBoxParentPhone.Clear();
            TextBoxStudentsFIO.Clear();
            TextBoxStudentsPhone.Clear();
            TextBoxStudentsResidence.Clear();
            DatePickerDateBirthday.Text = null;
            ListBoxStudents.SelectedIndex = -1;
        }
        private void RootDialog_ButtonLeftClick(object sender, RoutedEventArgs e)
        {

        }
    }
}