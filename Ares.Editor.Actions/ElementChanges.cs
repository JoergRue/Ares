using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Editor.Actions
{
    public class ElementChanges
    {
        public static ElementChanges Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new ElementChanges();
                }
                return s_Instance;
            }
        }

        public enum ChangeType { Changed, Renamed, Removed }

        public void AddListener(int elementID, Action<int, ChangeType> callback)
        {
            if (!m_Listeners.ContainsKey(elementID))
            {
                m_Listeners[elementID] = new List<Action<int, ChangeType>>();
            }
            m_Listeners[elementID].Add(callback);
        }

        public void RemoveListener(int elementID, Action<int, ChangeType> callback)
        {
            if (m_Listeners.ContainsKey(elementID))
            {
                List<Action<int, ChangeType>> callbacks = m_Listeners[elementID];
                callbacks.Remove(callback);
                if (callbacks.Count == 0)
                {
                    m_Listeners.Remove(elementID);
                }
            }
        }

        public void ElementChanged(int elementID)
        {
            Notify(elementID, ChangeType.Changed);
        }

        public void ElementRenamed(int elementID)
        {
            Notify(elementID, ChangeType.Renamed);
        }

        public void ElementRemoved(int elementID)
        {
            Notify(elementID, ChangeType.Removed);
        }

        private void Notify(int elementID, ChangeType changeType)
        {
            if (m_Listeners.ContainsKey(elementID))
            {
                List<Action<int, ChangeType>> copy = new List<Action<int, ChangeType>>(m_Listeners[elementID]);
                copy.ForEach(action => action(elementID, changeType));
            }
        }

        private ElementChanges()
        {
        }

        private Dictionary<int, List<Action<int, ChangeType>>> m_Listeners = new Dictionary<int, List<Action<int, ChangeType>>>();

        private static ElementChanges s_Instance;
    }
}
