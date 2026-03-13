namespace HeatOptimization.Data;

using HeatOptimization.Logic;
using System.Globalization;

public class HourlyChartProvider : IHourlyChartProvider
{
    public List<HourlyData> GetHourlyData(string fname)
    {
        List<HourlyData> hourlyData = [];
        string path = "./Data/InputData/HourlyData/" + fname;
       
    if (!File.Exists(path))
    {
        Console.WriteLine("File does not exist: " + path);
        return [];
    }

    using (StreamReader reader = new StreamReader(path))
    {
        reader.ReadLine(); 

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(',');

            // add parts length check
            
            if (!DateTime.TryParseExact(
                    parts[0],
                    "dd.MM.yyyy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime from) ||
                !DateTime.TryParseExact(
                    parts[1],
                    "dd.MM.yyyy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime to) ||
                !double.TryParse(
                    parts[2],
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double heatDemand) ||
                !double.TryParse(
                        parts[2],
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out double electricityPrice))
                {
                    // TBD: display the line SCRUM-73
                    throw new Exception("Error while reading line ");
                }

            hourlyData.Add(new HourlyData
            {
                TimeFrom = from,
                TimeTo = to,
                HeatDemandMWh = heatDemand,
                ElectricityPriceDKK = electricityPrice,
            });
            }
        }

        return hourlyData;
    }

    // public Dictionary<DateTime, double> GetElectricityPrices(string fname = "summerSeason.csv")
    //     => GetHourlyData(fname, 3);

    // public Dictionary<DateTime, double> GetHeatDemand(string fname = "summerSeason.csv")
    //     => GetHourlyData(fname, 2);
}