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
    public partial class CutControl : UserControl
    {
        private Ares.Data.IProject m_Project;
        private Ares.Data.IEffectsElement m_Element;
        private bool listen = true;

        public CutControl()
        {
            InitializeComponent();
        }

        public void SetEffects(Ares.Data.IEffectsElement element, Ares.Data.IProject project)
        {
            m_Project = project;
            if (m_Element != null)
            {
                Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
            }
            m_Element = element;
            if (m_Element != null)
            {
                Enabled = true;
                Update(m_Element.Id, Actions.ElementChanges.ChangeType.Changed);
                Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
            }
            else
            {
                Enabled = false;
            }

        }

        private void Update(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                listen = false;

                cueOutActive.Checked = m_Element.Effects.CueOut.Active;
                cueInActive.Checked = m_Element.Effects.CueIn.Active;
                cueInTime.Enabled = cueInActive.Checked;
                cueOutTime.Enabled = cueOutActive.Checked;
                setCueTime(cueOutTime, m_Element.Effects.CueOut.Position);
                setCueTime(cueInTime, m_Element.Effects.CueIn.Position);

                listen = true;
            }
        }

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
            List<Ares.Data.IEffectsElement> elements = new List<Ares.Data.IEffectsElement>();
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
                    eeca.SetData(cueOutActive.Checked, getCueTime(cueOutTime, m_Element.Effects.CueOut.Position));
                    eeca.Do(m_Project);
                    listen = true;
                    return;
                }
            }
            List<Ares.Data.IEffectsElement> elements = new List<Ares.Data.IEffectsElement>();
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
            catch (FormatException)
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
            {
                commitCueIn();
                cueInTime.Enabled = cueInActive.Checked;
            }
        }

        private void cueOutActive_CheckedChanged(object sender, EventArgs e)
        {
            if (listen)
            {
                commitCueOut();
                cueOutTime.Enabled = cueOutActive.Checked;
            }
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

    }
}
