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
        foreach (HourlyData data in _assetManager.GetHourlyData())
        {
            switch (unit.Type)
            {
                case UnitType.GasBoiler:
                case UnitType.OilBoiler:
                    
                    break;
                case UnitType.GasMotor:
                    
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