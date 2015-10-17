/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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

namespace Ares.ModelInfo
{
    class ReferenceChecks : ModelCheck, IElementVisitor, IMacroCommandVisitor
    {
        public ReferenceChecks()
            : base(CheckType.Reference)
        {
        }

        private IModelErrors m_ModelErrors;
        private int m_TagLanguage = -1;

        public override void DoChecks(IProject project, IModelErrors errors)
        {
            m_ModelErrors = errors;
            m_TagLanguage = project.TagLanguageId;
            foreach (IMode mode in project.GetModes())
            {
                foreach (IModeElement element in mode.GetElements())
                {
                    element.Visit(this);
                }
            }
        }

        public void VisitFileElement(IFileElement fileElement)
        {
        }

        public void VisitWebRadioElement(IWebRadioElement webRadio)
        {

        }

        public void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
            foreach (ISequentialElement element in sequentialContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IParallelElement element in parallelContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
            foreach (IChoiceElement element in choiceContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
        }

        public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
        }

        public void VisitMacro(IMacro macro)
        {
            macro.VisitMacro(this);
        }

        public void VisitMacroCommand(IMacroCommand macroCommand)
        {
            macroCommand.VisitMacroCommand(this);
        }

        public void VisitReference(IReferenceElement reference)
        {
            if (Data.DataModule.ElementRepository.GetElement(reference.ReferencedId) == null)
            {
                AddError(m_ModelErrors, ModelError.ErrorSeverity.Error, StringResources.ReferencedElementNotFound, reference);
            }
            // no need to follow the reference, referenced element will be checked through its 'default' path
        }

        public void VisitMusicByTags(IMusicByTags musicByTags)
        {
            // ignore missing tags here, because there is no way to remove them in the GUI anyway
            /*
            int languageId = m_TagLanguage;
            if (m_TagLanguage == -1)
                m_TagLanguage = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
            List<int> tagIds = new List<int>();
            tagIds.AddRange(musicByTags.GetAllTags());
            var tagInfos = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_TagLanguage).GetTagInfos(tagIds);
            if (tagInfos.Count != tagIds.Count)
            {
                AddError(m_ModelErrors, ModelError.ErrorSeverity.Error, StringResources.TagNotFound, musicByTags);
            }
            */
        }

        private void CheckCondition(IMacroCommand command)
        {
            switch (command.Condition.ConditionType)
            {
                case MacroConditionType.ElementNotRunning:
                case MacroConditionType.ElementRunning:
                    if (command.Condition.Conditional == null || Data.DataModule.ElementRepository.GetElement(command.Condition.Conditional.Id) == null)
                    {
                        AddError(m_ModelErrors, ModelError.ErrorSeverity.Error, StringResources.ConditionElementNotFound, command);
                    }
                    break;
                default:
                    break;
            }
        }

        public void VisitStartCommand(IStartCommand startCommand)
        {
            CheckCondition(startCommand);
            if (startCommand.StartedElement == null || Data.DataModule.ElementRepository.GetElement(startCommand.StartedElement.Id) == null)
            {
                AddError(m_ModelErrors, ModelError.ErrorSeverity.Error, StringResources.StartedElementNotFound, startCommand);
            }
        }

        public void VisitStopCommand(IStopCommand stopCommand)
        {
            CheckCondition(stopCommand);
            if (stopCommand.StoppedElement == null || Data.DataModule.ElementRepository.GetElement(stopCommand.StoppedElement.Id) == null)
            {
                AddError(m_ModelErrors, ModelError.ErrorSeverity.Error, StringResources.StoppedElementNotFound, stopCommand);
            }
        }

        public void VisitWaitTimeCommand(IWaitTimeCommand waitTimeCommand)
        {
            CheckCondition(waitTimeCommand);
        }

        public void VisitWaitConditionCommand(IWaitConditionCommand waitConditionCommand)
        {
            CheckCondition(waitConditionCommand);
            if (waitConditionCommand.AwaitedCondition.Conditional == null || Data.DataModule.ElementRepository.GetElement(waitConditionCommand.AwaitedCondition.Conditional.Id) == null)
            {
                AddError(m_ModelErrors, ModelError.ErrorSeverity.Error, StringResources.AwaitedElementNotFound, waitConditionCommand);
            }
        }

        public void VisitTagCommand(ITagCommand tagCommand)
        {
            CheckCondition(tagCommand);
            try
            {
                int languageId = m_TagLanguage;
                if (m_TagLanguage == -1)
                    m_TagLanguage = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                List<int> tagIds = new List<int>();
                tagIds.Add(tagCommand.TagId);
                var tagInfos = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_TagLanguage).GetTagInfos(tagIds);
                if (tagInfos.Count == 0)
                {
                    AddError(m_ModelErrors, ModelError.ErrorSeverity.Error, StringResources.TagNotFound, tagCommand);
                }
            }
            catch (Ares.Tags.TagsDbException)
            {
                // ignore here
            }
        }

        public void VisitRemoveAllTagsCommand(IRemoveAllTagsCommand command)
        {
            CheckCondition(command);
        }
    }
}
