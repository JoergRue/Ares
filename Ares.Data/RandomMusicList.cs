using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Data
{
    [Serializable]
    abstract class ModeElementBase : ElementBase
    {
        public bool IsPlaying { get; set; }

        protected ModeElementBase(int ID) 
            : base(ID)
        {
            // IsPlaying = false;
        }
    }

    [Serializable]
    class RandomBackgroundMusicList : ModeElementBase, IRandomBackgroundMusicList
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

        public IChoiceElement AddElement(IElement element)
        {
            return m_ThirdContainer.AddElement(element);
        }

        public void RemoveElement(int ID)
        {
            m_ThirdContainer.RemoveElement(ID);
        }

        public IList<IChoiceElement> GetElements()
        {
            return m_ThirdContainer.GetElements();
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
            return true;
        }

        #endregion

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitSequentialContainer(m_FirstContainer);
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
            m_ParallelElement.Repeat = true;
        }

        private ITrigger m_Trigger;

        private IElementContainer<ISequentialElement> m_FirstContainer;
        private IElementContainer<IParallelElement> m_SecondContainer;
        private IElementContainer<IChoiceElement> m_ThirdContainer;

        private ISequentialElement m_SequentialElement;
        private IParallelElement m_ParallelElement;

    }
}
