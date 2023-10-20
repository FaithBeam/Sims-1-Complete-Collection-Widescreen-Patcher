using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Factories;

public class CheckboxViewModelFactory: UserControlViewModelCreator
{
    public override IReactiveObject FactoryMethod(string label)
    {
        return new CheckboxViewModel(label);
    }
}