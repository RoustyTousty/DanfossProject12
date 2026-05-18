namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class OptimizationService
{
    private readonly AssetService _assetService;
    Optimizer _optimizer = new();

    public OptimizationService(AssetService assetService)
    {
        _assetService = assetService;
    }

    public void Optimize(string? unitToDisable, int? maintenanceTime, List<string> activeUnits)
    {
        var data = _assetService.HourlyData.Cast<IHourlyData>().ToList();
        var units = _assetService.GetProductionUnits().Where(unit => activeUnits.Any(active => active == unit.Name)).ToList();
            
        if (!string.IsNullOrWhiteSpace(unitToDisable))
        {
            (List<IResultData> res, double impact) = _optimizer.OptimizeWithMaintenance(data, units, unitToDisable, maintenanceTime ?? 30);
            Console.WriteLine(impact);
            _assetService.ResultData = new(res);
            _assetService.CostImpact = impact;

        } else
        {
            List<IResultData> res = _optimizer.OptimizeWithoutMaintenance(data, units);
            _assetService.ResultData = new(res);
            _assetService.CostImpact = 0;
        }
    

    }
}