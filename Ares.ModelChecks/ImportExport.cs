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
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace Ares.ModelInfo
{

    #region Import

    public class Importer
    {
        private ProgressMonitor m_Monitor;
        private System.ComponentModel.BackgroundWorker m_Worker;
        private System.Action m_DataLoadedFunc;

        public static void Import(System.Windows.Forms.Form parent, String importFileName, String targetFileName,
            System.Action dataLoaded)
        {
            Importer importer = new Importer();
            importer.DoImport(parent, importFileName, targetFileName, dataLoaded);
        }

        private Importer()
        {
        }

        private void DoImport(System.Windows.Forms.Form parent, String importFileName, String targetFileName,
            System.Action dataLoaded)
        {
            m_DataLoadedFunc = dataLoaded;
            try
            {
                long overallSize = 0;
                bool overWrite = false;
                bool hasAskedForOverwrite = false;
                bool hasInnerFile = false;
                using (ZipFile file = new ZipFile(importFileName))
                {
                    for (int i = 0; i < file.Count; ++i)
                    {
                        ZipEntry entry = file[i];
                        if (entry.IsDirectory)
                            continue;
                        String fileName = GetFileName(entry);
                        if (fileName == String.Empty)
                        {
                            if (hasInnerFile)
                            {
                                throw new ArgumentException(StringResources.InvalidImportFile);
                            }
                            else
                            {
                                hasInnerFile = true;
                                overallSize += entry.Size;
                            }
                        }
                        else if (System.IO.File.Exists(fileName))
                        {
                            if (!hasAskedForOverwrite)
                            {
                                switch (System.Windows.Forms.MessageBox.Show(StringResources.ImportOverwrite, StringResources.Ares,
                                    System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Question))
                                {
                                    case System.Windows.Forms.DialogResult.Yes:
                                        overWrite = true;
                                        hasAskedForOverwrite = true;
                                        break;
                                    case System.Windows.Forms.DialogResult.No:
                                        overWrite = false;
                                        hasAskedForOverwrite = true;
                                        break;
                                    default:
                                        return;
                                }
                            }
                            if (overWrite)
                                overallSize += entry.Size;
                        }
                        else
                        {
                            overallSize += entry.Size;
                        }
                    }
                    if (!hasInnerFile)
                    {
                        throw new ArgumentException(StringResources.InvalidImportFile);
                    }
                }
                ImportData data = new ImportData();
                data.Overwrite = overWrite;
                data.ImportFile = importFileName;
                data.TargetFile = targetFileName;
                data.OverallSize = overallSize;
                m_Worker = new System.ComponentModel.BackgroundWorker();
                m_Worker.WorkerReportsProgress = true;
                m_Worker.WorkerSupportsCancellation = true;
                m_Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_Worker_DoWork);
                m_Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(m_Worker_ProgressChanged);
                m_Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(m_Worker_RunWorkerCompleted);
                m_Monitor = new ProgressMonitor(parent, StringResources.Importing);
                m_Worker.RunWorkerAsync(data);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(StringResources.ImportError, ex.Message), StringResources.Ares,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        void m_Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            m_Monitor.Close();
            if (e.Error != null)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(StringResources.ImportError, e.Error.Message), StringResources.Ares,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            else if (!e.Cancelled)
            {
                m_DataLoadedFunc();
            }
        }

        void m_Worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (m_Monitor.Canceled)
            {
                m_Worker.CancelAsync();
            }
            else
            {
                m_Monitor.SetProgress(e.ProgressPercentage, e.UserState.ToString());
            }
        }

        void m_Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ImportData data = (ImportData)e.Argument;
            using (ZipInputStream stream = new ZipInputStream(System.IO.File.OpenRead(data.ImportFile)))
            {
                ZipEntry entry;
                byte[] buffer = new byte[4096];
                long writtenSize = 0;
                int lastPercent = -1;
                String lastFile = String.Empty;
                while ((entry = stream.GetNextEntry()) != null)
                {
                    String fileName = GetFileName(entry);
                    if (fileName == String.Empty)
                    {
                        fileName = data.TargetFile;
                    }
                    else if (!data.Overwrite && System.IO.File.Exists(fileName))
                    {
                        continue;
                    }
                    String directoryName = System.IO.Path.GetDirectoryName(fileName);
                    if (directoryName.Length > 0)
                    {
                        System.IO.Directory.CreateDirectory(directoryName);
                    }
                    using (System.IO.FileStream fileStream = System.IO.File.Create(fileName))
                    {
                        StreamUtils.Copy(stream, fileStream, buffer, new ProgressHandler((object o, ProgressEventArgs e2) =>
                            {
                                long currentSize = writtenSize + e2.Processed;
                                int currentPercent = (int)(((double)currentSize / (double)data.OverallSize) * 100.0);
                                if (currentPercent != lastPercent || e2.Name != lastFile)
                                {
                                    lastPercent = currentPercent;
                                    lastFile = e2.Name;
                                    m_Worker.ReportProgress(lastPercent, lastFile);
                                    e2.ContinueRunning = !m_Worker.CancellationPending;
                                }
                            }), TimeSpan.FromMilliseconds(50), entry, fileName, entry.Size);
                    }
                    writtenSize += entry.Size;
                    if (m_Worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
        }

        class ImportData
        {
            public bool Overwrite { get; set; }
            public String ImportFile { get; set; }
            public String TargetFile { get; set; }
            public long OverallSize { get; set; }
        }

        private String GetFileName(ZipEntry entry)
        {
            String fileName = String.Empty;
            if (entry.Name.StartsWith(Exporter.MUSIC_DIR))
            {
                fileName = entry.Name.Substring(Exporter.MUSIC_DIR.Length + 1);
                fileName = fileName.Replace('/', System.IO.Path.DirectorySeparatorChar);
                fileName = System.IO.Path.Combine(Settings.Settings.Instance.MusicDirectory, fileName);
            }
            else if (entry.Name.StartsWith(Exporter.SOUND_DIR))
            {
                fileName = entry.Name.Substring(Exporter.SOUND_DIR.Length + 1);
                fileName = fileName.Replace('/', System.IO.Path.DirectorySeparatorChar);
                fileName = System.IO.Path.Combine(Settings.Settings.Instance.SoundDirectory, fileName);
            }
            return fileName;
        }
    }

    #endregion

    #region Export

    public class Exporter
    {
        private ProgressMonitor m_Monitor;
        private System.ComponentModel.BackgroundWorker m_Worker;

        public static String MUSIC_DIR = "Music";
        public static String SOUND_DIR = "Sounds";

        public static void Export(System.Windows.Forms.Form parent, Object data, String innerFileName, String exportFileName)
        {
            Exporter exporter = new Exporter();
            ExportData exportData = new ExportData();
            exportData.Data = data;
            exportData.InnerFileName = innerFileName;
            exportData.ExportFileName = exportFileName;
            exporter.DoExport(parent, exportData);
        }

        private void m_Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            m_Monitor.Close();
            if (e.Error != null)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(StringResources.ExportError, e.Error.Message), StringResources.Ares,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void m_Worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (m_Monitor.Canceled)
            {
                m_Worker.CancelAsync();
            }
            else
            {
                m_Monitor.SetProgress(e.ProgressPercentage, e.UserState.ToString());
            }
        }

        private static void SetZipEntryAttributes(ZipEntry entry, String file, out long size)
        {
            entry.CompressionMethod = CompressionMethod.Deflated;
            entry.DateTime = System.IO.File.GetLastWriteTime(file);
            size = (new System.IO.FileInfo(file)).Length;
        }

        private class ExportData
        {
            public String InnerFileName { get; set; }
            public String ExportFileName { get; set; }
            public Object Data { get; set; }
        }

        private void DoExport(System.Windows.Forms.Form parent, ExportData exportData)
        {
            m_Worker = new System.ComponentModel.BackgroundWorker();
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_Worker_DoWork);
            m_Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(m_Worker_ProgressChanged);
            m_Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(m_Worker_RunWorkerCompleted);
            m_Monitor = new ProgressMonitor(parent, StringResources.Exporting);
            m_Worker.RunWorkerAsync(exportData);
        }

        private Exporter()
        {
        }

        private void m_Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ExportData data = (ExportData)e.Argument;
            long overallSize = 0;

            // collect entries and calculate overall size
            Dictionary<ZipEntry, String> zipEntries = new Dictionary<ZipEntry, string>();
            Dictionary<ZipEntry, long> zipSizes = new Dictionary<ZipEntry, long>();

            // project file: no path
            ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(data.InnerFileName));
            long size;
            SetZipEntryAttributes(entry, data.InnerFileName, out size);
            zipEntries[entry] = data.InnerFileName;
            zipSizes[entry] = size;
            overallSize += size;

            // entries for the music and sound files
            Ares.ModelInfo.FileLists fileLists = new Ares.ModelInfo.FileLists();
            foreach (IFileElement element in fileLists.GetAllFiles(data.Data))
            {
                String name = element.SoundFileType == SoundFileType.Music ? MUSIC_DIR : SOUND_DIR;
                name = System.IO.Path.Combine(name, element.FilePath);
                String fileName = element.SoundFileType == SoundFileType.Music ?
                    Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
                fileName = System.IO.Path.Combine(fileName, element.FilePath);
                ZipEntry fileEntry = new ZipEntry(ZipEntry.CleanName(name));
                SetZipEntryAttributes(fileEntry, fileName, out size);
                zipEntries[fileEntry] = fileName;
                zipSizes[fileEntry] = size;
                overallSize += size;
            }

            // now write zip file
            long writtenSize = 0;
            int lastPercent = -1;
            String lastFile = String.Empty;

            using (ZipOutputStream stream = new ZipOutputStream(System.IO.File.Create(data.ExportFileName)))
            {
                stream.SetLevel(7);
                byte[] buffer = new byte[4096];
                foreach (KeyValuePair<ZipEntry, String> zipEntry in zipEntries)
                {
                    stream.PutNextEntry(zipEntry.Key);
                    using (System.IO.FileStream fileStream = System.IO.File.OpenRead(zipEntry.Value))
                    {
                        StreamUtils.Copy(fileStream, stream, buffer, new ProgressHandler((object o, ProgressEventArgs e2) =>
                        {
                            long currentSize = writtenSize + e2.Processed;
                            int currentPercent = (int)(((double)currentSize / (double)overallSize) * 100.0);
                            if (currentPercent != lastPercent || e2.Name != lastFile)
                            {
                                lastPercent = currentPercent;
                                lastFile = e2.Name;
                                m_Worker.ReportProgress(lastPercent, lastFile);
                                e2.ContinueRunning = !m_Worker.CancellationPending;
                            }
                        }), TimeSpan.FromMilliseconds(50), zipEntry.Key, zipEntry.Value, zipEntry.Key.Size);
                    }
                    writtenSize += zipSizes[zipEntry.Key];
                    if (m_Worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                }
                stream.Finish();
                stream.Close();
            }
        }
    }

    #endregion

    #region Copy / Move

    public class FileOperations
    {
        private ProgressMonitor m_Monitor;
        private System.ComponentModel.BackgroundWorker m_Worker;

        public static void CopyOrMove(System.Windows.Forms.Form parent, System.Collections.Generic.Dictionary<String, object> uniqueElements, bool move, String targetPath, Action completedAction)
        {
            FileOperations privateInstance = new FileOperations(completedAction);
            FileOpData data = new FileOpData();
            data.TargetPath = targetPath;
            data.UniqueElements = uniqueElements;
            data.Move = move;
            privateInstance.DoCopyOrMove(parent, data);
        }

        private class FileOpData
        {
            public System.Collections.Generic.Dictionary<String, object> UniqueElements { get; set; }
            public bool Move { get; set; }
            public String TargetPath { get; set; }
        }

        private FileOperations(Action completedAction)
        {
            m_CompletedAction = completedAction;
        }

        private Action m_CompletedAction;


        private void DoCopyOrMove(System.Windows.Forms.Form parent, FileOpData data)
        {
            m_Monitor = new ProgressMonitor(parent, data.Move ? StringResources.Moving : StringResources.Copying);
            m_Worker = new System.ComponentModel.BackgroundWorker();
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_Worker_DoWork);
            m_Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(m_Worker_ProgressChanged);
            m_Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(m_Worker_RunWorkerCompleted);
            m_Worker.RunWorkerAsync(data);
        }

        void m_Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            m_Monitor.Close();
            if (e.Error != null)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(StringResources.FileOpError, e.Error.Message), StringResources.Ares,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            if (m_CompletedAction != null)
                m_CompletedAction();
        }

        void m_Worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (m_Monitor.Canceled)
            {
                m_Worker.CancelAsync();
            }
            else
            {
                m_Monitor.SetProgress(e.ProgressPercentage, e.UserState.ToString());
            }
        }

        private void m_Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            FileOpData data = (FileOpData)e.Argument;

            // determine amount to copy / move
            long allBytes = 0;
            foreach (String file in data.UniqueElements.Keys)
            {
                allBytes += GetBytes(file);
            }

            System.Collections.Generic.Dictionary<String, String> filesMoved = new Dictionary<String, String>();

            long currentBytes = 0;
            int lastPercent = -1;
            foreach (String file in data.UniqueElements.Keys)
            {
                String nameOnly = System.IO.Path.GetFileName(file);
                String fileTargetPath = System.IO.Path.Combine(data.TargetPath, nameOnly);
                if (file.Equals(fileTargetPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    currentBytes += GetBytes(fileTargetPath);
                    ReportProgress(currentBytes, allBytes, ref lastPercent, fileTargetPath);
                    continue;
                }
                if (System.IO.Directory.Exists(file))
                {
                    if (data.Move)
                    {
                        AddMovedFiles(file, fileTargetPath, filesMoved);
                        System.IO.Directory.Move(file, fileTargetPath);
                        currentBytes += GetBytes(fileTargetPath);
                        ReportProgress(currentBytes, allBytes, ref lastPercent, fileTargetPath);
                    }
                    else
                    {
                        CopyDirectory(file, fileTargetPath, ref currentBytes, allBytes, ref lastPercent);
                    }
                }
                else if (System.IO.File.Exists(file))
                {
                    if (data.Move)
                    {
                        filesMoved[file] = fileTargetPath;
                        System.IO.File.Move(file, fileTargetPath);
                    }
                    else
                    {
                        System.IO.File.Copy(file, fileTargetPath, true);
                    }
                    currentBytes += GetBytes(fileTargetPath);
                    ReportProgress(currentBytes, allBytes, ref lastPercent, fileTargetPath);
                }
                if (m_Worker.CancellationPending)
                    return;
            }

            if (data.Move)
            {
                AdaptElementPaths(filesMoved);
            }
        }

        private static void AdaptElementPaths(Dictionary<String, String> filesMoved)
        {
            if (ModelChecks.Instance.Project == null)
                return;

            FileLists lists = new FileLists();
            IList<IFileElement> elements = lists.GetAllFiles(ModelChecks.Instance.Project);
            foreach (IFileElement element in elements)
            {
                String basePath = element.SoundFileType == SoundFileType.Music ? Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
                String currentPath = System.IO.Path.Combine(basePath, element.FilePath);
                if (filesMoved.ContainsKey(currentPath))
                {
                    String newPath = filesMoved[currentPath];
                    if (newPath.StartsWith(basePath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        element.FilePath = newPath.Substring(basePath.Length + 1);
                    }
                }
            }
        }

        private void ReportProgress(long currentBytes, long allBytes, ref int lastPercent, String file)
        {
            int currentPercent = (int)(((double)currentBytes / (double)allBytes) * 100.0);
            if (currentPercent != lastPercent)
            {
                lastPercent = currentPercent;
                m_Worker.ReportProgress(lastPercent, System.IO.Path.GetFileName(file));
            }
        }

        private static long GetBytes(String file)
        {
            if (System.IO.Directory.Exists(file))
            {
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(file);
                return GetBytes(info);
            }
            else if (System.IO.File.Exists(file))
            {
                return (new System.IO.FileInfo(file)).Length;
            }
            else
                return 0;
        }

        private static long GetBytes(System.IO.DirectoryInfo directoryInfo)
        {
            long sum = 0;
            foreach (System.IO.DirectoryInfo info in directoryInfo.GetDirectories())
                sum += GetBytes(info);
            foreach (System.IO.FileInfo info in directoryInfo.GetFiles())
                sum += info.Length;
            return sum;
        }

        private void CopyDirectory(String source, String destination, ref long currentBytes, long allBytes, ref int lastPercent)
        {
            if (!System.IO.Directory.Exists(destination))
            {
                System.IO.Directory.CreateDirectory(destination);
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(source);
                foreach (System.IO.DirectoryInfo subDir in info.GetDirectories())
                {
                    CopyDirectory(subDir.FullName, System.IO.Path.Combine(destination, subDir.Name), ref currentBytes, allBytes, ref lastPercent);
                    if (m_Worker.CancellationPending)
                        return;
                }
                foreach (System.IO.FileInfo file in info.GetFiles())
                {
                    System.IO.File.Copy(file.FullName, System.IO.Path.Combine(destination, file.Name), true);
                    currentBytes += GetBytes(file.FullName);
                    ReportProgress(currentBytes, allBytes, ref lastPercent, file.FullName);
                    if (m_Worker.CancellationPending)
                        return;
                }
            }
        }

        private static void AddMovedFiles(String source, String destination, Dictionary<String, String> movedFiles)
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(source);
            foreach (System.IO.DirectoryInfo subDir in info.GetDirectories())
            {
                AddMovedFiles(subDir.FullName, System.IO.Path.Combine(destination, subDir.Name), movedFiles);
            }
            foreach (System.IO.FileInfo file in info.GetFiles())
            {
                movedFiles[file.FullName] = System.IO.Path.Combine(destination, file.Name);
            }
        }
    }

    #endregion

    #region Progress

    class ProgressMonitor
    {
        private System.Timers.Timer timer;
        private bool canceled;
        private bool closed;
        private ProgressDialog dialog;
        private System.Windows.Forms.Form parent;

        private const int DELAY = 700;

        private Object syncObject = new object();

        public ProgressMonitor(System.Windows.Forms.Form parent, String text)
        {
            dialog = new ProgressDialog(text);
            dialog.Text = text;
            closed = false;
            canceled = false;
            this.parent = parent;
            timer = new System.Timers.Timer(DELAY);
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (syncObject)
            {
                if (closed)
                    return;
            }
            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => { OpenDialog(); }));
            }
            else
            {
                OpenDialog();
            }
        }

        private void OpenDialog()
        {
            lock (syncObject)
            {
                if (closed)
                    return;
            }
            DialogClosed(dialog.ShowDialog(parent));
        }

        private void DialogClosed(System.Windows.Forms.DialogResult result)
        {
            lock (syncObject)
            {
                if (!closed)
                    canceled = true;
            }
        }

        public bool Canceled
        {
            get
            {
                lock (syncObject)
                {
                    return canceled;
                }
            }
        }

        public void Close()
        {
            lock (syncObject)
            {
                closed = true;
                if (timer.Enabled)
                {
                    timer.Stop();
                }
            }
            if (dialog != null && dialog.InvokeRequired)
            {
                dialog.Invoke(new Action(() => { Close(); }));
            }
            else
            {
                if (dialog != null && dialog.Visible)
                {
                    dialog.Visible = false;
                    dialog.Close();
                }
                timer.Dispose();
            }
        }

        public void SetProgress(int progress, String text)
        {
            if (dialog.InvokeRequired)
            {
                dialog.Invoke(new Action(() => { dialog.SetProgress(progress, text); }));
            }
            else
            {
                dialog.SetProgress(progress, text);
            }
        }
    }

    #endregion
}
