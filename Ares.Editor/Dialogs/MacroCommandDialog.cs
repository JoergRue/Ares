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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Ares.Data;

namespace Ares.Editor.Dialogs
{
    public partial class MacroCommandDialog : Form
    {
        public MacroCommandDialog()
        {
            InitializeComponent();
        }

        private IProject m_Project;
        private IMacro m_ContainingMacro;
        private IModeElement m_CurrentCommandElement;
        private IModeElement m_CurrentConditionElement;

        public void SetData(IMacroCommand macroCommand, IProject project, IMacro containingMacro)
        {
            m_Project = project;
            m_ContainingMacro = containingMacro;
            waitUnitCombo.SelectedIndex = 0;
            if (macroCommand != null)
            {
                switch (macroCommand.CommandType)
                {
                    case MacroCommandType.StartElement:
                        commandTypeCombo.SelectedIndex = 0;
                        m_CurrentCommandElement = ((IStartCommand)macroCommand).StartedElement;
                        commandElementBox.Text = GetElementDisplayName(m_CurrentCommandElement, m_Project);
                        break;
                    case MacroCommandType.StopElement:
                        commandTypeCombo.SelectedIndex = 1;
                        m_CurrentCommandElement = ((IStopCommand)macroCommand).StoppedElement;
                        commandElementBox.Text = GetElementDisplayName(m_CurrentCommandElement, m_Project);
                        break;
                    case MacroCommandType.WaitCondition:
                        commandTypeCombo.SelectedIndex = ((IWaitConditionCommand)macroCommand).AwaitedCondition.ConditionType == MacroConditionType.ElementRunning ? 2 : 3;
                        m_CurrentCommandElement = ((IWaitConditionCommand)macroCommand).AwaitedCondition.Conditional;
                        commandElementBox.Text = GetElementDisplayName(m_CurrentCommandElement, m_Project);
                        break;
                    case MacroCommandType.WaitTime:
                        commandTypeCombo.SelectedIndex = 4;
                        m_CurrentCommandElement = null;
                        commandElementBox.Text = String.Empty;
                        waitTimeUpDown.Value = ((IWaitTimeCommand)macroCommand).TimeInMillis;
                        break;
                    default:
                        commandTypeCombo.SelectedIndex = 0;
                        m_CurrentCommandElement = null;
                        commandElementBox.Text = GetElementDisplayName(m_CurrentCommandElement, m_Project);
                        break;
                }
                m_CurrentConditionElement = macroCommand.Condition.Conditional;
                switch (macroCommand.Condition.ConditionType)
                {
                    case MacroConditionType.ElementRunning:
                        conditionCombo.SelectedIndex = 1;
                        conditionElementBox.Text = GetElementDisplayName(m_CurrentConditionElement, m_Project);
                        break;
                    case MacroConditionType.ElementNotRunning:
                        conditionCombo.SelectedIndex = 2;
                        conditionElementBox.Text = GetElementDisplayName(m_CurrentConditionElement, m_Project);
                        break;
                    case MacroConditionType.None:
                        conditionCombo.SelectedIndex = 0;
                        conditionElementBox.Text = String.Empty;
                        break;
                    default:
                        conditionCombo.SelectedIndex = 0;
                        conditionElementBox.Text = String.Empty;
                        break;
                }
            }
            else
            {
                commandTypeCombo.SelectedIndex = 0;
                commandElementBox.Text = String.Empty;
                conditionCombo.SelectedIndex = 0;
                conditionElementBox.Text = String.Empty;
            }
        }

        private enum TimeUnit { MilliSeconds, Seconds, Minutes };

        private TimeUnit m_CurrentUnit = TimeUnit.MilliSeconds;

        private void conditionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectConditionElementButton.Enabled = conditionCombo.SelectedIndex > 0;
            if (!String.IsNullOrEmpty(errorProvider.GetError(conditionElementBox)) && conditionCombo.SelectedIndex == 0)
            {
                errorProvider.SetError(conditionElementBox, String.Empty);
            }
        }

