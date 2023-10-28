namespace Sims1WidescreenPatcher.Core.Services.Interfaces;

public interface IResolutionPatchService
{
    bool CanPatchResolution();
    bool BackupExists();
    void CreateBackup();
    void EditSimsExe();
}