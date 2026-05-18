using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HeatOptimization.Presentation.ViewModels;
using System.Threading.Tasks;

namespace HeatOptimization.Presentation.Views;

public partial class ErrorWindow : Window
{
    public string ErrorMessage { get; set; } = "";

    public ErrorWindow()
    {
        InitializeComponent();
    }

    public ErrorWindow(string message)
        : this()
    {
        ErrorMessage = message;
        DataContext = this;
    }

    private void Ok_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}