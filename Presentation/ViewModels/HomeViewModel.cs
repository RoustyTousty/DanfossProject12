using Avalonia.Media.Imaging;

namespace HeatOptimization.Presentation.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public override string Title => "Home";
    public override Bitmap Icon => LoadAsset("home-icon.png");
}