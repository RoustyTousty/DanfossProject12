using Avalonia.Media.Imaging;

namespace HeatOptimization.Presentation.ViewModels;

public partial class AboutUsViewModel : ViewModelBase
{
    public override string Title => "About Us";
    public override Bitmap Icon => LoadAsset("about-us.png");
}