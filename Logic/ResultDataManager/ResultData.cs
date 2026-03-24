namespace HeatOptimization.Logic;


public class ResultData : HourlyData
{
    public double CO2ProductionKG { get; set; }
    public double ElectricityProductionMWh { get; set; }
    public double ElectricityConsumptionMWh { get; set; }
}
