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

namespace Ares.MGPlugin
{

    sealed class CommandButtonMapping
    {
        private bool m_ForTags = false;

        public CommandButtonMapping(bool forTags)
        {
            m_ForTags = forTags;
        }

        private Dictionary<int, System.Windows.Forms.CheckBox> m_Buttons = new Dictionary<int, System.Windows.Forms.CheckBox>();
        private Dictionary<int, EventHandler> m_Handlers = new Dictionary<int, EventHandler>();
        private HashSet<int> m_ActiveCommands = new HashSet<int>();

        public bool ButtonsActive { get; set; }

        public void RegisterButton(int id, int secondId, System.Windows.Forms.CheckBox button)
        {
            m_Buttons[id] = button;
            if (m_ActiveCommands.Contains(id))
                button.Checked = true;
            EventHandler handler = new EventHandler((Object sender, EventArgs args) =>
            {
                if (ButtonsActive && m_Listen)
                {
                    if (m_ForTags)
                    {
                        Controllers.Control.Instance.SwitchTag(secondId, id, (sender as System.Windows.Forms.CheckBox).Checked);
                    }
                    else
                    {
                        Controllers.Control.Instance.SwitchElement(id);
                    }
                }
            });
            m_Handlers[id] = handler;
            button.CheckedChanged += handler;
        }

        public void UnregisterButton(int id)
        {
            if (m_Buttons.ContainsKey(id))
            {
                m_Buttons[id].CheckedChanged -= m_Handlers[id];
                m_Handlers.Remove(id);
                m_Buttons.Remove(id);
            }
        }

        public bool IsCommandActive(int id)
        {
            return m_ActiveCommands.Contains(id);
        }

        private bool m_Listen = true;

        public void CommandStateChanged(int id, bool active)
        {
            if (m_ActiveCommands.Contains(id) && !active)
                m_ActiveCommands.Remove(id);
            else if (!m_ActiveCommands.Contains(id) && active)
                m_ActiveCommands.Add(id);
            if (m_Buttons.ContainsKey(id))
            {
                m_Listen = false;
                m_Buttons[id].Checked = active;
                m_Listen = true;
            }
        }

        public void AllCommandsInactive()
        {
            m_Listen = false;
            foreach (int id in m_ActiveCommands)
            {
                if (m_Buttons.ContainsKey(id))
                {
                    m_Buttons[id].Checked = false;
                }
            }
            m_Listen = true;
            m_ActiveCommands.Clear();
        }
    }
}
