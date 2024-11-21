using sims_iff.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;

using System.Collections.Generic;
using ReactiveUI;

public class IffViewModel : ReactiveObject
{
    private string _signature;
    private int _offsetToResourceMap;
    private List<ResourceViewModel> _resources;

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

    public List<ResourceViewModel> Resources
    {
        get => _resources;
        set => this.RaiseAndSetIfChanged(ref _resources, value);
    }

    public IffViewModel(Iff iff)
    {
        _signature = iff.Signature;
        _offsetToResourceMap = iff.OffsetToResourceMap;
        _resources = iff.Resources.Select(x => new ResourceViewModel(x)).ToList();
    }

    public Iff MapToIff() =>
        new Iff(Signature, OffsetToResourceMap, Resources.Select(x => x.MapToResource()).ToList());
}
