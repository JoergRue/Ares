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
    partial class SettingsDialog : Form
    {
        public SettingsDialog(MusicSettings musicSettings)
        {
            InitializeComponent();
            LoadSettings(musicSettings);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings(MusicSettings musicSettings)
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
            allChannelsCheckBox.Checked = musicSettings.MusicOnAllSpeakers;
            noFadeButton.Checked = musicSettings.MusicFadeOption == 0;
            fadeButton.Checked = musicSettings.MusicFadeOption == 1;
            crossFadeButton.Checked = musicSettings.MusicFadeOption == 2;
            crossFadingUpDown.Value = musicSettings.MusicFadeTime;
            crossFadingUpDown.Enabled = musicSettings.MusicFadeOption != 0;
        }

        public MusicSettings MusicSettings { get; private set; }

        private void SaveSettings()
        {
            Settings.Default.ServerSearchPort = (int)udpPortUpDown.Value;
            Settings.Default.StartLocalPlayer = startLocalPlayerBox.Checked;
            Settings.Default.AskBeforePlayerStart = askBeforePlayerStartBox.Checked;
            Settings.Default.HasCheckedSettings = true;
            Settings.Default.LocalPlayerPath = playerPathBox.Text;
            Settings.Default.Save();
            MusicSettings musicSettings = new MusicSettings();
            musicSettings.MusicFadeTime = (int)crossFadingUpDown.Value;
            musicSettings.MusicFadeOption = (noFadeButton.Checked ? 0 : (fadeButton.Checked ? 1 : 2));
            musicSettings.MusicOnAllSpeakers = allChannelsCheckBox.Checked;
            this.MusicSettings = musicSettings;
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

        private void noFadeButton_CheckedChanged(object sender, EventArgs e)
        {
            crossFadingUpDown.Enabled = !noFadeButton.Checked;
        }

        private void fadeButton_CheckedChanged(object sender, EventArgs e)
        {
            crossFadingUpDown.Enabled = !noFadeButton.Checked;
        }

        private void crossFadeButton_CheckedChanged(object sender, EventArgs e)
        {
            crossFadingUpDown.Enabled = !noFadeButton.Checked;
        }

    }

    class MusicSettings
    {
        public bool MusicOnAllSpeakers { get; set; }
        public int MusicFadeOption { get; set; }
        public int MusicFadeTime { get; set; }
    }
}
