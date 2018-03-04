/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using System.Threading.Tasks;
using System.Threading;

namespace Ares.Editor.Controls
{
    public class ContainerControl : UserControl, Actions.IContainerDisplay
    {
        public event EventHandler<ElementDoubleClickEventArgs> ElementDoubleClick;

        public event EventHandler<ElementImportEventArgs> ElementsImported;

        public void AddElements(IList<IElement> elements, int insertionRow)
        {
            bool oldListen = listen;
            listen = false;
            int index = ElementsContainer.GetGeneralElements().Count;
            Actions.Actions.Instance.AddNew(new Actions.AddContainerElementsAction(mMassOperationControl, ElementsContainer, elements, insertionRow), m_Project);
            RefillGrid();
            if (Grid.Rows.Count > 0)
            {
                if (insertionRow < 0)
                    Grid.FirstDisplayedScrollingRowIndex = 0;
                else if (insertionRow < Grid.Rows.Count)
                    Grid.FirstDisplayedScrollingRowIndex = insertionRow;
                else
                    Grid.FirstDisplayedScrollingRowIndex = Grid.Rows.Count - 1;
            }
            listen = oldListen;
        }

        public void AddImportedElements(IList<IXmlWritable> elements, int insertionRow)
        {
            bool oldListen = listen;
            listen = false;
            int index = insertionRow;
            Actions.Actions.Instance.AddNew(new Actions.AddImportedContainerElementsAction(mMassOperationControl, ElementsContainer, elements, insertionRow), m_Project);
            RefillGrid();
            if (Grid.Rows.Count > 0)
            {
                if (insertionRow < 0)
                    Grid.FirstDisplayedScrollingRowIndex = 0;
                else if (insertionRow < Grid.Rows.Count)
                    Grid.FirstDisplayedScrollingRowIndex = insertionRow;
                else
                    Grid.FirstDisplayedScrollingRowIndex = Grid.Rows.Count - 1;
            }
            listen = oldListen;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelRefillTask();
                mMassOperationControl.ControlDisposed();
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
        private ToolStripMenuItem effectsMenuItem;
        private ToolStripMenuItem volumeToolStripMenuItem;
        private ToolStripMenuItem balanceToolStripMenuItem;
        private ToolStripMenuItem pitchToolStripMenuItem;
        private ToolStripMenuItem volumedBToolStripMenuItem;
        private ToolStripMenuItem reverbToolStripMenuItem;
        private ToolStripMenuItem speakersToolStripMenuItem;
        private ToolStripMenuItem tempoToolStripMenuItem;

        protected bool listen = true;

        private class MassOperationControl : Actions.IContainerDisplay
        {
            private ContainerControl mControl;

            public MassOperationControl(ContainerControl control)
            {
                mControl = control;
            }

            public void ControlDisposed()
            {
                mControl = null;
            }

            public bool BeginMassAction()
            {
                if (mControl != null) return mControl.BeginMassAction(); else return false;
            }

            public void EndMassAction(bool oldVal)
            {
                if (mControl != null) mControl.EndMassAction(oldVal);
            }
        }

        private MassOperationControl mMassOperationControl;

        protected ContainerControl()
        {
            mMassOperationControl = new MassOperationControl(this);
            Actions.FilesWatcher.Instance.AnyDirChanges += new EventHandler<EventArgs>(FileDirChanges);
            InitializeComponent();
        }

        protected void AttachGridEvents()
        {
            Grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseDown);
            Grid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseMove);
            Grid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseUp);
            Grid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.Grid_RowsRemoved);
            //Grid.ContextMenuStrip = gridContextMenu;
            Grid.CellMouseClick += Grid_CellMouseClick;
        }

        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var rect = Grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                gridContextMenu.Show(Grid, rect.X + e.X, rect.Y + e.Y);
            }
        }

        protected Ares.Data.IProject m_Project;

        protected void ContainerSet(IProject project)
        {
            m_Project = project;
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

        protected int GetElementIndex(DataGridViewRow row)
        {
            if (row.Tag != null)
                return (int)row.Tag;
            else
                // during row creation
                return row.Index;
        }

        private void FillGrid()
        {
            bool oldListen = listen;
            listen = false;
            RefillGrid();
            listen = oldListen;
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
            grid.Rows[row].Tag = row;
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
            else
            {
                grid.Rows[row].Cells[0].Style.ForeColor = Color.Black;
                grid.Rows[row].Cells[0].ErrorText = String.Empty;
            }
        }

        private void Grid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!listen)
                return;
            bool c = CancelRefillTask();
            listen = false;
            List<IElement> remaining = new List<IElement>();
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            foreach (DataGridViewRow row in Grid.Rows)
            {
                remaining.Add(containerElements[GetElementIndex(row)]);
            }
            List<IElement> elements = (new List<IElement>(containerElements)).Except(remaining).ToList();
            Actions.Actions.Instance.AddNew(new Actions.RemoveContainerElementsAction(mMassOperationControl, ElementsContainer, elements,
                            containerElements.IndexOf(elements[0] as IContainerElement)), m_Project);
            listen = true;
            if (c) StartRefillTask();
        }


        private Task<List<Object>> refillGridTask;
        private CancellationTokenSource tokenSource;

        protected bool CancelRefillTask()
        {
            if (refillGridTask != null)
            {
                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                }
                try
                { 
                    refillGridTask.Wait();
                }
                    catch (AggregateException)
                { }
                if (tokenSource != null)
                {
                    tokenSource.Dispose();
                    tokenSource = null;
                }
                refillGridTask = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void WaitForRefillTask()
        {
            if (refillGridTask != null)
            {
                try
                {
                    refillGridTask.Wait();
                }
                catch (AggregateException)
                { }
            }
        }

        protected void StartRefillTask()
        {
            if (HasAdditionalElementData())
            {
                tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                refillGridTask = Task.Factory.StartNew(() =>
                {
                    List<Object> ret = new List<object>();
                    foreach (IContainerElement element in ElementsContainer.GetGeneralElements())
                    {
                        token.ThrowIfCancellationRequested();
                        ret.Add(GetElementData(element));
                    }
                    return ret;
                });
                refillGridTask.ContinueWith((result) =>
                {
                    if (result.Status == TaskStatus.RanToCompletion)
                    {
                        bool cancel = tokenSource.IsCancellationRequested;
                        refillGridTask = null;
                        tokenSource.Dispose();
                        tokenSource = null;
                        if (!cancel)
                        {
                            SetAdditionalGridData(result.Result);
                        }
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        protected void RefillGrid()
        {
            CancelRefillTask();
            StartRefillTask();

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
                foreach (int id in GetInterestingElementIds(element))
                {
                    if (!m_ElementsToRows.ContainsKey(id))
                    {
                        m_ElementsToRows[id] = new List<int>();
                        Actions.ElementChanges.Instance.AddListener(id, Update);
                    }
                    m_ElementsToRows[id].Add(row);
                }
                SetElementAttributes(Grid, element, row);
                ++row;
            }
            Grid.ResumeLayout();
        }

        private void SetAdditionalGridData(List<object> data)
        {
            for (int row = 0; row < data.Count; ++row)
            {
                SetAdditionalElementData(row, data[row]);
            }
        }

        protected virtual IEnumerable<int> GetInterestingElementIds(IContainerElement element)
        {
            yield return element.Id;
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
                Grid.EndEdit();
                List<int> selectedRows = new List<int>();
                foreach (DataGridViewRow row in Grid.SelectedRows) selectedRows.Add(GetElementIndex(row));
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

        public bool PerformDrop(DragEventArgs e, out int row)
        {
            Point clientPoint = Grid.PointToClient(new Point(e.X, e.Y));
            int targetRow = Grid.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            if (targetRow < 0 || targetRow > Grid.RowCount)
            {
                targetRow = Grid.RowCount;
            }
            row = targetRow;
            if (!acceptDrop)
                return false;
            if (dragStartedHere)
            {
                if (e.Effect == DragDropEffects.Move)
                {
                    bool c = CancelRefillTask();
                    MoveRows(((GridDataExchangeData)e.Data.GetData(typeof(GridDataExchangeData))).SelectedRows, GetElementIndex(Grid.Rows[targetRow]));
                    dragStoppedHere = true;
                    if (c) StartRefillTask();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                bool c = CancelRefillTask();
                String serializedData = ((GridDataExchangeData)e.Data.GetData(typeof(GridDataExchangeData))).SerializedData;
                FireElementsImported(serializedData, targetRow);
                StartRefillTask();
                return true;
            }
        }

        protected String SerializeSelectedRows()
        {
            if (Grid.SelectedRows.Count == 0)
                return String.Empty;
            WaitForRefillTask();
            List<IXmlWritable> elements = new List<IXmlWritable>();
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            List<int> selectedIndices = new List<int>();
            foreach (DataGridViewRow row in Grid.SelectedRows)
            {
                selectedIndices.Add(GetElementIndex(row));
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
            WaitForRefillTask();
            bool oldListen = listen;
            listen = false;
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            List<IElement> elements = new List<IElement>();
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            int minIndex = Grid.Rows.Count;
            foreach (DataGridViewRow row in Grid.SelectedRows)
            {
                selectedRows.Add(row);
                int elementIndex = GetElementIndex(row);
                if (elementIndex < minIndex)
                    minIndex = elementIndex;
            }
            foreach (DataGridViewRow row in selectedRows.OrderBy(row => GetElementIndex(row)))
            {
                elements.Add(containerElements[GetElementIndex(row)]);
                rows.Add(row);
            }
            Actions.Actions.Instance.AddNew(new Actions.RemoveContainerElementsAction(mMassOperationControl, ElementsContainer, elements, minIndex), m_Project);
            RefillGrid();
            listen = oldListen;
        }

        private void MoveRows(List<int> rows, int targetIndex)
        {
            bool oldListen = listen;
            listen = false;
            WaitForRefillTask();
            rows.Sort();
            Actions.Actions.Instance.AddNew(new Actions.ReorderContainerElementsAction(ElementsContainer, rows, targetIndex), m_Project);
            RefillGrid();
            listen = oldListen;
        }

        public bool BeginMassAction()
        {
            bool oldVal = listen;
            listen = false;
            return oldVal;
        }

        public void EndMassAction(bool oldVal)
        {
            listen = oldVal;
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (!listen)
                return;
            if (elementID == ElementsContainer.Id && changeType == Actions.ElementChanges.ChangeType.PreRemoved)
            {
                listen = false;
                return;
            }
            else if (elementID == ElementsContainer.Id && changeType == Actions.ElementChanges.ChangeType.Removed)
            {
                listen = true;
                return;
            }
            listen = false;
            CancelRefillTask();
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
                    foreach (int row in m_ElementsToRows[elementID])
                    {
                        ChangeElementDataInGrid(elementID, row);
                    }
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Changed)
                {
                    foreach (int row in m_ElementsToRows[elementID])
                    {
                        ChangeElementDataInGrid(elementID, row);
                        SetElementAttributes(Grid, ElementsContainer.GetGeneralElements()[row], row);
                    }
                }
            }
            listen = true;
        }

        private void editMenuItem_Click(object sender, EventArgs e)
        {
            WaitForRefillTask();
            if (Grid.SelectedRows.Count > 0)
            {
                FireElementDoubleClick(ElementsContainer.GetGeneralElements()[GetElementIndex(Grid.SelectedRows[0])]);
            }
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            WaitForRefillTask();
            RemoveSelectedRows();
        }

        private void pasteMenuItem_Click(object sender, EventArgs e)
        {
            bool c = CancelRefillTask();
            String serializedData = (String)Clipboard.GetData(DataFormats.GetFormat(formatName).Name);
            FireElementsImported(serializedData, Grid.RowCount);
            if (c) StartRefillTask();
        }

        private void cutMenuItem_Click(object sender, EventArgs e)
        {
            WaitForRefillTask();
            CopyRowsToClipboard();
            RemoveSelectedRows();
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            WaitForRefillTask();
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
            speakersToolStripMenuItem.Enabled = enable;
            volumedBToolStripMenuItem.Enabled = enable;
            volumeToolStripMenuItem.Enabled = enable;
            pitchToolStripMenuItem.Enabled = enable;
            tempoToolStripMenuItem.Enabled = enable;
            reverbToolStripMenuItem.Enabled = enable;
            balanceToolStripMenuItem.Enabled = enable;
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

        protected virtual bool HasAdditionalElementData()
        {
            return false;
        }

        protected virtual void SetAdditionalElementData(int row, Object data)
        {
        }

        protected virtual object GetElementData(IContainerElement element)
        {
            return null;
        }

        protected virtual void ChangeElementDataInGrid(int elementID, int row)
        {
        }

        private Dictionary<int, List<int>> m_ElementsToRows = new Dictionary<int, List<int>>();

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
            this.effectsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.balanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pitchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volumedBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reverbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speakersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tempoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.effectsMenuItem,
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
            // effectsMenuItem
            // 
            this.effectsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.volumeToolStripMenuItem,
            this.balanceToolStripMenuItem,
            this.pitchToolStripMenuItem,
            this.volumedBToolStripMenuItem,
            this.reverbToolStripMenuItem,
            this.speakersToolStripMenuItem,
            this.tempoToolStripMenuItem});
            this.effectsMenuItem.Name = "effectsMenuItem";
            resources.ApplyResources(this.effectsMenuItem, "effectsMenuItem");
            // 
            // volumeToolStripMenuItem
            // 
            this.volumeToolStripMenuItem.Name = "volumeToolStripMenuItem";
            resources.ApplyResources(this.volumeToolStripMenuItem, "volumeToolStripMenuItem");
            this.volumeToolStripMenuItem.Click += new System.EventHandler(this.volumeToolStripMenuItem_Click);
            // 
            // balanceToolStripMenuItem
            // 
            this.balanceToolStripMenuItem.Name = "balanceToolStripMenuItem";
            resources.ApplyResources(this.balanceToolStripMenuItem, "balanceToolStripMenuItem");
            this.balanceToolStripMenuItem.Click += new System.EventHandler(this.balanceToolStripMenuItem_Click);
            // 
            // pitchToolStripMenuItem
            // 
            this.pitchToolStripMenuItem.Name = "pitchToolStripMenuItem";
            resources.ApplyResources(this.pitchToolStripMenuItem, "pitchToolStripMenuItem");
            this.pitchToolStripMenuItem.Click += new System.EventHandler(this.pitchToolStripMenuItem_Click);
            // 
            // volumedBToolStripMenuItem
            // 
            this.volumedBToolStripMenuItem.Name = "volumedBToolStripMenuItem";
            resources.ApplyResources(this.volumedBToolStripMenuItem, "volumedBToolStripMenuItem");
            this.volumedBToolStripMenuItem.Click += new System.EventHandler(this.volumedBToolStripMenuItem_Click);
            // 
            // reverbToolStripMenuItem
            // 
            this.reverbToolStripMenuItem.Name = "reverbToolStripMenuItem";
            resources.ApplyResources(this.reverbToolStripMenuItem, "reverbToolStripMenuItem");
            this.reverbToolStripMenuItem.Click += new System.EventHandler(this.reverbToolStripMenuItem_Click);
            // 
            // speakersToolStripMenuItem
            // 
            this.speakersToolStripMenuItem.Name = "speakersToolStripMenuItem";
            resources.ApplyResources(this.speakersToolStripMenuItem, "speakersToolStripMenuItem");
            this.speakersToolStripMenuItem.Click += new System.EventHandler(this.speakersToolStripMenuItem_Click);
            // 
            // tempoToolStripMenuItem
            // 
            this.tempoToolStripMenuItem.Name = "tempoToolStripMenuItem";
            resources.ApplyResources(this.tempoToolStripMenuItem, "tempoToolStripMenuItem");
            this.tempoToolStripMenuItem.Click += new System.EventHandler(this.tempoToolStripMenuItem_Click);
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

        private IList<IFileElement> GetAllSelectedFileElements()
        {
            List<IXmlWritable> elements = new List<IXmlWritable>();
            IList<IContainerElement> containerElements = ElementsContainer.GetGeneralElements();
            List<int> selectedIndices = new List<int>();
            foreach (DataGridViewRow row in Grid.SelectedRows)
            {
                elements.Add(containerElements[GetElementIndex(row)]);
            }
            Ares.ModelInfo.FileLists fileLists = new ModelInfo.FileLists(ModelInfo.DuplicateRemoval.None);
            return fileLists.GetAllFiles(elements);
        }

        private void volumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = GetAllSelectedFileElements();
            if (list.Count == 0) return;
            var convertedList = new List<IEffectsElement>();
            convertedList.AddRange(list);
            FileVolumeDialog dialog = new FileVolumeDialog(convertedList, m_Project);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action, m_Project);
            }
        }

        private void balanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = GetAllSelectedFileElements();
            if (list.Count == 0) return;
            BalanceDialog dialog = new BalanceDialog(list, m_Project);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action, m_Project);
            }
        }

        private void pitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = GetAllSelectedFileElements();
            if (list.Count == 0) return;
            PitchDialog dialog = new PitchDialog(list, m_Project);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action, m_Project);
            }
        }

        private void volumedBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = GetAllSelectedFileElements();
            if (list.Count == 0) return;
            VolumeDBDialog dialog = new VolumeDBDialog(list, m_Project);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action, m_Project);
            }
        }

        private void reverbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = GetAllSelectedFileElements();
            if (list.Count == 0) return;
            ReverbDialog dialog = new ReverbDialog(list, m_Project);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action, m_Project);
            }
        }

        private void speakersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = GetAllSelectedFileElements();
            if (list.Count == 0) return;
            SpeakersDialog dialog = new SpeakersDialog(list, m_Project);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action, m_Project);
            }
        }

        private void tempoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = GetAllSelectedFileElements();
            if (list.Count == 0) return;
            TempoDialog dialog = new TempoDialog(list, m_Project);
            if (dialog.ShowDialog(Parent) == DialogResult.OK)
            {
                dialog.UpdateAction();
                Actions.Actions.Instance.AddNew(dialog.Action, m_Project);
            }
        }

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
