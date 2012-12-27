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
using Ares.Editor.Controls;

namespace Ares.Editor.ElementEditors
{
    public partial class DelayableControl : UserControl
    {
        public DelayableControl()
        {
            InitializeComponent();
            fixedUnitBox.SelectedIndex = 0;
            randomUnitBox.SelectedIndex = 0;
        }

        private Ares.Data.IProject m_Project;

        public void SetElement(Ares.Data.IDelayableElement element, Ares.Data.IProject project)
        {
            if (m_Element != null)
            {
                Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
            }
            m_Element = element;
            m_Project = project;
            if (m_Element != null)
            {
                Update(m_Element.Id, Actions.ElementChanges.ChangeType.Changed);
                Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
            }
            else
            {
                fixedDelayUpDown.Value = 0;
                maxDelayUpDown.Value = 0;
            }
        }

        private void Update(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                listen = false;
                fixedDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.FixedStartDelay, fixedUnitBox);
                maxDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.MaximumRandomStartDelay, randomUnitBox);
                listen = true;
            }
        }

        private void Commit()
        {
            if (m_Element == null)
                return;
            listen = false;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && action is Actions.DelayableElementChangeAction)
            {
                Actions.DelayableElementChangeAction deca = action as Actions.DelayableElementChangeAction;
                if (deca.Element == m_Element)
                {
                    deca.SetData(
                        TimeConversion.GetTimeInMillis(fixedDelayUpDown, fixedUnitBox),
                        TimeConversion.GetTimeInMillis(maxDelayUpDown, randomUnitBox));
                    deca.Do(m_Project);
                    listen = true;
                    return;
                }
            }
            Actions.Actions.Instance.AddNew(new Actions.DelayableElementChangeAction(m_Element, 
                TimeConversion.GetTimeInMillis(fixedDelayUpDown, fixedUnitBox),
                TimeConversion.GetTimeInMillis(maxDelayUpDown, randomUnitBox)), m_Project);
            listen = true;
        }

        private Ares.Data.IDelayableElement m_Element;
        private bool listen = true;

        private void fixedDelayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

        private void maxDelayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

        private void fixedUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Element == null)
                return;
            listen = false;
            fixedDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.FixedStartDelay, fixedUnitBox);
            listen = true;
        }

        private void randomUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Element == null)
                return;
            listen = false;
            maxDelayUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.MaximumRandomStartDelay, randomUnitBox);
            listen = true;
        }

    }
}
