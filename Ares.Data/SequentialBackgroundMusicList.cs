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
using System.Text;

namespace Ares.Data
{
    class SequentialBackgroundMusicList : ElementBase, ISequentialBackgroundMusicList
    {
        #region IRepeatableElement Members

        public int RepeatCount
        {
            get
            {
                return m_ParallelElement.RepeatCount;
            }
            set
            {
                m_ParallelElement.RepeatCount = value;
            }
        }

        public TimeSpan FixedIntermediateDelay
        {
            get
            {
                return m_ParallelElement.FixedIntermediateDelay;
            }
            set
            {
                m_ParallelElement.FixedIntermediateDelay = value;
            }
        }

        public TimeSpan MaximumRandomIntermediateDelay
        {
            get 
            { 
                return m_ParallelElement.MaximumRandomIntermediateDelay; 
            }
            set
            {
                m_ParallelElement.MaximumRandomIntermediateDelay = value;
            }
        }

        #endregion

        #region IElementContainer<ISequentialElement> Members

        public IElement AddGeneralElement(IElement element)
        {
            return AddElement(element);
        }

        public IList<IElement> AddGeneralImportedElement(IXmlWritable element)
        {
            List<IElement> result = new List<IElement>();
            if (element is ISequentialElement)
            {
                ISequentialElement sequential = element as ISequentialElement;
                if (sequential.InnerElement is IFileElement)
                {
                    result.AddRange(m_SecondContainer.AddGeneralImportedElement(element));
                    return result;
                }
            }
            foreach (IFileElement fileElement in element.GetFileElements())
            {
                result.Add(m_SecondContainer.AddElement(fileElement));
            }
            return result;
        }

        public void InsertGeneralElement(int index, IElement element)
        {
            m_SecondContainer.InsertGeneralElement(index, element);
        }

        public ISequentialElement AddElement(IElement element)
        {
            return m_SecondContainer.AddElement(element);
        }

        public void RemoveElement(int ID)
        {
            m_SecondContainer.RemoveElement(ID);
        }

        public IList<IContainerElement> GetGeneralElements()
        {
            return m_SecondContainer.GetGeneralElements();
        }

        public IList<ISequentialElement> GetElements()
        {
            return m_SecondContainer.GetElements();
        }

        public IList<IFileElement> GetFileElements()
        {
            return m_SecondContainer.GetFileElements();
        }

        public ISequentialElement GetElement(int ID)
        {
            return m_SecondContainer.GetElement(ID);
        }

        public void MoveElements(int startIndex, int endIndex, int offset)
        {
            m_SecondContainer.MoveElements(startIndex, endIndex, offset);
        }

        #endregion

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitSequentialMusicList(this);
        }

        public void VisitElements(IElementVisitor visitor)
        {
            visitor.VisitParallelContainer(m_FirstContainer);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("SequentialMusicList");
            DoWriteToXml(writer);
            m_FirstContainer.WriteToXml(writer);
            writer.WriteEndElement();
        }

        internal SequentialBackgroundMusicList(Int32 id, String title)
            : base(id)
        {
            Title = title;
            m_SecondContainer = DataModule.ElementFactory.CreateSequentialContainer(title + "_Sequence");
            m_FirstContainer = DataModule.ElementFactory.CreateParallelContainer(title + "_Repeat");
            m_ParallelElement = m_FirstContainer.AddElement(m_SecondContainer);
            m_ParallelElement.RepeatCount = 1;
        }

        internal SequentialBackgroundMusicList(System.Xml.XmlReader reader)
            : base(reader)
        {
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            reader.Read();
            m_FirstContainer = DataModule.TheElementFactory.CreateParallelContainer(reader);
            m_ParallelElement = m_FirstContainer.GetElements()[0];
            m_SecondContainer = (ISequentialContainer)((ParallelElement)m_ParallelElement).InnerElement;
            reader.ReadEndElement();
        }

        private IElementContainer<IParallelElement> m_FirstContainer;
        private ISequentialContainer m_SecondContainer;

        private IParallelElement m_ParallelElement;

    }
}
