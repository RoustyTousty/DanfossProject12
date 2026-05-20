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
            new() { Name = "GM1", Type = UnitType.GasMotor, MaxHeatMW = 3.3, MaxElectricityMW = 3.9, BaseProductionCostDKK = 975, CO2EmissionsKg = 227 }
        ];
    }

    private HourlyData GetSampleHourlyData(int hourOffset = 0)
    {
        return new HourlyData
        {
            TimeFrom = DateTime.Now.AddHours(hourOffset),
            TimeTo = DateTime.Now.AddHours(hourOffset + 1),
            HeatDemandMWh = 8.47,
            ElectricityPriceDKK = 690.58
        };
    }

    [Fact]
    public void OptimizeMany_ShouldDistributeHeatCorrectly()
    {
        // Arrange
        var units = GetSampleUnits();
        var data = new List<IHourlyData> { GetSampleHourlyData() };

        // Act
        var results = _optimizer.OptimizeWithoutMaintenance(data, units, 1);

        // Assert
        Assert.Single(results);
        var result = results.First();
        Assert.Equal(8.47, Math.Round(result.UnitProduction.Sum(up => up.heatProduced), 2));
        Assert.True(result.CO2ProductionKG > 0);
    }

    [Fact]
    public void OptimizeMany_ShouldThrowWhenDemandCannotBeMet()
    {
        // Arrange
        var units = new List<ProductionUnit>
        {
            new() { Name = "SmallBoiler", Type = UnitType.GasBoiler, MaxHeatMW = 5, BaseProductionCostDKK = 100 }
        };
        var data = new List<IHourlyData>
        {
            new HourlyData() { TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(1), HeatDemandMWh = 10, ElectricityPriceDKK = 0.5 }
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _optimizer.OptimizeWithoutMaintenance(data, units, 1));
        Assert.Contains("cannot be met", exception.Message);
    }

    [Fact]
    public void OptimizeWithMaintenance_ShouldOptimizeWithMaintenanceWindow()
    {
        // Arrange
        var units = GetSampleUnits();
        var data = new List<IHourlyData>
        {
            GetSampleHourlyData(0),
            GetSampleHourlyData(1),
            GetSampleHourlyData(2)
        };

        // Act
        var (results, costImpact) = _optimizer.OptimizeWithMaintenance(data, units, "GB1", 1, 1);

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
        var data = new List<IHourlyData>
        {
            new HourlyData { TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(1), HeatDemandMWh = 10, ElectricityPriceDKK = 0.5 }
        };


        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _optimizer.OptimizeWithMaintenance(data, units, "Unit1", 1, 1));
        Assert.Contains("Could not optimize", exception.Message);
    }

    [Fact]
    public void OptimizeWithMaintenance_ShouldThrowWhenUnitDoesNotExist()
    {
        // Arrange
        var units = GetSampleUnits();
        var data = new List<IHourlyData> { GetSampleHourlyData() };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _optimizer.OptimizeWithMaintenance(data, units, "NonExistentUnit", 1, 1));
        Assert.Contains("Specified unit does not exist", exception.Message);
    }
}