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
    public partial class VolumeWindow : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public VolumeWindow()
        {
            InitializeComponent();
        }

        protected override string GetPersistString()
        {
            return "VolumeWindow";
        }

        private void VolumeWindow_Load(object sender, EventArgs e)
        {
            UpdateVolumes();
            Settings.Settings.Instance.SettingsChanged += new EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
        }

        void SettingsChanged(object sender, Settings.Settings.SettingsEventArgs e)
        {
            SettingsChanged();
        }

        void SettingsChanged()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => SettingsChanged()));
                return;
            }
            UpdateVolumes();
            Playing.PlayingModule.ElementPlayer.SetVolume(Playing.VolumeTarget.Both, Settings.Settings.Instance.GlobalVolume);
            Playing.PlayingModule.ElementPlayer.SetVolume(Playing.VolumeTarget.Music, Settings.Settings.Instance.MusicVolume);
            Playing.PlayingModule.ElementPlayer.SetVolume(Playing.VolumeTarget.Sounds, Settings.Settings.Instance.SoundVolume);
        }

        private bool listenForVolumes = true;

        void UpdateVolumes()
        {
            listenForVolumes = false;
            overallVolumeBar.Value = Settings.Settings.Instance.GlobalVolume;
            musicVolumeBar.Value = Settings.Settings.Instance.MusicVolume;
            soundVolumeBar.Value = Settings.Settings.Instance.SoundVolume;
            listenForVolumes = true;
        }

        private void overallVolumeBar_Scroll(object sender, EventArgs e)
        {
            if (!listenForVolumes) return;
            Playing.PlayingModule.ElementPlayer.SetVolume(Playing.VolumeTarget.Both, overallVolumeBar.Value);
            Settings.Settings.Instance.GlobalVolume = overallVolumeBar.Value;
            Settings.Settings.Instance.Commit();
        }

        private void musicVolumeBar_Scroll(object sender, EventArgs e)
        {
            if (!listenForVolumes) return;
            Playing.PlayingModule.ElementPlayer.SetVolume(Playing.VolumeTarget.Music, musicVolumeBar.Value);
            Settings.Settings.Instance.MusicVolume = musicVolumeBar.Value;
            Settings.Settings.Instance.Commit();
        }

        private void soundVolumeBar_Scroll(object sender, EventArgs e)
        {
            if (!listenForVolumes) return;
            Playing.PlayingModule.ElementPlayer.SetVolume(Playing.VolumeTarget.Sounds, soundVolumeBar.Value);
            Settings.Settings.Instance.SoundVolume = soundVolumeBar.Value;
            Settings.Settings.Instance.Commit();
        }
    }
}
