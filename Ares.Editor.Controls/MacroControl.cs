/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
    public partial class MacroControl : ContainerControl
    {
        public MacroControl()
        {
            InitializeComponent();
            AttachGridEvents();
        }

        public void SetContainer(IMacro container)
        {
            m_Container = container;
            ContainerSet();
            EnableUpDownButtons();
        }

        protected override void AddElementToGrid(IContainerElement element)
        {
            IMacroCommand c = (IMacroCommand)element.InnerElement;
            elementsGrid.Rows.Add(new object[] { c.DisplayDescription(), c.Condition.DisplayDescription() });
        }

        protected override void ChangeElementDataInGrid(int elementID, int row)
        {
            IMacroCommand c = (IMacroCommand)m_Container.GetElements()[row].InnerElement;
            elementsGrid.Rows[row].Cells[0].Value = c.DisplayDescription();
            elementsGrid.Rows[row].Cells[1].Value = c.Condition.DisplayDescription();
        }

        protected override IEnumerable<int> GetInterestingElementIds(IContainerElement element)
        {
            IMacroCommand c = element.InnerElement as IMacroCommand;
            yield return c.Id;
            if (c.Condition != null)
                yield return c.Condition.ConditionalId;
            IWaitConditionCommand wc = c as IWaitConditionCommand;
            if (wc != null && wc.AwaitedCondition != null)
                yield return wc.AwaitedCondition.ConditionalId;
            IStartCommand sc = c as IStartCommand;
            if (sc != null)
                yield return sc.StartedElementId;
            IStopCommand sc2 = c as IStopCommand;
            if (sc2 != null)
                yield return sc2.StoppedElementId;
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

        private IMacro m_Container;
        private Dictionary<int, int> m_ElementsToRows = new Dictionary<int, int>();

        private void upButton_Click(object sender, EventArgs e)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < elementsGrid.SelectedRows.Count; ++i)
            {
                indices.Add(elementsGrid.SelectedRows[i].Index);
            }
            Actions.Actions.Instance.AddNew(new Actions.ReorderElementsAction<IMacroElement>(m_Container, indices, -1));
            // note: the action modified the list
            elementsGrid.ClearSelection();
            for (int i = 0; i < indices.Count; ++i)
            {
                elementsGrid.Rows[indices[i]].Selected = true;                
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < elementsGrid.SelectedRows.Count; ++i)
            {
                indices.Add(elementsGrid.SelectedRows[i].Index);
            }
            Actions.Actions.Instance.AddNew(new Actions.ReorderElementsAction<IMacroElement>(m_Container, indices, 1));
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
            deleteButton.Enabled = elementsGrid.Rows.Count > 0 && elementsGrid.SelectedRows.Count > 0;
            editButton.Enabled = elementsGrid.Rows.Count > 0 && elementsGrid.SelectedRows.Count == 1;
        }

        private void EnableUpDownButtons()
        {
            upButton.Enabled = elementsGrid.Rows.Count > 0 && elementsGrid.SelectedRows.Count > 0 && !elementsGrid.Rows[0].Selected;
            downButton.Enabled = elementsGrid.Rows.Count > 0 && elementsGrid.SelectedRows.Count > 0 && !elementsGrid.Rows[elementsGrid.Rows.Count - 1].Selected;
        }

        private void elementsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FireElementDoubleClick(m_Container.GetElements()[e.RowIndex]);
        }

        public void EnableStopButton(bool enable)
        {
            // stopButton.Enabled = enable;
        }

        public event EventHandler<EventArgs> PlayButtonClick;
        public event EventHandler<EventArgs> StopButtonClick;
        public event EventHandler<EventArgs> AddButtonClick;

        private void playButton_Click(object sender, EventArgs e)
        {
            FirePlayButtonClick(e);
        }

        private void FirePlayButtonClick(EventArgs e)
        {
            if (PlayButtonClick != null)
                PlayButtonClick(this, e);
        }

        private void FireStopButtonClick(EventArgs e)
        {
            if (StopButtonClick != null)
                StopButtonClick(this, e);
        }

        private void FireAddButtonClick(EventArgs e)
        {
            if (AddButtonClick != null)
                AddButtonClick(this, e);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            FireStopButtonClick(e);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            FireAddButtonClick(e);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedRows();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (Grid.SelectedRows.Count == 1)
            {
                FireElementDoubleClick(m_Container.GetElements()[Grid.SelectedRows[0].Index]);
            }
        }
    }

    public static class MacroDisplay
    {
        public static String DisplayDescription(this IMacroCommand command)
        {
            switch (command.CommandType)
            {
                case MacroCommandType.StartElement:
                    return String.Format(StringResources.StartCommand, GetElementDisplayName(((IStartCommand)command).StartedElement));
                case MacroCommandType.StopElement:
                    return String.Format(StringResources.StopCommand, GetElementDisplayName(((IStopCommand)command).StoppedElement));
                case MacroCommandType.WaitTime:
                    return String.Format(StringResources.WaitTimeCommand, ((IWaitTimeCommand)command).TimeInMillis.ToString(System.Globalization.CultureInfo.CurrentUICulture));
                case MacroCommandType.WaitCondition:
                    {
                        IWaitConditionCommand wcc = (IWaitConditionCommand)command;
                        switch (wcc.AwaitedCondition.ConditionType)
                        {
                            case MacroConditionType.ElementNotRunning:
                                return String.Format(StringResources.WaitElementNotRunningCommand, GetElementDisplayName(wcc.AwaitedCondition.Conditional));
                            case MacroConditionType.ElementRunning:
                                return String.Format(StringResources.WaitElementRunningCommand, GetElementDisplayName(wcc.AwaitedCondition.Conditional));
                            default:
                                return StringResources.NoCondition;
                        }
                    }
                default:
                    return String.Empty;
            }
        }

        public static String DisplayDescription(this IMacroCondition condition)
        {
            switch (condition.ConditionType)
            {
                case MacroConditionType.ElementRunning:
                    return String.Format(StringResources.ElementRunningCondition, GetElementDisplayName(condition.Conditional));
                case MacroConditionType.ElementNotRunning:
                    return String.Format(StringResources.ElementNotRunningCondition, GetElementDisplayName(condition.Conditional));
                default:
                    return StringResources.NoCondition;
            }
        }

        private static String GetElementDisplayName(IModeElement modeElement)
        {
            if (modeElement == null)
            {
                return StringResources.InvalidModeElement;
            }
            IMode mode = FindMode(modeElement);
            return String.Format(StringResources.ModeElementDisplay, mode != null ? mode.Title : StringResources.InvalidModeElement, modeElement.Title);
        }

        private static IMode FindMode(IModeElement modeElement)
        {
            foreach (IMode mode in ModelInfo.ModelChecks.Instance.Project.GetModes())
            {
                foreach (IModeElement element in mode.GetElements())
                {
                    if (element == modeElement)
                        return mode;
                }
            }
            return null;
        }
    }
}
