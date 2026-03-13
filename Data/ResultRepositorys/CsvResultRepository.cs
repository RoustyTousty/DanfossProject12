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
        List<ResultData> resultDataList = new List<ResultData> {resultData};
        await SaveManyAsync(resultDataList);
    }

    public async Task SaveManyAsync(List<ResultData> resultDataList)
    {
        try 
        {
            StreamWriter writer = new StreamWriter(_filePath);

            if (!File.Exists(_filePath))
            {
                await writer.WriteLineAsync("TimeFrom,TimeTo,HeatDemandMWh,ElectricityPriceDKK,HeatProductionMWh,ElectricityProductionMWh,ElectricityConsumptionMWh,ConsumptionOfPrimaryEnegryMWh,CO2ProductionKG,ExpensesDKK,ProfitDKK");
            }

            foreach (ResultData resultData in resultDataList)
            {
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
        }
        catch (IOException ex)
        {
            throw new Exception($"Failed to write results to CSV file: {_filePath}", ex);
        }
    }

    public async Task<List<ResultData>> GetAllAsync()
    {
        List<ResultData> resultData = new List<ResultData>();



        return resultData;
    }

    public async Task<List<ResultData>> GetByTimeRangeAsync(DateTime from, DateTime to)
    {
        List<ResultData> all = await GetAllAsync();

        return all;
    }
}