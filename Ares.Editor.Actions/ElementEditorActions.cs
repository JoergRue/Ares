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
using System.Linq;
using System.Text;

using Ares.Data;
using Ares.ModelInfo;

namespace Ares.Editor.Actions
{
    public class ElementVolumeChangeAction : Action
    {
        public ElementVolumeChangeAction(IElement element, bool setsMusic, bool setsSound, int music, int sound)
        {
            m_Element = element;

            m_OldSetsMusic = element.SetsMusicVolume;
            m_OldSetsSound = element.SetsSoundVolume;
            m_OldMusic = element.MusicVolume;
            m_OldSound = element.SoundVolume;

            m_NewMusic = music;
            m_NewSetsMusic = setsMusic;
            m_NewSetsSound = setsSound;
            m_NewSound = sound;
        }

        public void SetData(bool setsMusic, bool setsSound, int music, int sound)
        {
            m_NewSound = sound;
            m_NewMusic = music;
            m_NewSetsSound = setsSound;
            m_NewSetsMusic = setsMusic;
        }

        public IElement Element
        {
            get
            {
                return m_Element;
            }
        }

        public override void Do()
        {
            m_Element.SetsMusicVolume = m_NewSetsMusic;
            m_Element.MusicVolume = m_NewMusic;
            m_Element.SetsSoundVolume = m_NewSetsSound;
            m_Element.SoundVolume = m_NewSound;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            m_Element.SetsMusicVolume = m_OldSetsMusic;
            m_Element.SetsSoundVolume = m_OldSetsSound;
            m_Element.MusicVolume = m_OldMusic;
            m_Element.SoundVolume = m_OldSound;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IElement m_Element;

        private bool m_OldSetsMusic;
        private bool m_OldSetsSound;
        private int m_OldMusic;
        private int m_OldSound;

        private bool m_NewSetsMusic;
        private bool m_NewSetsSound;
        private int m_NewMusic;
        private int m_NewSound;
    }

    public class AllFileElementsVolumeChangeAction : Action
    {
        private IList<IFileElement> m_FileElements;
        private List<int> m_OldVolumes;
        private List<bool> m_OldRandoms;
        private List<int> m_OldMinRandoms;
        private List<int> m_OldMaxRandoms;
        private int m_NewVolume;
        private bool m_NewRandom;
        private int m_NewMinRandom;
        private int m_NewMaxRandom;

        public AllFileElementsVolumeChangeAction(IGeneralElementContainer container, bool randomVolume, int volume, int minRandomVolume, int maxRandomVolume)
        {
            m_FileElements = container.GetFileElements();
            m_OldVolumes = new List<int>();
            m_OldRandoms = new List<bool>();
            m_OldMinRandoms = new List<int>();
            m_OldMaxRandoms = new List<int>();
            foreach (IFileElement element in m_FileElements)
            {
                m_OldVolumes.Add(element.Effects.Volume);
                m_OldRandoms.Add(element.Effects.HasRandomVolume);
                m_OldMinRandoms.Add(element.Effects.MinRandomVolume);
                m_OldMaxRandoms.Add(element.Effects.MaxRandomVolume);
            }
            m_NewVolume = volume;
            m_NewRandom = randomVolume;
            m_NewMinRandom = minRandomVolume;
            m_NewMaxRandom = maxRandomVolume;
        }

        public override void Do()
        {
            foreach (IFileElement element in m_FileElements)
            {
                element.Effects.Volume = m_NewVolume;
                element.Effects.HasRandomVolume = m_NewRandom;
                element.Effects.MinRandomVolume = m_NewMinRandom;
                element.Effects.MaxRandomVolume = m_NewMaxRandom;
                ElementChanges.Instance.ElementChanged(element.Id);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                m_FileElements[i].Effects.Volume = m_OldVolumes[i];
                m_FileElements[i].Effects.HasRandomVolume = m_OldRandoms[i];
                m_FileElements[i].Effects.MinRandomVolume = m_OldMinRandoms[i];
                m_FileElements[i].Effects.MaxRandomVolume = m_OldMaxRandoms[i];
                ElementChanges.Instance.ElementChanged(m_FileElements[i].Id);
            }
        }
    }

