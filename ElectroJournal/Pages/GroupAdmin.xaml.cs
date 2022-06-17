using ElectroJournal.DataBase;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
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
        public GroupAdmin()
        {
            InitializeComponent();
            StartTimer();
        }

        List<int> idStud = new();
        public System.Windows.Threading.DispatcherTimer timer2 = new();

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
        private void MenuItemReportScores_Click(object sender, RoutedEventArgs e) => RootDialogDiapDate.Show();
        private void RootDialogNewSchedule_ButtonRightClick(object sender, RoutedEventArgs e) => RootDialogDiapDate.Hide();
        private void RootDialogNewSchedule_ButtonLeftClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(DatePickerStart.Text) && !string.IsNullOrWhiteSpace(DatePickerEnd.Text))
                {
                    RootDialogDiapDate.Hide();
                    StackPanelGenerateSheet.Visibility = Visibility.Visible;
                }
                else ((MainWindow)Application.Current.MainWindow).Notifications("Ошибка", "Заполните все поля");
                ((Storyboard)Resources["AnimOpenInfo"]).Begin();
                FrameStud.Navigate(new SheetReport(LabelGroupName.Content.ToString(), DateOnly.Parse(DatePickerStart.Text), DateOnly.Parse(DatePickerEnd.Text)));
            }
            catch (Exception ex)
            {

            }
        }
        private async void FillListBoxPresences(object sender, EventArgs e)
        {
            try
            {
                ListBoxPresences.Items.Clear();
                DateOnly dt = DateOnly.FromDateTime(DateTime.Now);
                using zhirovContext db = new();

                var p = await db.Presences
                    .Where(p => p.Student.GroupsIdgroupsNavigation.GroupsNameAbbreviated == LabelGroupName.Content
                    && DateOnly.FromDateTime(p.PresenceDatetime) == dt)
                    .Include(p => p.Student)
                    .OrderByDescending(p => p.PresenceDatetime)
                    .ToListAsync();

                foreach (var a in p)
                {
                    ListBoxPresences.Items.Add($"{a.Student.StudentsSurname} {a.Student.StudentsName} пришел в {a.PresenceDatetime.ToShortTimeString()}");
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void StartTimer()
        {
            timer2.Tick += new EventHandler(FillListBoxPresences);
            timer2.Interval = new TimeSpan(0, 0, 5);
            timer2.Start();
        }
    }
}