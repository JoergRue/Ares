using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Editor.ElementEditors
{
    class EditorBase : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        protected int ElementId 
        {
            get
            {
                return m_ElementId;
            }
            set
            {
                if (m_ElementId != -1)
                {
                    EditorRegistry.Instance.UnregisterEditor(m_ElementId);
                }
                m_ElementId = value;
                if (m_ElementId != -1)
                {
                    EditorRegistry.Instance.RegisterEditor(m_ElementId, this);
                }
            }
        }

        protected EditorBase() { ElementId = -1; }

        protected override void Dispose(bool disposing)
        {
            if (ElementId != -1 && disposing)
            {
                EditorRegistry.Instance.UnregisterEditor(ElementId);
            }
            base.Dispose(disposing);
        }

        private int m_ElementId;
    }
}
