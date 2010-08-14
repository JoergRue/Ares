/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ares.Data;
using Ares.Playing;
using Ares.Settings;

namespace Ares.Player
{
    public partial class Player : Form, INetworkClient
    {
        private Ares.Ipc.ApplicationInstance m_Instance;

        private Network m_Network;

        public Player()
        {
            String projectName = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1] : String.Empty;
            if (String.IsNullOrEmpty(projectName))
            {
                m_Instance = Ares.Ipc.ApplicationInstance.CreateOrActivate("Ares.Player");
            }
            else
            {
                m_Instance = Ares.Ipc.ApplicationInstance.CreateOrActivate("Ares.Player", projectName);
            }
            if (m_Instance == null)
            {
                throw new Ares.Ipc.ApplicationAlreadyStartedException();
            }
            InitializeComponent();
            m_Instance.SetWindowHandle(Handle);
            m_Instance.ProjectOpenAction = (projectName2, projectPath) => OpenProjectFromRequest(projectName2, projectPath);
            m_PlayingControl = new PlayingControl();
        }

        private void MessageReceived(Message m)
        {
            if (m.Type >= MessageType.Warning)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(() => ShowMessagesForm()));
                }
                else
                {
                    ShowMessagesForm();
                }
            }
        }

        private bool listen = true;

        private void ShowMessagesForm()
        {
            if (!listen)
                return;
            if (m_MessagesForm == null)
            {
                m_MessagesForm = new MessagesForm();
                m_MessagesForm.Location = new Point(Location.X + Size.Width, Location.Y);
                m_MessagesForm.FormClosed += new FormClosedEventHandler(m_MessagesForm_FormClosed);
                m_MessagesForm.Show(this);
            }
        }

        void m_MessagesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            listen = false;
            messagesButton.Checked = false;
            m_MessagesForm = null;
            listen = true;
        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (m_Project == null)
                return;
            fileSystemWatcher1.EnableRaisingEvents = false;
            String path = m_Project.FileName;
            if (e.FullPath == path)
            {
                if (MessageBox.Show(this, StringResources.ReloadProject, StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    PlayingModule.ProjectPlayer.StopAll();
                    OpenProject(path);
                }
            }
            fileSystemWatcher1.EnableRaisingEvents = true;
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private void ShowSettingsDialog()
        {
            PlayingModule.ProjectPlayer.StopAll();
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            Ares.Settings.SettingsDialog dialog = new Ares.Settings.SettingsDialog(Ares.Settings.Settings.Instance, m_BasicSettings);
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_PlayingControl.UpdateDirectories();
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void OpenProject()
        {
            PlayingModule.ProjectPlayer.StopAll();
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return;
            OpenProject(openFileDialog1.FileName);
        }

        private void OpenProjectFromRequest(String projectName, String projectPath)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => OpenProjectFromRequest(projectName, projectPath)));
            }
            else
            {
                if (MessageBox.Show(this, String.Format(StringResources.OpenProjectQuestion, projectName), StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    OpenProject(projectPath);
                }
            }
        }

        private void OpenProject(String filePath)
        {
            if (m_Project != null)
            {
                Ares.Data.DataModule.ProjectManager.UnloadProject(m_Project);
                m_Project = null;
                m_Instance.SetLoadedProject("-");
            }
            try
            {
                m_Project = Ares.Data.DataModule.ProjectManager.LoadProject(filePath);
                Ares.Settings.Settings.Instance.RecentFiles.AddFile(new RecentFiles.ProjectEntry(m_Project.FileName, m_Project.Title));
            }
            catch (Exception e)
            {
                MessageBox.Show(this, String.Format(StringResources.LoadError, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_Project = null;
            }
            projectNameLabel.Text = m_Project != null ? m_Project.Title : StringResources.NoOpenedProject;
            this.Text = String.Format(StringResources.AresPlayer, projectNameLabel.Text);
            PlayingModule.ProjectPlayer.SetProject(m_Project);
            DoModelChecks();
            fileSystemWatcher1.Path = m_Project != null ? System.IO.Path.GetDirectoryName(m_Project.FileName) : String.Empty;
            m_Instance.SetLoadedProject(filePath);
        }

        private void DoModelChecks()
        {
            Ares.ModelInfo.ModelChecks.Instance.Project = m_Project;
            Ares.ModelInfo.ModelChecks.Instance.CheckAll();
            foreach (Ares.ModelInfo.ModelError error in Ares.ModelInfo.ModelChecks.Instance.GetAllErrors())
            {
                Messages.AddMessage(error.Severity == ModelInfo.ModelError.ErrorSeverity.Error ? MessageType.Error : MessageType.Warning,
                    error.Message);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            if (msg.Msg == WM_KEYDOWN)
            {
                m_PlayingControl.KeyReceived(keyData);
            }
            return true;
        }

        private int lastMusicElementId = -1;

        private void UpdateStatus()
        {
            PlayingControl control = m_PlayingControl;
            IMode mode = control.CurrentMode;
            modeLabel.Text = mode != null ? mode.Title : String.Empty;
            StringBuilder modeElementsText = new StringBuilder();
            foreach (IModeElement modeElement in control.CurrentModeElements)
            {
                if (modeElementsText.Length > 0)
                    modeElementsText.Append(", ");
                modeElementsText.Append(modeElement.Title);
            }
            elementsLabel.Text = modeElementsText.ToString();
            StringBuilder soundElementsText = new StringBuilder();
            foreach (int element in control.CurrentSoundElements)
            {
                if (soundElementsText.Length > 0)
                    soundElementsText.Append(", ");
                soundElementsText.Append(Ares.Data.DataModule.ElementRepository.GetElement(element).Title);
            }
            soundsLabel.Text = soundElementsText.ToString();
            int musicElementId = control.CurrentMusicElement;
            if (musicElementId != lastMusicElementId)
            {
                musicLabel.Text = MusicInfo.GetInfo(musicElementId);
                lastMusicElementId = musicElementId;
            }
            bool settingsChanged = false;
            if (overallVolumeBar.Value != control.GlobalVolume)
            {
                overallVolumeBar.Value = control.GlobalVolume;
                m_Network.InformClientOfVolume(VolumeTarget.Both, control.GlobalVolume);
                Settings.Settings.Instance.GlobalVolume = control.GlobalVolume;
                settingsChanged = true;
            }
            if (musicVolumeBar.Value != control.MusicVolume)
            {
                musicVolumeBar.Value = control.MusicVolume;
                m_Network.InformClientOfVolume(VolumeTarget.Music, control.MusicVolume);
                Settings.Settings.Instance.MusicVolume = control.MusicVolume;
                settingsChanged = true;
            }
            if (soundVolumeBar.Value != control.SoundVolume)
            {
                soundVolumeBar.Value = control.SoundVolume;
                m_Network.InformClientOfVolume(VolumeTarget.Sounds, control.SoundVolume);
                Settings.Settings.Instance.SoundVolume = control.SoundVolume;
                settingsChanged = true;
            }
            if (settingsChanged)
            {
                Settings.Settings.Instance.Commit();
            }
        }

        private BasicSettings m_BasicSettings;
        private IProject m_Project;
        private PlayingControl m_PlayingControl;
        private bool commitVolumes = true;

        private void overallVolumeBar_Scroll(object sender, EventArgs e)
        {
            m_PlayingControl.GlobalVolume = overallVolumeBar.Value;
            m_Network.InformClientOfVolume(VolumeTarget.Both, overallVolumeBar.Value);
            if (!commitVolumes) return;
            Settings.Settings.Instance.GlobalVolume = overallVolumeBar.Value;
            Settings.Settings.Instance.Commit();
        }

        private void musicVolumeBar_Scroll(object sender, EventArgs e)
        {
            m_PlayingControl.MusicVolume = musicVolumeBar.Value;
            m_Network.InformClientOfVolume(VolumeTarget.Music, musicVolumeBar.Value);
            if (!commitVolumes) return;
            Settings.Settings.Instance.MusicVolume = musicVolumeBar.Value;
            Settings.Settings.Instance.Commit();
        }

        private void soundVolumeBar_Scroll(object sender, EventArgs e)
        {
            m_PlayingControl.SoundVolume = soundVolumeBar.Value;
            m_Network.InformClientOfVolume(VolumeTarget.Sounds, soundVolumeBar.Value);
            if (!commitVolumes) return;
            Settings.Settings.Instance.SoundVolume = soundVolumeBar.Value;
            Settings.Settings.Instance.Commit();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            Settings.AboutDialog dialog = new AboutDialog();
            dialog.ShowDialog(this);
        }

        private void Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            Shutdown();
        }

        private void Shutdown()
        {
            if (m_Network.ClientConnected)
            {
                m_Network.DisconnectClient(false);
            }
            else
            {
                m_Network.StopListenForClient();
                System.Threading.Thread.Sleep(400);
                if (m_Network.ClientConnected)
                {
                    m_Network.DisconnectClient(false);
                }
            }
            broadCastTimer.Enabled = false;
            m_Network.StopUdpBroadcast();
            m_PlayingControl.Dispose();
            WriteSettings();
            Settings.Settings.Instance.Shutdown();
        }

        private bool warnOnNetworkFail = true;
        
        void broadCastTimer_Tick(object sender, EventArgs e)
        {
            if (!m_Network.SendUdpPacket())
            {
                if (warnOnNetworkFail)
                {
                    warnOnNetworkFail = false;
                    MessageBox.Show(this, StringResources.NoStatusInfoError, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void WriteSettings()
        {
            try
            {
                Ares.Settings.Settings.Instance.WriteToFile(m_BasicSettings.GetSettingsDir());
                m_BasicSettings.WriteToFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.WriteSettingsError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ReadSettings()
        {
            bool foundSettings = m_BasicSettings.ReadFromFile();
            bool hasSettings = Ares.Settings.Settings.Instance.Initialize(Ares.Settings.Settings.PlayerID, foundSettings ? m_BasicSettings.GetSettingsDir() : null);
            if (!hasSettings)
            {
                MessageBox.Show(this, StringResources.NoSettings, StringResources.Ares);
                ShowSettingsDialog();
                SettingsChanged(true);
            }
            else
            {
                SettingsChanged(true);
            }
            Ares.Settings.Settings.Instance.SettingsChanged += new EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
        }

        void SettingsChanged(object sender, Settings.Settings.SettingsEventArgs e)
        {
            SettingsChanged(e.FundamentalChange);
        }

        private void SettingsChanged(bool fundamentalChange)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => SettingsChanged(fundamentalChange)));
                return;
            }
            if (fundamentalChange)
            {
                m_PlayingControl.KeyReceived(Keys.Escape);
                m_PlayingControl.UpdateDirectories();
            }
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            listenForPorts = false;
            udpPortUpDown.Value = settings.UdpPort;
            tcpPortUpDown.Value = settings.TcpPort;
            listenForPorts = true;
            commitVolumes = false;
            overallVolumeBar.Value = settings.GlobalVolume;
            musicVolumeBar.Value = settings.MusicVolume;
            soundVolumeBar.Value = settings.SoundVolume;
            m_PlayingControl.GlobalVolume = settings.GlobalVolume;
            m_PlayingControl.MusicVolume = settings.MusicVolume;
            m_PlayingControl.SoundVolume = settings.SoundVolume;
            commitVolumes = true;
            if (fundamentalChange)
            {
                if (m_Network != null && m_Network.ClientConnected)
                {
                    m_Network.DisconnectClient(true);
                }
            }
        }

        private MessagesForm m_MessagesForm;

        private void messagesButton_CheckedChanged(object sender, EventArgs e)
        {
            if (m_MessagesForm != null)
            {
                m_MessagesForm.Close();
                m_MessagesForm = null;
            }
            else
            {
                ShowMessagesForm();
            }
        }

        private void editorButton_Click(object sender, EventArgs e)
        {
            StartEditor();
        }

        private void StartEditor()
        {
            String appDir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            String commandLine = System.IO.Path.Combine(appDir, "Ares.Editor.exe");
            try
            {
                Ares.Ipc.ApplicationInstance.CreateOrActivate("Ares.Editor", commandLine,
                    m_Project != null ? m_Project.Title : "", m_Project != null ? m_Project.FileName : "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.EditorStartError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            m_PlayingControl.KeyReceived(Keys.Escape);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            m_PlayingControl.KeyReceived(Keys.Left);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            m_PlayingControl.KeyReceived(Keys.Right);
        }

        public void KeyReceived(Keys key)
        {
            m_PlayingControl.KeyReceived(key);
        }

        public void VolumeReceived(Ares.Playing.VolumeTarget target, int value)
        {
            switch (target)
            {
                case VolumeTarget.Both:
                    m_PlayingControl.GlobalVolume = value;
                    break;
                case VolumeTarget.Music:
                    m_PlayingControl.MusicVolume = value;
                    break;
                case VolumeTarget.Sounds:
                    m_PlayingControl.SoundVolume = value;
                    break;
                default:
                    break;
            }
        }

        public void ClientDataChanged()
        {
            this.Invoke(new MethodInvoker(UpdateClientData));
        }

        private void UpdateClientData()
        {
            if (m_Network.ClientConnected)
            {
                clientStateLabel.Text = StringResources.ConnectedWith + m_Network.ClientName;
                clientStateLabel.ForeColor = System.Drawing.Color.DarkGreen;
                m_Network.InformClientOfVolume(VolumeTarget.Both, m_PlayingControl.GlobalVolume);
                m_Network.InformClientOfVolume(VolumeTarget.Music, m_PlayingControl.MusicVolume);
                m_Network.InformClientOfVolume(VolumeTarget.Sounds, m_PlayingControl.SoundVolume);
                disconnectButton.Enabled = true;
            }
            else
            {
                clientStateLabel.Text = StringResources.NotConnected;
                clientStateLabel.ForeColor = System.Drawing.Color.Red;
                disconnectButton.Enabled = false;
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            m_Network.DisconnectClient(true);
        }

        private bool listenForPorts = false;

        private void udpPortUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listenForPorts) return;
            Settings.Settings.Instance.UdpPort = (int)udpPortUpDown.Value;
            Settings.Settings.Instance.Commit();
            if (m_Network.ClientConnected)
            {
                m_Network.DisconnectClient(true);
            }
        }

        private void tcpPortUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listenForPorts) return;
            Settings.Settings.Instance.TcpPort = (int)tcpPortUpDown.Value;
            Settings.Settings.Instance.Commit();
            if (m_Network.ClientConnected)
            {
                m_Network.DisconnectClient(true);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void extrasToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            messagesToolStripMenuItem.Checked = messagesButton.Checked;
        }

        private void messagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            messagesButton.PerformClick();
        }

        private void recentToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            foreach (RecentFiles.ProjectEntry projectEntry in Ares.Settings.Settings.Instance.RecentFiles.GetFiles())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(projectEntry.ProjectName);
                item.ToolTipText = projectEntry.FilePath;
                item.Tag = projectEntry;
                item.Click += new EventHandler(recentItem_Click);

                recentToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        void recentItem_Click(object sender, EventArgs e)
        {
            OpenProject(((sender as ToolStripMenuItem).Tag as RecentFiles.ProjectEntry).FilePath);
        }

        private void Player_Load(object sender, EventArgs e)
        {
            m_BasicSettings = new BasicSettings();
            ReadSettings();
            Messages.Instance.MessageReceived += new MessageReceivedHandler(MessageReceived);
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                OpenProject(Environment.GetCommandLineArgs()[1]);
            }
            else if (Ares.Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
            {
                OpenProject(Ares.Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath);
            }
            else
            {
                projectNameLabel.Text = m_Project != null ? m_Project.Title : StringResources.NoOpenedProject;
                this.Text = String.Format(StringResources.AresPlayer, projectNameLabel.Text);
            }
            listenForPorts = false;
            udpPortUpDown.Value = Settings.Settings.Instance.UdpPort;
            tcpPortUpDown.Value = Settings.Settings.Instance.TcpPort;
            bool foundAddress = false;
            foreach (System.Net.IPAddress address in System.Net.Dns.GetHostAddresses(String.Empty))
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    continue;
                String s = address.ToString();
                ipAddressBox.Items.Add(s);
                if (s == Settings.Settings.Instance.IPAddress)
                    foundAddress = true;
            }
            if (foundAddress)
            {
                ipAddressBox.SelectedItem = Settings.Settings.Instance.IPAddress;
            }
            else if (ipAddressBox.Items.Count > 0)
            {
                ipAddressBox.SelectedIndex = 0;
                Settings.Settings.Instance.IPAddress = ipAddressBox.SelectedItem.ToString();
            }
            else
            {
                ipAddressBox.Enabled = false;
            }
            listenForPorts = true;
            m_Network = new Network(this);
            m_Network.InitConnectionData();
            m_Network.StartUdpBroadcast();
            broadCastTimer.Tick += new EventHandler(broadCastTimer_Tick);
            broadCastTimer.Enabled = true;
            m_Network.ListenForClient();
            Timer updateTimer = new Timer();
            updateTimer.Interval = 50;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Start();
            fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(fileSystemWatcher1_Changed);
        }

        private void ipAddressBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listenForPorts) return;
            Settings.Settings.Instance.IPAddress = ipAddressBox.SelectedItem.ToString();
            if (m_Network.ClientConnected)
            {
                m_Network.DisconnectClient(true);
            }
            m_Network.StopUdpBroadcast();
            m_Network.InitConnectionData();
            m_Network.StartUdpBroadcast();
        }
    }
}
