using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public override string Title => "Main"; 
    public override Bitmap Icon => LoadAsset("danfoss-logo.png");

    public AssetManager AssetManager { get; }

    [ObservableProperty]
    private ViewModelBase _currentPage;


    public ObservableCollection<ViewModelBase> Pages { get; }

    public MainWindowViewModel(AssetManager assetManager) 
    {  
        AssetManager = assetManager;

        Pages = new ObservableCollection<ViewModelBase>
        {
            new HomeViewModel(),
            new HeatOptimizationViewModel(AssetManager),
            new PriceDataViewModel()
        };

        _currentPage = Pages[0]; 
    }
}