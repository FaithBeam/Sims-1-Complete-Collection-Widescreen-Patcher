using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Microsoft.VisualBasic;
using ReactiveUI;
using sims_iff.Models;
using Sims1WidescreenPatcher.Core.Tabs;
using Sims1WidescreenPatcher.Core.ViewModels;
using Sims1WidescreenPatcher.UI.Dialogs;

namespace Sims1WidescreenPatcher.UI.Tabs;

public partial class ExtrasTab : ReactiveUserControl<IExtrasTabViewModel>
{
    private TopLevel? _topLevel;
    private Window? _window;

    public ExtrasTab()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel == null)
                return;

            _topLevel = TopLevel.GetTopLevel(this);
            _window = (Window)_topLevel!;

            d(
                ViewModel.ShowCareerEditorDialogInteraction.RegisterHandler(
                    ShowCareerEditorDialogAsync
                )
            );
        });
    }

    private async Task ShowCareerEditorDialogAsync(
        IInteractionContext<ICareerEditorTabViewModel, Iff?> context
    )
    {
        var dialog = new CareerEditorDialog { DataContext = context.Input };
        var result = await dialog.ShowDialog<Iff?>(
            _window ?? throw new InvalidOperationException()
        );
        context.SetOutput(result);
    }
}
