$ErrorActionPreference = 'Stop'

$root = $PSScriptRoot
$ue4ss = Join-Path $root 'third_party\ue4ss\bin'
$gameConfig = Join-Path $root 'third_party\ue4ss\split-fiction-config'
$mod = Join-Path $root 'mod\FullFiction'
$project = Join-Path $root 'installer\FullFiction.Setup.csproj'
$build = Join-Path $root 'build'
$payload = Join-Path $build 'payload'
$payloadZip = Join-Path $build 'payload.zip'
$dist = Join-Path $root 'dist'

Remove-Item $build, $dist -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force (Join-Path $payload 'ue4ss\Mods') | Out-Null
New-Item -ItemType Directory -Force (Join-Path $payload 'ue4ss\UE4SS_Signatures') | Out-Null

Copy-Item (Join-Path $ue4ss 'dwmapi.dll') $payload
Copy-Item (Join-Path $ue4ss 'ue4ss\UE4SS.dll') (Join-Path $payload 'ue4ss')
Copy-Item (Join-Path $root 'third_party\ue4ss\LICENSE') (Join-Path $payload 'ue4ss\LICENSE-UE4SS')

Copy-Item (Join-Path $gameConfig 'MemberVariableLayout.ini') (Join-Path $payload 'ue4ss')
Copy-Item (Join-Path $gameConfig 'VTableLayout.ini') (Join-Path $payload 'ue4ss')
Copy-Item (Join-Path $gameConfig 'UE4SS_Signatures\*.lua') (Join-Path $payload 'ue4ss\UE4SS_Signatures')

# Start from the tested Split Fiction settings and keep only the engine tick hook the mod needs (ExecuteInGameThread).
$settings = Get-Content (Join-Path $gameConfig 'UE4SS-settings.ini') -Raw
$settings = [regex]::Replace($settings, '(?m)^(Hook(?!EngineTick\b)\w+\s*=\s*)1', '${1}0')
$settings = [regex]::Replace($settings, '(?m)^(bUseUObjectArrayCache\s*=\s*)true', '${1}false')
$settings = [regex]::Replace($settings, '(?m)^(ConsoleEnabled\s*=\s*)\d', '${1}0')
Set-Content (Join-Path $payload 'ue4ss\UE4SS-settings.ini') $settings -NoNewline -Encoding utf8

Copy-Item $mod (Join-Path $payload 'ue4ss\Mods') -Recurse
Set-Content (Join-Path $payload 'ue4ss\Mods\mods.txt') "FullFiction : 1" -Encoding ascii

Compress-Archive -Path (Join-Path $payload '*') -DestinationPath $payloadZip

dotnet build $project -c Release -o (Join-Path $build 'installer') --nologo -v q
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

$version = ([xml](Get-Content $project)).Project.PropertyGroup.Version
New-Item -ItemType Directory -Force $dist | Out-Null
Copy-Item (Join-Path $build 'installer\FullFiction-Setup.exe') $dist -Force
Copy-Item (Join-Path $root 'README-install.txt') (Join-Path $dist 'How to install (RU).txt') -Force
Copy-Item (Join-Path $root 'README-install-en.txt') (Join-Path $dist 'How to install.txt') -Force

$releaseZip = Join-Path $dist "FullFiction-v$version.zip"
Compress-Archive -Path (Join-Path $dist 'FullFiction-Setup.exe'), (Join-Path $dist 'How to install (RU).txt'), (Join-Path $dist 'How to install.txt') -DestinationPath $releaseZip

# Manual package without the exe: a "Split Fiction" folder that is merged into steamapps\common\Split Fiction.
$manual = Join-Path $build 'manual'
$manualWin64 = Join-Path $manual 'Split Fiction\Split\Binaries\Win64'
New-Item -ItemType Directory -Force $manualWin64 | Out-Null
Copy-Item (Join-Path $payload '*') $manualWin64 -Recurse
Set-Content (Join-Path $manualWin64 'ue4ss\Mods\FullFiction\Scripts\fullfiction_config.lua') "return { AutoMode = false }" -Encoding ascii
Set-Content (Join-Path $manualWin64 'ue4ss\FullFiction.installed') $version -NoNewline -Encoding ascii
Copy-Item (Join-Path $root 'README-manual.txt') (Join-Path $manual 'Full Fiction - manual install.txt')

$manualZip = Join-Path $dist "FullFiction-v$version-manual.zip"
Compress-Archive -Path (Join-Path $manual '*') -DestinationPath $manualZip

Write-Host "Payload:" -ForegroundColor Cyan
Get-ChildItem $payload -Recurse -File | ForEach-Object { '  ' + $_.FullName.Substring($payload.Length + 1) }
Write-Host "Hooks:" -ForegroundColor Cyan
Select-String -Path (Join-Path $payload 'ue4ss\UE4SS-settings.ini') -Pattern '^(Hook\w+|bUseUObjectArrayCache|ConsoleEnabled)\s*=' | ForEach-Object { '  ' + $_.Line }
Write-Host ("Release: " + $releaseZip) -ForegroundColor Green
Write-Host ("Manual:  " + $manualZip) -ForegroundColor Green
