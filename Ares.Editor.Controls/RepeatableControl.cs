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
