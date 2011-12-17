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
using Ares.Data;

namespace Ares.Editor.Controls
{
    public class ContainerControl : UserControl
    {
        public event EventHandler<ElementDoubleClickEventArgs> ElementDoubleClick;

        public event EventHandler<ElementImportEventArgs> ElementsImported;

        public void AddElements(IList<IElement> elements)
        {
            listen = false;
            int index = ElementsContainer.GetGeneralElements().Count;
            Actions.Actions.Instance.AddNew(new Actions.AddContainerElementsAction(ElementsContainer, elements));
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            for (int i = index; i < containerElements.Count; ++i)
            {
                AddElementToGrid(containerElements[i]);
                SetElementAttributes(Grid, containerElements[i], i);
                m_ElementsToRows[containerElements[i].Id] = i;
                Actions.ElementChanges.Instance.AddListener(containerElements[i].Id, Update);
            }
            listen = true;
        }

        public void AddImportedElements(IList<IXmlWritable> elements, int insertionRow)
        {
            listen = false;
            int index = insertionRow;
            Actions.Actions.Instance.AddNew(new Actions.AddImportedContainerElementsAction(ElementsContainer, elements, insertionRow));
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            for (int i = index; i < containerElements.Count; ++i)
            {
                AddElementToGrid(containerElements[i]);
                SetElementAttributes(Grid, containerElements[i], i);
                m_ElementsToRows[containerElements[i].Id] = i;
                Actions.ElementChanges.Instance.AddListener(containerElements[i].Id, Update);
            }
            listen = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (int key in m_ElementsToRows.Keys)
                {
                    Actions.ElementChanges.Instance.RemoveListener(key, Update);
                }
                if (ElementsContainer != null)
                    Actions.ElementChanges.Instance.RemoveListener(ElementsContainer.Id, Update);
                Actions.FilesWatcher.Instance.AnyDirChanges -= new EventHandler<EventArgs>(FileDirChanges);
            }
            base.Dispose(disposing);            
        }

        private ToolStripMenuItem copyMenuItem;
        private ToolStripMenuItem cutMenuItem;
        private ToolStripMenuItem pasteMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem deleteMenuItem;

        private ContextMenuStrip gridContextMenu;
        private ToolStripMenuItem editMenuItem;
        private ToolStripSeparator toolStripSeparator2;

        protected bool listen = true;

        protected ContainerControl()
        {
            Actions.FilesWatcher.Instance.AnyDirChanges += new EventHandler<EventArgs>(FileDirChanges);
            InitializeComponent();
        }

