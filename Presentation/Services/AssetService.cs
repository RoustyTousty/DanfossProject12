namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class AssetService
{
    private AssetManager _assetManager;
    public List<IHourlyData> HourlyData = [];
    public List<IResultData> ResultData = [];
    public double CostImpact = 0;


    public AssetService(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }

    public List<IHourlyData> UpdateHourlyDatas(string fpath)
    {
        List<IHourlyData> data =  _assetManager.GetHourlyDatas(fpath);
        HourlyData = data;
        return data;
    }

    public List<ProductionUnit> GetProductionUnits()
    {
        return _assetManager.GetProductionUnits();
    }
}