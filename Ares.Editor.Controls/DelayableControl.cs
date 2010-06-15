using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor.ElementEditors
{
    public partial class DelayableControl : UserControl
    {
        public DelayableControl()
        {
            InitializeComponent();
        }

        public void SetElement(Ares.Data.IDelayableElement element)
        {
            m_Element = element;
            Update(m_Element.Id, Actions.ElementChanges.ChangeType.Changed);
            Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
        }

        private void Update(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (listen && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                fixedDelayUpDown.Value = m_Element.FixedStartDelay.Milliseconds;
                maxDelayUpDown.Value = m_Element.MaximumRandomStartDelay.Milliseconds;
            }
        }

        private void Commit()
        {
            listen = false;
            Actions.Actions.Instance.AddNew(new Actions.DelayableElementChangeAction(m_Element, 
                (int)fixedDelayUpDown.Value, (int)maxDelayUpDown.Value));
            listen = true;
        }

        private Ares.Data.IDelayableElement m_Element;
        private bool listen = true;

        private void fixedDelayUpDown_Leave(object sender, EventArgs e)
        {
            Commit();
        }

        private void maxDelayUpDown_Leave(object sender, EventArgs e)
        {
            Commit();
        }

    }
}
