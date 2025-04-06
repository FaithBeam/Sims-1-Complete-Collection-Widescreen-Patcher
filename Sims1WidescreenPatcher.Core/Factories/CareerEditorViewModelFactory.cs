using Sims.Far;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Factories;

public interface ICareerEditorViewModelFactory
{
    CareerEditorDialogViewModel Create();
}

public class CareerEditorViewModelFactory : ICareerEditorViewModelFactory
{
    private readonly IAppState _appState;
    private readonly IIffService _iffService;

    public CareerEditorViewModelFactory(IAppState appState, IIffService iffService)
    {
        _appState = appState;
        _iffService = iffService;
    }

    public CareerEditorDialogViewModel Create() =>
        new CareerEditorDialogViewModel(_appState, _iffService);
}
