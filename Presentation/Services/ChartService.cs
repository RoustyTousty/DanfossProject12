namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class ChartService(AssetService assetService)
{
    private readonly AssetService _assetService = assetService;

    public Dictionary<string, List<double>> BuildUnitSeries()
    {
        var unitNames = _assetService.ResultData
            .SelectMany(r => r.UnitProduction)
            .Select(u => u.unitName)
            .Distinct();

        return unitNames.ToDictionary(
            unit => unit,
            unit => _assetService.ResultData.Select(r =>
                r.UnitProduction.FirstOrDefault(u => u.unitName == unit)?.heatProduced ?? 0
            ).ToList()
        );
    }
}