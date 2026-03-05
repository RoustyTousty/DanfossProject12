namespace HeatOptimization.Logic;

public interface IHourlyChartProvider {
    List<HourlyData> GetHourlyData(string fname = "summerSeason.csv");   
}
