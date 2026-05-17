namespace HeatOptimization.Logic;

public class Optimizer
{

    private List<UnitProductionCost> GetUnitHourlyProdutionCostsForOneMWh(IHourlyData data, List<ProductionUnit> units)
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


    private List<UnitProduction> DistributeHeatLoad(IHourlyData data, List<ProductionUnit> units)
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


    private List<IResultData> OptimizeMany(List<IHourlyData> hourlyDataList, List<ProductionUnit> productionUnits) 
     {
        List<IResultData> results = new();

        foreach (IHourlyData data in hourlyDataList)
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


    private (DateTime from, DateTime to, double costImpact, List<IResultData> optimizedWindow)? FindMaintenanceWindow(List<IHourlyData> data, List<ProductionUnit> units, string unitToDisable, int durationHours)
    {
        if (!units.Any(u => u.Name == unitToDisable))
        {
            throw new Exception("Specified unit does not exist in the units!");
        }

        List<IResultData> baselineResults = OptimizeMany(data, units);
        var baselineCosts = baselineResults
            .Select(r => CalculateCost(r.HourlyData, r.UnitProduction, units))
            .ToList();

        double bestImpact = double.MaxValue;
        int bestStartIndex = -1;
        List<UnitProduction> productionDstribution = [];

        // list of units without the unit to put on downtime
        var reducedUnits = units
                .Where(u => u.Name != unitToDisable)
                .ToList();

        for (int start = 0; start <= data.Count - durationHours; start++)
        {
            double impact = 0;
            bool isValid = true;
            List<UnitProduction> distribution = [];
        
            for (int h = start; h < start + durationHours; h++)
            {
                try
                {
                    distribution = DistributeHeatLoad(data[h], reducedUnits);

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
                productionDstribution = distribution;
            }
        }

        if (bestStartIndex == -1)
            return null;

        

        return (
            data[bestStartIndex].TimeFrom,
            data[bestStartIndex + durationHours - 1].TimeTo,
            bestImpact,
            OptimizeMany(data.GetRange(bestStartIndex, durationHours), reducedUnits)
        );
    }
    

    private double CalculateCost(IHourlyData data, List<UnitProduction> distribution, List<ProductionUnit> units)
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

    // costImpact is how much it costs to have a downtime 
    public (List<IResultData>, double costImpact) OptimizeWithMaintenance(List<IHourlyData> hourlyDataList, List<ProductionUnit> productionUnits, string unitToDisable, int durationHours)
    {
        List<IResultData> results = OptimizeMany(hourlyDataList, productionUnits);
        // find the window
        var maintenanceResult = FindMaintenanceWindow(hourlyDataList, productionUnits, unitToDisable, durationHours);

        if (maintenanceResult == null)
        {
            throw new Exception("Could not optimize for a maintenance window, heat demand cannot be met with current unit configuration!");
        }

        // replace matching time window in results
        var (from, to, costImpact, optimizedWindow) = maintenanceResult.Value;

        var startIndex = results.FindIndex(r => r.HourlyData.TimeFrom == from);
        var endIndex = results.FindIndex(r => r.HourlyData.TimeTo == to);

        if (startIndex == -1 || endIndex == -1)
        {
            throw new Exception("Maintenance window does not exist in optimization results.");
        }

        int windowSize = endIndex - startIndex + 1;

        if (optimizedWindow.Count != windowSize)
        {
            throw new Exception("Optimized window size does not match target slice size.");
        }

        results.RemoveRange(startIndex, windowSize);
        results.InsertRange(startIndex, optimizedWindow);

        return (
            results, 
            costImpact
        );
    }

    public List<IResultData> OptimizeWithoutMaintenance(List<IHourlyData> data, List<ProductionUnit> units)
    {
        return OptimizeMany(data, units);
    }
}

public class UnitProductionCost {
    public IHourlyData HourlyData;
    public ProductionUnit Unit { get; set; }
    public double ProductionCostDKK {get; set; }

    public UnitProductionCost(IHourlyData hourlyData, ProductionUnit unit, double productionCostDKK)
    {
        Unit = unit;
        ProductionCostDKK = productionCostDKK;
        HourlyData = hourlyData;
    }
}