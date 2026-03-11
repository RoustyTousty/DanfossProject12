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
                    
                    var parts = line.Split(",");

                    if (parts.Length > valueColumnIndex)
                    {
                        try
                        {
                            DateTime time = DateTime.ParseExact(
                                parts[0],
                                "dd.MM.yyyy HH:mm",
                                CultureInfo.InvariantCulture);

                            double val = double.Parse(parts[valueColumnIndex], CultureInfo.InvariantCulture);
                            result[time] = val;
                        }
                        catch(FormatException e)
                        {
                            Console.WriteLine($"Format error in line: {line} | {e.Message}");
                        }
                        catch(IndexOutOfRangeException e)
                        {
                            Console.WriteLine($"Index error in line: {line} | {e.Message}");
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
