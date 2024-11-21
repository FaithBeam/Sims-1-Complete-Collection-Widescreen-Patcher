using sims_iff.Models.ResourceContent.Rsmp;

namespace Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.Rsmp;

using ReactiveUI;

public class ListEntryViewModel : ReactiveObject
{
    private int _offset;
    private ushort _id;
    private int _version;
    private ushort? _unknownField;
    private ushort _flags;
    private string _name;

    public int Offset
    {
        get => _offset;
        set => this.RaiseAndSetIfChanged(ref _offset, value);
    }

    public ushort Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public int Version
    {
        get => _version;
        set => this.RaiseAndSetIfChanged(ref _version, value);
    }

    public ushort? UnknownField
    {
        get => _unknownField;
        set => this.RaiseAndSetIfChanged(ref _unknownField, value);
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

    public ListEntryViewModel(ListEntry listEntry)
    {
        _offset = listEntry.Offset;
        _id = listEntry.Id;
        _version = listEntry.Version;
        _unknownField = listEntry.UnknownField;
        _flags = listEntry.Flags;
        _name = listEntry.Name;
    }
}
