using System;
using System.Collections.Generic;
using System.Text;

namespace Ares.Data
{
    /// <summary>
    /// Provides access to the main objects of the data module, which are singletons.
    /// </summary>
    public static class DataModule
    {
        /// <summary>
        /// Returns the project manager.
        /// </summary>
        public static IProjectManager ProjectManager { get { return s_ProjectManager;  } }

        /// <summary>
        /// Returns the element factory.
        /// </summary>
        public static IElementFactory ElementFactory { get { return s_ElementFactory; } }

        /// <summary>
        /// Returns the element repository.
        /// </summary>
        public static IElementRepository ElementRepository { get { return s_ElementRepository; } }

        internal static ElementFactory TheElementFactory { get { return s_ElementFactory; } }
        internal static ElementRepository TheElementRepository { get { return s_ElementRepository; } }

        private static ProjectManager s_ProjectManager = new ProjectManager();

        private static ElementFactory s_ElementFactory = new ElementFactory();

        private static ElementRepository s_ElementRepository = new ElementRepository();
    }
}
