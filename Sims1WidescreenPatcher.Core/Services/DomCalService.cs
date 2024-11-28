using System.Reflection;
using Sims.Far;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IDomCalService
{
    bool IsInstalled();
    Task Install();
    void Uninstall();
    bool CanInstall();
    void IncreaseSalaries();
    void DecreaseSalaries();
    bool IsSalariesEdited();
}

public class DomCalService : IDomCalService
{
    private const string DomcalName = "dd_domcal.iff";
    private readonly IAppState _appState;
    private readonly IFar _far;
    private readonly IIffService _iffService;

    public DomCalService(IAppState appState, IFar far, IIffService iffService)
    {
        _appState = appState;
        _far = far;
        _iffService = iffService;
    }

    public bool CanInstall()
    {
        var simsDir = Path.GetDirectoryName(_appState.SimsExePath);
        if (string.IsNullOrWhiteSpace(simsDir) || !Directory.Exists(simsDir))
        {
            return false;
        }
        var expansionSharedFolder = Path.Combine(simsDir, "ExpansionShared");
        return Directory.Exists(expansionSharedFolder);
    }

    public bool IsInstalled()
    {
        var simsDir = Path.GetDirectoryName(_appState.SimsExePath);
        if (string.IsNullOrWhiteSpace(simsDir) || !Directory.Exists(simsDir))
        {
            return false;
        }
        var expansionSharedFolder = Path.Combine(simsDir, "ExpansionShared");
        if (!Directory.Exists(expansionSharedFolder))
        {
            return false;
        }
        var domCalFile = Path.Combine(expansionSharedFolder, DomcalName);
        return File.Exists(domCalFile);
    }

