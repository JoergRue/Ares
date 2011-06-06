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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Settings;

namespace Ares.Editor
{
    public partial class MainForm : Form, ErrorWindow.IErrorWindowClient
    {
        private Ares.Ipc.ApplicationInstance m_Instance;

        public MainForm()
        {
            String projectName = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1] : String.Empty;
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
            InitializeComponent();
            m_Instance.SetWindowHandle(Handle);
            m_Instance.ProjectOpenAction = (projectName2, projectPath) => OpenProjectFromRequest(projectName2, projectPath);
            m_ProjectName = projectName;
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
        }

        private void Shutdown()
        {
            Actions.FilesWatcher.Instance.DeInit();
            Settings.Settings.Instance.SettingsChanged -= new EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
            Settings.Settings.Instance.Shutdown();
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
                m_FileExplorers[index] = new FileExplorer((FileType)index);
                return m_FileExplorers[index];
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
            else
            {
                return null;
            }
        }


        private void ShowSettingsDialog()
        {
            SettingsDialog dialog = new SettingsDialog(Ares.Settings.Settings.Instance, m_BasicSettings);
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Actions.Playing.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
                Actions.FilesWatcher.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
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
                m_ProjectExplorer.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                m_ProjectExplorer.Show(dockPanel);
            }
            else UpdateWindowState(m_ProjectExplorer);
            ActivateWindow(m_ProjectExplorer);
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
            ShowSettingsDialog();
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

        private FileExplorer[] m_FileExplorers = new FileExplorer[2];

        private void ShowFileExplorer(FileType fileType)
        {
            int index = (int)fileType;
            if (m_FileExplorers[index] == null)
            {
                m_FileExplorers[index] = new FileExplorer(fileType);
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
            Ares.ModelInfo.ModelChecks.Instance.Project = m_CurrentProject;
            Ares.ModelInfo.ModelChecks.Instance.CheckAll();
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
            DoModelChecks();

            m_ProjectExplorer.SetProject(m_CurrentProject);

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
                    (document as Form).Close();
                }
                Ares.Data.DataModule.ProjectManager.UnloadProject(m_CurrentProject);
                m_CurrentProject = null;
                DoModelChecks();
                Actions.Actions.Instance.Clear();
                m_Instance.SetLoadedProject("-");
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
            OpenProject(openFileDialog.FileName);
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
            if (!UnloadProject())
                return;

            try
            {
                m_CurrentProject = Ares.Data.DataModule.ProjectManager.LoadProject(filePath);
                Ares.Settings.Settings.Instance.RecentFiles.AddFile(new RecentFiles.ProjectEntry(m_CurrentProject.FileName, m_CurrentProject.Title));
            }
            catch (Exception e)
            {
                MessageBox.Show(this, String.Format(StringResources.LoadError, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_CurrentProject = null;
            }

            m_ProjectExplorer.SetProject(m_CurrentProject);
            DoModelChecks();
            m_Instance.SetLoadedProject(filePath);
            UpdateGUI();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Actions.Actions.Instance.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Actions.Actions.Instance.Redo();
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
            volumesToolStripMenuItem.Checked = m_VolumeWindow != null && !m_VolumeWindow.IsHidden;
            projectErrorsToolStripMenuItem.Checked = m_ErrorWindow != null && !m_ErrorWindow.IsHidden;
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
            OpenProject(((sender as ToolStripMenuItem).Tag as RecentFiles.ProjectEntry).FilePath);
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
            Actions.Actions.Instance.Undo();
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            Actions.Actions.Instance.Redo();
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
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.Settings.AboutDialog dialog = new Ares.Settings.AboutDialog();
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
                ShowSettingsDialog();
                ShowVolumeWindow();
                ShowProjectExplorer();
                ShowFileExplorer(FileType.Music);
                ShowFileExplorer(FileType.Sound);
            }
            else if (!String.IsNullOrEmpty(Ares.Settings.Settings.Instance.WindowLayout))
            {
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
                ShowProjectExplorer();
                ShowVolumeWindow();
                ShowFileExplorer(FileType.Music);
                ShowFileExplorer(FileType.Sound);
            }
            Settings.Settings.Instance.SettingsChanged += new EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
            fileExplorerToolStripMenuItem.Checked = !m_FileExplorers[0].IsHidden;
            soundFileExplorerToolStripMenuItem.Checked = !m_FileExplorers[1].IsHidden;
            projectExplorerToolStripMenuItem.Checked = !m_ProjectExplorer.IsHidden;
            volumesToolStripMenuItem.Checked = m_VolumeWindow != null && !m_VolumeWindow.IsHidden;
            projectErrorsToolStripMenuItem.Checked = m_ErrorWindow != null && !m_ErrorWindow.IsHidden;
            Actions.Actions.Instance.UpdateGUI = UpdateGUI;
            Actions.Playing.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
            Actions.FilesWatcher.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
            Actions.Playing.Instance.ErrorHandling = new Actions.Playing.ErrorHandler(this, HandlePlayingError);
            UpdateGUI();
            if (!String.IsNullOrEmpty(m_ProjectName))
            {
                OpenProject(m_ProjectName);
            }
            else if (Ares.Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
            {
                OpenProject(Ares.Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath);
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
            try
            {
                System.Diagnostics.Process.Start(StringResources.HelpPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.OpenHelpError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
