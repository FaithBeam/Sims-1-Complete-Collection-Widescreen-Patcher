using System.Reactive;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Tabs;

public interface IExtrasTabViewModel
{
    ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    CheckboxViewModel UnlockCheatsViewModel { get; }
    bool ApplyBtnVisible { get; }
}
