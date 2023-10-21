using Sims1WidescreenPatcher.Core.Factories;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.ViewModels;
using Splat;

namespace Sims1WidescreenPatcher.Core.Tabs;

public class OptionalTabViewModel : ViewModelBase, IOptionalTabViewModel
{
    private readonly ICheatsService _cheatsService;
    public OptionalTabViewModel() : this(Locator.Current.GetService<UserControlViewModelCreator>() ?? throw new InvalidOperationException(),
        Locator.Current.GetService<ICheatsService>() ?? throw new InvalidOperationException()) {}
    
    public OptionalTabViewModel(UserControlViewModelCreator creator, ICheatsService cheatsService)
    {
        _cheatsService = cheatsService;
        UnlockCheatsViewModel = (CheckboxViewModel)creator.Create("Unlock Cheats");
        UnlockCheatsViewModel.ToolTipText = "Unlock all cheats";
    }

    public CheckboxViewModel UnlockCheatsViewModel { get; }
}