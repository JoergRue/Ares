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
using System.Threading;
using Ares.Data;
using Ares.ModelInfo;

namespace Ares.Playing
{
    class SoundFile : ISoundFile
    {
        public int Id
        {
            get;
            private set; 
        }

        public string Path
        {
            get;
            private set;
        }

        public SoundFileType SoundFileType
        {
            get;
            private set;
        }

        public int Volume
        {
            get;
            private set;
        }

        public IEffects Effects
        {
            get;
            private set;
        }

        public SoundFile(IFileElement element)
        {
            Id = element.Id;
            SoundFileType = element.SoundFileType == Data.SoundFileType.Music ? SoundFileType.Music : SoundFileType.SoundEffect;
            String dir = (SoundFileType == Playing.SoundFileType.Music) ? PlayingModule.ThePlayer.MusicPath : PlayingModule.ThePlayer.SoundPath;
            Path = System.IO.Path.Combine(dir, element.FilePath);
            Volume = SoundFileType == Playing.SoundFileType.Music ? PlayingModule.ThePlayer.MusicVolume : PlayingModule.ThePlayer.SoundVolume;
            Effects = element.Effects;
        }
    }

    class ErrorHandling
    {
        public static void BassErrorOccurred(int elementID, String msgFormat)
        {
            Un4seen.Bass.BASSError error = Un4seen.Bass.Bass.BASS_ErrorGetCode();
            String msg = GetMsgForError(error);
            String message = String.Format(msgFormat, msg);
            if (PlayingModule.ThePlayer.ElementCallbacks != null)
            {
                PlayingModule.ThePlayer.ElementCallbacks.ErrorOccurred(elementID, message);
            }
            else if (PlayingModule.ThePlayer.ProjectCallbacks != null)
            {
                PlayingModule.ThePlayer.ProjectCallbacks.ErrorOccurred(elementID, message);
            }
        }

        private static String GetMsgForError(Un4seen.Bass.BASSError error)
        {
            switch (error)
            {
                case Un4seen.Bass.BASSError.BASS_ERROR_MEM:
                    return StringResources.MemError;
                case Un4seen.Bass.BASSError.BASS_ERROR_FILEOPEN:
                    return StringResources.FileOpenError;
                case Un4seen.Bass.BASSError.BASS_ERROR_DRIVER:
                    return StringResources.DriverError;
                case Un4seen.Bass.BASSError.BASS_ERROR_BUFLOST:
                    return StringResources.BufLostError;
                case Un4seen.Bass.BASSError.BASS_ERROR_HANDLE:
                    return StringResources.HandleError;
                case Un4seen.Bass.BASSError.BASS_ERROR_FORMAT:
                    return StringResources.FormatError;
                case Un4seen.Bass.BASSError.BASS_ERROR_POSITION:
                    return StringResources.PositionError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NOTAUDIO:
                    return StringResources.NotAudioError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NOCHAN:
                    return StringResources.NoChanError;
                case Un4seen.Bass.BASSError.BASS_ERROR_ILLTYPE:
                    return StringResources.IllTypeError;
                case Un4seen.Bass.BASSError.BASS_ERROR_ILLPARAM:
                    return StringResources.IllParamError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NO3D:
                    return StringResources.No3DError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NOEAX:
                    return StringResources.NoEaxError;
                case Un4seen.Bass.BASSError.BASS_ERROR_DEVICE:
                    return StringResources.DeviceError;
                case Un4seen.Bass.BASSError.BASS_ERROR_FREQ:
                    return StringResources.FreqError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NOTFILE:
                    return StringResources.NotFileError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NOHW:
                    return StringResources.NoHwError;
                case Un4seen.Bass.BASSError.BASS_ERROR_CREATE:
                    return StringResources.CreateError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NOFX:
                    return StringResources.NoFxError;
                case Un4seen.Bass.BASSError.BASS_ERROR_NOPLAY:
                    return StringResources.NoPlayError;
                case Un4seen.Bass.BASSError.BASS_ERROR_PLAYING:
                    return StringResources.PlayingError;
                case Un4seen.Bass.BASSError.BASS_ERROR_DX:
                    return StringResources.DxError;
                case Un4seen.Bass.BASSError.BASS_ERROR_FILEFORM:
                    return StringResources.FileformError;
                case Un4seen.Bass.BASSError.BASS_ERROR_SPEAKER:
                    return StringResources.SpeakerError;
                case Un4seen.Bass.BASSError.BASS_ERROR_CODEC:
                    return StringResources.CodecError;
                case Un4seen.Bass.BASSError.BASS_ERROR_ENDED:
                    return StringResources.EndedError;
                default:
                    return String.Format(StringResources.UnexpectedError, error);
            }
        }
    }

