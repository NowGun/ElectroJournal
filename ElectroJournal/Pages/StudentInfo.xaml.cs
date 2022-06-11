using ElectroJournal.DataBase;
using LiveCharts;
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
    /// Логика взаимодействия для StudentInfo.xaml
    /// </summary>
    public partial class StudentInfo : Page
    {
        public StudentInfo(int? idStud)
        {
            InitializeComponent();

            LoadInfo(idStud);

            PointLabel = chartPoint =>
               string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            
        }
        private async void LoadInfo(int? idStud)
        {
            try
            {
                if (idStud != null)
                {
                    using zhirovContext db = new();

                    var s = await db.Students
                        .Include(s => s.GroupsIdgroupsNavigation)
                        .FirstOrDefaultAsync(s => s.Idstudents == idStud);
                    var smart = await db.Smartcards.FirstOrDefaultAsync(s => s.StudentId == idStud);

                    if (s != null)
                    {
                        LabelNameStud.Content = $"{s.StudentsSurname} {s.StudentsName} {s.StudentsPatronymic}";
                        LabelGroup.Content = $"Студент группы {s.GroupsIdgroupsNavigation.GroupsNameAbbreviated}";
                        LabelDateBirth.Content = string.IsNullOrWhiteSpace(s.StudentsBirthday.ToString()) ? "Дата рождения: информация отсутствует" : $"Дата рождения: {s.StudentsBirthday}";
                        LabelPhone.Content = string.IsNullOrWhiteSpace(s.StudentsPhone) ? "Номер телефона: информация отсутствует" : $"Номер телефона: {s.StudentsPhone}";
                        LabelCardNum.Content = smart != null ? $"Номер карточки: {smart.SmartcardIdentifier}" : "Номер карточки: информация отсутствует";
                        LabelParent.Content = string.IsNullOrWhiteSpace(s.StudentsParent) ? "Родитель: информация отсутствует" : $"Родитель: {s.StudentsParent}";
                        LabelParentPhone.Content = string.IsNullOrWhiteSpace(s.StudentsParentPhone) ? "Номер телефона родителя: информация отсутствует" : $"Номер телефона родителя: {s.StudentsParentPhone}";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                ((Storyboard)Resources["AnimCloseLoading"]).Begin();
            }
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        public Func<ChartPoint, string> PointLabel { get; set; }


        private void FillPresencesChart()
        {
            try
            {
                
            }
            catch (Exception ex)
            {

            }
        }
    }
}
