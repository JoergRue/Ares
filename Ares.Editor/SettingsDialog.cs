using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor
{
    partial class SettingsDialog : Form
    {
        public SettingsDialog(Settings settings)
        {
            InitializeComponent();
            musicDirLabel.Text = settings.MusicDirectory;
            soundDirLabel.Text = settings.SoundDirectory;
            if (AppSettings.Default.SettingsLocation == 0)
            {
                userDirButton.Checked = true;
            }
            else if (AppSettings.Default.SettingsLocation == 1)
            {
                appDirButton.Checked = true;
            }
            else
            {
                otherDirButton.Checked = true;
            }
            otherDirLabel.Text = AppSettings.Default.CustomSettingsDirectory;
            userDirLabel.Text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ares");
            appDirLabel.Text = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            m_Settings = settings;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_Settings.MusicDirectory = musicDirLabel.Text;
            m_Settings.SoundDirectory = soundDirLabel.Text;
            if (userDirButton.Checked)
            {
                AppSettings.Default.SettingsLocation = 0;
            }
            else if (appDirButton.Checked)
            {
                AppSettings.Default.SettingsLocation = 1;
            }
            else
            {
                AppSettings.Default.SettingsLocation = 2;
            }
            AppSettings.Default.CustomSettingsDirectory = otherDirLabel.Text;
        }

        private Settings m_Settings { get; set; }

        private void selectMusicButton_Click(object sender, EventArgs e)
        {
            SelectDirectory(musicDirLabel);
        }

        private void SelectDirectory(Label label)
        {
            folderBrowserDialog.SelectedPath = label.Text;
            if (folderBrowserDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                label.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void selectSoundButton_Click(object sender, EventArgs e)
        {
            SelectDirectory(soundDirLabel);
        }

        private void selectOtherDirButton_Click(object sender, EventArgs e)
        {
            SelectDirectory(otherDirLabel);
        }
    }
}
