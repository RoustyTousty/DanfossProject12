namespace HeatOptimization.Data;

using HeatOptimization.Logic;

public class CsvResultRepository : IResultRepository
{
    private readonly string _filePath;

    // no path validation here! besides, there should be a better way to specify a path name for saving the file in general
    public CsvResultRepository(string fileName)
    {
        string appRoot = AppContext.BaseDirectory;
        appRoot = Path.GetFullPath(Path.Combine(appRoot, "..", "..", "..", ".."));
        string baseDir = Path.Combine(appRoot, "Data", "OutputData");

        // Directory.CreateDirectory(baseDir);

        _filePath = Path.Combine(baseDir, fileName);
    }

    public async Task SaveAsync(IResultData resultData)
    {
        await SaveManyAsync(new List<IResultData> { resultData });
    }


    public async Task SaveManyAsync(List<IResultData> resultDataList)
    {
        try
        {
            StreamWriter writer = new StreamWriter(_filePath, true);
            
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                await writer.WriteLineAsync("Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),ElectricityProduction (MWh),ElectricityConsumption (MWh),CO2Production (KG),UnitLoadDistribution(MWh)");
            }

            foreach (var resultData in resultDataList)
            {
                string unitsString = SerializeUnitProduction(resultData.UnitProduction);

                string line = string.Join(',',
                    resultData.HourlyData.TimeFrom,
                    resultData.HourlyData.TimeTo,
                    resultData.HourlyData.HeatDemandMWh,
                    resultData.HourlyData.ElectricityPriceDKK,
                    resultData.ElectricityProductionMWh,
                    resultData.ElectricityConsumptionMWh,
                    resultData.CO2ProductionKG,
                    unitsString
                );

                await writer.WriteLineAsync(line);
            }
            
        }
        catch (IOException ex)
        {
            throw new Exception($"Failed to write multiple results to CSV file: {_filePath}", ex);
        }

        Console.WriteLine($"Saved data successfully into {_filePath}");
    }

    public async Task<List<IResultData>> GetAllAsync()
    {
        List<IResultData> resultDataList = new List<IResultData>();

        try
        {
            using (StreamReader streamReader = new StreamReader(_filePath))
            {
                await streamReader.ReadLineAsync();

                while (!streamReader.EndOfStream)
                {
                    string? line = await streamReader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    string[] values = line.Split(",");

                    try
                    {
                        resultDataList.Add(MapResultDataToValues(values));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return resultDataList;
        }
        catch(IOException ex)
        {
            throw new Exception($"Failed to read results from CSV file: {_filePath}", ex);
        }
    }


    public async Task<List<IResultData>> GetByTimeRangeAsync(DateTime timeFrom, DateTime timeTo)
    {
        var allResults = await GetAllAsync();
    
        return allResults
            .Where(
                r => 
                r.HourlyData.TimeFrom >= timeFrom 
                && 
                r.HourlyData.TimeTo <= timeTo
            )
            .ToList();
    }



    private IResultData MapResultDataToValues(string[] values)
    {
        HourlyData hourlyData = new()
        {
            TimeFrom = DateTime.Parse(values[0]),
            TimeTo = DateTime.Parse(values[1]),
            HeatDemandMWh = double.Parse(values[2]),
            ElectricityPriceDKK = double.Parse(values[3]),
        };

        return new ResultData
        {
            HourlyData = hourlyData,
            ElectricityProductionMWh = double.Parse(values[4]),
            ElectricityConsumptionMWh = double.Parse(values[5]),
            CO2ProductionKG = double.Parse(values[6]),
            UnitProduction = DeserializeUnitProduction(values.Length > 7 ? values[7] : "")
        };
    }



    private string SerializeUnitProduction(List<UnitProduction> units)
    {
        if (units == null || !units.Any()) return string.Empty;
        
        return string.Join("|", units.Select(u => $"{u.unitName}:{u.heatProduced}"));
    }



    private List<UnitProduction> DeserializeUnitProduction(string data)
    {
        var result = new List<UnitProduction>();
        if (string.IsNullOrWhiteSpace(data)) return result;

        var unitStrings = data.Split('|');
        foreach (var unitString in unitStrings)
        {
            var parts = unitString.Split(':');
            if (parts.Length == 2)
            {
                result.Add(new UnitProduction
                {
                    unitName = parts[0],
                    heatProduced = double.Parse(parts[1])
                });
            }
        }
        return result;
    }
}
