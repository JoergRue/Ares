using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using Ares.ModelInfo;
using Ares.Data;

namespace Ares.TagsImport
{
    public class FileOperations
    {
        private IProgressMonitor m_Monitor;
        private CancellationToken m_Token;

        public static Task CopyOrMoveAsync(IProgressMonitor monitor, Ares.Data.IProject project, String musicDir, String soundsDir, System.Collections.Generic.Dictionary<String, object> uniqueElements, 
            bool move, String targetPath, CancellationToken token)
        {
            FileOperations privateInstance = new FileOperations(monitor, token);
            FileOpData data = new FileOpData();
            data.TargetPath = targetPath;
            data.UniqueElements = uniqueElements;
            data.Project = project;
            data.Move = move;
            data.MusicDirectory = musicDir;
            data.SoundsDirectory = soundsDir;
            return Task.Factory.StartNew(() => { privateInstance.DoCopyOrMove(data); });
        }

        private class FileOpData
        {
            public System.Collections.Generic.Dictionary<String, object> UniqueElements { get; set; }
            public bool Move { get; set; }
            public String TargetPath { get; set; }
            public Ares.Data.IProject Project { get; set; }
            public String MusicDirectory { get; set; }
            public String SoundsDirectory { get; set; }
        }

        private FileOperations(IProgressMonitor monitor, CancellationToken token)
        {
            m_Monitor = monitor;
            m_Token  = token;
        }

        private void DoCopyOrMove(FileOpData data)
        {
            m_Monitor.SetProgress(0, String.Empty);

            // determine amount to copy / move
            long allBytes = 0;
            foreach (String file in data.UniqueElements.Keys)
            {
                allBytes += GetBytes(file);
            }

            System.Collections.Generic.Dictionary<String, String> filesMoved = new Dictionary<String, String>();

            long currentBytes = 0;
            int lastPercent = 0;
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
                    AddMovedFiles(file, fileTargetPath, filesMoved);
                    if (data.Move)
                    {
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
                    filesMoved[file] = fileTargetPath;
                    if (data.Move)
                    {
                        System.IO.File.Move(file, fileTargetPath);
                    }
                    else
                    {
                        System.IO.File.Copy(file, fileTargetPath, true);
                    }
                    currentBytes += GetBytes(fileTargetPath);
                    ReportProgress(currentBytes, allBytes, ref lastPercent, fileTargetPath);
                }
                m_Token.ThrowIfCancellationRequested();
            }

            if (data.Move)
            {
                AdaptElementPaths(filesMoved, data.MusicDirectory, data.SoundsDirectory, data.Project);
            }
            AdaptTags(filesMoved, data.MusicDirectory, data.Move);
        }

        private static void AdaptTags(Dictionary<String, String> files, String basePath, bool filesMoved)
        {
            Dictionary<String, String> adaptedFiles = new Dictionary<string, string>();
            List<String> removedFiles = new List<string>();
            foreach (var entry in files)
            {
                if (entry.Key.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                {
                    if (entry.Value.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                    {
                        adaptedFiles[entry.Key.Substring(basePath.Length + 1)] = entry.Value.Substring(basePath.Length + 1);
                    }
                    else if (filesMoved)
                    {
                        removedFiles.Add(entry.Key.Substring(basePath.Length + 1));
                    }
                }
            }
            try
            {
                var dbWrite = Ares.Tags.TagsModule.GetTagsDB().WriteInterface;
                if (filesMoved)
                {
                    dbWrite.MoveFiles(adaptedFiles);
                    if (removedFiles.Count > 0)
                        dbWrite.RemoveFiles(removedFiles);
                }
                else
                {
                    dbWrite.CopyFiles(adaptedFiles);
                }
            }
            catch (Ares.Tags.TagsDbException /*ex*/)
            {
                throw;
            }
        }

        private static void AdaptElementPaths(Dictionary<String, String> filesMoved, String musicDir, String soundsDir, IProject project)
        {
            if (project == null)
                return;

            FileLists lists = new FileLists();
            IList<IFileElement> elements = lists.GetAllFiles(project);
            foreach (IFileElement element in elements)
            {
                String basePath = element.SoundFileType == SoundFileType.Music ? musicDir : soundsDir;
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
            if (currentPercent > lastPercent)
            {
                lastPercent = currentPercent;
                m_Monitor.SetProgress(currentPercent, System.IO.Path.GetFileName(file));
                m_Token.ThrowIfCancellationRequested();
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
                    m_Token.ThrowIfCancellationRequested();
                }
                foreach (System.IO.FileInfo file in info.GetFiles())
                {
                    System.IO.File.Copy(file.FullName, System.IO.Path.Combine(destination, file.Name), true);
                    currentBytes += GetBytes(file.FullName);
                    ReportProgress(currentBytes, allBytes, ref lastPercent, file.FullName);
                    m_Token.ThrowIfCancellationRequested();
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
}
