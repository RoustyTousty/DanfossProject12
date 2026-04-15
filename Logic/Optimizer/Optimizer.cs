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


    private List<UnitProduction> DistributeHeatLoad(HourlyData data, List<ProductionUnit> units)
    {
        double remainingDemand = data.HeatDemandMWh;
        
        
        List<UnitProductionCost> costs = GetUnitHourlyProdutionCostsForOneMWh(data, units)
        .OrderBy(x => x.ProductionCostDKK)
        .ToList();

        
        List<UnitProduction> result = new();

        foreach (UnitProductionCost entry in costs)
        {
            if(remainingDemand <= 0)
            {
                break;
            }

            double maxHeat = entry.Unit.MaxHeatMW;

            double heatProduced = Math.Min(maxHeat, remainingDemand);

            result.Add(new UnitProduction {
                unitName = entry.Unit.Name, 
                heatProduced = heatProduced
            });
            
            remainingDemand -= heatProduced;
        }

        if (remainingDemand > 0)
        {
            throw new Exception($"Heat demand: {data.HeatDemandMWh} MWh cannot be met, because only {data.HeatDemandMWh - remainingDemand} MWh heat can be produced with existing generators.");
        }

        return result;
    }


    public List<IResultData> OptimizeMany(List<HourlyData> hourlyDataList, List<ProductionUnit> productionUnits) 
     {
        List<IResultData> results = new();

        foreach (HourlyData data in hourlyDataList)
        {
            List<UnitProduction> distribution = DistributeHeatLoad(data, productionUnits);

            double totalCO2 = 0;
            double totalElectricityProduced = 0;
            double totalElectricityConsumed = 0;

            foreach (UnitProduction production in distribution)
            {
                var unit = productionUnits.First(u => u.Name == production.unitName);

                if (unit.CO2EmissionsKg != null)
                {
                    totalCO2 += production.heatProduced * unit.CO2EmissionsKg.Value;
                }

                if (unit.MaxElectricityMW != null)
                {
                    double ratio = unit.MaxElectricityMW.Value / unit.MaxHeatMW;
                    totalElectricityProduced += production.heatProduced * ratio;
                }

                if (unit.Type == UnitType.ElectricBoiler)
                {
                    totalElectricityConsumed += production.heatProduced;
                }
            }

            ResultData result = new() {
                UnitProduction = distribution,
                HourlyData = data,
                CO2ProductionKG = totalCO2,
                ElectricityProductionMWh = totalElectricityProduced,
                ElectricityConsumptionMWh = totalElectricityConsumed
            };

            results.Add(result);
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