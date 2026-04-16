namespace HeatOptimization.Logic;

public interface IResultRepository
{
    public Task SaveAsync(IResultData result);

    public Task SaveManyAsync(List<IResultData> results);

    public Task<List<IResultData>> GetAllAsync();

    public Task<List<IResultData>> GetByTimeRangeAsync(DateTime from, DateTime to);
}