    public class AllFileElementsFadingChangeAction : Action
    {
        private IList<IFileElement> m_FileElements;
        private List<int> m_OldTimes;
        private int m_NewTime;
        private bool m_FadeIn;

        public AllFileElementsFadingChangeAction(IGeneralElementContainer container, int time, bool fadeIn)
        {
            m_FileElements = container.GetFileElements();
            m_OldTimes = new List<int>();
            foreach (IFileElement element in m_FileElements)
            {
                m_OldTimes.Add(fadeIn ? element.Effects.FadeInTime : element.Effects.FadeOutTime);
            }
            m_NewTime = time;
            m_FadeIn = fadeIn;
        }

        public override void Do()
        {
            foreach (IFileElement element in m_FileElements)
            {
                if (m_FadeIn)
                {
                    element.Effects.FadeInTime = m_NewTime;
                }
                else
                {
                    element.Effects.FadeOutTime = m_NewTime;
                }
                ElementChanges.Instance.ElementChanged(element.Id);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < m_FileElements.Count; ++i)
            {
                if (m_FadeIn)
                {
                    m_FileElements[i].Effects.FadeInTime = m_OldTimes[i];
                }
                else
                {
                    m_FileElements[i].Effects.FadeOutTime = m_OldTimes[i];
                }
                ElementChanges.Instance.ElementChanged(m_FileElements[i].Id);
            }
        }
    }

    public class ElementVolumeEffectsChangeAction : Action
    {
        public ElementVolumeEffectsChangeAction(IList<IFileElement> elements, bool randomVolume, int volume, int minRandomVolume, int maxRandomVolume, int fadeIn, int fadeOut)
        {
            m_Elements = elements;
            m_OldVolume = new List<int>();
            m_OldFadeIn = new List<int>();
            m_OldFadeOut = new List<int>();
            m_OldRandom = new List<bool>();
            m_OldMinRandom = new List<int>();
            m_OldMaxRandom = new List<int>();
            for (int i = 0; i < elements.Count; ++i)
            {
                m_OldVolume.Add(m_Elements[i].Effects.Volume);
                m_OldFadeIn.Add(m_Elements[i].Effects.FadeInTime);
                m_OldFadeOut.Add(m_Elements[i].Effects.FadeOutTime);
                m_OldRandom.Add(m_Elements[i].Effects.HasRandomVolume);
                m_OldMinRandom.Add(m_Elements[i].Effects.MinRandomVolume);
                m_OldMaxRandom.Add(m_Elements[i].Effects.MaxRandomVolume);
            }
            m_NewVolume = volume;
            m_NewFadeIn = fadeIn;
            m_NewFadeOut = fadeOut;
            m_NewRandom = randomVolume;
            m_NewMinRandom = minRandomVolume;
            m_NewMaxRandom = maxRandomVolume;
        }

        public IList<IFileElement> Elements
        {
            get
            {
                return m_Elements;
            }
        }

        public void SetData(bool randomVolume, int volume, int minRandomVolume, int maxRandomVolume, int fadeIn, int fadeOut)
        {
            m_NewVolume = volume;
            m_NewFadeIn = fadeIn;
            m_NewFadeOut = fadeOut;
            m_NewRandom = randomVolume;
            m_NewMinRandom = minRandomVolume;
            m_NewMaxRandom = maxRandomVolume;
        }

        public override void Do()
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                m_Elements[i].Effects.Volume = m_NewVolume;
                m_Elements[i].Effects.FadeInTime = m_NewFadeIn;
                m_Elements[i].Effects.FadeOutTime = m_NewFadeOut;
                m_Elements[i].Effects.HasRandomVolume = m_NewRandom;
                m_Elements[i].Effects.MinRandomVolume = m_NewMinRandom;
                m_Elements[i].Effects.MaxRandomVolume = m_NewMaxRandom;
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < m_Elements.Count; ++i)
            {
                m_Elements[i].Effects.Volume = m_OldVolume[i];
                m_Elements[i].Effects.FadeInTime = m_OldFadeIn[i];
                m_Elements[i].Effects.FadeOutTime = m_OldFadeOut[i];
                m_Elements[i].Effects.HasRandomVolume = m_OldRandom[i];
                m_Elements[i].Effects.MinRandomVolume = m_OldMinRandom[i];
                m_Elements[i].Effects.MaxRandomVolume = m_OldMaxRandom[i];
                ElementChanges.Instance.ElementChanged(m_Elements[i].Id);
            }
        }

