using System;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Views;

public partial class CustomResolutionDialog : ReactiveWindow<CustomResolutionDialogViewModel>
{
    public CustomResolutionDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        
        this.WhenActivated(d =>
        {
            if (ViewModel != null) d(ViewModel!.OkCommand.Subscribe(Close));
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }


    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}