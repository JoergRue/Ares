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
