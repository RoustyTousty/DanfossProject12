namespace HeatOptimization.Data;

using HeatOptimization.Logic;

public class CsvResultRepository : IResultRepository
{
    private readonly string _filePath;

    public CsvResultRepository(string filePath)
    {
        _filePath = filePath;
    }



    public async Task SaveAsync(ResultData resultData)
    {
        try 
        {
            StreamWriter writer = new StreamWriter(_filePath);

            if (!File.Exists(_filePath))
            {
                await writer.WriteLineAsync("Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),HeatProduction (MWh),ElectricityProduction (MWh),ElectricityConsumption (MWh),ConsumptionOfPrimaryEnegry (MWh),CO2Production (KG),Expenses (DKK),Profit (DKK)");
            }

            string line = string.Join(',', 
                resultData.TimeFrom, 
                resultData.TimeTo, 
                resultData.HeatDemandMWh, 
                resultData.ElectricityPriceDKK, 
                resultData.HeatProductionMWh, 
                resultData.ElectricityProductionMWh, 
                resultData.ElectricityConsumptionMWh, 
                resultData.ConsumptionOfPrimaryEnegryMWh, 
                resultData.CO2ProductionKG, 
                resultData.ExpensesDKK, 
                resultData.ProfitDKK
            );
            await writer.WriteLineAsync(line);
        }
        catch (IOException ex)
        {
            throw new Exception($"Failed to write results to CSV file: {_filePath}", ex);
        }
    }


    public async Task SaveManyAsync(List<ResultData> resultDataList)
    {
        // Andrei save multiple at the same time
    }


    public async Task<List<ResultData>> GetAllAsync()
    {
        List<ResultData> resultDataList = new List<ResultData>();

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


    public async Task<List<ResultData>> GetByTimeRangeAsync(DateTime timeFrom, DateTime timeTo)
    {
        // Martina filter and find by time
    }



    private ResultData MapResultDataToValues(string[] values)
    {
        return new ResultData
        {
            TimeFrom = DateTime.Parse(values[0]),
            TimeTo = DateTime.Parse(values[1]),
            HeatDemandMWh = double.Parse(values[2]),
            ElectricityPriceDKK = double.Parse(values[3]),
            HeatProductionMWh = double.Parse(values[4]),
            ElectricityProductionMWh = double.Parse(values[5]),
            ElectricityConsumptionMWh = double.Parse(values[6]),
            ExpensesDKK = double.Parse(values[7]),
            ProfitDKK = double.Parse(values[8]),
            ConsumptionOfPrimaryEnegryMWh = double.Parse(values[9]),
            CO2ProductionKG = double.Parse(values[10])
        };
    }
}