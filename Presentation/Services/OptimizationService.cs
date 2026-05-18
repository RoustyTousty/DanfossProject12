namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;
using System.Linq;

public class OptimizationService
{
    private readonly AssetService _assetService;
    Optimizer _optimizer = new();

    public OptimizationService(AssetService assetService)
    {
        _assetService = assetService;
    }

    public (List<IResultData> result, double costImpact) Optimize(string? unitToDisable, int? maintenanceTime)
    {
        var data = _assetService.GetHourlyDatas();
        var units = _assetService.GetProductionUnits();
        
        if (unitToDisable != null)
        {
            return _optimizer.OptimizeWithMaintenance(data, units, unitToDisable, maintenanceTime ?? 30);

        } else
        {
            return (_optimizer.OptimizeWithoutMaintenance(data, units), 0);
        }
        
    }

}
