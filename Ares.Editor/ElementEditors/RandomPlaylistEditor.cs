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
    partial class RandomPlaylistOrBGSoundChoiceEditor : EditorBase
    {
        public RandomPlaylistOrBGSoundChoiceEditor()
        {
            InitializeComponent();
        }

        public void SetPlaylist(IRandomBackgroundMusicList playList)
        {
            ElementId = playList.Id;
            m_Element = playList;
            delayableControl.SetElement(playList);
            repeatableControl.SetElement(playList);
            choiceContainerControl.SetContainer(playList);
            volumeControl.SetElement(playList);
            label1.Text = String.Format(label1.Text, String.Format(StringResources.FileExplorerTitle, StringResources.Music));
            Update(m_Element.Id, Actions.ElementChanges.ChangeType.Renamed);
            Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
            if (Actions.Playing.Instance.IsElementPlaying(playList))
            {
                DisableControls(true);
            }
            else if (Actions.Playing.Instance.IsElementOrSubElementPlaying(playList))
            {
                DisableControls(false);
            }
        }

        public void SetBGSoundChoice(IBackgroundSoundChoice bgSoundChoice)
        {
            ElementId = bgSoundChoice.Id;
            m_Element = bgSoundChoice;
            delayableControl.SetElement(bgSoundChoice);
            repeatableControl.SetElement(bgSoundChoice);
            choiceContainerControl.SetContainer(bgSoundChoice);
            volumeControl.SetElement(bgSoundChoice);
            label1.Text = String.Format(label1.Text, String.Format(StringResources.FileExplorerTitle, StringResources.Sounds));
            Update(m_Element.Id, Actions.ElementChanges.ChangeType.Renamed);
            Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
            if (Actions.Playing.Instance.IsElementPlaying(bgSoundChoice))
            {
                DisableControls(true);
            }
            else if (Actions.Playing.Instance.IsElementOrSubElementPlaying(bgSoundChoice))
            {
                DisableControls(false);
            }
        }

        private bool listen = true;

        private void Update(int elementId, Actions.ElementChanges.ChangeType changeType)
        {
            if (!listen) 
                return;
            if (elementId == m_Element.Id)
            {
                if (changeType == Actions.ElementChanges.ChangeType.Renamed)
                {
                    this.Text = m_Element.Title;
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

        private IElement m_Element;

        private void RandomPlaylistEditor_SizeChanged(object sender, EventArgs e)
        {
            Font font = label1.Font;
            String text = label1.Text;
            using (Graphics g = label1.CreateGraphics())
            {
                float textWidth = g.MeasureString(text, font).Width;
                if (textWidth == 0) return;
                float factor = label1.Width / textWidth;
                if (factor == 0) return;
                label1.Font = new Font(font.Name, font.SizeInPoints * factor);
            }
        }

        private bool m_AcceptDrop;

        private void RandomPlaylistEditor_DragEnter(object sender, DragEventArgs e)
        {
            m_AcceptDrop = choiceContainerControl.Enabled && e.Data.GetDataPresent(typeof(List<DraggedItem>));
        }

        private void RandomPlaylistEditor_DragLeave(object sender, EventArgs e)
        {
            m_AcceptDrop = false;
        }

        private void RandomPlaylistEditor_DragOver(object sender, DragEventArgs e)
        {
            if (m_AcceptDrop && (e.AllowedEffect & DragDropEffects.Copy) != 0)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void RandomPlaylistEditor_DragDrop(object sender, DragEventArgs e)
        {
            List<DraggedItem> list = e.Data.GetData(typeof(List<DraggedItem>)) as List<DraggedItem>;
            if (list != null)
            {
                List<IElement> elements = new List<IElement>(DragAndDrop.GetElementsFromDroppedItems(list));
                choiceContainerControl.AddElements(elements);
            }
        }

        private void DisableControls(bool allowStop)
        {
            playButton.Enabled = false;
            stopButton.Enabled = allowStop;
            choiceContainerControl.Enabled = false;
            delayableControl.Enabled = false;
            repeatableControl.Enabled = false;
            volumeControl.Enabled = false;
        }

        private void EnableControls()
        {
            playButton.Enabled = true;
            stopButton.Enabled = false;
            choiceContainerControl.Enabled = true;
            delayableControl.Enabled = true;
            repeatableControl.Enabled = true;
            volumeControl.Enabled = true;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (m_Element != null)
            {
                listen = false;
                DisableControls(true);
                Actions.Playing.Instance.PlayElement(m_Element, this, () => {});
                listen = true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopElement(m_Element);
        }

        private void choiceContainerControl_ElementDoubleClick(object sender, Controls.ElementDoubleClickEventArgs e)
        {
            Editors.ShowEditor(e.Element.InnerElement, this.DockPanel);
        }

    }
}
