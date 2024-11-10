using System;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Dialogs;

public partial class CustomResolutionDialog : ReactiveWindow<CustomResolutionDialogViewModel>
{
    public CustomResolutionDialog()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel != null)
                d(ViewModel.OkCommand.Subscribe(Close));
        });
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
