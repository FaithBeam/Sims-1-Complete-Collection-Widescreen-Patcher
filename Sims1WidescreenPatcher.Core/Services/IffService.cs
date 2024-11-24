using System.Runtime.Serialization;
using sims_iff.Models;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IIffService
{
    IffViewModel Load(string pathToIff);
    void Write(string pathToIff, IffViewModel viewModel);
    void ApplyPreset(IEnumerable<JobInfoViewModel> jobInfoViewModels, IffPreset preset);
    Task<IffViewModel> LoadAsync(string pathToIff);
}

public enum IffPreset
{
    [EnumMember(Value = "Cap Decay At -5")]
    CapDecayAtNegative5,
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

    public void ApplyPreset(IEnumerable<JobInfoViewModel> jobInfoViewModels, IffPreset preset)
    {
        switch (preset)
        {
            case IffPreset.CapDecayAtNegative5:
                foreach (var vm in jobInfoViewModels)
                {
                    vm.FriendsRequired.Value = CapAtNegative5(vm.FriendsRequired.Value);
                    vm.CookingSkillRequired.Value = CapAtNegative5(vm.CookingSkillRequired.Value);
                    vm.MechanicalSkillRequired.Value = CapAtNegative5(
                        vm.MechanicalSkillRequired.Value
                    );
                    vm.CharismaRequired.Value = CapAtNegative5(vm.CharismaRequired.Value);
                    vm.BodySkillRequired.Value = CapAtNegative5(vm.BodySkillRequired.Value);
                    vm.LogicSkillRequired.Value = CapAtNegative5(vm.LogicSkillRequired.Value);
                    vm.CreativitySkillRequired.Value = CapAtNegative5(
                        vm.CreativitySkillRequired.Value
                    );
                    vm.HungerDecay.Value = CapAtNegative5(vm.HungerDecay.Value);
                    vm.ComfortDecay.Value = CapAtNegative5(vm.ComfortDecay.Value);
                    vm.HygieneDecay.Value = CapAtNegative5(vm.HygieneDecay.Value);
                    vm.BladderDecay.Value = CapAtNegative5(vm.BladderDecay.Value);
                    vm.EnergyDecay.Value = CapAtNegative5(vm.EnergyDecay.Value);
                    vm.FunDecay.Value = CapAtNegative5(vm.FunDecay.Value);
                    vm.SocialDecay.Value = CapAtNegative5(vm.SocialDecay.Value);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
        }
    }

    private static int CapAtNegative5(int value) => value < -5 ? -5 : value;
}
