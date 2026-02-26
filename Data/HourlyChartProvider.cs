namespace HeatOptimization.Data;

using HeatOptimization.Logic;
using System.Globalization;

public class HourlyChartProvider : IHourlyChartProvider
{

    private static Dictionary<DateTime, double> GetHourlyData(string? fname, int valueColumnIndex)
    {
        var dict = new Dictionary<DateTime, double>();
        var path = $"./Data/InputData/HourlyData/{fname}";

        try
        {
            StreamReader sr = new(path);

            string? line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                var valueArr = line.Split(",");

                DateTime dt = DateTime.ParseExact(
                    valueArr[0],
                    "dd.MM.yyyy HH:mm",
                    CultureInfo.InvariantCulture
                );

                dict[dt] = double.Parse(valueArr[valueColumnIndex], CultureInfo.InvariantCulture);
            }
        }
        catch (Exception e)
        {
            //  TBD Proper error handling
            Console.WriteLine($"Error reading {path}: {e.Message}");
        }

        return dict;
    }

    public Dictionary<DateTime, double> GetElectricityPrices(string fname = "summerSeason.csv") => GetHourlyData(fname, 3);

    public Dictionary<DateTime, double> GetHeatDemand(string fname = "summerSeason.csv") => GetHourlyData(fname, 2);
}