        protected void AttachGridEvents()
        {
            Grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseDown);
            Grid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseMove);
            Grid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseUp);
            Grid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.Grid_RowsRemoved);
            Grid.ContextMenuStrip = gridContextMenu;
        }

        protected void ContainerSet()
        {
            Update(ElementsContainer.Id, Actions.ElementChanges.ChangeType.Changed);
            Actions.ElementChanges.Instance.AddListener(ElementsContainer.Id, Update);
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
            if (ElementDoubleClick != null)
                ElementDoubleClick(this, new ElementDoubleClickEventArgs(element));
        }

        protected void FireElementsImported(String data, int row)
        {
            if (ElementsImported != null)
                ElementsImported(this, new ElementImportEventArgs(data, row));
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

        private void Grid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!listen)
                return;
            listen = false;
            List<IElement> elements = new List<IElement>();
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            for (int i = 0; i < e.RowCount; ++i)
            {
                elements.Add(containerElements[e.RowIndex + i]);
            }
            Actions.Actions.Instance.AddNew(new Actions.RemoveContainerElementsAction(ElementsContainer, elements, e.RowIndex));
            listen = true;
        }

        protected void RefillGrid() 
        {
            Grid.SuspendLayout();
            Grid.Rows.Clear();
            foreach (int key in m_ElementsToRows.Keys)
            {
                Actions.ElementChanges.Instance.RemoveListener(key, Update);
            }
            m_ElementsToRows.Clear();
            int row = 0;
            foreach (IContainerElement element in ElementsContainer.GetGeneralElements())
            {
                AddElementToGrid(element);                
                m_ElementsToRows[element.Id] = row;
                Actions.ElementChanges.Instance.AddListener(element.Id, Update);
                SetElementAttributes(Grid, element, row);
                ++row;
            }
            Grid.ResumeLayout();
        }

        private void Grid_MouseDown(Object sender, MouseEventArgs e)
        {
            mouseDownRowIndex = Grid.HitTest(e.X, e.Y).RowIndex;
            if (mouseDownRowIndex != -1)
            {
                Size dragSize = SystemInformation.DragSize;
                dragStartRect = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
                Grid.Capture = true;
            }
            else
            {
                dragStartRect = Rectangle.Empty;
            }
        }

        private void Grid_MouseMove(Object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && mouseDownRowIndex != -1 && dragStartRect != Rectangle.Empty && !dragStartRect.Contains(e.X, e.Y))
            {
                List<int> selectedRows = new List<int>();
                foreach (DataGridViewRow row in Grid.SelectedRows) selectedRows.Add(row.Index);
                GridDataExchangeData data = new GridDataExchangeData() { SelectedRows = selectedRows, SerializedData = SerializeSelectedRows() };
                dragStoppedHere = false;
                dragStartedHere = true;
                DragDropEffects dropEffect = Grid.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
                if (dropEffect == DragDropEffects.Move && !dragStoppedHere)
                {
                    RemoveSelectedRows();
                }
                dragStartedHere = false;
            }
        }

        private void Grid_MouseUp(Object sender, MouseEventArgs e)
        {
            dragStartRect = Rectangle.Empty;
            Grid.Capture = false;
        }

        public bool PerformDragEnter(DragEventArgs e)
        {
            acceptDrop = Enabled && e.Data.GetDataPresent(typeof(GridDataExchangeData));
            return acceptDrop;
        }

        public bool PerformDragOver(DragEventArgs e)
        {
            if (acceptDrop && (e.KeyState & 8) != 0 && (e.AllowedEffect & DragDropEffects.Copy) != 0)
            {
                e.Effect = DragDropEffects.Copy;
                return true;
            }
            else if (acceptDrop && (e.AllowedEffect & DragDropEffects.Move) != 0)
            {
                e.Effect = DragDropEffects.Move;
                return true;
            }
            else
                return false;
        }

        public void PerformDragLeave(EventArgs e)
        {
            acceptDrop = false;
        }

        public bool PerformDrop(DragEventArgs e)
        {
            if (!acceptDrop)
                return false;
            Point clientPoint = Grid.PointToClient(new Point(e.X, e.Y));
            int targetRow = Grid.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            if (targetRow < 0 || targetRow > Grid.RowCount)
            {
                targetRow = Grid.RowCount;
            }
            if (dragStartedHere)
            {
                if (e.Effect == DragDropEffects.Move)
                {
                    MoveRows(((GridDataExchangeData)e.Data.GetData(typeof(GridDataExchangeData))).SelectedRows, targetRow);
                    dragStoppedHere = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                String serializedData = ((GridDataExchangeData)e.Data.GetData(typeof(GridDataExchangeData))).SerializedData;
                FireElementsImported(serializedData, targetRow);
                return true;
            }
        }

        protected String SerializeSelectedRows()
        {
            if (Grid.SelectedRows.Count == 0)
                return String.Empty;
            List<IXmlWritable> elements = new List<IXmlWritable>();
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            List<int> selectedIndices = new List<int>();
            foreach (DataGridViewRow row in Grid.SelectedRows)
            {
                selectedIndices.Add(row.Index);
            }
            selectedIndices.Sort();
            foreach (int rowIndex in selectedIndices)
            {
                elements.Add(containerElements[rowIndex]);
            }
            StringBuilder builder = new StringBuilder();
            Data.DataModule.ProjectManager.ExportElements(elements, builder);
            return builder.ToString();
        }

        protected void RemoveSelectedRows()
        {
            if (Grid.SelectedRows.Count == 0)
                return;
            listen = false;
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            List<IElement> elements = new List<IElement>();
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            foreach (DataGridViewRow row in Grid.SelectedRows)
            {
                elements.Add(containerElements[row.Index]);
                rows.Add(row);
            }
            Actions.Actions.Instance.AddNew(new Actions.RemoveContainerElementsAction(ElementsContainer, elements, Grid.SelectedRows[0].Index));
            RefillGrid();
            listen = true;
        }

        private void MoveRows(List<int> rows, int targetIndex)
        {
            listen = false;
            rows.Sort();
            Actions.Actions.Instance.AddNew(new Actions.ReorderContainerElementsAction(ElementsContainer, rows, targetIndex));
            RefillGrid();
            listen = true;
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (!listen)
                return;
            listen = false;
            if (elementID == ElementsContainer.Id && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                RefillGrid();
            }
            else if (m_ElementsToRows.ContainsKey(elementID))
            {
                if (changeType == Actions.ElementChanges.ChangeType.Removed)
                {
                    RefillGrid();
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Renamed)
                {
                    Grid.Rows[m_ElementsToRows[elementID]].Cells[0].Value =
                        Ares.Data.DataModule.ElementRepository.GetElement(elementID).Title;
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Changed)
                {
                    ChangeElementDataInGrid(elementID, m_ElementsToRows[elementID]);
                }
            }
            listen = true;
        }

        private void editMenuItem_Click(object sender, EventArgs e)
        {
            if (Grid.SelectedRows.Count > 0)
            {
                FireElementDoubleClick(ElementsContainer.GetGeneralElements()[Grid.SelectedRows[0].Index]);
            }
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedRows();
        }

        private void pasteMenuItem_Click(object sender, EventArgs e)
        {
            String serializedData = (String)Clipboard.GetData(DataFormats.GetFormat(formatName).Name);
            FireElementsImported(serializedData, Grid.RowCount);
        }

        private void cutMenuItem_Click(object sender, EventArgs e)
        {
            CopyRowsToClipboard();
            RemoveSelectedRows();
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            CopyRowsToClipboard();
        }

        private void gridContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool enable = Grid.Enabled && Grid.SelectedRows.Count > 0;
            copyMenuItem.Enabled = enable;
            cutMenuItem.Enabled = enable;
            deleteMenuItem.Enabled = enable;
            pasteMenuItem.Enabled = Grid.Enabled && Clipboard.ContainsData(DataFormats.GetFormat(formatName).Name);
            editMenuItem.Enabled = enable;
        }

        private static readonly String formatName = "AresGridDataExchangeFormat";

        private void CopyRowsToClipboard()
        {
            String serializedData = SerializeSelectedRows();
            Clipboard.SetData(DataFormats.GetFormat(formatName).Name, serializedData);
        }

        protected virtual DataGridView Grid { get { return null; } }
        protected virtual Ares.Data.IGeneralElementContainer ElementsContainer { get { return null; } }

        protected virtual void AddElementToGrid(IContainerElement element)
        {
        }

        protected virtual void ChangeElementDataInGrid(int elementID, int row)
        {
        }

        private Dictionary<int, int> m_ElementsToRows = new Dictionary<int, int>();

        private int mouseDownRowIndex = -1;
        private Rectangle dragStartRect = Rectangle.Empty;
        private bool dragStartedHere = false;
        private bool acceptDrop = false;
        private bool dragStoppedHere = false;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContainerControl));
            this.gridContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridContextMenu
            // 
            this.gridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editMenuItem,
            this.toolStripSeparator2,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.toolStripSeparator1,
            this.deleteMenuItem});
            this.gridContextMenu.Name = "contextMenuStrip1";
            resources.ApplyResources(this.gridContextMenu, "gridContextMenu");
            this.gridContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.gridContextMenu_Opening);
            // 
            // editMenuItem
            // 
            this.editMenuItem.Name = "editMenuItem";
            resources.ApplyResources(this.editMenuItem, "editMenuItem");
            this.editMenuItem.Click += new System.EventHandler(this.editMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // cutMenuItem
            // 
            this.cutMenuItem.Name = "cutMenuItem";
            resources.ApplyResources(this.cutMenuItem, "cutMenuItem");
            this.cutMenuItem.Click += new System.EventHandler(this.cutMenuItem_Click);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Name = "copyMenuItem";
            resources.ApplyResources(this.copyMenuItem, "copyMenuItem");
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            // 
            // pasteMenuItem
            // 
            this.pasteMenuItem.Name = "pasteMenuItem";
            resources.ApplyResources(this.pasteMenuItem, "pasteMenuItem");
            this.pasteMenuItem.Click += new System.EventHandler(this.pasteMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Name = "deleteMenuItem";
            resources.ApplyResources(this.deleteMenuItem, "deleteMenuItem");
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // ContainerControl
            // 
            this.Name = "ContainerControl";
            this.gridContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.ComponentModel.IContainer components;

    }

    public class ElementDoubleClickEventArgs : EventArgs
    {
        public Ares.Data.IContainerElement Element { get; private set; }

        public ElementDoubleClickEventArgs(Ares.Data.IContainerElement element)
        {
            Element = element;
        }
    }

    public class ElementImportEventArgs : EventArgs
    {
        public String SerializedData { get; private set; }
        public int InsertionRow { get; private set; }

        public ElementImportEventArgs(String data, int row)
        {
            SerializedData = data;
            InsertionRow = row;
        }
    }

    class GridDataExchangeData
    {
        public List<int> SelectedRows { get; set; }
        public String SerializedData { get; set; }
    }

}
