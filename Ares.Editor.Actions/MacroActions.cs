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

using Ares.Data;

namespace Ares.Editor.Actions
{
    public class AddMacroCommandAction : Action
    {
        public AddMacroCommandAction(IMacro macro, IMacroCommand command)
        {
            m_Macro = macro;
            m_MacroElement = macro.AddElement(command);
            m_Index = macro.GetElements().Count - 1;
            macro.RemoveElement(m_MacroElement.Id);
        }

        public override void Do()
        {
            m_Macro.InsertGeneralElement(m_Index, m_MacroElement);
            ElementRemoval.NotifyUndo(m_MacroElement);
            ElementChanges.Instance.ElementChanged(m_Macro.Id);
        }

        public override void Undo()
        {
            m_Macro.RemoveElement(m_MacroElement.Id);
            ElementRemoval.NotifyRemoval(m_MacroElement.InnerElement);
            ElementChanges.Instance.ElementChanged(m_Macro.Id);
        }

        private IMacro m_Macro;
        private IMacroElement m_MacroElement;
        private int m_Index;
    }

    public class ReplaceMacroCommandAction : Action
    {
        public ReplaceMacroCommandAction(IMacro macro, int oldCommandId, IMacroCommand newCommand)
        {
            m_Macro = macro;
            m_OldElement = macro.GetElement(oldCommandId);
            IList<IMacroElement> elements = m_Macro.GetElements();
            for (int i = 0; i < elements.Count; ++i)
            {
                if (elements[i] == m_OldElement)
                {
                    m_Index = i;
                    break;
                }
            }
            m_NewElement = macro.AddElement(newCommand);
            macro.RemoveElement(m_NewElement.Id);
        }

        public override void Do()
        {
            m_Macro.RemoveElement(m_OldElement.Id);
            m_Macro.InsertGeneralElement(m_Index, m_NewElement);
            ElementRemoval.NotifyRemoval(m_OldElement.InnerElement);
            ElementRemoval.NotifyUndo(m_NewElement);
            ElementChanges.Instance.ElementChanged(m_Macro.Id);
        }

        public override void Undo()
        {
            m_Macro.RemoveElement(m_NewElement.Id);
            m_Macro.InsertGeneralElement(m_Index, m_OldElement);
            ElementRemoval.NotifyRemoval(m_NewElement.InnerElement);
            ElementRemoval.NotifyUndo(m_OldElement);
            ElementChanges.Instance.ElementChanged(m_Macro.Id);
        }

        private IMacro m_Macro;
        private IMacroElement m_OldElement;
        private IMacroElement m_NewElement;
        private int m_Index;
    }
}
