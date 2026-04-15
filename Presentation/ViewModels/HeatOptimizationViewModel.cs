using Avalonia.Media.Imaging;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class HeatOptimizationViewModel : ViewModelBase
{
    private readonly AssetManager _assetManager;
    
    public override string Title => "Heat Optimization";
    public override Bitmap Icon => LoadAsset("opti-icon.png");

    public HeatOptimizationViewModel(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }
}