public class CsvResultRepository : IResultRepository
{
    private readonly string _filePath;

    public CsvResultRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task SaveAsync(ResultData resultData)
    {
        // TBD (save results to csv here)
    }

    public async Task SaveManyAsync(List<ResultData> resultDataList)
    {
        // TBD (save multiple results to csv here)
    }
}