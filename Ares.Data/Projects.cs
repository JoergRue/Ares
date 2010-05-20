using System;
using System.Collections.Generic;

namespace Ares.Data
{
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
                reader.Read();
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Modes") && !reader.IsEmptyElement)
                    {
                        reader.Read();
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
