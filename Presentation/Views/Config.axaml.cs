using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HeatOptimization.Logic;
using HeatOptimization.Presentation.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace HeatOptimization.Presentation.Views
{
    public partial class Config : Window
    {
        public Config()
        {
            InitializeComponent();
            if (App.AssetService != null && App.OptimizationService != null)
            {
                DataContext = new ConfigViewModel(App.AssetService, App.OptimizationService, PickFileCrossPlatformAsync);
            }
        }

        public Config(AssetService assetService, OptimizationService optimizationService)
        {
            InitializeComponent();
            DataContext = new ConfigViewModel(assetService, optimizationService, PickFileCrossPlatformAsync);
        }

        private async Task<string?> PickFileCrossPlatformAsync()
        {
            var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Pick file with hourly data",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Data Files") { Patterns = new[] { "*.csv", "*.xlsx", "*.json" } }
                }
            });

            return files?.FirstOrDefault()?.Path.LocalPath;
        }
    }

    public class ConfigResult
    {
        public string? FilePath { get; set; }
        public int MaintenanceHours { get; set; }
    }
}