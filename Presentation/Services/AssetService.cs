namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class AssetService
{
    private AssetManager _assetManager;

    public AssetService(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }

    public List<IHourlyData> GetHourlyDatas()
    {
        return _assetManager.GetHourlyDatas();
    }

    public List<ProductionUnit> GetProductionUnits()
    {
        return _assetManager.GetProductionUnits();
    }
}