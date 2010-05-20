using System;
using System.Collections.Generic;

namespace Ares.Data
{
    [Serializable]
    abstract class ContainerBase : ElementBase
    {
        protected ContainerBase(int id, String title)
            : base(id)
        {
            Title = title;
        }

        protected ContainerBase(System.Xml.XmlReader reader)
            : base(reader)
        {
        }
    }

    [Serializable]
    abstract class ContainerElement
    {
        public int Id
        {
            get { return m_Element.Id; }
        }

        public string Title
        {
            get
            {
                return m_Element.Title;
            }
            set
            {
                m_Element.Title = value;
            }
        }

        public IElement Clone()
        {
            return (IElement) MemberwiseClone();
        }

        public void Visit(IElementVisitor visitor)
        {
            m_Element.Visit(visitor);
        }

        protected void DoWriteToXml(System.Xml.XmlWriter writer)
        {
            m_Element.WriteToXml(writer);
        }

        protected ContainerElement()
        {
        }

        protected void DoReadFromXml(System.Xml.XmlReader reader)
        {
            m_Element = DataModule.TheElementFactory.CreateElement(reader);
        }

        internal IElement InnerElement { set { m_Element = value; } get { return m_Element; } }

        protected IElement m_Element;
    }

    [Serializable]
    abstract class Container<I, T> : ContainerBase where I : IContainerElement where T : ContainerElement, I, new()
    {
        public I AddElement(IElement element)
        {
            T wrapper = new T();
            wrapper.InnerElement = element;
            m_Elements.Add(wrapper);
            return wrapper;
        }

        public void RemoveElement(int ID)
        {
            T element = m_Elements.Find(e => e.Id == ID);
            if (element != null)
            {
                m_Elements.Remove(element);
            }
        }

        public IList<I> GetElements()
        {
            List<I> result = new List<I>();
            m_Elements.ForEach(e => result.Add(e));
            return result;
        }

        internal Container(int ID, String title)
            : base(ID, title)
        {
            m_Elements = new List<T>();
        }

        protected override void DoWriteToXml(System.Xml.XmlWriter writer)
        {
            base.DoWriteToXml(writer);
            writer.WriteStartElement("SubElements");
            m_Elements.ForEach(e => e.WriteToXml(writer));
            writer.WriteEndElement();
        }

        internal Container(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_Elements = new List<T>();
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            reader.Read();
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("SubElements") && !reader.IsEmptyElement)
                {
                    reader.Read();
                    while (reader.IsStartElement())
                    {
                        T element = ReadContainerElement(reader);
                        if (element != null)
                        {
                            m_Elements.Add(element);
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

        protected abstract T ReadContainerElement(System.Xml.XmlReader reader);

        private List<T> m_Elements;
    }

    [Serializable]
    class SequentialElement : ContainerElement, ISequentialElement
    {
        public TimeSpan FixedStartDelay { get; set; }

        public TimeSpan MaximumRandomStartDelay { get; set; }

        public SequentialElement()
            : base()
        {
            FixedStartDelay = TimeSpan.Zero;
            MaximumRandomStartDelay = TimeSpan.Zero;
        }

        public SequentialElement(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            FixedStartDelay = reader.GetTimeSpanAttribute("fixedDelay");
            MaximumRandomStartDelay = reader.GetTimeSpanAttribute("maxDelay");
            reader.Read();
            DoReadFromXml(reader);
            reader.ReadEndElement();
        }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Sequential");
            writer.WriteAttributeString("fixedDelay", FixedStartDelay.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("maxDelay", MaximumRandomStartDelay.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            
            DoWriteToXml(writer);

            writer.WriteEndElement();
        }
    }

    [Serializable]
    class SequentialContainer : Container<ISequentialElement, SequentialElement>, IElementContainer<ISequentialElement>
    {
        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitSequentialContainer(this);
        }

        internal SequentialContainer(int ID, String title)
            : base(ID, title)
        {
        }

        internal SequentialContainer(System.Xml.XmlReader reader)
            : base(reader)
        {
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("SequentialContainer");
            DoWriteToXml(writer);
            writer.WriteEndElement();
        }

        protected override SequentialElement ReadContainerElement(System.Xml.XmlReader reader)
        {
            if (!reader.IsStartElement("Sequential"))
            {
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "Sequential"), reader);
            }
            return new SequentialElement(reader);
        }
    }

