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

namespace Ares.Data
{
    /// <summary>
    /// Repository for all elements.
    /// </summary>
    public interface IElementRepository
    {
        /// <summary>
        /// Returns a specific element. Returns null if the element isn't found.
        /// </summary>
        /// <param name="id">ID of the element.</param>
        IElement GetElement(int id);
        /// <summary>
        /// Removes an element from the repository. No effect if the element isn't found.
        /// </summary>
        /// <param name="id">ID of the element.</param>
        void DeleteElement(int id);
    }

    class ElementRepository : IElementRepository
    {
        public IElement GetElement(int id)
        {
            IElement value;
            m_Elements.TryGetValue(id, out value);
            return value;
        }

        public void DeleteElement(int id)
        {
            m_Elements.Remove(id);
        }

        public void AddElement(ref int id, IElement element)
        {
            if (m_Elements.ContainsKey(id))
            {
                id = Data.DataModule.TheElementFactory.GetNextID();
            }
            m_Elements.Add(id, element);
        }

        internal void Clear()
        {
            m_Elements.Clear();
        }

        internal ElementRepository()
        {
            m_Elements = new Dictionary<int, IElement>();
        }

        private Dictionary<int, IElement> m_Elements;
    }
}
