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
                listen = false;
                fixedDelayUpDown.Value = (int)m_Element.FixedStartDelay.TotalMilliseconds;
                maxDelayUpDown.Value = (int)m_Element.MaximumRandomStartDelay.TotalMilliseconds;
                listen = true;
            }
        }

        private void Commit()
        {
            listen = false;
            Actions.Action action = Actions.Actions.Instance.LastAction;
            if (action != null && action is Actions.DelayableElementChangeAction)
            {
                Actions.DelayableElementChangeAction deca = action as Actions.DelayableElementChangeAction;
                if (deca.Element == m_Element)
                {
                    deca.SetData((int)fixedDelayUpDown.Value, (int)maxDelayUpDown.Value);
                    deca.Do();
                    listen = true;
                    return;
                }
            }
            Actions.Actions.Instance.AddNew(new Actions.DelayableElementChangeAction(m_Element, 
                (int)fixedDelayUpDown.Value, (int)maxDelayUpDown.Value));
            listen = true;
        }

        private Ares.Data.IDelayableElement m_Element;
        private bool listen = true;

        private void fixedDelayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

        private void maxDelayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

    }
}
