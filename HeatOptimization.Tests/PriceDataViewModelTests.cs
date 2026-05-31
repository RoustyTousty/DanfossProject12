using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HeatOptimization.Logic;
using HeatOptimization.Presentation;
using HeatOptimization.Presentation.ViewModels;
using Xunit;

public class PriceDataViewModelTests
{
    private class DummyHourlyChartProvider : IHourlyChartProvider
    {
        public List<IHourlyData> GetHourlyData(string fname = "summerSeason.csv") => new();
    }

    private class DummyProductionUnitLibraryProvider : IProductionUnitLibraryProvider
    {
        public List<ProductionUnit> GetProductionUnits() => new();
    }

    private class DummyResultRepository : IResultRepository
    {
        private const string Header = "Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),ElectricityProduction (MWh),ElectricityConsumption (MWh),CO2Production (KG),UnitLoadDistribution(MWh)";

        public Task SaveAsync(IResultData result, string filepath)
        {
            return SaveManyAsync(new List<IResultData> { result }, filepath);
        }

        public async Task SaveManyAsync(List<IResultData> results, string filepath)
        {
            using var writer = new StreamWriter(filepath, false);
            await writer.WriteLineAsync(Header);

            foreach (var result in results)
            {
                var unitsString = SerializeUnitProduction(result.UnitProduction);
                var line = string.Join(',',
                    result.HourlyData.TimeFrom,
                    result.HourlyData.TimeTo,
                    result.HourlyData.HeatDemandMWh,
                    result.HourlyData.ElectricityPriceDKK,
                    result.ElectricityProductionMWh,
                    result.ElectricityConsumptionMWh,
                    result.CO2ProductionKG,
                    unitsString
                );
                await writer.WriteLineAsync(line);
            }
        }

        public Task<List<IResultData>> GetAllAsync(string filepath) => Task.FromResult(new List<IResultData>());
        public Task<List<IResultData>> GetByTimeRangeAsync(DateTime from, DateTime to, string filepath) => Task.FromResult(new List<IResultData>());

        private static string SerializeUnitProduction(List<UnitProduction> units)
        {
            return units == null || !units.Any()
                ? string.Empty
                : string.Join("|", units.Select(u => $"{u.unitName}:{u.heatProduced}"));
        }
    }

    private PriceDataViewModel CreateViewModel(IEnumerable<IResultData> results)
    {
        var assetManager = new AssetManager(new DummyHourlyChartProvider(), new DummyProductionUnitLibraryProvider());
        var assetService = new AssetService(assetManager)
        {
            ResultData = new ObservableCollection<IResultData>(results)
        };

        var chartService = new ChartService(assetService);
        var resultManager = new ResultDataManager(new DummyResultRepository());
        var resultService = new ResultService(assetService, resultManager);
        return new PriceDataViewModel(assetService, chartService, resultService);
    }

    private static IResultData CreateResult(DateTime timeFrom, double heatProduced, double price, double co2)
    {
        return new ResultData
        {
            HourlyData = new HourlyData
            {
                TimeFrom = timeFrom,
                TimeTo = timeFrom.AddHours(1),
                HeatDemandMWh = 10,
                ElectricityPriceDKK = price
            },
            UnitProduction = new List<UnitProduction>
            {
                new() { unitName = "GM1", heatProduced = heatProduced }
            },
            CO2ProductionKG = co2,
            ElectricityProductionMWh = heatProduced,
            ElectricityConsumptionMWh = heatProduced
            ,
            TotalPrice = heatProduced * price
        };
    }

