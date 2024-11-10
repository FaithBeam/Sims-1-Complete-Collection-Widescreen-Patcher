using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Dialogs;

public partial class CustomInformationDialog : ReactiveWindow<CustomInformationDialogViewModel>
{
    public CustomInformationDialog()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
