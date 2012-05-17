using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.MGPlugin
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            udpPortUpDown.Value = Settings.Default.ServerSearchPort;
            startLocalPlayerBox.Checked = Settings.Default.StartLocalPlayer;
            askBeforePlayerStartBox.Checked = Settings.Default.AskBeforePlayerStart;
            playerPathBox.Text = Settings.Default.LocalPlayerPath;
            if (String.IsNullOrEmpty(Settings.Default.LocalPlayerPath) || !System.IO.File.Exists(Settings.Default.LocalPlayerPath))
            {
                startLocalPlayerBox.Enabled = false;
                askBeforePlayerStartBox.Enabled = false;
            }
        }

        private void SaveSettings()
        {
            Settings.Default.ServerSearchPort = (int)udpPortUpDown.Value;
            Settings.Default.StartLocalPlayer = startLocalPlayerBox.Checked;
            Settings.Default.AskBeforePlayerStart = askBeforePlayerStartBox.Checked;
            Settings.Default.HasCheckedSettings = true;
            Settings.Default.LocalPlayerPath = playerPathBox.Text;
            Settings.Default.Save();
        }

        private void selectPlayerButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                playerPathBox.Text = openFileDialog1.FileName;
                if (!String.IsNullOrEmpty(playerPathBox.Text) && System.IO.File.Exists(playerPathBox.Text))
                {
                    startLocalPlayerBox.Enabled = true;
                    askBeforePlayerStartBox.Enabled = true;
                }
                else
                {
                    startLocalPlayerBox.Enabled = false;
                    askBeforePlayerStartBox.Enabled = false;
                }
            }
        }
    }
}
