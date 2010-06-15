using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ares.Data;

namespace Ares.Editor.Actions
{
    public class DelayableElementChangeAction : Action
    {
        public DelayableElementChangeAction(IDelayableElement element, int fixedDelay, int maxDelay)
        {
            m_Element = element;
            m_OldFixed = element.FixedStartDelay;
            m_OldMax = element.MaximumRandomStartDelay;
            m_NewFixed = TimeSpan.FromMilliseconds(fixedDelay);
            m_NewMax = TimeSpan.FromMilliseconds(maxDelay);
        }

        public override void Do()
        {
            m_Element.FixedStartDelay = m_NewFixed;
            m_Element.MaximumRandomStartDelay = m_NewMax;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            m_Element.FixedStartDelay = m_OldFixed;
            m_Element.MaximumRandomStartDelay = m_OldMax;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IDelayableElement m_Element;
        private TimeSpan m_OldFixed;
        private TimeSpan m_NewFixed;
        private TimeSpan m_OldMax;
        private TimeSpan m_NewMax;
    }

    public class RepeatableElementChangeAction : Action
    {
        public RepeatableElementChangeAction(IRepeatableElement element, bool repeat, int fixedDelay, int maxDelay)
        {
            m_Element = element;
            m_OldRepeat = element.Repeat;
            m_OldFixed = element.FixedIntermediateDelay;
            m_OldMax = element.MaximumRandomIntermediateDelay;
            m_NewRepeat = repeat;
            m_NewFixed = TimeSpan.FromMilliseconds(fixedDelay);
            m_NewMax = TimeSpan.FromMilliseconds(maxDelay);
        }

        public override void Do()
        {
            m_Element.Repeat = m_NewRepeat;
            m_Element.FixedIntermediateDelay = m_NewFixed;
            m_Element.MaximumRandomIntermediateDelay = m_NewMax;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            m_Element.Repeat = m_OldRepeat;
            m_Element.FixedIntermediateDelay = m_OldFixed;
            m_Element.MaximumRandomIntermediateDelay = m_OldMax;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IRepeatableElement m_Element;
        private bool m_OldRepeat;
        private bool m_NewRepeat;

        private TimeSpan m_OldFixed;
        private TimeSpan m_NewFixed;
        private TimeSpan m_OldMax;
        private TimeSpan m_NewMax;
    }

    public class ChoiceElementChangeAction : Action
    {
        public ChoiceElementChangeAction(IChoiceElement element, int chance)
        {
            m_Element = element;
            m_OldChance = element.RandomChance;
            m_NewChance = chance;
        }

        public override void Do()
        {
            m_Element.RandomChance = m_NewChance;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            m_Element.RandomChance = m_OldChance;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IChoiceElement m_Element;
        private int m_OldChance;
        private int m_NewChance;
    }

    public class AddContainerElementsAction : Action
    {
        public AddContainerElementsAction(IGeneralElementContainer container, IList<IElement> elements)
        {
            m_Container = container;
            m_Elements = new List<IElement>();
            foreach (IElement element in elements)
            {
                IElement newElement = container.AddGeneralElement(element);
                m_Elements.Add(newElement);
                container.RemoveElement(newElement.Id);
            }
        }

        public override void Do()
        {
            int index = m_Container.GetGeneralElements().Count;
            foreach (IElement element in m_Elements)
            {
                m_Container.InsertGeneralElement(index, element);
                ++index;
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        public override void Undo()
        {
            foreach (IElement element in m_Elements)
            {
                m_Container.RemoveElement(element.Id);
                ElementRemoval.NotifyRemoval(element);
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private IGeneralElementContainer m_Container;
        private List<IElement> m_Elements;
    }

    public class RemoveContainerElementsAction : Action
    {
        public RemoveContainerElementsAction(IGeneralElementContainer container, IList<IElement> elements, int index)
        {
            m_Container = container;
            m_Elements = elements;
            m_Index = index;
        }

        public override void Do()
        {
            foreach (IElement element in m_Elements)
            {
                m_Container.RemoveElement(element.Id);
                ElementRemoval.NotifyRemoval(element);
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        public override void Undo()
        {
            int index = m_Index;
            foreach (IElement element in m_Elements)
            {
                m_Container.InsertGeneralElement(index, element);
                ++index;
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private IGeneralElementContainer m_Container;
        private IList<IElement> m_Elements;
        private int m_Index;
    }
}
