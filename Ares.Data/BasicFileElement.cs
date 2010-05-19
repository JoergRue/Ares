using System;

namespace Ares.Data
{
    [Serializable]
    class BasicFileElement : ElementBase, IElement, IFileElement
    {
        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitFileElement(this);
        }

        internal BasicFileElement(int ID, String filePath, SoundFileType fileType)
            : base(ID)
        {
            FilePath = filePath;
            Title = FileName;
            SoundFileType = fileType;
        }

        public String FileName { get { return m_FileName; } }
        public String FilePath { get { return m_FilePath; } 
            set {
                m_FilePath = value;
                int pos = m_FilePath.LastIndexOf('\\');
                m_FileName = (pos != -1) ? m_FilePath.Substring(pos + 1) : m_FilePath;
            }
        }

        public SoundFileType SoundFileType { get; set; }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("FileElement");
            DoWriteToXml(writer);
            writer.WriteAttributeString("Path", FilePath);
            writer.WriteAttributeString("SoundType", SoundFileType == Data.SoundFileType.Music ? "Music" : "Sound");
            writer.WriteEndElement();
        }

        internal BasicFileElement(System.Xml.XmlReader reader)
            : base(reader)
        {
            FilePath = reader.GetNonEmptyAttribute("Path");
            String soundType = reader.GetNonEmptyAttribute("SoundType");
            SoundFileType = soundType == "Music" ? SoundFileType.Music : SoundFileType.SoundEffect;
            reader.ReadOuterXml();
        }

        private String m_FileName;
        private String m_FilePath;
    }
}
