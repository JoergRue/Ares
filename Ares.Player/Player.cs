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
using Ares.Players;
using Ares.Playing;
using Ares.Settings;

namespace Ares.Player
{
    public partial class Player : Form, INetworkClient
    {
#if !MONO
        private Ares.Ipc.ApplicationInstance m_Instance;
#endif

        private Network m_Network;

        private bool m_HideToTray = false;
        private bool m_FirstShow = true;

#if !MONO
        private readonly MouseKeyboardActivityMonitor.KeyboardHookListener m_KeyboardHookManager;
#endif

        public Player()
        {
            String projectName = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1] : String.Empty;
            if (projectName.StartsWith("Language="))
            {
                projectName = Environment.GetCommandLineArgs().Length > 2 ? Environment.GetCommandLineArgs()[2] : String.Empty;
            }
            if (projectName.StartsWith("--minimized"))
            {
                m_HideToTray = true;
                projectName = Environment.GetCommandLineArgs().Length > 3 ? Environment.GetCommandLineArgs()[3] : String.Empty;
            }
#if !MONO
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
#endif
#if !MONO
            m_KeyboardHookManager = new MouseKeyboardActivityMonitor.KeyboardHookListener(new MouseKeyboardActivityMonitor.WinApi.AppHooker());
            m_KeyboardHookManager.Enabled = true;
            m_KeyboardHookManager.KeyDown += new KeyEventHandler(m_KeyboardHookManager_KeyDown);
#endif
            InitializeComponent();
            if (!m_HideToTray)
            {
                notifyIcon.Visible = false;
            }
#if !MONO
            else
            {
                Visible = false;
                WindowState = FormWindowState.Minimized;
            }
#endif
#if !MONO
            m_Instance.SetWindowHandle(Handle);
            m_Instance.ProjectOpenAction = (projectName2, projectPath) => OpenProjectFromRequest(projectName2, projectPath);
#endif
            m_PlayingControl = new PlayingControl();
        }

#if !MONO
        void m_KeyboardHookManager_KeyDown(object sender, KeyEventArgs e)
        {
            m_PlayingControl.KeyReceived(GetPlayersKey(e.KeyData));
        }
#endif

