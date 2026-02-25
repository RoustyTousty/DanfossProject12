using HeatOptimization.Logic;

namespace HeatOptimization.Presentation.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public Optimizer? Optimizer { get; }

    public MainWindowViewModel(Optimizer optimizer) 
    {  
       Optimizer = optimizer;
    }
}
