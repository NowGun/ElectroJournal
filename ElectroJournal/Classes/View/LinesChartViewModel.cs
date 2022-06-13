using ElectroJournal.DataBase;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroJournal.Classes.View
{
    public partial class LinesChartViewModel
    {
        private readonly ObservableCollection<ObservableValue> _observableValues;

        public LinesChartViewModel()
        {
            _observableValues = new ObservableCollection<ObservableValue> {  };

            Series = new ObservableCollection<ISeries> 
            { 
                new LineSeries<ObservableValue>
                {
                    Values = _observableValues,
                }
            };

            FillDates();
        }

        public ObservableCollection<ISeries> Series { get; set; }
        private void FillDates()
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));

            using zhirovContext db = new();

            var g =  db.Groups.Where(g => g.TeachersIdteachers == Properties.Settings.Default.UserID).FirstOrDefault();

            var s = db.Presences
                .Where(s => DateOnly.FromDateTime(s.PresenceDatetime) >= date
                && s.Student.GroupsIdgroupsNavigation.GroupsNameAbbreviated == g.GroupsNameAbbreviated)
                .GroupBy(s => new { DateOnly = DateOnly.FromDateTime(s.PresenceDatetime), s.Student.GroupsIdgroupsNavigation.GroupsNameAbbreviated })
                .OrderBy(s => s.Key.DateOnly)
                .Select(s => new
                {
                    date = s.Key,
                    Count = s.Count()
                });


            foreach (var p in s)
            {
                var a = p.date;
                var sdf = p.Count;
                _observableValues.Add(new(sdf));
            }
        }
        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Labeler = (value) => "День " + value,
                MinStep = 1,
            }
        };
        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
               // Labeler = Labelers.Currency,
            }
        };
    }



}
