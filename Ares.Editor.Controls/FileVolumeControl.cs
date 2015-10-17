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
    public partial class FileVolumeControl : UserControl
    {
        public FileVolumeControl()
        {
            InitializeComponent();
            ToolTip tt = new ToolTip();
            tt.SetToolTip(allVolumeButton, StringResources.ForAllFileElements);
            tt.SetToolTip(allFadeInButton, StringResources.ForAllFileElements);
            tt.SetToolTip(allFadeOutButton, StringResources.ForAllFileElements);
            tt.SetToolTip(allCrossFadeButton, StringResources.ForAllFileElements);
            fadeInUnitBox.SelectedIndex = 0;
            fadeOutUnitBox.SelectedIndex = 0;
            tt.SetToolTip(crossFadingBox, StringResources.CrossfadingTooltip);
        }

        private Ares.Data.IProject m_Project;

        public void SetContainer(Ares.Data.IGeneralElementContainer container, Ares.Data.IProject project)
        {
            m_Container = container;
            bool hasContainer = (m_Container != null);
            allFadeInButton.Enabled = hasContainer;
            allFadeOutButton.Enabled = hasContainer;
            allVolumeButton.Enabled = hasContainer;
            allCrossFadeButton.Enabled = hasContainer;
            allFadeInButton.Visible = hasContainer;
            allFadeOutButton.Visible = hasContainer;
            allVolumeButton.Visible = hasContainer;
            allCrossFadeButton.Visible = hasContainer;
            m_Project = project;
        }

        public void SetEffects(Ares.Data.IEffectsElement element)
        {
            if (m_Element != null)
            {
                Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
            }
            m_Element = element;
            if (m_Element != null)
            {
                Update(m_Element.Id, Actions.ElementChanges.ChangeType.Changed);
                Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
                if (m_Element is Ares.Data.IWebRadioElement)
                {
                    fadeOutUnitBox.Enabled = false;
                    fadeOutUpDown.Enabled = false;
                }
            }
            else
            {
                volumeBar.Value = 100;
                fadeInUpDown.Value = 0;
                fadeOutUpDown.Value = 0;
            }
        }

        private void Update(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                listen = false;
                fixedVolumeButton.Checked = !m_Element.Effects.HasRandomVolume;
                randomButton.Checked = m_Element.Effects.HasRandomVolume;
                volumeBar.Value = m_Element.Effects.Volume;
                minRandomUpDown.Value = m_Element.Effects.MinRandomVolume;
                maxRandomUpDown.Value = m_Element.Effects.MaxRandomVolume;
                fadeInUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.Effects.FadeInTime, fadeInUnitBox);
                fadeOutUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.Effects.FadeOutTime, fadeOutUnitBox);
                crossFadingBox.Enabled = m_Element.Effects.FadeOutTime > 0;
                crossFadingBox.Checked = m_Element.Effects.CrossFading;

                listen = true;
            }
        }

        private void Commit()
        {
            if (m_Element == null)
                return;
            listen = false;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && action is Actions.ElementVolumeEffectsChangeAction)
            {
                Actions.ElementVolumeEffectsChangeAction eeca = action as Actions.ElementVolumeEffectsChangeAction;
                if (eeca.Elements[0] == m_Element)
                {
                    eeca.SetData(
                        randomButton.Checked,
                        volumeBar.Value, 
                        (int)minRandomUpDown.Value,
                        (int)maxRandomUpDown.Value,
                        TimeConversion.GetTimeInMillis(fadeInUpDown, fadeInUnitBox), 
                        TimeConversion.GetTimeInMillis(fadeOutUpDown, fadeOutUnitBox),
                        crossFadingBox.Checked);
                    eeca.Do(m_Project);
                    listen = true;
                    return;
                }
            }
            List<Ares.Data.IEffectsElement> elements = new List<Ares.Data.IEffectsElement>();
            elements.Add(m_Element);
            Actions.Actions.Instance.AddNew(new Actions.ElementVolumeEffectsChangeAction(elements,
                randomButton.Checked,
                volumeBar.Value,
                (int)minRandomUpDown.Value,
                (int)maxRandomUpDown.Value,
                TimeConversion.GetTimeInMillis(fadeInUpDown, fadeInUnitBox),
                TimeConversion.GetTimeInMillis(fadeOutUpDown, fadeOutUnitBox),
                crossFadingBox.Checked)
              , m_Project);
            listen = true;
        }

        private Ares.Data.IEffectsElement m_Element;
        private bool listen = true;

        private void volumeBar_ValueChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

        private void allVolumeButton_Click(object sender, EventArgs e)
        {
            if (m_Element == null || m_Container == null)
                return;
            listen = false;
            Actions.Actions.Instance.AddNew(new Actions.AllFileElementsVolumeChangeAction(m_Container, 
                randomButton.Checked,
                volumeBar.Value,
                (int)minRandomUpDown.Value,
                (int)maxRandomUpDown.Value), m_Project);
            listen = true;
        }

        private Ares.Data.IGeneralElementContainer m_Container;

        private void allFadeInButton_Click(object sender, EventArgs e)
        {
            if (m_Element == null || m_Container == null)
                return;
            listen = false;
            Actions.Actions.Instance.AddNew(new Actions.AllFileElementsFadingChangeAction(m_Container,
                TimeConversion.GetTimeInMillis(fadeInUpDown, fadeInUnitBox), true), m_Project);
            listen = true;
        }

        private void allFadeOutButton_Click(object sender, EventArgs e)
        {
            if (m_Element == null || m_Container == null)
                return;
            listen = false;
            Actions.Actions.Instance.AddNew(new Actions.AllFileElementsFadingChangeAction(m_Container,
                TimeConversion.GetTimeInMillis(fadeOutUpDown, fadeOutUnitBox), false), m_Project);
            listen = true;
        }

        private void allCrossFadeButton_Click(object sender, EventArgs e)
        {
            if (m_Element == null || m_Container == null)
                return;
            listen = false;
            Actions.Actions.Instance.AddNew(new Actions.AllFileElementsCrossFadingChangeAction(m_Container,
                crossFadingBox.Checked), m_Project);
            listen = true;
        }

        private void fadeInUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

        private void fadeOutUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            crossFadingBox.Enabled = fadeOutUpDown.Value > 0;
            Commit();
        }

        private void fadeInUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Element == null)
                return;
            listen = false;
            fadeInUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.Effects.FadeInTime, fadeInUnitBox);
            listen = true;
        }

        private void fadeOutUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Element == null)
                return;
            listen = false;
            fadeOutUpDown.Value = TimeConversion.GetTimeInUnit(m_Element.Effects.FadeOutTime, fadeOutUnitBox);
            listen = true;
        }

        private void fixedVolumeButton_CheckedChanged(object sender, EventArgs e)
        {
            ChangeRandom();
        }

        private void ChangeRandom()
        {
            bool fixedVol = fixedVolumeButton.Checked;
            volumeBar.Enabled = fixedVol;
            minRandomUpDown.Enabled = !fixedVol;
            maxRandomUpDown.Enabled = !fixedVol;
            if (listen)
                Commit();
        }

        private void minRandomUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (minRandomUpDown.Value > maxRandomUpDown.Value)
            {
                maxRandomUpDown.Value = minRandomUpDown.Value;
            }
            if (listen)
                Commit();
        }

        private void maxRandomUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (maxRandomUpDown.Value < minRandomUpDown.Value)
            {
                minRandomUpDown.Value = maxRandomUpDown.Value;
            }
            if (listen)
                Commit();
        }

        private void randomButton_CheckedChanged(object sender, EventArgs e)
        {
            ChangeRandom();
        }

        private void crossFadingBox_CheckedChanged(object sender, EventArgs e)
        {
            if (listen)
                Commit();
        }

    }
}
