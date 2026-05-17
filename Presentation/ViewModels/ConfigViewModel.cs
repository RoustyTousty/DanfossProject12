using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class ConfigViewModel : ViewModelBase
{
    private AssetService _assetService;
    private OptimizationService _optimizationService;

    public ObservableCollection<ProductionUnit> ActiveUnits { get; set; } = [];

public ObservableCollection<ProductionUnit> DisabledUnits { get; set; } = [];

    [ObservableProperty]
    private int maintenanceHours = 30;
    [ObservableProperty]
    private ProductionUnit? unitToputOnMaintenance;
    [ObservableProperty]
    private bool isPutOnMaintenance = false;
    [ObservableProperty]
    private int costToCO2Ratio = 5;

    public override string Title => "Configuration";
    public override Bitmap Icon => LoadAsset("config_icon.png");

    public ConfigViewModel(AssetService assetService, OptimizationService optimizationService)
    {
        _assetService = assetService;
        List<ProductionUnit> units = _assetService.GetProductionUnits();
        foreach (ProductionUnit unit in units) {
            ActiveUnits.Add(unit);
        }
        _optimizationService = optimizationService;
    }

    [RelayCommand]
    public void ToggleUnitStatus(ProductionUnit unit)
    {
        if (ActiveUnits.Contains(unit))
        {
            ActiveUnits.Remove(unit);
            DisabledUnits.Add(unit);
        }
        else if (DisabledUnits.Contains(unit))
        {
            DisabledUnits.Remove(unit);
            ActiveUnits.Add(unit);
        }
    }
}