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

    public async Task<List<IResultData>> RunAndSaveAsync()
    {
        var results = _optimizationService.Optimize();

        await _repository.SaveManyAsync(results);

        return results;
    }
}