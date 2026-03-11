namespace HeatOptimization.Data;

using HeatOptimization.Logic;

public class ProductionUnitLibraryProvider: IProductionUnitLibraryProvider
{
    public List<ProductionUnit> GetProductionUnits(List<string> names)
    {
        List<ProductionUnit> units = [];
        try
        {
            StreamReader sr = new("./Data/InputData/PruductionUnits/productionUnits.csv");
            string? line = sr.ReadLine();
            
            while ((line = sr.ReadLine()) != null)
            {
                var valueArr = line.Split(",");
                
                
                if (names.Contains(valueArr[0]))
                {
                    Enum.TryParse(valueArr[1], true, out UnitType type);
                
                    units.Add(new ProductionUnit
                    {
                        Name = valueArr[0],
                        Type = type,
                        MaxHeatMW = double.Parse(valueArr[2]),
                        MaxElectricityMW = string.IsNullOrWhiteSpace(valueArr[3])
                            ? null
                            : double.Parse(valueArr[3]),
                        BaseProductionCostsDKK = int.Parse(valueArr[4]),
                        CO2EmissionsKg = string.IsNullOrWhiteSpace(valueArr[5])
                            ? null
                            : int.Parse(valueArr[5]),                  
                        GasConsumptionMWh = string.IsNullOrWhiteSpace(valueArr[6])
                            ? null
                            : double.Parse(valueArr[6]),
                        OilConsumptionMWh = string.IsNullOrWhiteSpace(valueArr[7])
                            ? null
                            : double.Parse(valueArr[7]),
                    }); 
                }
            }
        }
        catch (Exception e)
        {
            // TBD Proper error handling``
            Console.WriteLine($"Error reading productionUnits.csv: {e.Message}");
        }

        return units;
    }
}