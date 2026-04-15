namespace HeatOptimization.Logic;


public class ResultData : IResultData
{
    public required HourlyData HourlyData {get; set;}
    public required List<UnitProduction> UnitProduction { get; set; }
    public double CO2ProductionKG { get; set; }
    public double ElectricityProductionMWh { get; set; }
    public double ElectricityConsumptionMWh { get; set; }
}
