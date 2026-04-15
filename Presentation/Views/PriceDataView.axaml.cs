using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace HeatOptimization.Presentation.Views;

public partial class PriceDataView : UserControl
{
    public PriceDataView()
    {
        InitializeComponent();
    }

    private void DateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var popup = this.FindControl<Popup>("DatePopup");
        if (popup != null)
        {
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
}