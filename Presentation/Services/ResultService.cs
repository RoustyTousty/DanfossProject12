namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class ResultService
{
    private readonly IResultRepository _repository;
    private readonly AssetService _assetService;
    private readonly OptimizationService _optimizationService;

    public ResultService(
        AssetService assetService,
        OptimizationService optimizationService,
        IResultRepository repository)
    {
        _optimizationService = optimizationService;
        _repository = repository;
        _assetService = assetService;
    }

    public async Task RunAndSaveAsync(string unitToDisable, int maintenanceTime)
    {
        _optimizationService.Optimize(unitToDisable, maintenanceTime);
        await _repository.SaveManyAsync(_assetService.ResultData);
    }

    public async Task RunAndSaveAsync()
    {
        _optimizationService.Optimize(null, null);
        
        await _repository.SaveManyAsync(_assetService.ResultData);
        Console.WriteLine("Saved the file");
    }
}