namespace HeatOptimization.Logic;

public class Optimizer
{

    private List<UnitProductionCost> GetUnitHourlyProdutionCostsForOneMWh(HourlyData data, List<ProductionUnit> units)
    {
        List<UnitProductionCost> costs = [];
        foreach (ProductionUnit unit in units)
        {
            if (unit.MaxElectricityMW == null)
                costs.Add(new UnitProductionCost(data, unit, unit.BaseProductionCostDKK));

            if (unit.MaxElectricityMW != null)
            {
                double electricityPerHeatRatio = unit.MaxElectricityMW.Value / unit.MaxHeatMW;
                costs.Add(new UnitProductionCost(data, unit, unit.BaseProductionCostDKK
                    - electricityPerHeatRatio * data.ElectricityPriceDKK));
            }
        }
        return costs;
    }


    public List<(string unitName, double heatProduced)> DistributeHeatLoad(HourlyData data, List<ProductionUnit> units)
    {
        double remainingDemand = data.HeatDemandMWh;
        
        
        List<UnitProductionCost> costs = GetUnitHourlyProdutionCostsForOneMWh(data, units)
        .OrderBy(x => x.ProductionCostDKK)
        .ToList();

        
        List<(string UnitName, double HeatProduced)> result = new();

        foreach (UnitProductionCost entry in costs)
        {
            if(remainingDemand <= 0)
            {
                break;
            }

            double maxHeat = entry.Unit.MaxHeatMW;

            double heatProduced = Math.Min(maxHeat, remainingDemand);

            result.Add((entry.Unit.Name, heatProduced));
            
            remainingDemand -= heatProduced;
        }

        if (remainingDemand > 0)
        {
            throw new Exception($"Heat demand: {data.HeatDemandMWh} MWh cannot be met, because only {data.HeatDemandMWh - remainingDemand} MWh heat can be produced with existing generators.");
        }
        return result;

    }

    // we need a function that goes over the whole HourlyData list and applies DistributeHeatLoad() to it. Then DistributeHeatLoad can become private instead. 
}

public class UnitProductionCost {
    public HourlyData HourlyData;
    public ProductionUnit Unit { get; set; }
    public double ProductionCostDKK {get; set; }

    public UnitProductionCost(HourlyData hourlyData, ProductionUnit unit, double productionCostDKK)
    {
        Unit = unit;
        ProductionCostDKK = productionCostDKK;
        HourlyData = hourlyData;
    }
}