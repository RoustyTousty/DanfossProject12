namespace HeatOptimization.Data;

using HeatOptimization.Logic;
using System.Globalization;
using System.IO;

public class HourlyChartProvider : IHourlyChartProvider
{
    public List<HourlyData> GetHourlyData(string fname)
    {
        List<HourlyData> hourlyData = new();
        

        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "InputData",
            "HourlyData",
            fname
        );

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Input file not found: {path}");
        }

        using StreamReader reader = new(path);

        reader.ReadLine(); 
        int lineNumber = 1;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;

            try
            {
                var parts = line.Split(',');

                if (parts.Length < 4)
                {
                    throw new FormatException(
                        $"Expected 4 columns but found {parts.Length}.");
                }

                if (!DateTime.TryParseExact(
                        parts[0],
                        "dd.MM.yyyy HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime from))
                {
                    throw new FormatException("Invalid 'TimeFrom' value.");
                }

                if (!DateTime.TryParseExact(
                        parts[1],
                        "dd.MM.yyyy HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime to))
                {
                    throw new FormatException("Invalid 'TimeTo' value.");
                }

                if (!double.TryParse(
                        parts[2],
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out double heatDemand))
                {
                    throw new FormatException("Invalid 'HeatDemandMWh' value.");
                }

                if (!double.TryParse(
                        parts[3],
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out double electricityPrice))
                {
                    throw new FormatException("Invalid 'ElectricityPriceDKK' value.");
                }

                hourlyData.Add(new HourlyData
                {
                    TimeFrom = from,
                    TimeTo = to,
                    HeatDemandMWh = heatDemand,
                    ElectricityPriceDKK = electricityPrice
                });
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error parsing file '{fname}' at line {lineNumber}: {line}. {ex.Message}",
                    ex);  // here I solved Scrum-73
            }
        }

        return hourlyData;
    }
}