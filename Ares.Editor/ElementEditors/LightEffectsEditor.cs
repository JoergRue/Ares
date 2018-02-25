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
                Ares.Editor.Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
            }
            m_Element = element;
            if (m_Element != null)
            {
                Ares.Editor.Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
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

            // update stuff

            m_Listen = true;
        }

        private void buttonCenterLRMix_Click(object sender, EventArgs e)
        {
            trackBarLRMix.Value = 127;
        }

        private void checkBoxChangeMaster_CheckedChanged(object sender, EventArgs e)
        {
            m_Element.SetsMasterBrightness=checkBoxChangeMaster.Checked;
        }
    }
}
