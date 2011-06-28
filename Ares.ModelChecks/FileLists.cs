using System;
using System.Collections.Generic;
using Ares.Data;

namespace Ares.ModelInfo
{
    public class FileLists : IElementVisitor
    {
        private Dictionary<String, IFileElement> m_Files = new Dictionary<string, IFileElement>();

        public IList<IFileElement> GetAllFiles(IProject project)
        {
            foreach (IMode mode in project.GetModes())
            {
                foreach (IModeElement element in mode.GetElements())
                {
                    element.Visit(this);
                }
            }
            return new List<IFileElement>(m_Files.Values);
        }

        public IList<String> GetAllFilePaths(IProject project)
        {
            List<String> paths = new List<string>();
            foreach (IFileElement element in GetAllFiles(project))
            {
                String path = element.SoundFileType == SoundFileType.Music ?
                    Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
                path = System.IO.Path.Combine(path, element.FilePath);
                path = System.IO.Path.GetFullPath(path);
                paths.Add(path);
            }
            return paths;
        }

        public void VisitFileElement(IFileElement fileElement)
        {
            String path;
            if (fileElement.SoundFileType == SoundFileType.Music)
            {
                path = Ares.Settings.Settings.Instance.MusicDirectory;
            }
            else
            {
                path = Ares.Settings.Settings.Instance.SoundDirectory;
            }
            path = System.IO.Path.Combine(path, fileElement.FilePath);
            path = System.IO.Path.GetFullPath(path).ToUpperInvariant();
            if (!m_Files.ContainsKey(path))
            {
                m_Files.Add(path, fileElement);
            }
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
            foreach (ISequentialElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            foreach (IChoiceElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }
    }
}
