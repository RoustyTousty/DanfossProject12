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



    public ProductionUnit? GetProductionUnitByName(string name)
    {
        try
        {
            return ProductionUnits.FirstOrDefault(productionUnit => productionUnit.Name == name);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving production unit: {e.Message}");
            return null;
        }
    }

    public double? GetHeatDemandByTime(DateTime dateTime)
    {
        try
        {
            return HourlyHeatDemand[dateTime];
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving hourly heat demand for {dateTime}: {e.Message}");
            return null;
        }
    }
}