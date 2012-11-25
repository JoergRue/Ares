/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
using Ares.Data;
using System.Threading;

namespace Ares.Playing
{

    interface IMusicPlayer
    {
        void Next();
        void Previous();
        void Stop();
        void SetMusicVolume(int volume);
        void PlayMusicTitle(Int32 elementId);
        bool RepeatCurrentMusic { set; }
    }

    abstract class MusicPlayer : ElementPlayerBase, IMusicPlayer
    {
        public override void VisitFileElement(IFileElement fileElement)
        {
            CurrentPlayedHandle = Client.PlayFile(fileElement, m_MusicFadeInTime, (success) =>
            {
                bool stop = false;
                lock (syncObject)
                {
                    CurrentPlayedHandle = 0;
                    CurrentFadeOut = 0;
                    stop = shallStop;
                }
                if (stop || (!success && IsSingleFileList()))
                {
                    if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                    {
                        PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistFinished();
                    }
                    Client.SubPlayerFinished(this, stop);
                }
                else if (!success)
                {
                    // must get out of current call stack
                    m_PlayAfterErrorTimer = new System.Timers.Timer(5);
                    m_PlayAfterErrorTimer.AutoReset = false;
                    m_PlayAfterErrorTimer.Elapsed += new System.Timers.ElapsedEventHandler(playAfterErrorTimer_Elapsed);
                    m_PlayAfterErrorTimer.Start();
                }
                else
                {
                    PlayNext();
                }
            }, false);

            if (CurrentPlayedHandle != 0 && m_RepeatCurrentMusic)
            {
                ((FilePlayer)PlayingModule.FilePlayer).SetRepeatFile(CurrentPlayedHandle, true);
            }
            CurrentFadeOut = fileElement.Effects.FadeOutTime;
            PlayingModule.ThePlayer.ActiveMusicPlayer = this;
        }

        private System.Timers.Timer m_PlayAfterErrorTimer;

        private void playAfterErrorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_PlayAfterErrorTimer.Dispose();
            m_PlayAfterErrorTimer = null;
            bool stop = false;
            lock (syncObject)
            {
                stop = shallStop;
            }
            if (!stop)
            {
                PlayNext();
            }
        }

        public abstract void PlayNext();

        protected abstract bool IsSingleFileList();

        public void Stop()
        {
            Stop(0);
        }

        public void Stop(int crossFadeMusicTime)
        {
            lock (syncObject)
            {
                shallStop = true;
            }
            StopCurrentFile(crossFadeMusicTime > 0, crossFadeMusicTime / 2);
            Action action = StopDelayWait();
            if (action != null)
                action();
        }

        public override void StopMusic(int crossFadeMusicTime)
        {
            Stop(crossFadeMusicTime);
        }

        public override void StopSounds()
        {
        }

        public override void SetSoundVolume(int volume)
        {
        }

        public void SetMusicVolume(int volume)
        {
            lock (syncObject)
            {
                if (CurrentPlayedHandle != 0)
                {
                    ((FilePlayer)PlayingModule.FilePlayer).SetVolume(CurrentPlayedHandle, volume);
                }
            }
        }

        public abstract void PlayMusicTitle(Int32 elementId);

        public void Next()
        {
            StopCurrentFile(true, 0); // will automatically start the next file
        }

        public abstract void Previous();

        public bool RepeatCurrentMusic
        {
            set
            {
                m_RepeatCurrentMusic = value;
                if (CurrentPlayedHandle != 0)
                {
                    ((FilePlayer)PlayingModule.FilePlayer).SetRepeatFile(CurrentPlayedHandle, value);
                }
            }
        }

        private bool m_RepeatCurrentMusic;

        protected MusicPlayer(WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_RepeatCurrentMusic = PlayingModule.ThePlayer.RepeatCurrentMusic;
        }

