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

    public interface IReferenceHolder
    {
        int ReferencedId { get; set; }
    }

    [Serializable]
    class ReferenceHolder : IReferenceHolder
    {
        public int ReferencedId { get; set; }

        public void WriteToXml(System.Xml.XmlWriter writer, String name)
        {
            writer.WriteAttributeString(name, ReferencedId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public ReferenceHolder(int id)
        {
            ReferencedId = id;
        }

        public ReferenceHolder(System.Xml.XmlReader reader, String name)
        {
            ReferencedId = reader.GetIntegerAttribute(name);
            if (DataModule.TheElementRepository.Redirector != null)
            {
                DataModule.TheElementRepository.Redirector.AddReferenceHolder(this);
            }
        }
    }

    class ReferenceRedirector
    {
        public ReferenceRedirector()
        {
            m_ReferenceHolders = new Dictionary<int, List<IReferenceHolder>>();
            m_Redirections = new Dictionary<int,int>();
        }

        public void AddReferenceHolder(IReferenceHolder referenceHolder)
        {
            int id = referenceHolder.ReferencedId;
            if (!m_ReferenceHolders.ContainsKey(id))
            {
                m_ReferenceHolders[id] = new List<IReferenceHolder>();
            }
            m_ReferenceHolders[id].Add(referenceHolder);
            if (m_Redirections.ContainsKey(id))
            {
                referenceHolder.ReferencedId = m_Redirections[id];
            }
        }

        public void AddRedirection(int originalId, int newId)
        {
            m_Redirections[originalId] = newId;
            if (m_ReferenceHolders.ContainsKey(originalId))
            {
                foreach (IReferenceHolder holder in m_ReferenceHolders[originalId])
                {
                    holder.ReferencedId = newId;
                }
            }
        }

        private Dictionary<int, List<IReferenceHolder>> m_ReferenceHolders;
        private Dictionary<int, int> m_Redirections;
    }

    public interface IReferenceElement : IElement, IReferenceHolder
    {
    }

    class ReferenceElement : ElementBase, IReferenceElement
    {
        public int ReferencedId { get; set; }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ReferenceElement");
            DoWriteToXml(writer);
            writer.WriteAttributeString("ReferencedId", ReferencedId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        public ReferenceElement(int id)
            : base(DataModule.TheElementFactory.GetNextID())
        {
            ReferencedId = id;
            IElement referencedElement = DataModule.ElementRepository.GetElement(id);
            Title = referencedElement != null ? referencedElement.Title : "Reference";
            SetsMusicVolume = SetsSoundVolume = false;
        }

        public ReferenceElement(System.Xml.XmlReader reader)
            : base(reader)
        {
            ReferencedId = reader.GetIntegerAttribute("ReferencedId");
            if (DataModule.TheElementRepository.Redirector != null)
            {
                DataModule.TheElementRepository.Redirector.AddReferenceHolder(this);
            }

            if (!reader.IsEmptyElement)
            {
                reader.Read();
                reader.ReadInnerXml();
                reader.ReadEndElement();
            }
            else
            {
                reader.Read();
            }
        }

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitReference(this);
        }

    }

    /*
    class ContainerReference<T, U> : IContainerReference<T, U> where T : IElementContainer<U> where U : IContainerElement
    {
        public ContainerReference(int id, T element)
        {
            m_Id = id;
            m_Element = element;
        }

        // IElementReference

        public T ReferencedContainer
        {
            get { return m_Element; }
        }

        // IContainerReference

        public IElement ReferencedElement
        {
            get { return m_Element; }
        }

        // IElement

        public int Id { get { return m_Id; } set { m_Id = Id; }  }

        public String Title 
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
            get
            {
                return m_Element.SetsMusicVolume;
            }
            set
            {
                m_Element.SetsMusicVolume = value;
            }
        }


        public int MusicVolume
        {
            get
            {
                return m_Element.MusicVolume;
            }
            set
            {
                m_Element.MusicVolume = value;
            }
        }

        public bool SetsSoundVolume
        {
            get
            {
                return m_Element.SetsSoundVolume;
            }
            set
            {
                m_Element.SetsSoundVolume = value;
            }
        }

        public int SoundVolume
        {
            get
            {
                return m_Element.SoundVolume;
            }
            set
            {
                m_Element.SoundVolume = value;
            }
        }

        public IElement Clone()
        {
            return m_Element.Clone();
        }

        public void Visit(IElementVisitor visitor)
        {
            m_Element.Visit(visitor);
        }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            // TODO
        }

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


        // IGeneralElementContainer

        public IList<IContainerElement> GetGeneralElements()
        {
            return m_Element.GetGeneralElements();
        }

        public IElement AddGeneralElement(IElement element)
        {
            return m_Element.AddGeneralElement(element);
        }

        public IList<IElement> AddGeneralImportedElement(IXmlWritable element)
        {
            return m_Element.AddGeneralImportedElement(element);
        }

        public void InsertGeneralElement(int index, IElement element)
        {
            m_Element.InsertGeneralElement(index, element);
        }

        public void RemoveElement(int id)
        {
            m_Element.RemoveElement(id);
        }

        public IList<IFileElement> GetFileElements()
        {
            return m_Element.GetFileElements();
        }

        // IElementContainer<U>

        public U AddElement(IElement element)
        {
            return m_Element.AddElement(element);
        }

        public IList<U> GetElements()
        {
            return m_Element.GetElements();
        }

        public U GetElement(int id)
        {
            return m_Element.GetElement(id);
        }

        private T m_Element;
        private int m_Id;
    }
     */
}