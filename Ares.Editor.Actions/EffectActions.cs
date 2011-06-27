using System;
using System.Collections.Generic;
using Ares.Data;

namespace Ares.Editor.Actions
{
    public class SpeakerChangeAction : Action
    {
        public SpeakerChangeAction(IFileElement element, bool active, bool random, SpeakerAssignment assignment)
        {
            m_Element = element;
            m_OldActive = element.Effects.SpeakerAssignment.Active;
            m_OldRandom = element.Effects.SpeakerAssignment.Random;
            m_OldAssignment = element.Effects.SpeakerAssignment.Assignment;
            m_OldBalance = element.Effects.Balance.Active;
            m_NewActive = active;
            m_NewRandom = random;
            m_NewAssignment = assignment;
        }

        public void SetData(bool active, bool random, SpeakerAssignment assignment)
        {
            m_NewActive = active;
            m_NewRandom = random;
            m_NewAssignment = assignment;
        }

        public override void Do()
        {
            ISpeakerAssignmentEffect effect = m_Element.Effects.SpeakerAssignment;
            effect.Active = m_NewActive;
            effect.Random = m_NewRandom;
            effect.Assignment = m_NewAssignment;
            if (m_OldBalance && m_NewActive)
                m_Element.Effects.Balance.Active = false;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            ISpeakerAssignmentEffect effect = m_Element.Effects.SpeakerAssignment;
            effect.Active = m_OldActive;
            effect.Random = m_OldRandom;
            effect.Assignment = m_OldAssignment;
            m_Element.Effects.Balance.Active = m_OldBalance;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public IFileElement Element
        {
            get
            {
                return m_Element;
            }
        }

        private IFileElement m_Element;
        private bool m_OldActive;
        private bool m_OldRandom;
        private bool m_OldBalance;
        private SpeakerAssignment m_OldAssignment;
        private bool m_NewActive;
        private bool m_NewRandom;
        private SpeakerAssignment m_NewAssignment;
    }

    public class IntEffectChangeAction : Action
    {
        public IntEffectChangeAction(IFileElement element, IIntEffect effect, 
            bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            m_Element = element;
            m_Effect = effect;
            m_OldActive = m_Effect.Active;
            m_OldFixValue = m_Effect.FixValue;
            m_OldRandom = m_Effect.Random;
            m_OldMinRandom = m_Effect.MinRandomValue;
            m_OldMaxRandom = m_Effect.MaxRandomValue;
            m_NewActive = active;
            m_NewFixValue = fixValue;
            m_NewRandom = random;
            m_NewMinRandom = minRandomValue;
            m_NewMaxRandom = maxRandomValue;
        }

        public IFileElement Element
        {
            get
            {
                return m_Element;
            }
        }

        public void SetData(bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            m_NewActive = active;
            m_NewFixValue = fixValue;
            m_NewRandom = random;
            m_NewMinRandom = minRandomValue;
            m_NewMaxRandom = maxRandomValue;
        }

        public override void Do()
        {
            m_Effect.Active = m_NewActive;
            m_Effect.FixValue = m_NewFixValue;
            m_Effect.Random = m_NewRandom;
            m_Effect.MinRandomValue = m_NewMinRandom;
            m_Effect.MaxRandomValue = m_NewMaxRandom;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            m_Effect.Active = m_OldActive;
            m_Effect.FixValue = m_OldFixValue;
            m_Effect.Random = m_OldRandom;
            m_Effect.MinRandomValue = m_OldMinRandom;
            m_Effect.MaxRandomValue = m_OldMaxRandom;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        protected IFileElement m_Element;
        private IIntEffect m_Effect;
        private bool m_OldActive;
        private int m_OldFixValue;
        private bool m_OldRandom;
        private int m_OldMinRandom;
        private int m_OldMaxRandom;
        protected bool m_NewActive;
        private int m_NewFixValue;
        private bool m_NewRandom;
        private int m_NewMinRandom;
        private int m_NewMaxRandom;
    }

    public class BalanceChangeAction : IntEffectChangeAction
    {
        private bool m_OldSpeakers;

        public BalanceChangeAction(IFileElement element, 
            bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
            : base(element, element.Effects.Balance, active, random, fixValue, minRandomValue, maxRandomValue)
        {
            m_OldSpeakers = element.Effects.SpeakerAssignment.Active;
        }

        public override void Do()
        {
            if (m_Element.Effects.SpeakerAssignment.Active && m_NewActive)
                m_Element.Effects.SpeakerAssignment.Active = false;
            base.Do();
        }

        public override void Undo()
        {
            m_Element.Effects.SpeakerAssignment.Active = m_OldSpeakers;
            base.Undo();
        }
    }

    public class AllFileElementsSpeakerChangeAction : Action
    {
        private IList<IFileElement> m_FileElements;
        private List<bool> m_OldActives;
        private List<bool> m_OldRandoms;
        private List<bool> m_OldBalances;
        private List<SpeakerAssignment> m_OldAssignments;
        private bool m_NewActive;
        private bool m_NewRandom;
        private SpeakerAssignment m_NewAssignment;

        public AllFileElementsSpeakerChangeAction(IGeneralElementContainer container, bool active, bool random, SpeakerAssignment assignment)
        {
            m_FileElements = container.GetFileElements();
            m_OldActives = new List<bool>();
            m_OldRandoms = new List<bool>();
            m_OldBalances = new List<bool>();
            m_OldAssignments = new List<SpeakerAssignment>();
            foreach (IFileElement element in m_FileElements)
            {
                ISpeakerAssignmentEffect effect = element.Effects.SpeakerAssignment;
                m_OldActives.Add(effect.Active);
                m_OldRandoms.Add(effect.Random);
                m_OldAssignments.Add(effect.Assignment);
                m_OldBalances.Add(element.Effects.Balance.Active);
            }
            m_NewActive = active;
            m_NewRandom = random;
            m_NewAssignment = assignment;
        }

