using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace HeatOptimization.Presentation.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class PriceDataViewModel : ViewModelBase 
{
    public override string Title => "Price Data";
    public override Bitmap Icon => LoadAsset("price-icon.png");
    private readonly ResultService _resultService;
    private readonly ChartService _chartService;

    [ObservableProperty]
    private ISeries[] series = [];
    [ObservableProperty]
    private Axis[] xAxes = [];

    [ObservableProperty]
    private Axis[] yAxes = [];


    public async Task LoadAsync()
    {
        (var results, var costImpact) = await _resultService.RunAndSaveAsync();

        Console.WriteLine("Received data from resultService");

        var unitSeries = _chartService.BuildUnitSeries(results);

        

        Series = unitSeries.Select(unit =>
            new StackedColumnSeries<double>
            {
                Name = unit.Key,
                Values = unit.Value
            }
        ).ToArray();

        var labels = results
            .Select(r => r.HourlyData.TimeFrom.ToString("yyyy-MM-dd HH:mm"))
            .ToArray();

        XAxes = [ new Axis { Labels = labels } ];

        YAxes = [ new Axis { Name = "Energy (MWh)" } ];
                

    }
    
    public PriceDataViewModel(ResultService resultService, ChartService chartService)
    {
        _resultService = resultService;
        _chartService = chartService;

        _ = LoadAsync(); // TEMP TEST ONLY
    }
}