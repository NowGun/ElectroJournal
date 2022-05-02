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
    /// Логика взаимодействия для Groups.xaml
    /// </summary>
    public partial class Groups : Page
    {
        public Groups()
        {
            InitializeComponent();

            //FillListBoxGroups();
            FillComboBoxCourse();
            FillComboBoxClassTeacher();
            FillComboBoxTypeLearning();
        }

        List<int> idGroups = new List<int>();
        List<int> idTeachers = new List<int>();
        List<int> idTypeLearning = new List<int>();

        private async void ButtonGroupSave_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;

            if (!string.IsNullOrWhiteSpace(TextBoxGroupsName.Text) && ComboBoxClassTeacher.SelectedItem != null)
            {
                if (ListBoxGroups.SelectedItem != null)
                {
                    using (zhirovContext db = new zhirovContext())
                    {
                        Group? groups = await db.Groups
                            .Include(p => p.TypelearningIdtypelearningNavigation)
                            .Include(p => p.CourseIdcourseNavigation)
                            .Include(p => p.TeachersIdteachersNavigation)
                            .FirstOrDefaultAsync(p => p.Idgroups == idGroups[ListBoxGroups.SelectedIndex]);

                        if (groups != null)
                        {
                            groups.GroupsName = TextBoxGroupsName.Text;
                            groups.GroupsNameAbbreviated = TextBoxGroupsNameAbbreviated.Text;
                            groups.GroupsPrefix = TextBoxGroupsPrefix.Text;
                            groups.CourseIdcourse = (uint)ComboBoxCourse.SelectedIndex + 1;
                            groups.TypelearningIdtypelearning = (uint)idTypeLearning[ComboBoxTypeLearning.SelectedIndex];
                            groups.TeachersIdteachers = (uint)idTeachers[ComboBoxClassTeacher.SelectedIndex];

                            await db.SaveChangesAsync();

                            FillListBoxGroups();

                            TextBoxGroupsName.Clear();
                            TextBoxGroupsNameAbbreviated.Clear();
                            TextBoxGroupsPrefix.Clear();
                            ComboBoxTypeLearning.SelectedIndex = 0;
                            ComboBoxClassTeacher.SelectedItem = null;
                            ComboBoxCourse.SelectedIndex = 0;

                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                        }
                    }
                }
                else
                {
                    using (zhirovContext db = new zhirovContext())
                    {

                        Group gr = new Group
                        {
                            GroupsName = TextBoxGroupsName.Text,
                            GroupsNameAbbreviated = TextBoxGroupsNameAbbreviated.Text,
                            GroupsPrefix = TextBoxGroupsPrefix.Text,
                            CourseIdcourse = (uint)ComboBoxCourse.SelectedIndex + 1,
                            TypelearningIdtypelearning = (uint)idTypeLearning[ComboBoxTypeLearning.SelectedIndex],
                            TeachersIdteachers = (uint)idTeachers[ComboBoxClassTeacher.SelectedIndex],
                        };


                        await db.Groups.AddAsync(gr);
                        await db.SaveChangesAsync();

                        FillListBoxGroups();

                        TextBoxGroupsName.Clear();
                        TextBoxGroupsNameAbbreviated.Clear();
                        TextBoxGroupsPrefix.Clear();
                        ComboBoxTypeLearning.SelectedIndex = 0;
                        ComboBoxClassTeacher.SelectedItem = null;
                        ComboBoxCourse.SelectedIndex = 0;

                        ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                    }
                }
            }
            else
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");
            }

            ProgressBar.Visibility = Visibility.Hidden;
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
            ComboBoxCourse.SelectedIndex = 0;
        }
        private async void FillListBoxGroups()
        {
            ListBoxGroups.Items.Clear();
            idGroups.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                switch (ComboBoxGroupsSorting.SelectedIndex)
                {
                    case 0:
                        await db.Groups.OrderBy(t => t.GroupsNameAbbreviated).ForEachAsync(t =>
                        {
                            ListBoxGroups.Items.Add($"{t.GroupsNameAbbreviated}");
                            idGroups.Add((int)t.Idgroups);
                        });
                        break;
                    case 1:

                        break;
                }
            }

            /*
            if (ComboBoxGroupsSorting.SelectedIndex == 0) command.CommandText = ("SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` ORDER BY `groups_name`");
            else if (ComboBoxGroupsSorting.SelectedIndex == 1) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 1 ORDER BY `groups_name`";
            else if (ComboBoxGroupsSorting.SelectedIndex == 2) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 2 ORDER BY `groups_name`";
            else if (ComboBoxGroupsSorting.SelectedIndex == 3) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 3 ORDER BY `groups_name`";
            else if (ComboBoxGroupsSorting.SelectedIndex == 4) command.CommandText = "SELECT `idgroups`, `groups_name_abbreviated` FROM `groups` WHERE `groups_course` = 4 ORDER BY `groups_name`";
        */
        }
        private async void FillComboBoxClassTeacher()
        {
            ComboBoxClassTeacher.Items.Clear();
            idTeachers.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Teachers.OrderBy(t => t.TeachersSurname).ForEachAsync(t =>
                {
                    ComboBoxClassTeacher.Items.Add($"{t.TeachersSurname} {t.TeachersName} {t.TeachersPatronymic}");
                    idTeachers.Add((int)t.Idteachers);
                });
            }
        }
        private async void FillComboBoxTypeLearning()
        {
            idTypeLearning.Clear();
            ComboBoxTypeLearning.Items.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Typelearnings.OrderBy(t => t.TypelearningName).ForEachAsync(t =>
                {
                    ComboBoxTypeLearning.Items.Add(t.TypelearningName);
                    idTypeLearning.Add((int)t.Idtypelearning);
                });

                ComboBoxTypeLearning.SelectedIndex = 0;
            }
        }
        private async void ListBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonGroupDelete.IsEnabled = true;

            if (ListBoxGroups.SelectedItem != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
                    var t = await db.Groups.Where(p => p.Idgroups == idGroups[ListBoxGroups.SelectedIndex])
                        .Include(p => p.TypelearningIdtypelearningNavigation)
                        .Include(p => p.CourseIdcourseNavigation)
                        .Include(p => p.TeachersIdteachersNavigation)
                        .FirstOrDefaultAsync();

                    string fio = $"{t.TeachersIdteachersNavigation.TeachersSurname} {t.TeachersIdteachersNavigation.TeachersName} {t.TeachersIdteachersNavigation.TeachersPatronymic}";

                    TextBoxGroupsName.Text = t.GroupsName;
                    TextBoxGroupsNameAbbreviated.Text = t.GroupsNameAbbreviated;
                    TextBoxGroupsPrefix.Text = t.GroupsPrefix;
                    ComboBoxTypeLearning.SelectedItem = t.TypelearningIdtypelearningNavigation.TypelearningName;
                    ComboBoxCourse.SelectedItem = t.CourseIdcourseNavigation.CourseName;
                    ComboBoxClassTeacher.SelectedItem = fio;
                }
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
            if (ListBoxGroups.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListBoxGroups.SelectedItem != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
                    Group? groups = await db.Groups.FirstOrDefaultAsync(p => p.Idgroups == idGroups[ListBoxGroups.SelectedIndex]);

                    if (groups != null)
                    {
                        db.Groups.Remove(groups);
                        await db.SaveChangesAsync();

                        ListBoxGroups.Items.Clear();
                        TextBoxGroupsName.Clear();
                        TextBoxGroupsNameAbbreviated.Clear();
                        TextBoxGroupsPrefix.Clear();
                        ComboBoxTypeLearning.SelectedIndex = 0;
                        ComboBoxClassTeacher.SelectedItem = null;
                        ComboBoxCourse.SelectedItem = null;
                        FillListBoxGroups();
                    }
                }
            }
        }
        private void ComboBoxGroupsSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListBoxGroups();
        }
        private async void FillComboBoxCourse()
        {
            ComboBoxCourse.Items.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Courses.OrderBy(t => t.CourseName).ForEachAsync(t =>
                {
                    ComboBoxCourse.Items.Add(t.CourseName);
                });
            }
        }
    }
}
