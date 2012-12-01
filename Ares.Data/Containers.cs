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
            set { m_Element.Id = value; }
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

        public bool SetsMusicVolume
        {
            get { return m_Element.SetsMusicVolume; }
            set { m_Element.SetsMusicVolume = value; }
        }

        public bool SetsSoundVolume
        {
            get { return m_Element.SetsSoundVolume; }
            set { m_Element.SetsMusicVolume = value; }
        }

        public int MusicVolume
        {
            get { return m_Element.MusicVolume; }
            set { m_Element.MusicVolume = value; }
        }

        public int SoundVolume
        {
            get { return m_Element.SoundVolume; }
            set { m_Element.SoundVolume = value; }
        }

        public IElement Clone()
        {
            return (IElement) MemberwiseClone();
        }

        public void Visit(IElementVisitor visitor)
        {
            m_Element.Visit(visitor);
        }

        /*
        public IList<IElementReference> References
        {
            get
            {
                return m_Element.References;
            }
        }

        public void AddReference(IElementReference reference)
        {
            m_Element.AddReference(reference);
        }
         */

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

        public IElement InnerElement { internal set { m_Element = value; } get { return m_Element; } }

        protected IElement m_Element;
    }

    class FileElementSearcher : IElementVisitor
    {
        private List<IFileElement> mFileElements;
        private HashSet<IFileElement> mElementsSet;
        private HashSet<IElement> mCheckedReferences = new HashSet<IElement>();

        public FileElementSearcher()
        {
            mFileElements = new List<IFileElement>();
            mElementsSet = new HashSet<IFileElement>();
        }

        public IList<IFileElement> GetFoundElements()
        {
            return mFileElements;
        }


        public void VisitFileElement(IFileElement fileElement)
        {
            if (!mElementsSet.Contains(fileElement))
            {
                mFileElements.Add(fileElement);
                mElementsSet.Add(fileElement);
            }
        }

        public void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
            foreach (ISequentialElement element in sequentialContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IParallelElement element in parallelContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
            foreach (IChoiceElement element in choiceContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
            foreach (IElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            foreach (IElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitMacro(IMacro macro)
        {
        }

        public void VisitMacroCommand(IMacroCommand macroCommand)
        {
        }

        public void VisitReference(IReferenceElement reference)
        {
            if (!mCheckedReferences.Contains(reference))
            {
                mCheckedReferences.Add(reference);
                IElement referencedElement = DataModule.ElementRepository.GetElement(reference.ReferencedId);
                if (referencedElement != null)
                    referencedElement.Visit(this);
                mCheckedReferences.Remove(reference);
            }
        }
    }

    [Serializable]
    abstract class Container<I, T> : ContainerBase where I : IContainerElement where T : ContainerElement, I, new()
    {
        public IElement AddGeneralElement(IElement element)
        {
            return AddElement(element);
        }

        public IList<IElement> AddGeneralImportedElement(IXmlWritable element)
        {
            List<IElement> result = new List<IElement>();
            T wrapper = element as T;
            if (wrapper != null)
            {
                m_Elements.Add(wrapper);
                result.Add(wrapper);
            }
            else if (element is IContainerElement)
            {
                return AddGeneralImportedElement((element as IContainerElement).InnerElement);
            }
            else
            {
                I added = AddElement(element as IElement);
                if (added != null)
                    result.Add(added);
            }
            return result;
        }

        public void InsertGeneralElement(int index, IElement element)
        {
            m_Elements.Insert(index, (T)element);
        }

        protected virtual bool CanAddElement(IElement element)
        {
            return !(element is IMacroCommand);
        }

        public I AddElement(IElement element)
        {
            if (!CanAddElement(element))
                return default(I);
            T wrapper = new T();
            ((ContainerElement)wrapper).InnerElement = element;
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

        public IList<IContainerElement> GetGeneralElements()
        {
            List<IContainerElement> result = new List<IContainerElement>();
            m_Elements.ForEach(e => result.Add(e));
            return result;
        }

        public IList<I> GetElements()
        {
            List<I> result = new List<I>();
            m_Elements.ForEach(e => result.Add(e));
            return result;
        }

        public IList<IFileElement> GetFileElements()
        {
            FileElementSearcher searcher = new FileElementSearcher();
            this.Visit(searcher);
            return searcher.GetFoundElements();
        }

        public I GetElement(int id)
        {
            return m_Elements.Find(e => e.Id == id);
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
            m_Elements.ForEach(e => ((IElement)e).WriteToXml(writer));
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

        protected List<T> m_Elements;
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
    class SequentialContainer : Container<ISequentialElement, SequentialElement>, ISequentialContainer
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

        public void MoveElements(int startIndex, int endIndex, int offset)
        {
            List<SequentialElement> elements = m_Elements.GetRange(startIndex, endIndex - startIndex + 1);
            m_Elements.RemoveRange(startIndex, endIndex - startIndex + 1);
            m_Elements.InsertRange(startIndex + offset, elements);
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

        public int RepeatCount { get; set; }

        public TimeSpan FixedIntermediateDelay { get; set; }

        public TimeSpan MaximumRandomIntermediateDelay { get; set; }

        public ParallelElement()
        {
            FixedStartDelay = TimeSpan.Zero;
            MaximumRandomStartDelay = TimeSpan.Zero;
            FixedIntermediateDelay = TimeSpan.Zero;
            MaximumRandomIntermediateDelay = TimeSpan.Zero;
            RepeatCount = 1;
        }

        public ParallelElement(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            FixedStartDelay = reader.GetTimeSpanAttribute("fixedDelay");
            MaximumRandomStartDelay = reader.GetTimeSpanAttribute("maxDelay");
            RepeatCount = reader.GetIntegerAttribute("repeatCount");
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
            writer.WriteAttributeString("repeatCount", RepeatCount.ToString(System.Globalization.CultureInfo.InvariantCulture));
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
