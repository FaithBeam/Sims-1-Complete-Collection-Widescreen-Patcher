﻿using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface ICustomResolutionDialogViewModel
{
    ReactiveCommand<Unit, Resolution> OkCommand { get; }
    string Width { get; set; }
    string Height { get; set; }
    AspectRatio? AspectRatio { get; }
}

public class CustomResolutionDialogViewModel : ViewModelBase, ICustomResolutionDialogViewModel
{
    #region Fields

    private string _width = "";
    private string _height = "";
    private readonly ObservableAsPropertyHelper<AspectRatio?> _aspectRatio;

    #endregion

    #region Constructors

    public CustomResolutionDialogViewModel()
    {
        OkCommand = ReactiveCommand.Create(
            () => new Resolution(int.Parse(Width), int.Parse(Height))
        );
        _aspectRatio = this.WhenAnyValue(
                x => x.Width,
                x => x.Height,
                (w, h) =>
                {
                    if (!int.TryParse(w, out var width) || !int.TryParse(h, out var height))
                    {
                        return null;
                    }
                    return new AspectRatio(width, height);
                }
            )
            .Throttle(TimeSpan.FromMilliseconds(100))
            .ToProperty(this, x => x.AspectRatio);
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

    public AspectRatio? AspectRatio => _aspectRatio.Value;

    #endregion
}
