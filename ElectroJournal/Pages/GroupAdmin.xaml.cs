﻿using ElectroJournal.DataBase;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для GroupAdmin.xaml
    /// </summary>
    public partial class GroupAdmin : Page
    {
        private double _lastLecture;
        private double _trend;

        public GroupAdmin()
        {
            InitializeComponent();
        }

        List<int> idStud = new();

        private async void FillListBoxStud()
        {
            try
            {
                ListBoxStudent.Items.Clear();
                using zhirovContext db = new();

                if (String.IsNullOrWhiteSpace(SearchBoxStudents.Text))
                {
                    await db.Students
                        .Where(s => s.GroupsIdgroupsNavigation.TeachersIdteachers == Properties.Settings.Default.UserID)
                        .OrderBy(s => s.StudentsSurname)
                        .ForEachAsync(s => 
                        {
                            ListBoxStudent.Items.Add($"{s.StudentsSurname} {s.StudentsName}");
                            idStud.Add((int)s.Idstudents);
                        });
                }
                else
                {
                    await db.Students
                        .Where(s => s.GroupsIdgroupsNavigation.TeachersIdteachers == Properties.Settings.Default.UserID
                        && (EF.Functions.Like(s.StudentsSurname, $"%{SearchBoxStudents.Text}%")
                        || EF.Functions.Like(s.StudentsName, $"%{SearchBoxStudents.Text}%")
                        || EF.Functions.Like(s.StudentsPatronymic, $"%{SearchBoxStudents.Text}%")))
                        .OrderBy(s => s.StudentsSurname)
                        .ForEachAsync(s => 
                        {
                            ListBoxStudent.Items.Add($"{s.StudentsSurname} {s.StudentsName}");
                            idStud.Add((int)s.Idstudents);
                        });
                }

                ((Storyboard)Resources["AnimCloseLoad"]).Begin();
            }
            catch (Exception ex)
            {

            }
        }
        private async void FillLabelName()
        {
            try
            {
                using zhirovContext db = new();

                var gr = await db.Groups.FirstOrDefaultAsync(g => g.TeachersIdteachers == Properties.Settings.Default.UserID);
                LabelGroupName.Content = gr != null ? gr.GroupsNameAbbreviated : "Произошла ошибка";
            }
            catch (Exception ex)
            {

            }
        }
        private void SearchBoxStudents_PreviewKeyUp(object sender, KeyEventArgs e) => FillListBoxStud();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GridStudentInfo.Visibility = Visibility.Collapsed;
            stackPanel.Visibility = Visibility.Visible;

            grid.Opacity = 0;
            grid1.Opacity = 0;

            FillLabelName();
            FillListBoxStud();
        }
        private void CardActionOpenStatsStud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListBoxStudent.SelectedIndex != -1)
                {
                    ((Storyboard)Resources["AnimOpenInfo"]).Begin();
                    LabelNameStud.Content = LabelFIOStudent.Content;
                    FrameStud.Navigate(new StudentInfo(idStud[ListBoxStudent.SelectedIndex]));
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void ListBoxStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListBoxStudent.SelectedIndex != -1)
                {
                    CardActionOpenStatsStud.Visibility = Visibility.Visible;

                    using zhirovContext db = new();
                    var s = await db.Students.FirstOrDefaultAsync(s => s.Idstudents == idStud[ListBoxStudent.SelectedIndex]);
                    LabelFIOStudent.Content = s != null ? $"{s.StudentsSurname} {s.StudentsName} {s.StudentsPatronymic}" : "Произошла ошибка";
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void SymbolIconBack_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)Resources["AnimCloseInfo"]).Begin();
        }
    }
}