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

namespace Ares.Editor.ElementEditors
{
    partial class ParallelContainerEditor : EditorBase
    {
        public ParallelContainerEditor()
        {
            InitializeComponent();
        }

        private bool m_HasActiveElement = false;

        public void SetContainer(Ares.Data.IElementContainer<Ares.Data.IParallelElement> container)
        {
            ElementId = container.Id;
            m_Element = container;
            parallelContainerControl.SetContainer(container);
            volumeControl.SetElement(container);
            Update(m_Element.Id, Actions.ElementChanges.ChangeType.Renamed);
            UpdateActiveElement();
            Actions.ElementChanges.Instance.AddListener(-1, Update);
            if (Actions.Playing.Instance.IsElementPlaying(container))
            {
                DisableControls(true);
            }
            else if (Actions.Playing.Instance.IsElementOrSubElementPlaying(container))
            {
                DisableControls(false);
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
            else if (changeType == Actions.ElementChanges.ChangeType.Played || changeType == Actions.ElementChanges.ChangeType.Stopped)
            {
                if (Actions.Playing.Instance.IsElementOrSubElementPlaying(m_Element))
                {
                    DisableControls(false);
                }
                else
                {
                    EnableControls();
                }
            }
        }

        private bool m_AcceptDrop;

        private void ParallelContainerEditor_DragEnter(object sender, DragEventArgs e)
        {
            m_AcceptDrop = parallelContainerControl.Enabled && e.Data.GetDataPresent(typeof(List<DraggedItem>));
        }

        private void ParallelContainerEditor_DragLeave(object sender, EventArgs e)
        {
            m_AcceptDrop = false;
        }

        private void ParallelContainerEditor_DragOver(object sender, DragEventArgs e)
        {
            if (m_AcceptDrop && (e.AllowedEffect & DragDropEffects.Copy) != 0)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void ParallelContainerEditor_DragDrop(object sender, DragEventArgs e)
        {
            List<DraggedItem> list = e.Data.GetData(typeof(List<DraggedItem>)) as List<DraggedItem>;
            if (list != null)
            {
                List<Ares.Data.IElement> elements = new List<Ares.Data.IElement>(DragAndDrop.GetElementsFromDroppedItems(list));
                parallelContainerControl.AddElements(elements);
            }
        }

        private void DisableControls(bool allowStop)
        {
            playButton.Enabled = false;
            stopButton.Enabled = allowStop;
            parallelContainerControl.Enabled = false;
            volumeControl.Enabled = false;
            delayableControl.Enabled = false;
            repeatableControl.Enabled = false;
        }

        private void EnableControls()
        {
            playButton.Enabled = true;
            stopButton.Enabled = false;
            parallelContainerControl.Enabled = true;
            volumeControl.Enabled = true;
            delayableControl.Enabled = m_HasActiveElement;
            repeatableControl.Enabled = m_HasActiveElement;
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

        private bool listen = true;

        private Ares.Data.IElementContainer<Ares.Data.IParallelElement> m_Element;

        private void parallelContainerControl_ActiveRowChanged(object sender, EventArgs e)
        {
            UpdateActiveElement();
        }

        private void UpdateActiveElement()
        {
            Ares.Data.IParallelElement element = parallelContainerControl.ActiveElement;
            if (element != null)
            {
                m_HasActiveElement = true;
                delayableControl.SetElement(element);
                repeatableControl.SetElement(element);
                bool enabled = !Actions.Playing.Instance.IsElementOrSubElementPlaying(m_Element);
                delayableControl.Enabled = enabled;
                repeatableControl.Enabled = enabled;
            }
            else
            {
                m_HasActiveElement = false;
                delayableControl.SetElement(null);
                repeatableControl.SetElement(null);
                delayableControl.Enabled = false;
                repeatableControl.Enabled = false;
            }
        }

        private void parallelContainerControl_ElementDoubleClick(object sender, Controls.ElementDoubleClickEventArgs e)
        {
            Editors.ShowEditor(e.Element.InnerElement, this.DockPanel);
        }
    }
}
