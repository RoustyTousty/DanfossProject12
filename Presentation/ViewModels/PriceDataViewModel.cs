using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace HeatOptimization.Presentation.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class PriceDataViewModel : ViewModelBase 
{
    public override string Title => "Price Data";
    public override Bitmap Icon => LoadAsset("price-icon.png");
    private readonly AssetService _assetService;
    private readonly ChartService _chartService;

    [ObservableProperty]
    private ISeries[] series = [];
    [ObservableProperty]
    private Axis[] xAxes = [];

    [ObservableProperty]
    private Axis[] yAxes = [];


    public async Task LoadAsync()
    {
        var unitSeries = _chartService.BuildUnitSeries();

        

        Series = unitSeries.Select(unit =>
            new StackedColumnSeries<double>
            {
                Name = unit.Key,
                Values = unit.Value
            }
        ).ToArray();

        var labels = _assetService.ResultData
            .Select(r => r.HourlyData.TimeFrom.ToString("yyyy-MM-dd HH:mm"))
            .ToArray();

        XAxes = [ new Axis { Labels = labels } ];

        YAxes = [ new Axis { Name = "Energy (MWh)" } ];
                

    }
    
    public PriceDataViewModel(AssetService assetService, ChartService chartService)
    {
        _assetService = assetService;
        _chartService = chartService;

        _ = LoadAsync(); // TEMP TEST ONLY
    }
}