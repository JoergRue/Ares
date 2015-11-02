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

    [Serializable]
    class ModeElement : ElementBase, IModeElement
    {
        public bool IsPlaying { get; set; }

        public IElement StartElement { get; set; }

        public ITrigger Trigger 
        {
            get
            {
                return m_Trigger;
            }
            set
            {
                m_Trigger = value;
                if (m_Trigger != null)
                {
                    m_Trigger.TargetElementId = StartElement.Id;
                }
            }
        }

        public bool IsVisibleInPlayer { get; set; }

        public override void Visit(IElementVisitor visitor)
        {
            StartElement.Visit(visitor);
        }

        internal ModeElement(int ID, String title, IElement startElement)
            : base(ID)
        {
            // IsPlaying = false;
            Title = title;
            StartElement = startElement;
            IsVisibleInPlayer = true;
        }

        internal ModeElement(System.Xml.XmlReader reader)
            : base(reader)
        {
            // IsPlaying = false;
            IsVisibleInPlayer = reader.GetBooleanAttributeOrDefault("visibleInPlayer", true);
            if (!reader.IsEmptyElement)
            {
                reader.Read();
                StartElement = DataModule.TheElementFactory.CreateElement(reader);
                m_Trigger = DataModule.TheElementFactory.CreateTrigger(reader);
                reader.ReadEndElement();
            }
            else
            {
                reader.Read();
            }
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ModeElement");
            writer.WriteAttributeString("visibleInPlayer", IsVisibleInPlayer ? "true" : "false");
            DoWriteToXml(writer);
            StartElement.WriteToXml(writer);
            if (Trigger != null)
            {
                Trigger.WriteToXml(writer);
            }
            writer.WriteEndElement();
        }

        private ITrigger m_Trigger;
    }

    [Serializable]
    class Mode : IMode
    {
        public String Title { get; set; }

        public Int32 KeyCode { get; set; }

        public void AddElement(IModeElement element)
        {
            m_Elements.Add(element);
        }

        public void InsertElement(int index, IModeElement element)
        {
            m_Elements.Insert(index, element);
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
            return m_Elements.Find(e => e.Trigger != null && e.Trigger.TriggerType == TriggerType.Key
                && (e.Trigger as IKeyTrigger).KeyCode == keyCode);
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
            m_Elements = new List<IModeElement>();
            Title = reader.GetNonEmptyAttribute("Title");
            KeyCode = reader.GetIntegerAttribute("Key");
            if (!reader.IsEmptyElement)
            {
                reader.Read();
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Elements") && !reader.IsEmptyElement)
                    {
                        reader.Read();
                        while (reader.IsStartElement())
                        {
                            if (reader.IsStartElement("ModeElement"))
                            {
                                ModeElement element = new ModeElement(reader);
                                if (element.StartElement != null)
                                {
                                    m_Elements.Add(element);
                                }
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
        }

        private List<IModeElement> m_Elements;
    }

}