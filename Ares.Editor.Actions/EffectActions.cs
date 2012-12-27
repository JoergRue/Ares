/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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
using Ares.Data;

namespace Ares.Editor.Actions
{
    public class SpeakerChangeAction : Action
    {
        public SpeakerChangeAction(IList<IFileElement> elements, bool active, bool random, SpeakerAssignment assignment)
        {
            m_Elements = elements;
            m_OldActive = new List<bool>();
            m_OldRandom = new List<bool>();
            m_OldAssignment = new List<SpeakerAssignment>();
            m_OldBalance = new List<bool>();
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                m_OldActive.Add(elements[i].Effects.SpeakerAssignment.Active);
                m_OldRandom.Add(elements[i].Effects.SpeakerAssignment.Random);
                m_OldAssignment.Add(elements[i].Effects.SpeakerAssignment.Assignment);
                m_OldBalance.Add(elements[i].Effects.Balance.Active);
            }
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

        public override void Do(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                ISpeakerAssignmentEffect effect = m_Elements[i].Effects.SpeakerAssignment;
                effect.Active = m_NewActive;
                effect.Random = m_NewRandom;
                effect.Assignment = m_NewAssignment;
                if (m_OldBalance[i] && m_NewActive)
                    m_Elements[i].Effects.Balance.Active = false;
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        public override void Undo(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                ISpeakerAssignmentEffect effect = m_Elements[i].Effects.SpeakerAssignment;
                effect.Active = m_OldActive[i];
                effect.Random = m_OldRandom[i];
                effect.Assignment = m_OldAssignment[i];
                m_Elements[i].Effects.Balance.Active = m_OldBalance[i];
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        public IList<IFileElement> Elements
        {
            get
            {
                return m_Elements;
            }
        }

        private IList<IFileElement> m_Elements;
        private List<bool> m_OldActive;
        private List<bool> m_OldRandom;
        private List<bool> m_OldBalance;
        private List<SpeakerAssignment> m_OldAssignment;
        private bool m_NewActive;
        private bool m_NewRandom;
        private SpeakerAssignment m_NewAssignment;
    }

    public class ReverbEffectChangeAction : Action
    {
        private IList<IFileElement> m_Elements;
        private List<bool> m_OldActive;
        private List<int> m_OldLevel;
        private List<int> m_OldDelay;
        private bool m_NewActive;
        private int m_NewLevel;
        private int m_NewDelay;

        public ReverbEffectChangeAction(IList<IFileElement> elements, bool active)
        {
            m_Elements = elements;
            m_OldActive = new List<bool>();
            m_OldLevel = new List<int>();
            m_OldDelay = new List<int>();
            for (int i = 0; i < elements.Count; ++i)
            {
                m_OldActive.Add(elements[i].Effects.Reverb.Active);
                m_OldLevel.Add(elements[i].Effects.Reverb.Level);
                m_OldDelay.Add(elements[i].Effects.Reverb.Delay);
            }
            m_NewActive = active;
            m_NewDelay = m_OldDelay.Count > 0 ? m_OldDelay[0] : 0;
            m_NewLevel = m_OldLevel.Count > 0 ? m_OldLevel[0] : 0;
        }

        public void SetData(bool active, int level, int delay)
        {
            m_NewActive = active;
            m_NewLevel = level;
            m_NewDelay = delay;
        }

        public override void Do(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                IReverbEffect effect = m_Elements[i].Effects.Reverb;
                effect.Active = m_NewActive;
                effect.Delay = m_NewDelay;
                effect.Level = m_NewLevel;
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        public override void Undo(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                IReverbEffect effect = m_Elements[i].Effects.Reverb;
                effect.Active = m_OldActive[i];
                effect.Delay = m_OldDelay[i];
                effect.Level = m_OldLevel[i];
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        public IList<IFileElement> Elements
        {
            get
            {
                return m_Elements;
            }
        }
    }

    public class IntEffectChangeAction : Action
    {
        public IntEffectChangeAction(IList<IFileElement> elements, IList<IIntEffect> effects, 
            bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            m_Elements = elements;
            m_Effects = effects;
            m_OldActive = new List<bool>();
            m_OldFixValue = new List<int>();
            m_OldMinRandom = new List<int>();
            m_OldMaxRandom = new List<int>();
            m_OldRandom = new List<bool>();
            for (int i = 0; i < elements.Count; ++i)
            {
                m_OldActive.Add(m_Effects[i].Active);
                m_OldFixValue.Add(m_Effects[i].FixValue);
                m_OldRandom.Add(m_Effects[i].Random);
                m_OldMinRandom.Add(m_Effects[i].MinRandomValue);
                m_OldMaxRandom.Add(m_Effects[i].MaxRandomValue);
            }
            m_NewActive = active;
            m_NewFixValue = fixValue;
            m_NewRandom = random;
            m_NewMinRandom = minRandomValue;
            m_NewMaxRandom = maxRandomValue;
        }

        public IList<IFileElement> Elements
        {
            get
            {
                return m_Elements;
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

        public override void Do(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Effects.Count; ++i)
            {
                m_Effects[i].Active = m_NewActive;
                m_Effects[i].FixValue = m_NewFixValue;
                m_Effects[i].Random = m_NewRandom;
                m_Effects[i].MinRandomValue = m_NewMinRandom;
                m_Effects[i].MaxRandomValue = m_NewMaxRandom;
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        public override void Undo(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Effects.Count; ++i)
            {
                m_Effects[i].Active = m_OldActive[i];
                m_Effects[i].FixValue = m_OldFixValue[i];
                m_Effects[i].Random = m_OldRandom[i];
                m_Effects[i].MinRandomValue = m_OldMinRandom[i];
                m_Effects[i].MaxRandomValue = m_OldMaxRandom[i];
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        protected IList<IFileElement> m_Elements;
        private IList<IIntEffect> m_Effects;
        private List<bool> m_OldActive;
        private List<int> m_OldFixValue;
        private List<bool> m_OldRandom;
        private List<int> m_OldMinRandom;
        private List<int> m_OldMaxRandom;
        protected bool m_NewActive;
        private int m_NewFixValue;
        private bool m_NewRandom;
        private int m_NewMinRandom;
        private int m_NewMaxRandom;
    }

    public class BalanceChangeAction : IntEffectChangeAction
    {
        private List<bool> m_OldSpeakers;
        private List<bool> m_OldPanning;
        private List<int> m_OldPanningStart;
        private List<int> m_OldPanningEnd;
        private bool m_NewPanning;
        private int m_NewPanningStart;
        private int m_NewPanningEnd;

        private static IList<IIntEffect> GetEffectList(IList<IFileElement> elements)
        {
            List<IIntEffect> result = new List<IIntEffect>();
            for (int i = 0; i < elements.Count; ++i)
            {
                result.Add(elements[i].Effects.Balance);
            }
            return result;
        }


        public BalanceChangeAction(IList<IFileElement> elements, bool active)
            : base(elements, GetEffectList(elements), active, elements[0].Effects.Balance.Random,
            elements[0].Effects.Balance.FixValue, elements[0].Effects.Balance.MinRandomValue, elements[0].Effects.Balance.MaxRandomValue)
        {
            m_OldSpeakers = new List<bool>();
            m_OldPanning = new List<bool>();
            m_OldPanningStart = new List<int>();
            m_OldPanningEnd = new List<int>();
            for (int i = 0; i < elements.Count; ++i)
            {
                m_OldSpeakers.Add(elements[i].Effects.SpeakerAssignment.Active);
                IBalanceEffect effect = elements[i].Effects.Balance;
                m_OldPanning.Add(effect.IsPanning);
                m_OldPanningStart.Add(effect.PanningStart);
                m_OldPanningEnd.Add(effect.PanningEnd);
            }
            m_NewPanning = m_OldPanning[0];
            m_NewPanningStart = m_OldPanningStart[0];
            m_NewPanningEnd = m_OldPanningEnd[0];
        }

        public override void Do(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                if (m_Elements[i].Effects.SpeakerAssignment.Active && m_NewActive)
                    m_Elements[i].Effects.SpeakerAssignment.Active = false;
                IBalanceEffect effect = m_Elements[i].Effects.Balance;
                effect.IsPanning = m_NewPanning;
                effect.PanningStart = m_NewPanningStart;
                effect.PanningEnd = m_NewPanningEnd;
            }
            base.Do(project);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                m_Elements[i].Effects.SpeakerAssignment.Active = m_OldSpeakers[i];
                IBalanceEffect effect = m_Elements[i].Effects.Balance;
                effect.IsPanning = m_OldPanning[i];
                effect.PanningStart = m_OldPanningStart[i];
                effect.PanningEnd = m_OldPanningEnd[i];
            }
            base.Undo(project);
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

        public override void Do(Ares.Data.IProject project)
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

        public override void Undo(Ares.Data.IProject project)
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

        public override void Do(Ares.Data.IProject project)
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

        public override void Undo(Ares.Data.IProject project)
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

        public override void Do(Ares.Data.IProject project)
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

        public override void Undo(Ares.Data.IProject project)
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

    public class AllFileElementsTempoChangeAction : AllFileElementsIntEffectChangeAction
    {
        public AllFileElementsTempoChangeAction(IGeneralElementContainer container, bool active, bool random, int fixValue, int minRandomValue, int maxRandomValue)
        {
            SetValues(container, active, random, fixValue, minRandomValue, maxRandomValue);
        }

        protected override IIntEffect GetEffect(IFileElement element)
        {
            return element.Effects.Tempo;
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

        public override void Do(Ares.Data.IProject project)
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
            base.Do(project);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                m_FileElements[i].Effects.SpeakerAssignment.Active = m_OldSpeakers[i];
                IBalanceEffect effect = m_FileElements[i].Effects.Balance;
                effect.IsPanning = m_OldPanning[i];
                effect.PanningStart = m_OldPanningStarts[i];
                effect.PanningEnd = m_OldPanningEnds[i];
            }
            base.Undo(project);
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
