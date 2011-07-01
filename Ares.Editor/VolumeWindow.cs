/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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

namespace Ares.Editor
{
    partial class VolumeWindow : ToolWindow
    {
        public VolumeWindow()
        {
            InitializeComponent();
        }

#if !MONO
        protected override string GetPersistString()
        {
            return "VolumeWindow";
        }
#endif

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
