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

        public override void DoChecks(IProject project, IModelErrors errors)
        {
            m_ModelErrors = errors;
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

        public void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
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
    }
}