        public override void Do()
        {
            foreach (IFileElement element in m_FileElements)
            {
                ISpeakerAssignmentEffect effect = element.Effects.SpeakerAssignment;
                effect.Active = m_NewActive;
                effect.Random = m_NewRandom;
                effect.Assignment = m_NewAssignment;
                if (element.Effects.Balance.Active && m_NewActive)
                    element.Effects.Balance.Active = false;
                ElementChanges.Instance.ElementChanged(element.Id);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                ISpeakerAssignmentEffect effect = m_FileElements[i].Effects.SpeakerAssignment;
                effect.Active = m_OldActives[i];
                effect.Random = m_OldRandoms[i];
                effect.Assignment = m_OldAssignments[i];
                m_FileElements[i].Effects.Balance.Active = m_OldBalances[i];
                ElementChanges.Instance.ElementChanged(m_FileElements[i].Id);
            }
        }
    }

    public abstract class AllFileElementsIntEffectChangeAction : Action
    {
        protected IList<IFileElement> m_FileElements;
        private List<bool> m_OldActives;
        private List<int> m_OldFixValues;
        private List<bool> m_OldRandoms;
        private List<int> m_OldMinRandoms;
        private List<int> m_OldMaxRandoms;
        protected bool m_NewActive;
        private int m_NewFixValue;
        private bool m_NewRandom;
        private int m_NewMinRandom;
        private int m_NewMaxRandom;

        protected abstract IIntEffect GetEffect(IFileElement element);

        protected void SetValues(IGeneralElementContainer container, bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            m_FileElements = container.GetFileElements();
            m_OldActives = new List<bool>();
            m_OldFixValues = new List<int>();
            m_OldRandoms = new List<bool>();
            m_OldMinRandoms = new List<int>();
            m_OldMaxRandoms = new List<int>();
            foreach (IFileElement element in m_FileElements)
            {
                IIntEffect effect = GetEffect(element);
                m_OldActives.Add(effect.Active);
                m_OldFixValues.Add(effect.FixValue);
                m_OldRandoms.Add(effect.Random);
                m_OldMinRandoms.Add(effect.MinRandomValue);
                m_OldMaxRandoms.Add(effect.MaxRandomValue);
            }
            m_NewActive = active;
            m_NewFixValue = fixValue;
            m_NewRandom = random;
            m_NewMinRandom = minRandomValue;
            m_NewMaxRandom = maxRandomValue;
        }

        public override void Do()
        {
            foreach (IFileElement element in m_FileElements)
            {
                IIntEffect effect = GetEffect(element);
                effect.Active = m_NewActive;
                effect.FixValue = m_NewFixValue;
                effect.Random = m_NewRandom;
                effect.MinRandomValue = m_NewMinRandom;
                effect.MaxRandomValue = m_NewMaxRandom;
                ElementChanges.Instance.ElementChanged(element.Id);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                IIntEffect effect = GetEffect(m_FileElements[i]);
                effect.Active = m_OldActives[i];
                effect.FixValue = m_OldFixValues[i];
                effect.Random = m_OldRandoms[i];
                effect.MinRandomValue = m_OldMinRandoms[i];
                effect.MinRandomValue = m_OldMaxRandoms[i];
                ElementChanges.Instance.ElementChanged(m_FileElements[i].Id);
            }
        }
    }

    public class AllFileElementsPitchChangeAction : AllFileElementsIntEffectChangeAction
    {
        public AllFileElementsPitchChangeAction(IGeneralElementContainer container, bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            SetValues(container, active, random, fixValue, minRandomValue, maxRandomValue);
        }

        protected override IIntEffect GetEffect(IFileElement element)
        {
            return element.Effects.Pitch;
        }
    }

    public class AllFileElementsBalanceChangeAction : AllFileElementsIntEffectChangeAction
    {
        private List<bool> m_OldSpeakers;

        public AllFileElementsBalanceChangeAction(IGeneralElementContainer container, bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            m_OldSpeakers = new List<bool>();
            SetValues(container, active, random, fixValue, minRandomValue, maxRandomValue);
            foreach (IFileElement element in m_FileElements)
            {
                m_OldSpeakers.Add(element.Effects.SpeakerAssignment.Active);
            }
        }

        protected override IIntEffect GetEffect(IFileElement element)
        {
            return element.Effects.Balance;
        }

        public override void Do()
        {
            foreach (IFileElement element in m_FileElements)
            {
                if (element.Effects.SpeakerAssignment.Active && m_NewActive)
                {
                    element.Effects.SpeakerAssignment.Active = false;
                }
            }
            base.Do();
        }

        public override void Undo()
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                m_FileElements[i].Effects.SpeakerAssignment.Active = m_OldSpeakers[i];
            }
            base.Undo();
        }
    }

    public class AllFileElementsVolumeDBChangeAction : AllFileElementsIntEffectChangeAction
    {
        public AllFileElementsVolumeDBChangeAction(IGeneralElementContainer container, bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            SetValues(container, active, random, fixValue, minRandomValue, maxRandomValue);
        }

        protected override IIntEffect GetEffect(IFileElement element)
        {
            return element.Effects.VolumeDB;
        }
    }
}
