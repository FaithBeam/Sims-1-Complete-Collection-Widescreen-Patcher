using sims_iff.Models;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IIffService
{
    IffViewModel Load(string pathToIff);
    void Write(string pathToIff, IffViewModel viewModel);
    void ApplyPreset(IEnumerable<JobInfoViewModel> jobInfoViewModels, IffPreset preset);
}

public enum IffPreset
{
    Default,
    Cheater,
}

public class IffService : IIffService
{
    public IffViewModel Load(string pathToIff)
    {
        return new IffViewModel(Iff.Read(pathToIff));
    }

    public void Write(string pathToIff, IffViewModel viewModel)
    {
        var iff = viewModel.MapToIff();
        iff.Write(pathToIff);
    }

    public void ApplyPreset(IEnumerable<JobInfoViewModel> jobInfoViewModels, IffPreset preset)
    {
        switch (preset)
        {
            case IffPreset.Default:
                break;
            case IffPreset.Cheater:
                foreach (var vm in jobInfoViewModels)
                {
                    vm.FriendsRequired.Value = 0;
                    vm.CookingSkillRequired.Value = 0;
                    vm.MechanicalSkillRequired.Value = 0;
                    vm.CharismaRequired.Value = 0;
                    vm.BodySkillRequired.Value = 0;
                    vm.LogicSkillRequired.Value = 0;
                    vm.CreativitySkillRequired.Value = 0;
                    vm.HungerDecay.Value = 0;
                    vm.ComfortDecay.Value = 0;
                    vm.HygieneDecay.Value = 0;
                    vm.BladderDecay.Value = 0;
                    vm.EnergyDecay.Value = 0;
                    vm.FunDecay.Value = 0;
                    vm.SocialDecay.Value = 0;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
        }
    }
}
