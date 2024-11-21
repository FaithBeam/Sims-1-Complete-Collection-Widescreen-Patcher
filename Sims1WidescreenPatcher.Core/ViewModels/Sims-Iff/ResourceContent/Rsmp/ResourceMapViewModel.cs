using sims_iff.Models.ResourceContent.Rsmp;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Rsmp;

using System.Collections.Generic;
using ReactiveUI;

public class ResourceMapViewModel : ReactiveObject, IResourceContentViewModel
{
    private int _field1;
    private int _version;
    private int _pmsr;
    private int _size;
    private int _numberOfTypes;
    private List<TypeListViewModel> _typeLists;

    public int Field1
    {
        get => _field1;
        set => this.RaiseAndSetIfChanged(ref _field1, value);
    }

    public int Version
    {
        get => _version;
        set => this.RaiseAndSetIfChanged(ref _version, value);
    }

    public int Pmsr
    {
        get => _pmsr;
        set => this.RaiseAndSetIfChanged(ref _pmsr, value);
    }

    public int Size
    {
        get => _size;
        set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    public int NumberOfTypes
    {
        get => _numberOfTypes;
        set => this.RaiseAndSetIfChanged(ref _numberOfTypes, value);
    }

    public List<TypeListViewModel> TypeLists
    {
        get => _typeLists;
        set => this.RaiseAndSetIfChanged(ref _typeLists, value);
    }

    public ResourceMapViewModel(ResourceMap resourceMap)
    {
        _field1 = resourceMap.Field1;
        _version = resourceMap.Version;
        _pmsr = resourceMap.Pmsr;
        _size = resourceMap.Size;
        _numberOfTypes = resourceMap.NumberOfTypes;
        _typeLists = resourceMap.TypeLists.Select(x => new TypeListViewModel(x)).ToList();
    }
}
