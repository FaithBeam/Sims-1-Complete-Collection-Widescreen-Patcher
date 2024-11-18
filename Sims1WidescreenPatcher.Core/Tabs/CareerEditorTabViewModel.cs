using sims_iff.Models;
using Sims.Far;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Tabs;

public class CareerEditorTabViewModel : ViewModelBase
{
    private IAppState AppState { get; }
    private readonly IFar _far;
    private string? _pathToExpansionShared;
    private string? _pathToExpansionSharedFar;
    private string? _pathToWorkIff;
    private bool _workIffExtracted;
    private Iff? _workIff;

    public CareerEditorTabViewModel(IAppState appState, IFar far)
    {
        AppState = appState;
        _far = far;
        (_pathToExpansionShared, _pathToExpansionSharedFar) = GetPathToExpansionSharedDir(appState.SimsExePath);
        _pathToWorkIff = GetPathToWorkIff(_pathToExpansionShared);
        _workIffExtracted = GetWorkIffExtracted(_pathToWorkIff);
        
        
    }

    private void ExtractExpansionShared()
    {
        if (string.IsNullOrWhiteSpace(_pathToExpansionSharedFar) || !File.Exists(_pathToExpansionSharedFar))
        {
            return;
        }
        _far.PathToFar = _pathToExpansionSharedFar;
        _far.ParseFar();
        _far.Extract(_far.Manifest.ManifestEntries.First(x => x.Filename == "Work.iff"), _pathToExpansionShared);
    }

    private static bool GetWorkIffExtracted(string? pathToWorkIff) => File.Exists(pathToWorkIff);
    
    private static string? GetPathToWorkIff(string? expansionSharedDir) => string.IsNullOrWhiteSpace(expansionSharedDir) ? null : Path.Combine(expansionSharedDir, "Work.iff");

    private static (string? dir, string? file) GetPathToExpansionSharedDir(string? pathToSimsExe)
    {
        if (string.IsNullOrWhiteSpace(pathToSimsExe))
        {
            return (null, null);
        }
        var directory = Path.GetDirectoryName(pathToSimsExe);
        if (string.IsNullOrWhiteSpace(directory))
        {
            return (null, null);
        }
        var pathToExpansionSharedDir = Path.Combine(directory, "ExpansionShared");
        if (string.IsNullOrWhiteSpace(pathToExpansionSharedDir))
        {
            return (null, null);
        }
        var pathToExpansionSharedFar = Path.Combine(pathToExpansionSharedDir, "ExpansionShared.far");
        return (pathToExpansionSharedDir, pathToExpansionSharedFar);
    }
}