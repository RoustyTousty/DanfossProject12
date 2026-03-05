using Avalonia;
using HeatOptimization.Data;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    // [STAThread]
    // public static void Main(string[] args) => BuildAvaloniaApp()
    //     .StartWithClassicDesktopLifetime(args);

    // // Avalonia configuration, don't remove; also used by visual designer.
    // public static AppBuilder BuildAvaloniaApp()
    //     => AppBuilder.Configure<App>()
    //         .UsePlatformDetect()
    //         .WithInterFont()
    //         .LogToTrace();
    public static void Main()
    {   
        HourlyChartProvider hourlyChartProvider = new();
        ProductionUnitLibraryProvider productionUnitLibraryProvider = new();
        AssetManager assetManager = new(hourlyChartProvider, productionUnitLibraryProvider, ["GB1", "GB2", "GB3", "OB1"]);
        ResultDataManager resultDataManager = new();
        Optimizer optimizer = new(assetManager, resultDataManager);
    }

}
