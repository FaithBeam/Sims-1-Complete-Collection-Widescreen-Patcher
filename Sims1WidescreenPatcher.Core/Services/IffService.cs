using sims_iff.Models;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IIffService
{
    IffViewModel Load(string pathToIff);
}

public class IffService : IIffService
{
    public IffViewModel Load(string pathToIff)
    {
        return new IffViewModel(Iff.Read(pathToIff));
    }
}