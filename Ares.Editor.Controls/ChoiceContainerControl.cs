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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ares.Data;

namespace Ares.Editor.ElementEditorControls
{
    public partial class ChoiceContainerControl : Ares.Editor.Controls.ContainerControl
    {
        public ChoiceContainerControl()
        {
            InitializeComponent();
        }

        public void SetContainer(IElementContainer<IChoiceElement> container)
        {
            m_Container = container;
            Update(m_Container.Id, Actions.ElementChanges.ChangeType.Changed);
            Actions.ElementChanges.Instance.AddListener(m_Container.Id, Update);
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (!listen)
                return;
            listen = false;
            if (elementID == m_Container.Id && changeType == Actions.ElementChanges.ChangeType.Changed)
            {
                elementsGrid.SuspendLayout();
                elementsGrid.Rows.Clear();
                foreach (int key in m_ElementsToRows.Keys)
                {
                    Actions.ElementChanges.Instance.RemoveListener(key, Update);
                }
                m_ElementsToRows.Clear();
                int row = 0;
                foreach (IChoiceElement element in m_Container.GetElements())
                {
                    elementsGrid.Rows.Add(new object[] { element.Title, element.RandomChance });
                    if (element.InnerElement is IFileElement)
                    {
                        elementsGrid.Rows[row].Cells[0].ToolTipText = (element.InnerElement as IFileElement).FilePath;
                    }
                    m_ElementsToRows[element.Id] = row;
                    Actions.ElementChanges.Instance.AddListener(element.Id, Update);
                    
                    ++row;
                }
                elementsGrid.ResumeLayout();
            }
            else if (m_ElementsToRows.ContainsKey(elementID))
            {
                if (changeType == Actions.ElementChanges.ChangeType.Removed)
                {
                    Actions.ElementChanges.Instance.RemoveListener(elementID, Update);
                    elementsGrid.Rows.RemoveAt(m_ElementsToRows[elementID]);
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Renamed)
                {
                    elementsGrid.Rows[m_ElementsToRows[elementID]].Cells[0].Value =
                        Ares.Data.DataModule.ElementRepository.GetElement(elementID).Title;
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Changed)
                {
                    elementsGrid.Rows[m_ElementsToRows[elementID]].Cells[1].Value =
                        (m_Container.GetElement(elementID)).RandomChance;
                }
            }
            listen = true;
        }

        public void AddElements(IList<IElement> elements)
        {
            listen = false;
            int index = m_Container.GetElements().Count;
            Actions.Actions.Instance.AddNew(new Actions.AddContainerElementsAction(m_Container, elements));
            IList<IChoiceElement> containerElements = m_Container.GetElements();
            for (int i = index; i < containerElements.Count; ++i)
            {
                elementsGrid.Rows.Add(new object[] { containerElements[i].Title, containerElements[i].RandomChance });
                if (containerElements[i].InnerElement is IFileElement)
                {
                    elementsGrid.Rows[index].Cells[0].ToolTipText = (containerElements[i].InnerElement as IFileElement).FilePath;
                }
                m_ElementsToRows[containerElements[i].Id] = index;
            }
            listen = true;
        }

        private IElementContainer<IChoiceElement> m_Container;
        private Dictionary<int, int> m_ElementsToRows = new Dictionary<int, int>();
        private bool listen = true;

        private void elementsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            listen = false;
            if (e.ColumnIndex == 1)
            {
                int chance = Convert.ToInt32(elementsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                Actions.Actions.Instance.AddNew(new Actions.ChoiceElementChangeAction(
                    m_Container.GetElements()[e.RowIndex], chance));
            }
            listen = true;
        }

        private void elementsGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!listen)
                return;
            listen = false;
            List<IElement> elements = new List<IElement>();
            IList<IChoiceElement> containerElements = m_Container.GetElements();
            for (int i = 0; i < e.RowCount; ++i)
            {
                elements.Add(containerElements[e.RowIndex + i]);
            }
            Actions.Actions.Instance.AddNew(new Actions.RemoveContainerElementsAction(m_Container, elements, e.RowIndex));
            listen = true;
        }

        private void elementsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FireElementDoubleClick(m_Container.GetElements()[e.RowIndex]);
        }
    }
}
