namespace HeatOptimization.Logic;

public class AssetManager
{
    private IProductionUnitLibraryProvider _productionUnitLibraryProvider;
    private IHourlyChartProvider _hourlyChartProvider;
    private List<ProductionUnit> _productionUnits = [];

    public AssetManager (IHourlyChartProvider hourlyChartProvider, IProductionUnitLibraryProvider productionUnitLibraryProvider)
    {
        _productionUnitLibraryProvider = productionUnitLibraryProvider;
        _hourlyChartProvider = hourlyChartProvider;

        _productionUnits = _productionUnitLibraryProvider.GetProductionUnits();
    }


    public List<IHourlyData> GetHourlyDatas(string fpath)
    {
        return _hourlyChartProvider.GetHourlyData(fpath);
    }
    
    public List<ProductionUnit> GetProductionUnits()
    {
        return _productionUnits;
    }
}


