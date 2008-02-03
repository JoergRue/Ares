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
        /// <param name="Id">ID of the element.</param>
        IElement GetElement(int id);
        /// <summary>
        /// Removes an element from the repository. No effect if the element isn't found.
        /// </summary>
        /// <param name="Id">ID of the element.</param>
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

        public void AddElement(int id, IElement element)
        {
            m_Elements.Add(id, element);
        }

        internal ElementRepository()
        {
            m_Elements = new Dictionary<int, IElement>();
        }

        private Dictionary<int, IElement> m_Elements;
    }
}
