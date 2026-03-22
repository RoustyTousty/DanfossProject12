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

    public List<UnitProductionCost> GetUnitHourlyProdutionCostsForOneMWh(HourlyData data)
    {
        List<UnitProductionCost> costs = [];
        foreach (ProductionUnit unit in _assetManager.ProductionUnits)
        {
            if (unit.MaxElectricityMW == null)
                costs.Add(new UnitProductionCost(data, unit.Name, unit.BaseProductionCostDKK));

            if (unit.MaxElectricityMW != null)
            {
                double electricityPerHeatRatio = unit.MaxElectricityMW.Value / unit.MaxHeatMW;
                costs.Add(new UnitProductionCost(data, unit.Name, unit.BaseProductionCostDKK
                    - electricityPerHeatRatio * data.ElectricityPriceDKK));
            }
        }
        return costs;
    }


    public List<(string unitName, double heatProduced)>
        DistributeHeatLoad(HourlyData data)
    {
        double remainingDemand = data.HeatDemandMWh;
        
        var costs = GetUnitHourlyProdutionCostsForOneMWh(data);

        var sortedUnits = _assetManager.ProductionUnits
            .Join(costs,
                  unit => unit.Name,
                  cost => cost.Name,

                  (unit, cost) =>
                   new {
                      Unit = unit,
                      Cost = cost.ProductionCostDKK
                  })

            .OrderBy(x => x.Cost)
            .ToList();

        
        var result = new List<(string UnitName, double HeatProduced)>();


        foreach (var entry in sortedUnits)
        {
            if(remainingDemand <= 0)
            {
                break;
            }

            double maxHeat = entry.Unit.MaxHeatMW ?? 0.0;

            double heatProduced = Math.Min(maxHeat, remainingDemand);

            result.Add((entry.Unit.Name, heatProduced));
            
            remainingDemand -= heatProduced;
        }


        // I am not sure if we need this, but if demand for heat
        // was not met, we can check it. It may help with detecting
        // that system is under-supplying heat.

        if (remainingDemand > 0)
        {
            throw new Exception("Heat demand cannot be met");
        }
        return result;

    }

}

public class UnitProductionCost {
    public HourlyData HourlyData;
    public string Name { get; set; }
    public double ProductionCostDKK {get; set; }

    public UnitProductionCost(HourlyData hourlyData, string name, double productionCostDKK)
    {
        Name = name;
        ProductionCostDKK = productionCostDKK;
        HourlyData = hourlyData;
    }
}