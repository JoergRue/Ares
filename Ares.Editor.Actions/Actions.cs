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

namespace Ares.Editor.Actions
{
    public class Actions
    {
        public static Actions Instance 
        { 
            get 
            { 
                if (s_Instance == null) 
                    s_Instance = new Actions(); 
                return s_Instance; 
            } 
        }

        public Action LastAction
        {
            get
            {
                return m_Actions.Count > 0 ? m_Actions[m_Actions.Count - 1] : null;
            }
        }

        public void AddNew(Action action)
        {
            if (m_Actions.Count > m_Index)
            {
                m_Actions.RemoveRange(m_Index, m_Actions.Count - m_Index);
            }
            m_Actions.Add(action);
            m_Index++;
            action.Do();
            if (UpdateGUI != null) UpdateGUI();
        }

        public void Clear()
        {
            m_Actions.Clear();
            m_Index = 0;
            m_NoChangeIndex = 0;
        }

        public void SetCurrentAsNoChange()
        {
            m_NoChangeIndex = m_Index;
            if (UpdateGUI != null) UpdateGUI();
        }

        public bool CanUndo
        {
            get
            {
                return m_Index > 0;
            }
        }

        public void Undo()
        {
            if (CanUndo)
            {
                m_Actions[--m_Index].Undo();
                if (UpdateGUI != null) UpdateGUI();
            }
        }

        public bool CanRedo
        {
            get
            {
                return m_Index < m_Actions.Count;
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                m_Actions[m_Index++].Do();
                if (UpdateGUI != null) UpdateGUI();
            }
        }

        public bool IsChanged
        {
            get
            {
                return m_Index != m_NoChangeIndex;
            }
        }
  
        private static Actions s_Instance;
        private List<Action> m_Actions = new List<Action>();
        private int m_Index = 0;
        private int m_NoChangeIndex = 0;

        public System.Action UpdateGUI { get; set; }
    }
}
