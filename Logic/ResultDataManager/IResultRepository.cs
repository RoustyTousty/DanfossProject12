namespace HeatOptimization.Logic;

public interface IResultRepository
{
    public Task SaveAsync(IResultData result, string filepath);

    public Task SaveManyAsync(List<IResultData> results, string filepath);

    public Task<List<IResultData>> GetAllAsync(string filepath);

    public Task<List<IResultData>> GetByTimeRangeAsync(DateTime from, DateTime to, string filepath);
}