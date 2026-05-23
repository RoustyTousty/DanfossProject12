namespace HeatOptimization.Logic;

public class ResultDataManager
{
    private readonly IResultRepository _repository;

    public ResultDataManager(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task StoreResultAsync(IResultData resultData, string filepath)
    {
        await _repository.SaveAsync(resultData, filepath);
    }


    public async Task StoreResultsAsync(List<IResultData> resultDataList, string filepath)
    {
        await _repository.SaveManyAsync(resultDataList, filepath);
    }


    public async Task<List<IResultData>> GetResultsAsync(string filepath)
    {
        return await _repository.GetAllAsync(filepath);
    }


    public async Task<List<IResultData>> GetResultsInPeriod(DateTime from, DateTime to, string filepath)
    {
        return await _repository.GetByTimeRangeAsync(from, to, filepath);
    }
}