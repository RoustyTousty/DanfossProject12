namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class ResultService
{
    private readonly IResultRepository _repository;
    private readonly AssetService _assetService;


    public ResultService(
        AssetService assetService,
        IResultRepository repository)
    {
        _repository = repository;
        _assetService = assetService;
    }


    public async Task SaveAsync()
    {
        await _repository.SaveManyAsync(_assetService.ResultData.Cast<IResultData>().ToList());
        Console.WriteLine("Saved the file");
    }
}