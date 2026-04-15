using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace HeatOptimization.Presentation.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public abstract string Title { get; }
    
    public abstract Bitmap Icon { get; }

    protected static Bitmap LoadAsset(string fileName)
    {
        return new Bitmap(AssetLoader.Open(new Uri($"avares://Presentation/Assets/{fileName}")));
    }
}