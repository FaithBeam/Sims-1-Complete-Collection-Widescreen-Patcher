using System;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Dialogs;

public partial class CustomYesNoDialog : ReactiveWindow<CustomYesNoDialogViewModel>
{
    public CustomYesNoDialog()
    {
        InitializeComponent();

        this.WhenActivated(
            delegate(Action<IDisposable> action)
            {
                action(ViewModel!.YesCommand.Subscribe(Close));
                action(ViewModel!.NoCommand.Subscribe(Close));
            }
        );
    }
}
