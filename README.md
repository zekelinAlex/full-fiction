# Full Fiction

A [Split Fiction](https://store.steampowered.com/app/2001120/Split_Fiction/) mod that gives each player their own full screen in online co-op instead of the always-on split screen.

Press **F10** in game to cycle: split screen > your view fullscreen > partner's view fullscreen. Cutscenes are left to the game. An optional auto mode switches to your view after every cutscene and loading screen.

The mod is client-side only. Your partner doesn't need it.

**Download:** [Full Fiction on Nexus Mods](https://www.nexusmods.com/splitfiction/mods/16)

## How it works

Hazelight's engine already supports giving one player the whole screen (the game uses it in some story moments). The mod is a small [UE4SS](https://github.com/UE4SS-RE/RE-UE4SS) Lua script that calls the game's own reflected functions:

- `UHazeGameStatics::GetFirstLocalPlayer` to find the player controlled on this PC
- `UHazeSceneViewStatics::SetViewSizeOverride` / `ClearViewSizeOverride` to resize the views
- `bIsControlledByCutscene` / `bIsParticipatingInCutscene` and `GetSplitScreenMode` / `IsFullScreen` to step aside when the game controls the screen itself

No game files are modified.

## Repository layout

| Path | What |
| --- | --- |
| `mod/FullFiction/Scripts/main.lua` | the mod itself |
| `installer/` | `FullFiction-Setup.exe`, a WinForms installer (.NET Framework 4.8) with the mod files embedded |
| `third_party/ue4ss/` | vendored UE4SS binaries and the community Split Fiction config ([details](third_party/ue4ss/README.md)) |
| `build.ps1` | builds everything into `dist/` |

## Building

Requirements: Windows, PowerShell 7 (`pwsh`), .NET SDK 6 or newer (the installer targets .NET Framework 4.8 through the SDK).

```powershell
pwsh -File build.ps1
```

Output in `dist/`:

- `FullFiction-v<version>.zip`: installer plus install guides
- `FullFiction-v<version>-manual.zip`: a `Split Fiction` folder to merge into `steamapps\common\Split Fiction`, no exe

`build.ps1` assembles the payload (UE4SS + Split Fiction config + mod), trims the UE4SS hooks to the single `EngineTick` hook the mod needs, embeds the payload into the installer as a resource and builds it with `dotnet build`.

## Installing

See [README-install-en.txt](README-install-en.txt) (installer) or [README-manual.txt](README-manual.txt) (manual).

## License

The mod and installer are MIT licensed, see [LICENSE](LICENSE). UE4SS is MIT licensed by its authors, see [third_party/ue4ss/LICENSE](third_party/ue4ss/LICENSE).
