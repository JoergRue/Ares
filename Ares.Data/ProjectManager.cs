using System;
using System.Collections.Generic;
using System.Xml;

namespace Ares.Data
{
    /// <summary>
    /// Management of ARES projects.
    /// </summary>
    public interface IProjectManager
    {
        /// <summary>
        /// Creates a new project.
        /// </summary>
        IProject CreateProject(String title);

        /// <summary>
        /// Loads a project from a file.
        /// </summary>
        IProject LoadProject(String fileName);

        /// <summary>
        /// Saves a project. The file name must already be set.
        /// </summary>
        void SaveProject(IProject project);

        /// <summary>
        /// Saves a project to a file.
        /// </summary>
        void SaveProject(IProject project, String fileName);

        /// <summary>
        /// Unloads a project.
        /// </summary>
        void UnloadProject(IProject project);

    }

    class ProjectManager : IProjectManager
    {
        #region IProjectManager Members

        public IProject CreateProject(String title)
        {
            return new Project(title);
        }

        public IProject LoadProject(String fileName)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.ProhibitDtd = false;
            using (System.IO.FileStream stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    reader.Read();
                    reader.MoveToElement();
                    return new Project(reader, fileName);
                }
            }
        }

        public void SaveProject(IProject project)
        {
            if (String.IsNullOrEmpty(project.FileName))
            {
                throw new ArgumentException(StringResources.FileNameMustBeSet);
            }
            DoSaveProject(project, project.FileName);
            project.Changed = false;
        }

        public void SaveProject(IProject project, String fileName)
        {
            DoSaveProject(project, fileName);
            project.FileName = fileName;
            project.Changed = false;
        }

        private static void DoSaveProject(IProject project, String fileName)
        {
            String tempFileName = System.IO.Path.GetTempFileName();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(tempFileName, settings))
            {
                writer.WriteStartDocument();
                project.WriteToXml(writer);
                writer.WriteEndDocument();
                writer.Flush();
            }
            System.IO.File.Copy(tempFileName, fileName, true);
            System.IO.File.Delete(tempFileName);
        }

        private class ElementRemover : IElementVisitor
        {
            public ElementRemover()
            {
                repository = DataModule.TheElementRepository;
            }

            private ElementRepository repository;

            public void VisitFileElement(IFileElement fileElement)
            {
                repository.DeleteElement((fileElement as IElement).Id);
            }

            public void VisitSequentialContainer(IElementContainer<ISequentialElement> sequentialContainer)
            {
                repository.DeleteElement(sequentialContainer.Id);
                foreach (ISequentialElement element in sequentialContainer.GetElements())
                {
                    element.Visit(this);
                }
            }

            public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
            {
                repository.DeleteElement(parallelContainer.Id);
                foreach (IParallelElement element in parallelContainer.GetElements())
                {
                    element.Visit(this);
                }
            }

            public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
            {
                repository.DeleteElement(choiceContainer.Id);
                foreach (IChoiceElement element in choiceContainer.GetElements())
                {
                    element.Visit(this);
                }
            }

            public void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
            {
                musicList.VisitElements(this);
            }

            public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
            {
                musicList.VisitElements(this);
            }
        }

        public void UnloadProject(IProject project)
        {
            ElementRemover remover = new ElementRemover();
            foreach (IMode mode in project.GetModes())
            {
                foreach (IModeElement modeElement in mode.GetElements())
                {
                    modeElement.StartElement.Visit(remover);   
                }
            }
        }

        #endregion
    }
}
