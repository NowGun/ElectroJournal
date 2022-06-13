using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
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

public partial class BarsChartViewModel
{
    private int _index = 0;
    private readonly ObservableCollection<ObservablePoint> _observableValues;

    public BarsChartViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). // mark
        _observableValues = new ObservableCollection<ObservablePoint>
        {
            // Use the ObservableValue or ObservablePoint types to let the chart listen for property changes // mark
            // or use any INotifyPropertyChanged implementation // mark
            new ObservablePoint(_index++, 2),
            new(_index++, 5), // the ObservablePoint type is redundant and inferred by the compiler (C# 9 and above)
            new(_index++, 4),
            new(_index++, 5),
            new(_index++, 2),
            new(_index++, 6),
            new(_index++, 6),
            new(_index++, 6),
            new(_index++, 4),
            new(_index++, 2),
            new(_index++, 3),
            new(_index++, 8),
            new(_index++, 3)
        };

        Series = new ObservableCollection<ISeries>
        {
            new ColumnSeries<ObservablePoint>
            {
                Values = _observableValues
            }
        };

        // in the following sample notice that the type int does not implement INotifyPropertyChanged
        // and our Series.Values property is of type List<T>
        // List<T> does not implement INotifyCollectionChanged
        // this means the following series is not listening for changes.
        // Series.Add(new ColumnSeries<int> { Values = new List<int> { 2, 4, 6, 1, 7, -2 } }); // mark
    }

    public ObservableCollection<ISeries> Series { get; set; }

    private Axis[] _xAxes = { new Axis { SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220)) } };
    private Axis[] _yAxes = { new Axis { IsVisible = false } };
}

