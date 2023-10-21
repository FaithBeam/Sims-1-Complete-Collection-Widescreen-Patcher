using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Factories;

public class CheckboxViewModelFactory: UserControlViewModelCreator
{
    public override IReactiveObject Create(string label)
    {
        return new CheckboxViewModel(label, "");
    }

    public IReactiveObject Create(string label, string tooltip)
    {
        return new CheckboxViewModel(label, tooltip);
    }
}