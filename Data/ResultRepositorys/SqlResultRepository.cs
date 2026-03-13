// namespace HeatOptimization.Data;

// using HeatOptimization.Logic;

// public class SqlResultRepository : IResultRepository
// {
//     private readonly string _filePath;

//     public SqlResultRepository(string filePath)
//     {
//         _filePath = filePath;
//     }

//     public async Task SaveAsync(ResultData resultData)
//     {
//         List<ResultData> resultDataList = new List<ResultData> {resultData};
//         await SaveManyAsync(resultDataList);
//     }

//     public async Task SaveManyAsync(List<ResultData> resultDataList)
//     {
//         // Loop the list and save it to SQL
//     }

//     public Task<List<ResultData>> GetAllAsync()
//     {
//         // Retrieve all data from SQL
//     }

//     public Task<List<ResultData>> GetByTimeRangeAsync(DateTime from, DateTime to)
//     {
//         // Call filter SQL data for time (Maybe pull from GetAllAsync())
//     }
// }