        private void MessageReceived(Ares.Players.Message m)
        {
            if (m.Type >= MessageType.Warning && (int)m.Type >= Ares.Settings.Settings.Instance.MessageFilterLevel)
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
            if (WindowState == FormWindowState.Minimized && m_HideToTray)
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

        bool m_InFileSystemWatcherHandler = false;

        private void StopAllPlaying()
        {
            PlayingModule.ProjectPlayer.StopAll();
            if (PlayingModule.Streamer.IsStreaming)
            {
                PlayingModule.Streamer.EndStreaming();
            }
        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (m_Project == null || m_InFileSystemWatcherHandler)
                return;
            m_InFileSystemWatcherHandler = true;
            String path = m_Project.FileName;
            if (e.FullPath == path)
            {
                if (MessageBox.Show(this, StringResources.ReloadProject, StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    StopAllPlaying();
                    OpenProject(path, false);
                }
            }
            m_InFileSystemWatcherHandler = false;
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
            StopAllPlaying();
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            Ares.Settings.SettingsDialog dialog = new Ares.Settings.SettingsDialog(Ares.Settings.Settings.Instance, m_BasicSettings);
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_PlayingControl.UpdateDirectories();
                LoadTagsDB();
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void OpenProject()
        {
            StopAllPlaying();
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return;
            OpenProject(openFileDialog1.FileName, false);
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
                    OpenProject(projectPath, false);
                }
            }
        }

        private void OpenProjectFromController(String fileName)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => OpenProjectFromController(fileName)));
                return;
            }
            String path = fileName;
            if (!System.IO.Path.IsPathRooted(path))
            {
                String oldPath = m_Project != null ? m_Project.FileName : System.Environment.CurrentDirectory;
                if (m_Project != null)
                {
                    oldPath = System.IO.Directory.GetParent(oldPath).FullName;
                }
                path = oldPath + System.IO.Path.DirectorySeparatorChar + fileName;
            }
            if (System.IO.File.Exists(path) && !path.Equals(m_CurrentProjectPath, StringComparison.OrdinalIgnoreCase))
            {
                OpenProject(path, true);
            }
        }

        private String m_CurrentProjectPath = String.Empty;

        private void OpenProject(String filePath, bool onControllerRequest)
        {
            StopAllPlaying();
            if (m_Project != null)
            {
                if (onControllerRequest && m_Project.FileName.Equals(filePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (m_Network != null)
                    {
                        m_Network.InformClientOfProject(m_Project.Title);
                    }
                    return;
                }
                Ares.Data.DataModule.ProjectManager.UnloadProject(m_Project);
                m_Project = null;
                m_TagLanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
#if !MONO
                m_Instance.SetLoadedProject("-");
#endif
            }
            try
            {
                m_Project = Ares.Data.DataModule.ProjectManager.LoadProject(filePath);
                m_CurrentProjectPath = filePath;
                if (m_Project != null && m_Project.TagLanguageId != -1)
                {
                    m_TagLanguageId = m_Project.TagLanguageId;
                }
                else
                {
                    m_TagLanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                }
                Ares.Settings.Settings.Instance.RecentFiles.AddFile(new RecentFiles.ProjectEntry(m_Project.FileName, m_Project.Title));
            }
            catch (Exception e)
            {
                if (!onControllerRequest)
                {
                    MessageBox.Show(this, String.Format(StringResources.LoadError, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    m_Network.ErrorOccurred(-1, String.Format(StringResources.LoadError, e.Message));
                }
                m_Project = null;
            }
            projectNameLabel.Text = m_Project != null ? m_Project.Title : StringResources.NoOpenedProject;
            this.Text = String.Format(StringResources.AresPlayer, projectNameLabel.Text);
            PlayingModule.ProjectPlayer.SetProject(m_Project);
            DoModelChecks();
            if (m_Project != null)
            {
                fileSystemWatcher1.Path = System.IO.Path.GetDirectoryName(m_Project.FileName);
            }
#if !MONO
            m_Instance.SetLoadedProject(filePath);
#endif
            if (m_Network != null)
            {
                m_Network.InformClientOfProject(m_Project != null ? m_Project.Title : String.Empty);
            }
            UpdateModesList();
        }

        private bool m_Listen = true;

        private void UpdateModesList()
        {
            foreach (CheckBox box in m_CurrentButtons.Keys)
            {
                box.CheckedChanged -= m_CurrentButtons[box];
            }
            m_CurrentButtons.Clear();
            foreach (CheckBox box in m_CurrentTagButtons.Keys)
            {
                box.CheckedChanged -= m_CurrentTagButtons[box];
            }
            m_ButtonsForIds.Clear();

            modesList.BeginUpdate();
            modesList.Items.Clear();
            m_Listen = false;
            if (m_Project != null)
            {
                IList<Ares.Data.IMode> modes = m_Project.GetModes();
                if (modes.Count == 0)
                    return;
                modesList.Items.Add(StringResources.MusicList);
                modesList.Items.Add(StringResources.MusicTags);
                int i = 2;
                int currentMode = 0;
                bool showKeys = Settings.Settings.Instance.ShowKeysInButtons;
                KeysConverter converter = new KeysConverter();
                foreach (Ares.Data.IMode mode in modes)
                {
                    String title = mode.Title;
                    if (showKeys && mode.KeyCode != 0)
                    {
                        title += " (" + converter.ConvertToString((System.Windows.Forms.Keys)mode.KeyCode) + ")";
                    }
                    modesList.Items.Add(title);
                    if (mode == m_PlayingControl.CurrentMode)
                    {
                        currentMode = i;
                    }
                    ++i;
                }
                modesList.SelectedIndex = currentMode;
            }
            modesList.EndUpdate();
            UpdateElementsPanel();
            m_Listen = true;
        }

        private Dictionary<CheckBox, EventHandler> m_CurrentButtons = new Dictionary<CheckBox, EventHandler>();
        private Dictionary<CheckBox, EventHandler> m_CurrentTagButtons = new Dictionary<CheckBox, EventHandler>();
        private Dictionary<int, CheckBox> m_ButtonsForIds = new Dictionary<int, CheckBox>();
        private bool m_ButtonsActive = true;
        private bool m_TagsControlsActive = false;

        private int m_TagLanguageId;
        private int m_LastTagCategoryId = -1;
        IList<Ares.Tags.CategoryForLanguage> m_MusicTagCategories;
        
        private void UpdateElementsPanel()
        {
            int modeIndex = modesList.SelectedIndex;
            if (modeIndex == -1)
                return;
            if (modeIndex == 0)
            {
                musicList.BeginUpdate();
                musicList.Items.Clear();
                Ares.Data.IElement element = Ares.Data.DataModule.ElementRepository.GetElement(m_PlayingControl.CurrentMusicList);
                if (element != null && element is Ares.Data.IMusicList)
                {
                    Ares.Data.IMusicList currentMusicList = (Ares.Data.IMusicList)element;
                    var fileElements = currentMusicList.GetFileElements();
                    int selIndex = -1;
                    for (int i = 0; i < fileElements.Count; ++i)
                    {
                        musicList.Items.Add(fileElements[i].Title);
                        if (fileElements[i].Id == m_PlayingControl.CurrentMusicElement)
                            selIndex = i;
                    }
                    if (selIndex != -1)
                        musicList.SelectedIndex = selIndex;
                }
                musicList.EndUpdate();
                elementsPanel.Visible = false;
                tagsPanel.Visible = false;
                m_ButtonsActive = false;
                m_TagsControlsActive = false;
                musicList.Visible = true;
            }
            else if (modeIndex == 1)
            {
                foreach (CheckBox box in m_CurrentTagButtons.Keys)
                {
                    box.CheckedChanged -= m_CurrentTagButtons[box];
                }
                m_CurrentTagButtons.Clear();

                m_TagsControlsActive = false;
                Ares.Tags.ITagsDBReadByLanguage dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_TagLanguageId);

                musicTagCategoryBox.Items.Clear();
                int selIndex = -1;
                try
                {
                    m_MusicTagCategories = dbRead.GetAllCategories();
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    Messages.AddMessage(MessageType.Error, String.Format(StringResources.TagsDbError, ex.Message));
                    m_MusicTagCategories = new List<Ares.Tags.CategoryForLanguage>();
                }
                foreach (var category in m_MusicTagCategories)
                {
                    musicTagCategoryBox.Items.Add(category.Name);
                    if (category.Id == m_LastTagCategoryId)
                        selIndex = musicTagCategoryBox.Items.Count - 1;
                }
                if (selIndex == -1 && musicTagCategoryBox.Items.Count > 0)
                {
                    selIndex = 0;
                    m_LastTagCategoryId = m_MusicTagCategories[0].Id;
                }
                if (selIndex != -1)
                {
                    musicTagCategoryBox.SelectedIndex = selIndex;
                }

                HashSet<int> currentTags = m_PlayingControl.GetCurrentMusicTags();

                tagSelectionPanel.Controls.Clear();
                if (m_LastTagCategoryId != -1)
                {
                    IList<Ares.Tags.TagForLanguage> tags;
                    try 
                    {
                        tags = dbRead.GetAllTags(m_LastTagCategoryId);
                    }
                    catch (Ares.Tags.TagsDbException ex)
                    {
                        Messages.AddMessage(MessageType.Error, String.Format(StringResources.TagsDbError, ex.Message));
                        tags = new List<Ares.Tags.TagForLanguage>();
                    }

                    int maxWidth = 0;
                    CheckBox checkBox = new CheckBox();
                    checkBox.Appearance = Appearance.Button;
                    for (int i = 0; i < tags.Count; ++i)
                    {
                        String text = tags[i].Name;
                        checkBox.Text = text;
                        int width = checkBox.PreferredSize.Width;
                        if (width > maxWidth)
                            maxWidth = width;
                    }
                    checkBox.Dispose();
                    int count = 0;
                    for (int i = 0; i < tags.Count; ++i)
                    {
                        //if (!elements[i].IsVisibleInPlayer)
                        //    continue;
                        ++count;
                        int row = count / 4;
                        int column = count % 4;
                        checkBox = new CheckBox();
                        String text = tags[i].Name;
                        checkBox.Text = text;
                        checkBox.Appearance = Appearance.Button;
                        Size size = checkBox.PreferredSize;
                        size.Width = maxWidth;
                        checkBox.SetBounds(0, 0, size.Width, size.Height);
                        checkBox.Checked = currentTags.Contains(tags[i].Id);
                        int tagId = tags[i].Id;
                        int categoryId = m_LastTagCategoryId;
                        EventHandler handler = new EventHandler((Object sender, EventArgs args) =>
                        {
                            if (m_Listen && m_TagsControlsActive)
                            {
                                if ((sender as CheckBox).Checked)
                                {
                                    m_PlayingControl.AddMusicTag(categoryId, tagId);
                                }
                                else
                                {
                                    m_PlayingControl.RemoveMusicTag(categoryId, tagId);
                                }
                            }
                        });
                        checkBox.CheckedChanged += handler;
                        m_CurrentTagButtons.Add(checkBox, handler);
                        tagSelectionPanel.Controls.Add(checkBox);
                    }
                    if (count == 0)
                    {
                        Label label = new Label();
                        label.Text = StringResources.NoTags;
                        label.SetBounds(0, 0, label.PreferredSize.Width, label.PreferredSize.Height);
                        tagSelectionPanel.Controls.Add(label);
                    }
                }

                tagCategoriesAndButton.Checked = m_PlayingControl.IsMusicTagCategoriesOperatorAnd();
                tagCategoriesOrButton.Checked = !tagCategoriesAndButton.Checked;

                StringBuilder currentTagsBuilder = new StringBuilder();
                IList<Ares.Tags.TagInfoForLanguage> currentTagInfos;
                try 
                { 
                    currentTagInfos = dbRead.GetTagInfos(currentTags);
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    Messages.AddMessage(MessageType.Error, String.Format(StringResources.TagsDbError, ex.Message));
                    currentTagInfos = new List<Ares.Tags.TagInfoForLanguage>();
                }
                int catId = -1;
                foreach (var tagInfo in currentTagInfos)
                {
                    if (tagInfo.CategoryId != catId)
                    {
                        if (currentTagsBuilder.Length > 0)
                        {
                            currentTagsBuilder.Append("; ");
                        }
                        currentTagsBuilder.Append(tagInfo.Category);
                        currentTagsBuilder.Append(": ");
                        catId = tagInfo.CategoryId;
                    }
                    else
                    {
                        currentTagsBuilder.Append(", ");
                    }
                    currentTagsBuilder.Append(tagInfo.Name);
                }
                currentTagsLabel.Text = currentTagsBuilder.ToString();

                elementsPanel.Visible = false;
                tagsPanel.Visible = true;
                m_TagsControlsActive = true;
                m_ButtonsActive = false;
                musicList.Visible = false;
            }
            else
            {
                foreach (CheckBox box in m_CurrentButtons.Keys)
                {
                    box.CheckedChanged -= m_CurrentButtons[box];
                }
                m_CurrentButtons.Clear();
                m_ButtonsForIds.Clear();
                elementsPanel.Controls.Clear();

                if (m_Project != null && m_Project.GetModes().Count > modeIndex - 2)
                {
                    bool showKeys = Settings.Settings.Instance.ShowKeysInButtons;
                    KeysConverter converter = new KeysConverter();
                    IList<Ares.Data.IModeElement> elements = m_Project.GetModes()[modeIndex - 2].GetElements();
                    int maxWidth = 0;
                    CheckBox checkBox = new CheckBox();
                    checkBox.Appearance = Appearance.Button;
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        if (!elements[i].IsVisibleInPlayer)
                            continue;
                        String text = elements[i].Title;
                        if (showKeys && elements[i].Trigger != null && elements[i].Trigger.TriggerType == TriggerType.Key)
                        {
                            int keyCode = (elements[i].Trigger as IKeyTrigger).KeyCode;
                            if (keyCode != 0)
                                text += " (" + converter.ConvertToString((System.Windows.Forms.Keys)keyCode) + ")";
                        }
                        checkBox.Text = text;
                        int width = checkBox.PreferredSize.Width;
                        if (width > maxWidth)
                            maxWidth = width;
                    }
                    checkBox.Dispose();
                    int count = 0;
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        if (!elements[i].IsVisibleInPlayer)
                            continue;
                        ++count;
                        int row = count / 4;
                        int column = count % 4;
                        checkBox = new CheckBox();
                        String text = elements[i].Title;
                        if (showKeys && elements[i].Trigger != null && elements[i].Trigger.TriggerType == TriggerType.Key)
                        {
                            int keyCode = (elements[i].Trigger as IKeyTrigger).KeyCode;
                            if (keyCode != 0)
                                text += " (" + converter.ConvertToString((System.Windows.Forms.Keys)keyCode) + ")";
                        }
                        checkBox.Text = text;
                        checkBox.Appearance = Appearance.Button;
                        Size size = checkBox.PreferredSize;
                        size.Width = maxWidth;
                        checkBox.SetBounds(0, 0, size.Width, size.Height);
                        checkBox.Checked = m_PlayingControl.CurrentModeElements.Contains(elements[i]);
                        int id = elements[i].Id;
                        EventHandler handler = new EventHandler((Object, EventArgs) =>
                        {
                            if (m_Listen && m_ButtonsActive)
                            {
                                m_PlayingControl.SwitchElement(id);
                            }
                        });
                        checkBox.CheckedChanged += handler;
                        m_CurrentButtons.Add(checkBox, handler);
                        m_ButtonsForIds.Add(id, checkBox);
                        elementsPanel.Controls.Add(checkBox);
                    }
                    if (count == 0)
                    {
                        Label label = new Label();
                        label.Text = StringResources.NoElements;
                        label.SetBounds(0, 0, label.PreferredSize.Width, label.PreferredSize.Height);
                        elementsPanel.Controls.Add(label);
                    }
                }
                else
                {
                    Label label = new Label();
                    label.Text = StringResources.NoElements;
                    label.SetBounds(0, 0, label.PreferredSize.Width, label.PreferredSize.Height);
                    elementsPanel.Controls.Add(label);
                }
                m_ButtonsActive = true;
                musicList.Visible = false;
                tagsPanel.Visible = false;
                m_TagsControlsActive = false;
                elementsPanel.Visible = true;
            }
        }

        private void DoModelChecks()
        {
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
            foreach (Ares.ModelInfo.ModelError error in Ares.ModelInfo.ModelChecks.Instance.GetAllErrors())
            {
                Messages.AddMessage(error.Severity == ModelInfo.ModelError.ErrorSeverity.Error ? MessageType.Error : MessageType.Warning,
                    error.Message);
            }
        }

        private int GetPlayersKey(System.Windows.Forms.Keys key)
        {
            return (int)key;
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
#if MONO
            const int WM_KEYDOWN = 0x100;
            if (msg.Msg == WM_KEYDOWN)
            {
                return m_PlayingControl.KeyReceived(GetPlayersKey(keyData));
            }
#endif
            if (keyData == (System.Windows.Forms.Keys.F4 | System.Windows.Forms.Keys.Alt))
            {
                StopAllPlaying();
                Close();
                return true;
            }
            return false;
        }

        private int lastMusicElementId = -1;
        private String lastMode = String.Empty;
        private int lastMusicListId = -1;
        private HashSet<int> lastTagIds = null;

        private void UpdateStatus()
        {
            PlayingControl control = m_PlayingControl;
            m_Listen = false;
            IMode mode = control.CurrentMode;
            modeLabel.Text = mode != null ? mode.Title : String.Empty;
            if (m_Project != null && mode.Title != lastMode)
            {
                for (int i = 0; i < m_Project.GetModes().Count; ++i)
                {
                    if (m_Project.GetModes()[i].Title == mode.Title)
                    {
                        modesList.SelectedIndex = i + 2;
                        break;
                    }
                }
                UpdateElementsPanel();
                lastMode = mode.Title;
            }
            StringBuilder modeElementsText = new StringBuilder();
            HashSet<int> currentModeElementIds = new HashSet<int>();
            foreach (IModeElement modeElement in control.CurrentModeElements)
            {
                if (modeElementsText.Length > 0)
                    modeElementsText.Append(", ");
                modeElementsText.Append(modeElement.Title);
                currentModeElementIds.Add(modeElement.Id);
            }
            foreach (int id in m_ButtonsForIds.Keys)
            {
                m_ButtonsForIds[id].Checked = currentModeElementIds.Contains(id);
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
                musicLabel.Text = MusicInfo.GetInfo(musicElementId).LongTitle;
                lastMusicElementId = musicElementId;
                var currentMusicList = Ares.Data.DataModule.ElementRepository.GetElement(m_PlayingControl.CurrentMusicList) as IMusicList;
                var elements = currentMusicList != null ? currentMusicList.GetFileElements() : null;
                if (elements != null && musicList.Items.Count > 0)
                {
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        if (elements[i].Id == musicElementId)
                        {
                            if (musicList.Items.Count > i)
                                musicList.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            bool repeat = m_PlayingControl.MusicRepeat;
            if (musicElementId == -1)
            {
                repeat = false;
            }
            repeatButton.Checked = repeat;
            repeatCurrentMusicToolStripMenuItem.Checked = repeat;
            if (control.CurrentMusicList != lastMusicListId && modesList.SelectedIndex == 0)
            {
                UpdateElementsPanel();
            }
            else if (modesList.SelectedIndex == 1)
            {
                HashSet<int> tags = lastTagIds != null ? new HashSet<int>(lastTagIds) : new HashSet<int>();
                HashSet<int> currentTags = control.GetCurrentMusicTags();
                tags.SymmetricExceptWith(currentTags);
                if (tags.Count > 0)
                {
                    // current tag set has changed
                    lastTagIds = currentTags;
                    UpdateElementsPanel();
                }
                else
                {
                    m_TagsControlsActive = false;
                    tagCategoriesAndButton.Checked = m_PlayingControl.IsMusicTagCategoriesOperatorAnd();
                    tagCategoriesOrButton.Checked = !tagCategoriesAndButton.Checked;
                    m_TagsControlsActive = true;
                }
            }
            lastMusicListId = control.CurrentMusicList;
            m_Listen = true;
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
            m_Network.Shutdown();
            m_PlayingControl.Dispose();
#if !MONO
            m_KeyboardHookManager.Dispose();
#endif
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
#if !MONO
            Ares.Settings.Settings.Instance.SettingsChanged += new EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
#endif
        }
		
#if !MONO
        void SettingsChanged(object sender, Settings.Settings.SettingsEventArgs e)
        {
            SettingsChanged(e.FundamentalChange);
        }
#endif

        private void SettingsChanged(bool fundamentalChange)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => SettingsChanged(fundamentalChange)));
                return;
            }
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            if (fundamentalChange)
            {
                m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Escape);
                m_PlayingControl.UpdateDirectories();
                LoadTagsDB();
            }
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

        private void LoadTagsDB()
        {
            try
            {
                Ares.Tags.ITagsDBFiles tagsDBFiles = Ares.Tags.TagsModule.GetTagsDB().FilesInterface;
                String path = System.IO.Path.Combine(Ares.Settings.Settings.Instance.MusicDirectory, tagsDBFiles.DefaultFileName);
                tagsDBFiles.OpenOrCreateDatabase(path);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
#if !MONO
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
#endif
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Escape);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Left);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Right);
        }

        public void KeyReceived(int key)
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

        public void ProjectShallChange(String newProjectPath)
        {
            OpenProjectFromController(newProjectPath);
        }

        public void PlayOtherMusic(Int32 id)
        {
            m_PlayingControl.SelectMusicElement(id);
        }

        public void SwitchElement(Int32 id)
        {
            m_PlayingControl.SwitchElement(id);
        }

        public void SetMusicRepeat(bool repeat)
        {
            m_PlayingControl.SetRepeatCurrentMusic(repeat);
        }

        public void SwitchTag(Int32 categoryId, Int32 tagId, bool active)
        {
            if (active)
            {
                m_PlayingControl.AddMusicTag(categoryId, tagId);
            }
            else
            {
                m_PlayingControl.RemoveMusicTag(categoryId, tagId);
            }
        }

        public void DeactivateAllTags()
        {
            m_PlayingControl.RemoveAllMusicTags();
        }

        public void SetTagCategoryOperator(bool operatorIsAnd)
        {
            m_PlayingControl.SetMusicTagCategoriesOperator(operatorIsAnd);
        }

        private bool m_WasConnected = false;

        private void UpdateClientData()
        {
            if (m_Network.ClientConnected)
            {
                clientStateLabel.Text = StringResources.ConnectedWith + m_Network.ClientName;
                clientStateLabel.ForeColor = System.Drawing.Color.DarkGreen;
                m_Network.InformClientOfEverything(m_PlayingControl.GlobalVolume, m_PlayingControl.MusicVolume,
                    m_PlayingControl.SoundVolume, m_PlayingControl.CurrentMode, MusicInfo.GetInfo(m_PlayingControl.CurrentMusicElement),
                    m_PlayingControl.CurrentModeElements, m_Project != null ? m_Project.Title : String.Empty, 
                    m_PlayingControl.CurrentMusicList, m_PlayingControl.MusicRepeat,
                    m_TagLanguageId, new List<int>(m_PlayingControl.GetCurrentMusicTags()), m_PlayingControl.IsMusicTagCategoriesOperatorAnd());
                disconnectButton.Text = StringResources.Disconnect;
                m_WasConnected = true;
            }
            else
            {
                clientStateLabel.Text = StringResources.NotConnected;
                clientStateLabel.ForeColor = System.Drawing.Color.Red;
                disconnectButton.Text = Settings.Settings.Instance.NetworkEnabled ? StringResources.Disable : StringResources.Enable;
                if (m_HideToTray && m_WasConnected)
                {
                    Close();
                }
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (m_Network.ClientConnected)
            {
                m_Network.DisconnectClient(true);
            }
            else
            {
                bool isNowEnabled = !Settings.Settings.Instance.NetworkEnabled;
                Settings.Settings.Instance.NetworkEnabled = isNowEnabled;
                Settings.Settings.Instance.Commit();
                disconnectButton.Text = isNowEnabled ? StringResources.Disable : StringResources.Enable;
                if (isNowEnabled)
                {
                    clientStateLabel.Text = StringResources.NotConnected;
                    m_Network.StartUdpBroadcast();
                }
                else
                {
                    m_Network.StopUdpBroadcast();
                    clientStateLabel.Text = StringResources.Disabled;
                }
                clientStateLabel.ForeColor = System.Drawing.Color.Red;
            }
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
            OpenProject(((sender as ToolStripMenuItem).Tag as RecentFiles.ProjectEntry).FilePath, false);
        }

        private void Player_Load(object sender, EventArgs e)
        {
            m_BasicSettings = new BasicSettings();
            ReadSettings();
            showKeysMenuItem.Checked = Settings.Settings.Instance.ShowKeysInButtons;
#if !MONO
            globalKeyHookItem.Checked = Settings.Settings.Instance.GlobalKeyHook;
            if (Settings.Settings.Instance.GlobalKeyHook)
            {
                m_KeyboardHookManager.Replace(new MouseKeyboardActivityMonitor.WinApi.GlobalHooker());
            }
#else
            globalKeyHookItem.Checked = false;
            globalKeyHookItem.Enabled = false;
#endif
            Messages.Instance.MessageReceived += new MessageReceivedHandler(MessageReceived);
            String projectName = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1] : String.Empty;
            if (projectName.StartsWith("Language="))
            {
                projectName = Environment.GetCommandLineArgs().Length > 2 ? Environment.GetCommandLineArgs()[2] : String.Empty;
            }
            if (projectName.StartsWith("--minimized"))
            {
                m_HideToTray = true;
                projectName = Environment.GetCommandLineArgs().Length > 3 ? Environment.GetCommandLineArgs()[3] : String.Empty;
            }
            if (!String.IsNullOrEmpty(projectName))
            {
                if (projectName.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
                    ImportProject(projectName);
                else
                    OpenProject(projectName, false);
            }
            else if (Ares.Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
            {
                OpenProject(Ares.Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath, false);
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
#if MONO
			editorButton.Enabled = false;
			startEditorToolStripMenuItem.Enabled = false;
			streamingToolStripMenuItem.Enabled = false;
#endif
            listenForPorts = true;
            m_Network = new Network(this);
            m_Network.InitConnectionData();
            if (Settings.Settings.Instance.NetworkEnabled)
            {
                m_Network.StartUdpBroadcast();
                disconnectButton.Text = StringResources.Disable;
                clientStateLabel.Text = StringResources.NotConnected;
            }
            else
            {
                disconnectButton.Text = StringResources.Enable;
                clientStateLabel.Text = StringResources.Disabled;
            }
            clientStateLabel.ForeColor = System.Drawing.Color.Red;
            broadCastTimer.Tick += new EventHandler(broadCastTimer_Tick);
            broadCastTimer.Enabled = true;
            m_Network.ListenForClient();
            Timer updateTimer = new Timer();
            updateTimer.Interval = 50;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Start();
            fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(fileSystemWatcher1_Changed);
            if (Settings.Settings.Instance.CheckForUpdate)
            {
                Ares.Online.OnlineOperations.CheckForUpdate(this, false);
            }
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
            if (Settings.Settings.Instance.NetworkEnabled)
            {
                m_Network.StartUdpBroadcast();
            }
        }

        private void helpOnlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.Online.OnlineOperations.ShowHelppage(StringResources.HelpPage, this);
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.Online.OnlineOperations.CheckForUpdate(this, true);
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Player_Resize(object sender, EventArgs e)
        {
#if !MONO
            if (m_HideToTray && WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
#endif
        }

        private void Player_Shown(object sender, EventArgs e)
        {
            if (m_HideToTray && m_FirstShow)
            {
                Visible = false;
                WindowState = FormWindowState.Minimized;
            }
            m_FirstShow = false;
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopAllPlaying();

            DialogResult result = importFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return;
            ImportProject(importFileDialog.FileName);
        }

        private void ImportProject(String fileName)
        {
            String defaultProjectName = fileName;
            if (defaultProjectName.EndsWith(".apkg"))
            {
                defaultProjectName = defaultProjectName.Substring(0, defaultProjectName.Length - 5);
            }
            defaultProjectName = defaultProjectName + ".ares";
            saveFileDialog.FileName = defaultProjectName;
            DialogResult result = saveFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return;

            Ares.ModelInfo.Importer.Import(this, fileName, saveFileDialog.FileName, 
                () => OpenProject(saveFileDialog.FileName, false));
        }

        private void streamingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopAllPlaying();
            StreamingDialog dialog = new StreamingDialog();
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Settings.Instance.Commit();
            }
        }

        private void modesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            if (modesList.SelectedIndex > 1 && m_Project != null && (modesList.SelectedIndex - 2) < m_Project.GetModes().Count)
            {
                m_PlayingControl.SetMode(m_Project.GetModes()[modesList.SelectedIndex - 2]);
            }
            m_Listen = false;
            UpdateElementsPanel();
            m_Listen = true;
        }

        private void musicList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            IMusicList list = Ares.Data.DataModule.ElementRepository.GetElement(m_PlayingControl.CurrentMusicList) as IMusicList;
            var elements = list != null ? list.GetFileElements() : null;
            if (elements != null && musicList.SelectedIndex < elements.Count)
            {
                m_PlayingControl.SelectMusicElement(elements[musicList.SelectedIndex].Id);
            }
        }

        private void showKeysMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Settings.Instance.ShowKeysInButtons = showKeysMenuItem.Checked;
            Settings.Settings.Instance.Commit();
            m_Listen = false;
            UpdateModesList();
            m_Listen = true;
        }

        private void globalKeyHookItem_CheckedChanged(object sender, EventArgs e)
        {
#if !MONO
            Settings.Settings.Instance.GlobalKeyHook = globalKeyHookItem.Checked;
            Settings.Settings.Instance.Commit();
            MouseKeyboardActivityMonitor.WinApi.Hooker hook;

            if (!globalKeyHookItem.Checked)
            {
                hook = new MouseKeyboardActivityMonitor.WinApi.AppHooker();
            }
            else
            {
                hook = new MouseKeyboardActivityMonitor.WinApi.GlobalHooker();
            }

            m_KeyboardHookManager.Replace(hook);
#endif
            
        }

        private void repeatButton_Click(object sender, EventArgs e)
        {
            m_PlayingControl.SetRepeatCurrentMusic(!m_PlayingControl.MusicRepeat);
        }

        private void musicTagCategoryBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_TagsControlsActive)
            {
                m_LastTagCategoryId = m_MusicTagCategories[musicTagCategoryBox.SelectedIndex].Id;
                UpdateElementsPanel();
            }
        }

        private void clearTagsButton_Click(object sender, EventArgs e)
        {
            if (m_TagsControlsActive)
            {
                m_PlayingControl.RemoveAllMusicTags();
            }
        }

        private void tagCategoriesOrButton_CheckedChanged(object sender, EventArgs e)
        {
            if (m_TagsControlsActive)
            {
                m_PlayingControl.SetMusicTagCategoriesOperator(!tagCategoriesOrButton.Checked);
            }
        }

        private void tagCategoriesAndButton_CheckedChanged(object sender, EventArgs e)
        {
            if (m_TagsControlsActive)
            {
                m_PlayingControl.SetMusicTagCategoriesOperator(tagCategoriesAndButton.Checked);
            }
        }
    }
}
