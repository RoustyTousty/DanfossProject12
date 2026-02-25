namespace HeatOptimization.Logic;

public interface IHourlyChartProvider {
    Dictionary<DateTime, double> GetElectricityPrices(string fname = "summerSeason.csv");
    Dictionary<DateTime, double> GetHeatDemand(string fname = "summerSeason.csv");
    
}
