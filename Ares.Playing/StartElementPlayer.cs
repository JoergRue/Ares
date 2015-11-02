/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
using System.Threading;

using Ares.Data;
using Ares.ModelInfo;

namespace Ares.Playing
{
    abstract class StartElementPlayer : IElementPlayerClient
    {
        public int PlayFile(IFileElement fileElement, int fadeInTime, Action<bool> afterPlayed, bool loop)
        {
            SoundFile soundFile = new SoundFile(fileElement, m_PlayMusicOnAllSpeakers);
            FileStarted(fileElement);
            int handle = PlayingModule.FilePlayer.PlayFile(soundFile, fadeInTime, (id, handle2) =>
            {
                FileFinished(id, fileElement.SoundFileType);
                lock (syncObject)
                {
                    m_CurrentFiles.Remove(handle2);
                }
                afterPlayed(true);
            }, loop);
            if (handle != 0)
            {
                lock (syncObject)
                {
                    m_CurrentFiles.Add(handle);
                }
            }
            else
            {
                FileFinished(soundFile.Id, fileElement.SoundFileType);
                afterPlayed(false);
            }
            return handle;
        }

        public int PlayWebRadio(IWebRadioElement webRadioElement, int fadeInTime, Action<bool> afterPlayed)
        {
            SoundFile soundFile = new SoundFile(webRadioElement, m_PlayMusicOnAllSpeakers);
            FileStarted(webRadioElement);
            int handle = PlayingModule.FilePlayer.PlayFile(soundFile, fadeInTime, (id, handle2) =>
            {
                FileFinished(id, Data.SoundFileType.Music);
                lock (syncObject)
                {
                    m_CurrentFiles.Remove(handle2);
                }
                afterPlayed(true);
            }, false);
            if (handle != 0)
            {
                lock (syncObject)
                {
                    m_CurrentFiles.Add(handle);
                }
            }
            else
            {
                FileFinished(soundFile.Id, Data.SoundFileType.Music);
                afterPlayed(false);
            }
            return handle;
        }

        public bool Stopped
        {
            get
            {
                lock (syncObject)
                {
                    return m_Stopped;
                }
            }
        }

        public ManualResetEvent StopMusic(int crossFadeMusicTime)
        {
            List<ElementPlayerBase> players = new List<ElementPlayerBase>();
            lock (syncObject)
            {
                players.AddRange(m_SubPlayers.Keys);
                m_MusicStopped = true;
            }
            bool mustStop = false;
            foreach (ElementPlayerBase player in players)
            {
                if (player.StopMusic(crossFadeMusicTime))
                {
                    mustStop = true;
                }
            }
            return mustStop ? m_StoppedEvent : null;
        }

        public ManualResetEvent StopSounds(int fadeTime)
        {
            List<ElementPlayerBase> players = new List<ElementPlayerBase>();
            lock (syncObject)
            {
                players.AddRange(m_SubPlayers.Keys);
                m_SoundsStopped = true;
            }
            bool mustStop = false;
            foreach (ElementPlayerBase player in players)
            {
                if (player.StopSounds(fadeTime))
                {
                    mustStop = true;
                }
            }
            return mustStop ? m_StoppedEvent : null;
        }

        public void SetSoundVolume(int volume)
        {
            List<ElementPlayerBase> players = new List<ElementPlayerBase>();
            lock (syncObject)
            {
                players.AddRange(m_SubPlayers.Keys);
            }
            foreach (ElementPlayerBase player in players)
            {
                player.SetSoundVolume(volume);
            }
        }

        public void SubPlayerStarted(ElementPlayerBase subPlayer)
        {
            lock (syncObject)
            {
                m_SubPlayers[subPlayer] = true;
            }
        }

        public void SubPlayerFinished(ElementPlayerBase subPlayer, bool stopMusic, bool stopSounds)
        {
            Monitor.Enter(syncObject);
            m_SubPlayers.Remove(subPlayer);
            stopMusic = stopMusic && !m_MusicStopped;
            stopSounds = stopSounds && !m_SoundsStopped;
            if (m_SubPlayers.Count == 0 && Playing)
            {
                Playing = false;
                Monitor.Exit(syncObject);
                PlayerFinished();
                m_FinishedAction(this);
            }
            else
            {
                Monitor.Exit(syncObject);
            }
            if (stopSounds)
                StopSounds(0);
            if (stopMusic)
                StopMusic(0);
        }

        public void Start(int musicFadeInTime)
        {
            PlayerStarted();
            if (m_Element != null)
            {
                Playing = true;
                ElementPlayer subPlayer = new ElementPlayer(this, m_StoppedEvent, m_Element);
                lock (syncObject)
                {
                    m_SubPlayers[subPlayer] = true;
                }
                subPlayer.Start(musicFadeInTime);
            }
        }

