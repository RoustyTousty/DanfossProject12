namespace HeatOptimization.Logic;

public interface IHourlyChartProvider {

    List<IHourlyData> GetHourlyData(string fname = "summerSeason.csv");   
}
