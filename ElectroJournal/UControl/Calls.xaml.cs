using ElectroJournal.DataBase;
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

namespace ElectroJournal.UControl
{
    /// <summary>
    /// Логика взаимодействия для Calls.xaml
    /// </summary>
    public partial class Calls : UserControl
    {
        public Calls()
        {
            InitializeComponent();
            //StartCalls();
        }

        public bool checkFillScheduleDB = true;
        public bool startAnim = true;

        public System.Windows.Threading.DispatcherTimer timer2 = new();

        List<string> ScheduleStart = new();
        List<string> ScheduleEnd = new();
        List<int> ScheduleNumber = new();

        private async void SheduleCall(object sender, EventArgs e)
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                LabelScheduleCall.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (checkFillScheduleDB)
                {
                    using zhirovContext db = new();
                    var time = await db.Periodclasses.ToListAsync();

                    foreach (var t in time)
                    {
                        ScheduleStart.Add(t.PeriodclassesStart.ToString());
                        ScheduleEnd.Add(t.PeriodclassesEnd.ToString());
                        ScheduleNumber.Add(t.PeriodclassesNumber);
                        checkFillScheduleDB = false;
                    }
                }

                var anim = (Storyboard)FindResource("AnimLabelScheduleCall");

                if (startAnim)
                {
                    anim.Begin();
                    startAnim = false;
                }

                for (int i = 0; i <= ScheduleStart.Count; i++)
                {
                    if (i != ScheduleStart.Count)
                    {
                        if (DateTime.Parse(ScheduleStart[i]) < DateTime.Now && DateTime.Now < DateTime.Parse(ScheduleEnd[i]))
                        {
                            LabelScheduleCall.Content = $"Урок: {ScheduleNumber[i]}    Период занятий: {ScheduleStart[i]} - {ScheduleEnd[i]}    До конца занятий: " +
                               (DateTime.Parse(ScheduleEnd[i]) - DateTime.Now).ToString("mm':'ss");
                            break;
                        }
                        else if (i != ScheduleStart.Count - 1 && DateTime.Parse(ScheduleStart[0]) < DateTime.Now && DateTime.Parse(ScheduleStart[i + 1]) > DateTime.Now && DateTime.Now < DateTime.Parse(ScheduleEnd[i]))
                        {
                            LabelScheduleCall.Content = "До конца перемены: " + (DateTime.Parse(ScheduleStart[i]) - DateTime.Now).ToString("mm':'ss");
                            break;
                        }

                        if ((DateTime.Parse(ScheduleStart[i]) - DateTime.Now).ToString("mm':'ss") == "00:00" || (DateTime.Parse(ScheduleEnd[i]) - DateTime.Now).ToString("mm':'ss") == "00:00") anim.Begin();
                    }
                    else if (i == ScheduleEnd.Count)
                    {
                        LabelScheduleCall.Content = "";
                    }
                }
            }
        }
        public void StartTimer()
        {
            timer2.Tick += new EventHandler(SheduleCall);
            timer2.Interval = new TimeSpan(0, 0, 1);
            timer2.Start();
        }
        public void StopTimer() => timer2.Stop();
    }
}
