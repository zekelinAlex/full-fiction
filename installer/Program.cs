using System;
using System.Windows.Forms;

namespace FullFiction.Setup
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Elevated relaunch: "--<action> <win64> [--auto]".
            if (args.Length >= 2 && TryParseAction(args[0], out var action))
            {
                var autoMode = args.Length >= 3 && args[2] == "--auto";
                return ActionRunner.Run(action, args[1], autoMode) ? 0 : 1;
            }

            Application.Run(new SetupForm());
            return 0;
        }

        private static bool TryParseAction(string arg, out SetupAction action)
        {
            return Enum.TryParse(arg.TrimStart('-'), ignoreCase: true, out action);
        }
    }
}
