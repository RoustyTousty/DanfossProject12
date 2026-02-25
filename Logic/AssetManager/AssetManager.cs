namespace HeatOptimization.Logic;

public class AssetManager
{
    private IProductionUnitLibraryProvider _productionUnitLibraryProvider;
    private IHourlyChartProvider _hourlyChartProvider;
    private Dictionary<DateTime, double> _hourlyElectricityPrices = [];
    private Dictionary<DateTime, double> _hourlyHeatDemand = [];
    private List<ProductionUnit> _productionUnits = [];

    public AssetManager (IHourlyChartProvider hourlyChartProvider, IProductionUnitLibraryProvider productionUnitLibraryProvider, List<string> productionUnits)
    {
        _productionUnitLibraryProvider = productionUnitLibraryProvider;
        _hourlyChartProvider = hourlyChartProvider;

        _hourlyElectricityPrices = _hourlyChartProvider.GetElectricityPrices();
        _hourlyHeatDemand = _hourlyChartProvider.GetHeatDemand();
        _productionUnits = _productionUnitLibraryProvider.GetProductionUnits(productionUnits);
    }
}