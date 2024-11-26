using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using sims_iff.Models;
using Sims1WidescreenPatcher.Core.Factories;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
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
    private const string UnlockCheatsKey = "UnlockCheats";
    private const string DomCalKey = "DomCal";
    private const string DomCalSalaryKey = "DomCalSalary";
    private readonly ICheatsService _cheatsService;
    private readonly ICareerEditorViewModelFactory _careerEditorViewModelFactory;
    private readonly IDomCalService _domCalService;
    private readonly ObservableAsPropertyHelper<bool> _applyBtnVisible;
    private CheckboxSelectionSnapshot _previousSnapshot;
    private IAppState AppState { get; }

    public ExtrasTabViewModel(
        CheckboxViewModelFactory creator,
        ICheatsService cheatsService,
        IAppState appState,
        IProgressService progressService,
        ICareerEditorViewModelFactory careerEditorViewModelFactory,
        IDomCalService domCalService
    )
    {
        _cheatsService = cheatsService;
        _careerEditorViewModelFactory = careerEditorViewModelFactory;
        _domCalService = domCalService;
        AppState = appState;
        UnlockCheatsViewModel = (CheckboxViewModel)creator.Create("Unlock Cheats");
        UnlockCheatsViewModel.ToolTipText =
            "Unlock cheats that were disabled in the release build of the game.";
        UnlockCheatsViewModel.Checked = _cheatsService.CheatsEnabled();

        DomcalCheckboxViewModel = (CheckboxViewModel)creator.Create("Weekends Off");
        DomcalCheckboxViewModel.ToolTipText =
            "dd_domcal mod: No work and school on the weekends.\nThis adds a hacked calendar that you can purchase that removes the need to work every 6th and 7th day.";
        DomcalCheckboxViewModel.Checked = domCalService.IsInstalled();

        DomcalAdjustCareerSalariesViewModel = (CheckboxViewModel)
            creator.Create("Adjust Salaries for DomCal Mod");
        DomcalAdjustCareerSalariesViewModel.ToolTipText =
            "This increases the salaries of jobs by about 30% when using the DomCal mod to even out the days lost from not working.";
        DomcalAdjustCareerSalariesViewModel.Checked = _domCalService.IsSalariesEdited();

        _previousSnapshot = new CheckboxSelectionSnapshot(
            (UnlockCheatsKey, UnlockCheatsViewModel.Checked),
            (DomCalKey, DomcalCheckboxViewModel.Checked),
            (DomCalSalaryKey, DomcalAdjustCareerSalariesViewModel.Checked)
        );
        // make apply button visible/invisible if it is different from the previous state
        _applyBtnVisible = this.WhenAnyValue(
                x => x.UnlockCheatsViewModel.Checked,
                x => x.DomcalCheckboxViewModel.Checked,
                x => x.DomcalAdjustCareerSalariesViewModel.Checked,
                x => x.PreviousSnapshot,
                x => x.AppState.SimsExePath,
                (unlockCheats, domcal, domCalSalary, previousSnapshot, simsExePath) =>
                    (
                        new CheckboxSelectionSnapshot(
                            (UnlockCheatsKey, unlockCheats),
                            (DomCalKey, domcal),
                            (DomCalSalaryKey, domCalSalary)
                        ),
                        previousSnapshot,
                        simsExePath
                    )
            )
            .Select(x =>
            {
                if (!AppState.SimsExePathExists)
                {
                    return false;
                }
                return !x.Item1.Equals(x.previousSnapshot);
            })
            .ToProperty(this, x => x.ApplyBtnVisible);

        this.WhenAnyValue(x => x.AppState.SimsExePath)
            .Select(_ => AppState.SimsExePathExists && _cheatsService.CanEnableCheats())
            .Subscribe(x => UnlockCheatsViewModel.IsEnabled = x);

        this.WhenAnyValue(x => x.AppState.SimsExePath)
            .Select(_ => AppState.SimsExePathExists && domCalService.CanInstall())
            .Subscribe(x => DomcalCheckboxViewModel.IsEnabled = x);

        this.WhenAnyValue(x => x.DomcalCheckboxViewModel.Checked)
            .Subscribe(x => DomcalAdjustCareerSalariesViewModel.IsEnabled = x);

        progressService.Uninstall.Subscribe(_ =>
        {
            UnlockCheatsViewModel.Checked = _cheatsService.CheatsEnabled();
            DomcalCheckboxViewModel.Checked = domCalService.IsInstalled();
            PreviousSnapshot = new CheckboxSelectionSnapshot(
                (UnlockCheatsKey, UnlockCheatsViewModel.Checked),
                (DomCalKey, DomcalCheckboxViewModel.Checked),
                (DomCalSalaryKey, DomcalAdjustCareerSalariesViewModel.Checked)
            );
        });

        ApplyCommand = ReactiveCommand.CreateFromTask(OnApplyClickedAsync);

        ShowCareerEditorDialogInteraction = new Interaction<ICareerEditorTabViewModel, Iff?>();
        ShowCareerEditorDialogCmd = ReactiveCommand.CreateFromTask(ShowCareerEditorDialogAsync);
    }

    private async Task<Unit> ShowCareerEditorDialogAsync()
    {
        var res = await ShowCareerEditorDialogInteraction.Handle(
            _careerEditorViewModelFactory.Create()
        );
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
            if (UnlockCheatsViewModel.Checked != PreviousSnapshot.States[UnlockCheatsKey])
            {
                if (UnlockCheatsViewModel.Checked)
                {
                    _cheatsService.EnableCheats();
                }
                else
                {
                    _cheatsService.DisableCheats();
                }
            }

            if (DomcalCheckboxViewModel.Checked != PreviousSnapshot.States[DomCalKey])
            {
                if (DomcalCheckboxViewModel.Checked)
                {
                    _domCalService.Install();
                }
                else
                {
                    _domCalService.Uninstall();
                }
            }

            if (
                DomcalAdjustCareerSalariesViewModel.Checked
                != PreviousSnapshot.States[DomCalSalaryKey]
            )
            {
                if (DomcalAdjustCareerSalariesViewModel.Checked)
                {
                    _domCalService.IncreaseSalaries();
                }
                else
                {
                    _domCalService.DecreaseSalaries();
                }
            }
        });
        PreviousSnapshot = new CheckboxSelectionSnapshot(
            (UnlockCheatsKey, UnlockCheatsViewModel.Checked),
            (DomCalKey, DomcalCheckboxViewModel.Checked),
            (DomCalSalaryKey, DomcalAdjustCareerSalariesViewModel.Checked)
        );
    }

    private CheckboxSelectionSnapshot PreviousSnapshot
    {
        get => _previousSnapshot;
        set => this.RaiseAndSetIfChanged(ref _previousSnapshot, value);
    }

    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }

    public CheckboxViewModel UnlockCheatsViewModel { get; }
    public CheckboxViewModel DomcalCheckboxViewModel { get; }
    public CheckboxViewModel DomcalAdjustCareerSalariesViewModel { get; }

    public bool ApplyBtnVisible => _applyBtnVisible.Value;
}
