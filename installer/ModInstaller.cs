using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace FullFiction.Setup
{
    internal static class ModInstaller
    {
        private const string LoaderDll = "dwmapi.dll";
        private const string LoaderFolder = "ue4ss";
        private const string MarkerFile = "FullFiction.installed";
        private const string LegacyMarkerFile = "SplitFictionFullscreen.installed";
        private const string BackupSuffix = ".backup-";
        private static readonly string ConfigFile = Path.Combine(LoaderFolder, "Mods", "FullFiction", "Scripts", "fullfiction_config.lua");

        public static bool IsInstalled(string win64)
        {
            return File.Exists(Path.Combine(win64, LoaderFolder, MarkerFile))
                || File.Exists(Path.Combine(win64, LoaderFolder, LegacyMarkerFile));
        }

        public static bool IsOutdated(string win64)
        {
            var marker = Path.Combine(win64, LoaderFolder, MarkerFile);
            return IsInstalled(win64) && (!File.Exists(marker) || File.ReadAllText(marker).Trim() != CurrentVersion);
        }

        public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public static bool ReadAutoMode(string win64)
        {
            var path = Path.Combine(win64, ConfigFile);
            return File.Exists(path) && File.ReadAllText(path).Contains("AutoMode = true");
        }

        public static void WriteAutoMode(string win64, bool autoMode)
        {
            var content = "return { AutoMode = " + (autoMode ? "true" : "false") + " }\n";
            File.WriteAllText(Path.Combine(win64, ConfigFile), content);
        }

        public static bool IsGameRunning()
        {
            return Process.GetProcessesByName("SplitFiction").Any();
        }

        public static void Install(string win64, bool autoMode)
        {
            if (IsInstalled(win64))
            {
                RemoveOurFiles(win64);
            }
            else
            {
                BackUpForeignLoader(win64);
            }

            using (var payload = Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.zip"))
            using (var zip = new ZipArchive(payload, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    var target = Path.GetFullPath(Path.Combine(win64, entry.FullName));
                    if (!target.StartsWith(Path.GetFullPath(win64), StringComparison.OrdinalIgnoreCase)) continue;

                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(target);
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    entry.ExtractToFile(target, overwrite: true);
                }
            }

            File.WriteAllText(Path.Combine(win64, LoaderFolder, MarkerFile), CurrentVersion);
            WriteAutoMode(win64, autoMode);
        }

        public static void Uninstall(string win64)
        {
            RemoveOurFiles(win64);
            RestoreForeignLoader(win64);
        }

        private static void RemoveOurFiles(string win64)
        {
            var dll = Path.Combine(win64, LoaderDll);
            if (File.Exists(dll)) File.Delete(dll);

            var folder = Path.Combine(win64, LoaderFolder);
            if (Directory.Exists(folder)) Directory.Delete(folder, recursive: true);
        }

        // Someone else's UE4SS install is kept aside and put back on uninstall.
        private static void BackUpForeignLoader(string win64)
        {
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");

            var dll = Path.Combine(win64, LoaderDll);
            if (File.Exists(dll)) File.Move(dll, dll + BackupSuffix + stamp);

            var folder = Path.Combine(win64, LoaderFolder);
            if (Directory.Exists(folder)) Directory.Move(folder, folder + BackupSuffix + stamp);
        }

        private static void RestoreForeignLoader(string win64)
        {
            var dllBackup = LatestBackup(Directory.GetFiles(win64, LoaderDll + BackupSuffix + "*"));
            if (dllBackup != null) File.Move(dllBackup, Path.Combine(win64, LoaderDll));

            var folderBackup = LatestBackup(Directory.GetDirectories(win64, LoaderFolder + BackupSuffix + "*"));
            if (folderBackup != null) Directory.Move(folderBackup, Path.Combine(win64, LoaderFolder));
        }

        private static string LatestBackup(string[] paths)
        {
            return paths.OrderByDescending(p => p, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
        }
    }
}
