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
