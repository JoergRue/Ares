/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ares.MGPlugin
{
    public partial class Controller : UserControl, Controllers.INetworkClient, Controllers.IUIThreadDispatcher
    {

        private bool m_ConnectWithFirstServer = true;
        private Ares.Controllers.ServerSearch m_ServerSearch;
        private Dictionary<String, Controllers.ServerInfo> m_Servers = new Dictionary<string, Controllers.ServerInfo>();

        private bool m_HasLocalPlayer;
        private bool m_IsLocalPlayer;
        private String m_PlayerProject = String.Empty;


        private System.Windows.Forms.Timer m_FirstTimer;

        private String FindLocalPlayer()
        {
            String path = Settings.Default.LocalPlayerPath;
            if (String.IsNullOrEmpty(path))
            {
                // try to find via registry
                try
                {
                    if (System.Environment.Is64BitProcess)
                    {
                        path = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Ares", "PlayerPath", null);
                    }
                    else
                    {
                        path = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Ares", "PlayerPath", null);
                    }
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
                    path = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Ares\Player_Editor\Ares.Player.exe");
                    if (System.IO.File.Exists(path))
                    {
                        Settings.Default.LocalPlayerPath = path;
                        Settings.Default.Save();
                        return path;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Settings.Default.LocalPlayerPath = path;
                    Settings.Default.Save();
                }
            }
            return System.IO.File.Exists(path) ? path : null;
        }

        public Controller()
        {
            InitializeComponent();
            Ares.Controllers.UIThreadDispatcher.Dispatcher = this;
            Ares.Controllers.Messages.Instance.MessageAdded += new EventHandler<Controllers.MessageEventArgs>(MessageAdded);
            EnableProjectSpecificControls(false);
        }

        private void DoDispose()
        {
            Controllers.Control.Instance.Disconnect(true);
            Controllers.Messages.Instance.MessageAdded -= new EventHandler<Controllers.MessageEventArgs>(MessageAdded);
            m_ServerSearch.Dispose();
        }

        private bool m_Listen = true;

        private void EnableProjectSpecificControls(bool enabled)
        {
            m_Listen = false;
            stopButton.Enabled = enabled;
            nextMusicButton.Enabled = enabled;
            prevMusicButton.Enabled = enabled;
            modesList.Items.Clear();
            elementsPanel.Controls.Clear();
            m_CurrentMode = String.Empty;
            m_CurrentMusicList.Clear();
            m_ModeElements.Clear();
            foreach (int id in m_CurrentButtons)
            {
                CommandButtonMapping.Instance.UnregisterButton(id);
            }
            m_CurrentButtons.Clear();
            if (enabled)
            {
                Ares.Data.IProject project = Controllers.Control.Instance.Project;
                if (project == null)
                    return;
                IList<Ares.Data.IMode> modes = project.GetModes();
                if (modes.Count == 0)
                    return;
                modesList.Items.Add("Musikliste");
                int i = 1;
                int currentMode = 0;
                foreach (Ares.Data.IMode mode in modes)
                {
                    modesList.Items.Add(mode.Title);
                    if (mode.Title == m_CurrentMode)
                    {
                        currentMode = i;
                    }
                    ++i;
                }
                modesList.SelectedIndex = currentMode;
            }
            m_Listen = true;
            UpdateElementsPanel();
            UpdateModeElements();
        }

        void m_FirstTimer_Tick(object sender, EventArgs e)
        {
            m_FirstTimer.Stop();
            m_FirstTimer.Dispose();
            m_FirstTimer = null;
            if (Settings.Default.StartLocalPlayer && m_HasLocalPlayer)
            {
                if (Settings.Default.ShowOverlayPanel)
                {
                    Settings.Default.ShowOverlayPanel = false;
                    Settings.Default.Save();
                }
                MaybeStartPlayer();
            }
            else if (!m_HasLocalPlayer && !Controllers.Control.Instance.IsConnected && Settings.Default.ShowOverlayPanel)
            {
                overLayPanel.Visible = true;
            }
            else if (Settings.Default.ShowOverlayPanel)
            {
                Settings.Default.ShowOverlayPanel = false;
                Settings.Default.Save();
            }
            if (Settings.Default.CheckForNewVersion)
            {
                Online.OnlineOperations.CheckForUpdate(System.Windows.Forms.Form.ActiveForm, false);
            }
        }

        void m_FirstTimer_Tick2(object sender, EventArgs e)
        {
            m_FirstTimer.Stop();
            m_FirstTimer.Dispose();
            m_FirstTimer = null;
            if (Settings.Default.StartLocalPlayer)
            {
                MaybeStartPlayer();
            }
        }

        private static void UpdateLastProjects(String path, String name)
        {
            int nrOfLastProjects = Settings.Default.LastUsedProjectsCount;
            if (nrOfLastProjects < 4) ++nrOfLastProjects;
            int previousPos = nrOfLastProjects - 1;
            for (int i = nrOfLastProjects - 2; i >= 0; --i)
            {
                String oldPath = (String)Settings.Default["LastUsedProjectFile" + i];
                if (oldPath != null && oldPath == path)
                {
                    previousPos = i;
                    --nrOfLastProjects;
                    break;
                }
            }
            for (int i = previousPos; i > 0; --i)
            {
                String oldPath = (String)Settings.Default["LastUsedProjectFile" + (i - 1)];
                String oldName = (String)Settings.Default["LastUsedProjectName" + (i - 1)];
                Settings.Default["LastUsedProjectFile" + i] = oldPath;
                Settings.Default["LastUsedProjectName" + i] = oldName;
            }
            Settings.Default["LastUsedProjectFile" + 0] = path;
            Settings.Default["LastUsedProjectName" + 0] = name;
            Settings.Default.LastUsedProjectsCount = nrOfLastProjects;
            Settings.Default.Save();
        }

        public void OpenFile(String fileName)
        {
            if (!System.IO.File.Exists(fileName))
                return;

            try
            {
                Controllers.Control.Instance.OpenFile(fileName);
            }
            catch (Exception e)
            {
                Controllers.Messages.AddMessage(Controllers.MessageType.Error, e.Message);
            }
            if (Controllers.Control.Instance.Project != null)
            {
                EnableProjectSpecificControls(true);
                Settings.Default.LastProjectFile = fileName;
                Settings.Default.Save();
                UpdateLastProjects(fileName, Controllers.Control.Instance.Project.Title);
            }
            else
            {
                EnableProjectSpecificControls(false);
            }
            UpdateProjectTitle();
        }

        private void UpdateProjectTitle()
        {
            if (Controllers.Control.Instance.Project != null)
            {
                String text = Controllers.Control.Instance.Project.Title;
                if (text != m_PlayerProject && Controllers.Control.Instance.IsConnected)
                {
                    text += StringResources.ProjectsDifferent;
                }
                projectLabel.Text = text;
            }
            else
            {
                projectLabel.Text = "- (bitte zuerst ein Projekt öffnen)";
            }
        }

        void m_ServerSearch_ServerFound(object sender, Controllers.ServerEventArgs e)
        {
            if (m_Servers.ContainsKey(e.Server.Name))
                return;
            Controllers.Messages.AddMessage(Controllers.MessageType.Info, String.Format(StringResources.ServerFound, e.Server.Name));
            serverBox.Items.Add(e.Server.Name);
            if (serverBox.Items.Count == 1)
                serverBox.SelectedIndex = 0;
            connectButton.Enabled = true;
            m_Servers[e.Server.Name] = e.Server;
            if (m_ConnectWithFirstServer)
            {
                ConnectOrDisconnect();
                m_ConnectWithFirstServer = false;
            }
        }

        void MessageAdded(object sender, Controllers.MessageEventArgs e)
        {
            if ((int)e.Message.Type <= (int)Controllers.MessageType.Warning)
            {
                if ((int)e.Message.Type <= Settings.Default.MessageLevel)
                {
                    if (!messagesButton.Checked)
                        messagesButton.PerformClick();
                }
            }
        }

        private void overallVolumeBar_ValueChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            Controllers.Control.Instance.SetVolume(2, overallVolumeBar.Value);
        }

        private void musicBar_ValueChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            Controllers.Control.Instance.SetVolume(1, musicBar.Value);
        }

        private void soundsBar_ValueChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            Controllers.Control.Instance.SetVolume(0, soundsBar.Value);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            CloseOverlay();
            SelectFile();
        }

        private void SelectFile()
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                OpenFile(openFileDialog1.FileName);
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            ConnectOrDisconnect();
        }

        private void StartLocalPlayer()
        {
            String playerFile = FindLocalPlayer();
            if (playerFile == null)
                return;
            String arguments = "--minimized";
            try
            {
                m_ConnectWithFirstServer = true;
                m_IsLocalPlayer = true;
                System.Diagnostics.Process.Start(playerFile, arguments);
            }
            catch (Exception e)
            {
                MessageBox.Show(this, String.Format(StringResources.CouldNotStartPlayer, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MaybeStartPlayer()
        {
            if (Controllers.Control.Instance.IsConnected)
                return;
            String playerFile = FindLocalPlayer();
            if (playerFile == null)
                return;
            if (!Settings.Default.AskBeforePlayerStart || MessageBox.Show(this, StringResources.ShallStartPlayer, StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StartLocalPlayer();
                if (Settings.Default.AskBeforePlayerStart)
                {
                    Settings.Default.StartLocalPlayer = true;
                    if (!Settings.Default.HasCheckedSettings)
                    {
                        Settings.Default.AskBeforePlayerStart = false;
                        Settings.Default.HasCheckedSettings = true;
                    }
                    Settings.Default.Save();
                }
            }
            else if (Settings.Default.AskBeforePlayerStart)
            {
                Settings.Default.StartLocalPlayer = false;
                if (!Settings.Default.HasCheckedSettings)
                {
                    Settings.Default.AskBeforePlayerStart = false;
                    Settings.Default.HasCheckedSettings = true;
                }
                Settings.Default.Save();
            }
        }

        private void ConnectOrDisconnect()
        {
            if (Controllers.Control.Instance.IsConnected)
            {
                Controllers.Control.Instance.Disconnect(true);
            }
            else if (serverBox.Items.Count == 0 && m_HasLocalPlayer)
            {
                if (MessageBox.Show(this, StringResources.ShallStartPlayer2, StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    StartLocalPlayer();
            }
            else if (serverBox.Items.Count > 0)
            {
                String server = serverBox.SelectedItem.ToString();
                Controllers.ServerInfo serverInfo = m_Servers.ContainsKey(server) ? m_Servers[server] : null;
                if (serverInfo != null)
                    Controllers.Control.Instance.Connect(serverInfo, this, m_IsLocalPlayer);
            }
            UpdateNetworkState();
        }

        private void UpdateNetworkState()
        {
            if (Controllers.Control.Instance.IsConnected)
            {
                m_ServerSearch.StopSearch();
                connectButton.Text = StringResources.Disconnect;
            }
            else
            {
                m_Servers.Clear();
                serverBox.Items.Clear();
                connectButton.Enabled = m_HasLocalPlayer;
                connectButton.Text = StringResources.Connect;
                m_ServerSearch.StartSearch();
            }
        }

        private MessagesForm m_MessagesForm = null;

        private void messagesButton_Click(object sender, EventArgs e)
        {
            CloseOverlay();
            if (m_MessagesForm != null)
            {
                m_MessagesForm.FormClosed -= new FormClosedEventHandler(m_MessagesForm_FormClosed);
                m_MessagesForm.Close();
                m_MessagesForm = null;
                messagesButton.Checked = false;   
            }
            else
            {
                m_MessagesForm = new MessagesForm();
                m_MessagesForm.FormClosed += new FormClosedEventHandler(m_MessagesForm_FormClosed);
                m_MessagesForm.Show(this);
                messagesButton.Checked = true;
            }
        }

        void m_MessagesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_MessagesForm.FormClosed -= new FormClosedEventHandler(m_MessagesForm_FormClosed);
            messagesButton.Checked = false;
            m_MessagesForm = null;
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            CloseOverlay();
            ShowSettings();
        }

        private void ShowSettings()
        {
            SettingsDialog dialog = new SettingsDialog();
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (m_ServerSearch.IsSearching)
                {
                    m_ServerSearch.StopSearch();
                    m_ServerSearch.Dispose();
                    m_ServerSearch = new Controllers.ServerSearch(Settings.Default.ServerSearchPort);
                    m_ServerSearch.ServerFound += new EventHandler<Controllers.ServerEventArgs>(m_ServerSearch_ServerFound);
                    m_ServerSearch.StartSearch();
                }
                else
                {
                    m_ServerSearch.Dispose();
                    m_ServerSearch = new Controllers.ServerSearch(Settings.Default.ServerSearchPort);
                    m_ServerSearch.ServerFound += new EventHandler<Controllers.ServerEventArgs>(m_ServerSearch_ServerFound);
                }
                m_HasLocalPlayer = FindLocalPlayer() != null;
                connectButton.Enabled = m_HasLocalPlayer || Controllers.Control.Instance.IsConnected;
                if (!Controllers.Control.Instance.IsConnected && m_HasLocalPlayer && Settings.Default.StartLocalPlayer)
                {
                    m_FirstTimer = new Timer();
                    m_FirstTimer.Interval = 2000;
                    m_FirstTimer.Tick += new EventHandler(m_FirstTimer_Tick2);
                    m_FirstTimer.Start();
                }
            }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            CloseOverlay();
            AboutDialog dialog = new AboutDialog();
            dialog.ShowDialog(this);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Controllers.Control.Instance.SendKey(Keys.Escape);
        }

        private void prevMusicButton_Click(object sender, EventArgs e)
        {
            Controllers.Control.Instance.SendKey(Keys.Left);
        }

        private void nextMusicButton_Click(object sender, EventArgs e)
        {
            Controllers.Control.Instance.SendKey(Keys.Right);
        }

        private List<String> m_ModeElements = new List<string>();

        private void UpdateModeElements()
        {
            String text = String.Empty;
            for (int i = 0; i < m_ModeElements.Count; ++i)
            {
                text += m_ModeElements[i];
                if (i != m_ModeElements.Count - 1)
                {
                    text += ", ";
                }
            }
            String fit = Compact(text, elementsLabel);
            elementsLabel.Text = fit;
        }

        public void DispatchToUIThread(Action action)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(action));
            }
            else
            {
                action();
            }
        }

        private String m_CurrentMode = String.Empty;

        public void ModeChanged(string newMode)
        {
            DispatchToUIThread(() =>
                {
                    modeLabel.Text = newMode;
                    m_CurrentMode = newMode;
                    for (int i = 0; i < modesList.Items.Count; ++i)
                    {
                        if (newMode == (string)modesList.Items[i])
                        {
                            modesList.SelectedIndex = i;
                            break;
                        }
                    }
                }
            );
        }

        public void ModeElementStarted(int element)
        {
            DispatchToUIThread(() =>
                {
                    CommandButtonMapping.Instance.CommandStateChanged(element, true);
                    Ares.Data.IElement dataElement = Ares.Data.DataModule.ElementRepository.GetElement(element);
                    if (dataElement != null)
                    {
                        m_ModeElements.Add(dataElement.Title);
                        UpdateModeElements();
                    }
                }
            );
        }

        public void ModeElementStopped(int element)
        {
            DispatchToUIThread(() =>
            {
                CommandButtonMapping.Instance.CommandStateChanged(element, false);
                Ares.Data.IElement dataElement = Ares.Data.DataModule.ElementRepository.GetElement(element);
                if (dataElement != null)
                {
                    m_ModeElements.Remove(dataElement.Title);
                    UpdateModeElements();
                }
            }
            );
        }

        public void AllModeElementsStopped()
        {
            DispatchToUIThread(() =>
            {
                CommandButtonMapping.Instance.AllCommandsInactive();
                m_ModeElements.Clear();
                UpdateModeElements();
            }
            );
        }

        public void VolumeChanged(int index, int volume)
        {
            DispatchToUIThread(() =>
            {
                m_Listen = false;
                switch (index)
                {
                    case 2:
                        overallVolumeBar.Value = volume;
                        break;
                    case 1:
                        musicBar.Value = volume;
                        break;
                    case 0:
                        soundsBar.Value = volume;
                        break;
                    default:
                        break;
                }
                m_Listen = true;
            }
            );
        }

        public void MusicChanged(string newMusic, string shortTitle)
        {
            DispatchToUIThread(() =>
            {
                String fit = Compact(newMusic, musicLabel);
                musicLabel.Text = fit;
                m_Listen = false;
                for (int i = 0; i < musicList.Items.Count; ++i)
                {
                    if ((String)musicList.Items[i] == shortTitle)
                    {
                        musicList.SelectedIndex = i;
                        break;
                    }
                }
                m_Listen = true;
            }
            );
        }

        public void ProjectChanged(string newTitle)
        {
            DispatchToUIThread(() =>
            {
                m_PlayerProject = newTitle;
                UpdateProjectTitle();
            }
            );
        }

        List<Controllers.MusicListItem> m_CurrentMusicList = new List<Controllers.MusicListItem>();

        public void MusicListChanged(List<Controllers.MusicListItem> newList)
        {
            DispatchToUIThread(() =>
            {
                m_CurrentMusicList = newList;
                if (modesList.SelectedIndex == 0)
                    UpdateElementsPanel();
            }
            );
        }

        public void Disconnect()
        {
            DispatchToUIThread(() =>
            {
                Controllers.Control.Instance.Disconnect(false);
                UpdateNetworkState();
            }
            );
        }

        public void ConnectionFailed()
        {
            DispatchToUIThread(() =>
            {
                Controllers.Control.Instance.Disconnect(false);
                UpdateNetworkState();
            }
            );
        }

        private void openButton_DropDownOpening(object sender, EventArgs e)
        {
            openButton.DropDown.Items.Clear();
            for (int i = 0; i < Settings.Default.LastUsedProjectsCount; ++i)
            {
                ToolStripItem item = new ToolStripMenuItem((String)Settings.Default["LastUsedProjectName" + i]);
                int j = i;
                item.Click += new EventHandler((Object, EventArgs) =>
                {
                    OpenFile((String)Settings.Default["LastUsedProjectFile" + j]);
                });
                openButton.DropDown.Items.Add(item);
            }
        }

        private List<int> m_CurrentButtons = new List<int>();

        private void UpdateElementsPanel()
        {
            int modeIndex = modesList.SelectedIndex;
            if (modeIndex == -1)
                return;
            if (modeIndex == 0)
            {
                musicList.BeginUpdate();
                musicList.Items.Clear();
                for (int i = 0; i < m_CurrentMusicList.Count; ++i)
                {
                    musicList.Items.Add(m_CurrentMusicList[i].Title);
                }
                musicList.EndUpdate();
                elementsPanel.Visible = false;
                CommandButtonMapping.Instance.ButtonsActive = false;
                musicList.Visible = true;
            }
            else
            {
                foreach (int id in m_CurrentButtons)
                {
                    CommandButtonMapping.Instance.UnregisterButton(id);
                }
                m_CurrentButtons.Clear();
                elementsPanel.Controls.Clear();

                if (Controllers.Control.Instance.Project != null &&
                    Controllers.Control.Instance.Project.GetModes().Count >= modeIndex)
                {
                    IList<Ares.Data.IModeElement> elements = Controllers.Control.Instance.Project.GetModes()[modeIndex - 1].GetElements();
                    int maxWidth = 0;
                    CheckBox checkBox = new CheckBox();
                    checkBox.Appearance = Appearance.Button;
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        checkBox.Text = elements[i].Title;
                        int width = checkBox.PreferredSize.Width + 15;
                        if (width > maxWidth)
                            maxWidth = width;
                    }
                    checkBox.Dispose();
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        int row = i / 4;
                        int column = i % 4;
                        checkBox = new CheckBox();
                        checkBox.Text = elements[i].Title;
                        checkBox.Appearance = Appearance.Button;
                        Size size = checkBox.PreferredSize;
                        size.Width = maxWidth;
                        checkBox.SetBounds(0, 0, size.Width, size.Height);
                        CommandButtonMapping.Instance.RegisterButton(elements[i].Id, checkBox);
                        m_CurrentButtons.Add(elements[i].Id);
                        elementsPanel.Controls.Add(checkBox);
                    }
                }
                else
                {
                    Label label = new Label();
                    label.Text = "Keine Elemente vorhanden";
                    elementsPanel.Controls.Add(label);
                }
                CommandButtonMapping.Instance.ButtonsActive = true;
                musicList.Visible = false;
                elementsPanel.Visible = true;
            }
        }

        private static String Compact(String text, Label label)
        {
            int maxWidth = label.Width;
            using (Graphics g = label.CreateGraphics())
            {
                Font font = label.Font;

                int width = (int)g.MeasureString(text, font).Width;
                if (width < maxWidth)
                    return text;

                int len = 0;
                int seg = text.Length;
                String fit = String.Empty;
                String ELLIPSIS = "...";

                // find the longest string that fits into
                // the control boundaries using bisection method 
                while (seg > 1)
                {
                    seg -= seg / 2;

                    int left = len + seg;
                    int right = text.Length;

                    if (left > right)
                        continue;

                    right -= left;
                    left = 0;

                    // build and measure a candidate string with ellipsis
                    String tst = text.Substring(0, left) +
                        ELLIPSIS + text.Substring(right);

                    int testWidth = (int)g.MeasureString(tst, font).Width;

                    // candidate string fits into control boundaries, 
                    // try a longer string
                    // stop when seg <= 1 
                    if (testWidth <= maxWidth)
                    {
                        len += seg;
                        fit = tst;
                    }
                }

                if (len == 0) // string can't fit into control
                {
                    return ELLIPSIS;
                }
                return fit;
            }
        }

        private void modesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateElementsPanel();
        }

        private void musicList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            if (musicList.SelectedIndex < m_CurrentMusicList.Count)
                Controllers.Control.Instance.SetMusicTitle(m_CurrentMusicList[musicList.SelectedIndex].Id);
        }

        private void Controller_Load(object sender, EventArgs e)
        {
            if (this.DesignMode || System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLowerInvariant().StartsWith("devenv"))
                return;

            if (!String.IsNullOrEmpty(Settings.Default.LastProjectFile))
            {
                OpenFile(Settings.Default.LastProjectFile);
            }

            m_HasLocalPlayer = FindLocalPlayer() != null;
            connectButton.Enabled = m_HasLocalPlayer;

            m_ServerSearch = new Controllers.ServerSearch(Settings.Default.ServerSearchPort);
            m_ServerSearch.ServerFound += new EventHandler<Controllers.ServerEventArgs>(m_ServerSearch_ServerFound);
            m_ServerSearch.StartSearch();

            if (Settings.Default.CheckForNewVersion || Settings.Default.StartLocalPlayer || Settings.Default.ShowOverlayPanel)
            {
                m_FirstTimer = new Timer();
                m_FirstTimer.Interval = 2000;
                m_FirstTimer.Tick += new EventHandler(m_FirstTimer_Tick);
                m_FirstTimer.Start();
            }
        }

        private void CloseOverlay()
        {
            if (overLayPanel.Visible)
            {
                overLayPanel.Visible = false;
                Settings.Default.ShowOverlayPanel = false;
                Settings.Default.Save();
            }
        }

        private void homepageLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Ares.Online.OnlineOperations.ShowHomepage(null);
        }

        private void setupLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Ares.Online.OnlineOperations.DownloadLatestSetup(null, true);
        }

        private void settingsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseOverlay();
            ShowSettings();
        }

        private void overlayCloseButton_Click(object sender, EventArgs e)
        {
            CloseOverlay();
        }

        private void overlayOKButton_Click(object sender, EventArgs e)
        {
            CloseOverlay();
        }
    }
}
