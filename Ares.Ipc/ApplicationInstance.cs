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

namespace Ares.Ipc
{
    /// <summary>
    /// Assures that only one instance of an application is active.
    /// Can tell that application instance to load a new project.
    /// </summary>
    public class ApplicationInstance : IDisposable
    {
        #region Public Static Interface

        /// <summary>
        /// For use in the own application. If an object is returned, it
        /// serves as a handle which must be disposed once the application 
        /// is closed.
        /// If no object (null) is returned, another instance was already 
        /// active and has been activated.
        /// </summary>
        /// <param name="appName">Identifier for the application</param>
        /// <returns>Single object for the application</returns>
        public static ApplicationInstance CreateOrActivate(String appName)
        {
            while (true)
            {
                try
                {
                    return new ApplicationInstance(appName);
                }
                catch (Exception)
                {
                    if (Activate(appName))
                    {
                        return null;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }
        }

        /// <summary>
        /// For use in the own application. If an object is returned, it
        /// serves as a handle which must be disposed once the application 
        /// is closed.
        /// If no object (null) is returned, another instance was already 
        /// active and has been activated, and was asked to open the project.
        /// </summary>
        /// <param name="appName">Identifier for the application</param>
        /// <param name="project">Path to a project file</param>
        /// <returns>Single object for the application</returns>
        public static ApplicationInstance CreateOrActivate(String appName, String projectPath)
        {
            while (true)
            {
                try
                {
                    return new ApplicationInstance(appName);
                }
                catch (Exception)
                {
                    if (Activate(appName, System.IO.Path.GetFileName(projectPath), projectPath))
                    {
                        return null;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }
        }

        /// <summary>
        /// For use in other applications. Starts the specified application if necessary,
        /// else activates it. Tells the application that a project shall be loaded.
        /// </summary>
        /// <param name="appName">Identifier for the application</param>
        /// <param name="appPath">Path to the executable</param>
        /// <param name="project">Path to a project file</param>
        /// <returns>true on success, false if the application could neither be activated nor started</returns>
        public static void CreateOrActivate(String appName, String appPath, String projectName, String projectPath)
        {
            if (!Activate(appName, projectName, projectPath))
            {
                System.Diagnostics.Process.Start(appPath, "\"" + projectPath + "\"");
            }
        }

        #endregion

        #region Sending to other process

        /// <summary>
        /// Tries to activate the other process
        /// </summary>
        private static bool Activate(String appName)
        {
            ApplicationInfo info = GetApplicationInfo(appName);
            if (info != null)
            {
                NativeWindowMethods.SwitchToThisWindow(info.WindowHandle, false);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to get information about the other process
        /// </summary>
        private static ApplicationInfo GetApplicationInfo(String appName)
        {
            try
            {
                using (SharedMemory mem = new SharedMemory(appName))
                {
                    ApplicationInfo info = (ApplicationInfo)mem.GetObject();
                    return info;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Tries to load a project in another process and activate it.
        /// </summary>
        private static bool Activate(String appName, String projectName, string projectPath)
        {
            ApplicationInfo info = GetApplicationInfo(appName);
            if (info == null)
                return false;
            if (info.LoadedProject != projectPath && !String.IsNullOrEmpty(projectName) && !String.IsNullOrEmpty(projectPath))
            {
                if (!SendProjectLoadRequest(appName, projectName, projectPath))
                    return false;
            }
            NativeWindowMethods.SwitchToThisWindow(info.WindowHandle, false);
            return true;
        }

        /// <summary>
        /// Tries to load a project in another process.
        /// </summary>
        private static bool SendProjectLoadRequest(String appName, String projectName, String projectPath)
        {
            try
            {
                using (System.IO.Pipes.NamedPipeClientStream stream = new System.IO.Pipes.NamedPipeClientStream(".", appName + "_Pipe", System.IO.Pipes.PipeDirection.Out))
                {
                    stream.Connect(300);
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(projectName);
                        writer.WriteLine(projectPath);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Giving info to other process

        private SharedMemory m_Memory;
        private System.IO.Pipes.NamedPipeServerStream m_PipeStream;
        private IAsyncResult m_AsyncResult;

        private readonly String m_Appname;
        private ApplicationInfo m_Info;

        private ApplicationInstance(String appName)
        {
            m_Info = new ApplicationInfo(IntPtr.Zero, String.Empty);
            m_Appname = appName;
            m_Memory = new SharedMemory(appName, m_Info);
            CreatePipe();
        }

        private void CreatePipe()
        {
            m_PipeStream = new System.IO.Pipes.NamedPipeServerStream(m_Appname + "_Pipe", System.IO.Pipes.PipeDirection.In,
                1, System.IO.Pipes.PipeTransmissionMode.Byte, System.IO.Pipes.PipeOptions.Asynchronous);
            m_AsyncResult = m_PipeStream.BeginWaitForConnection(new AsyncCallback(result => HandleConnection(result)), null);
        }

        /// <summary>
        /// Sets the window handle of the application
        /// </summary>
        public bool SetWindowHandle(IntPtr handle)
        {
            if (m_Disposed)
                throw new ObjectDisposedException("ApplicationInstance");
            if (m_Memory != null)
            {
                m_Memory.Dispose();
                m_Memory = null;
            }
            try
            {
                m_Info.WindowHandle = handle;
                m_Memory = new SharedMemory(m_Appname, m_Info);
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the currently loaded project
        /// </summary>
        public bool SetLoadedProject(String project)
        {
            if (m_Disposed)
                throw new ObjectDisposedException("ApplicationInstance");
            if (m_Memory != null)
            {
                m_Memory.Dispose();
                m_Memory = null;
            }
            try
            {
                m_Info.LoadedProject = project;
                m_Memory = new SharedMemory(m_Appname, m_Info);
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        #endregion

        #region Receiving commands from other applications

        public Action<String, String> ProjectOpenAction { get; set; }

        private void HandleConnection(IAsyncResult result)
        {
            String projectName = String.Empty;
            String projectPath = String.Empty;
            try
            {
                if (m_PipeStream != null)
                {
                    m_PipeStream.EndWaitForConnection(result);
                    if (m_PipeStream.IsConnected)
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(m_PipeStream))
                        {
                            projectName = reader.ReadLine();
                            projectPath = reader.ReadLine();
                            
                        }
                    }
                    // the pipe has been closed now!
                    CreatePipe();
                }
            }
            catch (Exception )
            {
                projectName = String.Empty;
                projectPath = String.Empty;
            }
            if (ProjectOpenAction != null && !String.IsNullOrEmpty(projectName) && !String.IsNullOrEmpty(projectPath))
            {
                ProjectOpenAction(projectName, projectPath);
            }
        }

        #endregion

        #region Dispose / Finalize

        private bool m_Disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ApplicationInstance()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_PipeStream != null)
                {
                    if (m_AsyncResult != null)
                    {
                        try
                        {
                            m_PipeStream.EndWaitForConnection(m_AsyncResult);
                        }
                        catch (Exception)
                        { 
                        }
                    }
                    m_PipeStream.Dispose();
                    m_PipeStream = null;
                }
                if (m_Memory != null)
                {
                    m_Memory.Dispose();
                    m_Memory = null;
                }
            }
            m_Disposed = true;
        }

        #endregion

    }

    /// <summary>
    /// Exception which can be used by clients to indicate that they will not start
    /// due to another instance already existing.
    /// </summary>
    public class ApplicationAlreadyStartedException : System.Exception
    {
    }
}
