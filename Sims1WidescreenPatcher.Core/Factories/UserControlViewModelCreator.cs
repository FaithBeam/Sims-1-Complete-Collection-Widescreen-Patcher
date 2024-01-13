using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.Factories;

public abstract class UserControlViewModelCreator : IUserControlViewModelCreator
{
    public abstract IReactiveObject Create(string arg0);
}