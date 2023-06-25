using System.Reactive;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CustomYesNoDialogViewModel : ViewModelBase
{
	public CustomYesNoDialogViewModel()
	{
		YesCommand = ReactiveCommand.Create(() => new YesNoDialogResponse { Result = true });
		NoCommand = ReactiveCommand.Create(() => new YesNoDialogResponse { Result = false });
	}

	public ReactiveCommand<Unit, YesNoDialogResponse> YesCommand { get; }
	public ReactiveCommand<Unit, YesNoDialogResponse> NoCommand { get; }

	public string Title { get; set; } = "Default Title";
	public string Message { get; set; } = "Default message.";
}