namespace HeatOptimization.Logic;

public class ProductionUnit
{
    public string Name { get; set; } = "";
    public string UnitType { get; set; } = "";
    public double MaxHeatMW { get; set; }
    public double? MaxElectricityMW { get; set; }
    public int ProductionCostsDKK { get; set; } 
    public int? CO2EmissionsKg { get; set; } 
    public double? GasConsumptionMWh { get; set; } 
    public double? OilConsumptionMWh { get; set; } 
}