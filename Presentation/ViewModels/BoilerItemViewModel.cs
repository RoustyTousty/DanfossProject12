namespace HeatOptimization.Presentation.ViewModels;

using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

public class BoilerItemViewModel : ObservableObject
{
    private string _name = "";
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    private string _type = "";
    public string Type { get => _type; set => SetProperty(ref _type, value); }

    private double _maxHeatMW;
    public double MaxHeatMW { get => _maxHeatMW; set => SetProperty(ref _maxHeatMW, value); }

    private double? _maxElectricityMW;
    public double? MaxElectricityMW { get => _maxElectricityMW; set => SetProperty(ref _maxElectricityMW, value); }

    private int _baseProductionCostDKK;
    public int BaseProductionCostDKK { get => _baseProductionCostDKK; set => SetProperty(ref _baseProductionCostDKK, value); }

    private int? _co2EmissionsKg;
    public int? CO2EmissionsKg { get => _co2EmissionsKg; set => SetProperty(ref _co2EmissionsKg, value); }

    private double? _gasConsumptionMWh;
    public double? GasConsumptionMWh { get => _gasConsumptionMWh; set => SetProperty(ref _gasConsumptionMWh, value); }

    private double? _oilConsumptionMWh;
    public double? OilConsumptionMWh { get => _oilConsumptionMWh; set => SetProperty(ref _oilConsumptionMWh, value); }


    private string _displayMaxHeat = "";
    public string DisplayMaxHeat { get => _displayMaxHeat; set => SetProperty(ref _displayMaxHeat, value); }

    private string _displayBaseProductionCost = "";
    public string DisplayBaseProductionCost { get => _displayBaseProductionCost; set => SetProperty(ref _displayBaseProductionCost, value); }

    public Bitmap ImagePath => Type switch
    {
        "Gas Boiler" => new Bitmap(AssetLoader.Open(
            new Uri("avares://Presentation/Assets/GasBoiler.png"))),

        "Oil Boiler" => new Bitmap(AssetLoader.Open(
            new Uri("avares://Presentation/Assets/OilBoiler.png"))),

        "Gas Motor" => new Bitmap(AssetLoader.Open(
            new Uri("avares://Presentation/Assets/GasMotor.png"))),

        "Electric Boiler" => new Bitmap(AssetLoader.Open(
            new Uri("avares://Presentation/Assets/ElectricBoiler.png"))),

        _ => new Bitmap(AssetLoader.Open(
            new Uri("avares://Presentation/Assets/Default.png")))
    };
}
