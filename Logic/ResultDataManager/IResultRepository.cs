public interface IResultRepository
{
    Task SaveAsync(ResultData result);

    Task SaveManyAsync(List<ResultData> results);

    Task<List<ResultData>> GetAllAsync();

    Task<List<ResultData>> GetByTimeRangeAsync(DateTime from, DateTime to);
}