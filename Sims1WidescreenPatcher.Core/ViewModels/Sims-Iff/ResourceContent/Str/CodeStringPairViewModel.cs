using sims_iff.Enums;
using sims_iff.Models.ResourceContent.Str;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Str;

using ReactiveUI;

public class CodeStringPairViewModel : ReactiveObject
{
    private LanguageCode _languageCode;
    private StringPairViewModel _stringPair;

    public LanguageCode LanguageCode
    {
        get => _languageCode;
        set => this.RaiseAndSetIfChanged(ref _languageCode, value);
    }

    public StringPairViewModel StringPair
    {
        get => _stringPair;
        set => this.RaiseAndSetIfChanged(ref _stringPair, value);
    }

    public CodeStringPairViewModel(CodeStringPair codeStringPair)
    {
        _languageCode = codeStringPair.LanguageCode;
        _stringPair = new StringPairViewModel(codeStringPair.StringPair);
    }

    public CodeStringPair MapToCodeStringPair() => new(LanguageCode, StringPair.MapToStringPair());
}
