using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using HeatOptimization.Data;

public class CsvResultRepositoryTests
{
    [Fact]
    public async Task GetByTimeRangeAsync_Returns_Only_Overlapping_Rows()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();

        var lines = new List<string>
        {
            "Time From (DK local time),Time To (DK local time),Heat Demand (MWh),Electricity Price (DKK/Mwh(el)),ElectricityProduction (MWh),ElectricityConsumption (MWh),CO2Production (KG)",

            "05.01.2026 00:00,05.01.2026 01:00,8.27,690.58,1,1,1",
            "05.01.2026 01:00,05.01.2026 02:00,8.56,663.66,1,1,1",
            "05.01.2026 02:00,05.01.2026 03:00,8.72,646.18,1,1,1"
        };

        await File.WriteAllLinesAsync(tempFile, lines);

        var repo = new CsvResultRepository(tempFile);

        var from = new DateTime(2026, 1, 5, 0, 30, 0);
        var to   = new DateTime(2026, 1, 5, 1, 30, 0);

        // Act
        var result = await repo.GetByTimeRangeAsync(from, to);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(result, r => r.TimeFrom == new DateTime(2026, 1, 5, 0, 0, 0));
        Assert.Contains(result, r => r.TimeFrom == new DateTime(2026, 1, 5, 1, 0, 0));

        // Cleanup
        File.Delete(tempFile);
    }
}