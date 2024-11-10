using System.Reactive;
using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface INotificationViewModel
{
    ReactiveCommand<Unit, Unit> WikiCommand { get; }
    bool IsVisible { get; set; }
    bool HasBeenShown { get; set; }
    ReactiveCommand<Unit, Unit> OkCommand { get; }
}
