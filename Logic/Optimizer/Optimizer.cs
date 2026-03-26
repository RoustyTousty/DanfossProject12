namespace HeatOptimization.Logic;

public class HeatDistribution
{
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public List<(string unitName, double heatProduced)> Units { get; set; }
}
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


    private List<(string unitName, double heatProduced)> DistributeHeatLoad(HourlyData data, List<ProductionUnit> units)
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
    public List<HeatDistribution> OptimizeMany(List<HourlyData> hourlyDataList, List<ProductionUnit> productionUnits) 
     {
         List<HeatDistribution> results = new();

         foreach (var data in hourlyDataList)
         {
             var distribution = DistributeHeatLoad(data, productionUnits);

             HeatDistribution heatDistribution = new HeatDistribution
             {
                 TimeFrom = data.TimeFrom,
                 TimeTo = data.TimeTo,
                 Units = distribution
             };

             results.Add(heatDistribution);
         }

         return results;
     }
    
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