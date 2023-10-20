using Sims1WidescreenPatcher.Core.Factories;
using Sims1WidescreenPatcher.Core.ViewModels;
using Splat;

namespace Sims1WidescreenPatcher.Core.Tabs;

public class OptionalTabViewModel : ViewModelBase, IOptionalTabViewModel
{
    public OptionalTabViewModel() : this(Locator.Current.GetService<UserControlViewModelCreator>() ?? throw new InvalidOperationException()) {}
    
    public OptionalTabViewModel(UserControlViewModelCreator creator)
    {
        UnlockCheatsViewModel = (CheckboxViewModel)creator.FactoryMethod("Unlock Cheats");
    }

    public CheckboxViewModel UnlockCheatsViewModel { get; }
}