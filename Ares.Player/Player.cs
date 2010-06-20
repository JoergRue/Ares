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
    public partial class Player : Form
    {
        public Player()
        {
            InitializeComponent();
            Timer t = new Timer();
            t.Interval = 150;
            t.Tick += new EventHandler((o, args) =>
            {
                t.Stop();
                m_BasicSettings = new BasicSettings();
                ReadSettings();
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
            });
            t.Start();
            Timer updateTimer = new Timer();
            updateTimer.Interval = 50;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Start();
            fileSystemWatcher1.Changed += new System.IO.FileSystemEventHandler(fileSystemWatcher1_Changed);
        }

        void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (m_Project == null)
                return;
            String path = m_Project.FileName;
            if (e.FullPath == path)
            {
                PlayingModule.ProjectPlayer.StopAll();
                OpenProject(path);
            }
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
                PlayingControl.Instance.UpdateDirectories();
                WriteSettings();
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

        private void OpenProject(String filePath)
        {
            if (m_Project != null)
            {
                Ares.Data.DataModule.ProjectManager.UnloadProject(m_Project);
                m_Project = null;
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
            fileSystemWatcher1.Path = m_Project != null ? System.IO.Path.GetDirectoryName(m_Project.FileName) : String.Empty;
        }

        private void Player_KeyDown(object sender, KeyEventArgs e)
        {
            PlayingControl.Instance.KeyReceived(e.KeyCode);
            e.Handled = true;
        }

        private int lastMusicElementId = -1;

        private void UpdateStatus()
        {
            PlayingControl control = PlayingControl.Instance;
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
                if (musicElementId != -1)
                {
                    IElement musicElement = Ares.Data.DataModule.ElementRepository.GetElement(musicElementId);
                    IFileElement fileElement = musicElement as IFileElement;
                    if (fileElement != null)
                    {
                        String path = Settings.Settings.Instance.MusicDirectory;
                        path = System.IO.Path.Combine(path, fileElement.FilePath);
                        Un4seen.Bass.AddOn.Tags.TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(path, true, false);
                        if (tag != null)
                        {
                            StringBuilder musicInfoBuilder = new StringBuilder();
                            musicInfoBuilder.Append(tag.artist);
                            if (musicInfoBuilder.Length > 0)
                                musicInfoBuilder.Append(" - ");
                            musicInfoBuilder.Append(tag.album);
                            if (musicInfoBuilder.Length > 0)
                                musicInfoBuilder.Append(" - ");
                            musicInfoBuilder.Append(tag.title);
                            musicLabel.Text = musicInfoBuilder.ToString();
                        }
                        else
                        {
                            musicLabel.Text = musicElement.Title;
                        }
                    }
                    else
                    {
                        musicLabel.Text = musicElement.Title;
                    }
                }
                else
                {
                    musicLabel.Text = String.Empty;
                }
                lastMusicElementId = musicElementId;
            }
            overallVolumeBar.Value = control.GlobalVolume;
            musicVolumeBar.Value = control.MusicVolume;
            soundVolumeBar.Value = control.SoundVolume;
        }

        private BasicSettings m_BasicSettings;
        private IProject m_Project;

        private void overallVolumeBar_Scroll(object sender, EventArgs e)
        {
            PlayingControl.Instance.GlobalVolume = overallVolumeBar.Value;
        }

        private void musicVolumeBar_Scroll(object sender, EventArgs e)
        {
            PlayingControl.Instance.MusicVolume = musicVolumeBar.Value;
        }

        private void soundVolumeBar_Scroll(object sender, EventArgs e)
        {
            PlayingControl.Instance.SoundVolume = soundVolumeBar.Value;
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            Settings.AboutDialog dialog = new AboutDialog();
            dialog.ShowDialog(this);
        }

        private void Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteSettings();
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
            if (!m_BasicSettings.ReadFromFile() || !Ares.Settings.Settings.Instance.ReadFromFile(m_BasicSettings.GetSettingsDir()))
            {
                MessageBox.Show(this, StringResources.NoSettings, StringResources.Ares);
                ShowSettingsDialog();
            }
            else
            {
                PlayingControl.Instance.UpdateDirectories();
            }
            settingsFileWatcher.Path = m_BasicSettings.GetSettingsDir();
        }

        private void settingsFileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            String path = System.IO.Path.Combine(m_BasicSettings.GetSettingsDir(), Ares.Settings.Settings.Instance.settingsFileName);
            if (path == e.FullPath)
            {
                PlayingModule.ProjectPlayer.StopAll();
                ReadSettings();
            }
        }
    }
}
