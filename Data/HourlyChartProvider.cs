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
            throw new FileNotFoundException($"No file found with name {path}");
        }

        using (StreamReader reader = new(path))
        {
            reader.ReadLine(); 

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');

                // add parts length check - check if variable parts has exactly 4 rows with data
                
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
                        // TBD: display the line (SCRUM-73)
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
}