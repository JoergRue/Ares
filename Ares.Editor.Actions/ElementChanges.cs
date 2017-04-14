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

        public enum ChangeType { Changed, Renamed, Removed, Played, Stopped, TriggerChanged, PreRemoved }

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
            NotifyAll(elementID, ChangeType.Changed);
        }

        public void ElementRenamed(int elementID)
        {
            Notify(elementID, ChangeType.Renamed);
            NotifyAll(elementID, ChangeType.Renamed);
        }

        public void PreElementRemoved(int elementID)
        {
            Notify(elementID, ChangeType.PreRemoved);
            NotifyAll(elementID, ChangeType.PreRemoved);
        }

        public void ElementRemoved(int elementID)
        {
            Notify(elementID, ChangeType.Removed);
            NotifyAll(elementID, ChangeType.Removed);
        }

        public void ElementPlayed(int elementID)
        {
            Notify(elementID, ChangeType.Played);
            NotifyAll(elementID, ChangeType.Played);
        }

        public void ElementStopped(int elementID)
        {
            Notify(elementID, ChangeType.Stopped);
            NotifyAll(elementID, ChangeType.Stopped);
        }

        public void ElementTriggerChanged(int elementID)
        {
            Notify(elementID, ChangeType.TriggerChanged);
            NotifyAll(elementID, ChangeType.TriggerChanged);
        }

        private void NotifyAll(int elementID, ChangeType changeType)
        {
            if (m_Listeners.ContainsKey(-1))
            {
                List<Action<int, ChangeType>> copy = new List<Action<int, ChangeType>>(m_Listeners[-1]);
                copy.ForEach(action => action(elementID, changeType));
            }
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