    [Fact]
    public async Task LoadAsync_PopulatesSummaryFields()
    {
        // Arrange
        var results = new[]
        {
            CreateResult(new DateTime(2026, 1, 1, 0, 0, 0), 2, 100, 5),
            CreateResult(new DateTime(2026, 1, 2, 0, 0, 0), 3, 200, 10)
        };

        var vm = CreateViewModel(results);

        // Act
        await vm.LoadAsync();

        // Assert
        Assert.Equal("5.0 MW", vm.TotalHeatProduced);
        Assert.Equal("15 kg", vm.TotalCO2);
        Assert.Equal("800 DKK", vm.TotalCost);
        Assert.Equal("0 DKK", vm.MaintenanceCost);
        Assert.Equal("01.01.2026 - 02.01.2026", vm.DateRange);
        Assert.Equal(new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0)), vm.DateFrom);
        Assert.Equal(new DateTime(2026, 1, 2), vm.DateTo?.Date);
        Assert.Equal(1, vm.DateTo?.Hour);
    }

    [Fact]
    public async Task ApplyDateRange_FiltersBySelectedDate()
    {
        // Arrange
        var results = new[]
        {
            CreateResult(new DateTime(2026, 1, 1, 0, 0, 0), 1, 100, 1),
            CreateResult(new DateTime(2026, 1, 2, 0, 0, 0), 2, 100, 2),
            CreateResult(new DateTime(2026, 1, 3, 0, 0, 0), 3, 100, 3)
        };

        var vm = CreateViewModel(results);
        await vm.LoadAsync();

        // Act
        vm.DateFrom = new DateTimeOffset(new DateTime(2026, 1, 2));
        vm.DateTo = new DateTimeOffset(new DateTime(2026, 1, 2));
        vm.ApplyDateRange();

        // Assert
        Assert.Single(vm.FilteredHourlyRows);
        Assert.Contains("02.01.2026", vm.DateRange);
    }

    [Fact]
    public async Task HourlySearch_UpdatesFilteredHourlyRows()
    {
        // Arrange
        var results = new[]
        {
            CreateResult(new DateTime(2026, 1, 1, 0, 0, 0), 1, 100, 1),
            CreateResult(new DateTime(2026, 1, 2, 0, 0, 0), 2, 100, 2)
        };

        var vm = CreateViewModel(results);
        await vm.LoadAsync();

        // Act
        vm.HourlySearch = "02.01.2026";

        // Assert
        Assert.Single(vm.FilteredHourlyRows);
        Assert.Contains("02.01.2026", vm.FilteredHourlyRows.First().From);
    }

    [Fact]
    public async Task SaveAsync_WritesCsvFile()
    {
        // Arrange
        var results = new[]
        {
            CreateResult(new DateTime(2026, 1, 1, 0, 0, 0), 1, 100, 1)
        };

        var vm = CreateViewModel(results);
        await vm.LoadAsync();

        var tempFile = Path.GetTempFileName();
        vm.SaveFilePickerService = () => Task.FromResult<string?>(tempFile);

        try
        {
            // Act
            await vm.SaveAsync();

            // Assert
            var lines = await File.ReadAllLinesAsync(tempFile);
            Assert.Equal(2, lines.Length);
            Assert.Contains("Time From (DK local time)", lines[0]);
            Assert.Contains("100", lines[1]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task LoadAsync_WithNoResults_SetsEmptySummaryAndNoRows()
    {
        // Arrange
        var vm = CreateViewModel(Array.Empty<IResultData>());

        // Act
        await vm.LoadAsync();

        // Assert
        Assert.Equal("0.0 MW", vm.TotalHeatProduced);
        Assert.Equal("0 kg", vm.TotalCO2);
        Assert.Equal("0 DKK", vm.TotalCost);
        Assert.Equal("0 DKK", vm.MaintenanceCost);
        Assert.Equal(string.Empty, vm.DateRange);
        Assert.Null(vm.DateFrom);
        Assert.Null(vm.DateTo);
        Assert.Empty(vm.FilteredHourlyRows);
    }

    [Fact]
    public async Task SaveAsync_WithNoResults_WritesOnlyHeader()
    {
        // Arrange
        var vm = CreateViewModel(Array.Empty<IResultData>());
        await vm.LoadAsync();

        var tempFile = Path.GetTempFileName();
        vm.SaveFilePickerService = () => Task.FromResult<string?>(tempFile);

        try
        {
            // Act
            await vm.SaveAsync();

            // Assert
            var lines = await File.ReadAllLinesAsync(tempFile);
            Assert.Single(lines);
            Assert.Equal("Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),ElectricityProduction (MWh),ElectricityConsumption (MWh),CO2Production (KG),UnitLoadDistribution(MWh)", lines[0]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task HourlySearch_NoMatch_ClearsFilteredRows()
    {
        // Arrange
        var results = new[]
        {
            CreateResult(new DateTime(2026, 1, 1, 0, 0, 0), 1, 100, 1),
            CreateResult(new DateTime(2026, 1, 2, 0, 0, 0), 2, 100, 2)
        };

        var vm = CreateViewModel(results);
        await vm.LoadAsync();

        // Act
        vm.HourlySearch = "no match";

        // Assert
        Assert.Empty(vm.FilteredHourlyRows);
    }
}
