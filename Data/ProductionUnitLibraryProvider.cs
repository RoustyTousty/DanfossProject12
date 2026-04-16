namespace HeatOptimization.Data;

using HeatOptimization.Logic;

public class ProductionUnitLibraryProvider : IProductionUnitLibraryProvider
{
    public List<ProductionUnit> GetProductionUnits(List<string> names)
    {
        List<ProductionUnit> units = [];

        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "InputData",
            "ProductionUnits",
            "productionUnits.csv"
        );

        try
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Error: productionUnits.csv file could not be found.");
                return units;
            }

            StreamReader sr = new(path);
            string? line = sr.ReadLine();

            while ((line = sr.ReadLine()) != null)
            {
                try
                {
                    var valueArr = line.Split(",");

                    if (valueArr.Length < 8)
                    {
                        Console.WriteLine($"Warning: Invalid line format skipped -> {line}");
                        continue;
                    }

                    if (names.Contains(valueArr[0]))
                    {
                        Enum.TryParse(valueArr[1], true, out UnitType type);

                        units.Add(new ProductionUnit
                        {
                            Name = valueArr[0],
                            Type = type,
                            MaxHeatMW = double.Parse(valueArr[2], System.Globalization.CultureInfo.InvariantCulture),
                            MaxElectricityMW = string.IsNullOrWhiteSpace(valueArr[3])
                                ? null
                                : double.Parse(valueArr[3], System.Globalization.CultureInfo.InvariantCulture),
                            BaseProductionCostDKK = int.Parse(valueArr[4], System.Globalization.CultureInfo.InvariantCulture),
                            CO2EmissionsKg = string.IsNullOrWhiteSpace(valueArr[5])
                                ? null
                                : int.Parse(valueArr[5], System.Globalization.CultureInfo.InvariantCulture),
                            GasConsumptionMWh = string.IsNullOrWhiteSpace(valueArr[6])
                                ? null
                                : double.Parse(valueArr[6], System.Globalization.CultureInfo.InvariantCulture),
                            OilConsumptionMWh = string.IsNullOrWhiteSpace(valueArr[7])
                                ? null
                                : double.Parse(valueArr[7], System.Globalization.CultureInfo.InvariantCulture),
                        });
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine($"Error: Could not parse numeric values in line -> {line} in ./Data/InputData/ProductionUnits/productionUnits.csv");
                    Console.WriteLine(e);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine($"Error: Missing values in line -> {line}");
                }
            }

            sr.Close();
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: productionUnits.csv file was not found.");
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error reading the file: {e.Message}");
        }

        return units;
    }
}