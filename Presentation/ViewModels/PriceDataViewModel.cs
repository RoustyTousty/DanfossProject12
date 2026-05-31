using Avalonia.Media;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class PriceDataViewModel : ViewModelBase 
{
    public override string Title => "Price Data";
    public override Bitmap Icon => LoadAsset("price-icon.png");
    private readonly AssetService _assetService;
    private readonly ChartService _chartService;
    private readonly ResultService _resultService;

    [ObservableProperty]
    private Func<Task<string?>>? saveFilePickerService;
    [ObservableProperty]
    private bool hasResults = false;

    private readonly Dictionary<string, string> _unitColorMap = new()
    {
        ["GM1"] = "#3FA9F5",
        ["GB1"] = "#fd4388",
        ["GB2"] = "#7adb26",
        ["GB3"] = "#e2e616",
        ["EB1"] = "#861654",
        ["EB2"] = "#6f06f0",
        ["EB3"] = "#00C2A3",
        ["EB4"] = "#F48C2A",
    };

    public ObservableCollection<UnitDataRow> UnitRows { get; } = new();
    public ObservableCollection<HourlyDataRow> HourlyRows { get; } = new();
    public ObservableCollection<HourlyDataRow> FilteredHourlyRows { get; } = new();

    [ObservableProperty]
    private DateTimeOffset? dateFrom;

    [ObservableProperty]
    private DateTimeOffset? dateTo;

    [ObservableProperty]
    private ISeries[] series = [];

    [ObservableProperty]
    private Axis[] xAxes = [];

    [ObservableProperty]
    private Axis[] yAxes = [];

    [ObservableProperty]
    private string dateRange = string.Empty;

    [ObservableProperty]
    private string hourlySearch = string.Empty;

    [ObservableProperty]
    private string totalCO2 = "VALUE";

    [ObservableProperty]
    private string totalCost = "VALUE";

    [ObservableProperty]
    private string maintenanceCost = "VALUE";

    [ObservableProperty]
    private string totalHeatProduced = "VALUE";

    public Task LoadAsync()
    {
        List<IResultData> results = _assetService.ResultData.Cast<IResultData>().OrderBy(r => r.HourlyData.TimeFrom).ToList();
        

        DateFrom = results.FirstOrDefault() is { } first
            ? new DateTimeOffset(first.HourlyData.TimeFrom)
            : null;
        DateTo = results.LastOrDefault() is { } last
            ? new DateTimeOffset(last.HourlyData.TimeTo)
            : null;

        Console.WriteLine("Received data from resultService");

        var totalHeat = results.Sum(r => r.UnitProduction.Sum(u => u.heatProduced));
        var totalCO2Value = results.Sum(r => r.CO2ProductionKG);
        var totalCostValue = results.Sum(r => r.TotalPrice);

        TotalHeatProduced = $"{totalHeat:N1} MW";
        TotalCO2 = $"{totalCO2Value:N0} kg";
        TotalCost = $"{totalCostValue:N0} DKK";
        MaintenanceCost = $"{Math.Round(_assetService.CostImpact, 0)} DKK";

        ApplyDateRange();
        return Task.CompletedTask;
    }

    public async Task SaveResultsAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        await _resultService.SaveAsync(filePath);
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (SaveFilePickerService == null)
        {
            return;
        }

        var filePath = await SaveFilePickerService.Invoke();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        await SaveResultsAsync(filePath);
    }


    [RelayCommand]
    public void ApplyDateRange()
    {
        var filteredResults = _assetService.ResultData.Cast<IResultData>().Where(r =>
            (!DateFrom.HasValue || r.HourlyData.TimeFrom.Date >= DateFrom.Value.Date) &&
            (!DateTo.HasValue || r.HourlyData.TimeFrom.Date <= DateTo.Value.Date)
        ).ToList();

        UpdateChartAndHourlyRows(filteredResults);
        ClosePopup();
    }

    [RelayCommand]
    public void ResetDateRange()
    {
        if (_assetService.ResultData.Cast<IResultData>().Any())
        {
            DateFrom = new DateTimeOffset(_assetService.ResultData.Cast<IResultData>().First().HourlyData.TimeFrom);
            DateTo = new DateTimeOffset(_assetService.ResultData.Cast<IResultData>().Last().HourlyData.TimeTo);
        }
        else
        {
            DateFrom = null;
            DateTo = null;
        }

        ApplyDateRange();
    }

    public Action? ClosePopupAction { get; set; }

    private void ClosePopup()
    {
        ClosePopupAction?.Invoke();
    }

    private void UpdateChartAndHourlyRows(List<IResultData> filteredResults)
    {
        var unitSeries = _chartService.BuildUnitSeries(filteredResults);

        

        Series = unitSeries.Select(unit =>
        {
            var colorHex = _unitColorMap.TryGetValue(unit.Key, out var hex) ? hex : "#FFFFFF";

            var paint = new SolidColorPaint(SKColor.Parse(colorHex));

            return new StackedColumnSeries<double>
            {
                Name = unit.Key,
                Values = unit.Value,
                Fill = paint,
                Stroke = paint
            };
        }).ToArray();

        var labels = filteredResults.Select(r => r.HourlyData.TimeFrom.ToString("yyyy-MM-dd HH:mm")).ToArray();
        XAxes = [new Axis { Labels = labels }];
        YAxes = [new Axis { Name = "Energy (MWh)" }];

        DateRange = filteredResults.Any()
            ? $"{filteredResults.First().HourlyData.TimeFrom:dd.MM.yyyy} - {filteredResults.Last().HourlyData.TimeTo:dd.MM.yyyy}"
            : string.Empty;

        var totalHeat = filteredResults.Sum(r => r.UnitProduction.Sum(u => u.heatProduced));
        var totalCO2Value = filteredResults.Sum(r => r.CO2ProductionKG);
        var totalCostValue = filteredResults.Sum(r => r.TotalPrice);

        TotalHeatProduced = $"{totalHeat:N1} MW";
        TotalCO2 = $"{totalCO2Value:N0} kg";
        TotalCost = $"{totalCostValue:N0} DKK";

        var unitSummaries = filteredResults
            .SelectMany(r => r.UnitProduction.Select(u => new
            {
                u.unitName,
                u.heatProduced,
                Price = r.HourlyData.ElectricityPriceDKK
            }))
            .GroupBy(x => x.unitName)
            .Select(g => new UnitDataRow
            {
                Unit = g.Key,
                TotalHeat = g.Sum(x => x.heatProduced),
                HoursActive = g.Count(x => x.heatProduced > 0),
                TotalElectricity = g.Sum(x => x.heatProduced),
                TotalPrice = g.Sum(x => x.heatProduced * x.Price),
                Colour = _unitColorMap.TryGetValue(g.Key, out var colorHex)
                    ? new SolidColorBrush(Color.Parse(colorHex))
                    : Brushes.White
            })
            .OrderBy(r => r.Unit)
            .ToList();

        UnitRows.Clear();
        foreach (var row in unitSummaries)
        {
            UnitRows.Add(row);
        }

        HourlyRows.Clear();
        foreach (var result in filteredResults)
        {
            var primaryUnit = result.UnitProduction
                .OrderByDescending(u => u.heatProduced)
                .FirstOrDefault()?.unitName ?? string.Empty;

            var totalHeatForHour = result.UnitProduction.Sum(u => u.heatProduced);
            var productionDist = string.Join(", ", result.UnitProduction
                .Select(u => new { u.unitName, share = totalHeatForHour > 0 ? u.heatProduced / totalHeatForHour : 0 })
                .Select(x => $"{x.unitName}:{x.share:P0}"));

            var hourlyRow = new HourlyDataRow
            {
                From = result.HourlyData.TimeFrom.ToString("dd.MM.yyyy HH:mm"),
                To = result.HourlyData.TimeTo.ToString("dd.MM.yyyy HH:mm"),
                HeatDemand = result.HourlyData.HeatDemandMWh,
                Price = $"{result.HourlyData.ElectricityPriceDKK:N0}",
                Unit = primaryUnit,
                ProductionDistribution = productionDist,
                CO2Emission = result.CO2ProductionKG,
                ElectricityConsumption = totalHeatForHour
            };

            HourlyRows.Add(hourlyRow);
        }

        ApplyHourlyFilter(HourlySearch);
    }

    private void AssetService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AssetService.ResultData))
        {
            Console.WriteLine("ResultData changed → reloading chart");
            _ = LoadAsync();
            HasResults = _assetService.ResultData.Count > 0;
        }
        else if (e.PropertyName == nameof(AssetService.CostImpact))
        {
            Console.WriteLine("CostImpact changed → updating maintenance cost");
            MaintenanceCost = $"{Math.Round(_assetService.CostImpact, 0)} DKK";
        }
    }
    
    public PriceDataViewModel(AssetService assetService, ChartService chartService, ResultService resultService)
    {
        _assetService = assetService;
        _chartService = chartService;
        _resultService = resultService;

         _assetService.PropertyChanged += AssetService_PropertyChanged;
    }

    partial void OnHourlySearchChanged(string value)
    {
        ApplyHourlyFilter(value);
    }

    private void ApplyHourlyFilter(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            // Reset
            FilteredHourlyRows.Clear();
            foreach (var r in HourlyRows) FilteredHourlyRows.Add(r);
            return;
        }

        query = query.Trim();
        var qLower = query.ToLowerInvariant();

        var matches = HourlyRows.Where(r =>
            (r.From ?? string.Empty).ToLowerInvariant().Contains(qLower) ||
            (r.To ?? string.Empty).ToLowerInvariant().Contains(qLower) ||
            r.HeatDemand.ToString("N1").ToLowerInvariant().Contains(qLower) ||
            (r.Price ?? string.Empty).ToLowerInvariant().Contains(qLower) ||
            (r.Unit ?? string.Empty).ToLowerInvariant().Contains(qLower) ||
            (r.ProductionDistribution ?? string.Empty).ToLowerInvariant().Contains(qLower) ||
            r.CO2Emission.ToString("N0").ToLowerInvariant().Contains(qLower) ||
            r.ElectricityConsumption.ToString("N1").ToLowerInvariant().Contains(qLower)
        ).ToList();

        FilteredHourlyRows.Clear();
        foreach (var m in matches) FilteredHourlyRows.Add(m);
    }

    private string SerializeUnitProduction(List<UnitProduction> units)
    {
        if (units == null || !units.Any())
        {
            return string.Empty;
        }

        return string.Join("|", units.Select(u => $"{u.unitName}:{u.heatProduced}"));
    }
}
public sealed class UnitDataRow
{
    public string Unit { get; set; } = string.Empty;
    public double TotalHeat { get; set; }
    public int HoursActive { get; set; }
    public double TotalElectricity { get; set; }
    public double TotalPrice { get; set; }
    public IBrush Colour { get; set; } = Brushes.White;
}

public sealed class HourlyDataRow
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public double HeatDemand { get; set; }
    public string Price { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string ProductionDistribution { get; set; } = string.Empty;
    public double CO2Emission { get; set; }
    public double ElectricityConsumption { get; set; }
}
