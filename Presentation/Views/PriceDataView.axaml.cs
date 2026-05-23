using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform.Storage;
using Avalonia;
using HeatOptimization.Presentation.ViewModels;

namespace HeatOptimization.Presentation.Views;

public partial class PriceDataView : UserControl
{
    public PriceDataView()
    {
        InitializeComponent();
        SetupScrollSync();
        this.DataContextChanged += PriceDataView_DataContextChanged;
    }

    private void PriceDataView_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is PriceDataViewModel vm)
        {
            vm.SaveFilePickerService = async () =>
            {
                var topLevel = TopLevel.GetTopLevel(this) as Window;
                if (topLevel == null) return null;

                var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save price data",
                    SuggestedFileName = "price-data.csv",
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("CSV Files") { Patterns = new[] { "*.csv" } }
                    }
                });

                return file?.Path?.LocalPath;
            };
        }
    }

    private void DateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var popup = this.FindControl<Popup>("DatePopup");
        if (popup != null)
        {
            popup.DataContext = this.DataContext;
            popup.IsOpen = true;
        }
    }


    private void DayButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var popup = this.FindControl<Popup>("DatePopup");
        if (popup != null)
        {
            popup.IsOpen = false;
        }
    }

    private bool _scrollSyncing = false;

    private void SetupScrollSync()
    {
        var header = this.FindControl<ScrollViewer>("HeaderScrollViewer");
        var right = this.FindControl<ScrollViewer>("RightDataScrollViewer");
        if (header == null || right == null) return;

        header.ScrollChanged += (s, e) =>
        {
            if (_scrollSyncing) return;
            try { _scrollSyncing = true; right.Offset = new Avalonia.Vector(header.Offset.X, right.Offset.Y); } finally { _scrollSyncing = false; }
        };

        right.ScrollChanged += (s, e) =>
        {
            if (_scrollSyncing) return;
            try { _scrollSyncing = true; header.Offset = new Avalonia.Vector(right.Offset.X, header.Offset.Y); } finally { _scrollSyncing = false; }
        };
    }
}