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
    public partial class SequentialContainerControl : UserControl
    {
        public SequentialContainerControl()
        {
            InitializeComponent();
        }

        public void SetContainer(IElementContainer<ISequentialElement> container)
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
                foreach (ISequentialElement element in m_Container.GetElements())
                {
                    elementsGrid.Rows.Add(new object[] { element.Title, element.FixedStartDelay.TotalMilliseconds, element.MaximumRandomStartDelay.TotalMilliseconds });
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
                        (m_Container.GetElement(elementID)).FixedStartDelay.TotalMilliseconds;
                    elementsGrid.Rows[m_ElementsToRows[elementID]].Cells[2].Value =
                        (m_Container.GetElement(elementID)).MaximumRandomStartDelay.TotalMilliseconds;
                }
            }
            listen = true;
        }

        public void AddElements(IList<IElement> elements)
        {
            listen = false;
            int index = m_Container.GetElements().Count;
            Actions.Actions.Instance.AddNew(new Actions.AddContainerElementsAction(m_Container, elements));
            IList<ISequentialElement> containerElements = m_Container.GetElements();
            for (int i = index; i < containerElements.Count; ++i)
            {
                elementsGrid.Rows.Add(new object[] { containerElements[i].Title, containerElements[i].FixedStartDelay.TotalMilliseconds, containerElements[i].MaximumRandomStartDelay.TotalMilliseconds });
                if (containerElements[i].InnerElement is IFileElement)
                {
                    elementsGrid.Rows[index].Cells[0].ToolTipText = (containerElements[i].InnerElement as IFileElement).FilePath;
                }
                m_ElementsToRows[containerElements[i].Id] = index;
            }
            listen = true;
        }

        private IElementContainer<ISequentialElement> m_Container;
        private Dictionary<int, int> m_ElementsToRows = new Dictionary<int, int>();
        private bool listen = true;

        private void elementsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            listen = false;
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                int fixedDelay = Convert.ToInt32(elementsGrid.Rows[e.RowIndex].Cells[1].Value);
                int randomDelay = Convert.ToInt32(elementsGrid.Rows[e.RowIndex].Cells[2].Value);
                Actions.Actions.Instance.AddNew(new Actions.SequentialElementChangeAction(
                    m_Container.GetElements()[e.RowIndex], fixedDelay, randomDelay));
            }
            listen = true;
        }

        private void elementsGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!listen)
                return;
            listen = false;
            List<IElement> elements = new List<IElement>();
            IList<ISequentialElement> containerElements = m_Container.GetElements();
            for (int i = 0; i < e.RowCount; ++i)
            {
                elements.Add(containerElements[e.RowIndex + i]);
            }
            Actions.Actions.Instance.AddNew(new Actions.RemoveContainerElementsAction(m_Container, elements, e.RowIndex));
            listen = true;
        }
    }
}
