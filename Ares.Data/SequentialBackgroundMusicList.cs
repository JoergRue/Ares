using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Data
{
    class SequentialBackgroundMusicList : ModeElementBase, ISequentialBackgroundMusicList
    {
        #region IRepeatableElement Members

        public bool Repeat
        {
            get
            {
                return m_ParallelElement.Repeat;
            }
            set
            {
                m_ParallelElement.Repeat = value;
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

        public ISequentialElement AddElement(IElement element)
        {
            return m_SecondContainer.AddElement(element);
        }

        public void RemoveElement(int ID)
        {
            m_SecondContainer.RemoveElement(ID);
        }

        public IList<ISequentialElement> GetElements()
        {
            return m_SecondContainer.GetElements();
        }

        #endregion

        #region IModeElement Members

        public ITrigger Trigger
        {
            get
            {
                return m_Trigger;
            }
            set
            {
                m_Trigger = value;
                m_Trigger.TargetElement = m_FirstContainer;
                m_Trigger.StopMusic = true;
            }
        }

        public IElement StartElement { get { return m_FirstContainer; } }

        #endregion

        #region ICompositeElement Members

        public bool IsEndless()
        {
            return Repeat;
        }

        #endregion

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitParallelContainer(m_FirstContainer);
        }
        
        internal SequentialBackgroundMusicList(Int32 id, String title)
            : base(id)
        {
            Title = title;
            m_SecondContainer = DataModule.ElementFactory.CreateSequentialContainer(title + "_Sequence");
            m_FirstContainer = DataModule.ElementFactory.CreateParallelContainer(title + "_Repeat");
            m_ParallelElement = m_FirstContainer.AddElement(m_SecondContainer);
            m_ParallelElement.Repeat = false;
        }

        private ITrigger m_Trigger;

        private IElementContainer<IParallelElement> m_FirstContainer;
        private IElementContainer<ISequentialElement> m_SecondContainer;

        private IParallelElement m_ParallelElement;

    }
}
