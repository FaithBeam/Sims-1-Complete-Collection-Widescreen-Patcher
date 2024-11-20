namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;

using ReactiveUI;

public class ResourceViewModel : ReactiveObject
{
    private Models.ResourceContent.TypeCode _typeCode;
    private int _size;
    private ushort _id;
    private ushort _flags;
    private string _name;
    private IResourceContent _content;

    public Models.ResourceContent.TypeCode TypeCode
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

    public IResourceContent Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public ResourceViewModel(Resource resource)
    {
        _typeCode = resource.TypeCode;
        _size = resource.Size;
        _id = resource.Id;
        _flags = resource.Flags;
        _name = resource.Name;
        _content = resource.Content;
    }
}