    public async Task Install()
    {
        var simsDir = Path.GetDirectoryName(_appState.SimsExePath);
        if (string.IsNullOrWhiteSpace(simsDir) || !Directory.Exists(simsDir))
        {
            throw new DirectoryNotFoundException(simsDir);
        }
        var expansionSharedFolder = Path.Combine(simsDir, "ExpansionShared");
        if (!Directory.Exists(expansionSharedFolder))
        {
            throw new DirectoryNotFoundException(expansionSharedFolder);
        }
        var domCalFile = Path.Combine(expansionSharedFolder, DomcalName);
        if (File.Exists(domCalFile))
        {
            throw new InvalidOperationException($"Duplicate file: {domCalFile}");
        }

        const string resourceStream = "Sims1WidescreenPatcher.Core.Resources.dd_domcal.iff";
        await using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream(resourceStream);
        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Failed to load embedded resource: {resourceStream}"
            );
        }
        await using var fs = File.Create(domCalFile);
        await stream.CopyToAsync(fs);
    }

    public void Uninstall()
    {
        var simsDir = Path.GetDirectoryName(_appState.SimsExePath);
        if (string.IsNullOrWhiteSpace(simsDir) || !Directory.Exists(simsDir))
        {
            throw new DirectoryNotFoundException(simsDir);
        }
        var expansionSharedFolder = Path.Combine(simsDir, "ExpansionShared");
        if (!Directory.Exists(expansionSharedFolder))
        {
            throw new DirectoryNotFoundException(expansionSharedFolder);
        }
        var domCalFile = Path.Combine(expansionSharedFolder, DomcalName);
        if (File.Exists(domCalFile))
        {
            File.Delete(domCalFile);
        }
        else
        {
            throw new InvalidOperationException(
                $"Tried to delete {domCalFile} but it doesn't exist."
            );
        }
    }

    public void IncreaseSalaries()
    {
        var simsDir = Path.GetDirectoryName(_appState.SimsExePath);
        if (string.IsNullOrWhiteSpace(simsDir) || !Directory.Exists(simsDir))
        {
            throw new InvalidOperationException($"SimsDir {simsDir} does not exist.");
        }
        var expansionSharedFolder = Path.Combine(simsDir, "ExpansionShared");
        if (!Directory.Exists(expansionSharedFolder))
        {
            throw new InvalidOperationException($"SimsDir {expansionSharedFolder} does not exist.");
        }

        var workIffFile = Path.Combine(expansionSharedFolder, "work.iff");
        if (!File.Exists(workIffFile))
        {
            var expansionSharedFile = Path.Combine(expansionSharedFolder, "ExpansionShared.far");
            if (!File.Exists(expansionSharedFile))
            {
                throw new FileNotFoundException(expansionSharedFile);
            }
            _far.PathToFar = expansionSharedFile;
            _far.ParseFar();
            _far.Extract("work.iff", expansionSharedFolder);
            if (!File.Exists(workIffFile))
            {
                throw new FileNotFoundException(workIffFile);
            }
        }

        var salaryFile = Path.Combine(expansionSharedFolder, "salaries.txt");
        if (File.Exists(salaryFile))
        {
            throw new InvalidOperationException($"Salary file {salaryFile} already exists.");
        }
        using var salaryFs = File.Create(salaryFile);
        using var salarySw = new StreamWriter(salaryFs);
        salarySw.WriteLine("File generated by Sims1WidescreenPatcher, do not modify or delete.");
        var workIff = _iffService.Load(workIffFile);
        var carrViewModels = workIff
            .Resources.Where(r => r.TypeCode.Value == "CARR")
            .Select(c => c.Content)
            .Cast<CarrViewModel>();
        foreach (var cvm in carrViewModels)
        {
            foreach (var jivm in cvm.JobInfos)
            {
                salarySw.WriteLine(
                    $"{cvm.CareerInfo.CareerName}:{jivm.JobName}={jivm.Salary.Value}"
                );
                jivm.Salary.Value = (int)Math.Round(jivm.Salary.Value * 1.3);
            }
        }
        _iffService.Write(workIffFile, workIff);
    }

    public void DecreaseSalaries()
    {
        var simsDir = Path.GetDirectoryName(_appState.SimsExePath);
        if (string.IsNullOrWhiteSpace(simsDir) || !Directory.Exists(simsDir))
        {
            throw new InvalidOperationException($"SimsDir {simsDir} does not exist.");
        }

        var expansionSharedFolder = Path.Combine(simsDir, "ExpansionShared");
        if (!Directory.Exists(expansionSharedFolder))
        {
            throw new InvalidOperationException($"SimsDir {expansionSharedFolder} does not exist.");
        }

        var workIffFile = Path.Combine(expansionSharedFolder, "work.iff");
        if (!File.Exists(workIffFile))
        {
            throw new FileNotFoundException(workIffFile);
        }
        var salaryFile = Path.Combine(expansionSharedFolder, "salaries.txt");
        if (!File.Exists(salaryFile))
        {
            throw new FileNotFoundException(salaryFile);
        }

        var originalSalaries = File.ReadAllLines(salaryFile)[1..]
            .Select(l => l.Split('='))
            .GroupBy(x => x[0])
            .ToDictionary(l => l.Key, l => int.Parse(l.First()[1]));
        var workIff = _iffService.Load(workIffFile);
        var carrViewModels = workIff
            .Resources.Where(r => r.TypeCode.Value == "CARR")
            .Select(c => c.Content)
            .Cast<CarrViewModel>();
        foreach (var cvm in carrViewModels)
        {
            foreach (var jivm in cvm.JobInfos)
            {
                jivm.Salary.Value = originalSalaries[$"{cvm.CareerInfo.CareerName}:{jivm.JobName}"];
            }
        }
        _iffService.Write(workIffFile, workIff);
        File.Delete(salaryFile);
    }

    public bool IsSalariesEdited()
    {
        var simsDir = Path.GetDirectoryName(_appState.SimsExePath);
        if (string.IsNullOrWhiteSpace(simsDir) || !Directory.Exists(simsDir))
        {
            return false;
        }

        var expansionSharedFolder = Path.Combine(simsDir, "ExpansionShared");
        if (!Directory.Exists(expansionSharedFolder))
        {
            return false;
        }

        var workIffFile = Path.Combine(expansionSharedFolder, "work.iff");
        if (!File.Exists(workIffFile))
        {
            return false;
        }

        var salaryFile = Path.Combine(expansionSharedFolder, "salaries.txt");
        if (!File.Exists(salaryFile))
        {
            return false;
        }

        return true;
    }
}
