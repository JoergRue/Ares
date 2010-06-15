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
                fixedDelayUpDown.Value = m_Element.FixedIntermediateDelay.Milliseconds;
                maxDelayUpDown.Value = m_Element.MaximumRandomIntermediateDelay.Milliseconds;
                fixedDelayUpDown.Enabled = repeatBox.Checked;
                maxDelayUpDown.Enabled = repeatBox.Checked;
                this.Refresh();
                listen = true;
            }
        }

        private void Commit()
        {
            listen = false;
            Actions.Actions.Instance.AddNew(new Actions.RepeatableElementChangeAction(m_Element,
                repeatBox.Checked, (int)fixedDelayUpDown.Value, (int)maxDelayUpDown.Value));
            listen = true;
            this.Refresh();
        }

        private IRepeatableElement m_Element;
        private bool listen = true;

        private void fixedDelayUpDown_Leave(object sender, EventArgs e)
        {
            Commit();
        }

        private void maxDelayUpDown_Leave(object sender, EventArgs e)
        {
            Commit();
        }

        private void repeatBox_CheckedStateChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            Commit();
            fixedDelayUpDown.Enabled = repeatBox.Checked;
            maxDelayUpDown.Enabled = repeatBox.Checked;
        }

    }
}
