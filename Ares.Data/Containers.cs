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
    }

    [Serializable]
    abstract class ContainerElement
    {
        public int ID
        {
            get { return m_Element.ID; }
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

        protected ContainerElement()
        {
        }

        internal IElement InnerElement { set { m_Element = value; } }

        protected IElement m_Element;
    }

    [Serializable]
    class Container<I, T> : ContainerBase where I : IContainerElement where T : ContainerElement, I, new()
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
            T element = m_Elements.Find(e => e.ID == ID);
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
    }

    class SequentialContainer : Container<ISequentialElement, SequentialElement>, IElementContainer<ISequentialElement>
    {
        internal SequentialContainer(int ID, String title)
            : base(ID, title)
        {
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
    }

    class ChoiceContainer : Container<IChoiceElement, ChoiceElement>, IElementContainer<IChoiceElement>
    {
        internal ChoiceContainer(int ID, String title)
            : base(ID, title)
        {
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
    }

    class ParallelContainer : Container<IParallelElement, ParallelElement>, IElementContainer<IParallelElement>
    {
        internal ParallelContainer(int ID, String title)
            : base(ID, title)
        {
        }
    }

}
