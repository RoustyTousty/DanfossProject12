namespace HeatOptimization.Logic;

public class AssetManager
{
    private IProductionUnitLibraryProvider _productionUnitLibraryProvider;
    private IHourlyChartProvider _hourlyChartProvider;
    private List<HourlyData> _hourlyData;
    private List<ProductionUnit> _productionUnits = [];

    public AssetManager (IHourlyChartProvider hourlyChartProvider, IProductionUnitLibraryProvider productionUnitLibraryProvider, List<string> productionUnits)
    {
        _productionUnitLibraryProvider = productionUnitLibraryProvider;
        _hourlyChartProvider = hourlyChartProvider;

        _hourlyData = _hourlyChartProvider.GetHourlyData();
        _productionUnits = _productionUnitLibraryProvider.GetProductionUnits(productionUnits);
    }



    public ProductionUnit? GetProductionUnitByName(string name)
    {
        try
        {
            return _productionUnits.FirstOrDefault(productionUnit => productionUnit.Name == name);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving production unit: {e.Message}");
            return null;
        }
    }

    
    public HourlyData? GetHourlyData(DateTime dateTime)
    {
        try
        {
            HourlyData? data = _hourlyData.Find(x => x.TimeFrom < dateTime && x.TimeTo > dateTime);
            if (data != null)
            {
                return data;
            } else {
                Console.WriteLine($"Error while retrieving hourly data at {dateTime}");
                return null;
            }
        } catch (Exception e)
        {
            Console.WriteLine($"Error retrieving hourly data for {dateTime}: {e.Message}");
            return null;
        }
    }

    public List<HourlyData> GetHourlyDatas()
    {
        return _hourlyData;
    }
    public List<ProductionUnit> GetProductionUnits()
    {
        return _productionUnits;
    }
}


