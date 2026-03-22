using System.Security.Cryptography.X509Certificates;

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

    public class UnitWithCost
    {
        public required ProductionUnit Unit {get; set;}
        public double Cost {get; set;}
    }


    public List<(string unitName, double heatProduced)> DistributeHeatLoad(HourlyData data)
    {
        double remainingDemand = data.HeatDemandMWh;
        
        
        List<UnitProductionCost> costs = GetUnitHourlyProdutionCostsForOneMWh(data);

        List<UnitWithCost> sortedUnits = _assetManager.ProductionUnits
        .Join(
            costs,
            unit => unit.Name,
            cost => cost.Name,
            (unit, cost) => new UnitWithCost
            {
                Unit = unit,
                Cost = cost.ProductionCostDKK
            }
        )
        .OrderBy(x => x.Cost)
        .ToList();

        
        List<(string UnitName, double HeatProduced)> result = new List<(string UnitName, double HeatProduced)>();

        double totalProducedHeat = 0;

        foreach (var entry in sortedUnits)
        {
            if(remainingDemand <= 0)
            {
                break;
            }

            double maxHeat = entry.Unit.MaxHeatMW;

            double heatProduced = Math.Min(maxHeat, remainingDemand);

            result.Add((entry.Unit.Name, heatProduced));
            
            remainingDemand -= heatProduced;
            totalProducedHeat += heatProduced;
        }

        if (remainingDemand > 0)
        {
            throw new Exception($"Heat demand: {data.HeatDemandMWh} MWh cannot be met, because only {totalProducedHeat} MWh heat can be produced with existing generators.");
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