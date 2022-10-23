using System;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Views;

public partial class CustomYesNoDialog : ReactiveWindow<CustomYesNoDialogViewModel>
{
    public CustomYesNoDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        
        this.WhenActivated(delegate(Action<IDisposable> action)
        {
            action(ViewModel!.YesCommand.Subscribe(Close));
            action(ViewModel!.NoCommand.Subscribe(Close));
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}