using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor
{
    public partial class ErrorWindow : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public interface IErrorWindowClient
        {
            void MoveToElement(Object element);
        }
        
        public ErrorWindow()
        {
            InitializeComponent();
            HideOnClose = true;
            m_ErrorElements = new List<object>();
            Ares.ModelInfo.ModelChecks.Instance.ErrorsUpdated += new EventHandler<EventArgs>(ErrorsUpdated);
        }

        private void ErrorsUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(Refill));
            }
            else
            {
                Refill();
            }
        }

        protected override string GetPersistString()
        {
            return "ErrorWindow";
        }

        public IErrorWindowClient Client { get; set; }

        public void Refill()
        {
            m_ErrorElements.Clear();
            messagesGrid.SuspendLayout();
            messagesGrid.Rows.Clear();
            foreach (Ares.ModelInfo.ModelError error in Ares.ModelInfo.ModelChecks.Instance.GetAllErrors())
            {
                if (error.Severity == ModelInfo.ModelError.ErrorSeverity.Error)
                {
                    messagesGrid.Rows.Add(ImageResources.eventlogError, error.Message);
                }
                else
                {
                    messagesGrid.Rows.Add(ImageResources.eventlogWarn, error.Message);
                }
                m_ErrorElements.Add(error.Element);
            }
            messagesGrid.ResumeLayout();
        }

        private void ErrorWindow_Load(object sender, EventArgs e)
        {
            Refill();
        }


        private List<Object> m_ErrorElements;

        private void messagesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Client != null)
            {
                Client.MoveToElement(m_ErrorElements[e.RowIndex]);
            }
        }
    }
}
