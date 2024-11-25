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

    [EnumMember(Value = "No Decay")]
    NoDecay,
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
                    vm.FriendsRequired.Value = CapValue(-5, vm.FriendsRequired.Value);
                    vm.CookingSkillRequired.Value = CapValue(-5, vm.CookingSkillRequired.Value);
                    vm.MechanicalSkillRequired.Value = CapValue(
                        -5,
                        vm.MechanicalSkillRequired.Value
                    );
                    vm.CharismaRequired.Value = CapValue(-5, vm.CharismaRequired.Value);
                    vm.BodySkillRequired.Value = CapValue(-5, vm.BodySkillRequired.Value);
                    vm.LogicSkillRequired.Value = CapValue(-5, vm.LogicSkillRequired.Value);
                    vm.CreativitySkillRequired.Value = CapValue(
                        -5,
                        vm.CreativitySkillRequired.Value
                    );
                    vm.HungerDecay.Value = CapValue(-5, vm.HungerDecay.Value);
                    vm.ComfortDecay.Value = CapValue(-5, vm.ComfortDecay.Value);
                    vm.HygieneDecay.Value = CapValue(-5, vm.HygieneDecay.Value);
                    vm.BladderDecay.Value = CapValue(-5, vm.BladderDecay.Value);
                    vm.EnergyDecay.Value = CapValue(-5, vm.EnergyDecay.Value);
                    vm.FunDecay.Value = CapValue(-5, vm.FunDecay.Value);
                    vm.SocialDecay.Value = CapValue(-5, vm.SocialDecay.Value);
                }
                break;
            case IffPreset.NoDecay:
                foreach (var vm in jobInfoViewModels)
                {
                    vm.FriendsRequired.Value = CapValue(0, vm.FriendsRequired.Value);
                    vm.CookingSkillRequired.Value = CapValue(0, vm.CookingSkillRequired.Value);
                    vm.MechanicalSkillRequired.Value = CapValue(
                        0,
                        vm.MechanicalSkillRequired.Value
                    );
                    vm.CharismaRequired.Value = CapValue(0, vm.CharismaRequired.Value);
                    vm.BodySkillRequired.Value = CapValue(0, vm.BodySkillRequired.Value);
                    vm.LogicSkillRequired.Value = CapValue(0, vm.LogicSkillRequired.Value);
                    vm.CreativitySkillRequired.Value = CapValue(
                        0,
                        vm.CreativitySkillRequired.Value
                    );
                    vm.HungerDecay.Value = CapValue(0, vm.HungerDecay.Value);
                    vm.ComfortDecay.Value = CapValue(0, vm.ComfortDecay.Value);
                    vm.HygieneDecay.Value = CapValue(0, vm.HygieneDecay.Value);
                    vm.BladderDecay.Value = CapValue(0, vm.BladderDecay.Value);
                    vm.EnergyDecay.Value = CapValue(0, vm.EnergyDecay.Value);
                    vm.FunDecay.Value = CapValue(0, vm.FunDecay.Value);
                    vm.SocialDecay.Value = CapValue(0, vm.SocialDecay.Value);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
        }
    }

    private static int CapValue(int cap, int value) => value < cap ? cap : value;
}
