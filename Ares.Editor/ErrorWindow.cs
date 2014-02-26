/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
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
    partial class ErrorWindow : ToolWindow
    {
        public interface IErrorWindowClient
        {
            void MoveToElement(Object element);
        }
        
        public ErrorWindow()
        {
            InitializeComponent();
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
