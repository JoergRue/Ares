using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();

            SuspendLayout();

            ResumeLayout();

            Timer t = new Timer();
            t.Interval = 300;
            t.Tick += new EventHandler((o, args) =>
                {
                    t.Stop();
                    m_BasicSettings = new BasicSettings();
                    if (!m_BasicSettings.ReadFromFile() || !Settings.Instance.ReadFromFile(m_BasicSettings.GetSettingsDir()))
                    {
                        MessageBox.Show(this, StringResources.NoSettings, StringResources.Ares);
                        ShowSettingsDialog();
                        ShowProjectExplorer();
                        ShowFileExplorer();
                    }
                    else if (!String.IsNullOrEmpty(Settings.Instance.WindowLayout))
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream(
                            System.Text.Encoding.UTF8.GetBytes(Settings.Instance.WindowLayout));
                        dockPanel.LoadFromXml(stream, new WeifenLuo.WinFormsUI.Docking.DeserializeDockContent(DeserializeDockContent));
                    }
                    else
                    {
                        ShowProjectExplorer();
                        ShowFileExplorer();
                    }
                    fileExplorerToolStripMenuItem.Checked = !m_FileExplorer.IsHidden;
                    projectExplorerToolStripMenuItem.Checked = !m_ProjectExplorer.IsHidden;
                    Actions.Actions.Instance.UpdateGUI = UpdateGUI;
                    UpdateGUI();
                });
            t.Start();
        }

        private WeifenLuo.WinFormsUI.Docking.IDockContent DeserializeDockContent(string persistString)
        {
            if (persistString == "FileExplorer")
            {
                m_FileExplorer = new FileExplorer();
                return m_FileExplorer;
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
            SettingsDialog dialog = new SettingsDialog(Settings.Instance, m_BasicSettings);
            dialog.ShowDialog(this);
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
            projectExplorerToolStripMenuItem.Checked = !m_ProjectExplorer.IsHidden;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = Settings.Instance;
            String oldSoundsDir = settings.SoundDirectory;
            String oldMusicDir = settings.MusicDirectory;
            ShowSettingsDialog();
            if (oldSoundsDir != settings.SoundDirectory || oldMusicDir != settings.MusicDirectory)
            {
                if (m_FileExplorer != null)
                {
                    m_FileExplorer.ReFillTree();
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

            try
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    dockPanel.SaveAsXml(stream, System.Text.Encoding.UTF8, true);
                    string layout = System.Text.Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                    Settings.Instance.WindowLayout = layout;
                }
                Settings.Instance.WriteToFile(m_BasicSettings.GetSettingsDir());
                m_BasicSettings.WriteToFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.WriteSettingsError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFileExplorer();
        }

        private FileExplorer m_FileExplorer;

        private void ShowFileExplorer()
        {
            if (m_FileExplorer == null)
            {
                m_FileExplorer = new FileExplorer();
                m_FileExplorer.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
                m_FileExplorer.Show(dockPanel);
            }
            else
            {
                m_FileExplorer.IsHidden = !m_FileExplorer.IsHidden;
            }
            fileExplorerToolStripMenuItem.Checked = !m_FileExplorer.IsHidden;
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

            m_ProjectExplorer.RenameProject();

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

            undoToolStripMenuItem.Enabled = Actions.Actions.Instance.CanUndo;
            redoToolStripMenuItem.Enabled = Actions.Actions.Instance.CanRedo;
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
            return SaveProject();
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

            if (!UnloadProject())
                return;

            try
            {
                m_CurrentProject = Ares.Data.DataModule.ProjectManager.LoadProject(openFileDialog.FileName);
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

    }

    public static class ControlHelpers
    {
        public static void Invoke(this Control control, Action action)
        {
            control.Invoke(action);
        }
    }

}
