public interface IResultRepository
{
    public Task SaveAsync(ResultData result);

    public Task SaveManyAsync(List<ResultData> results);

    public Task<List<ResultData>> GetAllAsync();

    public Task<List<ResultData>> GetByTimeRangeAsync(DateTime from, DateTime to);
}