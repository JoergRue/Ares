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

        private Dictionary<int, EditorBase> m_Editors = new Dictionary<int, EditorBase>();
    }
}
