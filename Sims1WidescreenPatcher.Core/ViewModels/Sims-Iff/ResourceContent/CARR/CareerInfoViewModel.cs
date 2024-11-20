namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

using ReactiveUI;

public class CareerInfoViewModel : ReactiveObject
{
    private byte _compressionCode;
    private string _careerName;
    private Field _numberJobLevels;

    public byte CompressionCode
    {
        get => _compressionCode;
        set => this.RaiseAndSetIfChanged(ref _compressionCode, value);
    }

    public string CareerName
    {
        get => _careerName;
        set => this.RaiseAndSetIfChanged(ref _careerName, value);
    }

    public Field NumberJobLevels
    {
        get => _numberJobLevels;
        set => this.RaiseAndSetIfChanged(ref _numberJobLevels, value);
    }

    public CareerInfoViewModel(CareerInfo careerInfo)
    {
        _compressionCode = careerInfo.CompressionCode;
        _careerName = careerInfo.CareerName;
        _numberJobLevels = careerInfo.NumberJobLevels;
    }
}
