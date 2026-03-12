namespace HeatOptimization.Logic;

public class HourlyData
{
    public DateTime TimeFrom { get; set; }

    public DateTime TimeTo { get; set; }

    public double HeatDemandMWh { get; set; }

    public double ElectricityPriceDKK { get; set; }
}