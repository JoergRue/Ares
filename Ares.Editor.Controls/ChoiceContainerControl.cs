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
            AttachGridEvents();
        }

        public void SetContainer(IElementContainer<IChoiceElement> container)
        {
            m_Container = container;
            ContainerSet();
        }

        private IElementContainer<IChoiceElement> m_Container;

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

        protected override void AddElementToGrid(IContainerElement element)
        {
            elementsGrid.Rows.Add(new object[] { element.Title, ((IChoiceElement)element).RandomChance });
        }

        protected override void ChangeElementDataInGrid(int elementID, int row)
        {
            elementsGrid.Rows[row].Cells[0].Value =
                Ares.Data.DataModule.ElementRepository.GetElement(elementID).Title;
            elementsGrid.Rows[row].Cells[1].Value =
                (m_Container.GetElement(elementID)).RandomChance;
        }

        private void elementsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
            {
                FireElementDoubleClick(m_Container.GetElements()[e.RowIndex]);
            }
            else
            {
                elementsGrid.BeginEdit(true);
            }
        }

    }
}
