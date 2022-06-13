using ElectroJournal.DataBase;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroJournal.Classes.View;

public partial class PieChartViewModel
{
    public PieChartViewModel()
    {
        Series = new ObservableCollection<ISeries> { };
        FillPie();
    }
    public ObservableCollection<ISeries> Series { get; set; }
    private void FillPie()
    {
        if (Series.Count == 15) return;
        using zhirovContext db = new();
        DateOnly dt = DateOnly.FromDateTime(DateTime.Now);
        var g = db.Groups.Where(g => g.TeachersIdteachers == Properties.Settings.Default.UserID).FirstOrDefault();

        if (g != null)
        {
            /*            var j5 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == dt.Day.ToString() && j.JournalScore == "5").ToList();
                        var j4 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == dt.Day.ToString() && j.JournalScore == "4").ToList();
                        var j3 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == dt.Day.ToString() && j.JournalScore == "3").ToList();
                        var j2 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == dt.Day.ToString() && j.JournalScore == "2").ToList();
                        var jn = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == dt.Day.ToString() && j.JournalScore == "н").ToList();*/
            var j5 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == "16" && j.JournalScore == "5").ToList();
            var j4 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == "16" && j.JournalScore == "4").ToList();
            var j3 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == "16" && j.JournalScore == "3").ToList();
            var j2 = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == "16" && j.JournalScore == "2").ToList();
            var jn = db.Journals.Where(j => j.StudentsIdstudentsNavigation.GroupsIdgroups == g.Idgroups && j.JournalDay == "16" && j.JournalScore == "н").ToList();
            Series.Add(
                    new PieSeries<ObservableValue>
                    {
                        Values = new[] { new ObservableValue(double.Parse(j5.Count.ToString())) },
                        Name = "Отлично",
                    });
            Series.Add(
                    new PieSeries<ObservableValue>
                    {
                        Values = new[] { new ObservableValue(double.Parse(j4.Count.ToString())) },
                        Name = "Хорошо",
                    });
            Series.Add(
                    new PieSeries<ObservableValue>
                    {
                        Values = new[] { new ObservableValue(double.Parse(j3.Count.ToString())) },
                        Name = "Удовлетворительно",
                    });
            Series.Add(
                    new PieSeries<ObservableValue>
                    {
                        Values = new[] { new ObservableValue(double.Parse(j2.Count.ToString())) },
                        Name = "Неудовлетворительно",
                    });
            Series.Add(
                    new PieSeries<ObservableValue>
                    {
                        Values = new[] { new ObservableValue(double.Parse(jn.Count.ToString())) },
                        Name = "Отсутствия",
                    });
        }
    }
}

