using sims_iff.Models.ResourceContent.CARR;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

using ReactiveUI;

public class FieldViewModel : ReactiveObject
{
    private int _value;

    public int Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public FieldViewModel(Field field)
    {
        _value = field.Value;
    }
}
