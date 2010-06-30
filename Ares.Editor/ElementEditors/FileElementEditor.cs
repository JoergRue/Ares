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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Data;

namespace Ares.Editor.ElementEditors
{
    partial class FileElementEditor : EditorBase
    {
        public FileElementEditor()
        {
            InitializeComponent();
        }

        private IFileElement m_Element;
        private bool listen = true;

        public void SetElement(IFileElement element)
        {
            ElementId = element.Id;
            m_Element = element;
            fileEffectsControl.SetEffects(element);
            UpdateStaticInfo();
            Update(m_Element.Id, Actions.ElementChanges.ChangeType.Renamed);
            Actions.ElementChanges.Instance.AddListener(ElementId, Update);
        }

        private void UpdateStaticInfo()
        {
            typeLabel.Text = m_Element.SoundFileType == SoundFileType.Music ? StringResources.Music : StringResources.Sounds;
            filePathLabel.Text = m_Element.FilePath;
            String path = m_Element.SoundFileType == SoundFileType.Music ? Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
            path = System.IO.Path.Combine(path, m_Element.FilePath);
            Un4seen.Bass.AddOn.Tags.TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(path, true, true);
            if (tag != null)
            {
                titleLabel.Text = String.IsNullOrEmpty(tag.title) ? StringResources.Unknown : tag.title;
                artistLabel.Text = String.IsNullOrEmpty(tag.artist) ? StringResources.Unknown : tag.artist;
                albumLabel.Text = String.IsNullOrEmpty(tag.album) ? StringResources.Unknown : tag.album;
                TimeSpan duration = TimeSpan.FromSeconds(tag.duration);
                LengthLabel.Text = (DateTime.Today + duration).ToString("HH::mm::ss.fff");
            }
            else
            {
                titleLabel.Text = StringResources.Unknown;
                artistLabel.Text = StringResources.Unknown;
                albumLabel.Text = StringResources.Unknown;
                LengthLabel.Text = StringResources.Unknown;
            }
        }

        private void Update(int elementId, Actions.ElementChanges.ChangeType changeType)
        {
            if (!listen)
                return;
            if (elementId == m_Element.Id)
            {
                if (changeType == Actions.ElementChanges.ChangeType.Renamed)
                {
                    listen = false;
                    this.Text = m_Element.Title;
                    nameBox.Text = m_Element.Title;
                    listen = true;
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Removed)
                {
                    Close();
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Played)
                {
                    DisableControls(false);
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Stopped)
                {
                    EnableControls();
                }
            }
        }

        private void DisableControls(bool allowStop)
        {
            playButton.Enabled = false;
            stopButton.Enabled = allowStop;
            fileEffectsControl.Enabled = false;
            nameBox.Enabled = false;
        }

        private void EnableControls()
        {
            playButton.Enabled = true;
            stopButton.Enabled = false;
            fileEffectsControl.Enabled = true;
            nameBox.Enabled = true;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (m_Element != null)
            {
                listen = false;
                DisableControls(true);
                Actions.Playing.Instance.PlayElement(m_Element, this, () => { });
                listen = true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopElement(m_Element);
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            String newName = nameBox.Text;
            listen = false;
            Actions.ElementRenamedAction action = Actions.Actions.Instance.LastAction as Actions.ElementRenamedAction;
            if (action != null && action.Element == m_Element)
            {
                action.SetName(newName);
                action.Do();
            }
            else
            {
                Actions.Actions.Instance.AddNew(new Actions.ElementRenamedAction(m_Element, newName));
            }
            this.Text = newName;
            listen = true;
        }

    }
}
