using Avalonia.Media.Imaging;

namespace HeatOptimization.Presentation.ViewModels;

public partial class PriceDataViewModel : ViewModelBase
{
    public override string Title => "Price Data";
    public override Bitmap Icon => LoadAsset("price-icon.png");
}