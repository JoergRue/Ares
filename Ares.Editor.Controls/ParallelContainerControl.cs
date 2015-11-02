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

namespace Ares.Editor.Controls
{
    public partial class ParallelContainerControl : ContainerControl
    {
        public ParallelContainerControl()
        {
            InitializeComponent();
            AttachGridEvents();
        }

        public void SetContainer(IElementContainer<IParallelElement> container, IProject project)
        {
            m_Container = container;
            ContainerSet(project);
        }

        public event EventHandler ActiveRowChanged;

        public IParallelElement ActiveElement
        {
            get
            {
                if (m_Container.GetElements().Count > 0 && elementsGrid.CurrentRow != null)
                {
                    return m_Container.GetElements()[elementsGrid.CurrentRow.Index];
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void AddElementToGrid(IContainerElement element)
        {
            elementsGrid.Rows.Add(new object[] { element.Title });
        }

        protected override void ChangeElementDataInGrid(int elementID, int row)
        {
            ActiveRowChanged(this, new EventArgs());            
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

        private IElementContainer<IParallelElement> m_Container;
        private Dictionary<int, int> m_ElementsToRows = new Dictionary<int, int>();

        private void elementsGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            ActiveRowChanged(this, new EventArgs());
        }

        private void elementsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FireElementDoubleClick(m_Container.GetElements()[e.RowIndex]);
        }
    
    }
}
