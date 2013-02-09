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

        public bool Enabled 
        {
            get { return m_Enabled; }
            set 
            {
                m_Enabled = value;
                if (m_Enabled)
                {
                    if (HadMusicChangesWhileDisabled || HadSoundChangesWhileDisabled)
                    {
                        m_MusicWatcher.EnableRaisingEvents = false;
                        m_SoundWatcher.EnableRaisingEvents = false;
                        Ares.ModelInfo.ModelChecks.Instance.Check(Ares.ModelInfo.CheckType.File, Project);
                        if (MusicDirChanges != null && HadMusicChangesWhileDisabled) MusicDirChanges(this, new EventArgs());
                        if (SoundDirChanges != null && HadSoundChangesWhileDisabled) SoundDirChanges(this, new EventArgs());
                        if (AnyDirChanges != null) AnyDirChanges(this, new EventArgs());
                        HadMusicChangesWhileDisabled = false;
                        HadSoundChangesWhileDisabled = false;
                        m_MusicWatcher.EnableRaisingEvents = true;
                        m_SoundWatcher.EnableRaisingEvents = true;
                    }
                }
            }
        }

        private bool m_Enabled;

        private bool HadMusicChangesWhileDisabled { get; set; }
        private bool HadSoundChangesWhileDisabled { get; set; }

        public Ares.Data.IProject Project { get; set; }

        private FilesWatcher()
        {
            Enabled = true;
            HadMusicChangesWhileDisabled = false;
            HadSoundChangesWhileDisabled = false;
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
            if (!Enabled)
            {
                HadMusicChangesWhileDisabled = true;
                return;
            }
            if (e.FullPath.EndsWith("sqlite-journal", StringComparison.OrdinalIgnoreCase))
                return;
            if (e.FullPath.EndsWith("sqlite", StringComparison.OrdinalIgnoreCase))
                return;
            m_MusicWatcher.EnableRaisingEvents = false;
            Ares.ModelInfo.ModelChecks.Instance.Check(Ares.ModelInfo.CheckType.File, Project);
            if (MusicDirChanges != null) MusicDirChanges(this, new EventArgs());
            if (AnyDirChanges != null) AnyDirChanges(this, new EventArgs());
            m_MusicWatcher.EnableRaisingEvents = true;
        }

        private void m_SoundWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (!Enabled)
            {
                HadSoundChangesWhileDisabled = true;
                return;
            }
            if (e.FullPath.EndsWith("sqlite-journal", StringComparison.OrdinalIgnoreCase))
                return;
            if (e.FullPath.EndsWith("sqlite", StringComparison.OrdinalIgnoreCase))
                return;
            m_SoundWatcher.EnableRaisingEvents = false;
            Ares.ModelInfo.ModelChecks.Instance.Check(Ares.ModelInfo.CheckType.File, Project);
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
            if (System.IO.Directory.Exists(musicDir))
            {
                m_MusicWatcher.Path = musicDir;
            }
            if (System.IO.Directory.Exists(soundsDir))
            {
                m_SoundWatcher.Path = soundsDir;
            }
            if (!firstSet)
            {
                Ares.ModelInfo.ModelChecks.Instance.Check(Ares.ModelInfo.CheckType.File, Project);
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
            if (System.IO.Directory.Exists(musicDir))
            {
                m_MusicWatcher.EnableRaisingEvents = true;
            }
            if (System.IO.Directory.Exists(soundsDir))
            {
                m_SoundWatcher.EnableRaisingEvents = true;
            }
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
