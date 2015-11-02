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

namespace Ares.Editor.ElementEditors
{
    class EditorRegistry
    {
        public static EditorRegistry Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new EditorRegistry();
                }
                return sInstance;
            }
        }

        private static EditorRegistry sInstance;

        private EditorRegistry()
        {
        }

        public void RegisterEditor(int id, EditorBase editor)
        {
            m_Editors[id] = editor;
        }

        public void UnregisterEditor(int id)
        {
            m_Editors.Remove(id);
        }

        public EditorBase GetEditor(int id)
        {
            if (m_Editors.ContainsKey(id))
            {
                return m_Editors[id];
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<EditorBase> GetAllEditors()
        {
            return new List<EditorBase>(m_Editors.Values);
        }

        private Dictionary<int, EditorBase> m_Editors = new Dictionary<int, EditorBase>();
    }
}