    interface IElementPlayerClient
    {
        int PlayFile(IFileElement fileElement, Action<bool> afterPlayed, bool loop);

        bool Stopped { get; }

        void SubPlayerStarted(ElementPlayerBase subPlayer);
        void SubPlayerFinished(ElementPlayerBase subPlayer);
    }

    interface IMusicPlayer
    {
        void Next();
        void Previous();
        void Stop();
        void SetMusicVolume(int volume);
    }

    abstract class ElementPlayerBase : IElementVisitor
    {
        public abstract void VisitFileElement(IFileElement fileElement);

        public virtual void VisitSequentialContainer(ISequentialContainer sequentialContainer) { }

        public virtual void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer) { }

        public virtual void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer) { }

        public virtual void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList) { }

        public virtual void VisitRandomMusicList(IRandomBackgroundMusicList musicList) { }

        public abstract void StopMusic();
        public abstract void StopSounds();
        public abstract void SetSoundVolume(int volume);

        protected int CurrentPlayedHandle { get; set; }

        protected void StopCurrentFile()
        {
            lock (syncObject)
            {
                if (CurrentPlayedHandle != 0)
                {
                    int handle = CurrentPlayedHandle;
                    CurrentPlayedHandle = 0;
                    PlayingModule.FilePlayer.StopFile(handle);
                }
            }
        }

        protected void Delay(IDelayableElement element, IElement original)
        {
            ProcessAfterDelay(original, element.FixedStartDelay, element.MaximumRandomStartDelay);
        }

        protected void Repeat(IRepeatableElement element, IElement original)
        {
            ProcessAfterDelay(original, element.FixedIntermediateDelay, element.MaximumRandomIntermediateDelay);
        }

        private void ProcessAfterDelay(IElement element, TimeSpan fixedDelay, TimeSpan randomDelay)
        {
            TimeSpan delay = fixedDelay;
            if (randomDelay.Ticks > 0)
            {
                int millis = (int)(randomDelay.Ticks / TimeSpan.TicksPerMillisecond);
                delay = TimeSpan.FromTicks(delay.Ticks + PlayingModule.Randomizer.Next(millis) * TimeSpan.TicksPerMillisecond);
            }
            if (delay.Ticks > 0)
            {
                m_RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(StoppedEvent, (Object state, bool timedOut) =>
                {
                    m_RegisteredWaitHandle.Unregister(null);
                    if (timedOut && !Client.Stopped)
                    {
                        Process(element);
                    }
                },
                null, delay, true);
            }
            else
            {
                Process(element);
            }
        }

        protected void Process(IElement element)
        {
            if (element.SetsMusicVolume)
            {
                PlayingModule.ThePlayer.SetVolume(VolumeTarget.Music, element.MusicVolume);
                PlayingModule.ThePlayer.ProjectCallbacks.VolumeChanged(VolumeTarget.Music, element.MusicVolume);
            }
            if (element.SetsSoundVolume)
            {
                PlayingModule.ThePlayer.SetVolume(VolumeTarget.Sounds, element.MusicVolume);
                PlayingModule.ThePlayer.ProjectCallbacks.VolumeChanged(VolumeTarget.Sounds, element.SoundVolume);
            }
            element.Visit(this);
        }

        protected IChoiceElement SelectRandomElement(IElementContainer<IChoiceElement> choiceContainer)
        {
            int sum = 0;
            foreach (IChoiceElement element in choiceContainer.GetElements())
            {
                sum += element.RandomChance;
            }
            if (sum > 0)
            {
                int rolled = PlayingModule.Randomizer.Next(sum);
                sum = 0;
                foreach (IChoiceElement element in choiceContainer.GetElements())
                {
                    sum += element.RandomChance;
                    if (sum >= rolled)
                    {
                        // this element was randomly selected
                        return element;
                    }
                }
            }
            return null; // only reached if container is empty
        }

        protected ElementPlayerBase(WaitHandle stoppedEvent, IElementPlayerClient client)
        {
            m_StoppedEvent = stoppedEvent;
            m_Client = client;
        }

        protected IElementPlayerClient Client { get { return m_Client; } }
        protected WaitHandle StoppedEvent { get { return m_StoppedEvent; } }

        private RegisteredWaitHandle m_RegisteredWaitHandle;
        private WaitHandle m_StoppedEvent;
        private IElementPlayerClient m_Client;

        protected Object syncObject = new Int16();
    }

    abstract class MusicPlayer : ElementPlayerBase, IMusicPlayer
    {
        public override void VisitFileElement(IFileElement fileElement)
        {
            CurrentPlayedHandle = Client.PlayFile(fileElement, (success) =>
                {
                    bool stop = false;
                    lock (syncObject)
                    {
                        CurrentPlayedHandle = 0;
                        stop = shallStop;
                    }
                    if (stop || !success)
                    {
                        Client.SubPlayerFinished(this);
                    }
                    else
                    {
                        PlayNext();
                    }
                }, false);
            PlayingModule.ThePlayer.ActiveMusicPlayer = this;
        }

        public abstract void PlayNext();

        public void Stop()
        {
            lock (syncObject)
            {
                shallStop = true;
            }
            StopCurrentFile();
        }

        public override void StopMusic()
        {
            Stop();
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

        public void Next()
        {
            StopCurrentFile(); // will automatically start the next file
        }

        public abstract void Previous();

        protected MusicPlayer(WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
        }

        protected bool shallStop = false;
    }

    class SequentialMusicPlayer : MusicPlayer, IMusicPlayer
    {
        public override void Previous()
        {
            lock(syncObject)
            {
                m_Index -= 2;
                if (m_Index < -1) 
                    m_Index = -1;
            }
            StopCurrentFile(); // will automatically start the next file
        }

        public override void PlayNext()
        {
            Monitor.Enter(syncObject);
            if (Client.Stopped || shallStop)
            {
                Monitor.Exit(syncObject);
                Client.SubPlayerFinished(this);
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
                    Client.SubPlayerFinished(this);
                }
            }
            else
            {
                ISequentialElement element = m_Container.GetElements()[m_Index];
                Monitor.Exit(syncObject);
                Delay(element, element);
            }
        }

        public override void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
            // called when starting / repeating
            lock(syncObject)
            {
                m_Index = -1;
            }
            PlayNext();
        }

        public SequentialMusicPlayer(ISequentialBackgroundMusicList list, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_Container = list;
            m_RepeatCount = 0;
        }

        public void Start()
        {
            if (m_Container.GetElements().Count > 0)
            {
                ThreadPool.QueueUserWorkItem(state => Process(m_Container));
            }
            else
            {
                Client.SubPlayerFinished(this);
            }
        }

        private ISequentialBackgroundMusicList m_Container;
        private int m_Index;
        private int m_RepeatCount;
    }

    class RandomMusicPlayer : MusicPlayer, IMusicPlayer
    {
        public override void Previous()
        {
            m_GoBack = true;
            Next();
        }

        public override void  PlayNext()
        {
            Monitor.Enter(syncObject);
            if (Client.Stopped || shallStop || (m_Container.RepeatCount != -1 && m_Container.RepeatCount <= ++m_RepeatCount))
            {
                Monitor.Exit(syncObject);
                Client.SubPlayerFinished(this);
            }
            else if (m_GoBack && m_LastElementsStack.Count > 1)
            {
                IChoiceElement element = m_LastElementsStack[m_LastElementsStack.Count - 2];
                m_LastElementsStack.RemoveAt(m_LastElementsStack.Count - 1);
                m_GoBack = false;
                Monitor.Exit(syncObject);
                Repeat(m_Container, element);
            }
            else
            {
                IChoiceElement element = SelectRandomElement(m_Container);
                m_LastElementsStack.Add(element);
                m_GoBack = false;
                Monitor.Exit(syncObject);
                Repeat(m_Container, element);
            }
        }

        public override void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            // called on first start
            PlayNext();
        }

        public void Start()
        {
            Monitor.Enter(syncObject);
            // Don't delay, start immediately (start delay was already processed by calling player)
            IChoiceElement element = SelectRandomElement(m_Container);
            m_LastElementsStack.Add(element);
            m_GoBack = false;
            Monitor.Exit(syncObject);
            Process(element);
        }

        public RandomMusicPlayer(IRandomBackgroundMusicList list, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_Container = list;
            m_LastElementsStack = new List<IChoiceElement>();
            m_GoBack = false;
            m_RepeatCount = 0;
        }

        private IRandomBackgroundMusicList m_Container;
        private List<IChoiceElement> m_LastElementsStack;
        private bool m_GoBack;
        private int m_RepeatCount;
    }

    class SingleMusicPlayer : MusicPlayer, IMusicPlayer
    {
        public override void Previous()
        {
        }

        public override void  PlayNext()
        {
            Client.SubPlayerFinished(this);
        }

        public void Start()
        {
            Process(m_Element);
        }

        public SingleMusicPlayer(IFileElement musicFile, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_Element = musicFile;
        }

        private IFileElement m_Element;
    }

    class ElementPlayer : ElementPlayerBase, IElementPlayerClient
    {
        // Interface IElementPlayerClient -- for sub-players 
        // in parallel containers
        // Mostly just pass through to our own client

        public int PlayFile(IFileElement element, Action<bool> afterPlayed, bool loop)
        {
            return Client.PlayFile(element, afterPlayed, loop);
        }

        public bool Stopped { get { return Client.Stopped; } }

        public void SubPlayerStarted(ElementPlayerBase player)
        {
            Client.SubPlayerStarted(player);
            Interlocked.Increment(ref m_ActiveSubPlayers);
        }

        public void SubPlayerFinished(ElementPlayerBase player)
        {
            Client.SubPlayerFinished(player);
            if (Interlocked.Decrement(ref m_ActiveSubPlayers) == 0)
            {
                // done with the parallel container or music player
                // note: there can't be several counts running in parallel,
                // because for parallel execution, new players are created
                Next();
            }
        }

        // From ElementPlayerBase
        
        // Because this player doesn't play music by itself, but delegates to sub-players,
        // no implementation is necessary
        public override void StopMusic()
        { 
        }

        public override void StopSounds()
        {
            lock (syncObject)
            {
                m_StopSounds = true;
            }
            StopCurrentFile();
        }

        public override void SetSoundVolume(int volume)
        {
            lock (syncObject)
            {
                if (CurrentPlayedHandle != 0)
                {
                    ((FilePlayer)PlayingModule.FilePlayer).SetVolume(CurrentPlayedHandle, volume);
                }
            }            
        }

        // Interface IElementVisitor

        public override void VisitFileElement(IFileElement fileElement)
        {
            bool stopSounds = false;
            lock (syncObject)
            {
                stopSounds = m_StopSounds;
            }
            if (stopSounds)
            {
                Next();
            }
            else if (fileElement.SoundFileType == Data.SoundFileType.Music)
            {
                SingleMusicPlayer subPlayer = new SingleMusicPlayer(fileElement, StoppedEvent, this);
                Client.SubPlayerStarted(subPlayer);
                Interlocked.Increment(ref m_ActiveSubPlayers);
                subPlayer.Start();
            }
            else
            {
                int handle = Client.PlayFile(fileElement, (success) =>
                    {
                        lock (syncObject)
                        {
                            CurrentPlayedHandle = 0;
                            if (!success)
                                m_ElementQueue.Clear();
                        }
                        Next();
                    }, m_LoopElement);
                lock (syncObject)
                {
                    CurrentPlayedHandle = handle;
                }
            }
        }

        public override void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
            List<QueueElement> newElements = new List<QueueElement>();
            foreach (ISequentialElement element in sequentialContainer.GetElements())
            {
                newElements.Add(new QueueElement(element));
            }
            m_ElementQueue.InsertRange(0, newElements);
            Next();
        }

        public override void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IParallelElement element in parallelContainer.GetElements())
            {
                ElementPlayer subPlayer = new ElementPlayer(this, StoppedEvent, element);
                Client.SubPlayerStarted(subPlayer);
                Interlocked.Increment(ref m_ActiveSubPlayers);
                subPlayer.Start();
            }
        }

        public override void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
            if (choiceContainer.GetElements().Count == 0)
                return;
            IChoiceElement element = SelectRandomElement(choiceContainer);
            if (element != null)
            {
                m_ElementQueue.Insert(0, new QueueElement(element));
            }
            Next();
        }

        public override void VisitSequentialMusicList(ISequentialBackgroundMusicList list)
        {
            if (list.GetElements().Count == 0)
                return;
            SequentialMusicPlayer subPlayer = new SequentialMusicPlayer(list, StoppedEvent, this);
            Client.SubPlayerStarted(subPlayer);
            Interlocked.Increment(ref m_ActiveSubPlayers);
            subPlayer.Start();
        }

        public override void VisitRandomMusicList(IRandomBackgroundMusicList list)
        {
            if (list.GetElements().Count == 0)
                return;
            RandomMusicPlayer subPlayer = new RandomMusicPlayer(list, StoppedEvent, this);
            Client.SubPlayerStarted(subPlayer);
            Interlocked.Increment(ref m_ActiveSubPlayers);
            subPlayer.Start();
        }

        // Interface for the mode player

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(state => Next());
        }

        private bool HasOnlyOneLoopableFileElement(IElement element)
        {
            if (element is IFileElement)
                return true;
            if (element is IGeneralElementContainer)
            {
                IList<IContainerElement> elements = (element as IGeneralElementContainer).GetGeneralElements();
                if (elements.Count == 1)
                {
                    IContainerElement inner = elements[0];
                    if (inner is IDelayableElement)
                    {
                        if ((inner as IDelayableElement).FixedStartDelay.Ticks > 0)
                            return false;
                        if ((inner as IDelayableElement).MaximumRandomStartDelay.Ticks > 0)
                            return false;
                    }
                    if (inner is IRepeatableElement)
                    {
                        if ((inner as IRepeatableElement).RepeatCount != 1)
                            return false;
                    }
                    return HasOnlyOneLoopableFileElement(inner.InnerElement);
                }
            }
            else if (element is IContainerElement)
            {
                return HasOnlyOneLoopableFileElement((element as IContainerElement).InnerElement);
            }
            return false;
        }

        private void Next()
        {
            if (Client.Stopped)
            {
                return;
            }
            Monitor.Enter(syncObject);
            if (m_ElementQueue.Count == 0)
            {
                // finished, nothing to play any more
                Monitor.Exit(syncObject);
                Client.SubPlayerFinished(this);
            }
            else
            {
                QueueElement element = m_ElementQueue[0];
                bool firstPlay = element.PlayCount == 0;
                ++element.PlayCount;
                IRepeatableElement repeatable = element.Element as IRepeatableElement;
                IMusicList musicList = element.Element as IMusicList;
                if (m_StopSounds || musicList != null || repeatable == null || (repeatable.RepeatCount != -1 && element.PlayCount >= repeatable.RepeatCount))
                {
                    // will not be played again; music lists are repeated inside their own players
                    m_ElementQueue.RemoveAt(0);
                }
                else if (repeatable != null && repeatable.RepeatCount == -1 && repeatable.MaximumRandomIntermediateDelay.Ticks == 0 && repeatable.FixedIntermediateDelay.Ticks == 0
                    && HasOnlyOneLoopableFileElement(element.Element))
                {
                    m_ElementQueue.RemoveAt(0);
                    m_LoopElement = true;
                }
                Monitor.Exit(syncObject);
                if (firstPlay)
                {
                    IDelayableElement delayable = element.Element as IDelayableElement;
                    if (delayable != null)
                    {
                        Delay(delayable, element.Element);
                    }
                    else
                    {
                        Process(element.Element);
                    }
                }
                else
                {
                    Repeat(repeatable, element.Element);
                }
            }
        }

        public ElementPlayer(IElementPlayerClient client, WaitHandle stoppedEvent, IElement startElement)
            : base(stoppedEvent, client)
        {
            m_ElementQueue = new List<QueueElement>();
            m_ElementQueue.Add(new QueueElement(startElement));
            m_ActiveSubPlayers = 0;
            m_StopSounds = false;
            m_LoopElement = false;
        }

        private class QueueElement
        {
            public IElement Element { get; set; }
            public int PlayCount { get; set; }

            public QueueElement(IElement element)
            {
                Element = element;
                PlayCount = 0;
            }
        }

        private List<QueueElement> m_ElementQueue;

        private int m_ActiveSubPlayers;

        private bool m_StopSounds;

        private bool m_LoopElement;
    }

    abstract class StartElementPlayer : IElementPlayerClient
    {
        public int PlayFile(IFileElement fileElement, Action<bool> afterPlayed, bool loop)
        {
            SoundFile soundFile = new SoundFile(fileElement);
            FileStarted(fileElement);
            int handle = PlayingModule.FilePlayer.PlayFile(soundFile, (id, handle2) =>
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

        public void StopMusic()
        {
            List<ElementPlayerBase> players = new List<ElementPlayerBase>();
            lock (syncObject)
            {
                players.AddRange(m_SubPlayers.Keys);
            }
            foreach (ElementPlayerBase player in players)
            {
                player.StopMusic();
            }
        }

        public void StopSounds()
        {
            List<ElementPlayerBase> players = new List<ElementPlayerBase>();
            lock (syncObject)
            {
                players.AddRange(m_SubPlayers.Keys);
            }
            foreach (ElementPlayerBase player in players)
            {
                player.StopSounds();
            }
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

        public void SubPlayerFinished(ElementPlayerBase subPlayer)
        {
            Monitor.Enter(syncObject);
            m_SubPlayers.Remove(subPlayer);
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
        }

        public void Start()
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
                subPlayer.Start();
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

        protected abstract void FileStarted(IFileElement fileElement);
        protected abstract void FileFinished(Int32 id, Data.SoundFileType soundFileType);

        protected abstract void PlayerStarted();
        protected abstract void PlayerFinished();

        protected abstract bool Playing { get; set; }

        protected StartElementPlayer(IElement element, Action<StartElementPlayer> finishedAction)
        {
            m_Element = element;
            m_CurrentFiles = new List<int>();
            m_SubPlayers = new Dictionary<ElementPlayerBase,bool>();
            m_Stopped = false;
            m_StoppedEvent = new ManualResetEvent(false);
            m_FinishedAction = finishedAction;
        }

        private Dictionary<ElementPlayerBase, bool> m_SubPlayers;
        private List<Int32> m_CurrentFiles;
        private bool m_Stopped;
        private ManualResetEvent m_StoppedEvent;
        private Action<StartElementPlayer> m_FinishedAction;

        private IElement m_Element;

        protected Object syncObject = new Int16();
    }

    class SingleElementPlayer : StartElementPlayer
    {
        protected override void FileStarted(IFileElement fileElement)
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
            get; set; 
        }

        public SingleElementPlayer(IElement element, IElementPlayingCallbacks callbacks, Action<StartElementPlayer> finishedAction)
            : base(element, finishedAction)
        {
            m_Callbacks = callbacks;
            m_ElementId = element.Id;
        }

        private IElementPlayingCallbacks m_Callbacks;
        private int m_ElementId;
    }

    class ModeElementPlayer : StartElementPlayer
    {
        protected override void FileStarted(IFileElement fileElement)
        {
            if (m_Callbacks != null)
            {
                if (fileElement.SoundFileType == Data.SoundFileType.Music)
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

        public ModeElementPlayer(IModeElement modeElement, IProjectPlayingCallbacks callbacks, Action<StartElementPlayer> finishedAction)
            : base(modeElement.StartElement, finishedAction)
        {
            m_Mode = modeElement;
            m_Callbacks = callbacks;
        }

        private IModeElement m_Mode;
        private bool m_IsPlaying;
        private IProjectPlayingCallbacks m_Callbacks;
    }

    class Player : IProjectPlayer, IElementPlayer
    {
        public void SetProject(Data.IProject project)
        {
            m_Project = project;
            if (m_Project == null)
            {
                m_ActiveMode = null;
                return;
            }
            if (m_Project.GetModes().Count > 0)
            {
                m_ActiveMode = m_Project.GetModes()[0];
                if (ProjectCallbacks != null)
                {
                    ProjectCallbacks.ModeChanged(m_ActiveMode);
                }
            }
        }

        public bool KeyReceived(int keyCode)
        {
            if (m_Project == null)
                return false;
            IMode mode = m_Project.GetMode(keyCode);
            if (mode != null)
            {
                m_ActiveMode = mode;
                if (ProjectCallbacks != null)
                {
                    ProjectCallbacks.ModeChanged(mode);
                }
                return true;
            }
            else if (m_ActiveMode != null)
            {
                return TriggerElement(keyCode);
            }
            else
            {
                return false;
            }
        }

        public void NextMusicTitle()
        {
            if (ActiveMusicPlayer != null)
            {
                ActiveMusicPlayer.Next();
            }
        }

        public void PreviousMusicTitle()
        {
            if (ActiveMusicPlayer != null)
            {
                ActiveMusicPlayer.Previous();
            }
        }

        public void StopAll()
        {
            List<StartElementPlayer> copy = new List<StartElementPlayer>();
            foreach (IElement key in m_Players.Keys)
            {
                copy.AddRange(m_Players[key]);
            }
            copy.ForEach(player => player.Stop());
            if (nrOfActivePlayers > 0)
            {
                stoppingEvent.WaitOne();
            }
        }

        private void StopMusic()
        {
            List<StartElementPlayer> copy = new List<StartElementPlayer>();
            foreach (IElement key in m_Players.Keys)
            {
                copy.AddRange(m_Players[key]);
            }
            copy.ForEach(player => player.StopMusic());
        }

        private void StopSounds()
        {
            List<StartElementPlayer> copy = new List<StartElementPlayer>();
            foreach (IElement key in m_Players.Keys)
            {
                copy.AddRange(m_Players[key]);
            }
            copy.ForEach(player => player.StopSounds());
        }

        public void SetVolume(VolumeTarget target, Int32 value)
        {
            if (value < 0 || value > 100)
                throw new ArgumentException("Volume must be between 0 and 100");
            if (target == VolumeTarget.Music)
            {
                MusicVolume = value;
                if (ActiveMusicPlayer != null)
                {
                    ActiveMusicPlayer.SetMusicVolume(MusicVolume);
                }
            }
            else if (target == VolumeTarget.Sounds)
            {
                SoundVolume = value;
                List<StartElementPlayer> copy = new List<StartElementPlayer>();
                foreach (IElement key in m_Players.Keys)
                {
                    copy.AddRange(m_Players[key]);
                }
                copy.ForEach(player => player.SetSoundVolume(value));
            }
            else
            {
                if (!Un4seen.Bass.Bass.BASS_SetVolume(value / 100.0f))
                {
                    ErrorHandling.BassErrorOccurred(-1, StringResources.SetVolumeError);
                }
            }
        }

        public void SetMusicPath(String path)
        {
            MusicPath = path;
        }

        public void SetSoundPath(String path)
        {
            SoundPath = path;
        }

        private bool TriggerElement(int keyCode)
        {
            IModeElement element = m_ActiveMode.GetTriggeredElement(keyCode);
            if (element == null)
                return false;
            if (element.IsEndless() && element.IsPlaying)
            {
                StopElement(element);
            }
            else
            {
                StartElement(element);
            }
            return true;
        }

        private void DoStartElement(IElement element, StartElementPlayer player)
        {
            lock (syncObject)
            {
                if (!m_Players.ContainsKey(element))
                {
                    m_Players[element] = new List<StartElementPlayer>();
                }
                m_Players[element].Add(player);
                ++nrOfActivePlayers;
            }
            player.Start();
        }

        private void DoStopElement(IElement element)
        {
            StartElementPlayer player = null;
            lock (syncObject)
            {
                if (m_Players.ContainsKey(element))
                {
                    player = m_Players[element][0];
                }
            }
            if (player != null)
            {
                player.Stop();
            }
        }

        private void StopElement(IModeElement element)
        {
            DoStopElement(element);
        }

        private void StartElement(IModeElement element)
        {
            if (element.Trigger != null && element.Trigger.StopMusic)
            {
                StopMusic();
            }
            if (element.Trigger != null && element.Trigger.StopSounds)
            {
                StopSounds();
            }
            ModeElementPlayer player = new ModeElementPlayer(element, ProjectCallbacks, player2 => PlayerStopped(player2, element));
            DoStartElement(element, player);
        }

        public void PlayElement(IElement element)
        {
            SingleElementPlayer player = new SingleElementPlayer(element, ElementCallbacks, player2 => PlayerStopped(player2, element));
            DoStartElement(element, player);
        }

        public void StopElement(IElement element)
        {
            DoStopElement(element);
        }

        private void PlayerStopped(StartElementPlayer player, IElement element)
        {
            lock (syncObject)
            {
                if (m_Players.ContainsKey(element))
                {
                    m_Players[element].Remove(player);
                    if (m_Players[element].Count == 0)
                    {
                        m_Players.Remove(element);
                    }
                    --nrOfActivePlayers;
                }
            }
            if (nrOfActivePlayers <= 0)
                stoppingEvent.Set();
        }

        internal IProjectPlayingCallbacks ProjectCallbacks { get; set; }
        internal IElementPlayingCallbacks ElementCallbacks { get; set; }

        internal int MusicVolume { get; set; }
        internal int SoundVolume { get; set; }

        internal IMusicPlayer ActiveMusicPlayer
        {
            get
            {
                return m_ActiveMusicPlayer;
            }
            set
            {
                if (m_ActiveMusicPlayer != null && m_ActiveMusicPlayer != value)
                {
                    m_ActiveMusicPlayer.Stop();
                }
                m_ActiveMusicPlayer = value;
            }
        }

        private IMusicPlayer m_ActiveMusicPlayer;

        internal String MusicPath { get; private set; }
        internal String SoundPath { get; private set; }

        public Player()
        {
            m_Players = new Dictionary<IElement, List<StartElementPlayer>>();
            MusicVolume = 100;
            SoundVolume = 100;
        }

        private IProject m_Project;
        private IMode m_ActiveMode;

        private IDictionary<IElement, List<StartElementPlayer>> m_Players;

        private AutoResetEvent stoppingEvent = new AutoResetEvent(false);
        private int nrOfActivePlayers;
        private Object syncObject = new Int16();
    }
}
