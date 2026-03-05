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

    public List<UnitProductionCost> GetUnitHourlyProcutionCostsForHour(HourlyData data)
    {
        List<UnitProductionCost> costs = [];
        foreach (ProductionUnit unit in _assetManager.ProductionUnits)
        {
            if (unit.MaxElectricityMW == null)
                costs.Add(new UnitProductionCost(unit.Name, unit.BaseProductionCostDKK));

            if (unit.MaxElectricityMW != null)
            {
                double electricityPerHeatRatio = unit.MaxElectricityMW.Value / unit.MaxHeatMW;
                costs.Add(new UnitProductionCost(unit.Name, unit.BaseProductionCostDKK
                    - electricityPerHeatRatio * data.ElectricityPriceDKK));
            }
        }
        return costs;
    }
}

public class UnitProductionCost {
    public string Name { get; set; }
    public double ProductionCostDKK {get; set; }

    public UnitProductionCost(string name, double productionCostDKK)
    {
        Name = name;
        ProductionCostDKK = productionCostDKK;
    }
}