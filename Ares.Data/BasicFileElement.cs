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
    class Effects : IEffects
    {
        public int Volume { get; set; }

        public int FadeInTime { get; set; }

        public int FadeOutTime { get; set; }

        internal void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Effects");
            writer.WriteAttributeString("Volume", Volume.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("FadeIn", FadeInTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("FadeOut", FadeOutTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        internal Effects()
        {
            Volume = 100;
            FadeInTime = 0;
            FadeOutTime = 0;
        }

        internal Effects(System.Xml.XmlReader reader)
        {
            Volume = reader.GetIntegerAttribute("Volume");
            FadeInTime = reader.GetIntegerAttributeOrDefault("FadeIn", 0);
            FadeOutTime = reader.GetIntegerAttributeOrDefault("FadeOut", 0);
            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.Read();
                reader.ReadInnerXml();
                reader.ReadEndElement();
            }
        }
    }

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
            m_Effects = new Effects();
        }

        public String FileName { get { return m_FileName; } }
        public String FilePath { get { return m_FilePath; } 
            set {
                m_FilePath = value;
                int pos = m_FilePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                m_FileName = (pos != -1) ? m_FilePath.Substring(pos + 1) : m_FilePath;
            }
        }

        public SoundFileType SoundFileType { get; set; }

        public IEffects Effects { get { return m_Effects; } }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("FileElement");
            DoWriteToXml(writer);
            writer.WriteAttributeString("Path", FilePath);
            writer.WriteAttributeString("SoundType", SoundFileType == Data.SoundFileType.Music ? "Music" : "Sound");
            m_Effects.WriteToXml(writer);
            writer.WriteEndElement();
        }

        internal BasicFileElement(System.Xml.XmlReader reader)
            : base(reader)
        {
            FilePath = reader.GetNonEmptyAttribute("Path");
            FilePath = FilePath.Replace('\\', System.IO.Path.DirectorySeparatorChar);
            String soundType = reader.GetNonEmptyAttribute("SoundType");
            SoundFileType = soundType == "Music" ? SoundFileType.Music : SoundFileType.SoundEffect;
            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.Read();
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Effects"))
                    {
                        m_Effects = new Effects(reader);
                    }
                    else
                    {
                        reader.ReadOuterXml();
                    }
                }
                reader.ReadEndElement();
            }
            if (m_Effects == null)
            {
                m_Effects = new Effects();
            }
        }

        private String m_FileName;
        private String m_FilePath;
        private Effects m_Effects;
    }
}
