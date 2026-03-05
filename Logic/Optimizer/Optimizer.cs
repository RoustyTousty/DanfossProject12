namespace HeatOptimization.Logic;

public class Optimizer
{
    private AssetManager _assetManager;
    private ResultDataManager _resultDataManager;

    public Optimizer(AssetManager assetManager, ResultDataManager resultDataManager)
    {
        _assetManager = assetManager;
        _resultDataManager = resultDataManager;
    }

    public Dictionary<DateTime, double>? GetHourlyProcutionCostsForUnit(ProductionUnit unit)
    {
        Dictionary<DateTime, double> hourlyCostsPerMWh = [];
        foreach (KeyValuePair<DateTime, double> entry in _assetManager.HourlyElectricityPrices)
        {
            switch (unit.Type)
            {
                case UnitType.GasBoiler:
                case UnitType.OilBoiler:
                    hourlyCostsPerMWh.Add(entry.Key, Math.Round(unit.BaseProductionCostsDKK/unit.MaxHeatMW, 2));
                    break;
                case UnitType.GasMotor:
                    double? electricityPrice = _assetManager.GetElectricityPriceByTime(entry.Key);
                    if (electricityPrice == null)
                    {
                        Console.WriteLine($"Warning! No electricity price for time {entry.Key}!");
                        return null;
                    } 
                    if (unit.MaxElectricityMW == null)
                    {
                        Console.WriteLine($"Warning! No electricity parameter specified for {unit.Name}!");
                        Console.WriteLine($"Units of type {unit.Type} must have MaxElectricityMW specified!");
                        return null;
                    }
                    // hourlyCostsPerMWh.Add(entry.Key, unit.MaxHeatMW/unit.ProductionCostsDKK + unit.MaxElectricityMW/electricityPrice);
                    break;
                case UnitType.ElectricBoiler:
                    
                    break;
                default: 
                    Console.WriteLine($"Warning! Wrong data type of production unit {unit.Name}!");
                    return null;
            }
            // hourlyCostsPerMWh.Add(entry, );
        }
        return hourlyCostsPerMWh;
    }
    // public Dictionary<DateTime, double> GetHourlyProcutionCosts()
    // {
    //     Dictionary<DateTime, double> unitProductionCosts = [];
    //     foreach (ProductionUnit unit in _assetManager.ProductionUnits)
    //     {
    //         unitProductionCosts[]
    //     }
    // }

}