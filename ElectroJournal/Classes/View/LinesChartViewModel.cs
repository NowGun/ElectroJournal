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
                Labels = new string[] {""},
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
