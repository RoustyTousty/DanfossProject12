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
        try 
        {
            StreamWriter writer = new StreamWriter(_filePath);

            if (!File.Exists(_filePath))
            {
                await writer.WriteLineAsync("Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),ElectricityProduction (MWh),ElectricityConsumption (MWh),CO2Production (KG)");
            }

            string line = string.Join(',', 
                resultData.HourlyData.TimeFrom, 
                resultData.HourlyData.TimeTo, 
                resultData.HourlyData.HeatDemandMWh, 
                resultData.HourlyData.ElectricityPriceDKK, 
                resultData.ElectricityProductionMWh, 
                resultData.ElectricityConsumptionMWh, 
                resultData.CO2ProductionKG
            );
            await writer.WriteLineAsync(line);
        }
        catch (IOException ex)
        {
            throw new Exception($"Failed to write results to CSV file: {_filePath}", ex);
        }

        Console.WriteLine($"Successfully saved results to {_filePath}");
    }


    public async Task SaveManyAsync(List<IResultData> resultDataList)
    {
        try
        {
            StreamWriter writer = new StreamWriter(_filePath, true);
            
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                await writer.WriteLineAsync("Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),ElectricityProduction (MWh),ElectricityConsumption (MWh),CO2Production (KG)");
            }

            foreach (var resultData in resultDataList)
            {
                string line = string.Join(',',
                    resultData.HourlyData.TimeFrom,
                    resultData.HourlyData.TimeTo,
                    resultData.HourlyData.HeatDemandMWh,
                    resultData.HourlyData.ElectricityPriceDKK,
                    resultData.ElectricityProductionMWh,
                    resultData.ElectricityConsumptionMWh,
                    resultData.CO2ProductionKG
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
        // Martina filter and find by time
        return [];
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
            // TBD!
            UnitProduction = new(),
            HourlyData = hourlyData,
            ElectricityProductionMWh = double.Parse(values[4]),
            ElectricityConsumptionMWh = double.Parse(values[5]),
            CO2ProductionKG = double.Parse(values[6])
        };
    }
}
