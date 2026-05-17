using Avalonia.Media;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class PriceDataViewModel : ViewModelBase 
{
    public override string Title => "Price Data";
    public override Bitmap Icon => LoadAsset("price-icon.png");
    private readonly ResultService _resultService;
    private readonly ChartService _chartService;
    private readonly List<IResultData> _allResults = new();

    private readonly Dictionary<string, string> _unitColorMap = new()
    {
        ["GM1"] = "#3FA9F5",
        ["GB1"] = "#FF8BB5",
        ["GB2"] = "#C6F0A0",
        ["GB3"] = "#61D8F0",
        ["EB1"] = "#9F92F8",
        ["EB2"] = "#9C4AFF",
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

    public async Task LoadAsync()
    {
        (var results, var costImpact) = await _resultService.RunAndSaveAsync();
        results = results.OrderBy(r => r.HourlyData.TimeFrom).ToList();
            

        _allResults.Clear();
        _allResults.AddRange(results);

        DateFrom = results.FirstOrDefault() is { } first
            ? new DateTimeOffset(first.HourlyData.TimeFrom)
            : null;
        DateTo = results.LastOrDefault() is { } last
            ? new DateTimeOffset(last.HourlyData.TimeTo)
            : null;

        Console.WriteLine("Received data from resultService");

        var totalHeat = results.Sum(r => r.UnitProduction.Sum(u => u.heatProduced));
        var totalCO2Value = results.Sum(r => r.CO2ProductionKG);
        var totalCostValue = results.Sum(r => r.UnitProduction.Sum(u => u.heatProduced * r.HourlyData.ElectricityPriceDKK));

        TotalHeatProduced = $"{totalHeat:N1} MW";
        TotalCO2 = $"{totalCO2Value:N0} kg";
        TotalCost = $"{totalCostValue:N0} DKK";
        MaintenanceCost = "N/A";

        ApplyDateRange();
    }

    public void ApplyDateRange()
    {
        var filteredResults = _allResults.Where(r =>
            (!DateFrom.HasValue || r.HourlyData.TimeFrom.Date >= DateFrom.Value.Date) &&
            (!DateTo.HasValue || r.HourlyData.TimeFrom.Date <= DateTo.Value.Date)
        ).ToList();

        UpdateChartAndHourlyRows(filteredResults);
    }

    public void ResetDateRange()
    {
        if (_allResults.Any())
        {
            DateFrom = new DateTimeOffset(_allResults.First().HourlyData.TimeFrom);
            DateTo = new DateTimeOffset(_allResults.Last().HourlyData.TimeTo);
        }
        else
        {
            DateFrom = null;
            DateTo = null;
        }

        ApplyDateRange();
    }

    private void UpdateChartAndHourlyRows(List<IResultData> filteredResults)
    {
        var unitSeries = _chartService.BuildUnitSeries(filteredResults);

        

        Series = unitSeries.Select(unit =>
            new StackedColumnSeries<double>
            {
                Name = unit.Key,
                Values = unit.Value
            }
        ).ToArray();

        var labels = filteredResults.Select(r => r.HourlyData.TimeFrom.ToString("yyyy-MM-dd HH:mm")).ToArray();
        XAxes = new Axis[] { new Axis { Labels = labels } };
        YAxes = new Axis[] { new Axis { Name = "Energy (MWh)" } };

        DateRange = filteredResults.Any()
            ? $"{filteredResults.First().HourlyData.TimeFrom:dd.MM.yyyy} - {filteredResults.Last().HourlyData.TimeTo:dd.MM.yyyy}"
            : string.Empty;

        var totalHeat = filteredResults.Sum(r => r.UnitProduction.Sum(u => u.heatProduced));
        var totalCO2Value = filteredResults.Sum(r => r.CO2ProductionKG);
        var totalCostValue = filteredResults.Sum(r => r.UnitProduction.Sum(u => u.heatProduced * r.HourlyData.ElectricityPriceDKK));

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

    public PriceDataViewModel(ResultService resultService, ChartService chartService)
    {
        _resultService = resultService;
        _chartService = chartService;

        _ = LoadAsync(); // TEMP TEST ONLY
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
