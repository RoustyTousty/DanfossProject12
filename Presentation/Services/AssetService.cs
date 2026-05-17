namespace HeatOptimization.Presentation;

using System.Collections.ObjectModel;
using HeatOptimization.Logic;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class AssetService : ObservableObject
{
    private AssetManager _assetManager;
    [ObservableProperty]
    private ObservableCollection<IHourlyData> hourlyData = [];
    [ObservableProperty]
    private ObservableCollection<IResultData> resultData = [];
    [ObservableProperty]
    public double costImpact = 0;


    public AssetService(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }

    public List<IHourlyData> UpdateHourlyDatas(string fpath)
    {
        List<IHourlyData> data = _assetManager.GetHourlyDatas(fpath);
        HourlyData = new(data);
        return data;
    }

    

    public List<IHourlyData> GetHourlyDatasWithoutUpdate(string fpath)
    {
        return _assetManager.GetHourlyDatas(fpath);
    }

    public List<ProductionUnit> GetProductionUnits()
    {
        return _assetManager.GetProductionUnits();
    }
}