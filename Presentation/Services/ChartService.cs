namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class ChartService
{
    public Dictionary<string, List<double>> BuildUnitSeries(List<IResultData> results)
    {
        var unitNames = results
            .SelectMany(r => r.UnitProduction)
            .Select(u => u.unitName)
            .Distinct();

        return unitNames.ToDictionary(
            unit => unit,
            unit => results.Select(r =>
                r.UnitProduction.FirstOrDefault(u => u.unitName == unit)?.heatProduced ?? 0
            ).ToList()
        );
    }
}