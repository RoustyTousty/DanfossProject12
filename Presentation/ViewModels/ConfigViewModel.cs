using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using HeatOptimization.Logic;
using Avalonia.Platform.Storage;
using Avalonia.Interactivity;
using Avalonia.Controls;
using System.IO;

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
    private string unitToPutOnMaintenance = "";
    [ObservableProperty]
    private bool isPutOnMaintenance = false;
    [ObservableProperty]
    private int costToCO2Ratio = 5;

    [ObservableProperty]
    private string? _selectedFilePath;

    [ObservableProperty]
    private string fileButtonText = "Select";
    private List<IHourlyData> NewHourlyData = [];

    public bool IsFileSelected => !string.IsNullOrEmpty(SelectedFilePath);

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

    // command to select the file
    [RelayCommand]
    private async Task SelectFileAsync(){

        if (_filePickerService == null) return;

        string? localPath = await _filePickerService.Invoke();

        if (!string.IsNullOrEmpty(localPath))
        {
            SelectedFilePath = localPath; 

            NewHourlyData = _assetService.GetHourlyDatasWithoutUpdate(SelectedFilePath);

            FileButtonText = Path.GetFileName(localPath);
        }
    }

    // update isFileSelected whenever user selects a file in the UI
    partial void OnSelectedFilePathChanged(string? value)
    {
        OnPropertyChanged(nameof(IsFileSelected));
    }


    // command that moves active unit to inactive and vice versa
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

    [RelayCommand]
    public async Task Optimize(Window? window) {
        _assetService.HourlyData = new(NewHourlyData);
        if (IsPutOnMaintenance) {
            _optimizationService.Optimize(null, null);
        } else {
            _optimizationService.Optimize(UnitToPutOnMaintenance, MaintenanceHours);
        }
        window?.Close();
    }

}