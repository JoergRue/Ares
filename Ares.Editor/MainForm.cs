/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Settings;
using Ares.Editor.Plugins;
using Ares.Editor.AudioSourceSearch;

namespace Ares.Editor
{
    public partial class MainForm : Form, ErrorWindow.IErrorWindowClient, IFileExplorerParent, ElementEditors.IFileTagsEditorParent
    {
		#if !MONO
        private Ares.Ipc.ApplicationInstance m_Instance;
#endif

        private PluginManager m_PluginManager;

        public MainForm()
        {
            String projectName = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1] : String.Empty;
            if (projectName.StartsWith("Language="))
            {
                projectName = Environment.GetCommandLineArgs().Length > 2 ? Environment.GetCommandLineArgs()[2] : String.Empty;
            }
			#if !MONO
            if (String.IsNullOrEmpty(projectName))
            {
                m_Instance = Ares.Ipc.ApplicationInstance.CreateOrActivate("Ares.Editor");
            }
            else
            {
                m_Instance = Ares.Ipc.ApplicationInstance.CreateOrActivate("Ares.Editor", projectName);
            }
            if (m_Instance == null)
            {
                throw new Ares.Ipc.ApplicationAlreadyStartedException();
            }
            #endif

            m_PluginManager = new Plugins.PluginManager();

            PrepareModelChecks();
            InitializeComponent();
			#if !MONO
            m_Instance.SetWindowHandle(Handle);
            m_Instance.ProjectOpenAction = (projectName2, projectPath) => OpenProjectFromRequest(projectName2, projectPath);
			#endif
            m_ProjectName = projectName;
        }

        private void PrepareModelChecks()
        {
            Ares.ModelInfo.ModelChecks.Instance.AddCheck(new Ares.CommonGUI.KeyChecks());
        }

