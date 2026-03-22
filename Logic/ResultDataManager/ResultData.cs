namespace HeatOptimization.Logic;

public class ResultData : HourlyData
{
    public double HeatProductionMWh { get; set; }
    public double ElectricityProductionMWh { get; set; }
    public double ElectricityConsumptionMWh { get; set; }
    public double ConsumptionOfPrimaryEnegryMWh { get; set; }
    public double CO2ProductionKG { get; set; }

    public double ExpensesDKK { get; set; }
    public double ProfitDKK { get; set; }
}