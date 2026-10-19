using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace FullFiction.Setup
{
    internal static class GameLocator
    {
        private const string ExeName = "SplitFiction.exe";
        private static readonly string RelativeWin64 = Path.Combine("Split", "Binaries", "Win64");

        public static string FindWin64Folder()
        {
            foreach (var library in GetSteamLibraries())
            {
                var candidate = Path.Combine(library, "steamapps", "common", "Split Fiction", RelativeWin64);
                if (File.Exists(Path.Combine(candidate, ExeName))) return candidate;
            }

            return null;
        }

        public static string ResolveWin64Folder(string selectedFolder)
        {
            if (string.IsNullOrWhiteSpace(selectedFolder)) return null;

            var candidates = new[]
            {
                selectedFolder,
                Path.Combine(selectedFolder, RelativeWin64),
                Path.Combine(selectedFolder, "Binaries", "Win64"),
            };

            return candidates.FirstOrDefault(c => File.Exists(Path.Combine(c, ExeName)));
        }

        private static IEnumerable<string> GetSteamLibraries()
        {
            var steamRoot = GetSteamRoot();
            if (steamRoot == null) yield break;

            yield return steamRoot;

            var vdf = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdf)) yield break;

            foreach (Match match in Regex.Matches(File.ReadAllText(vdf), "\"path\"\\s+\"(?<path>[^\"]+)\""))
            {
                yield return match.Groups["path"].Value.Replace("\\\\", "\\");
            }
        }

        private static string GetSteamRoot()
        {
            var fromUser = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string;
            if (!string.IsNullOrEmpty(fromUser) && Directory.Exists(fromUser)) return Path.GetFullPath(fromUser);

            var fromMachine = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string;
            if (!string.IsNullOrEmpty(fromMachine) && Directory.Exists(fromMachine)) return fromMachine;

            return null;
        }
    }
}
