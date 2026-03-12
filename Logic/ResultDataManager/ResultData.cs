public class ResultData
{
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }

    public double HeatDemandMWh { get; set; }
    public double ElectricityPriceDKK { get; set; }

    public double HeatProductionMWh { get; set; }
    public double ElectricityProductionMWh { get; set; }
    public double ElectricityConsumptionMWh { get; set; }
    public double ConsumptionOfPrimaryEnegryMWh { get; set; }
    public double CO2ProductionKG { get; set; }

    public double ExpensesDKK { get; set; }
    public double ProfitDKK { get; set; }
}