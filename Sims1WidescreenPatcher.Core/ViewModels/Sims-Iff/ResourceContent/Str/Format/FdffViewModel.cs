using sims_iff.Enums;
using sims_iff.Models.ResourceContent.Str.Format;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Str.Format;

using System.Collections.Generic;
using ReactiveUI;

public class FdffViewModel : ReactiveObject, IStrViewModel
{
    private StrFormat _format;
    private short _numberEntries;
    private List<CodeStringPairViewModel> _codeStringPairs;
    private int[] _extraData;

    public StrFormat Format
    {
        get => _format;
        set => this.RaiseAndSetIfChanged(ref _format, value);
    }

    public short NumberEntries
    {
        get => _numberEntries;
        set => this.RaiseAndSetIfChanged(ref _numberEntries, value);
    }

    public List<CodeStringPairViewModel> CodeStringPairs
    {
        get => _codeStringPairs;
        set => this.RaiseAndSetIfChanged(ref _codeStringPairs, value);
    }

    public int[] ExtraData
    {
        get => _extraData;
        set => this.RaiseAndSetIfChanged(ref _extraData, value);
    }

    public FdffViewModel(Fdff fdff)
    {
       _format = fdff.Format;
       _numberEntries = fdff.NumberEntries;
       _codeStringPairs = fdff.CodeStringPairs.Select(x => new CodeStringPairViewModel(x)).ToList();
       _extraData = fdff.ExtraData;
    }
}
