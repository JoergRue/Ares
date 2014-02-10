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

    public enum MessageBoxResult
    {
        Yes,
        No,
        Cancel
    }

    public interface IMessageBoxProvider
    {
        MessageBoxResult ShowYesNoCancelBox(String prompt);
    }

    public class Importer
    {
        private IProgressMonitor m_Monitor;
        private System.ComponentModel.BackgroundWorker m_Worker;
        private System.Action<Exception, bool> m_DataLoadedFunc;
		
        public static void Import(IProgressMonitor monitor, 
		                          String importFileName, String targetFileName,
                                  bool silent, IMessageBoxProvider messageBoxProvider,
            					  System.Action<Exception, bool> dataLoaded)
        {
            Importer importer = new Importer();
            importer.DoImport(monitor, importFileName, targetFileName, silent, messageBoxProvider, dataLoaded);
        }

        private Importer()
        {
        }

        private void DoImport(IProgressMonitor monitor, 
		                      String importFileName, String targetFileName,
                              bool silent, IMessageBoxProvider messageBoxProvider,
            				  System.Action<Exception, bool> dataLoaded)
        {
            m_Monitor = monitor;
            m_DataLoadedFunc = dataLoaded;
            try
            {
                long overallSize = 0;
                bool overWrite = false;
                bool hasAskedForOverwrite = silent;
                bool hasInnerFile = false;
                using (ZipFile file = new ZipFile(importFileName))
                {
                    for (int i = 0; i < file.Count; ++i)
                    {
                        ZipEntry entry = file[i];
                        if (entry.IsDirectory)
                            continue;
                        bool isTagsDbFile;
                        String fileName = GetFileName(entry, out isTagsDbFile);
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
                        else if (isTagsDbFile)
                        {
                            overallSize += entry.Size;
                        }
                        else if (System.IO.File.Exists(fileName))
                        {
                            if (!hasAskedForOverwrite)
                            {
                                switch (messageBoxProvider.ShowYesNoCancelBox(StringResources.ImportOverwrite))
                                {
                                    case MessageBoxResult.Yes:
                                        overWrite = true;
                                        hasAskedForOverwrite = true;
                                        break;
                                    case MessageBoxResult.No:
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
                m_Monitor.SetProgress(0, String.Empty);
                m_Worker.RunWorkerAsync(data);
            }
            catch (Exception ex)
            {
                if (dataLoaded != null)
                {
                    dataLoaded(ex, false);
                }
            }
        }

        void m_Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (m_DataLoadedFunc != null)
            {
                m_DataLoadedFunc(e.Error, e.Cancelled);
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
            String tagsDbFile = String.Empty;
            using (ZipInputStream stream = new ZipInputStream(System.IO.File.OpenRead(data.ImportFile)))
            {
                ZipEntry entry;
                byte[] buffer = new byte[4096];
                long writtenSize = 0;
                int lastPercent = -1;
                String lastFile = String.Empty;
                while ((entry = stream.GetNextEntry()) != null)
                {
                    bool isTagsDBFile;
                    String fileName = GetFileName(entry, out isTagsDBFile);
                    if (fileName == String.Empty)
                    {
                        fileName = data.TargetFile;
                    }
                    else if (isTagsDBFile)
                    {
                        tagsDbFile = fileName;
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
                                    m_Worker.ReportProgress(currentPercent, lastFile);
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
            if (!String.IsNullOrEmpty(tagsDbFile))
            {
                String logFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "arestagsdbimport.log");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFileName))
                {
                    Ares.Tags.TagsModule.GetTagsDB().FilesInterface.ImportDatabase(tagsDbFile, writer);
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

        private String GetFileName(ZipEntry entry, out bool isTagsDBFile)
        {
            String fileName = String.Empty;
            if (entry.Name.StartsWith(Exporter.MUSIC_DIR))
            {
                fileName = entry.Name.Substring(Exporter.MUSIC_DIR.Length + 1);
                if (fileName.Equals("aresmusictags.jsv", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = System.IO.Path.GetTempFileName();
                    isTagsDBFile = true;
                }
                else
                {
                    fileName = fileName.Replace('/', System.IO.Path.DirectorySeparatorChar);
                    fileName = System.IO.Path.Combine(Settings.Settings.Instance.MusicDirectory, fileName);
                    isTagsDBFile = false;
                }
            }
            else if (entry.Name.StartsWith(Exporter.SOUND_DIR))
            {
                fileName = entry.Name.Substring(Exporter.SOUND_DIR.Length + 1);
                fileName = fileName.Replace('/', System.IO.Path.DirectorySeparatorChar);
                fileName = System.IO.Path.Combine(Settings.Settings.Instance.SoundDirectory, fileName);
                isTagsDBFile = false;
            }
            else
            {
                isTagsDBFile = false;
            }
            return fileName;
        }
    }

    #endregion

    #region Export
	
#if !MONO
	
    public class Exporter
    {
        private IProgressMonitor m_Monitor;
        private System.ComponentModel.BackgroundWorker m_Worker;
        private Action<Exception> m_FinishedAction;

        public static String MUSIC_DIR = "Music";
        public static String SOUND_DIR = "Sounds";

        public static void Export(IProgressMonitor monitor, Object data, String innerFileName, String exportFileName, Action<Exception> finishedAction)
        {
            Exporter exporter = new Exporter();
            ExportData exportData = new ExportData();
            exportData.Data = data;
            exportData.InnerFileName = innerFileName;
            exportData.ExportFileName = exportFileName;
            exporter.DoExport(monitor, exportData, finishedAction);
        }

        private void m_Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (m_FinishedAction != null)
            {
                m_FinishedAction(e.Error);
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

        private void DoExport(IProgressMonitor monitor, ExportData exportData, Action<Exception> finishedAction)
        {
            m_FinishedAction = finishedAction;
            m_Worker = new System.ComponentModel.BackgroundWorker();
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_Worker_DoWork);
            m_Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(m_Worker_ProgressChanged);
            m_Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(m_Worker_RunWorkerCompleted);
            m_Monitor = monitor;
            m_Monitor.SetProgress(0, String.Empty);
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

            List<String> musicFiles = new List<string>();

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
                if (element.FilePath.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase) ||
                    element.FilePath.EndsWith(".m3u8", StringComparison.InvariantCultureIgnoreCase) ||
                    element.FilePath.EndsWith(".pls", StringComparison.InvariantCultureIgnoreCase))
                {
                    List<String> filesInPlaylist = Playlists.ReadPlaylist(fileName, (String error) =>
                        {
                            throw new System.IO.IOException(error);
                        });
                    if (filesInPlaylist != null)
                    {
                        String pathStart = element.SoundFileType == SoundFileType.Music ?
                            Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
                        foreach (String playlistFile in filesInPlaylist)
                        {
                            if (playlistFile.StartsWith(pathStart, StringComparison.InvariantCultureIgnoreCase))
                            {
                                name = element.SoundFileType == SoundFileType.Music ? MUSIC_DIR : SOUND_DIR;
                                name = System.IO.Path.Combine(name, playlistFile.Substring(pathStart.Length + 1));
                                if (element.SoundFileType == SoundFileType.Music)
                                {
                                    musicFiles.Add(playlistFile.Substring(pathStart.Length + 1));
                                }
                                ZipEntry playlistFileEntry = new ZipEntry(ZipEntry.CleanName(name));
                                SetZipEntryAttributes(playlistFileEntry, playlistFile, out size);
                                zipEntries[playlistFileEntry] = playlistFile;
                                zipSizes[playlistFileEntry] = size;
                                overallSize += size;
                            }
                        }
                    }
                }
                else if (element.SoundFileType == SoundFileType.Music)
                {
                    musicFiles.Add(element.FilePath);
                }
            }

            // export tags database
            String tagFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "aresmusictags.jsv");
            Ares.Tags.TagsModule.GetTagsDB().FilesInterface.ExportDatabase(musicFiles, tagFileName);
            String tagFileEntryName = System.IO.Path.Combine(MUSIC_DIR, "aresmusictags.jsv");
            ZipEntry tagFileEntry = new ZipEntry(ZipEntry.CleanName(tagFileEntryName));
            SetZipEntryAttributes(tagFileEntry, tagFileName, out size);
            zipEntries[tagFileEntry] = tagFileName;
            zipSizes[tagFileEntry] = size;
            overallSize += size;

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
                                m_Worker.ReportProgress(currentPercent, lastFile);
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
	
#else
	
	public class Exporter
    {
        public static String MUSIC_DIR = "Music";
        public static String SOUND_DIR = "Sounds";
	}
	
#endif

    #endregion
	
}
