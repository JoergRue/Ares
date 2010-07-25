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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor.Controls
{
    public partial class VolumeControl : UserControl
    {
        public VolumeControl()
        {
            InitializeComponent();
        }

        public void SetElement(Ares.Data.IElement element)
        {
            m_Element = element;
            listen = false;
            UpdateData();
            Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
            listen = true;
        }

        private void setsMusicBox_CheckedChanged(object sender, EventArgs e)
        {
            musicVolumeBar.Enabled = setsMusicBox.Checked;
            Commit();
        }

        private void setsSoundBox_CheckedChanged(object sender, EventArgs e)
        {
            soundVolumeBar.Enabled = setsSoundBox.Checked;
            Commit();
        }

        private void musicVolumeBar_ValueChanged(object sender, EventArgs e)
        {
            Commit();
        }

        private void Update(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                listen = false;
                UpdateData();
                listen = true;
            }
        }

        private void UpdateData()
        {
            setsMusicBox.Checked = m_Element.SetsMusicVolume;
            setsSoundBox.Checked = m_Element.SetsSoundVolume;
            musicVolumeBar.Value = m_Element.MusicVolume;
            soundVolumeBar.Value = m_Element.SoundVolume;
        }

        private void Commit()
        {
            if (!listen)
                return;
            listen = false;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && (action is Actions.ElementVolumeChangeAction))
            {
                Actions.ElementVolumeChangeAction evca = action as Actions.ElementVolumeChangeAction;
                if (evca.Element == m_Element)
                {
                    evca.SetData(setsMusicBox.Checked, setsSoundBox.Checked, musicVolumeBar.Value, soundVolumeBar.Value);
                    evca.Do();
                    listen = true;
                    return;
                }
            }
            Actions.Actions.Instance.AddNew(new Actions.ElementVolumeChangeAction(m_Element, setsMusicBox.Checked, setsSoundBox.Checked,
                musicVolumeBar.Value, soundVolumeBar.Value));
            listen = true;
        }

        private Ares.Data.IElement m_Element;
        private bool listen;

        private void currentSoundVolumeButton_Click(object sender, EventArgs e)
        {
            listen = false;
            setsSoundBox.Checked = true;
            soundVolumeBar.Value = Ares.Settings.Settings.Instance.SoundVolume;
            listen = true;
            Commit();
        }

        private void currentMusicVolumeButton_Click(object sender, EventArgs e)
        {
            listen = false;
            setsMusicBox.Checked = true;
            musicVolumeBar.Value = Ares.Settings.Settings.Instance.MusicVolume;
            listen = true;
            Commit();
        }

    }
}
