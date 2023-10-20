using System;
using Avalonia.ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;
using Splat;

namespace Sims1WidescreenPatcher.UI.Views;

public partial class MainWindow : ReactiveWindow<IMainWindowViewModel>
{
    public MainWindow() : this(Locator.Current.GetService<IMainWindowViewModel>() ?? throw new InvalidOperationException()) {}
    public MainWindow(IMainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}