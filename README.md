# The Sims 1 Widescreen Patcher

This program patches **The Sims 1** to a custom resolution.

![image](https://github.com/user-attachments/assets/6e3f2e81-4eee-4373-84e7-9a86bcbac2cb)

## Requirements

* NoCD/Cracked Sims Executable
  * This exe cannot be previously patched to a custom resolution. For example, if you downloaded a crack that was patched to 1080p, this program will not work.
 
### Linux Requirements

* X11
  * You don't need to run X11, but you do need it installed

### OS Version Compatability

| Windows |
|---------|
|    11   |
|    10   |
|  7 SP 1 |
| XP ([Use this older version](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/releases/tag/1.24.0-legacy))      |

| Ubuntu | Fedora |
|--------|--------|
| 16.04+ | 33+    |

| macOS  |
|--------|
| 10.15+ |

## Usage

1. Download the latest [release](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/releases)
2. Extract everything from the zip
3. (Windows) Run Sims1WidescreenPatcher.exe as administrator
4. Locate and select your Sims.exe
5. Select your preferred resolution & wrapper
6. Click "Patch" and answer any prompts that appear
7. Run "The Sims...800x600" shortcut or create your own from Sims.exe

**MacOS Notes**

* Disable the quarantine attribute on the bundle with this Terminal command: ```sudo xattr -r -d com.apple.quarantine ~/Downloads/artifacts/Sims1WidescreenPatcher.app/```
* If you can't select the Sims.exe because it is installed in an app bundle from Wineskin Winery, click [here](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/MacOS---How-to-Enter-an-App-Bundle-to-Select-Sims-Exe) for a workaround.

## Graphics Wrappers

You can select between two graphics wrappers, [DgVoodoo2](http://dege.freeweb.hu/dgVoodoo2/dgVoodoo2/) and [DDrawCompat](https://github.com/narzoul/DDrawCompat). The short is they both fix graphical issues and improve performance. I recommend trying DDrawCompat first and if that doesn't work, try DgVoodoo2. If neither work, choose none.

**Wrapper OS Compatability**

|                 | Windows | Linux | macOS |
|-----------------|---------|-------|-------|
| **None**        | ✅       | ✅     | ✅     |
| **DGVoodoo2**   | ✅       | ✅     | ❌     |
| **DDrawCompat** | ✅       | ❌     | ❌     |

**Note:**
If you use DDrawCompat and are using a monitor with G-Sync enabled, your game will crash in exclusive fullscreen mode. Please be sure to enable __Borderless Fullscreen mode__ when prompted during the patch to avoid this issue.

## Uninstall

To unpatch and return to the original executale, locate your Sims.exe again and choose "Uninstall." Answer any prompts that appear.

## Wiki

[General Sims recommendations to fix common issues](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/General-Sims-Recommendations)

[Configure DDrawCompat Wrapper](https://github.com/narzoul/DDrawCompat/wiki/Configuration)

[Windowed/Borderless Fullscreen/FSR Upscaling](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/Windowed,-Borderless-Fullscreen,-FSR-Upscaling)

[Screenshots](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/Screenshots)

## Credits

[WSGF](http://www.wsgf.org/dr/sims)

[PCGamingWiki](https://www.pcgamingwiki.com/wiki/The_Sims)

[PatternFinder](https://github.com/mrexodia/PatternFinder)

[DgVoodoo2](http://dege.freeweb.hu/dgVoodoo2/dgVoodoo2/)

[DDrawCompat](https://github.com/narzoul/DDrawCompat)

[Magick.NET](https://github.com/dlemstra/Magick.NET)

[Abulph](https://www.reddit.com/r/thesims/comments/6snibn/the_sims_1_widescreen_fix_1080p/) - Ideas to fix certain graphical issues

[thesimsone](https://www.deviantart.com/thesimsone) - High resolution The Sims icon
