using Avalonia.Controls;

namespace HeatOptimization.Presentation.Views;

public partial class ChartView : UserControl
{
    public ChartView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
    }
}