# The Sims 1 Widescreen Patcher

This program patches `The Sims 1` to a custom resolution.

![Untitled](https://i.imgur.com/BwRreNH.png)

## Requirements

* NoCD/Crack Sims Executable
  * This exe cannot be previously patched to a custom resolution. For example, if you downloaded a crack that was patched to 1080p, this program will not work.

**Linux**

* xrandr
  * Ubuntu ```sudo apt install xrandr``` 
  * Fedora ```sudo dnf install xrandr```

### OS Version Compatability

| Windows |
|---------|
|    11   |
|    10   |
|  7 SP 1 |

| Ubuntu | Fedora |
|--------|--------|
| 16.04+ | 33+    |

| macOS  |
|--------|
| 10.15+ |

## Usage

1. Download the latest [release](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/releases)
2. Extract everything from the zip
3. Run Sims1WidescreenPatcher.exe as administrator
4. Select your Sims.exe
5. Enter your preferred resolution
6. Click Patch
7. Run "The Sims...800x600" shortcut or create your own from Sims.exe

**MacOS Note**

If you can't select the Sims.exe because it is installed in an app bundle from Wineskin Winery, click [here](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/MacOS---How-to-Enter-an-App-Bundle-to-Select-Sims-Exe) for a workaround.

## Graphics Wrappers

You can select between two graphics wrappers, [DgVoodoo2](http://dege.freeweb.hu/dgVoodoo2/dgVoodoo2/) and [DDrawCompat](https://github.com/narzoul/DDrawCompat). The short is they both fix graphical issues and improve performance. I recommend trying DDrawCompat first and if that doesn't work, try DgVoodoo2. If neither work, choose none.

**Wrapper OS Compatability**

|                 | Windows | Linux | macOS |
|-----------------|---------|-------|-------|
| **None**        | ✅       | ✅     | ✅     |
| **DGVoodoo2**   | ✅       | ✅     | ❌     |
| **DDrawCompat** | ✅       | ❌     | ❌     |

**Note**
If you use DDrawCompat and are using a GSync monitor, your game will crash. Refer to this page for the fix: [DDrawCompat-GSync-Fix](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/DDrawCompat-GSync-Fix)

## Uninstall

If you want to go back to your original Sims executable, select the uninstall button.

## Wiki

[General Sims recommendations to fix common issues](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/General-Sims-Recommendations)

[Linux Usage](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/Linux-Usage)

[Windowed Mode](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/Windowed-Mode)

[Screenshots](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/Screenshots)

## Credits

[WSGF](http://www.wsgf.org/dr/sims)

[PatternFinder](https://github.com/mrexodia/PatternFinder)

[DgVoodoo2](http://dege.freeweb.hu/dgVoodoo2/dgVoodoo2/)

[DDrawCompat](https://github.com/narzoul/DDrawCompat)

[Magick.NET](https://github.com/dlemstra/Magick.NET)

[Abulph](https://www.reddit.com/r/thesims/comments/6snibn/the_sims_1_widescreen_fix_1080p/) - Ideas to fix certain graphical issues

[thesimsone](https://www.deviantart.com/thesimsone) - High resolution The Sims icon
