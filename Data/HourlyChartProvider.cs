namespace HeatOptimization.Data;

using HeatOptimization.Logic;
using System.Globalization;

public class HourlyChartProvider : IHourlyChartProvider
{

    public List<HourlyData> GetHourlyData(string? fname)
    {
        var hourlyData = new List<HourlyData>();
        var path = $"./Data/InputData/HourlyData/{fname}";

        try
        {
            StreamReader sr = new(path);

            string? line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                var valueArr = line.Split(",");

                hourlyData.Add(new HourlyData {
                    TimeFrom = DateTime.ParseExact(
                        valueArr[0],
                        "dd.MM.yyyy HH:mm",
                        CultureInfo.InvariantCulture
                    ),
                    TimeTo = DateTime.ParseExact(
                        valueArr[1],
                        "dd.MM.yyyy HH:mm",
                        CultureInfo.InvariantCulture
                    ),
                    HeatDemandMWh = double.Parse(valueArr[2]),
                    ElectricityPriceDKK = double.Parse(valueArr[3]),
                });
            }
        }
        catch (Exception e)
        {
            //  TBD Proper error handling
            Console.WriteLine($"Error reading {path}: {e.Message}");
        }

        return hourlyData;
    }
}