# IOSubtitles
Subtitles for the game Insult Order. This plugin adds subtitles during the "gameplay". The subtitles are generated with [Whisper AI](https://github.com/openai/whisper) and slightly edited.

## Download
Check [Releases](https://github.com/Rieght/IOSubtitles/releases)

## How to use
Extract the files IOSubtitles.dll and subtitles.json into your "GameData\BepInEx\plugins" folder inside the game directory.

The fontsize can be changed in "GameData\BepInEx\config\rieght.insultorder.iosubtitles.cfg" (Game needs to run atleast once) or via the F12-menu (Updates after changing/reloading the scene).

## How to contribute
- Fork the repository.
- Make the changes.
- Submit a pull request.

### File Format
The "subtitles.json"-file has to be in the following format.
```
{
  "AUDIOCLIP1":
    [
      [
        "LINE1_ENDTIME",
        "LINE1_TEXT"
      ],
      ...
    ],
  ...
}
```

### View AudioClips with AssetStudio
- Download [AssetStudio](https://github.com/Perfare/AssetStudio)
- Extract and run AssetStudioGUI.exe
- In the top left corner select File -> Load folder and choose GameData/io_Data
- After the assets are loaded change the view to Asset List
- Change the Filter Type to AudioClip
- Select a AudioClip and play it

### Activate log of currently playing AudioClips
Open GameData/BepInEx/Config/BepInEx.cfg and add 'Debug' to the LogChannels (or set it to 'All'). 
The currently playing AudioClips should be printing in the console.
```
## Specifies which Harmony log channels to listen to.
## NOTE: IL channel dumps the whole patch methods, use only when needed!
# Setting type: LogChannel
# Default value: Warn, Error
# Acceptable values: None, Info, IL, Warn, Error, Debug, All
# Multiple values can be set at the same time by separating them with , (e.g. Debug, Warning)
LogChannels = Info, Error, Debug
```

## My other plugins
[IOLipsync](https://github.com/Rieght/IOLipsync)
