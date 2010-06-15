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
        public SettingsDialog(Settings settings, BasicSettings basicSettings)
        {
            InitializeComponent();
            musicDirLabel.Text = settings.MusicDirectory;
            soundDirLabel.Text = settings.SoundDirectory;
            if (basicSettings.UserSettingsLocation == BasicSettings.SettingsLocation.Custom)
            {
                otherDirButton.Checked = true;
            }
            else if (basicSettings.UserSettingsLocation == BasicSettings.SettingsLocation.AppDir && BasicSettings.IsAppDirAllowed())
            {
                appDirButton.Checked = true;
            }
            else
            {
                userDirButton.Checked = true;
            }
            otherDirLabel.Text = basicSettings.CustomSettingsDirectory;
            userDirLabel.Text = basicSettings.GetSettingsDir(BasicSettings.SettingsLocation.AppDataDir);
            appDirLabel.Text = basicSettings.GetSettingsDir(BasicSettings.SettingsLocation.AppDir);
            appDirButton.Enabled = BasicSettings.IsAppDirAllowed();
            m_Settings = settings;
            m_BasicSettings = basicSettings;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_Settings.MusicDirectory = musicDirLabel.Text;
            m_Settings.SoundDirectory = soundDirLabel.Text;
            if (otherDirButton.Checked)
            {
                m_BasicSettings.UserSettingsLocation = BasicSettings.SettingsLocation.Custom;
            }
            else if (appDirButton.Checked && BasicSettings.IsAppDirAllowed())
            {
                m_BasicSettings.UserSettingsLocation = BasicSettings.SettingsLocation.AppDir;
            }
            else
            {
                m_BasicSettings.UserSettingsLocation = BasicSettings.SettingsLocation.AppDataDir;
            }
            m_BasicSettings.CustomSettingsDirectory = otherDirLabel.Text;
        }

        private Settings m_Settings { get; set; }
        private BasicSettings m_BasicSettings { get; set; }

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
