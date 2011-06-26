using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Data;

namespace Ares.Editor.Controls
{
    public partial class FileEffectsControl : UserControl
    {
        public FileEffectsControl()
        {
            InitializeComponent();
        }

        private Ares.Data.IFileElement m_Element;
        private bool listen = true;
        private Ares.Data.IGeneralElementContainer m_Container;

        public void SetContainer(Ares.Data.IGeneralElementContainer container)
        {
            m_Container = container;
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
                pitchButton.Enabled = true;
                allPitchButton.Enabled = true;
            }
            else
            {
                pitchBox.Checked = false;
                pitchButton.Enabled = false;
                allPitchButton.Enabled = false;
            }
        }

        private void Update(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                listen = false;
                pitchBox.Checked = m_Element.Effects.Pitch.Active;
                listen = true;
            }
        }

        private void pitchBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            IIntEffect effect = m_Element.Effects.Pitch;
            Actions.Actions.Instance.AddNew(new Actions.IntEffectChangeAction(m_Element, effect,
                pitchBox.Checked, effect.Random, effect.FixValue, effect.MinRandomValue, effect.MaxRandomValue));
        }

        private void pitchButton_Click(object sender, EventArgs e)
        {
            PitchDialog dialog = new PitchDialog(m_Element);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action);
            }
        }

        private void allPitchButton_Click(object sender, EventArgs e)
        {
            IIntEffect effect = m_Element.Effects.Pitch;
            Actions.Actions.Instance.AddNew(new Actions.AllFileElementsPitchChangeAction(m_Container,
                pitchBox.Checked, effect.Random, effect.FixValue, effect.MinRandomValue, effect.MaxRandomValue));
        }


    }
}
