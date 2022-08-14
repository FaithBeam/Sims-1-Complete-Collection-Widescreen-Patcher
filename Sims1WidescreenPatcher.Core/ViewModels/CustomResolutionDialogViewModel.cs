using System.Reactive;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CustomResolutionDialogViewModel : ViewModelBase
{
    #region Fields

    private string _width = "";
    private string _height = "";

    #endregion

    #region Constructors

    public CustomResolutionDialogViewModel()
    {
        OkCommand = ReactiveCommand.Create(() => new Resolution(int.Parse(Width), int.Parse(Height)));
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Resolution> OkCommand { get; }

    #endregion

    #region Properties

    public string Width
    {
        get => _width;
        set => this.RaiseAndSetIfChanged(ref _width, value);
    }

    public string Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    #endregion
}