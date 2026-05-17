namespace HeatOptimization.Presentation;

using HeatOptimization.Logic;

public class ResultService
{
    private readonly IResultRepository _repository;
    private readonly OptimizationService _optimizationService;

    public ResultService(
        OptimizationService optimizationService,
        IResultRepository repository)
    {
        _optimizationService = optimizationService;
        _repository = repository;
    }

    public async Task<(List<IResultData>, double costImpact)> RunAndSaveAsync(string unitToDisable, int maintenanceTime)
    {
        
        
        (List<IResultData> results, double costImpact) = _optimizationService.Optimize(unitToDisable, maintenanceTime);
        await _repository.SaveManyAsync(results);

        return (results, costImpact);
    }

    public async Task<(List<IResultData>, double costImpact)> RunAndSaveAsync()
    {
        (List<IResultData> results, _) = _optimizationService.Optimize(null, null);
        
        await _repository.SaveManyAsync(results);
        Console.WriteLine("Saved the file");
        return (results, 0);
    }
}