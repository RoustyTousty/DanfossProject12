using Xunit;
using HeatOptimization.Logic;

public class OptimizerTests
{
    private Optimizer _optimizer = new();

    private List<ProductionUnit> GetSampleUnits()
    {
        return
        [
            new() { Name = "GB1", Type = UnitType.GasBoiler, MaxHeatMW = 3.0, BaseProductionCostDKK = 510, CO2EmissionsKg = 132 },
            new() { Name = "GB2", Type = UnitType.GasBoiler, MaxHeatMW = 2.0, BaseProductionCostDKK = 540, CO2EmissionsKg = 134 },
            new() { Name = "EB1", Type = UnitType.ElectricBoiler, MaxHeatMW = 6.0, BaseProductionCostDKK = 15, MaxElectricityMW = -6.0, CO2EmissionsKg = 0 },
            new() { Name = "GM1", Type = UnitType.GasMotor, MaxHeatMW = 5.3, MaxElectricityMW = 3.9, BaseProductionCostDKK = 975, CO2EmissionsKg = 227 }
        ];
    }

    private HourlyData GetSampleHourlyData(int hourOffset = 0)
    {
        return new HourlyData
        {
            TimeFrom = DateTime.Now.AddHours(hourOffset),
            TimeTo = DateTime.Now.AddHours(hourOffset + 1),
            HeatDemandMWh = 8.27,
            ElectricityPriceDKK = 690.58
        };
    }

    [Fact]
    public void OptimizeMany_ShouldDistributeHeatCorrectly()
    {
        // Arrange
        List<ProductionUnit> units = GetSampleUnits();
        List<HourlyData> data = [GetSampleHourlyData()];
        Optimizer optimizer = new();

        // Act
        var results = optimizer.OptimizeMany(data, units);

        // Assert
        Assert.Single(results);
        var result = results.First();
        Assert.Equal(8.27, result.UnitProduction.Sum(up => up.heatProduced));
    }

    [Fact]
    public void OptimizeMany_ShouldThrowWhenDemandCannotBeMet()
    {
        // Arrange
        List<ProductionUnit> units = [ new() { Name = "SmallBoiler", Type = UnitType.GasBoiler, MaxHeatMW = 5, BaseProductionCostDKK = 100 } ];
        List<HourlyData> data = [ new() { TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(1), HeatDemandMWh = 10, ElectricityPriceDKK = 0.5 }];
        Optimizer optimizer = new();

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => optimizer.OptimizeMany(data, units));
        Assert.Contains("cannot be met", exception.Message);
    }

    [Fact]
    public void OptimizeWithMaintenance_ShouldOptimizeWithMaintenanceWindow()
    {
        // Arrange
        List<ProductionUnit> units = GetSampleUnits();
        List<HourlyData> data = [ GetSampleHourlyData(0), GetSampleHourlyData(1), GetSampleHourlyData(2)];
        Optimizer optimizer = new();

        // Act
        var (results, costImpact) = optimizer.OptimizeWithMaintenance(data, units, "GB1", 1);

        // Assert
        Assert.Equal(3, results.Count);
        Assert.True(costImpact >= 0);
    }

    [Fact]
    public void OptimizeWithMaintenance_ShouldThrowWhenNoMaintenanceWindowFound()
    {
        // Arrange
        var units = new List<ProductionUnit>
        {
            new() { Name = "Unit1", Type = UnitType.GasBoiler, MaxHeatMW = 6, BaseProductionCostDKK = 100 },
            new() { Name = "Unit2", Type = UnitType.GasBoiler, MaxHeatMW = 6, BaseProductionCostDKK = 100 }
        };
        var data = new List<HourlyData>
        {
            new() { TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(1), HeatDemandMWh = 10, ElectricityPriceDKK = 0.5 }
        };
        var optimizer = new Optimizer();

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => optimizer.OptimizeWithMaintenance(data, units, "Unit1", 1));
        Assert.Contains("Could not optimize", exception.Message);
    }

    [Fact]
    public void OptimizeWithMaintenance_ShouldThrowWhenUnitDoesNotExist()
    {
        // Arrange
        var units = GetSampleUnits();
        var data = new List<HourlyData> { GetSampleHourlyData() };
        var optimizer = new Optimizer();

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => optimizer.OptimizeWithMaintenance(data, units, "NonExistentUnit", 1));
        Assert.Contains("Specified unit does not exist", exception.Message);
    }
}