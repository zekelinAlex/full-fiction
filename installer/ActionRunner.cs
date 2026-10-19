using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FullFiction.Setup
{
    internal static class ActionRunner
    {
        public const string Title = "Full Fiction";

        public static bool Run(SetupAction action, string win64, bool autoMode, IWin32Window owner = null)
        {
            if (action != SetupAction.Configure && ModInstaller.IsGameRunning())
            {
                MessageBox.Show(owner, UiText.CloseGameFirst, Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                switch (action)
                {
                    case SetupAction.Install:
                        ModInstaller.Install(win64, autoMode);
                        MessageBox.Show(owner, UiText.InstallDone(autoMode), Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case SetupAction.Uninstall:
                        ModInstaller.Uninstall(win64);
                        MessageBox.Show(owner, UiText.UninstallDone, Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case SetupAction.Configure:
                        ModInstaller.WriteAutoMode(win64, autoMode);
                        break;
                }

                return true;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || (ex is IOException && IsAccessDenied(ex)))
            {
                return RelaunchElevated(action, win64, autoMode, owner);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, UiText.Failed(ex.Message), Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static bool IsAccessDenied(Exception ex)
        {
            return (ex.HResult & 0xFFFF) == 5;
        }

        private static bool RelaunchElevated(SetupAction action, string win64, bool autoMode, IWin32Window owner)
        {
            if (IsElevated())
            {
                MessageBox.Show(owner, UiText.NoAccessEvenAsAdmin(win64), Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var answer = MessageBox.Show(owner, UiText.AskElevation, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return false;

            var startInfo = new ProcessStartInfo
            {
                FileName = Assembly.GetExecutingAssembly().Location,
                Arguments = "--" + action.ToString().ToLowerInvariant() + " \"" + win64 + "\"" + (autoMode ? " --auto" : ""),
                UseShellExecute = true,
                Verb = "runas",
            };

            try
            {
                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Win32Exception)
            {
                return false;
            }
        }

        private static bool IsElevated()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                return new System.Security.Principal.WindowsPrincipal(identity).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
    }
}
