using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

public class CarrViewModel : ReactiveObject
{
    private int _field1;
    private int _field2;
    private string _rrac;
    private CareerInfo _careerInfo;
    private List<JobInfo> _jobInfos;

    public int Field1
    {
        get => _field1;
        set => this.RaiseAndSetIfChanged(ref _field1, value);
    }

    public int Field2
    {
        get => _field2;
        set => this.RaiseAndSetIfChanged(ref _field2, value);
    }

    public string Rrac
    {
        get => _rrac;
        set => this.RaiseAndSetIfChanged(ref _rrac, value);
    }

    public CareerInfo CareerInfo
    {
        get => _careerInfo;
        set => this.RaiseAndSetIfChanged(ref _careerInfo, value);
    }

    public List<JobInfo> JobInfos
    {
        get => _jobInfos;
        set => this.RaiseAndSetIfChanged(ref _jobInfos, value);
    }

    public CarrViewModel(Carr carr)
    {
        _field1 = carr.Field1;
        _field2 = carr.Field2;
        _rrac = carr.Rrac;
        _careerInfo = carr.CareerInfo;
        _jobInfos = carr.JobInfos;
    }
}
