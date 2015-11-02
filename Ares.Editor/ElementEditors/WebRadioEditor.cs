/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using Ares.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor.ElementEditors
{
    partial class WebRadioEditor : EditorBase
    {
        public WebRadioEditor()
        {
            InitializeComponent();
        }

        private IWebRadioElement m_Element;
        private IModeElement m_ModeElement;
        private IProject m_Project;
        private bool listen = true;

        public void SetElement(IWebRadioElement element, IProject project, IModeElement modeElement)
        {
            ElementId = element.Id;
            m_Element = element;
            m_ModeElement = modeElement;
            m_Project = project;
            volumeControl.SetElement(element, project);
            fileVolumeControl.SetContainer(null, project);
            fileVolumeControl.SetEffects(element);
            listen = false;
            urlBox.Text = element.Url;
            listen = true;
            Update(m_Element.Id, Actions.ElementChanges.ChangeType.Renamed);
            Actions.ElementChanges.Instance.AddListener(ElementId, Update);
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
            nameBox.Enabled = false;
            urlBox.Enabled = false;
            volumeControl.Enabled = false;
            fileVolumeControl.Enabled = false;
        }

        private void EnableControls()
        {
            playButton.Enabled = true;
            stopButton.Enabled = false;
            nameBox.Enabled = true;
            urlBox.Enabled = true;
            volumeControl.Enabled = true;
            fileVolumeControl.Enabled = true;
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
                action.Do(m_Project);
            }
            else
            {
                Actions.Actions.Instance.AddNew(new Actions.ElementRenamedAction(m_Element, m_ModeElement, newName), m_Project);
            }

            this.Text = newName;
            listen = true;
        }

        private void urlBox_TextChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            String newUrl = urlBox.Text;
            listen = false;
            Actions.WebRadioElementUrlChangeAction action = Actions.Actions.Instance.LastAction as Actions.WebRadioElementUrlChangeAction;
            if (action != null && action.Element == m_Element)
            {
                action.SetData(newUrl);
                action.Do(m_Project);
            }
            else
            {
                Actions.Actions.Instance.AddNew(new Actions.WebRadioElementUrlChangeAction(m_Element, newUrl), m_Project);
            }
            listen = true;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (m_Element != null)
            {
                listen = false;
                DisableControls(true);
                Actions.Playing.Instance.PlayElement(m_Element, Parent, () => { });
                listen = true;
            }
        }
    }
}
