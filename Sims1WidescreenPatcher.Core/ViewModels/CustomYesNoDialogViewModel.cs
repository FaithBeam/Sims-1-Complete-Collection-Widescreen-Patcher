using System.Reactive;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CustomYesNoDialogViewModel : ViewModelBase
{
	public ReactiveCommand<Unit, YesNoDialogResponse> YesCommand { get; } = ReactiveCommand.Create(() => new YesNoDialogResponse { Result = true });
	public ReactiveCommand<Unit, YesNoDialogResponse> NoCommand { get; } = ReactiveCommand.Create(() => new YesNoDialogResponse { Result = false });

	public string Title { get; set; } = "Default Title";
	public string Message { get; set; } = "Default message.";
}