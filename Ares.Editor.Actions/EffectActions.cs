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

    public class ReverbEffectChangeAction : Action
    {
        private IFileElement m_Element;
        private bool m_OldActive;
        private int m_OldLevel;
        private int m_OldDelay;
        private bool m_NewActive;
        private int m_NewLevel;
        private int m_NewDelay;

        public ReverbEffectChangeAction(IFileElement element, bool active)
        {
            m_Element = element;
            m_OldActive = element.Effects.Reverb.Active;
            m_OldLevel = element.Effects.Reverb.Level;
            m_OldDelay = element.Effects.Reverb.Delay;
            m_NewActive = active;
            m_NewDelay = m_OldDelay;
            m_NewLevel = m_OldLevel;
        }

        public void SetData(bool active, int level, int delay)
        {
            m_NewActive = active;
            m_NewLevel = level;
            m_NewDelay = delay;
        }

        public override void Do()
        {
            IReverbEffect effect = m_Element.Effects.Reverb;
            effect.Active = m_NewActive;
            effect.Delay = m_NewDelay;
            effect.Level = m_NewLevel;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            IReverbEffect effect = m_Element.Effects.Reverb;
            effect.Active = m_OldActive;
            effect.Delay = m_OldDelay;
            effect.Level = m_OldLevel;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public IFileElement Element
        {
            get
            {
                return m_Element;
            }
        }
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
        private bool m_OldPanning;
        private int m_OldPanningStart;
        private int m_OldPanningEnd;
        private bool m_NewPanning;
        private int m_NewPanningStart;
        private int m_NewPanningEnd;

        public BalanceChangeAction(IFileElement element, bool active)
            : base(element, element.Effects.Balance, active, element.Effects.Balance.Random,
            element.Effects.Balance.FixValue, element.Effects.Balance.MinRandomValue, element.Effects.Balance.MaxRandomValue)
        {
            m_OldSpeakers = element.Effects.SpeakerAssignment.Active;
            IBalanceEffect effect = element.Effects.Balance;
            m_OldPanning = effect.IsPanning;
            m_OldPanningStart = effect.PanningStart;
            m_OldPanningEnd = effect.PanningEnd;
            m_NewPanning = m_OldPanning;
            m_NewPanningStart = m_OldPanningStart;
            m_NewPanningEnd = m_OldPanningEnd;
        }

        public override void Do()
        {
            if (m_Element.Effects.SpeakerAssignment.Active && m_NewActive)
                m_Element.Effects.SpeakerAssignment.Active = false;
            IBalanceEffect effect = m_Element.Effects.Balance;
            effect.IsPanning = m_NewPanning;
            effect.PanningStart = m_NewPanningStart;
            effect.PanningEnd = m_NewPanningEnd;
            base.Do();
        }

        public override void Undo()
        {
            m_Element.Effects.SpeakerAssignment.Active = m_OldSpeakers;
            IBalanceEffect effect = m_Element.Effects.Balance;
            effect.IsPanning = m_OldPanning;
            effect.PanningStart = m_OldPanningStart;
            effect.PanningEnd = m_OldPanningEnd;
            base.Undo();
        }

        public void SetData(bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue, bool pan, int panStart, int panEnd)
        {
            base.SetData(active, random, fixValue, minRandomValue, maxRandomValue);
            m_NewPanning = pan;
            m_NewPanningStart = panStart;
            m_NewPanningEnd = panEnd;
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

    public class AllFileElementsReverbChangeAction : Action
    {
        private IList<IFileElement> m_FileElements;
        private List<bool> m_OldActives;
        private List<int> m_OldLevels;
        private List<int> m_OldDelays;
        private bool m_NewActive;
        private int m_NewLevel;
        private int m_NewDelay;

        public AllFileElementsReverbChangeAction(IGeneralElementContainer container, IReverbEffect effect)
        {
            m_FileElements = container.GetFileElements();
            m_OldActives = new List<bool>();
            m_OldDelays = new List<int>();
            m_OldLevels = new List<int>();
            m_NewActive = effect.Active;
            m_NewDelay = effect.Delay;
            m_NewLevel = effect.Level;
            foreach (IFileElement element in m_FileElements)
            {
                IReverbEffect effect2 = element.Effects.Reverb;
                m_OldActives.Add(effect2.Active);
                m_OldDelays.Add(effect2.Delay);
                m_OldLevels.Add(effect2.Level);
            }
        }

        public override void Do()
        {
            foreach (IFileElement element in m_FileElements)
            {
                IReverbEffect effect = element.Effects.Reverb;
                effect.Active = m_NewActive;
                effect.Level = m_NewLevel;
                effect.Delay = m_NewDelay;
                ElementChanges.Instance.ElementChanged(element.Id);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                IReverbEffect effect = m_FileElements[i].Effects.Reverb;
                effect.Active = m_OldActives[i];
                effect.Level = m_OldLevels[i];
                effect.Delay = m_OldDelays[i];
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
        private List<bool> m_OldPanning;
        private List<int> m_OldPanningStarts;
        private List<int> m_OldPanningEnds;
        private bool m_NewPanning;
        private int m_NewPanningStart;
        private int m_NewPanningEnd;

        public AllFileElementsBalanceChangeAction(IGeneralElementContainer container, IBalanceEffect newValues)
        {
            m_OldSpeakers = new List<bool>();
            m_OldPanning = new List<bool>();
            m_OldPanningStarts = new List<int>();
            m_OldPanningEnds = new List<int>();
            SetValues(container, newValues.Active, newValues.Random, newValues.FixValue, newValues.MinRandomValue, newValues.MaxRandomValue);
            m_NewPanning = newValues.IsPanning;
            m_NewPanningStart = newValues.PanningStart;
            m_NewPanningEnd = newValues.PanningEnd;
            foreach (IFileElement element in m_FileElements)
            {
                m_OldSpeakers.Add(element.Effects.SpeakerAssignment.Active);
                m_OldPanning.Add(element.Effects.Balance.IsPanning);
                m_OldPanningStarts.Add(element.Effects.Balance.PanningStart);
                m_OldPanningEnds.Add(element.Effects.Balance.PanningEnd);
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
                IBalanceEffect effect = element.Effects.Balance;
                effect.IsPanning = m_NewPanning;
                effect.PanningStart = m_NewPanningStart;
                effect.PanningEnd = m_NewPanningEnd;
            }
            base.Do();
        }

        public override void Undo()
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                m_FileElements[i].Effects.SpeakerAssignment.Active = m_OldSpeakers[i];
                IBalanceEffect effect = m_FileElements[i].Effects.Balance;
                effect.IsPanning = m_OldPanning[i];
                effect.PanningStart = m_OldPanningStarts[i];
                effect.PanningEnd = m_OldPanningEnds[i];
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
