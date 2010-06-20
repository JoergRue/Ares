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
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
            Timer t = new Timer();
            t.Interval = 150;
            t.Tick += new EventHandler((o, args) =>
                {
                    t.Stop();
                    m_BasicSettings = new BasicSettings();
                    if (!m_BasicSettings.ReadFromFile() || !Ares.Settings.Settings.Instance.ReadFromFile(m_BasicSettings.GetSettingsDir()))
                    {
                        MessageBox.Show(this, StringResources.NoSettings, StringResources.Ares);
                        ShowSettingsDialog();
                        ShowProjectExplorer();
                        ShowFileExplorer(FileType.Music);
                        ShowFileExplorer(FileType.Sound);
                    }
                    else if (!String.IsNullOrEmpty(Ares.Settings.Settings.Instance.WindowLayout))
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream(
                            System.Text.Encoding.UTF8.GetBytes(Ares.Settings.Settings.Instance.WindowLayout));
                        dockPanel.LoadFromXml(stream, new WeifenLuo.WinFormsUI.Docking.DeserializeDockContent(DeserializeDockContent));
                    }
                    else
                    {
                        ShowProjectExplorer();
                        ShowFileExplorer(FileType.Music);
                        ShowFileExplorer(FileType.Sound);
                    }
                    fileExplorerToolStripMenuItem.Checked = !m_FileExplorers[0].IsHidden;
                    soundFileExplorerToolStripMenuItem.Checked = !m_FileExplorers[1].IsHidden;
                    projectExplorerToolStripMenuItem.Checked = !m_ProjectExplorer.IsHidden;
                    Actions.Actions.Instance.UpdateGUI = UpdateGUI;
                    Actions.Playing.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
                    UpdateGUI();
                    fileSystemWatcher1.Path = m_BasicSettings.GetSettingsDir();
                    if (Ares.Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
                    {
                        OpenProject(Ares.Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath);
                    }
                });
            t.Start();
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
            else
            {
                m_ProjectExplorer.IsHidden = !m_ProjectExplorer.IsHidden;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            String oldSoundsDir = settings.SoundDirectory;
            String oldMusicDir = settings.MusicDirectory;
            ShowSettingsDialog();
            if (oldSoundsDir != settings.SoundDirectory || oldMusicDir != settings.MusicDirectory)
            {
                if (m_FileExplorers[0] != null)
                {
                    m_FileExplorers[0].ReFillTree();
                }
                if (m_FileExplorers[1] != null)
                {
                    m_FileExplorers[1].ReFillTree();
                }
            }
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

        private void ReadSettings()
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            String oldSoundsDir = settings.SoundDirectory;
            String oldMusicDir = settings.MusicDirectory;
            if (!m_BasicSettings.ReadFromFile() || !settings.ReadFromFile(m_BasicSettings.GetSettingsDir()))
            {
                MessageBox.Show(this, StringResources.NoSettings, StringResources.Ares);
                ShowSettingsDialog();
            }
            else
            {
                Actions.Playing.Instance.SetDirectories(Ares.Settings.Settings.Instance.MusicDirectory, Ares.Settings.Settings.Instance.SoundDirectory);
            }
            if (oldSoundsDir != settings.SoundDirectory || oldMusicDir != settings.MusicDirectory)
            {
                if (m_FileExplorers[0] != null)
                {
                    m_FileExplorers[0].ReFillTree();
                }
                if (m_FileExplorers[1] != null)
                {
                    m_FileExplorers[1].ReFillTree();
                }
            }
            fileSystemWatcher1.Path = m_BasicSettings.GetSettingsDir();
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
            else
            {
                m_FileExplorers[index].IsHidden = !m_FileExplorers[index].IsHidden;
            }
        }

        private Ares.Data.IProject m_CurrentProject;

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void NewProject()
        {
            String title = StringResources.NewProject;
            
            if (!UnloadProject())
                return;
            
            m_CurrentProject = Ares.Data.DataModule.ProjectManager.CreateProject(title);

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
                Actions.Actions.Instance.Clear();
            }
            
            return true;

        }

        private bool SaveCurrentProject()
        {
            if (m_CurrentProject != null && Actions.Actions.Instance.IsChanged)
            {
                DialogResult result = MessageBox.Show(this, StringResources.ProjectChanged, StringResources.Ares, MessageBoxButtons.YesNoCancel);
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
            String commandLine = "Ares.Player.exe";
            String arguments = m_CurrentProject != null ? "\"" + m_CurrentProject.FileName + "\"" : String.Empty;
            try
            {
                System.Diagnostics.Process.Start(commandLine, arguments);
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

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            String path = System.IO.Path.Combine(m_BasicSettings.GetSettingsDir(), Ares.Settings.Settings.Instance.settingsFileName);
            if (path == e.FullPath)
            {
                Actions.Playing.Instance.StopAll();
                ReadSettings();
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
