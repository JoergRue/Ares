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
    partial class ParallelContainerEditor : ContainerEditorBase
    {
        public ParallelContainerEditor()
        {
            InitializeComponent();
            AttachOtherEvents(playButton, stopButton);
        }

        protected override Controls.ContainerControl ContainerControl
        {
            get
            {
                return parallelContainerControl;
            }
        }

        private bool m_HasActiveElement = false;

        public void SetContainer(Ares.Data.IElementContainer<Ares.Data.IParallelElement> container)
        {
            ElementId = container.Id;
            m_Element = container;
            parallelContainerControl.SetContainer(container);
            volumeControl.SetElement(container);
            ElementSet();
            UpdateActiveElement();
            Actions.ElementChanges.Instance.AddListener(-1, Update);
        }

        protected override void DisableControls(bool allowStop)
        {
            playButton.Enabled = false;
            stopButton.Enabled = allowStop;
            parallelContainerControl.Enabled = false;
            volumeControl.Enabled = false;
            delayableControl.Enabled = false;
            repeatableControl.Enabled = false;
        }

        protected override void EnableControls()
        {
            playButton.Enabled = true;
            stopButton.Enabled = false;
            parallelContainerControl.Enabled = true;
            volumeControl.Enabled = true;
            delayableControl.Enabled = m_HasActiveElement;
            repeatableControl.Enabled = m_HasActiveElement;
        }

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

    }
}
