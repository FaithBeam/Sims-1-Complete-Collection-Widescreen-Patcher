using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.Factories;

public abstract class UserControlViewModelCreator
{
    public abstract IReactiveObject FactoryMethod(string arg0);
}