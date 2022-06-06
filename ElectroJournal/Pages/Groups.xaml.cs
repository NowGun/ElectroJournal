using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Groups.xaml
    /// </summary>
    public partial class Groups : Page
    {
        public Groups()
        {
            InitializeComponent();

            FillComboBoxCourse();
            FillComboBoxClassTeacher();
            FillComboBoxTypeLearning();
        }

        List<int> idTeachers = new List<int>();

        private async void ButtonGroupSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBar.Visibility = Visibility.Visible;

                if (!string.IsNullOrWhiteSpace(TextBoxGroupsName.Text) && ComboBoxClassTeacher.SelectedIndex != -1)
                {
                    using zhirovContext db = new();

                    if (ListBoxGroups.SelectedItem != null)
                    {
                        Group? groups = await db.Groups
                                .Include(p => p.TypelearningIdtypelearningNavigation)
                                .Include(p => p.CourseIdcourseNavigation)
                                .Include(p => p.TeachersIdteachersNavigation)
                                .FirstOrDefaultAsync(p => p.GroupsNameAbbreviated == ListBoxGroups.SelectedItem.ToString());

                        if (groups != null)
                        {
                            groups.GroupsName = TextBoxGroupsName.Text;
                            groups.GroupsNameAbbreviated = TextBoxGroupsNameAbbreviated.Text;
                            groups.GroupsPrefix = TextBoxGroupsPrefix.Text;
                            groups.CourseIdcourseNavigation.CourseName = ComboBoxCourse.SelectedItem.ToString();
                            groups.TypelearningIdtypelearningNavigation.TypelearningName = ComboBoxTypeLearning.SelectedItem.ToString();
                            groups.TeachersIdteachers = ComboBoxClassTeacher.SelectedIndex == 0 ? null : (uint)idTeachers[ComboBoxClassTeacher.SelectedIndex-1];

                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var t = await db.Typelearnings.FirstOrDefaultAsync(t => t.TypelearningName == ComboBoxTypeLearning.SelectedItem.ToString());
                        var c = await db.Courses.FirstOrDefaultAsync(c => c.CourseName == ComboBoxCourse.SelectedItem.ToString());

                        Group gr = new Group
                        {
                            GroupsName = TextBoxGroupsName.Text,
                            GroupsNameAbbreviated = TextBoxGroupsNameAbbreviated.Text,
                            GroupsPrefix = TextBoxGroupsPrefix.Text,
                            CourseIdcourse = c.Idcourse,
                            TypelearningIdtypelearning = t.Idtypelearning,
                            TeachersIdteachers = ComboBoxClassTeacher.SelectedIndex == 0 ? null : (uint)idTeachers[ComboBoxClassTeacher.SelectedIndex-1],
                        };

                        await db.Groups.AddAsync(gr);
                        await db.SaveChangesAsync();
                    }
                    ClearValue();
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");

                ProgressBar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonGroupSave_Click | {ex.Message}");
            }
        }
        private void ButtonGroupDelete_Click(object sender, RoutedEventArgs e) => DeleteGroup();
        private void ButonGroupAdd_Click(object sender, RoutedEventArgs e)
        {
            ListBoxGroups.SelectedItem = null;
            TextBoxGroupsName.Clear();
            TextBoxGroupsNameAbbreviated.Clear();
            TextBoxGroupsPrefix.Clear();
            FillComboBoxClassTeacher();
            ButtonGroupDelete.IsEnabled = false;
            ComboBoxTypeLearning.SelectedIndex = 0;
            ComboBoxClassTeacher.SelectedIndex = -1;
        }
        private async void FillListBoxGroups()
        {
            try
            {
                ListBoxGroups.Items.Clear();
                using zhirovContext db = new();

                if (String.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    if (ComboBoxGroupsSorting.SelectedIndex == 0) await db.Groups.OrderBy(t => t.GroupsNameAbbreviated).ForEachAsync(t => ListBoxGroups.Items.Add($"{t.GroupsNameAbbreviated}"));
                    else await db.Groups.OrderBy(t => t.GroupsNameAbbreviated).Where(t => t.CourseIdcourseNavigation.CourseName == ComboBoxGroupsSorting.SelectedItem.ToString()).ForEachAsync(t => ListBoxGroups.Items.Add($"{t.GroupsNameAbbreviated}"));
                }
                else await db.Groups.OrderBy(t => t.GroupsNameAbbreviated).Where(t => EF.Functions.Like(t.GroupsNameAbbreviated, $"%{SearchBox.Text}%") || EF.Functions.Like(t.GroupsName, $"%{SearchBox.Text}%")).ForEachAsync(t => ListBoxGroups.Items.Add($"{t.GroupsNameAbbreviated}"));
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBoxGroups | {ex.Message}");
            }
        }
        private async void FillComboBoxClassTeacher()
        {
            try
            {
                ComboBoxClassTeacher.Items.Clear();
                idTeachers.Clear();
                using zhirovContext db = new();
                ComboBoxClassTeacher.Items.Add("Отсутствует");
                var t2 = await db.Teachers.OrderBy(t => t.TeachersSurname).Where(t => t.TeachersName != "CardReaderService").ToListAsync();

                foreach (var t in t2)
                {
                    var g = await db.Groups.Where(g => g.TeachersIdteachers == t.Idteachers).FirstOrDefaultAsync();

                    if (g == null)
                    {
                        ComboBoxClassTeacher.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                        idTeachers.Add((int)t.Idteachers);
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBoxClassTeacher | {ex.Message}");
            }
        }
        private async void FillComboBoxClassTeacher(string fio)
        {
            try
            {
                ComboBoxClassTeacher.Items.Clear();
                idTeachers.Clear();
                using zhirovContext db = new();
                ComboBoxClassTeacher.Items.Add("Отсутствует");
                string[] fio2 = fio.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var t3 = await db.Teachers.Where(t => t.TeachersSurname == fio2[0]
                    && t.TeachersName == fio2[1]
                    && t.TeachersPatronymic == fio2[2]).FirstOrDefaultAsync();

                if (t3 != null)
                {
                    ComboBoxClassTeacher.Items.Add($"{t3.TeachersSurname} {t3.TeachersName} {t3.TeachersPatronymic}");
                    idTeachers.Add((int)t3.Idteachers);
                }

                var t2 = await db.Teachers.OrderBy(t => t.TeachersSurname).Where(t => t.TeachersName != "CardReaderService").ToListAsync();

                foreach (var t in t2)
                {
                    var g = await db.Groups.Where(g => g.TeachersIdteachers == t.Idteachers).FirstOrDefaultAsync();

                    if (g == null)
                    {
                        ComboBoxClassTeacher.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                        idTeachers.Add((int)t.Idteachers);
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBoxClassTeacher | {ex.Message}");
            }
        }
        private async void FillComboBoxTypeLearning()
        {
            try
            {
                ComboBoxTypeLearning.Items.Clear();

                using zhirovContext db = new();
                await db.Typelearnings.OrderByDescending(t => t.TypelearningName).ForEachAsync(t => ComboBoxTypeLearning.Items.Add(t.TypelearningName));
                ComboBoxTypeLearning.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBoxTypeLearning | {ex.Message}");
            }
        }
        private async void ListBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ButtonGroupDelete.IsEnabled = true;

                if (ListBoxGroups.SelectedItem != null)
                {
                    ButtonGroupDelete.IsEnabled = true;

                    using zhirovContext db = new();
                    var t = await db.Groups.Where(p => p.GroupsNameAbbreviated == ListBoxGroups.SelectedItem.ToString())
                            .Include(p => p.TypelearningIdtypelearningNavigation)
                            .Include(p => p.CourseIdcourseNavigation)
                            .Include(p => p.TeachersIdteachersNavigation)
                            .FirstOrDefaultAsync();

                    if (t != null)
                    {
                        TextBoxGroupsName.Text = t.GroupsName;
                        TextBoxGroupsNameAbbreviated.Text = t.GroupsNameAbbreviated;
                        TextBoxGroupsPrefix.Text = t.GroupsPrefix;
                        ComboBoxTypeLearning.SelectedItem = t.TypelearningIdtypelearningNavigation.TypelearningName;
                        ComboBoxCourse.SelectedItem = t.CourseIdcourseNavigation.CourseName;
                        if (t.TeachersIdteachers != null) FillComboBoxClassTeacher($"{t.TeachersIdteachersNavigation.TeachersSurname} {t.TeachersIdteachersNavigation.TeachersName} {t.TeachersIdteachersNavigation.TeachersPatronymic}");
                        else FillComboBoxClassTeacher();
                        ComboBoxClassTeacher.SelectedItem = t.TeachersIdteachers == null ? ComboBoxClassTeacher.SelectedIndex = 0 : $"{t.TeachersIdteachersNavigation.TeachersSurname} {t.TeachersIdteachersNavigation.TeachersName} {t.TeachersIdteachersNavigation.TeachersPatronymic}";
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxGroups_SelectionChanged | {ex.Message}");
            }
        }
        private void ListBoxGroups_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteGroup();
            }
        }
        private async void DeleteGroup()
        {
            try
            {
                if (ListBoxGroups.Items.Count == 0)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
                }
                else if (ListBoxGroups.SelectedItem != null)
                {
                    using zhirovContext db = new();
                    Group? groups = await db.Groups.FirstOrDefaultAsync(p => p.GroupsNameAbbreviated == ListBoxGroups.SelectedItem.ToString());

                    if (groups != null)
                    {
                        db.Groups.Remove(groups);
                        await db.SaveChangesAsync();
                        ClearValue();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"DeleteGroup | {ex.Message}");
            }
        }
        private void ComboBoxGroupsSorting_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillListBoxGroups();
        private async void FillComboBoxCourse()
        {
            try
            {
                ComboBoxCourse.Items.Clear();
                ComboBoxGroupsSorting.Items.Clear();
                ComboBoxGroupsSorting.Items.Add("Все курсы");
                using zhirovContext db = new();
                await db.Courses.OrderBy(t => t.CourseName).ForEachAsync(t =>
                {
                    ComboBoxCourse.Items.Add(t.CourseName);
                    ComboBoxGroupsSorting.Items.Add(t.CourseName);
                });
                ComboBoxGroupsSorting.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBoxCourse | {ex.Message}");
            }
        }
        private void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) => FillListBoxGroups();
        private async void SaveData()
        {
            try
            {
                if (ListBoxGroups.SelectedItem != null)
                {
                    ProgressBar.Visibility = Visibility.Visible;
                    if (!string.IsNullOrWhiteSpace(TextBoxGroupsName.Text) && ComboBoxClassTeacher.SelectedIndex != -1)
                    {
                        using zhirovContext db = new();
                        Group? groups = await db.Groups
                                .Include(p => p.TypelearningIdtypelearningNavigation)
                                .Include(p => p.CourseIdcourseNavigation)
                                .Include(p => p.TeachersIdteachersNavigation)
                                .FirstOrDefaultAsync(p => p.GroupsNameAbbreviated == ListBoxGroups.SelectedItem.ToString());

                        if (groups != null)
                        {
                            groups.GroupsName = TextBoxGroupsName.Text;
                            groups.GroupsNameAbbreviated = TextBoxGroupsNameAbbreviated.Text;
                            groups.GroupsPrefix = TextBoxGroupsPrefix.Text;
                            groups.CourseIdcourseNavigation.CourseName = ComboBoxCourse.SelectedItem.ToString();
                            groups.TypelearningIdtypelearningNavigation.TypelearningName = ComboBoxTypeLearning.SelectedItem.ToString();
                            groups.TeachersIdteachers = ComboBoxClassTeacher.SelectedIndex == 0 ? null : (uint)idTeachers[ComboBoxClassTeacher.SelectedIndex];

                            await db.SaveChangesAsync();
                            ClearValue();
                        }
                    }
                }
                ProgressBar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonGroupSave_Click | {ex.Message}");
            }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => SaveData();
        private void Page_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.D)
            {
                ClearValue();
                TextBoxGroupsName.Focus();
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SearchBox.Clear();
                SearchBox.Focus();
            }
        }
        private void ClearValue()
        {
            FillComboBoxClassTeacher();
            FillListBoxGroups();
            TextBoxGroupsName.Clear();
            TextBoxGroupsNameAbbreviated.Clear();
            TextBoxGroupsPrefix.Clear();
            ComboBoxTypeLearning.SelectedIndex = 0;
            ComboBoxClassTeacher.SelectedIndex = -1;
            ButtonGroupDelete.IsEnabled = false;
        }
    }
}
