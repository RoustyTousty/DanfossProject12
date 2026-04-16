namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class OptimizationService
{
    private readonly AssetManager _assetManager;

    public OptimizationService(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }

    public List<IResultData> Optimize()
    {
        var data = _assetManager.GetHourlyDatas();
        var units = _assetManager.GetProductionUnits();

        var optimizer = new Optimizer();
        return optimizer.OptimizeMany(data, units);
    }
}