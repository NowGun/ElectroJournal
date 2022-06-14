using ElectroJournal.DataBase;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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
        private async void FillDates()
        {
            using zhirovContext db = new();

            /* await Task.Run(() =>
             {
                 for (int i = 1; i <= 7; i++)
                 {
                     DateOnly date = DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(i)));
                     var g = db.Groups.Where(g => g.TeachersIdteachers == Properties.Settings.Default.UserID).FirstOrDefault();

                     var ss = db.Presences
                 .Where(x => DateOnly.FromDateTime(x.PresenceDatetime) == date
                  && x.Student.GroupsIdgroupsNavigation.GroupsNameAbbreviated == g.GroupsNameAbbreviated)
                 .Select(x => x.StudentId)
                 .Distinct()
                 .Count();

                     data.Add(new Data { Count = ss, Date = date });
                 }
             });

             

             db.Presences.Where(x => DateOnly.FromDateTime(x.PresenceDatetime) >= date).DistinctBy(x => x.StudentId)
                 //.Select(x => x.StudentId) // DateOnly.FromDateTime(x.PresenceDatetime)
                 .GroupBy(x => new Data { Date = DateOnly.FromDateTime(x.PresenceDatetime), Count = });

             var hui = (from presences in db.Presences 
                        where DateOnly.FromDateTime(presences.PresenceDatetime) >= date
                         select db.Presences.First(x => x.StudentId)
                        );*/

            var date = DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(7)));

            List<Data> data = db.RawSqlQuery<Data>(@$"select date(presence_datetime) as Date, count(distinct student_id) as Count
                                                    from presences 
                                                    group by date(presence_datetime)",
                                                    (reader) =>
                                                    {
                                                        return new Data
                                                        {
                                                            Date = reader.GetFieldValue<DateOnly>("Date"),
                                                            Count = reader.GetFieldValue<int>("Count")
                                                        };
                                                    });

            var filteredData = data.Where(x => x.Date >= date);

            foreach (var p in filteredData)
            {
                _observableValues.Add(new(p.Count));
            }
        }

        class Data
        {
            public DateOnly Date { get; set; }
            public int Count { get; set; }
        };

        

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


    public static class Helper
    {
        public static List<T> RawSqlQuery<T>(this zhirovContext ctx, string query, Func<DbDataReader, T> map)
        {
            using (var command = ctx.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                ctx.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<T>();

                    while (result.Read())
                    {
                        entities.Add(map(result));
                    }

                    return entities;
                }
            }
        }
    }
}
