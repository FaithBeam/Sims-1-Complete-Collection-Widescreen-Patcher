using System.Runtime.Serialization;
using sims_iff.Models;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IIffService
{
    IffViewModel Load(string pathToIff);
    void Write(string pathToIff, IffViewModel viewModel);
    Task<IffViewModel> LoadAsync(string pathToIff);
}

public class IffService : IIffService
{
    public IffViewModel Load(string pathToIff)
    {
        return new IffViewModel(Iff.Read(pathToIff));
    }

    public async Task<IffViewModel> LoadAsync(string pathToIff)
    {
        return await Task.Run(() => Load(pathToIff));
    }

    public void Write(string pathToIff, IffViewModel viewModel)
    {
        var iff = viewModel.MapToIff();
        iff.Write(pathToIff);
    }
}
