namespace HeatOptimization.Logic;

public interface IResultData
{
    public HourlyData HourlyData { get; set; }
    public List<UnitProduction> UnitProduction { get; set; }
    public double CO2ProductionKG { get; set; }
    public double ElectricityProductionMWh { get; set; }
    public double ElectricityConsumptionMWh { get; set; }
}