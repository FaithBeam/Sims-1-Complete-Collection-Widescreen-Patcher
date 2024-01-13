using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.Factories;

public interface IUserControlViewModelCreator
{
    IReactiveObject Create(string arg0);
}