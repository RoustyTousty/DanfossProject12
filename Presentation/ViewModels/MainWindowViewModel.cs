using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public override string Title => "Main"; 
    public override Bitmap Icon => LoadAsset("danfoss-logo.png");
    public AssetService AssetService { get; }
    public OptimizationService OptimizationService { get; }
    public ResultService ResultService { get; }
    public ChartService ChartService { get; }

    [ObservableProperty]
    private ViewModelBase _currentPage;


    public ObservableCollection<ViewModelBase> Pages { get; }

    public MainWindowViewModel(AssetService assetService, OptimizationService optimizationService, ResultService resultService, ChartService chartService) 
    {  
        AssetService = assetService;
        OptimizationService = optimizationService;
        ResultService = resultService;
        ChartService = chartService;

        Pages = new ObservableCollection<ViewModelBase>
        {
            new HomeViewModel(),
            new HeatOptimizationViewModel(AssetService),
            new PriceDataViewModel(ResultService, ChartService),
            new AboutUsViewModel()
        };

        _currentPage = Pages[0]; 
    }
}