namespace HeatOptimization.Logic;

public class ResultDataManager
{
    private readonly IResultRepository _repository;

    public ResultDataManager(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task StoreResultAsync(IResultData resultData)
    {
        await _repository.SaveAsync(resultData);
    }


    public async Task StoreResultsAsync(List<IResultData> resultDataList)
    {
        await _repository.SaveManyAsync(resultDataList);
    }


    public async Task<List<IResultData>> GetResultsAsync()
    {
        return await _repository.GetAllAsync();
    }


    public async Task<List<IResultData>> GetResultsInPeriod(DateTime from, DateTime to)
    {
        return await _repository.GetByTimeRangeAsync(from, to);
    }
}