using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia;
using HeatOptimization.Presentation.ViewModels;

namespace HeatOptimization.Presentation.Views;

public partial class PriceDataView : UserControl
{
    public PriceDataView()
    {
        InitializeComponent();
        SetupScrollSync();
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

    private void ApplyDateRange_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is PriceDataViewModel vm)
        {
            vm.ApplyDateRange();
        }

        var popup = this.FindControl<Popup>("DatePopup");
        if (popup != null)
        {
            popup.IsOpen = false;
        }
    }

    private void ClearDateRange_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is PriceDataViewModel vm)
        {
            vm.ResetDateRange();
        }

        var popup = this.FindControl<Popup>("DatePopup");
        if (popup != null)
        {
            popup.IsOpen = false;
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