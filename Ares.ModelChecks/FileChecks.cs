using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ares.Data;

namespace Ares.ModelInfo
{
    class FileChecks : ModelCheck, IElementVisitor
    {
        public FileChecks()
            : base(CheckType.File)
        {
        }

        private IModelErrors m_ModelErrors;

        public override void DoChecks(Data.IProject project, IModelErrors errors)
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
            String basePath = fileElement.SoundFileType == SoundFileType.Music ?
                Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
            String fullPath = System.IO.Path.Combine(basePath, fileElement.FilePath);
            if (!System.IO.File.Exists(fullPath))
            {
                AddError(m_ModelErrors, ModelError.ErrorSeverity.Error,
                    String.Format(StringResources.FileNotFound, fullPath), fileElement);
            }
        }

        public void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
            foreach (IElement element in sequentialContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IElement element in parallelContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
            foreach (IElement element in choiceContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
            foreach (IElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            foreach (IElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }
    }
}
