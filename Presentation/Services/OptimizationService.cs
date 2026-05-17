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

    public void Optimize(string? unitToDisable, int? maintenanceTime)
    {
        var data = _assetService.HourlyData;
        var units = _assetService.GetProductionUnits();
        
        if (unitToDisable != null)
        {
            (List<IResultData> res, double impact) = _optimizer.OptimizeWithMaintenance(data, units, unitToDisable, maintenanceTime ?? 30);
            _assetService.ResultData = res;
            _assetService.CostImpact = impact;

        } else
        {
            List<IResultData> res = _optimizer.OptimizeWithoutMaintenance(data, units);
            _assetService.ResultData = res;
            _assetService.CostImpact = 0;
        }
        
    }
}