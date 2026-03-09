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

        // Convert resultData to list and call SaveManyAsync
    }

    public async Task SaveManyAsync(List<ResultData> resultDataList)
    {
        // TBD (save multiple results to csv here)

        // Check if the file exists

        // Using StreamWriter create writer

        // Write header if file dosnt exists

        // Write data
    }
}