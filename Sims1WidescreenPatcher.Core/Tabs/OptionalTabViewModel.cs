using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Factories;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Tabs;

public class OptionalTabViewModel : ViewModelBase, IOptionalTabViewModel
{
    private readonly ICheatsService _cheatsService;
    private readonly ObservableAsPropertyHelper<bool> _applyBtnVisible;
    private CheckboxSelectionSnapshot _previousSnapshot;
    private IAppState AppState { get; }

    public OptionalTabViewModel(CheckboxViewModelFactory creator, ICheatsService cheatsService, IAppState appState)
    {
        _cheatsService = cheatsService;
        AppState = appState;
        UnlockCheatsViewModel = (CheckboxViewModel)creator.Create("Unlock Cheats");
        UnlockCheatsViewModel.ToolTipText = "Unlock all cheats";
        UnlockCheatsViewModel.Checked = _cheatsService.CheatsEnabled();
        _previousSnapshot = new CheckboxSelectionSnapshot(UnlockCheatsViewModel.Checked);
        // make apply button visible/invisible if it is different from the previous state
        _applyBtnVisible = this
            .WhenAnyValue(x => x.UnlockCheatsViewModel.Checked, x => x.PreviousSnapshot, x => x.AppState.SimsExePath,
                (x, previousSnapshot, simsExePath) =>
                    (new CheckboxSelectionSnapshot(x), previousSnapshot, simsExePath))
            .Select(x =>
            {
                if (!AppState.SimsExePathExists) return false;
                return !x.Item1.Equals(x.previousSnapshot);
            })
            .ToProperty(this, x => x.ApplyBtnVisible);

        ApplyCommand = ReactiveCommand.CreateFromTask(OnApplyClickedAsync);
    }

    private async Task OnApplyClickedAsync()
    {
        PreviousSnapshot = new CheckboxSelectionSnapshot(UnlockCheatsViewModel.Checked);
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