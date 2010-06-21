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
