using sims_iff.Models.ResourceContent.Rsmp;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Rsmp;

using System.Collections.Generic;
using ReactiveUI;

public class TypeListViewModel : ReactiveObject
{
    private TypeCodeViewModel _typeCode;
    private int _numberListEntries;
    private List<ListEntryViewModel> _listEntries;

    public TypeCodeViewModel TypeCode
    {
        get => _typeCode;
        set => this.RaiseAndSetIfChanged(ref _typeCode, value);
    }

    public int NumberListEntries
    {
        get => _numberListEntries;
        set => this.RaiseAndSetIfChanged(ref _numberListEntries, value);
    }

    public List<ListEntryViewModel> ListEntries
    {
        get => _listEntries;
        set => this.RaiseAndSetIfChanged(ref _listEntries, value);
    }

    public TypeListViewModel(TypeList typelist)
    {
        _typeCode = new TypeCodeViewModel(typelist.TypeCode);
        _numberListEntries = typelist.NumberListEntries;
        _listEntries = typelist.ListEntries.Select(x => new ListEntryViewModel(x)).ToList();
    }
}
