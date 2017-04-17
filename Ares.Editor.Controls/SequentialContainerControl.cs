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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Data;
using Ares.ModelInfo;

namespace Ares.Editor.Controls
{
    public partial class SequentialContainerControl : ContainerControl
    {
        private bool mHasMusicColumns = false;

        public SequentialContainerControl()
        {
            InitializeComponent();
            AttachGridEvents();
        }

        public void SetContainer(ISequentialContainer container, Ares.Data.IProject project, bool hasMusicColumns)
        {
            m_Container = container;
            mHasMusicColumns = hasMusicColumns;
            if (mHasMusicColumns)
            {
                artistColumn.Visible = container.ShowArtistColumn;
                albumColumn.Visible = container.ShowAlbumColumn;
            }
            ContainerSet(project);
            EnableUpDownButtons();
        }

        protected override void AddElementToGrid(IContainerElement element)
        {
            ISequentialElement e = (ISequentialElement)element;
            elementsGrid.Rows.Add(new object[] { e.Title, string.Empty, string.Empty, e.FixedStartDelay.TotalMilliseconds, e.MaximumRandomStartDelay.TotalMilliseconds });
        }

        protected override bool HasAdditionalElementData()
        {
            return mHasMusicColumns;
        }

        protected override void SetAdditionalElementData(int row, object data)
        {
            var musicFileInfo = (MusicFileInfo)data;
            if (musicFileInfo != null)
            {
                elementsGrid.Rows[row].Cells[1].Value = musicFileInfo.Artist;
                elementsGrid.Rows[row].Cells[2].Value = musicFileInfo.Album;
            }
        }

        protected override object GetElementData(IContainerElement element)
        {
            return mHasMusicColumns ? element.GetMusicFileInfo() : null;
        }

        protected override void ChangeElementDataInGrid(int elementID, int row)
        {
            var element = Ares.Data.DataModule.ElementRepository.GetElement(elementID);
            elementsGrid.Rows[row].Cells[0].Value = element.Title;
            if (mHasMusicColumns)
            {
                var musicFileInfo = element.GetMusicFileInfo();
                elementsGrid.Rows[row].Cells[1].Value = musicFileInfo != null ? (musicFileInfo.Artist != null ? musicFileInfo.Artist : string.Empty) : string.Empty;
                elementsGrid.Rows[row].Cells[2].Value = musicFileInfo != null ? (musicFileInfo.Album != null ? musicFileInfo.Album : string.Empty) : string.Empty;
            }
            elementsGrid.Rows[row].Cells[3].Value =
                (m_Container.GetElement(elementID)).FixedStartDelay.TotalMilliseconds;
            elementsGrid.Rows[row].Cells[4].Value =
                (m_Container.GetElement(elementID)).MaximumRandomStartDelay.TotalMilliseconds;
        }

        protected override DataGridView Grid
        {
            get
            {
                return elementsGrid;
            }
        }

        protected override IGeneralElementContainer ElementsContainer
        {
            get
            {
                return m_Container;
            }
        }

        private ISequentialContainer m_Container;
        private Dictionary<int, int> m_ElementsToRows = new Dictionary<int, int>();

        private void elementsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool oldListen = listen;
            listen = false;
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                int fixedDelay = Convert.ToInt32(elementsGrid.Rows[e.RowIndex].Cells[1].Value);
                int randomDelay = Convert.ToInt32(elementsGrid.Rows[e.RowIndex].Cells[2].Value);
                Actions.Actions.Instance.AddNew(new Actions.SequentialElementChangeAction(
                    m_Container.GetElements()[GetElementIndex(elementsGrid.Rows[e.RowIndex])], fixedDelay, randomDelay), m_Project);
            }
            listen = oldListen;
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            WaitForRefillTask();
            List<int> indices = new List<int>();
            for (int i = 0; i < elementsGrid.SelectedRows.Count; ++i)
            {
                indices.Add(GetElementIndex(elementsGrid.SelectedRows[i]));
            }
            Actions.Actions.Instance.AddNew(new Actions.ReorderElementsAction<ISequentialElement>(m_Container, indices, -1), m_Project);
            // note: the action modified the list
            elementsGrid.ClearSelection();
            for (int i = 0; i < indices.Count; ++i)
            {
                elementsGrid.Rows[indices[i]].Selected = true;                
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            WaitForRefillTask();
            List<int> indices = new List<int>();
            for (int i = 0; i < elementsGrid.SelectedRows.Count; ++i)
            {
                indices.Add(GetElementIndex(elementsGrid.SelectedRows[i]));
            }
            Actions.Actions.Instance.AddNew(new Actions.ReorderElementsAction<ISequentialElement>(m_Container, indices, 1), m_Project);
            // note: the action modified the list
            elementsGrid.ClearSelection();
            for (int i = 0; i < indices.Count; ++i)
            {
                elementsGrid.Rows[indices[i]].Selected = true;
            }
        }

        private void elementsGrid_SelectionChanged(object sender, EventArgs e)
        {
            EnableUpDownButtons();
        }

        private void EnableUpDownButtons()
        {
            upButton.Enabled = elementsGrid.Rows.Count > 0 && elementsGrid.SelectedRows.Count > 0 && !elementsGrid.Rows[0].Selected;
            downButton.Enabled = elementsGrid.Rows.Count > 0 && elementsGrid.SelectedRows.Count > 0 && !elementsGrid.Rows[elementsGrid.Rows.Count - 1].Selected;
        }

        private void elementsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            WaitForRefillTask();
            FireElementDoubleClick(m_Container.GetElements()[GetElementIndex(elementsGrid.Rows[e.RowIndex])]);
        }

        private void elementsGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (mHasMusicColumns && e.Button == MouseButtons.Right)
            {
                albumToolStripMenuItem.Checked = albumColumn.Visible;
                artistToolStripMenuItem.Checked = artistColumn.Visible;
                var rect = elementsGrid.GetCellDisplayRectangle(e.ColumnIndex, -1, false);
                columnMenu.Show(elementsGrid, rect.X + e.X, rect.Y + e.Y);
            }
        }

        private void albumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleColumn(albumColumn, albumToolStripMenuItem.Checked);
            if (m_Container != null)
                m_Container.ShowAlbumColumn = albumToolStripMenuItem.Checked;
            if (m_Project != null)
                m_Project.Changed = true;
        }

        private void artistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleColumn(artistColumn, artistToolStripMenuItem.Checked);
            if (m_Container != null)
                m_Container.ShowArtistColumn = artistToolStripMenuItem.Checked;
            if (m_Project != null)
                m_Project.Changed = true;
        }

        private void ToggleColumn(DataGridViewTextBoxColumn column, bool @checked)
        {
            column.Visible = @checked;
        }
    }
}
