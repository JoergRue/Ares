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
        /// <param name="ID">ID of the element.</param>
        IElement GetElement(int ID);
        /// <summary>
        /// Removes an element from the repository. No effect if the element isn't found.
        /// </summary>
        /// <param name="ID">ID of the element.</param>
        void DeleteElement(int ID);
    }

    class ElementRepository : IElementRepository
    {
        public IElement GetElement(int ID)
        {
            IElement value;
            m_Elements.TryGetValue(ID, out value);
            return value;
        }

        public void DeleteElement(int ID)
        {
            m_Elements.Remove(ID);
        }

        internal ElementRepository()
        {
            m_Elements = new Dictionary<int, IElement>();
        }

        private Dictionary<int, IElement> m_Elements;
    }
}
