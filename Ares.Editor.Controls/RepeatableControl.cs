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

namespace Ares.Editor.ElementEditors
{
    public partial class RepeatableControl : UserControl
    {
        public RepeatableControl()
        {
            InitializeComponent();
        }

        public void SetElement(IRepeatableElement element)
        {
            m_Element = element;
            Update(element.Id, Actions.ElementChanges.ChangeType.Changed);
            Actions.ElementChanges.Instance.AddListener(element.Id, Update);
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                listen = false;
                repeatBox.Checked = m_Element.Repeat;
                fixedDelayUpDown.Value = (int)m_Element.FixedIntermediateDelay.TotalMilliseconds;
                maxDelayUpDown.Value = (int)m_Element.MaximumRandomIntermediateDelay.TotalMilliseconds;
                fixedDelayUpDown.Enabled = repeatBox.Checked;
                maxDelayUpDown.Enabled = repeatBox.Checked;
                this.Refresh();
                listen = true;
            }
        }

        private void Commit()
        {
            listen = false;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && action is Actions.RepeatableElementChangeAction)
            {
                Actions.RepeatableElementChangeAction reca = action as Actions.RepeatableElementChangeAction;
                if (reca.Element == m_Element)
                {
                    reca.SetData(repeatBox.Checked, (int)fixedDelayUpDown.Value, (int)maxDelayUpDown.Value);
                    reca.Do();
                    listen = true;
                    return;
                }
            }
            Actions.Actions.Instance.AddNew(new Actions.RepeatableElementChangeAction(m_Element,
                repeatBox.Checked, (int)fixedDelayUpDown.Value, (int)maxDelayUpDown.Value));
            listen = true;
        }

        private IRepeatableElement m_Element;
        private bool listen = true;

        private void repeatBox_CheckedStateChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            Commit();
            fixedDelayUpDown.Enabled = repeatBox.Checked;
            maxDelayUpDown.Enabled = repeatBox.Checked;
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

    }
}
