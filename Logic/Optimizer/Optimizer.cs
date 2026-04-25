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


    public (DateTime from, DateTime to, double costImpact)? FindMaintenanceWindow(List<HourlyData> data, List<ProductionUnit> units, string unitToDisable, int durationHours)
    {
        List<IResultData> baselineResults = OptimizeMany(data, units);
        var baselineCosts = baselineResults
            .Select(r => CalculateCost(r.HourlyData, r.UnitProduction, units))
            .ToList();

        double bestImpact = double.MaxValue;
        int bestStartIndex = -1;
        // list of units without the unit to put on downtime
        var reducedUnits = units
                .Where(u => u.Name != unitToDisable)
                .ToList();

        for (int start = 0; start <= data.Count - durationHours; start++)
        {
            double impact = 0;
            bool isValid = true;
        
            for (int h = start; h < start + durationHours; h++)
            {
                try
                {
                    var distribution = DistributeHeatLoad(data[h], reducedUnits);

                    double newCost = CalculateCost(data[h], distribution, reducedUnits);

                    impact += (newCost - baselineCosts[h]);
                }
                catch
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid && impact < bestImpact)
            {
                bestImpact = impact;
                bestStartIndex = start;
            }
        }

        if (bestStartIndex == -1)
            return null;

        return (
            data[bestStartIndex].TimeFrom,
            data[bestStartIndex + durationHours - 1].TimeTo,
            bestImpact
        );
    }
    

    private double CalculateCost(HourlyData data, List<UnitProduction> distribution, List<ProductionUnit> units)
    {
        double total = 0;

        foreach (UnitProduction production in distribution)
        {
            ProductionUnit unit = units.First(u => u.Name == production.unitName);

            double costPerMWh = unit.BaseProductionCostDKK;

            if (unit.MaxElectricityMW != null)
            {
                double ratio = unit.MaxElectricityMW.Value / unit.MaxHeatMW;
                costPerMWh -= ratio * data.ElectricityPriceDKK;
            }

            total += costPerMWh * production.heatProduced;
        }

        return total;
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