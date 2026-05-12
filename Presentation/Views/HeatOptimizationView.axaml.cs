using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Linq;
using System;

namespace HeatOptimization.Presentation.Views;

public partial class HeatOptimizationView : UserControl
{
    public HeatOptimizationView() => InitializeComponent();
    
    private async void OnOptimizeBtnClick(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is Window mainWindow)
        {
            var configWindow = new Config();

            var result = await configWindow.ShowDialog<ConfigResult>(mainWindow);
        }
    }
      
}