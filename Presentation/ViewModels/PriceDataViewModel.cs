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
    public Axis[] XAxes { get; set; } = [];
    public Axis[] YAxes { get; set; } = [];


    public async Task LoadAsync()
    {
        var results = await _resultService.RunAndSaveAsync();

        var unitSeries = _chartService.BuildUnitSeries(results);

        Series = unitSeries.Select(unit =>
            new StackedColumnSeries<double>
            {
                Name = unit.Key,
                Values = unit.Value
            }
        ).ToArray();

        // 👇 HERE: build X axis from real data
        var labels = results
            .Select(r => r.HourlyData.TimeFrom.ToString("HH:mm"))
            .ToArray();

        XAxes = [
            new Axis {
                Labels = labels
            }
        ];
    }
    public PriceDataViewModel(ResultService resultService, ChartService chartService)
    {
        _resultService = resultService;
        _chartService = chartService;

        _ = LoadAsync(); // TEMP TEST ONLY
    }
}