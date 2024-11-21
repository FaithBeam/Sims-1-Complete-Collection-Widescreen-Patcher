using sims_iff.Models.ResourceContent.CARR;
using sims_iff.Models.ResourceContent.Str;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

using ReactiveUI;

public class JobInfoViewModel : ReactiveObject
{
    private FieldViewModel _friendsRequired;
    private FieldViewModel _cookingSkillRequired;
    private FieldViewModel _mechanicalSkillRequired;
    private FieldViewModel _charismaRequired;
    private FieldViewModel _bodySkillRequired;
    private FieldViewModel _logicSkillRequired;
    private FieldViewModel _creativitySkillRequired;
    private FieldViewModel _unknown1;
    private FieldViewModel _unknown2;
    private FieldViewModel _unknown3;
    private FieldViewModel _hungerDecay;
    private FieldViewModel _comfortDecay;
    private FieldViewModel _hygieneDecay;
    private FieldViewModel _bladderDecay;
    private FieldViewModel _energyDecay;
    private FieldViewModel _funDecay;
    private FieldViewModel _socialDecay;
    private FieldViewModel _salary;
    private FieldViewModel _startTime;
    private FieldViewModel _endTime;
    private CarType _carType;
    private string _jobName;
    private string _maleUniformMesh;
    private string _femaleUniformMesh;
    private string _uniformSkin;
    private string _unknown4;

    public FieldViewModel FriendsRequired
    {
        get => _friendsRequired;
        set => this.RaiseAndSetIfChanged(ref _friendsRequired, value);
    }

    public FieldViewModel CookingSkillRequired
    {
        get => _cookingSkillRequired;
        set => this.RaiseAndSetIfChanged(ref _cookingSkillRequired, value);
    }

    public FieldViewModel MechanicalSkillRequired
    {
        get => _mechanicalSkillRequired;
        set => this.RaiseAndSetIfChanged(ref _mechanicalSkillRequired, value);
    }

    public FieldViewModel CharismaRequired
    {
        get => _charismaRequired;
        set => this.RaiseAndSetIfChanged(ref _charismaRequired, value);
    }

    public FieldViewModel BodySkillRequired
    {
        get => _bodySkillRequired;
        set => this.RaiseAndSetIfChanged(ref _bodySkillRequired, value);
    }

    public FieldViewModel LogicSkillRequired
    {
        get => _logicSkillRequired;
        set => this.RaiseAndSetIfChanged(ref _logicSkillRequired, value);
    }

    public FieldViewModel CreativitySkillRequired
    {
        get => _creativitySkillRequired;
        set => this.RaiseAndSetIfChanged(ref _creativitySkillRequired, value);
    }

    public FieldViewModel Unknown1
    {
        get => _unknown1;
        set => this.RaiseAndSetIfChanged(ref _unknown1, value);
    }

    public FieldViewModel Unknown2
    {
        get => _unknown2;
        set => this.RaiseAndSetIfChanged(ref _unknown2, value);
    }

    public FieldViewModel Unknown3
    {
        get => _unknown3;
        set => this.RaiseAndSetIfChanged(ref _unknown3, value);
    }

    public FieldViewModel HungerDecay
    {
        get => _hungerDecay;
        set => this.RaiseAndSetIfChanged(ref _hungerDecay, value);
    }

    public FieldViewModel ComfortDecay
    {
        get => _comfortDecay;
        set => this.RaiseAndSetIfChanged(ref _comfortDecay, value);
    }

    public FieldViewModel HygieneDecay
    {
        get => _hygieneDecay;
        set => this.RaiseAndSetIfChanged(ref _hygieneDecay, value);
    }

    public FieldViewModel BladderDecay
    {
        get => _bladderDecay;
        set => this.RaiseAndSetIfChanged(ref _bladderDecay, value);
    }

    public FieldViewModel EnergyDecay
    {
        get => _energyDecay;
        set => this.RaiseAndSetIfChanged(ref _energyDecay, value);
    }

    public FieldViewModel FunDecay
    {
        get => _funDecay;
        set => this.RaiseAndSetIfChanged(ref _funDecay, value);
    }

    public FieldViewModel SocialDecay
    {
        get => _socialDecay;
        set => this.RaiseAndSetIfChanged(ref _socialDecay, value);
    }

    public FieldViewModel Salary
    {
        get => _salary;
        set => this.RaiseAndSetIfChanged(ref _salary, value);
    }

    public FieldViewModel StartTime
    {
        get => _startTime;
        set => this.RaiseAndSetIfChanged(ref _startTime, value);
    }

    public FieldViewModel EndTime
    {
        get => _endTime;
        set => this.RaiseAndSetIfChanged(ref _endTime, value);
    }

    public CarType CarType
    {
        get => _carType;
        set => this.RaiseAndSetIfChanged(ref _carType, value);
    }

    public string JobName
    {
        get => _jobName;
        set => this.RaiseAndSetIfChanged(ref _jobName, value);
    }

    public string MaleUniformMesh
    {
        get => _maleUniformMesh;
        set => this.RaiseAndSetIfChanged(ref _maleUniformMesh, value);
    }

    public string FemaleUniformMesh
    {
        get => _femaleUniformMesh;
        set => this.RaiseAndSetIfChanged(ref _femaleUniformMesh, value);
    }

    public string UniformSkin
    {
        get => _uniformSkin;
        set => this.RaiseAndSetIfChanged(ref _uniformSkin, value);
    }

    public string Unknown4
    {
        get => _unknown4;
        set => this.RaiseAndSetIfChanged(ref _unknown4, value);
    }

    public JobInfoViewModel(JobInfo jobInfo)
    {
        _friendsRequired = new FieldViewModel(jobInfo.FriendsRequired);
        _cookingSkillRequired = new FieldViewModel(jobInfo.CookingSkillRequired);
        _mechanicalSkillRequired = new FieldViewModel(jobInfo.MechanicalSkillRequired);
        _charismaRequired = new FieldViewModel(jobInfo.CharismaRequired);
        _bodySkillRequired = new FieldViewModel(jobInfo.BodySkillRequired);
        _logicSkillRequired = new FieldViewModel(jobInfo.LogicSkillRequired);
        _creativitySkillRequired = new FieldViewModel(jobInfo.CreativitySkillRequired);
        _unknown1 = new FieldViewModel(jobInfo.Unknown1);
        _unknown2 = new FieldViewModel(jobInfo.Unknown2);
        _unknown3 = new FieldViewModel(jobInfo.Unknown3);
        _hungerDecay = new FieldViewModel(jobInfo.HungerDecay);
        _comfortDecay = new FieldViewModel(jobInfo.ComfortDecay);
        _hygieneDecay = new FieldViewModel(jobInfo.HygieneDecay);
        _bladderDecay = new FieldViewModel(jobInfo.BladderDecay);
        _energyDecay = new FieldViewModel(jobInfo.EnergyDecay);
        _funDecay = new FieldViewModel(jobInfo.FunDecay);
        _socialDecay = new FieldViewModel(jobInfo.SocialDecay);
        _salary = new FieldViewModel(jobInfo.Salary);
        _startTime = new FieldViewModel(jobInfo.StartTime);
        _endTime = new FieldViewModel(jobInfo.EndTime);
        _carType = jobInfo.CarType;
        _jobName = jobInfo.JobName;
        _maleUniformMesh = jobInfo.MaleUniformMesh;
        _femaleUniformMesh = jobInfo.FemaleUniformMesh;
        _uniformSkin = jobInfo.UniformSkin;
        _unknown4 = jobInfo.Unknown4;
    }
}