        private void commandTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasElement = commandTypeCombo.SelectedIndex < 4;
            selectCommandElementButton.Enabled = hasElement;
            commandElementBox.Enabled = hasElement;
            if (!hasElement)
            {
                commandElementBox.Text = String.Empty;
            }
            waitTimeUpDown.Enabled = !hasElement;
            if (!String.IsNullOrEmpty(errorProvider.GetError(commandElementBox)) && !hasElement)
            {
                errorProvider.SetError(commandElementBox, String.Empty);
            }
        }

        private void waitUnitCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimeUnit oldUnit = m_CurrentUnit;
            m_CurrentUnit = (TimeUnit)waitUnitCombo.SelectedIndex;
            if (m_CurrentUnit != oldUnit)
            {
                int currentTime = (int)waitTimeUpDown.Value;
                int[] factors = { 1, 1000, 60000 };
                int newTime = currentTime * factors[(int)oldUnit] / factors[(int)m_CurrentUnit];
                waitTimeUpDown.Value = newTime;
            }
        }


        private static String GetElementDisplayName(IModeElement modeElement, IProject project)
        {
            if (modeElement == null)
            {
                return StringResources.InvalidModeElement;
            }
            IMode mode = FindMode(modeElement, project);
            return String.Format(StringResources.ModeElementDisplay, mode != null ? mode.Title : StringResources.InvalidModeElement, modeElement.Title);
        }

        private static IMode FindMode(IModeElement modeElement, IProject project)
        {
            foreach (IMode mode in project.GetModes())
            {
                foreach (IModeElement element in mode.GetElements())
                {
                    if (element == modeElement)
                        return mode;
                }
            }
            return null;
        }

        public IMacroCommand MacroCommand { get; set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            bool needsCommandElement = commandTypeCombo.SelectedIndex < 4;
            if (needsCommandElement && m_CurrentCommandElement == null)
            {
                errorProvider.SetError(commandElementBox, StringResources.PleaseSelectElement);
                hasError = true;
            }
            bool needsConditionElement = conditionCombo.SelectedIndex > 0;
            if (needsConditionElement && m_CurrentConditionElement == null)
            {
                errorProvider.SetError(conditionElementBox, StringResources.PleaseSelectElement);
                hasError = true;
            }
            if (hasError)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, StringResources.InsufficientCommandData, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MacroConditionType conditionType = MacroConditionType.None;
            switch (conditionCombo.SelectedIndex)
            {
                case 1:
                    conditionType = MacroConditionType.ElementRunning;
                    break;
                case 2:
                    conditionType = MacroConditionType.ElementNotRunning;
                    break;
                case 0:
                    conditionType = MacroConditionType.None;
                    break;
                default:
                    break;
            }
            switch (commandTypeCombo.SelectedIndex)
            {
                case 0:
                    MacroCommand = DataModule.MacroFactory.CreateStartCommand(m_CurrentCommandElement, conditionType, m_CurrentConditionElement);
                    break;
                case 1:
                    MacroCommand = DataModule.MacroFactory.CreateStopCommand(m_CurrentCommandElement, conditionType, m_CurrentConditionElement);
                    break;
                case 2:
                    MacroCommand = DataModule.MacroFactory.CreateWaitCondition(MacroConditionType.ElementRunning, m_CurrentCommandElement, conditionType, m_CurrentConditionElement);
                    break;
                case 3:
                    MacroCommand = DataModule.MacroFactory.CreateWaitCondition(MacroConditionType.ElementNotRunning, m_CurrentCommandElement, conditionType, m_CurrentConditionElement);
                    break;
                case 4:
                    int[] factors = { 1, 1000, 60000 };
                    int time = (int)waitTimeUpDown.Value * factors[(int)m_CurrentUnit];
                    MacroCommand = DataModule.MacroFactory.CreateWaitTime(time, conditionType, m_CurrentConditionElement);
                    break;
                default:
                    DialogResult = DialogResult.None;
                    break;
            }
        }

        private void selectCommandElementButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ContextMenu menu = CreateElementSelectionMenu((IModeElement element) => 
                {
                    m_CurrentCommandElement = element;
                    commandElementBox.Text = GetElementDisplayName(element, m_Project);
                    if (element != null && !String.IsNullOrEmpty(errorProvider.GetError(commandElementBox)))
                    {
                        errorProvider.SetError(commandElementBox, String.Empty);
                    }
                });
            menu.Show(commandGroupBox, new Point(selectCommandElementButton.Left, selectCommandElementButton.Bottom));
        }

        private class ElementSelectionHandler
        {
            public ElementSelectionHandler(Action<IModeElement> selectionAction, IModeElement modeElement)
            {
                m_SelectionAction = selectionAction;
                m_ModeElement = modeElement;
            }

            public void HandleEvent(Object o, EventArgs args)
            {
                m_SelectionAction(m_ModeElement);
            }

            private Action<IModeElement> m_SelectionAction;
            private IModeElement m_ModeElement;
        }

        private ContextMenu CreateElementSelectionMenu(Action<IModeElement> selectionAction)
        {
            ContextMenu menu = new ContextMenu();
            foreach (IMode mode in m_Project.GetModes())
            {
                MenuItem menuItem = new MenuItem(mode.Title);
                foreach (IModeElement modeElement in mode.GetElements())
                {
                    if (modeElement.StartElement == m_ContainingMacro)
                    {
                        continue;
                    }
                    ElementSelectionHandler handler = new ElementSelectionHandler(selectionAction, modeElement);
                    MenuItem elementItem = new MenuItem(modeElement.Title, new EventHandler(handler.HandleEvent));
                    menuItem.MenuItems.Add(elementItem);
                }
                menu.MenuItems.Add(menuItem);
            }
            return menu;
        }

        private void selectConditionElementButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ContextMenu menu = CreateElementSelectionMenu((IModeElement element) =>
            {
                m_CurrentConditionElement = element;
                conditionElementBox.Text = GetElementDisplayName(element, m_Project);
                if (element != null && !String.IsNullOrEmpty(errorProvider.GetError(conditionElementBox)))
                {
                    errorProvider.SetError(conditionElementBox, String.Empty);
                }
            });
            menu.Show(conditionGroupBox, new Point(selectConditionElementButton.Left, selectConditionElementButton.Bottom));
        }
    }
}
