namespace HeatOptimization.Logic;

public class AssetManager
{
    private IProductionUnitLibraryProvider _productionUnitLibraryProvider;
    private IHourlyChartProvider _hourlyChartProvider;
    private List<IHourlyData> _hourlyData;
    private List<ProductionUnit> _productionUnits = [];

    public AssetManager (IHourlyChartProvider hourlyChartProvider, IProductionUnitLibraryProvider productionUnitLibraryProvider)
    {
        _productionUnitLibraryProvider = productionUnitLibraryProvider;
        _hourlyChartProvider = hourlyChartProvider;

        _hourlyData = _hourlyChartProvider.GetHourlyData();
        _productionUnits = _productionUnitLibraryProvider.GetProductionUnits();
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

    
    public IHourlyData? GetHourlyData(DateTime dateTime)
    {
        try
        {
            IHourlyData? data = _hourlyData.Find(x => x.TimeFrom < dateTime && x.TimeTo > dateTime);
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

    public List<IHourlyData> GetHourlyDatas()
    {
        return _hourlyData;
    }
    public List<ProductionUnit> GetProductionUnits()
    {
        return _productionUnits;
    }
}


