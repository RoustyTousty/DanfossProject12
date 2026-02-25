using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public AssetManager? AssetManager { get; }

    public MainWindowViewModel(AssetManager optimizer) 
    {  
       AssetManager = optimizer;
    }
}
