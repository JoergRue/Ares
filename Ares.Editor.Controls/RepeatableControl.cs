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

using Ares.Data;
using Ares.Editor.Controls;

namespace Ares.Editor.ElementEditors
{
    public partial class RepeatableControl : UserControl
    {
        public RepeatableControl()
        {
            InitializeComponent();
            fixedUnitBox.SelectedIndex = 0;
            randomUnitBox.SelectedIndex = 0;
        }

        public void SetElement(IRepeatableElement element)
        {
            if (m_Element != null)
            {
                Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
            }
            m_Element = element;
            if (m_Element != null)
            {
                Update(element.Id, Actions.ElementChanges.ChangeType.Changed);
                Actions.ElementChanges.Instance.AddListener(element.Id, Update);
            }
            else
            {
                loopButton.Checked = false;
                noLoopButton.Checked = true;
                repeatCountUpDown.Value = 1;
                fixedDelayUpDown.Value = 0;
                maxDelayUpDown.Value = 0;
            }
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                listen = false;
                loopButton.Checked = m_Element.RepeatCount == -1;
                noLoopButton.Checked = m_Element.RepeatCount != -1;
                repeatCountUpDown.Value = m_Element.RepeatCount == -1 ? 1 : m_Element.RepeatCount;
                fixedDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.FixedIntermediateDelay, fixedUnitBox);
                maxDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.MaximumRandomIntermediateDelay, randomUnitBox);
                UpdateControlActivation();
                this.Refresh();
                listen = true;
            }
        }

        private void Commit()
        {
            if (m_Element == null)
                return;
            listen = false;
            int repeatCount = loopButton.Checked ? -1 : (int)repeatCountUpDown.Value;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && action is Actions.RepeatableElementChangeAction)
            {
                Actions.RepeatableElementChangeAction reca = action as Actions.RepeatableElementChangeAction;
                if (reca.Element == m_Element)
                {
                    reca.SetData(repeatCount, 
                        TimeConversion.GetTimeInMillis(fixedDelayUpDown, fixedUnitBox),
                        TimeConversion.GetTimeInMillis(maxDelayUpDown, randomUnitBox));
                    reca.Do();
                    listen = true;
                    return;
                }
            }
            Actions.Actions.Instance.AddNew(new Actions.RepeatableElementChangeAction(m_Element,
                repeatCount,
                TimeConversion.GetTimeInMillis(fixedDelayUpDown, fixedUnitBox),
                TimeConversion.GetTimeInMillis(maxDelayUpDown, randomUnitBox)));
            listen = true;
        }

        private IRepeatableElement m_Element;
        private bool listen = true;

        private void UpdateControlActivation()
        {
            bool repeat = loopButton.Checked || repeatCountUpDown.Value > 1;
            repeatCountUpDown.Enabled = noLoopButton.Checked;
            fixedDelayUpDown.Enabled = repeat;
            maxDelayUpDown.Enabled = repeat;
        }

        private void fixedDelayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            Commit();
        }

        private void maxDelayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            Commit();
        }

        private void noLoopButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            UpdateControlActivation();
            Commit();
        }

        private void loopButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            UpdateControlActivation();
            Commit();
        }

        private void repeatCountUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            UpdateControlActivation();
            Commit();
        }

        private void fixedUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Element == null)
                return;
            listen = false;
            fixedDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.FixedIntermediateDelay, fixedUnitBox);
            listen = true;
        }

        private void randomUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Element == null)
                return;
            listen = false;
            maxDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.MaximumRandomIntermediateDelay, randomUnitBox);
            listen = true;
        }

    }
}
