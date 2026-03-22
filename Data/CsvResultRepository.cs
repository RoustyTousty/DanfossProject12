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

        StreamWriter writer = new StreamWriter(_filePath);

        if (!File.Exists(_filePath))
        {
            writer.WriteLine("TimeFrom,TimeTo,HeatDemandMWh,ElectricityPriceDKK,HeatProductionMWh,ElectricityProductionMWh,ElectricityConsumptionMWh,ConsumptionOfPrimaryEnegryMWh,CO2ProductionKG,ExpensesDKK,ProfitDKK");
        }

        string line = string.Join(',', resultData.TimeFrom, resultData.TimeTo, resultData.HeatDemandMWh, resultData.ElectricityPriceDKK, resultData.HeatProductionMWh, resultData.ElectricityProductionMWh, resultData.ElectricityConsumptionMWh, resultData.ConsumptionOfPrimaryEnegryMWh, resultData.CO2ProductionKG, resultData.ExpensesDKK, resultData.ProfitDKK);

        await writer.WriteLineAsync(line);

    }

    public async Task SaveManyAsync(List<ResultData> resultDataList)
    {
        // TBD (save multiple results to csv here)
    }
}