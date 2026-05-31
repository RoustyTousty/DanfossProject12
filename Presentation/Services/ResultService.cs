namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class ResultService
{
    private readonly ResultDataManager _resultDataManager;
    private readonly AssetService _assetService;


    public ResultService(
        AssetService assetService,
        ResultDataManager resultDataManager)
    {
        _resultDataManager = resultDataManager;
        _assetService = assetService;
    }


    public async Task SaveAsync(string fpath)
    {
        await _resultDataManager.StoreResultsAsync(_assetService.ResultData.Cast<IResultData>().ToList(), fpath);
        Console.WriteLine("Saved the file");
    }
}