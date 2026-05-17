using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using HeatOptimization.Logic;
using HeatOptimization.Presentation.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace HeatOptimization.Presentation.Views
{
    public partial class Config : Window
    {
        private string? _selectedFilePath;



        public Config()
        {
            InitializeComponent();
            if (App.AssetService != null && App.OptimizationService != null)
            {
                DataContext = new ConfigViewModel(App.AssetService, App.OptimizationService);
            }
        }

        public Config(AssetService assetService, OptimizationService optimizationService)
        {
            InitializeComponent();
            DataContext = new ConfigViewModel(assetService, optimizationService);
        }


        private async void OnFileSelectClick(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Pick file wih hourly data",
                AllowMultiple = false
            });

            if (files != null && files.Any())
            {
                var file = files[0];
                _selectedFilePath = file.Path.LocalPath;

                // This can be removed, but it shows name of the file
                
                FileSelectBtn.Content = file.Name;
            }
        }

        private void OnOptimizeClick(object sender, RoutedEventArgs e)
        {
            var result = new ConfigResult
            {
                FilePath = _selectedFilePath,
            };
            
            Close(result);
        }
    }

    public class ConfigResult
    {
        public string? FilePath { get; set; }

    }
}