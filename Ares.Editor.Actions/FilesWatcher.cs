using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Editor.Actions
{
    public class FilesWatcher
    {
        public static FilesWatcher Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new FilesWatcher();
                }
                return sInstance;
            }
        }

        private static FilesWatcher sInstance;

        public event EventHandler<EventArgs> MusicDirChanges;

        public event EventHandler<EventArgs> SoundDirChanges;

        public event EventHandler<EventArgs> AnyDirChanges;

        private FilesWatcher()
        {
        }

        private void Init()
        {
            m_MusicWatcher = new System.IO.FileSystemWatcher();
            m_MusicWatcher.Changed += new System.IO.FileSystemEventHandler(m_MusicWatcher_Changed);
            m_MusicWatcher.Created += new System.IO.FileSystemEventHandler(m_MusicWatcher_Changed);
            m_MusicWatcher.Deleted += new System.IO.FileSystemEventHandler(m_MusicWatcher_Changed);
            m_MusicWatcher.Renamed += new System.IO.RenamedEventHandler(m_MusicWatcher_Changed);
            m_MusicWatcher.IncludeSubdirectories = true;
            m_MusicWatcher.NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.LastWrite;
            m_SoundWatcher = new System.IO.FileSystemWatcher();
            m_SoundWatcher.Changed += new System.IO.FileSystemEventHandler(m_SoundWatcher_Changed);
            m_SoundWatcher.Created += new System.IO.FileSystemEventHandler(m_SoundWatcher_Changed);
            m_SoundWatcher.Deleted += new System.IO.FileSystemEventHandler(m_SoundWatcher_Changed);
            m_SoundWatcher.Renamed += new System.IO.RenamedEventHandler(m_SoundWatcher_Changed);
            m_SoundWatcher.IncludeSubdirectories = true;
            m_SoundWatcher.NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.LastWrite;
        }

        private void m_MusicWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            m_MusicWatcher.EnableRaisingEvents = false;
            Ares.ModelInfo.ModelChecks.Instance.Check(Ares.ModelInfo.CheckType.File);
            if (MusicDirChanges != null) MusicDirChanges(this, new EventArgs());
            if (AnyDirChanges != null) AnyDirChanges(this, new EventArgs());
            m_MusicWatcher.EnableRaisingEvents = true;
        }

        private void m_SoundWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            m_SoundWatcher.EnableRaisingEvents = false;
            Ares.ModelInfo.ModelChecks.Instance.Check(Ares.ModelInfo.CheckType.File);
            if (SoundDirChanges != null) SoundDirChanges(this, new EventArgs());
            if (AnyDirChanges != null) AnyDirChanges(this, new EventArgs());
            m_SoundWatcher.EnableRaisingEvents = true;
        }

        public void SetDirectories(String musicDir, String soundsDir)
        {
            bool firstSet = false;
            if (m_SoundWatcher == null)
            {
                firstSet = true;
                Init();
            }
            String oldMusicPath = m_MusicWatcher.Path;
            String oldSoundPath = m_SoundWatcher.Path;
            m_MusicWatcher.Path = musicDir;
            m_SoundWatcher.Path = soundsDir;
            if (!firstSet)
            {
                Ares.ModelInfo.ModelChecks.Instance.Check(Ares.ModelInfo.CheckType.File);
                if (oldMusicPath != musicDir && MusicDirChanges != null)
                {
                    MusicDirChanges(this, new EventArgs());
                }
                if (oldSoundPath != soundsDir && SoundDirChanges != null)
                {
                    SoundDirChanges(this, new EventArgs());
                }
                if ((oldMusicPath != musicDir || oldSoundPath != soundsDir) && AnyDirChanges != null)
                {
                    AnyDirChanges(this, new EventArgs());
                }
            }
            m_MusicWatcher.EnableRaisingEvents = true;
            m_SoundWatcher.EnableRaisingEvents = true;
        }

        public void DeInit()
        {
            if (m_SoundWatcher != null)
            {
                m_SoundWatcher.Dispose();
                m_MusicWatcher.Dispose();
                m_SoundWatcher = null;
                m_MusicWatcher = null;
            }
        }

        private System.IO.FileSystemWatcher m_MusicWatcher;
        private System.IO.FileSystemWatcher m_SoundWatcher;
    }
}
