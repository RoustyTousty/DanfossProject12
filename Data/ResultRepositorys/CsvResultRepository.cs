namespace HeatOptimization.Data;

using System.ComponentModel;
using HeatOptimization.Logic;
using Microsoft.VisualBasic;

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
                await writer.WriteLineAsync("Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),HeatProduction (MWh),ElectricityProduction (MWh),ElectricityConsumption (MWh),ConsumptionOfPrimaryEnegry (MWh),CO2Production (KG),Expenses (DKK),Profit (DKK)");
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
        if (timeFrom > timeTo)
        {
            throw new Exception("TimeFrom must be erlier than TimeTo");
        }

        List<ResultData> resultDataList = await GetAllAsync();

        return resultDataList
            .Where(
                resultData => 
                resultData.TimeFrom >= timeFrom
                &&
                resultData.TimeTo <= timeTo
            ).ToList();
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