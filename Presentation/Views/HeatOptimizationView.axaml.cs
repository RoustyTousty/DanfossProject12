using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Linq;
using System;

namespace HeatOptimization.Presentation.Views;

public partial class HeatOptimizationView : UserControl
{
    public HeatOptimizationView() => InitializeComponent();
    
    private async void OnUploadClick(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Heat Data File",
            AllowMultiple = false
        });

        // if (files != null && files.Any())
        // {
        //     var selectedFile = files[0];
            
        //     var configWindow = new ConfigWindow(selectedFile.Name);
            
        //     if (topLevel is Window mainWindow)
        //     {
        //         var result = await configWindow.ShowDialog<ConfigResult>(mainWindow);

        //         if (result != null)
        //         {
        //             this.FindControl<TextBlock>("StatusText").Text = $"Uploaded: {selectedFile.Name}";
        //         }
        //     }
        // }
    }
}
