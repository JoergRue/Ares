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
using Ares.Data;

namespace Ares.Editor.Controls
{
    public partial class ReverbDialog : Form
    {
        private IFileElement m_Element;

        public Actions.ReverbEffectChangeAction Action { get; set; }

        public ReverbDialog(IFileElement element)
        {
            InitializeComponent();
            m_Element = element;
            Action = new Actions.ReverbEffectChangeAction(m_Element, true);
            volumeUpDown.Value = m_Element.Effects.Reverb.Level;
            delayUpDown.Value = m_Element.Effects.Reverb.Delay;
        }

        public void UpdateAction()
        {
            Action.SetData(true, (int)volumeUpDown.Value, (int)delayUpDown.Value);
        }

        private void UpdateControls()
        {
            volumeUpDown.Enabled = !m_InPlay;
            delayUpDown.Enabled = !m_InPlay;
        }

        private bool m_InPlay = false;

        private void playButton_Click(object sender, EventArgs e)
        {
            m_InPlay = true;
            UpdateAction();
            UpdateControls();
            Action.Do();
            Actions.Playing.Instance.PlayElement(m_Element, this, () =>
            {
                m_InPlay = false;
                Action.Undo();
                UpdateControls();
            }
            );
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopElement(m_Element);
        }

        private void ReverbDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_InPlay)
            {
                Actions.Playing.Instance.StopElement(m_Element);
            }
        }
    }
}
