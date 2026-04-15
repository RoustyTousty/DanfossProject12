namespace HeatOptimization.Logic;

public interface IResultData
{
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public double HeatDemandMWh { get; set; }
    public double ElectricityPriceDKK { get; set; }
    public double CO2ProductionKG { get; set; }
    public double ElectricityProductionMWh { get; set; }
    public double ElectricityConsumptionMWh { get; set; }
}