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
        /// <summary>
        /// Adds a (previously deleted) element to the repository (for undo).
        /// </summary>
        /// <param name="element">the element</param>
        /// <returns>the (possibly new) Id of the element</returns>
        int AddElement(IElement element);
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
            if (m_Elements.ContainsKey(id))
            {
                IElement element = m_Elements[id];
                if (element is IGeneralElementContainer)
                {
                    foreach (IContainerElement containerElement in (element as IGeneralElementContainer).GetGeneralElements())
                    {
                        DeleteElement(containerElement.InnerElement.Id);
                    }
                }
                else if (element is IModeElement)
                {
                    IModeElement me = (element as IModeElement);
                    if (me.StartElement != null) 
                    {
                        DeleteElement(me.StartElement.Id);
                    }
                }
                // do NOT follow references ;-)
                m_Elements.Remove(id);
            }
        }

        public ReferenceRedirector Redirector { get; set; }

        public int AddElement(IElement element)
        {
            if (element is IGeneralElementContainer)
            {
                foreach (IContainerElement containerElement in (element as IGeneralElementContainer).GetGeneralElements())
                {
                    AddElement(containerElement.InnerElement);
                }
            }
            else if (element is IModeElement)
            {
                IModeElement me = (element as IModeElement);
                if (me.StartElement != null)
                {
                    AddElement(me.StartElement);
                }
            }
            // do NOT follow references
            int id = element.Id;
            AddElement(ref id, element);
            element.Id = id;
            return id;
        }

        public void AddElement(ref int id, IElement element)
        {
            if (m_Elements.ContainsKey(id))
            {
                int oldId = id;
                id = Data.DataModule.TheElementFactory.GetNextID();
                if (Redirector != null)
                {
                    Redirector.AddRedirection(oldId, id);
                }
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
