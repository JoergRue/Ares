using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        void SaveProject();

        /// <summary>
        /// Saves a project to a file.
        /// </summary>
        void SaveProject(String fileName);

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
            throw new NotImplementedException();
        }

        public void SaveProject()
        {
            throw new NotImplementedException();
        }

        public void SaveProject(String fileName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
