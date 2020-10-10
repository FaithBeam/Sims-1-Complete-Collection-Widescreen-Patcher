# The Sims 1: Complete Collection Widescreen Patcher

Patches `The Sims 1: Complete Collection` to a custom resolution. You must be using a NoCD executable. This has only been tested with NoCD executable with MD5 hash 42F9A3E11BD1A03515C77777CB97B5BC. Running this application will:

1. Create a backup of your existing Sims.exe as Sims Backup.exe
2. Edit Sims.exe to your custom resolution
3. Optionally extracts dgVoodoo2
4. Resize UI graphics to your custom resolution

![Main Application](https://i.imgur.com/2Fwq3qR.png)

## Requirements

* .NET 4.0

## Usage

1. Download the latest [release](https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/releases)
2. Extract everything from the zip
3. Run Sims1WidescreenPatcher.exe as administrator
4. Select your Sims.exe
5. Enter your preferred resolution
6. Click Patch

You do not need to touch the Width, Between, and Height pattern text boxes. Those are there to offer flexibility in the program's pattern search. 

## Selecting Valid Resolutions

While this application does allow you to input any resolution you want, your game most likely will crash on startup if you don't enter a valid resolution your monitor supports. You can find out which resolutions your monitor supports by:

1. Right Clicking your desktop
2. Select Display Settings
3. Select Advanced display settings
4. Select Display adapter properties for Display 1
5. Select List All Modes

These are resolutions that will potentially work for The Sims 1. There are resolutions that don't work without dgVoodoo2, however. These resolutions are > 1080p.

I'm sure you can increase the range of valid resolutions by creating custom resolutions using [Custom Resolution Utility](https://www.monitortests.com/forum/Thread-Custom-Resolution-Utility-CRU)

## Uninstall

If you want to go back to your default Sims executable, select the uninstall button.

## Credits

[WSGF](http://www.wsgf.org/dr/sims)

[PatternFinder](https://github.com/mrexodia/PatternFinder)

[dgVoodoo2](http://dege.freeweb.hu/dgVoodoo2/dgVoodoo2.html)

[Magick.NET](https://github.com/dlemstra/Magick.NET)

[Abulph](https://www.reddit.com/r/thesims/comments/6snibn/the_sims_1_widescreen_fix_1080p/) - Ideas to fix certain graphical issues
