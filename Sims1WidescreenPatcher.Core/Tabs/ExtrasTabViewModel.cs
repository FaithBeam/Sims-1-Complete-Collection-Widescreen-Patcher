using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using sims_iff.Models;
using Sims1WidescreenPatcher.Core.Factories;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Tabs;

public interface IExtrasTabViewModel
{
    ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    CheckboxViewModel UnlockCheatsViewModel { get; }
    bool ApplyBtnVisible { get; }
    ReactiveCommand<Unit, Unit> ShowCareerEditorDialogCmd { get; set; }
    IInteraction<ICareerEditorTabViewModel, Iff?> ShowCareerEditorDialogInteraction { get; set; }
}

public class ExtrasTabViewModel : ViewModelBase, IExtrasTabViewModel
{
    private readonly ICheatsService _cheatsService;
    private readonly ICareerEditorTabViewModel _careerEditorTabViewModel;
    private readonly ObservableAsPropertyHelper<bool> _applyBtnVisible;
    private CheckboxSelectionSnapshot _previousSnapshot;
    private IAppState AppState { get; }

    public ExtrasTabViewModel(
        CheckboxViewModelFactory creator,
        ICheatsService cheatsService,
        IAppState appState,
        IProgressService progressService,
        ICareerEditorTabViewModel careerEditorTabViewModel
    )
    {
        _cheatsService = cheatsService;
        _careerEditorTabViewModel = careerEditorTabViewModel;
        AppState = appState;
        UnlockCheatsViewModel = (CheckboxViewModel)creator.Create("Unlock Cheats");
        UnlockCheatsViewModel.ToolTipText =
            "Unlock cheats that were disabled in the release build of the game.";
        UnlockCheatsViewModel.Checked = _cheatsService.CheatsEnabled();
        _previousSnapshot = new CheckboxSelectionSnapshot(UnlockCheatsViewModel.Checked);
        // make apply button visible/invisible if it is different from the previous state
        _applyBtnVisible = this.WhenAnyValue(
                x => x.UnlockCheatsViewModel.Checked,
                x => x.PreviousSnapshot,
                x => x.AppState.SimsExePath,
                (x, previousSnapshot, simsExePath) =>
                    (new CheckboxSelectionSnapshot(x), previousSnapshot, simsExePath)
            )
            .Select(x =>
            {
                if (!AppState.SimsExePathExists)
                    return false;
                return !x.Item1.Equals(x.previousSnapshot);
            })
            .ToProperty(this, x => x.ApplyBtnVisible);

        this.WhenAnyValue(x => x.AppState.SimsExePath)
            .Select(_ => AppState.SimsExePathExists && _cheatsService.CanEnableCheats())
            .Subscribe(x => UnlockCheatsViewModel.IsEnabled = x);

        progressService.Uninstall.Subscribe(_ =>
        {
            UnlockCheatsViewModel.Checked = _cheatsService.CheatsEnabled();
            PreviousSnapshot = new CheckboxSelectionSnapshot(UnlockCheatsViewModel.Checked);
        });

        ApplyCommand = ReactiveCommand.CreateFromTask(OnApplyClickedAsync);

        ShowCareerEditorDialogInteraction = new Interaction<ICareerEditorTabViewModel, Iff?>();
        ShowCareerEditorDialogCmd = ReactiveCommand.CreateFromTask(ShowCareerEditorDialogAsync);
    }

    private async Task<Unit> ShowCareerEditorDialogAsync()
    {
        var res = await ShowCareerEditorDialogInteraction.Handle(_careerEditorTabViewModel);
        return Unit.Default;
    }

    public ReactiveCommand<Unit, Unit> ShowCareerEditorDialogCmd { get; set; }
    public IInteraction<
        ICareerEditorTabViewModel,
        Iff?
    > ShowCareerEditorDialogInteraction { get; set; }

    private async Task OnApplyClickedAsync()
    {
        await Task.Run(() =>
        {
            if (UnlockCheatsViewModel.Checked)
            {
                _cheatsService.EnableCheats();
            }
            else
            {
                _cheatsService.DisableCheats();
            }
        });
        PreviousSnapshot = new CheckboxSelectionSnapshot(UnlockCheatsViewModel.Checked);
    }

    private CheckboxSelectionSnapshot PreviousSnapshot
    {
        get => _previousSnapshot;
        set => this.RaiseAndSetIfChanged(ref _previousSnapshot, value);
    }

    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }

    public CheckboxViewModel UnlockCheatsViewModel { get; }

    public bool ApplyBtnVisible => _applyBtnVisible.Value;
}
