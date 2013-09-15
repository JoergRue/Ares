using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.MediaPortalPlugin
{
    public partial class SettingsDialog : Form
    {
        private String FindLocalPlayer()
        {
            String path = Settings.Settings.Instance.LocalPlayerPath;
            if (String.IsNullOrEmpty(path))
            {
                // try to find via registry
                try
                {
                    path = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Ares", "PlayerPath", null);
                }
                catch (System.Security.SecurityException)
                {
                    path = null;
                }
                catch (System.IO.IOException)
                {
                    path = null;
                }
                if (String.IsNullOrEmpty(path))
                {
                    // try default location
                    path = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Ares\Player_Editor\Ares.Player.exe");
                    if (System.IO.File.Exists(path))
                    {
                        Settings.Settings.Instance.LocalPlayerPath = path;
                        return path;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Settings.Settings.Instance.LocalPlayerPath = path;
                }
            }
            return System.IO.File.Exists(path) ? path : null;
        }


        public SettingsDialog()
        {
            InitializeComponent();
            AresSettings.ReadFromConfigFile();
            Settings.Settings settings = Settings.Settings.Instance;
            projectFileBox.Text = settings.RecentFiles.GetFiles().Count > 0 ? settings.RecentFiles.GetFiles()[0].FilePath : "";
            playerPathBox.Text = FindLocalPlayer();
            musicFilesBox.Text = settings.MusicDirectory;
            soundFilesBox.Text = settings.SoundDirectory;
            tcpPortUpDown.Value = settings.TcpPort;
            udpPortUpDown.Value = settings.UdpPort;
            bool foundAddress = false;
            foreach (System.Net.IPAddress address in System.Net.Dns.GetHostAddresses(String.Empty))
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    continue;
                String s = address.ToString();
                ipAddressBox.Items.Add(s);
                if (s.Equals(settings.IPAddress))
                {
                    foundAddress = true;
                }
            }
            if (foundAddress)
            {
                ipAddressBox.SelectedItem = settings.IPAddress;
            }
            else if (ipAddressBox.Items.Count > 0)
            {
                ipAddressBox.SelectedIndex = 0;
                settings.IPAddress = ipAddressBox.SelectedItem.ToString();
            }
            else
            {
                ipAddressBox.Enabled = false;
            }
        }

        private void projectFileButton_Click(object sender, EventArgs e)
        {
            if (projectFileBox.Text.Length > 0)
            {
                projectFileDialog.FileName = projectFileBox.Text;
            }
            if (projectFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                projectFileBox.Text = projectFileDialog.FileName;
            }
        }

        private void soundFilesButton_Click(object sender, EventArgs e)
        {
            if (soundFilesBox.Text.Length > 0)
            {
                folderBrowserDialog.SelectedPath = soundFilesBox.Text;
            }
            if (folderBrowserDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                soundFilesBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void musicFilesButton_Click(object sender, EventArgs e)
        {
            if (musicFilesBox.Text.Length > 0)
            {
                folderBrowserDialog.SelectedPath = musicFilesBox.Text;
            }
            if (folderBrowserDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                musicFilesBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Settings.Settings settings = Settings.Settings.Instance;
            settings.SoundDirectory = soundFilesBox.Text;
            settings.MusicDirectory = musicFilesBox.Text;
            if (projectFileBox.Text.Length > 0)
            {
                settings.RecentFiles.AddFile(new Settings.RecentFiles.ProjectEntry(projectFileBox.Text, ""));
            }
            settings.IPAddress = ipAddressBox.SelectedItem.ToString();
            settings.UdpPort = (int)udpPortUpDown.Value;
            settings.TcpPort = (int)tcpPortUpDown.Value;
            settings.LocalPlayerPath = playerPathBox.Text;
            try
            {
                AresSettings.SaveToConfigFile();
                Close();
            }
            catch (Exception e2)
            {
                MessageBox.Show(this, String.Format(StringResources.SettingsStoreError, e2.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playerPathButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(playerPathBox.Text))
            {
                playerPathDialog.FileName = playerPathBox.Text;
            }
            if (playerPathDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                playerPathBox.Text = playerPathDialog.FileName;
            }
        }


    }
}
