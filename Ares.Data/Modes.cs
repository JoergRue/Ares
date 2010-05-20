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
                m_Trigger.TargetElementId = StartElement.Id;
            }
        }

        public override void Visit(IElementVisitor visitor)
        {
            StartElement.Visit(visitor);
        }

        public bool IsEndless()
        {
            IRepeatableElement repeatable = StartElement as IRepeatableElement;
            if (repeatable != null)
            {
                return repeatable.Repeat;
            }
            ICompositeElement composite = StartElement as ICompositeElement;
            if (composite != null)
            {
                return composite.IsEndless();
            }
            return false;
        }

        internal ModeElement(int ID, String title, IElement startElement)
            : base(ID)
        {
            // IsPlaying = false;
            Title = title;
            StartElement = startElement;
        }

        internal ModeElement(System.Xml.XmlReader reader)
            : base(reader)
        {
            // IsPlaying = false;
            StartElement = DataModule.TheElementFactory.CreateElement(reader);
            m_Trigger = DataModule.TheElementFactory.CreateTrigger(reader);
            reader.ReadEndElement();
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ModeElement");
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
                                m_Elements.Add(element);
                            }
                            else
                            {
                                reader.ReadOuterXml();
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

}