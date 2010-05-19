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

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Mode");
            writer.WriteAttributeString("Title", Title);
            writer.WriteAttributeString("Key", KeyCode.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteStartElement("Elements");
            m_Elements.ForEach(e => e.WriteToXml(writer));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        internal Mode(String title)
        {
            Title = title;
            KeyCode = 0;
            m_Elements = new List<IModeElement>();
        }

        internal Mode(System.Xml.XmlReader reader)
        {
            Title = reader.GetNonEmptyAttribute("Title");
            KeyCode = reader.GetIntegerAttribute("Key");
            if (!reader.IsEmptyElement)
            {
                reader.MoveToContent();
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Elements") && !reader.IsEmptyElement)
                    {
                        reader.MoveToContent();
                        while (reader.IsStartElement())
                        {
                            IModeElement element = DataModule.TheElementFactory.CreateModeElement(reader);
                            if (element != null)
                            {
                                m_Elements.Add(element);
                            }
                        }
                    }
                    else
                    {
                        reader.ReadOuterXml();
                    }
                }
                reader.ReadEndElement();
            }
        }

        private List<IModeElement> m_Elements;
    }

    [Serializable]
    class Project : IProject
    {
        public String Title { get; set; }

        public String FileName 
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public bool Changed 
        {
            get { return m_Changed; }
            set { m_Changed = value; }
        }

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

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Project");
            writer.WriteAttributeString("Title", Title);
            writer.WriteAttributeString("SoundVolume", GetVolume(VolumeTarget.Sounds).ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("MusicVolume", GetVolume(VolumeTarget.Music).ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Volume", GetVolume(VolumeTarget.Both).ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteStartElement("Modes");
            m_Modes.ForEach(e => e.WriteToXml(writer));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        internal Project(String title)
        {
            Title = title;
            m_Modes = new List<IMode>();
            m_Volumes = new Int32[3];
            for (int i = 0; i < m_Volumes.Length; ++i) m_Volumes[i] = 100;
            FileName = "";
            Changed = false;
        }

        internal Project(System.Xml.XmlReader reader, String fileName)
        {
            m_Modes = new List<IMode>();
            m_Volumes = new Int32[3];

            if (!reader.IsStartElement("Project"))
            {
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "Project"), reader);
            }
            Title = reader.GetNonEmptyAttribute("Title");
            SetVolume(VolumeTarget.Sounds, reader.GetIntegerAttribute("SoundVolume"));
            SetVolume(VolumeTarget.Music, reader.GetIntegerAttribute("MusicVolume"));
            SetVolume(VolumeTarget.Both, reader.GetIntegerAttribute("Volume"));
            if (!reader.IsEmptyElement)
            {
                reader.MoveToContent();
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Modes") && !reader.IsEmptyElement)
                    {
                        reader.MoveToContent();
                        while (reader.IsStartElement())
                        {
                            if (reader.IsStartElement("Mode"))
                            {
                                m_Modes.Add(new Mode(reader));
                            }
                            else
                            {
                                reader.ReadOuterXml();
                            }
                        }
                        reader.ReadEndElement();
                    }
                    else
                    {
                        reader.ReadOuterXml();
                    }
                }
                reader.ReadEndElement();
            }
            else
            {
                reader.Read();
            }
            FileName = fileName;
            Changed = false;
        }

        private List<IMode> m_Modes;
        private Int32[] m_Volumes;

        [NonSerialized]
        private String m_FileName;
        [NonSerialized]
        private bool m_Changed;

    }
}