        private void SettingsChanged(object sender, Settings.Settings.SettingsEventArgs e)
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
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            Actions.Playing.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
            Actions.FilesWatcher.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
            Tags.TagsModule.GetTagsDB().FilesInterface.CloseDatabase();
            LoadTagsDB();
        }

        private void LoadTagsDB()
        {
            try
            {
                Ares.Tags.ITagsDBFiles tagsDBFiles = Ares.Tags.TagsModule.GetTagsDB().FilesInterface;
                String path = System.IO.Path.Combine(Ares.Settings.Settings.Instance.MusicDirectory, tagsDBFiles.DefaultFileName);
                tagsDBFiles.OpenOrCreateDatabase(path);
                Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Shutdown()
        {
            Actions.FilesWatcher.Instance.DeInit();
			#if !MONO
            Settings.Settings.Instance.SettingsChanged -= new EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
			#endif
            Settings.Settings.Instance.Shutdown();
            Tags.TagsModule.GetTagsDB().FilesInterface.CloseDatabase();
        }

        private void HandlePlayingError(Ares.Data.IElement element, String errorMessage)
        {
            if (element is Ares.Data.IFileElement)
            {
                errorMessage += "\n";
                errorMessage += String.Format(StringResources.FilePlayed, (element as Ares.Data.IFileElement).FilePath);
            }
            MessageBox.Show(this, errorMessage, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private WeifenLuo.WinFormsUI.Docking.IDockContent DeserializeDockContent(string persistString)
        {
            if (persistString.StartsWith("FileExplorer_"))
            {
                int index = 0;
                bool success = Int32.TryParse(persistString.Substring("FileExplorer_".Length), out index);
                if (!success)
                    return null;
                if (index < 0 || index > 1)
                    return null;
                m_FileExplorers[index] = new FileExplorer((FileType)index, this);
                return m_FileExplorers[index];
            }
            else if (persistString == "AudioSourceSearch")
            {
                m_AudioSourceSearch = new AudioSourceSearchWindow(m_PluginManager);
                return m_AudioSourceSearch;
            }
            else if (persistString == "ProjectExplorer")
            {
                m_ProjectExplorer = new ProjectExplorer();
                return m_ProjectExplorer;
            }
            else if (persistString == "VolumeWindow")
            {
                m_VolumeWindow = new VolumeWindow();
                return m_VolumeWindow;
            }
            else if (persistString == "ErrorWindow")
            {
                m_ErrorWindow = new ErrorWindow();
                m_ErrorWindow.Client = this;
                return m_ErrorWindow;
            }
            else if (persistString == "TagsEditor")
            {
                m_TagsEditor = new ElementEditors.TagsEditor();
                m_TagsEditor.SetProject(m_CurrentProject);
                return m_TagsEditor;
            }
            else if (persistString == "FileTagsEditor")
            {
                m_FileTagsEditor = new ElementEditors.FileTagsEditor();
                m_FileTagsEditor.SetParent(this);
                m_FileTagsEditor.SetProject(m_CurrentProject);
                m_FileTagsEditor.SetFiles(m_SelectedFiles);
                return m_FileTagsEditor;
            }
            else
            {
                return null;
            }
        }

        private void ShowSettingsDialog(int pageIndex)
        {
            CommonGUI.SettingsDialog dialog = new CommonGUI.SettingsDialog(Ares.Settings.Settings.Instance, m_BasicSettings);
            dialog.AddPage(new Dialogs.ToolsPageHost());
            dialog.AddPage(new Dialogs.OnlineDbPageHost());
            dialog.SetVisiblePage(pageIndex);
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                SettingsChanged(true);
                WriteSettings();
            }
        }

        private void projectExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowProjectExplorer();
        }

        private ProjectExplorer m_ProjectExplorer;

        private void ShowProjectExplorer()
        {
            if (m_ProjectExplorer == null)
            {
                m_ProjectExplorer = new ProjectExplorer();
                m_ProjectExplorer.SetProject(m_CurrentProject);
                m_ProjectExplorer.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                m_ProjectExplorer.Show(dockPanel);
            }
            else UpdateWindowState(m_ProjectExplorer);
            ActivateWindow(m_ProjectExplorer);
        }

        private ElementEditors.TagsEditor m_TagsEditor;

        private void ShowTagsEditor()
        {
            if (m_TagsEditor == null)
            {
                m_TagsEditor = new ElementEditors.TagsEditor();
                m_TagsEditor.SetProject(m_CurrentProject);
                m_TagsEditor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                m_TagsEditor.Show(dockPanel);
            }
            else UpdateWindowState(m_TagsEditor);
            ActivateWindow(m_TagsEditor);
        }


        private VolumeWindow m_VolumeWindow;

        private ErrorWindow m_ErrorWindow;

        private void UpdateWindowState(WeifenLuo.WinFormsUI.Docking.DockContent window)
        {
            if (window.VisibleState == WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide)
            {
                window.VisibleState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            }
            else if (window.VisibleState == WeifenLuo.WinFormsUI.Docking.DockState.DockLeftAutoHide)
            {
                window.VisibleState = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            }
            else if (window.VisibleState == WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide)
            {
                window.VisibleState = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            }
            else if (window.VisibleState == WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide)
            {
                window.VisibleState = WeifenLuo.WinFormsUI.Docking.DockState.DockTop;
            }
            else
            {
                window.IsHidden = !window.IsHidden;
            }
        }

        private void ActivateWindow(WeifenLuo.WinFormsUI.Docking.DockContent window)
        {
            if (!window.IsHidden)
            {
                window.Activate();
                window.Focus();
            }
        }

        private void ShowVolumeWindow()
        {
            if (m_VolumeWindow == null)
            {
                m_VolumeWindow = new VolumeWindow();
                m_VolumeWindow.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
                m_VolumeWindow.Show(dockPanel);
            }
            else UpdateWindowState(m_VolumeWindow);
            ActivateWindow(m_VolumeWindow);
        }

        private void ShowErrorWindow()
        {
            if (m_ErrorWindow == null)
            {
                m_ErrorWindow = new ErrorWindow();
                m_ErrorWindow.Client = this;
                m_ErrorWindow.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
                m_ErrorWindow.Show(dockPanel);
            }
            else UpdateWindowState(m_ErrorWindow);
            ActivateWindow(m_ErrorWindow);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            String oldSoundsDir = settings.SoundDirectory;
            String oldMusicDir = settings.MusicDirectory;
            ShowSettingsDialog(-1);
        }

        private BasicSettings m_BasicSettings;

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!UnloadProject())
            {
                e.Cancel = true;
                return;
            }

            WriteSettings();
        }

        private void WriteSettings()
        {
            try
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    dockPanel.SaveAsXml(stream, System.Text.Encoding.UTF8, true);
                    string layout = System.Text.Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                    Ares.Settings.Settings.Instance.WindowLayout = layout;
                }
                Ares.Settings.Settings.Instance.WriteToFile(m_BasicSettings.GetSettingsDir());
                m_BasicSettings.WriteToFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.WriteSettingsError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFileExplorer(FileType.Music);
        }

        private void audioSourceSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAudioSourceSearch();
        }

        private FileExplorer[] m_FileExplorers = new FileExplorer[2];
        private AudioSourceSearchWindow m_AudioSourceSearch;

        private void ShowAudioSourceSearch()
        {
            if (m_AudioSourceSearch == null)
            {
                m_AudioSourceSearch = new AudioSourceSearchWindow(m_PluginManager);
                m_AudioSourceSearch.SetProject(m_CurrentProject);
                m_AudioSourceSearch.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
                m_AudioSourceSearch.Show(dockPanel);
            }
            else UpdateWindowState(m_AudioSourceSearch);
            ActivateWindow(m_AudioSourceSearch);
        }

        private void ShowFileExplorer(FileType fileType)
        {
            int index = (int)fileType;
            if (m_FileExplorers[index] == null)
            {
                m_FileExplorers[index] = new FileExplorer(fileType, this);
                m_FileExplorers[index].SetProject(m_CurrentProject);
                m_FileExplorers[index].ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
                m_FileExplorers[index].Show(dockPanel);
            }
            else UpdateWindowState(m_FileExplorers[index]);
            ActivateWindow(m_FileExplorers[index]);
        }

        private Ares.Data.IProject m_CurrentProject;

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void DoModelChecks()
        {
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_CurrentProject);
            if (Ares.ModelInfo.ModelChecks.Instance.GetErrorCount() > 0)
            {
                if (m_ErrorWindow == null)
                {
                    ShowErrorWindow();
                }
                else
                {
                    if (m_ErrorWindow.IsHidden)
                    {
                        UpdateWindowState(m_ErrorWindow);
                    }
                    m_ErrorWindow.Refill();
                }
            }
            else if (m_ErrorWindow != null)
            {
                m_ErrorWindow.Refill();
            }
        }

        private void NewProject()
        {
            String title = StringResources.NewProject;
            
            if (!UnloadProject())
                return;
            
            m_CurrentProject = Ares.Data.DataModule.ProjectManager.CreateProject(title);
            try
            {
                m_CurrentProject.TagLanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DoModelChecks();

            Ares.Editor.Actions.FilesWatcher.Instance.Project = m_CurrentProject;
            if (m_ProjectExplorer != null)
            {
                m_ProjectExplorer.SetProject(m_CurrentProject);
            }
            if (m_TagsEditor != null)
            {
                m_TagsEditor.SetProject(m_CurrentProject);
            }
            if (m_FileTagsEditor != null)
            {
                m_FileTagsEditor.SetProject(m_CurrentProject);
            }
            for (int i = 0; i < m_FileExplorers.Length; ++i)
            {
                if (m_FileExplorers[i] != null)
                {
                    m_FileExplorers[i].SetProject(m_CurrentProject);
                }
            }
            if (m_AudioSourceSearch != null)
            {
                m_AudioSourceSearch.SetProject(m_CurrentProject);
            }

            if (m_ProjectExplorer.IsHidden)
                ShowProjectExplorer();
            UpdateGUI();

            m_ProjectExplorer.InitNewProject();

        }

        void UpdateGUI()
        {
            String projectName = m_CurrentProject != null ? m_CurrentProject.Title : StringResources.NoOpenedProject;
            if (projectName == String.Empty)
                projectName = StringResources.NoProjectName;
            String title = String.Format(StringResources.AresEditorTitle, projectName);
            if (Actions.Actions.Instance.IsChanged)
                title += "*";
            this.Text = title;

            undoButton.Enabled = Actions.Actions.Instance.CanUndo && !Actions.Playing.Instance.IsPlaying;
            redoButton.Enabled = Actions.Actions.Instance.CanRedo && !Actions.Playing.Instance.IsPlaying;

            stopButton.Enabled = Actions.Playing.Instance.IsPlaying;
        }

        private bool UnloadProject()
        {
            if (!SaveCurrentProject())
                return false;

            if (m_CurrentProject != null)
            {
                List<WeifenLuo.WinFormsUI.Docking.IDockContent> documents =
                    new List<WeifenLuo.WinFormsUI.Docking.IDockContent>(dockPanel.Documents);
                foreach (WeifenLuo.WinFormsUI.Docking.IDockContent document in documents)
                {
                    if (!(document is ElementEditors.FileTagsEditor) && !(document is ElementEditors.TagsEditor))
                    {
                        (document as Form).Close();
                    }
                }
                Ares.Data.DataModule.ProjectManager.UnloadProject(m_CurrentProject);
                m_CurrentProject = null;
                DoModelChecks();
                Actions.Actions.Instance.Clear();
				#if !MONO
                m_Instance.SetLoadedProject("-");
				#endif
            }
            
            return true;

        }

        private bool SaveCurrentProject()
        {
            if (m_CurrentProject != null && Actions.Actions.Instance.IsChanged)
            {
                DialogResult result = MessageBox.Show(this, StringResources.ProjectChanged, StringResources.Ares, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    return SaveProject();
                }
                else if (result == System.Windows.Forms.DialogResult.No)
                    return true;
                else
                    // cancel
                    return false;
            }
            else
                return true;
        }

        private bool SaveProject()
        {
            if (m_CurrentProject == null)
                return true;
            if (String.IsNullOrEmpty(m_CurrentProject.FileName))
            {
                return SaveProjectAs();
            }
            else
            {
                try
                {
                    Ares.Data.DataModule.ProjectManager.SaveProject(m_CurrentProject);
                    Actions.Actions.Instance.SetCurrentAsNoChange();
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(this, String.Format(StringResources.SaveError, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private bool SaveProjectAs()
        {
            if (m_CurrentProject == null)
                return true;
            if (!String.IsNullOrEmpty(m_CurrentProject.FileName))
            {
                saveFileDialog.FileName = m_CurrentProject.FileName;
            }
            else if (!String.IsNullOrEmpty(m_CurrentProject.Title))
            {
                saveFileDialog.FileName = m_CurrentProject.Title + ".ares";
            }
            DialogResult result = saveFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return false;
            m_CurrentProject.FileName = saveFileDialog.FileName;
            bool result2 = SaveProject();
            if (result2)
            {
                Ares.Settings.Settings.Instance.RecentFiles.AddFile(new RecentFiles.ProjectEntry(m_CurrentProject.FileName, m_CurrentProject.Title));
            }
            return result2;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectAs();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void OpenProject()
        {
            DialogResult result = openFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return;
            if (!OpenProject(openFileDialog.FileName))
            {
                MessageBox.Show(this, StringResources.OpenProjectError, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    if (!OpenProject(projectPath))
                    {
                        MessageBox.Show(this, StringResources.OpenProjectError, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool OpenProject(String filePath)
        {
            if (!UnloadProject())
                return true;

            bool result = true;
            try
            {
                m_CurrentProject = Ares.Data.DataModule.ProjectManager.LoadProject(filePath);
                Ares.Settings.Settings.Instance.RecentFiles.AddFile(new RecentFiles.ProjectEntry(m_CurrentProject.FileName, m_CurrentProject.Title));
            }
            catch (Ares.Data.InvalidProjectException)
            {
                m_CurrentProject = null;
                result = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, String.Format(StringResources.LoadError, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_CurrentProject = null;
            }

            Ares.Editor.Actions.FilesWatcher.Instance.Project = m_CurrentProject;
            if (m_ProjectExplorer != null)
            {
                m_ProjectExplorer.SetProject(m_CurrentProject);
            }
            if (m_TagsEditor != null)
            {
                m_TagsEditor.SetProject(m_CurrentProject);
            }
            if (m_FileTagsEditor != null)
            {
                m_FileTagsEditor.SetProject(m_CurrentProject);
            }
            for (int i = 0; i < m_FileExplorers.Length; ++i)
            {
                if (m_FileExplorers[i] != null)
                {
                    m_FileExplorers[i].SetProject(m_CurrentProject);
                }
            }
            if (m_AudioSourceSearch != null)
            {
                m_AudioSourceSearch.SetProject(m_CurrentProject);
            }

            DoModelChecks();
            Ares.ModelInfo.ModelChecks.Instance.AdaptHiddenTags(m_CurrentProject);
			#if !MONO
            m_Instance.SetLoadedProject(filePath);
			#endif
            UpdateGUI();
            return result;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Actions.Actions.Instance.Undo(m_CurrentProject);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Actions.Actions.Instance.Redo(m_CurrentProject);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editMenu_DropDownOpening(object sender, EventArgs e)
        {
            ContextMenuStrip editContext = m_ProjectExplorer != null ? m_ProjectExplorer.EditContextMenu : null;
            if (editContext != null && editContext.Items.Count > 0)
            {
                editMenu.DropDownItems.Add(new ToolStripSeparator());
                ToolStripItem[] items = new ToolStripItem[editContext.Items.Count];
                editContext.Items.CopyTo(items, 0);
                foreach (ToolStripItem item in items)
                {
                    editMenu.DropDownItems.Add(item);
                }
            }
            undoToolStripMenuItem.Enabled = Actions.Actions.Instance.CanUndo && !Actions.Playing.Instance.IsPlaying;
            redoToolStripMenuItem.Enabled = Actions.Actions.Instance.CanRedo && !Actions.Playing.Instance.IsPlaying;
        }

        private void editMenu_DropDownClosed(object sender, EventArgs e)
        {
            const int cNrOfStaticItems = 2;
            if (editMenu.DropDownItems.Count > cNrOfStaticItems)
            {
                ContextMenuStrip editContext = m_ProjectExplorer.EditContextMenu;
                int count = editMenu.DropDownItems.Count;
                for (int i = cNrOfStaticItems; i < count; ++i)
                {
                    if (i > cNrOfStaticItems)
                        editContext.Items.Add(editMenu.DropDownItems[cNrOfStaticItems]);
                    else
                        editMenu.DropDownItems.RemoveAt(cNrOfStaticItems); // Separator
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopAll();
        }

        private void extrasToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            settingsToolStripMenuItem.Enabled = !Actions.Playing.Instance.IsPlaying;
            stopAllToolStipMenuItem.Enabled = Actions.Playing.Instance.IsPlaying;
        }

        private void viewToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            projectExplorerToolStripMenuItem.Checked = m_ProjectExplorer != null && !m_ProjectExplorer.IsHidden;
            fileExplorerToolStripMenuItem.Checked =  m_FileExplorers[0] != null && !m_FileExplorers[0].IsHidden;
            soundFileExplorerToolStripMenuItem.Checked = m_FileExplorers[1] != null && !m_FileExplorers[1].IsHidden;
            audioSourceSearchToolStripMenuItem.Checked = m_AudioSourceSearch != null && !m_AudioSourceSearch.IsHidden;
            volumesToolStripMenuItem.Checked = m_VolumeWindow != null && !m_VolumeWindow.IsHidden;
            projectErrorsToolStripMenuItem.Checked = m_ErrorWindow != null && !m_ErrorWindow.IsHidden;
            tagsMenuItem.Checked = m_TagsEditor != null && !m_TagsEditor.IsHidden;
        }

        private void recentMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            recentMenuItem.DropDownItems.Clear();
            foreach (RecentFiles.ProjectEntry projectEntry in Ares.Settings.Settings.Instance.RecentFiles.GetFiles())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(projectEntry.ProjectName);
                item.ToolTipText = projectEntry.FilePath;
                item.Tag = projectEntry;
                item.Click += new EventHandler(recentItem_Click);
                
                recentMenuItem.DropDownItems.Add(item);
            }
        }

        void recentItem_Click(object sender, EventArgs e)
        {
            if (!OpenProject(((sender as ToolStripMenuItem).Tag as RecentFiles.ProjectEntry).FilePath))
            {
                MessageBox.Show(this, StringResources.OpenProjectError, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            try
            {
                Actions.Actions.Instance.Undo(m_CurrentProject);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            try
            {
                Actions.Actions.Instance.Redo(m_CurrentProject);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopAll();
        }

        private void startPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartPlayer();
        }

        private void StartPlayer()
        {
            if (!SaveCurrentProject())
                return;
            String appDir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
			#if !MONO
            String commandLine = System.IO.Path.Combine(appDir, "Ares.Player.exe");
            try
            {
                Ares.Ipc.ApplicationInstance.CreateOrActivate("Ares.Player", commandLine,
                    m_CurrentProject != null ? m_CurrentProject.Title : "", m_CurrentProject != null ? m_CurrentProject.FileName : "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.PlayerStartError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
			#else
			// TODO
			#endif
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.CommonGUI.AboutDialog dialog = new Ares.CommonGUI.AboutDialog();
            dialog.ShowDialog(this);
        }

        private void volumesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowVolumeWindow();
        }

        private String m_ProjectName;

        private void MainForm_Load(object sender, EventArgs e)
        {
            m_BasicSettings = new BasicSettings();
            bool foundSettings = m_BasicSettings.ReadFromFile();
            bool hasSettings = Ares.Settings.Settings.Instance.Initialize(Ares.Settings.Settings.EditorID, foundSettings ? m_BasicSettings.GetSettingsDir() : null);
            if (!hasSettings)
            {
                MessageBox.Show(this, StringResources.NoSettings, StringResources.Ares);
                ShowSettingsDialog(0);
                LoadTagsDB();
                ShowVolumeWindow();
                ShowProjectExplorer();
                ShowFileExplorer(FileType.Music);
                ShowFileExplorer(FileType.Sound);
                ShowAudioSourceSearch();
            }
            else if (!String.IsNullOrEmpty(Ares.Settings.Settings.Instance.WindowLayout))
            {
                LoadTagsDB();
                System.IO.MemoryStream stream = new System.IO.MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(Ares.Settings.Settings.Instance.WindowLayout));
                dockPanel.LoadFromXml(stream, new WeifenLuo.WinFormsUI.Docking.DeserializeDockContent(DeserializeDockContent));
                if (Settings.Settings.Instance.Version < 1)
                {
                    ShowVolumeWindow();
                }
            }
            else
            {
                LoadTagsDB();
                ShowProjectExplorer();
                ShowVolumeWindow();
                ShowFileExplorer(FileType.Music);
                ShowFileExplorer(FileType.Sound);
                ShowAudioSourceSearch();
            }
            if (m_FileExplorers[0] == null)
            {
                ShowFileExplorer(FileType.Music);
            }
            if (m_FileExplorers[1] == null)
            {
                ShowFileExplorer(FileType.Sound);
            }
            if (m_AudioSourceSearch == null)
            {
                ShowAudioSourceSearch();
            }
            if (m_ProjectExplorer == null)
            {
                ShowProjectExplorer();
            }
            if (m_VolumeWindow == null)
            {
                ShowVolumeWindow();
            }
			#if !MONO
            Settings.Settings.Instance.SettingsChanged += new EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
			#endif
            fileExplorerToolStripMenuItem.Checked = !m_FileExplorers[0].IsHidden;
            soundFileExplorerToolStripMenuItem.Checked = !m_FileExplorers[1].IsHidden;
            audioSourceSearchToolStripMenuItem.Checked = !m_AudioSourceSearch.IsHidden;
            projectExplorerToolStripMenuItem.Checked = !m_ProjectExplorer.IsHidden;
            volumesToolStripMenuItem.Checked = m_VolumeWindow != null && !m_VolumeWindow.IsHidden;
            projectErrorsToolStripMenuItem.Checked = m_ErrorWindow != null && !m_ErrorWindow.IsHidden;
            tagsMenuItem.Checked = m_VolumeWindow != null && !m_VolumeWindow.IsHidden;
            Actions.Actions.Instance.UpdateGUI = UpdateGUI;
            Actions.Playing.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
            Actions.FilesWatcher.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
            Actions.Playing.Instance.ErrorHandling = new Actions.Playing.ErrorHandler(this, HandlePlayingError);
            UpdateGUI();
            if (!String.IsNullOrEmpty(m_ProjectName))
            {
                if (m_ProjectName.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
                {
                    ImportProject(m_ProjectName);
                }
                else
                {
                    OpenProject(m_ProjectName);
                }
            }
            else if (Ares.Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
            {
                OpenProject(Ares.Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath);
            }
            if (Settings.Settings.Instance.CheckForUpdate)
            {
                Ares.Online.OnlineOperations.CheckForUpdate(this, false);
                Ares.Online.OnlineOperations.CheckForNews(this, false);
            }
            if (Settings.Settings.Instance.ShowTipOfTheDay)
            {
                ShowTipsOfTheDay();
            }
        }

        private void ShowTipsOfTheDay()
        {
            List<String> tips = TipsOfTheDay.GetTipsOfTheDay();
            int lastTip = Settings.Settings.Instance.LastTipOfTheDay;
            if (tips != null && tips.Count > 0)
            {
                ++lastTip;
                if (lastTip < 0 || lastTip >= tips.Count)
                    lastTip = 0;
                Dialogs.TipOfDayDialog dialog = new Dialogs.TipOfDayDialog();
                dialog.SetTips(tips, lastTip);
                dialog.ShowDialog();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (Actions.Playing.Instance.IsPlaying)
                {
                    Actions.Playing.Instance.StopAll();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.F6)
            {
                ShowProjectExplorer();
            }
            else if (e.KeyCode == Keys.F7)
            {
                ShowFileExplorer(FileType.Music);
            }
            else if (e.KeyCode == Keys.F8)
            {
                ShowFileExplorer(FileType.Sound);
            }
            else if (e.KeyCode == Keys.F9)
            {
                ShowVolumeWindow();
            }
            else if (e.KeyCode == Keys.F10)
            {
                ShowErrorWindow();
            }
            else if (e.KeyCode == Keys.F11)
            {
                ShowAudioSourceSearch();
            }
        }

        private void projectErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowErrorWindow();
        }


        public void MoveToElement(object element)
        {
            if (m_ProjectExplorer == null || m_ProjectExplorer.IsHidden)
            {
                ShowProjectExplorer();
            }
            m_ProjectExplorer.MoveToElement(element);
            ActivateWindow(m_ProjectExplorer);
        }

        private void helpOnlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.Online.OnlineOperations.ShowHelppage(StringResources.HelpPage, this);
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.Online.OnlineOperations.CheckForUpdate(this, true);
        }

        private void soundFileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFileExplorer(FileType.Sound);
        }

        private void usedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelInfo.FileLists lists = new ModelInfo.FileLists();
            try
            {
                String tempFileName = System.IO.Path.GetTempFileName() + ".txt";
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(tempFileName))
                {
                    foreach (String path in lists.GetAllFilePaths(m_CurrentProject).OrderBy(x => x))
                    {
                        writer.WriteLine(path);
                    }
                    writer.Flush();
                }
                System.Diagnostics.Process.Start(tempFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.WriteFileListError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void usedKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                KeysConverter converter = new KeysConverter();
                String tempFileName = System.IO.Path.GetTempFileName() + ".txt";
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(tempFileName))
                {
                    foreach (Data.IMode mode in m_CurrentProject.GetModes())
                    {
                        String modeKey = mode.KeyCode != 0 ? converter.ConvertToString((System.Windows.Forms.Keys)mode.KeyCode) : StringResources.NoKey;
                        String line = modeKey + " - " + mode.Title;
                        writer.WriteLine(line);
                        writer.WriteLine(new String('=', line.Length));
                        foreach (Data.IModeElement element in mode.GetElements())
                        {
                            String keyString = element.Trigger != null && element.Trigger.TriggerType == Data.TriggerType.Key ?
                                converter.ConvertToString((System.Windows.Forms.Keys)((Data.IKeyTrigger)element.Trigger).KeyCode) : StringResources.NoKey;
                            writer.WriteLine(keyString + " - " + element.Title);
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine(StringResources.General);
                    writer.WriteLine();
                    writer.Flush();
                }
                System.Diagnostics.Process.Start(tempFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.WriteFileListError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_CurrentProject == null)
                return;
            if (m_CurrentProject.Changed && !SaveProject())
                return;

            String exportFileName = m_CurrentProject.FileName;
            if (exportFileName.EndsWith(".ares", StringComparison.InvariantCultureIgnoreCase))
            {
                exportFileName = exportFileName.Substring(0, exportFileName.Length - 5);
            }
            exportFileName = exportFileName + ".apkg";
            exportFileDialog.FileName = exportFileName;

            DialogResult result = exportFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return;

            Ares.CommonGUI.ProgressMonitor monitor = new Ares.CommonGUI.ProgressMonitor(this, StringResources.Exporting);
            Ares.ModelInfo.Exporter.Export(monitor, m_CurrentProject, m_CurrentProject.FileName, exportFileDialog.FileName, error =>
            {
                monitor.Close();
                if (error != null)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(StringResources.ExportError, error.Message), StringResources.Ares,
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            });
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = importFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
                return;
            if (!UnloadProject())
                return;
            ImportProject(importFileDialog.FileName);
        }

        private class MessageBoxProvider : Ares.ModelInfo.IMessageBoxProvider
        {
            public Ares.ModelInfo.MessageBoxResult ShowYesNoCancelBox(string prompt)
            {
                switch (MessageBox.Show(prompt, StringResources.Ares, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        return Ares.ModelInfo.MessageBoxResult.Yes;
                    case DialogResult.No:
                        return Ares.ModelInfo.MessageBoxResult.No;
                    default:
                        return Ares.ModelInfo.MessageBoxResult.Cancel;
                }
            }
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

            Ares.Editor.Actions.FilesWatcher.Instance.Enabled = false;
            Ares.CommonGUI.ProgressMonitor monitor = new Ares.CommonGUI.ProgressMonitor(this, StringResources.Importing);
            Ares.ModelInfo.Importer.Import(monitor, fileName, saveFileDialog.FileName, false, new MessageBoxProvider(), (error, cancelled) => 
            {
                monitor.Close();
                if (error != null)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(StringResources.ImportError, error.Message), StringResources.Ares,
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                else if (!cancelled)
                {
                    if (!OpenProject(saveFileDialog.FileName))
                    {
                        MessageBox.Show(this, StringResources.ImportNoProject, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });
            Ares.Editor.Actions.FilesWatcher.Instance.Enabled = true;
        }

        public void SetEditor()
        {
            ShowSettingsDialog(1);
        }

        private ElementEditors.FileTagsEditor m_FileTagsEditor;

        public void ShowFileTagsEditor(IList<String> selectedFiles)
        {
            if (m_FileTagsEditor == null)
            {
                m_FileTagsEditor = new ElementEditors.FileTagsEditor();
                m_FileTagsEditor.SetParent(this);
                m_FileTagsEditor.SetProject(m_CurrentProject);
                m_FileTagsEditor.SetFiles(selectedFiles);
                m_FileTagsEditor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                m_FileTagsEditor.Show(dockPanel);
            }
            else UpdateWindowState(m_FileTagsEditor);
            ActivateWindow(m_FileTagsEditor);
        }

        public void SetSelectedFiles(IList<String> selectedFiles)
        {
            m_SelectedFiles = selectedFiles;
            if (m_FileTagsEditor != null)
            {
                m_FileTagsEditor.SetFiles(selectedFiles);
            }
        }

        private IList<String> m_SelectedFiles;

        public void SetOnlineUserId()
        {
            ShowSettingsDialog(2);
        }

        private void tagsMenuItem_Click(object sender, EventArgs e)
        {
            ShowTagsEditor();
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            Ares.Online.OnlineOperations.CheckForNews(this, true);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Ares.Online.OnlineOperations.ShowHelppage(StringResources.ForumPage, this);
        }

        private void showTipsMenuItem_Click(object sender, EventArgs e)
        {
            ShowTipsOfTheDay();
        }
    }

    public static class ControlHelpers
    {
        public static void Invoke(this Control control, Action action)
        {
            control.Invoke(action);
        }
    }

}
