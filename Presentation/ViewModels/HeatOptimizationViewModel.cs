using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using HeatOptimization.Logic;
using HeatOptimization.Presentation;

namespace HeatOptimization.Presentation.ViewModels;

public partial class HeatOptimizationViewModel : ViewModelBase
{
    private readonly AssetManager _assetManager;
    private readonly OptimizationService _optimizationService;
    private readonly ChartService _chartService;

    public override string Title => "Dashboard";
    public override Bitmap Icon => LoadAsset("opti-icon.png");

    public ObservableCollection<BoilerItemViewModel> Boilers { get; } = new();

    public ObservableCollection<string> UnitOptions { get; } = new() { "MW", "kW" };
    public ObservableCollection<string> CurrencyOptions { get; } = new() { "DKK", "EUR", "USD" };

    private string _selectedUnitOption = "MW";
    public string SelectedUnitOption
    {
        get => _selectedUnitOption;
        set
        {
            if (_selectedUnitOption == value) return;
            _selectedUnitOption = value;
            UpdateDisplayValues();
        }
    }

    private string _selectedCurrencyOption = "DKK";
    public string SelectedCurrencyOption
    {
        get => _selectedCurrencyOption;
        set
        {
            if (_selectedCurrencyOption == value) return;
            _selectedCurrencyOption = value;
            UpdateDisplayValues();
        }
    }

    private readonly Dictionary<string, double> _currencyRates = new()
    {
        { "DKK", 1.0 },
        { "EUR", 0.134 },
        { "USD", 0.15 }
    };

    public HeatOptimizationViewModel(AssetManager assetManager, OptimizationService optimizationService, ChartService chartService)
    {
        _assetManager = assetManager;
        _optimizationService = optimizationService;
        _chartService = chartService; 
        var units = _assetManager.GetProductionUnits();

        List<IResultData> results = new();
        try
        {
            results = _optimizationService.Optimize();
        }
        catch
        {
           
        }

        foreach (var unit in units)
        {
            double totalProduced = 0;
            if (results?.Count > 0)
            {
                totalProduced = results.Sum(r => r.UnitProduction.FirstOrDefault(u => u.unitName == unit.Name)?.heatProduced ?? 0);
            }

            Boilers.Add(new BoilerItemViewModel
            {
                Name = unit.Name,
                Type = unit.Type.ToString(),
                MaxHeatMW = unit.MaxHeatMW,
                MaxElectricityMW = unit.MaxElectricityMW,
                BaseProductionCostDKK = unit.BaseProductionCostDKK,
                CO2EmissionsKg = unit.CO2EmissionsKg,
                GasConsumptionMWh = unit.GasConsumptionMWh,
                OilConsumptionMWh = unit.OilConsumptionMWh,
                TotalHeatProduced = totalProduced
            });
        }

        UpdateDisplayValues();
    }

    private void UpdateDisplayValues()
    {
        foreach (var b in Boilers)
        {
            if (SelectedUnitOption == "MW")
            {
                b.DisplayMaxHeat = string.Format("{0:F2} MW", b.MaxHeatMW);
            }
            else
            {
                b.DisplayMaxHeat = string.Format("{0:F0} kW", b.MaxHeatMW * 1000.0);
            }

            double rate = _currencyRates.ContainsKey(SelectedCurrencyOption) ? _currencyRates[SelectedCurrencyOption] : 1.0;
            double converted = b.BaseProductionCostDKK * rate;
            b.DisplayBaseProductionCost = string.Format("{0:F2} {1}", converted, SelectedCurrencyOption);
        }
        OnPropertyChanged(nameof(Boilers));
    }

}
