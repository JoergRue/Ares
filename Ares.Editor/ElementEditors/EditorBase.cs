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
