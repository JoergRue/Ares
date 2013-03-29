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
using System.Text;

namespace Ares.Playing
{

    class ProjectCallbacks : IProjectPlayingCallbacks
    {
        private List<IProjectPlayingCallbacks> m_Clients = new List<IProjectPlayingCallbacks>();

        private Object syncObject = new Object();

        private static ProjectCallbacks s_Instance;

        public static ProjectCallbacks Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new ProjectCallbacks();
                    PlayingModule.ThePlayer.ProjectCallbacks = s_Instance;
                }
                return s_Instance;
            }
        }

        public void AddCallbacks(IProjectPlayingCallbacks callbacks)
        {
            lock (syncObject)
            {
                m_Clients.Add(callbacks);
            }
        }

        public void RemoveCallbacks(IProjectPlayingCallbacks callbacks)
        {
            lock (syncObject)
            {
                m_Clients.Remove(callbacks);
            }
        }

        public void ModeChanged(Data.IMode newMode)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.ModeChanged(newMode);
            }
        }

        public void ModeElementStarted(Data.IModeElement element)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.ModeElementStarted(element);
            }
        }

        public void ModeElementFinished(Data.IModeElement element)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.ModeElementFinished(element);
            }
        }

        public void SoundStarted(int elementId)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.SoundStarted(elementId);
            }
        }

        public void SoundFinished(int elementId)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.SoundFinished(elementId);
            }
        }

        public void MusicStarted(int elementId)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicStarted(elementId);
            }
        }

        public void MusicFinished(int elementId)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicFinished(elementId);
            }
        }

        public void VolumeChanged(VolumeTarget target, int newValue)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.VolumeChanged(target, newValue);
            }
        }

        public void MusicPlaylistStarted(int elementId)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicPlaylistStarted(elementId);
            }
        }

        public void MusicPlaylistFinished()
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicPlaylistFinished();
            }
        }

        public void MusicRepeatChanged(bool repeat)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicRepeatChanged(repeat);
            }
        }

        public void ErrorOccurred(int elementId, string errorMessage)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.ErrorOccurred(elementId, errorMessage);
            }
        }

        public void MusicTagAdded(int tagId)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicTagAdded(tagId);
            }
        }

        public void MusicTagRemoved(int tagId)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicTagRemoved(tagId);
            }
        }

        public void AllMusicTagsRemoved()
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.AllMusicTagsRemoved();
            }
        }

        public void MusicTagCategoriesOperatorChanged(bool isAndOperator)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicTagCategoriesOperatorChanged(isAndOperator);
            }
        }

        public void MusicTagsChanged(ICollection<int> newTags, bool isAndOperator, int fadeTime)
        {
            List<IProjectPlayingCallbacks> copy = null;
            lock (syncObject)
            {
                copy = new List<IProjectPlayingCallbacks>(m_Clients);
            }
            foreach (IProjectPlayingCallbacks callback in copy)
            {
                callback.MusicTagsChanged(newTags, isAndOperator, fadeTime);
            }
        }
    }

    public static class PlayingModule
    {
        public static IFilePlayer CreatePlayer()
        {
            return new FilePlayer();
        }

        public static IProjectPlayer ProjectPlayer
        {
            get { return sPlayer; }
        }

        public static IElementPlayer ElementPlayer
        {
            get { return sPlayer; }
        }

        public static IStreamer Streamer
        {
            get { return BassStreamer.Instance; }
        }

        public static void AddCallbacks(IProjectPlayingCallbacks callbacks)
        {
            ProjectCallbacks.Instance.AddCallbacks(callbacks);
        }

        public static void RemoveCallbacks(IProjectPlayingCallbacks callbacks)
        {
            ProjectCallbacks.Instance.RemoveCallbacks(callbacks);
        }

        public static void SetCallbacks(IElementPlayingCallbacks callbacks)
        {
            sPlayer.ElementCallbacks = callbacks;
        }

        internal static Player ThePlayer
        {
            get { return sPlayer; }
        }

        internal static IFilePlayer FilePlayer
        {
            get { return sFilePlayer; }
        }

        internal static Random Randomizer
        {
            get { return sRandom; }
        }

        private static Player sPlayer = new Player();
        private static FilePlayer sFilePlayer = new FilePlayer();
        private static Random sRandom = new Random();
        
   }
}
