using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Data
{
    [Serializable]
    class RandomBackgroundMusicList : ElementBase, IRandomBackgroundMusicList
    {
        #region IRepeatableElement Members

        public bool Repeat
        {
            get
            {
                return mParallelElement.Repeat;
            }
            set
            {
                mParallelElement.Repeat = value;
            }
        }

        public TimeSpan FixedIntermediateDelay
        {
            get
            {
                return mParallelElement.FixedIntermediateDelay;
            }
            set
            {
                mParallelElement.FixedIntermediateDelay = value;
            }
        }

        public TimeSpan MaximumRandomIntermediateDelay
        {
            get 
            { 
                return mParallelElement.MaximumRandomIntermediateDelay; 
            }
            set
            {
                mParallelElement.MaximumRandomIntermediateDelay = value;
            }
        }

        #endregion

        #region IDelayableElement Members

        public TimeSpan FixedStartDelay
        {
            get
            {
                return mSequentialElement.FixedStartDelay;
            }
            set
            {
                mSequentialElement.FixedStartDelay = value;
            }
        }

        public TimeSpan MaximumRandomStartDelay
        {
            get 
            { 
                return mSequentialElement.MaximumRandomStartDelay; 
            }
            set
            {
                mSequentialElement.MaximumRandomStartDelay = value;
            }
        }

        #endregion

        #region IElementContainer<IChoiceElement> Members

        public IChoiceElement AddElement(IElement element)
        {
            return mThirdContainer.AddElement(element);
        }

        public void RemoveElement(int ID)
        {
            mThirdContainer.RemoveElement(ID);
        }

        public IList<IChoiceElement> GetElements()
        {
            return mThirdContainer.GetElements();
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
                m_Trigger.TargetElement = mFirstContainer;
            }
        }

        #endregion

        #region ICompositeElement Members

        public bool IsEndless()
        {
            return true;
        }

        #endregion

        internal RandomBackgroundMusicList(Int32 id, String title)
            : base(id)
        {
            Title = title;
            mThirdContainer = DataModule.ElementFactory.CreateChoiceContainer(title + "_Choices");
            mSecondContainer = DataModule.ElementFactory.CreateParallelContainer(title + "_Repeat");
            mFirstContainer = DataModule.ElementFactory.CreateSequentialContainer(title + "_Delay");
            mParallelElement = mSecondContainer.AddElement(mThirdContainer);
            mSequentialElement = mFirstContainer.AddElement(mSecondContainer);
            mParallelElement.Repeat = true;
        }

        private ITrigger m_Trigger;

        private IElementContainer<ISequentialElement> mFirstContainer;
        private IElementContainer<IParallelElement> mSecondContainer;
        private IElementContainer<IChoiceElement> mThirdContainer;

        private ISequentialElement mSequentialElement;
        private IParallelElement mParallelElement;

    }
}
