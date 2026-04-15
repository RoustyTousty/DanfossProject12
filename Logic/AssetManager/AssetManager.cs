namespace HeatOptimization.Logic;

public class AssetManager
{
    private IProductionUnitLibraryProvider _productionUnitLibraryProvider;
    private IHourlyChartProvider _hourlyChartProvider;
    public readonly List<HourlyData> HourlyData;
    public readonly List<ProductionUnit> ProductionUnits = [];

    public AssetManager (IHourlyChartProvider hourlyChartProvider, IProductionUnitLibraryProvider productionUnitLibraryProvider, List<string> productionUnits)
    {
        _productionUnitLibraryProvider = productionUnitLibraryProvider;
        _hourlyChartProvider = hourlyChartProvider;

        HourlyData = _hourlyChartProvider.GetHourlyData();
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

    // Might be useful at some point 

    // public double? GetHeatDemandByTime(DateTime dateTime)
    // {
    //     try
    //     {
    //         return HourlyHeatDemand[dateTime];
    //         // HourlyData? data = _hourlyData.Find(x => x.TimeFrom < dateTime && x.TimeTo > dateTime)
    //         // if (data != null)
    //         // {
    //         //     return data.HeatDemandMWh;
    //         // } else {
    //         //     Console.WriteLine($"Error while retrieving hourly heat demand at {dateTime}")
    //         // }
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine($"Error retrieving hourly heat demand for {dateTime}: {e.Message}");
    //         return null;
    //     }
    // }

    // public double? GetElectricityPriceByTime(DateTime dateTime)
    // {
    //     try
    //     {
    //         return HourlyElectricityPrices[dateTime];
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine($"Error retrieving hourly electricity price for {dateTime}: {e.Message}");
    //         return null;
    //     }
    // }
    
    public HourlyData? GetHourlyData(DateTime dateTime)
    {
        try
        {
            HourlyData? data = HourlyData.Find(x => x.TimeFrom < dateTime && x.TimeTo > dateTime);
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
}


