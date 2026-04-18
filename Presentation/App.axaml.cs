using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HeatOptimization.Presentation.ViewModels;
using HeatOptimization.Presentation.Views;

using HeatOptimization.Logic;
using HeatOptimization.Data;

namespace HeatOptimization.Presentation;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            AssetManager assetManager = new AssetManager(
                new HourlyChartProvider(),
                new ProductionUnitLibraryProvider(),
                [ "GB1", "GB2", "GB3", "OB1", "GM1", "EB1" ]
            );

            var optimizationService = new OptimizationService(assetManager);
            var resultRepository = new CsvResultRepository("result.csv");

            var resultService = new ResultService(optimizationService, resultRepository);
            var chartService = new ChartService();

        

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(assetManager, optimizationService, resultService, chartService),
            };
        }


        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}