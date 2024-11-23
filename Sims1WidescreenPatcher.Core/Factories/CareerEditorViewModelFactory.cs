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
    private readonly IFar _far;
    private readonly IIffService _iffService;

    public CareerEditorViewModelFactory(IAppState appState, IFar far, IIffService iffService)
    {
        _appState = appState;
        _far = far;
        _iffService = iffService;
    }

    public CareerEditorDialogViewModel Create() =>
        new CareerEditorDialogViewModel(_appState, _far, _iffService);
}
