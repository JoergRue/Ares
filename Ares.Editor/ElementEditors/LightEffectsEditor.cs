using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ares.Tags;
using Ares.Data;


namespace Ares.Editor.ElementEditors
{
    partial class LightEffectsEditor : EditorBase
    {
        public LightEffectsEditor()
        {
            InitializeComponent();
            //AttachOtherEvents(playButton, stopButton);
        }

        private ILightEffects m_Element;

        public void SetElement(ILightEffects element, IProject project)
        {
            m_Project = project;
            if (m_Element != null)
            {
                Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
            }
            m_Element = element;
            if (m_Element != null)
            {
                ElementId = m_Element.Id;
                Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
                this.Text = m_Element.Title;
            }
            UpdateControls();
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (!m_Listen)
                return;

            switch (changeType)
            {
                case Actions.ElementChanges.ChangeType.Renamed:
                    this.Text = m_Element.Title;
                    break;
                default:
                    UpdateControls();
                    break;
            }
        }

        private bool m_Listen = true;

        private IProject m_Project;

        private void UpdateControls()
        {
            m_Listen = false;
            try
            {
                // update stuff
                checkBoxChangeMaster.Checked = m_Element.SetsMasterBrightness;
                checkBoxChangeLRMix.Checked = m_Element.SetsLeftRightMix;
                checkBoxChangeLeftScene.Checked = m_Element.SetsLeftScene;
                checkBoxChangeRightScene.Checked = m_Element.SetsRightScene;

                trackBarMaster.Value = m_Element.MasterBrightness;
                trackBarLRMix.Value = m_Element.LeftRightMix;
                numericUpDownLeftScene.Value = m_Element.LeftScene;
                numericUpDownRightScene.Value = m_Element.RightScene;
            }
            finally
            {
                m_Listen = true;
            }
        }

        private void Commit<T, V>(V val)
            where T : Actions.LightEffectsValueAction<V>
        {
            if (!m_Listen)
                return;
            m_Listen = false;
            try
            {
                Actions.Action action = Actions.Actions.Instance.LastAction;
                if (action != null && action is T)
                {
                    T ac = action as T;
                    ac.SetData(val);
                    ac.Do(m_Project);
                    return;
                }
                Actions.Actions.Instance.AddNew(
                    (T)Activator.CreateInstance(typeof(T), m_Element, val),
                    m_Project);
            }
            finally
            {
                m_Listen = true;
            }
        }

        private void buttonCenterLRMix_Click(object sender, EventArgs e)
        {
            if (trackBarLRMix.Value != 127)
                trackBarLRMix.Value = 127;
        }

        private void checkBoxChangeMaster_CheckedChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsSetsMasterBrightnessAction, bool>(checkBoxChangeMaster.Checked);
        }

        private void checkBoxChangeLRMix_CheckedChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsSetsLeftRightMixAction, bool>(checkBoxChangeLRMix.Checked);
        }

        private void checkBoxChangeLeftScene_CheckedChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsSetsLeftSceneAction, bool>(checkBoxChangeLeftScene.Checked);
        }

        private void checkBoxChangeRightScene_CheckedChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsSetsRightSceneAction, bool>(checkBoxChangeRightScene.Checked);
        }

        private void trackBarMaster_ValueChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsMasterBrightnessAction, int>(trackBarMaster.Value);
        }

        private void trackBarLRMix_ValueChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsLeftRightMixAction, int>(trackBarLRMix.Value);
        }

        private void numericUpDownLeftScene_ValueChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsLeftSceneAction, int>((int)numericUpDownLeftScene.Value);
        }

        private void numericUpDownRightScene_ValueChanged(object sender, EventArgs e)
        {
            Commit<Actions.LightEffectsRightSceneAction, int>((int)numericUpDownRightScene.Value);
        }


        private void DisableControls()
        {
            playButton.Enabled = false;
            checkBoxChangeMaster.Enabled = false;
            checkBoxChangeLRMix.Enabled = false;
            checkBoxChangeLeftScene.Enabled = false;
            checkBoxChangeRightScene.Enabled = false;
            trackBarMaster.Enabled = false;
            trackBarLRMix.Enabled = false;
            numericUpDownLeftScene.Enabled = false;
            numericUpDownRightScene.Enabled = false;
        }

        private void EnableControls()
        {
            playButton.Enabled = true;
            checkBoxChangeMaster.Enabled = true;
            checkBoxChangeLRMix.Enabled = true;
            checkBoxChangeLeftScene.Enabled = true;
            checkBoxChangeRightScene.Enabled = true;
            trackBarMaster.Enabled = true;
            trackBarLRMix.Enabled = true;
            numericUpDownLeftScene.Enabled = true;
            numericUpDownRightScene.Enabled = true;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (m_Element != null)
            {
                m_Listen = false;
                DisableControls();
                Actions.Playing.Instance.PlayElement(m_Element, Parent, () => { });
                EnableControls();
                m_Listen = true;
            }
        }

    }
}