    [Serializable]
    class ChoiceElement : ContainerElement, IChoiceElement
    {
        public int RandomChance { get; set; }

        public ChoiceElement()
            : base()
        {
            RandomChance = 100;
        }

        public ChoiceElement(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            RandomChance = reader.GetIntegerAttribute("chance");
            reader.Read();
            DoReadFromXml(reader);
            reader.ReadEndElement();
        }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Choice");
            writer.WriteAttributeString("chance", RandomChance.ToString(System.Globalization.CultureInfo.InvariantCulture));

            DoWriteToXml(writer);

            writer.WriteEndElement();
        }
    }

    [Serializable]
    class ChoiceContainer : Container<IChoiceElement, ChoiceElement>, IElementContainer<IChoiceElement>
    {
        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitChoiceContainer(this);
        }

        internal ChoiceContainer(int ID, String title)
            : base(ID, title)
        {
        }

        internal ChoiceContainer(System.Xml.XmlReader reader)
            : base(reader)
        {
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ChoiceContainer");
            DoWriteToXml(writer);
            writer.WriteEndElement();
        }

        protected override ChoiceElement ReadContainerElement(System.Xml.XmlReader reader)
        {
            if (!reader.IsStartElement("Choice"))
            {
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "Choice"), reader);
            }
            return new ChoiceElement(reader);
        }
    }

    [Serializable]
    class ParallelElement : ContainerElement, IParallelElement
    {
        public TimeSpan FixedStartDelay { get; set; }

        public TimeSpan MaximumRandomStartDelay { get; set; }

        public bool Repeat { get; set; }

        public TimeSpan FixedIntermediateDelay { get; set; }

        public TimeSpan MaximumRandomIntermediateDelay { get; set; }

        public ParallelElement()
        {
            FixedStartDelay = TimeSpan.Zero;
            MaximumRandomStartDelay = TimeSpan.Zero;
            FixedIntermediateDelay = TimeSpan.Zero;
            MaximumRandomIntermediateDelay = TimeSpan.Zero;
            Repeat = false;
        }

        public ParallelElement(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            FixedStartDelay = reader.GetTimeSpanAttribute("fixedDelay");
            MaximumRandomStartDelay = reader.GetTimeSpanAttribute("maxDelay");
            Repeat = reader.GetBooleanAttribute("repeat");
            FixedIntermediateDelay = reader.GetTimeSpanAttribute("fixedIntermediateDelay");
            MaximumRandomIntermediateDelay = reader.GetTimeSpanAttribute("maxIntermediateDelay");

            reader.Read();
            DoReadFromXml(reader);
            reader.ReadEndElement();
        }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Parallel");
            writer.WriteAttributeString("fixedDelay", FixedStartDelay.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("maxDelay", MaximumRandomStartDelay.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("repeat", Repeat ? "true" : "false");
            writer.WriteAttributeString("fixedIntermediateDelay", FixedIntermediateDelay.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("maxIntermediateDelay", MaximumRandomIntermediateDelay.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture));

            DoWriteToXml(writer);

            writer.WriteEndElement();
        }
    }

    [Serializable]
    class ParallelContainer : Container<IParallelElement, ParallelElement>, IElementContainer<IParallelElement>
    {

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitParallelContainer(this);
        }

        internal ParallelContainer(int ID, String title)
            : base(ID, title)
        {
        }

        internal ParallelContainer(System.Xml.XmlReader reader)
            : base(reader)
        {
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ParallelContainer");
            DoWriteToXml(writer);
            writer.WriteEndElement();
        }

        protected override ParallelElement ReadContainerElement(System.Xml.XmlReader reader)
        {
            if (!reader.IsStartElement("Parallel"))
            {
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "Parallel"), reader);
            }
            return new ParallelElement(reader);
        }
    }

}
