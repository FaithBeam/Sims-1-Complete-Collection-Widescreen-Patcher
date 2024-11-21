using sims_iff.Models.ResourceContent.Str;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Str;

using System.Reactive;
using ReactiveUI;

public class StringPairViewModel : ReactiveObject
{
    private string? _data;
    private string? _notes;

    public string? Data
    {
        get => _data;
        set => this.RaiseAndSetIfChanged(ref _data, value);
    }

    public string? Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    public StringPairViewModel(StringPair stringPair)
    {
        _data = stringPair.Data;
        _notes = stringPair.Notes;
    }
}
