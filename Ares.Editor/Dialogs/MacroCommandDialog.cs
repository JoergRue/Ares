/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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

        private int m_CurrentTagId = -1;
        private int m_CurrentCategoryId = -1;

        public void SetData(IMacroCommand macroCommand, IProject project, IMacro containingMacro)
        {
            m_Project = project;
            m_ContainingMacro = containingMacro;
            waitUnitCombo.SelectedIndex = 0;
            m_CurrentTagId = -1;
            m_CurrentCategoryId = -1;
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
                    case MacroCommandType.AddTag:
                        commandTypeCombo.SelectedIndex = 5;
                        m_CurrentCommandElement = null;
                        commandElementBox.Text = GetTagName(macroCommand as ITagCommand, m_Project);
                        m_CurrentCategoryId = (macroCommand as ITagCommand).CategoryId;
                        m_CurrentTagId = (macroCommand as ITagCommand).TagId;
                        break;
                    case MacroCommandType.RemoveTag:
                        commandTypeCombo.SelectedIndex = 6;
                        m_CurrentCommandElement = null;
                        commandElementBox.Text = GetTagName(macroCommand as ITagCommand, m_Project);
                        m_CurrentCategoryId = (macroCommand as ITagCommand).CategoryId;
                        m_CurrentTagId = (macroCommand as ITagCommand).TagId;
                        break;
                    case MacroCommandType.RemoveAllTags:
                        commandTypeCombo.SelectedIndex = 7;
                        m_CurrentCommandElement = null;
                        commandElementBox.Text = String.Empty;
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
            bool hasElement = commandTypeCombo.SelectedIndex < 4 || commandTypeCombo.SelectedIndex == 5 || commandTypeCombo.SelectedIndex == 6;
            selectCommandElementButton.Enabled = hasElement;
            commandElementBox.Enabled = hasElement;
            if (!hasElement)
            {
                commandElementBox.Text = String.Empty;
            }
            if (hasElement && m_CurrentCommandElement != null && (commandTypeCombo.SelectedIndex == 5 || commandTypeCombo.SelectedIndex == 6))
            {
                commandElementBox.Text = String.Empty;
                m_CurrentCommandElement = null;
                m_CurrentCategoryId = -1;
                m_CurrentTagId = -1;
            }
            else if (hasElement && m_CurrentTagId != -1 && commandTypeCombo.SelectedIndex < 4)
            {
                commandElementBox.Text = String.Empty;
                m_CurrentCommandElement = null;
                m_CurrentCategoryId = -1;
                m_CurrentTagId = -1;
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

        private static String GetTagName(ITagCommand tagCommand, IProject project)
        {
            return GetTagName(tagCommand.TagId, project);
        }

        private static String GetTagName(int tagId, IProject project)
        {
            try
            {
                int languageId = project.TagLanguageId;
                if (languageId == -1)
                {
                    languageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                }
                List<int> tagIds = new List<int>();
                tagIds.Add(tagId);
                var tagInfos = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId).GetTagInfos(tagIds);
                if (tagInfos.Count == 0)
                    return StringResources.InvalidModeElement;
                else
                    return tagInfos[0].Name;
            }
            catch (Ares.Tags.TagsDbException /*ex*/)
            {
                return StringResources.InvalidModeElement;
            }
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
            bool needsTag = commandTypeCombo.SelectedIndex > 4 && commandTypeCombo.SelectedIndex < 7;
            if (needsCommandElement && m_CurrentCommandElement == null)
            {
                errorProvider.SetError(commandElementBox, StringResources.PleaseSelectElement);
                hasError = true;
            }
            if (needsTag && m_CurrentTagId == -1)
            {
                errorProvider.SetError(commandElementBox, StringResources.PleaseSelectTag);
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
                case 5:
                    MacroCommand = DataModule.MacroFactory.CreateTagCommand(m_CurrentCategoryId, m_CurrentTagId, true, conditionType, m_CurrentConditionElement);
                    break;
                case 6:
                    MacroCommand = DataModule.MacroFactory.CreateTagCommand(m_CurrentCategoryId, m_CurrentTagId, false, conditionType, m_CurrentConditionElement);
                    break;
                case 7:
                    MacroCommand = DataModule.MacroFactory.CreateRemoveAllTagsCommand(conditionType, m_CurrentConditionElement);
                    break;
                default:
                    DialogResult = DialogResult.None;
                    break;
            }
        }

        private void selectCommandElementButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ContextMenu menu;
            if (commandTypeCombo.SelectedIndex < 4)
                menu = CreateElementSelectionMenu((IModeElement element) =>
                {
                    m_CurrentCommandElement = element;
                    commandElementBox.Text = GetElementDisplayName(element, m_Project);
                    if (element != null && !String.IsNullOrEmpty(errorProvider.GetError(commandElementBox)))
                    {
                        errorProvider.SetError(commandElementBox, String.Empty);
                    }
                });
            else
                menu = CreateTagSelectionMenu((int categoryId, int tagId) =>
                    {
                        m_CurrentTagId = tagId;
                        m_CurrentCategoryId = categoryId;
                        commandElementBox.Text = GetTagName(tagId, m_Project);
                        if (m_CurrentTagId != -1 && !String.IsNullOrEmpty(errorProvider.GetError(commandElementBox)))
                        {
                            errorProvider.SetError(commandElementBox, String.Empty);
                        }
                    }, m_Project);
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

        private class TagSelectionHandler
        {
            public TagSelectionHandler(Action<int, int> tagSelectionAction, int categoryId, int tagId)
            {
                m_SelectionAction = tagSelectionAction;
                m_CategoryId = categoryId;
                m_TagId = tagId;
            }

            public void HandleEvent(Object o, EventArgs args)
            {
                m_SelectionAction(m_CategoryId, m_TagId);
            }

            private Action<int, int> m_SelectionAction;
            private int m_CategoryId;
            private int m_TagId;
        }

        private ContextMenu CreateTagSelectionMenu(Action<int, int> tagSelectionAction, IProject project)
        {
            ContextMenu menu = new ContextMenu();
            try
            {
                int languageId = project.TagLanguageId;
                if (languageId == -1)
                {
                    languageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                }
                var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId);
                foreach (var category in dbRead.GetAllCategories())
                {
                    MenuItem menuItem = new MenuItem(category.Name);
                    foreach (var tag in dbRead.GetAllTags(category.Id))
                    {
                        TagSelectionHandler handler = new TagSelectionHandler(tagSelectionAction, category.Id, tag.Id);
                        MenuItem tagItem = new MenuItem(tag.Name, new EventHandler(handler.HandleEvent));
                        menuItem.MenuItems.Add(tagItem);
                    }
                    menu.MenuItems.Add(menuItem);
                }
            }
            catch (Ares.Tags.TagsDbException)
            {
                // ignore here
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
