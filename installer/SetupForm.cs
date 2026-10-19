using System;
using System.Drawing;
using System.Windows.Forms;

namespace FullFiction.Setup
{
    internal sealed class SetupForm : Form
    {
        private readonly Label pathLabel;
        private readonly Label statusLabel;
        private readonly CheckBox autoModeCheckBox;
        private readonly Button installButton;
        private readonly Button uninstallButton;
        private string win64;
        private bool refreshing;

        public SetupForm()
        {
            Text = ActionRunner.Title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(520, 262);
            Font = new Font("Segoe UI", 9.5f);

            var header = new Label
            {
                Text = "Full Fiction",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 14),
            };

            var description = new Label
            {
                Text = UiText.Description,
                AutoSize = true,
                Location = new Point(18, 50),
            };

            pathLabel = new Label { AutoSize = false, Location = new Point(18, 80), Size = new Size(486, 40), ForeColor = Color.DimGray };

            autoModeCheckBox = new CheckBox { Text = UiText.AutoModeCheckBox, AutoSize = true, Location = new Point(18, 124) };
            var tooltip = new ToolTip { AutoPopDelay = 30000, InitialDelay = 200, ToolTipTitle = UiText.AutoModeTitle };
            tooltip.SetToolTip(autoModeCheckBox, UiText.AutoModeHint);
            autoModeCheckBox.CheckedChanged += (s, e) => OnAutoModeChanged();

            statusLabel = new Label { AutoSize = false, Location = new Point(18, 156), Size = new Size(486, 40), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };

            installButton = new Button { Text = UiText.Install, Location = new Point(18, 206), Size = new Size(150, 36) };
            uninstallButton = new Button { Text = UiText.Uninstall, Location = new Point(178, 206), Size = new Size(130, 36) };
            var browseButton = new Button { Text = UiText.Browse, Location = new Point(334, 206), Size = new Size(170, 36) };

            installButton.Click += (s, e) => RunAndRefresh(SetupAction.Install);
            uninstallButton.Click += (s, e) => RunAndRefresh(SetupAction.Uninstall);
            browseButton.Click += (s, e) => Browse();

            Controls.AddRange(new Control[] { header, description, pathLabel, autoModeCheckBox, statusLabel, installButton, uninstallButton, browseButton });

            win64 = GameLocator.FindWin64Folder();
            RefreshState();
        }

        private void RunAndRefresh(SetupAction action)
        {
            if (win64 == null) return;
            ActionRunner.Run(action, win64, autoModeCheckBox.Checked, this);
            RefreshState();
        }

        // An up-to-date install only needs its tiny config file rewritten, so the checkbox applies instantly.
        private void OnAutoModeChanged()
        {
            if (refreshing || win64 == null) return;
            if (!ModInstaller.IsInstalled(win64) || ModInstaller.IsOutdated(win64)) return;

            if (!ActionRunner.Run(SetupAction.Configure, win64, autoModeCheckBox.Checked, this))
            {
                RefreshState();
                return;
            }

            statusLabel.Text = UiText.ModeSaved(autoModeCheckBox.Checked);
            statusLabel.ForeColor = Color.ForestGreen;
        }

        private void Browse()
        {
            using (var dialog = new FolderBrowserDialog { Description = UiText.BrowseDescription })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                var resolved = GameLocator.ResolveWin64Folder(dialog.SelectedPath);
                if (resolved == null)
                {
                    MessageBox.Show(this, UiText.ExeNotFound, ActionRunner.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                win64 = resolved;
                RefreshState();
            }
        }

        private void RefreshState()
        {
            refreshing = true;
            try
            {
                if (win64 == null)
                {
                    pathLabel.Text = UiText.GameNotFound;
                    statusLabel.Text = UiText.PressBrowse;
                    statusLabel.ForeColor = Color.DarkOrange;
                    installButton.Enabled = false;
                    uninstallButton.Enabled = false;
                    return;
                }

                var installed = ModInstaller.IsInstalled(win64);
                var outdated = ModInstaller.IsOutdated(win64);
                pathLabel.Text = UiText.GameFolder(win64);
                if (installed && !outdated) autoModeCheckBox.Checked = ModInstaller.ReadAutoMode(win64);

                if (outdated)
                {
                    statusLabel.Text = UiText.Outdated;
                    statusLabel.ForeColor = Color.DarkOrange;
                }
                else
                {
                    statusLabel.Text = installed ? UiText.Installed(ModInstaller.CurrentVersion) : UiText.NotInstalled;
                    statusLabel.ForeColor = installed ? Color.ForestGreen : Color.DimGray;
                }

                installButton.Enabled = true;
                installButton.Text = outdated ? UiText.Update : installed ? UiText.Reinstall : UiText.Install;
                uninstallButton.Enabled = installed;
            }
            finally
            {
                refreshing = false;
            }
        }
    }
}
