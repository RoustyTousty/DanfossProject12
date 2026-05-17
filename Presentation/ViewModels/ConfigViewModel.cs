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
    private readonly Func<Task<string?>>? _filePickerService;

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

    [ObservableProperty]
    private string? _selectedFilePath;

    [ObservableProperty]
    private string fileButtonText = "Select";


    public override string Title => "Configuration";
    public override Bitmap Icon => LoadAsset("config_icon.png");

    public ConfigViewModel(
        AssetService assetService, 
        OptimizationService optimizationService, 
        Func<Task<string?>>? filePickerService = null){

        _assetService = assetService;
        _optimizationService = optimizationService;
        _filePickerService = filePickerService;

        var units = _assetService.GetProductionUnits();
        if (units != null){

            foreach (var unit in units){
                ActiveUnits.Add(unit);
            }
        }
    }

    [RelayCommand]
    private async Task SelectFileAsync(){

        if (_filePickerService == null) return;

        string? localPath = await _filePickerService.Invoke();

        if (!string.IsNullOrEmpty(localPath))
        {
            SelectedFilePath = localPath; 

            _assetService.UpdateHourlyDatas(SelectedFilePath);
            FileButtonText = Path.GetFileName(localPath);
        }
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