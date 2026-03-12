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
                reader.ReadLine(); 

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');

                    if (parts.Length > valueColumnIndex)
                    {
                        if (DateTime.TryParseExact(
                                parts[0],
                                "dd.MM.yyyy HH:mm",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out DateTime time)
                            &&
                            double.TryParse(
                                parts[valueColumnIndex],
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture,
                                out double val))
                        {
                            dict[time] = val;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid data in line: {line}");
                        }
                    }
                }
            }
        }
        catch (IOException e)
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