        public void Stop()
        {
            List<Int32> copy = new List<int>();
            lock (syncObject)
            {
                m_Stopped = true;
                copy.AddRange(m_CurrentFiles);
            }
            copy.ForEach(handle => PlayingModule.FilePlayer.StopFile(handle));
            m_StoppedEvent.Set();
            Monitor.Enter(syncObject);
            m_CurrentFiles.Clear();
            m_SubPlayers.Clear();
            if (Playing)
            {
                Playing = false;
                Monitor.Exit(syncObject);
                PlayerFinished();
                m_FinishedAction(this);
            }
            else
            {
                Monitor.Exit(syncObject);
            }
        }

        public void SetMusicOnAllSpeakers(bool onAllSpeakers)
        {
            m_PlayMusicOnAllSpeakers = onAllSpeakers;
        }

        protected abstract void FileStarted(IElement fileElement);
        protected abstract void FileFinished(Int32 id, Data.SoundFileType soundFileType);

        protected abstract void PlayerStarted();
        protected abstract void PlayerFinished();

        protected abstract bool Playing { get; set; }

        protected StartElementPlayer(IElement element, Action<StartElementPlayer> finishedAction, bool playMusicOnAllSpeakers)
        {
            m_Element = element;
            m_CurrentFiles = new List<int>();
            m_SubPlayers = new Dictionary<ElementPlayerBase, bool>();
            m_Stopped = false;
            m_StoppedEvent = new ManualResetEvent(false);
            m_FinishedAction = finishedAction;
            m_PlayMusicOnAllSpeakers = playMusicOnAllSpeakers;
        }

        private Dictionary<ElementPlayerBase, bool> m_SubPlayers;
        private List<Int32> m_CurrentFiles;
        private bool m_Stopped;
        private bool m_MusicStopped;
        private bool m_SoundsStopped;
        private ManualResetEvent m_StoppedEvent;
        private Action<StartElementPlayer> m_FinishedAction;

        private IElement m_Element;
        private bool m_PlayMusicOnAllSpeakers = false;


        protected Object syncObject = new Int16();
    }

    class SingleElementPlayer : StartElementPlayer
    {
        protected override void FileStarted(IElement fileElement)
        {
        }

        protected override void FileFinished(int id, Data.SoundFileType soundFileType)
        {
        }

        protected override void PlayerStarted()
        {
        }

        protected override void PlayerFinished()
        {
            if (m_Callbacks != null)
            {
                m_Callbacks.ElementFinished(m_ElementId);
            }
        }

        protected override bool Playing
        {
            get;
            set;
        }

        public SingleElementPlayer(IElement element, IElementPlayingCallbacks callbacks, Action<StartElementPlayer> finishedAction, bool playMusicOnAllSpeakers)
            : base(element, finishedAction, playMusicOnAllSpeakers)
        {
            m_Callbacks = callbacks;
            m_ElementId = element.Id;
        }

        private IElementPlayingCallbacks m_Callbacks;
        private int m_ElementId;
    }

    class ModeElementPlayer : StartElementPlayer
    {
        protected override void FileStarted(IElement fileElement)
        {
            if (m_Callbacks != null)
            {
                if (fileElement is IWebRadioElement || ((IFileElement)fileElement).SoundFileType == Data.SoundFileType.Music)
                {
                    m_Callbacks.MusicStarted(fileElement.Id);
                }
                else
                {
                    m_Callbacks.SoundStarted(fileElement.Id);
                }
            }
        }

        protected override void FileFinished(int id, Data.SoundFileType soundFileType)
        {
            if (m_Callbacks != null)
            {
                if (soundFileType == Data.SoundFileType.Music)
                {
                    m_Callbacks.MusicFinished(id);
                }
                else
                {
                    m_Callbacks.SoundFinished(id);
                }
            }
        }

        protected override void PlayerStarted()
        {
            if (m_Callbacks != null)
            {
                m_Callbacks.ModeElementStarted(m_Mode);
            }
            Playing = true;
        }

        protected override void PlayerFinished()
        {
            if (m_Callbacks != null)
            {
                m_Callbacks.ModeElementFinished(m_Mode);
            }
        }

        protected override bool Playing
        {
            get
            {
                return m_IsPlaying;
            }
            set
            {
                m_IsPlaying = value;
                if (m_Mode.IsEndless())
                {
                    m_Mode.IsPlaying = value;
                }
            }
        }

        public ModeElementPlayer(IModeElement modeElement, IProjectPlayingCallbacks callbacks, Action<StartElementPlayer> finishedAction, bool playMusicOnAllSpeakers)
            : base(modeElement.StartElement, finishedAction, playMusicOnAllSpeakers)
        {
            m_Mode = modeElement;
            m_Callbacks = callbacks;
        }

        private IModeElement m_Mode;
        private bool m_IsPlaying;
        private IProjectPlayingCallbacks m_Callbacks;
    }

}