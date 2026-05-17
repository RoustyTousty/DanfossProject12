using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class ConfigViewModel : ViewModelBase
{
    private AssetService _assetService;
    private OptimizationService _optimizationService;

    public ObservableCollection<ProductionUnit> ActiveUnits = [];
    public ObservableCollection<ProductionUnit> DisabledUnits = [];

    [ObservableProperty]
    private int maintenanceHours = 30;

    public override string Title => "Configuration";
    public override Bitmap Icon => LoadAsset("config_icon.png");

    public ConfigViewModel(AssetService assetService, OptimizationService optimizationService)
    {
        _assetService = assetService;
        _optimizationService = optimizationService;
    }
}