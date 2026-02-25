namespace HeatOptimization.Logic;

public interface IProductionUnitLibraryProvider {
    public List<ProductionUnit> GetProductionUnits(List<string> names);
}
