namespace HeatOptimization.Logic;

public class AssetManager
{
    private IProductionUnitLibraryProvider _productionUnitLibraryProvider;
    private IHourlyChartProvider _hourlyChartProvider;
    public readonly Dictionary<DateTime, double> HourlyElectricityPrices = [];
    public readonly Dictionary<DateTime, double> HourlyHeatDemand = [];
    public readonly List<ProductionUnit> ProductionUnits = [];

    public AssetManager (IHourlyChartProvider hourlyChartProvider, IProductionUnitLibraryProvider productionUnitLibraryProvider, List<string> productionUnits)
    {
        _productionUnitLibraryProvider = productionUnitLibraryProvider;
        _hourlyChartProvider = hourlyChartProvider;

        HourlyElectricityPrices = _hourlyChartProvider.GetElectricityPrices();
        HourlyHeatDemand = _hourlyChartProvider.GetHeatDemand();
        ProductionUnits = _productionUnitLibraryProvider.GetProductionUnits(productionUnits);
    }

    public double GetElectricityPriceByTime(DateTime dateTime)
    {
        if (HourlyElectricityPrices.TryGetValue(dateTime, out var price))
        {
            return price;
        }
        
        return 0;
    }
}