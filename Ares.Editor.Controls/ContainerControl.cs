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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Ares.Editor.Controls
{
    public class ElementDoubleClickEventArgs : EventArgs
    {
        public Ares.Data.IContainerElement Element { get; private set; }

        public ElementDoubleClickEventArgs(Ares.Data.IContainerElement element)
        {
            Element = element;
        }
    }

    public abstract class ContainerControl : UserControl
    {
        public event EventHandler<ElementDoubleClickEventArgs> ElementDoubleClick;

        protected bool listen = true;

        protected ContainerControl()
        {
            Actions.FilesWatcher.Instance.AnyDirChanges += new EventHandler<EventArgs>(FileDirChanges);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Actions.FilesWatcher.Instance.AnyDirChanges -= new EventHandler<EventArgs>(FileDirChanges);
            }
            base.Dispose(disposing);
        }
        
        private void FileDirChanges(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(FillGrid));
            }
            else
            {
                FillGrid();
            }
        }

        private void FillGrid()
        {
            listen = false;
            RefillGrid();
            listen = true;
        }

        protected void FireElementDoubleClick(Ares.Data.IContainerElement element)
        {
            ElementDoubleClick(this, new ElementDoubleClickEventArgs(element));
        }

        protected void SetElementAttributes(DataGridView grid, Ares.Data.IContainerElement element, int row)
        {
            if (element.InnerElement is Ares.Data.IFileElement)
            {
                String path = (element.InnerElement as Ares.Data.IFileElement).FilePath;
                grid.Rows[row].Cells[0].ToolTipText = path;
            }
            IList<Ares.ModelInfo.ModelError> errors = Ares.ModelInfo.ModelChecks.Instance.GetErrorsForElement(element.InnerElement);
            if (errors.Count > 0)
            {
                bool onlyWarnings = true;
                String errorTexts = String.Empty;
                foreach (Ares.ModelInfo.ModelError error in errors)
                {
                    if (errorTexts.Length > 0)
                        errorTexts += "\n";
                    errorTexts += error.Message;
                    if (error.Severity == ModelInfo.ModelError.ErrorSeverity.Error)
                        onlyWarnings = false;
                }
                {
                    grid.Rows[row].Cells[0].Style.ForeColor = onlyWarnings ? Color.Orange : Color.DarkRed;
                    grid.Rows[row].Cells[0].ErrorText = errorTexts;
                }
            }
        }

        protected abstract void RefillGrid();
    }
}
