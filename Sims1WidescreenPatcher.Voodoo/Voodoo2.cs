using Sims1WidescreenPatcher.IO;
using System.IO;
using System.IO.Compression;

namespace Sims1WidescreenPatcher.Voodoo
{
    public static class Voodoo2
    {
        public static void ExtractVoodooZips(string path)
        {
            string directory = Path.GetDirectoryName(path);
            TryRemoveDgVoodoo(directory);
            foreach (var zip in new string[] { @"Content\D3DCompiler_47.zip", @"Content\dgVoodoo2_64.zip" })
                ZipFile.ExtractToDirectory(zip, $@"{directory}\");

            foreach (var file in Directory.GetFiles($@"{directory}\MS\x86"))
                File.Move(file, $@"{directory}\{Path.GetFileName(file)}");

            DirectoryHelper.DeleteDirectory($@"{directory}\3Dfx");
            DirectoryHelper.DeleteDirectory($@"{directory}\Doc");
            DirectoryHelper.DeleteDirectory($@"{directory}\MS");

            string text = File.ReadAllText($@"{directory}\dgVoodoo.conf");
            text = text.Replace("dgVoodooWatermark                   = true", "dgVoodooWatermark                   = false");
            text = text.Replace("FastVideoMemoryAccess               = false", "FastVideoMemoryAccess               = true");
            File.WriteAllText($@"{directory}\dgVoodoo.conf", text);
        }

        public static void TryRemoveDgVoodoo(string path)
        {
            var directory = Path.GetDirectoryName(path);
            FileHelper.DeleteFile($@"{directory}\d3dcompiler_47.dll");
            FileHelper.DeleteFile($@"{directory}\D3D8.dll");
            FileHelper.DeleteFile($@"{directory}\D3D9.dll");
            FileHelper.DeleteFile($@"{directory}\D3DImm.dll");
            FileHelper.DeleteFile($@"{directory}\DDraw.dll");
            FileHelper.DeleteFile($@"{directory}\dgVoodoo.conf");
            FileHelper.DeleteFile($@"{directory}\dgVoodooCpl.exe");
            FileHelper.DeleteFile($@"{directory}\QuickGuide.html");
            FileHelper.DeleteFile($@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp");
            DirectoryHelper.DeleteDirectory($@"{directory}\3Dfx");
            DirectoryHelper.DeleteDirectory($@"{directory}\Doc");
            DirectoryHelper.DeleteDirectory($@"{directory}\MS");
        }
    }
}
