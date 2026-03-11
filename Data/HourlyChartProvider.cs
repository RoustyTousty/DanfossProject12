namespace HeatOptimization.Data;

using HeatOptimization.Logic;
using System.Globalization;

public class HourlyChartProvider : IHourlyChartProvider
{
    private static Dictionary<DateTime, double> GetHourlyData(string? fname, int valueColumnIndex)
    {
        var dict = new Dictionary<DateTime, double>();
        string path = "./Data/InputData/HourlyData/" + fname;

        try
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File does not exist: " + path);
                return dict;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                string? line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == null)
                        continue;

                    var parts = line.Split(",");
                    if (parts.Length > valueColumnIndex)
                    {
                        try
                        {
                            DateTime time = DateTime.ParseExact(
                                parts[0],
                                "dd.MM.yyyy HH:mm",
                                CultureInfo.InvariantCulture);
                            double val = value;
                            result[time] = val;
                        }
                        catch
                        {
                            Console.WriteLine("Problem reading line:" + line);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error reading file: " + e.Message);
        }

        return dict;
    }

    public Dictionary<DateTime, double> GetElectricityPrices(string fname = "summerSeason.csv")
        => GetHourlyData(fname, 3);

    public Dictionary<DateTime, double> GetHeatDemand(string fname = "summerSeason.csv")
        => GetHourlyData(fname, 2);
}
