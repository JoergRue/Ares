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

        public bool ContainsKeyTrigger(Int32 keyCode)
        {
            return GetTriggeredElement(keyCode) != null;
        }

        public IModeElement GetTriggeredElement(Int32 keyCode)
        {
            return m_Elements.Find(e => e.Trigger.TriggerType == TriggerType.Key
                && (e as IKeyTrigger).KeyCode == keyCode);
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

        public bool ContainsKeyMode(Int32 keyCode)
        {
            return GetMode(keyCode) != null;
        }

        public IMode GetMode(Int32 keyCode)
        {
            return m_Modes.Find(m => m.KeyCode == keyCode);
        }

        public Int32 GetVolume(VolumeTarget target)
        {
            return m_Volumes[(int)target];
        }

        public void SetVolume(VolumeTarget target, Int32 value)
        {
            if (value < 0 || value > 100) throw new ArgumentException(StringResources.InvalidVolume);
            m_Volumes[(int)target] = value;
        }

        internal Project(String title)
        {
            Title = title;
            m_Modes = new List<IMode>();
            m_Volumes = new Int32[3];
            for (int i = 0; i < m_Volumes.Length; ++i) m_Volumes[i] = 100;
        }

        private List<IMode> m_Modes;
        private Int32[] m_Volumes;

    }
}
