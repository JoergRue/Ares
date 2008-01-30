using System;
using System.Collections.Generic;

namespace Ares.Data
{
    [Serializable]
    class Mode : IMode
    {
        public String Title { get; set; }

        public Int32 KeyCode { get; set; }

        public void AddElement(IModeElement element)
        {
            m_Elements.Add(element);
        }

        public void RemoveElement(IModeElement element)
        {
            m_Elements.Remove(element);
        }

        public IList<IModeElement> GetElements()
        {
            return new List<IModeElement>(m_Elements);
        }

        internal Mode(String title)
        {
            Title = title;
            KeyCode = 0;
            m_Elements = new List<IModeElement>();
        }

        private List<IModeElement> m_Elements;
    }

    [Serializable]
    class Project : IProject
    {
        public String Title { get; set; }

        public String FileName { get; set; }

        public bool Changed { get; set; }

        public IMode AddMode(String title)
        {
            Mode mode = new Mode(title);
            m_Modes.Add(mode);
            return mode;
        }

        public void RemoveMode(IMode mode)
        {
            m_Modes.Remove(mode);
        }

        public IList<IMode> GetModes()
        {
            return new List<IMode>(m_Modes);
        }

        internal Project(String title)
        {
            Title = title;
            m_Modes = new List<IMode>();
        }

        private List<IMode> m_Modes;
    }
}
