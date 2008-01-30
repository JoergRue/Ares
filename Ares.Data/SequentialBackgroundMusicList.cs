using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Data
{
    class SequentialBackgroundMusicList : ElementBase, ISequentialBackgroundMusicList
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

        #region IElementContainer<ISequentialElement> Members

        public ISequentialElement AddElement(IElement element)
        {
            return mSecondContainer.AddElement(element);
        }

        public void RemoveElement(int ID)
        {
            mSecondContainer.RemoveElement(ID);
        }

        public IList<ISequentialElement> GetElements()
        {
            return mSecondContainer.GetElements();
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
            return Repeat;
        }

        #endregion

        internal SequentialBackgroundMusicList(Int32 id, String title)
            : base(id)
        {
            Title = title;
            mSecondContainer = DataModule.ElementFactory.CreateSequentialContainer(title + "_Sequence");
            mFirstContainer = DataModule.ElementFactory.CreateParallelContainer(title + "_Repeat");
            mParallelElement = mFirstContainer.AddElement(mSecondContainer);
            mParallelElement.Repeat = false;
        }

        private ITrigger m_Trigger;

        private IElementContainer<IParallelElement> mFirstContainer;
        private IElementContainer<ISequentialElement> mSecondContainer;

        private IParallelElement mParallelElement;

    }
}
