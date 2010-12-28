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
    class RandomBackgroundMusicList : ElementBase, IRandomBackgroundMusicList
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

        #region IDelayableElement Members

        public TimeSpan FixedStartDelay
        {
            get
            {
                return m_SequentialElement.FixedStartDelay;
            }
            set
            {
                m_SequentialElement.FixedStartDelay = value;
            }
        }

        public TimeSpan MaximumRandomStartDelay
        {
            get 
            { 
                return m_SequentialElement.MaximumRandomStartDelay; 
            }
            set
            {
                m_SequentialElement.MaximumRandomStartDelay = value;
            }
        }

        #endregion

        #region IElementContainer<IChoiceElement> Members

        public IElement AddGeneralElement(IElement element)
        {
            return AddElement(element);
        }

        public void InsertGeneralElement(int index, IElement element)
        {
            m_ThirdContainer.InsertGeneralElement(index, element);
        }

        public IChoiceElement AddElement(IElement element)
        {
            return m_ThirdContainer.AddElement(element);
        }

        public void RemoveElement(int ID)
        {
            m_ThirdContainer.RemoveElement(ID);
        }

        public IList<IContainerElement> GetGeneralElements()
        {
            return m_ThirdContainer.GetGeneralElements();
        }

        public IList<IChoiceElement> GetElements()
        {
            return m_ThirdContainer.GetElements();
        }

        public IList<IFileElement> GetFileElements()
        {
            return m_ThirdContainer.GetFileElements();
        }

        public IChoiceElement GetElement(int ID)
        {
            return m_ThirdContainer.GetElement(ID);
        }

        #endregion

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitRandomMusicList(this);
        }

        public void VisitElements(IElementVisitor visitor)
        {
            visitor.VisitSequentialContainer(m_FirstContainer);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("RandomMusicList");
            DoWriteToXml(writer);
            m_FirstContainer.WriteToXml(writer);
            writer.WriteEndElement();
        }
        
        internal RandomBackgroundMusicList(Int32 id, String title)
            : base(id)
        {
            Title = title;
            m_ThirdContainer = DataModule.ElementFactory.CreateChoiceContainer(title + "_Choices");
            m_SecondContainer = DataModule.ElementFactory.CreateParallelContainer(title + "_Repeat");
            m_FirstContainer = DataModule.ElementFactory.CreateSequentialContainer(title + "_Delay");
            m_ParallelElement = m_SecondContainer.AddElement(m_ThirdContainer);
            m_SequentialElement = m_FirstContainer.AddElement(m_SecondContainer);
            m_ParallelElement.RepeatCount = -1;
        }

        internal RandomBackgroundMusicList(System.Xml.XmlReader reader)
            : base(reader)
        {
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            reader.Read();
            m_FirstContainer = DataModule.TheElementFactory.CreateSequentialContainer(reader);
            m_SequentialElement = m_FirstContainer.GetElements()[0];
            m_SecondContainer = (IElementContainer<IParallelElement>)((SequentialElement)m_SequentialElement).InnerElement;
            m_ParallelElement = m_SecondContainer.GetElements()[0];
            m_ThirdContainer = (IElementContainer<IChoiceElement>)((ParallelElement)m_ParallelElement).InnerElement;
            reader.ReadEndElement();
        }

        private ISequentialContainer m_FirstContainer;
        private IElementContainer<IParallelElement> m_SecondContainer;
        private IElementContainer<IChoiceElement> m_ThirdContainer;

        private ISequentialElement m_SequentialElement;
        private IParallelElement m_ParallelElement;

    }
}
