namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent;

using ReactiveUI;

public class TypeCodeViewModel : ReactiveObject
{
    private string _value;
    private Endianness _endianness;

    public string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public Endianness Endianness
    {
        get => _endianness;
        set => this.RaiseAndSetIfChanged(ref _endianness, value);
    }

    public TypeCodeViewModel(TypeCode typeCode)
    {
        _value = typeCode.Value;
        _endianness = typeCode.Endianness;
    }
}
