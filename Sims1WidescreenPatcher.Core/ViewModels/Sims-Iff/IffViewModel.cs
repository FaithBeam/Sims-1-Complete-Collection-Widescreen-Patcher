namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;

using ReactiveUI;
using System.Collections.Generic;

public class IffViewModel : ReactiveObject
{
    private string _signature;
    private int _offsetToResourceMap;
    private List<Resource> _resources;

    public string Signature
    {
        get => _signature;
        set => this.RaiseAndSetIfChanged(ref _signature, value);
    }

    public int OffsetToResourceMap
    {
        get => _offsetToResourceMap;
        set => this.RaiseAndSetIfChanged(ref _offsetToResourceMap, value);
    }

    public List<Resource> Resources
    {
        get => _resources;
        set => this.RaiseAndSetIfChanged(ref _resources, value);
    }

    public IffViewModel(Iff iff)
    {
        _signature = iff.Signature;
        _offsetToResourceMap = iff.OffsetToResourceMap;
        _resources = iff.Resources;
    }
}