        protected bool shallStop = false;
    }

    class SequentialMusicPlayer : MusicPlayer, IMusicPlayer
    {
        public override void PlayMusicTitle(Int32 elementID)
        {
            IList<ISequentialElement> elements = m_Container.GetElements();
            for (int i = 0; i < elements.Count; ++i)
            {
                if (elements[i].InnerElement.Id == elementID)
                {
                    lock (syncObject)
                    {
                        m_Index = i - 1;
                    }
                    StopCurrentFile(true, 0); // will automatically start the next file
                    break;
                }
            }
        }

        public override void Previous()
        {
            lock (syncObject)
            {
                m_Index -= 2;
                if (m_Index < -1)
                    m_Index = -1;
            }
            StopCurrentFile(true, 0); // will automatically start the next file
        }

        public override void PlayNext()
        {
            Monitor.Enter(syncObject);
            if (Client.Stopped || shallStop)
            {
                Monitor.Exit(syncObject);
                if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                {
                    PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistFinished();
                }
                Client.SubPlayerFinished(this, shallStop);
                return;
            }
            ++m_Index;
            if (m_Index >= m_Container.GetElements().Count)
            {
                ++m_RepeatCount;
                if (m_Container.RepeatCount == -1 || m_RepeatCount < m_Container.RepeatCount)
                {
                    Monitor.Exit(syncObject);
                    Repeat(m_Container, m_Container);
                }
                else
                {
                    Monitor.Exit(syncObject);
                    if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                    {
                        PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistFinished();
                    }
                    Client.SubPlayerFinished(this, false);
                }
            }
            else
            {
                ISequentialElement element = m_Container.GetElements()[m_Index];
                Monitor.Exit(syncObject);
                Delay(element, element);
            }
        }

        protected override bool IsSingleFileList()
        {
            return ((IMusicList)m_Container).GetFileElements().Count < 2;
        }

        public override void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
            // called when starting / repeating
            lock (syncObject)
            {
                m_Index = -1;
            }
            PlayNext();
        }

        public SequentialMusicPlayer(ISequentialBackgroundMusicList list, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_Container = Ares.ModelInfo.Playlists.ExpandSequentialMusicList(list, (String error) =>
            {
                ErrorHandling.ErrorOccurred(list.Id, error);
            });
            m_RepeatCount = 0;
        }

        public void Start(int musicFadeInTime)
        {
            if (m_Container.GetElements().Count > 0)
            {
                PlayingModule.ThePlayer.ActiveMusicPlayer = this; // once early to stop previous music player
                if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                {
                    PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistStarted(m_Container.Id);
                }
                ThreadPool.QueueUserWorkItem(state => Process(m_Container, musicFadeInTime));
            }
            else
            {
                Client.SubPlayerFinished(this, false);
            }
        }

        private ISequentialBackgroundMusicList m_Container;
        private int m_Index;
        private int m_RepeatCount;
    }

    class RandomMusicPlayer : MusicPlayer, IMusicPlayer
    {
        public override void PlayMusicTitle(Int32 elementId)
        {
            IList<IChoiceElement> elements = m_Container.GetElements();
            for (int i = 0; i < elements.Count; ++i)
            {
                if (elements[i].InnerElement.Id == elementId)
                {
                    lock (syncObject)
                    {
                        m_FixedNext = i;
                        m_GoBack = false;
                    }
                    Next();
                    break;
                }
            }
        }

        public override void Previous()
        {
            m_GoBack = true;
            Next();
        }

        public override void PlayNext()
        {
            Monitor.Enter(syncObject);
            bool stop = shallStop;
            if (Client.Stopped || shallStop || (m_Container.RepeatCount != -1 && m_Container.RepeatCount <= ++m_RepeatCount))
            {
                Monitor.Exit(syncObject);
                if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                {
                    PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistFinished();
                }
                Client.SubPlayerFinished(this, stop);
            }
            else if (m_FixedNext != -1)
            {
                IChoiceElement element = m_Container.GetElements()[m_FixedNext];
                m_LastElementsStack.Add(element);
                m_GoBack = false;
                m_FixedNext = -1;
                Monitor.Exit(syncObject);
                Repeat(m_Container, element);
            }
            else if (m_GoBack && m_LastElementsStack.Count > 1)
            {
                IChoiceElement element = m_LastElementsStack[m_LastElementsStack.Count - 2];
                m_LastElementsStack.RemoveAt(m_LastElementsStack.Count - 1);
                m_GoBack = false;
                m_FixedNext = -1;
                Monitor.Exit(syncObject);
                Repeat(m_Container, element);
            }
            else
            {
                IChoiceElement element = SelectRandomElement(m_Container);
                m_LastElementsStack.Add(element);
                m_GoBack = false;
                m_FixedNext = -1;
                Monitor.Exit(syncObject);
                Repeat(m_Container, element);
            }
        }

        protected override bool IsSingleFileList()
        {
            return ((IMusicList)m_Container).GetFileElements().Count < 2;
        }

        public override void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            // called on first start
            PlayNext();
        }

        public void Start(int musicFadeInTime)
        {
            if (m_Container.GetElements().Count == 0)
            {
                Client.SubPlayerFinished(this, false);
                return;
            }
            PlayingModule.ThePlayer.ActiveMusicPlayer = this; // once early to stop previous music player
            if (PlayingModule.ThePlayer.ProjectCallbacks != null)
            {
                PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistStarted(m_Container.Id);
            }
            Monitor.Enter(syncObject);
            // Don't delay, start immediately (start delay was already processed by calling player)
            IChoiceElement element = SelectRandomElement(m_Container);
            m_LastElementsStack.Add(element);
            m_GoBack = false;
            Monitor.Exit(syncObject);
            Process(element, musicFadeInTime);
        }

        public RandomMusicPlayer(IRandomBackgroundMusicList list, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_Container = Ares.ModelInfo.Playlists.ExpandRandomMusicList(list, (String error) =>
            {
                ErrorHandling.ErrorOccurred(list.Id, error);
            });
            m_LastElementsStack = new List<IChoiceElement>();
            m_GoBack = false;
            m_FixedNext = -1;
            m_RepeatCount = 0;
        }

        private IRandomBackgroundMusicList m_Container;
        private List<IChoiceElement> m_LastElementsStack;
        private bool m_GoBack;
        private int m_FixedNext;
        private int m_RepeatCount;
    }

    class SingleMusicPlayer : MusicPlayer, IMusicPlayer
    {
        public override void Previous()
        {
        }

        public override void PlayNext()
        {
            Client.SubPlayerFinished(this, false);
        }

        protected override bool IsSingleFileList()
        {
            return true;
        }

        public void Start(int musicFadeInTime)
        {
            Process(m_Element, musicFadeInTime);
        }

        public SingleMusicPlayer(IFileElement musicFile, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_Element = musicFile;
        }

        public override void PlayMusicTitle(int elementId)
        {
        }

        private IFileElement m_Element;
    }
}