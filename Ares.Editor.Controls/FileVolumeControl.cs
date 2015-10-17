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
            m_Project = project;
        }

        public void SetEffects(Ares.Data.IFileElement element)
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

                #region tdmod
                cueOutActive.Checked = m_Element.Effects.CueOut.Active;
                cueInActive.Checked = m_Element.Effects.CueIn.Active;
                setCueTime(cueOutTime, m_Element.Effects.CueOut.Position);
                setCueTime(cueInTime, m_Element.Effects.CueIn.Position);
                #endregion

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
            List<Ares.Data.IFileElement> elements = new List<Ares.Data.IFileElement>();
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

        private Ares.Data.IFileElement m_Element;
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

        #region tdmod
        private void commitCueIn() 
        {
            if (m_Element == null)
                return;
            listen = false;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && action is Actions.CueInEffectChangeAction)
            {
                Actions.CueInEffectChangeAction eeca = action as Actions.CueInEffectChangeAction;
                if (eeca.Elements[0] == m_Element)
                {
                    eeca.SetData(cueInActive.Checked, getCueTime(cueInTime, m_Element.Effects.CueIn.Position));
                    eeca.Do(m_Project);
                    listen = true;
                    return;
                }
            }
            List<Ares.Data.IFileElement> elements = new List<Ares.Data.IFileElement>();
            elements.Add(m_Element);
            Actions.Actions.Instance.AddNew(new Actions.CueInEffectChangeAction(elements, cueInActive.Checked, getCueTime(cueInTime, m_Element.Effects.CueIn.Position)), m_Project);
            listen = true;
        }

        private void commitCueOut()
        {
            if (m_Element == null)
                return;
            listen = false;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && action is Actions.CueOutEffectChangeAction)
            {
                Actions.CueOutEffectChangeAction eeca = action as Actions.CueOutEffectChangeAction;
                if (eeca.Elements[0] == m_Element)
                {
                    eeca.SetData(cueOutActive.Checked, getCueTime(cueOutTime,m_Element.Effects.CueOut.Position));
                    eeca.Do(m_Project);
                    listen = true;
                    return;
                }
            }
            List<Ares.Data.IFileElement> elements = new List<Ares.Data.IFileElement>();
            elements.Add(m_Element);
            Actions.Actions.Instance.AddNew(new Actions.CueOutEffectChangeAction(elements, cueOutActive.Checked, getCueTime(cueOutTime, m_Element.Effects.CueOut.Position)), m_Project);
            listen = true;
        }


        private double getCueTime(MaskedTextBox maskedInput, double original)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(maskedInput.Text, "mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture);
                DateTime dt0 = DateTime.ParseExact("00:00:000", "mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture);
                return TimeSpan.FromTicks(
                     dt.Ticks - dt0.Ticks
                 ).TotalSeconds;
            }
            catch (FormatException e)
            {
                setCueTime(maskedInput, original);
                return original;
            }
        }

        private void setCueTime(MaskedTextBox maskedInput, double cueTime)
        {
            TimeSpan ts = TimeSpan.FromSeconds(cueTime);
            maskedInput.Text = new DateTime(ts.Ticks).ToString("mm:ss:fff");
        }

        private void cueInActive_CheckedChanged(object sender, EventArgs e)
        {
            if (listen)
                commitCueIn();
        }

        private void cueOutActive_CheckedChanged(object sender, EventArgs e)
        {
            if (listen)
                commitCueOut();
        }

        private void cueInTime_TextChanged(object sender, EventArgs e)
        {
        }

        private void cueOutTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void cueInTime_Leave(object sender, EventArgs e)
        {
            if (listen)
                commitCueIn();
        }


        private void cueOutTime_Leave(object sender, EventArgs e)
        {
            if (listen)
                commitCueOut();
        }

        #endregion 


    }
}
