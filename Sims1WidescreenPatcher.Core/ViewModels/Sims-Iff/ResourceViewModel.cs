using sims_iff.Interfaces;
using sims_iff.Models;
using sims_iff.Models.ResourceContent.CARR;
using sims_iff.Models.ResourceContent.Rsmp;
using sims_iff.Models.ResourceContent.Str.Format;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Rsmp;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Str.Format;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;

using ReactiveUI;

public class ResourceViewModel : ReactiveObject
{
    private TypeCodeViewModel _typeCode;
    private int _size;
    private ushort _id;
    private ushort _flags;
    private string _name;
    private IResourceContentViewModel _content;

    public TypeCodeViewModel TypeCode
    {
        get => _typeCode;
        set => this.RaiseAndSetIfChanged(ref _typeCode, value);
    }

    public int Size
    {
        get => _size;
        set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    public ushort Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public ushort Flags
    {
        get => _flags;
        set => this.RaiseAndSetIfChanged(ref _flags, value);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public IResourceContentViewModel Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public ResourceViewModel(Resource resource)
    {
        _typeCode = new TypeCodeViewModel(resource.TypeCode);
        _size = resource.Size;
        _id = resource.Id;
        _flags = resource.Flags;
        _name = resource.Name;
        _content = MapResourceContent(resource.Content);
    }

    private IResourceContentViewModel MapResourceContent(IResourceContent resourceContent) =>
        resourceContent switch
        {
            Carr carr => new CarrViewModel(carr),
            ResourceMap rsmp => new ResourceMapViewModel(rsmp),
            Fdff fdff => new FdffViewModel(fdff),
            _ => throw new ArgumentOutOfRangeException(nameof(resourceContent), resourceContent, null)
        };
}