        private IList<IFileElement> m_Elements;
        private List<int> m_OldVolume;
        private List<int> m_OldFadeIn;
        private List<int> m_OldFadeOut;
        private List<bool> m_OldRandom;
        private List<int> m_OldMinRandom;
        private List<int> m_OldMaxRandom;
        private int m_NewVolume;
        private int m_NewFadeIn;
        private int m_NewFadeOut;
        private bool m_NewRandom;
        private int m_NewMinRandom;
        private int m_NewMaxRandom;
    }

    public class ElementRenamedAction : Action
    {
        public ElementRenamedAction(IElement element, String newName)
        {
            m_Element = element;
            m_OldName = element.Title;
            m_NewName = newName;
        }

        public IElement Element { get { return m_Element; } }

        public void SetName(String newName)
        {
            m_NewName = newName;
        }

        public override void Do()
        {
            m_Element.Title = m_NewName;
            ElementChanges.Instance.ElementRenamed(m_Element.Id);
        }

        public override void Undo()
        {
            m_Element.Title = m_OldName;
            ElementChanges.Instance.ElementRenamed(m_Element.Id);
        }

        private IElement m_Element;
        private String m_OldName;
        private String m_NewName;
    }

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

        public IDelayableElement Element
        {
            get
            {
                return m_Element;
            }
        }

        public void SetData(int fixedDelay, int maxDelay)
        {
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
        public RepeatableElementChangeAction(IRepeatableElement element, int repeatCount, int fixedDelay, int maxDelay)
        {
            m_Element = element;
            m_OldRepeatCount = element.RepeatCount;
            m_OldFixed = element.FixedIntermediateDelay;
            m_OldMax = element.MaximumRandomIntermediateDelay;
            m_NewRepeatCount = repeatCount;
            m_NewFixed = TimeSpan.FromMilliseconds(fixedDelay);
            m_NewMax = TimeSpan.FromMilliseconds(maxDelay);
        }

        public IRepeatableElement Element
        {
            get
            {
                return m_Element;
            }
        }

        public void SetData(int repeatCount, int fixedDelay, int maxDelay)
        {
            m_NewFixed = TimeSpan.FromMilliseconds(fixedDelay);
            m_NewMax = TimeSpan.FromMilliseconds(maxDelay);
            m_NewRepeatCount = repeatCount;
        }

        public override void Do()
        {
            m_Element.RepeatCount = m_NewRepeatCount;
            m_Element.FixedIntermediateDelay = m_NewFixed;
            m_Element.MaximumRandomIntermediateDelay = m_NewMax;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            m_Element.RepeatCount = m_OldRepeatCount;
            m_Element.FixedIntermediateDelay = m_OldFixed;
            m_Element.MaximumRandomIntermediateDelay = m_OldMax;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IRepeatableElement m_Element;
        private int m_OldRepeatCount;
        private int m_NewRepeatCount;

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

    public class SequentialElementChangeAction : Action
    {
        public SequentialElementChangeAction(ISequentialElement element, int newFixed, int newRandom)
        {
            m_Element = element;
            m_OldFixed = element.FixedStartDelay;
            m_OldRandom = element.MaximumRandomStartDelay;
            m_NewFixed = TimeSpan.FromMilliseconds(newFixed);
            m_NewRandom = TimeSpan.FromMilliseconds(newRandom);
        }

        public override void Do()
        {
            m_Element.FixedStartDelay = m_NewFixed;
            m_Element.MaximumRandomStartDelay = m_NewRandom;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo()
        {
            m_Element.FixedStartDelay = m_OldFixed;
            m_Element.MaximumRandomStartDelay = m_OldRandom;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private ISequentialElement m_Element;
        private TimeSpan m_OldFixed;
        private TimeSpan m_NewFixed;
        private TimeSpan m_OldRandom;
        private TimeSpan m_NewRandom;
    }

    public class AddContainerElementsAction : Action
    {
        public AddContainerElementsAction(IGeneralElementContainer container, IList<IElement> elements, int insertionIndex)
        {
            m_Container = container;
            m_InsertionIndex = insertionIndex;
            m_Elements = new List<IElement>();
            foreach (IElement element in elements)
            {
                IElement newElement = container.AddGeneralElement(element);
                m_Elements.Add(newElement);
                container.RemoveElement(newElement.Id);
                Data.DataModule.ElementRepository.DeleteElement(newElement.Id);
            }
        }

        public override void Do()
        {
            int index = m_InsertionIndex;
            foreach (IElement element in m_Elements)
            {
                m_Container.InsertGeneralElement(index, element);
                Data.DataModule.ElementRepository.AddElement(element);
                Ares.ModelInfo.ModelChecks.Instance.CheckAll();
                ElementRemoval.NotifyUndo(element);
                ++index;
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        public override void Undo()
        {
            foreach (IElement element in m_Elements)
            {
                m_Container.RemoveElement(element.Id);
                Data.DataModule.ElementRepository.DeleteElement(element.Id);
                Ares.ModelInfo.ModelChecks.Instance.CheckAll();
                ElementRemoval.NotifyRemoval(element);
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private IGeneralElementContainer m_Container;
        private List<IElement> m_Elements;
        private int m_InsertionIndex;
    }

    public class AddImportedContainerElementsAction : Action
    {
        public AddImportedContainerElementsAction(IGeneralElementContainer container, IList<IXmlWritable> elements, int insertionIndex)
        {
            m_Container = container;
            m_InsertionIndex = insertionIndex;
            m_Elements = new List<IElement>();
            foreach (IElement element in elements)
            {
                IList<IElement> newElements = container.AddGeneralImportedElement(element);
                m_Elements.AddRange(newElements);
                foreach (IElement newElement in newElements)
                {
                    container.RemoveElement(newElement.Id);
                    Data.DataModule.ElementRepository.DeleteElement(newElement.Id);
                }
            }
        }

        public override void Do()
        {
            int index = m_InsertionIndex;
            foreach (IElement element in m_Elements)
            {
                m_Container.InsertGeneralElement(index, element);
                Data.DataModule.ElementRepository.AddElement(element);
                Ares.ModelInfo.ModelChecks.Instance.CheckAll();
                ElementRemoval.NotifyUndo(element);
                ++index;
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        public override void Undo()
        {
            foreach (IElement element in m_Elements)
            {
                m_Container.RemoveElement(element.Id);
                Data.DataModule.ElementRepository.DeleteElement(element.Id);
                Ares.ModelInfo.ModelChecks.Instance.CheckAll();
                ElementRemoval.NotifyRemoval(element);
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private IGeneralElementContainer m_Container;
        private List<IElement> m_Elements;
        private int m_InsertionIndex;
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
                Data.DataModule.ElementRepository.DeleteElement(element.Id);
                Ares.ModelInfo.ModelChecks.Instance.CheckAll();
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
                Data.DataModule.ElementRepository.AddElement(element);
                Ares.ModelInfo.ModelChecks.Instance.CheckAll();
                ElementRemoval.NotifyUndo(element);
                ++index;
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private IGeneralElementContainer m_Container;
        private IList<IElement> m_Elements;
        private int m_Index;
    }

    public class ReorderContainerElementsAction : Action
    {
        public ReorderContainerElementsAction(IGeneralElementContainer container, IList<int> indices, int targetIndex)
        {
            m_Container = container;
            m_Indices = indices;
            m_targetIndex = targetIndex;
            m_OriginalOrder = container.GetGeneralElements();
        }

        public override void Do()
        {
            IList<IContainerElement> elements = m_Container.GetGeneralElements();
            List<IContainerElement> elems = new List<IContainerElement>();
            int targetIndex = m_targetIndex;
            foreach (int row in m_Indices)
            {
                if (row < targetIndex)
                    --targetIndex;
                elems.Add(elements[row]);
            }
            if (targetIndex < 0)
                targetIndex = 0;
            foreach (IContainerElement elem in elems)
            {
                m_Container.RemoveElement(elem.Id);
            }
            foreach (IContainerElement elem in elems)
            {
                m_Container.InsertGeneralElement(targetIndex, elem);
                ++targetIndex;
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        public override void Undo()
        {
            foreach (IContainerElement elem in m_OriginalOrder)
            {
                m_Container.RemoveElement(elem.Id);
            }
            int i = 0;
            foreach (IContainerElement elem in m_OriginalOrder)
            {
                m_Container.InsertGeneralElement(i, elem);
                ++i;
            }
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private IGeneralElementContainer m_Container;
        private IList<IContainerElement> m_OriginalOrder;
        private IList<int> m_Indices;
        private int m_targetIndex;
    }

    public class SetModeElementTriggerAction : Action
    {
        public SetModeElementTriggerAction(IModeElement modeElement, ITrigger trigger)
        {
            m_ModeElement = modeElement;
            m_OldTrigger = modeElement.Trigger;
            m_NewTrigger = trigger;
        }

        public override void Do()
        {
            m_ModeElement.Trigger = m_NewTrigger;
            ElementChanges.Instance.ElementTriggerChanged(m_ModeElement.Id);
            Ares.ModelInfo.ModelChecks.Instance.Check(ModelInfo.CheckType.Key);
        }

        public override void Undo()
        {
            m_ModeElement.Trigger = m_OldTrigger;
            ElementChanges.Instance.ElementTriggerChanged(m_ModeElement.Id);
            Ares.ModelInfo.ModelChecks.Instance.Check(ModelInfo.CheckType.Key);
        }

        private IModeElement m_ModeElement;
        private ITrigger m_OldTrigger;
        private ITrigger m_NewTrigger;
    }

    public class SetAllTriggerFadingAction : Action
    {
        public SetAllTriggerFadingAction(bool fade, bool crossFade, int fadeTime)
        {
            m_Fade = fade;
            m_CrossFade = crossFade;
            m_FadeTime = fadeTime;

            m_OldFades = new List<bool>();
            m_OldCrossFades = new List<bool>();
            m_OldFadeTimes = new List<int>();
            m_OldStopsMusic = new List<bool>();
            m_Triggers = new List<ITrigger>();
            m_TriggeredElementIds = new List<int>();

            Ares.Data.IProject project = Ares.ModelInfo.ModelChecks.Instance.Project;
            if (project != null)
            {
                foreach (IMode mode in project.GetModes())
                {
                    foreach (IModeElement modeElement in mode.GetElements())
                    {
                        if (modeElement.Trigger != null && (modeElement.Trigger.StopMusic || modeElement.AlwaysStartsMusic()))
                        {
                            m_Triggers.Add(modeElement.Trigger);
                            m_TriggeredElementIds.Add(modeElement.Id);
                            m_OldFades.Add(modeElement.Trigger.FadeMusic);
                            m_OldCrossFades.Add(modeElement.Trigger.CrossFadeMusic);
                            m_OldFadeTimes.Add(modeElement.Trigger.FadeMusicTime);
                            m_OldStopsMusic.Add(modeElement.Trigger.StopMusic);
                        }
                    }
                }
            }
        }

        public override void Do()
        {
            foreach (ITrigger trigger in m_Triggers)
            {
                trigger.FadeMusic = m_Fade;
                trigger.CrossFadeMusic = m_CrossFade;
                trigger.FadeMusicTime = m_FadeTime;
                trigger.StopMusic = true;
            }
            foreach (int id in m_TriggeredElementIds)
            {
                ElementChanges.Instance.ElementTriggerChanged(id);
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < m_Triggers.Count; ++i)
            {
                m_Triggers[i].CrossFadeMusic = m_OldCrossFades[i];
                m_Triggers[i].FadeMusic = m_OldFades[i];
                m_Triggers[i].FadeMusicTime = m_OldFadeTimes[i];
                m_Triggers[i].StopMusic = m_OldStopsMusic[i];
            }
            foreach (int id in m_TriggeredElementIds)
            {
                ElementChanges.Instance.ElementTriggerChanged(id);
            }
        }

        private bool m_Fade;
        private bool m_CrossFade;
        private int m_FadeTime;

        private List<ITrigger> m_Triggers;
        private List<int> m_TriggeredElementIds;
        private List<bool> m_OldFades;
        private List<bool> m_OldCrossFades;
        private List<bool> m_OldStopsMusic;
        private List<int> m_OldFadeTimes;
    }

}
