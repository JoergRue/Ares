using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor
{
    public static class ControlHelpers
    {
        public static void Invoke(this Control control, Action action)
        {
            control.Invoke(action);
        }
    }

    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();

            SuspendLayout();

            ShowProjectExplorer();

            ResumeLayout();

            Timer t = new Timer();
            t.Interval = 300;
            t.Tick += new EventHandler((o, args) =>
                {
                    t.Stop();
                    m_Settings = new Settings();
                    if (!m_Settings.ReadFromFile(GetSettingsDir()))
                    {
                        MessageBox.Show(this, StringResources.NoSettings, StringResources.Ares);
                        ShowSettingsDialog();
                    }
                });
            t.Start();
        }

        private static String GetSettingsDir()
        {
            String settingsDir = String.Empty;
            if (AppSettings.Default.SettingsLocation == 0)
            {
                settingsDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ares");
            }
            else if (AppSettings.Default.SettingsLocation == 1)
            {
                settingsDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            }
            else
            {
                settingsDir = AppSettings.Default.CustomSettingsDirectory;
            }
            return settingsDir;
        }

        private void ShowSettingsDialog()
        {
            SettingsDialog dialog = new SettingsDialog(m_Settings);
            dialog.ShowDialog(this);
        }

        private void projectExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowProjectExplorer();
        }

        private ProjectExplorer projectExplorer;

        private void ShowProjectExplorer()
        {
            if (projectExplorer == null)
            {
                projectExplorer = new ProjectExplorer();
                projectExplorer.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                projectExplorer.Show(dockPanel);
            }
            else
            {
                projectExplorer.Close();
                projectExplorer = null;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private Settings m_Settings;

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_Settings.WriteToFile(GetSettingsDir());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(StringResources.WriteSettingsError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
