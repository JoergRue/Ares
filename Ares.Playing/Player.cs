/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
#if ANDROID
using Ares.Settings;
#endif

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

        public SoundFile(IFileElement element, bool playMusicOnAllSpeakers)
        {
            Id = element.Id;
            SoundFileType = element.SoundFileType == Data.SoundFileType.Music ? SoundFileType.Music : SoundFileType.SoundEffect;
            String dir = (SoundFileType == Playing.SoundFileType.Music) ? PlayingModule.ThePlayer.MusicPath : PlayingModule.ThePlayer.SoundPath;
#if ANDROID
			if (dir.IsSmbFile())
				Path = dir + element.FilePath;
			else
#endif
            	Path = System.IO.Path.Combine(dir, element.FilePath);
            Volume = SoundFileType == Playing.SoundFileType.Music ? PlayingModule.ThePlayer.MusicVolume : PlayingModule.ThePlayer.SoundVolume;
            Effects = element.Effects;
            if (playMusicOnAllSpeakers && element.SoundFileType == Data.SoundFileType.Music && !element.Effects.SpeakerAssignment.Active && !element.Effects.Balance.Active)
            {
                element.Effects.SpeakerAssignment.Active = true;
                element.Effects.SpeakerAssignment.Assignment = SpeakerAssignment.AllSpeakers;
            }
        }

        public SoundFile(IWebRadioElement element, bool playMusicOnAllSpeakers)
        {
            Id = element.Id;
            SoundFileType = SoundFileType.WebRadio;
            Path = element.Url;
            Volume = PlayingModule.ThePlayer.MusicVolume;
            Effects = element.Effects;
            if (playMusicOnAllSpeakers && !element.Effects.SpeakerAssignment.Active && !element.Effects.Balance.Active)
            {
                element.Effects.SpeakerAssignment.Active = true;
                element.Effects.SpeakerAssignment.Assignment = SpeakerAssignment.AllSpeakers;
            }
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

        public static void ErrorOccurred(int elementID, String message)
        {
            if (PlayingModule.ThePlayer.ElementCallbacks != null)
            {
                PlayingModule.ThePlayer.ElementCallbacks.ErrorOccurred(elementID, message);
            }
            else if (PlayingModule.ThePlayer.ProjectCallbacks != null)
            {
                PlayingModule.ThePlayer.ProjectCallbacks.ErrorOccurred(elementID, message);
            }
        }

        public static void AddMessage(MessageType messageType, String message)
        {
            if (PlayingModule.ThePlayer.ElementCallbacks != null)
            {
                PlayingModule.ThePlayer.ElementCallbacks.AddMessage(messageType, message);
            }
            else if (PlayingModule.ThePlayer.ProjectCallbacks != null)
            {
                PlayingModule.ThePlayer.ProjectCallbacks.AddMessage(messageType, message);
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
        int PlayFile(IFileElement fileElement, int fadeInTime, Action<bool> afterPlayed, bool loop);
        int PlayWebRadio(IWebRadioElement webRadioElement, int fadeInTime, Action<bool> afterPlayed);
        void PlayLightEffects(ILightEffects lightEffectsElement, Action afterPlayed);

        bool Stopped { get; }

        void SubPlayerStarted(ElementPlayerBase subPlayer);
        void SubPlayerFinished(ElementPlayerBase subPlayer, bool stopMusic, bool stopSounds);
        void SetMusicOnAllSpeakers(bool onAllSpeakers);
    }

    abstract class ElementPlayerBase : IElementVisitor
    {
        public abstract void VisitFileElement(IFileElement fileElement);

        public abstract void VisitWebRadioElement(IWebRadioElement webRadio);

        public virtual void VisitSequentialContainer(ISequentialContainer sequentialContainer) { }

        public virtual void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer) { }

        public virtual void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer) { }

        public virtual void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList) { }

        public virtual void VisitRandomMusicList(IRandomBackgroundMusicList musicList) { }

        public virtual void VisitMacro(IMacro macro) { }

        public virtual void VisitMacroCommand(IMacroCommand macroCommand) { }

        public virtual void VisitReference(IReferenceElement reference)
        {
            IElement referencedElement = DataModule.ElementRepository.GetElement(reference.ReferencedId);
            if (referencedElement != null)
            {
                referencedElement.Visit(this);
            }
        }

        public virtual void VisitMusicByTags(IMusicByTags musicByTags) { }

        public abstract void VisitLightEffects(ILightEffects lightEffects);

        public abstract bool StopMusic(int crossFadeMusicTime);
        public abstract bool StopSounds(int fadeTime);
        public abstract void SetSoundVolume(int volume);

        protected int CurrentPlayedHandle { get; set; }
        protected int CurrentFadeOut { get; set; }

        protected bool StopCurrentFile(bool allowFadeOut, bool crossFade, int fadeOutTime)
        {
            lock (syncObject)
            {
                if (CurrentPlayedHandle != 0)
                {
                    int handle = CurrentPlayedHandle;
                    CurrentPlayedHandle = 0;
                    int fadeOut = CurrentFadeOut;
                    if (fadeOutTime > 0 && CurrentFadeOut > 0)
                        fadeOut = Math.Min(CurrentFadeOut, fadeOutTime);
                    else if (fadeOut == 0)
                        fadeOut = fadeOutTime;
                    PlayingModule.FilePlayer.StopFile(handle, allowFadeOut, fadeOut, crossFade);
                    return true;
                }
                else
                    return false;
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
                lock (syncObject)
                {
                    int fadeInTime = m_MusicFadeInTime;
                    m_AfterDelay = (Object state, bool timedOut) =>
                    {
                        lock (syncObject)
                        {
                            if (m_RegisteredWaitHandle != null)
                            {
                                m_RegisteredWaitHandle.Unregister(null);
                                m_RegisteredWaitHandle = null;
                            }
                        }
                        if (timedOut && !Client.Stopped)
                        {
                            Process(element, fadeInTime);
                        }
                    };
                    if (!m_DelayStopped)
                    {
                        m_RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(StoppedEvent, m_AfterDelay, null, delay, true);
                    }
                    else
                    {
                        m_AfterDelay(null, true);
                    }
                }
            }
            else
            {
                Process(element, m_MusicFadeInTime);
            }
        }

        protected Action StopDelayWait()
        {
            WaitOrTimerCallback callback = null;
            lock (syncObject)
            {
                if (m_RegisteredWaitHandle != null)
                {
                    m_RegisteredWaitHandle.Unregister(null);
                    m_RegisteredWaitHandle = null;
                    callback = m_AfterDelay;
                }
                m_DelayStopped = true;
            }
            if (callback != null)
            {
                return () => callback(null, true);
            }
            else
            {
                return null;
            }
        }

        protected int m_MusicFadeInTime;

        protected void Process(IElement element, int musicFadeInTime)
        {
            if (element.SetsMusicVolume)
            {
                PlayingModule.ThePlayer.SetVolume(VolumeTarget.Music, element.MusicVolume);
                if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                {
                    PlayingModule.ThePlayer.ProjectCallbacks.VolumeChanged(VolumeTarget.Music, element.MusicVolume);
                }
            }
            if (element.SetsSoundVolume)
            {
                PlayingModule.ThePlayer.SetVolume(VolumeTarget.Sounds, element.SoundVolume);
                if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                {
                    PlayingModule.ThePlayer.ProjectCallbacks.VolumeChanged(VolumeTarget.Sounds, element.SoundVolume);
                }
            }
            m_MusicFadeInTime = musicFadeInTime;
            element.Visit(this);
            m_MusicFadeInTime = 0;
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
        private WaitOrTimerCallback m_AfterDelay;
        private bool m_DelayStopped = false;

        private WaitHandle m_StoppedEvent;
        private IElementPlayerClient m_Client;

        protected Object syncObject = new Int16();
    }

    class ElementPlayer : ElementPlayerBase, IElementPlayerClient
    {
        // Interface IElementPlayerClient -- for sub-players 
        // in parallel containers
        // Mostly just pass through to our own client

        public int PlayFile(IFileElement element, int fadeInTime, Action<bool> afterPlayed, bool loop)
        {
            return Client.PlayFile(element, fadeInTime, afterPlayed, loop);
        }

        public int PlayWebRadio(IWebRadioElement element, int fadeInTime, Action<bool> afterPlayed)
        {
            return Client.PlayWebRadio(element, fadeInTime, afterPlayed);
        }

        public void PlayLightEffects(ILightEffects element, Action afterPlayed)
        {
            Client.PlayLightEffects(element, afterPlayed);
        }

        public void SetMusicOnAllSpeakers(bool onAllSpeakers)
        {
            Client.SetMusicOnAllSpeakers(onAllSpeakers);
        }

        public bool Stopped { get { return Client.Stopped; } }

        public void SubPlayerStarted(ElementPlayerBase player)
        {
            Client.SubPlayerStarted(player);
            Interlocked.Increment(ref m_ActiveSubPlayers);
        }

        public void SubPlayerFinished(ElementPlayerBase player, bool stopMusic, bool stopSounds)
        {
            Client.SubPlayerFinished(player, stopMusic, stopSounds);
            if (stopMusic || stopSounds)
            {
                lock (syncObject)
                {
                    m_StopSounds = m_StopSounds || stopSounds;
                    m_StopMusic = m_StopMusic || stopMusic;
                }
            }
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
        public override bool StopMusic(int crossFadeMusicTime)
        {
            return false;
        }

        public override bool StopSounds(int fadeTime)
        {
            lock (syncObject)
            {
                m_StopSounds = true;
            }
            bool mustStop = false;
            Action action = StopDelayWait();
            if (StopCurrentFile(fadeTime > 0, false, fadeTime) || action != null)
            {
                mustStop = true;
            }
            if (action != null)
                action();
            return mustStop;
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
            bool stopMusic = false;
            lock (syncObject)
            {
                stopSounds = m_StopSounds;
                stopMusic = m_StopMusic;
            }
            if ((fileElement.SoundFileType == Data.SoundFileType.Music) && !stopMusic)
            {
                SingleMusicPlayer subPlayer = new SingleMusicPlayer(fileElement, StoppedEvent, this);
                Client.SubPlayerStarted(subPlayer);
                Interlocked.Increment(ref m_ActiveSubPlayers);
                if (MusicFadeInTime != 0)
                {
                    subPlayer.Start(MusicFadeInTime);
                    MusicFadeInTime = 0;
                }
                else
                {
                    subPlayer.Start(0);
                }
            }
            else if ((fileElement.SoundFileType == Data.SoundFileType.SoundEffect) && !stopSounds)
            {
                int handle = Client.PlayFile(fileElement, 0, (success) =>
                    {
                        lock (syncObject)
                        {
                            CurrentPlayedHandle = 0;
                            CurrentFadeOut = 0;
                            if (!success)
                                m_ElementQueue.Clear();
                        }
                        Next();
                    }, m_LoopElement);
                lock (syncObject)
                {
                    CurrentPlayedHandle = handle;
                    CurrentFadeOut = fileElement.Effects.FadeOutTime;
                }
            }
            else
            {
                Next();
            }
        }

        public override void VisitWebRadioElement(IWebRadioElement webRadio)
        {
            bool stopMusic = false;
            lock (syncObject)
            {
                stopMusic = m_StopMusic;
            }
            if (!stopMusic)
            {
                SingleMusicPlayer subPlayer = new SingleMusicPlayer(webRadio, StoppedEvent, this);
                Client.SubPlayerStarted(subPlayer);
                Interlocked.Increment(ref m_ActiveSubPlayers);
                if (MusicFadeInTime != 0)
                {
                    subPlayer.Start(MusicFadeInTime);
                    MusicFadeInTime = 0;
                }
                else
                {
                    subPlayer.Start(0);
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
                if (m_StopSounds)
                {
                    subPlayer.StopSounds(0);
                }
                Client.SubPlayerStarted(subPlayer);
                Interlocked.Increment(ref m_ActiveSubPlayers);
                subPlayer.Start(MusicFadeInTime);
            }
            MusicFadeInTime = 0;
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
            if (m_StopMusic)
            {
                RemoveElementAndContinue();
                return;
            }
            if (list.GetElements().Count == 0)
            {
                RemoveElementAndContinue();
                return;
            }
            SequentialMusicPlayer subPlayer = new SequentialMusicPlayer(list, StoppedEvent, this);
            Client.SubPlayerStarted(subPlayer);
            Interlocked.Increment(ref m_ActiveSubPlayers);
            subPlayer.Start(MusicFadeInTime);
            MusicFadeInTime = 0;
        }

        public override void VisitRandomMusicList(IRandomBackgroundMusicList list)
        {
            if (m_StopMusic)
            {
                RemoveElementAndContinue();
                return;
            }
            if (list.GetElements().Count == 0)
            {
                RemoveElementAndContinue();
                return;
            }
            RandomMusicPlayer subPlayer = new RandomMusicPlayer(list, StoppedEvent, this);
            Client.SubPlayerStarted(subPlayer);
            Interlocked.Increment(ref m_ActiveSubPlayers);
            subPlayer.Start(MusicFadeInTime);
            MusicFadeInTime = 0;
        }

        public override void VisitMacro(IMacro macro)
        {
            if (macro.GetElements().Count == 0)
            {
                RemoveElementAndContinue();
                return;
            }
            MacroPlayer subPlayer = new MacroPlayer(macro, StoppedEvent, this);
            Client.SubPlayerStarted(subPlayer);
            Interlocked.Increment(ref m_ActiveSubPlayers);
            subPlayer.Start();
        }

        private void RemoveElementAndContinue()
        {
            if (m_ElementQueue.Count > 0)
                m_ElementQueue.RemoveAt(0);
            Next();
        }

        public override void VisitMusicByTags(IMusicByTags musicByTags)
        {
            if (m_StopMusic)
            {
                RemoveElementAndContinue();
                return;
            }
            try
            {
                var readIf = Ares.Tags.TagsModule.GetTagsDB().ReadInterface;
                IList<String> choices;
                switch (musicByTags.TagCategoryCombination)
                {
                    case TagCategoryCombination.UseAnyTag:
                        choices = readIf.GetAllFilesWithAnyTagInEachCategory(musicByTags.GetTags());
                        break;
                    case TagCategoryCombination.UseOneTagOfEachCategory:
                        choices = readIf.GetAllFilesWithAnyTag(musicByTags.GetAllTags());
                        break;
                    case TagCategoryCombination.UseAllTags:
                    default:
                        choices = readIf.GetAllFilesWithAllTags(musicByTags.GetAllTags());
                        break;
                }
                var musicList = TagsMusicPlayer.CreateTagsMusicList(musicByTags.Title, choices, musicByTags.FadeTime);
                PlayingModule.ThePlayer.SetMusicByTagsElementPlayed(musicByTags);
                if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                {
                    PlayingModule.ThePlayer.ProjectCallbacks.MusicTagsChanged(musicByTags.GetAllTags(), musicByTags.TagCategoryCombination, musicByTags.FadeTime);
                }
                musicList.Visit(this);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                ErrorHandling.ErrorOccurred(musicByTags.Id, ex.Message);
            }
        }

        public override void VisitLightEffects(ILightEffects lightEffects)
        {
            Client.PlayLightEffects(lightEffects, () =>
                 {
                     Next();
                 });
        }

        // Interface for the mode player

        protected int MusicFadeInTime { get; set; }

        public void Start(int musicFadeInTime)
        {
            if (musicFadeInTime != 0)
                MusicFadeInTime = musicFadeInTime;
            ThreadPool.QueueUserWorkItem(state => Next());
        }

        private bool HasOnlyOneLoopableFileElement(IElement element)
        {
            return HasOnlyOneLoopableFileElement(new HashSet<IElement>(), element);
        }

        private bool HasOnlyOneLoopableFileElement(HashSet<IElement> checkedReferences, IElement element)
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
            else if (element is IReferenceElement)
            {
                if (checkedReferences.Contains(element))
                    return false;
                checkedReferences.Add(element);
                IElement referencedElement = Data.DataModule.ElementRepository.GetElement((element as IReferenceElement).ReferencedId);
                bool result = referencedElement != null ? HasOnlyOneLoopableFileElement(referencedElement) : false;
                checkedReferences.Remove(element);
                return result;
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
            if (m_ElementQueue.Count == 0 || (m_StopSounds && m_StopMusic))
            {
                // finished, nothing to play any more
                Monitor.Exit(syncObject);
                Client.SubPlayerFinished(this, m_StopMusic, m_StopSounds);
            }
            else
            {
                QueueElement element = m_ElementQueue[0];
                bool firstPlay = element.PlayCount == 0;
                ++element.PlayCount;
                IElement testElement = element.Element;
                IRepeatableElement repeatable = testElement as IRepeatableElement;
                IMusicList musicList = testElement as IMusicList;
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
                        Process(element.Element, MusicFadeInTime);
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
            m_StopMusic = false;
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

        private bool m_StopMusic;

        private bool m_LoopElement;
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
            MacroPlayers registerMacroPlayers = MacroPlayers.Instance;
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

        public bool SwitchElement(Int32 elementId)
        {
            if (m_Project == null)
                return false;
            MacroPlayers registerMacroPlayers = MacroPlayers.Instance;
            IElement element = Ares.Data.DataModule.ElementRepository.GetElement(elementId);
            if (element == null)
                return false;
            IModeElement modeElement = element as IModeElement;
            if (modeElement == null)
                return false;
            return TriggerElement(modeElement);
        }

        public bool SetMode(String modeTitle)
        {
            if (m_Project == null)
                return false;
            foreach (IMode mode in m_Project.GetModes())
            {
                if (mode.Title == modeTitle)
                {
                    m_ActiveMode = mode;
                    if (ProjectCallbacks != null)
                    {
                        ProjectCallbacks.ModeChanged(mode);
                    }
                    return true;
                }
            }
            return false;
        }

        public void NextMusicTitle()
        {
            if (ActiveMusicPlayer != null)
            {
                ActiveMusicPlayer.Next(m_FadeOnPreviousNext, m_CrossFadeOnPreviousNext, m_FadeTimeOnPreviousNext);
            }
        }

        public void PreviousMusicTitle()
        {
            if (ActiveMusicPlayer != null)
            {
                ActiveMusicPlayer.Previous(m_FadeOnPreviousNext, m_CrossFadeOnPreviousNext, m_FadeTimeOnPreviousNext);
            }
        }

        public void SetMusicTitle(Int32 elementId)
        {
            if (ActiveMusicPlayer != null)
            {
                ActiveMusicPlayer.PlayMusicTitle(elementId, m_FadeOnPreviousNext, m_CrossFadeOnPreviousNext, m_FadeTimeOnPreviousNext);
            }
        }

        private bool m_RepeatCurrentMusic = false;

        public bool RepeatCurrentMusic
        {
            get { return m_RepeatCurrentMusic; }
            set
            {
                m_RepeatCurrentMusic = value;
                if (ActiveMusicPlayer != null)
                {
                    ActiveMusicPlayer.RepeatCurrentMusic = value;
                }
                if (ProjectCallbacks != null)
                {
                    ProjectCallbacks.MusicRepeatChanged(value);
                }
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
            RemoveAllMusicTags();
            if (nrOfActivePlayers > 0)
            {
                stoppingEvent.WaitOne();
            }
        }

        private TagsMusicPlayer tagsPlayer = null;

        public void AddMusicTag(int categoryId, int tagId)
        {
            if (tagsPlayer == null)
            {
                tagsPlayer = new TagsMusicPlayer();
                bool dummy;
                tagsPlayer.SetCategoriesCombination(m_TagCategoryCombination, out dummy);
                tagsPlayer.SetFading(m_MusicTagsFadeTime, m_MusicTagsFadeOnlyOnChange);
            }
            bool hadFiles = true;
            bool mustChange = tagsPlayer.AddTag(categoryId, tagId, out hadFiles);
            HandleTagChange(mustChange, hadFiles);
            if (ProjectCallbacks != null)
            {
                ProjectCallbacks.MusicTagAdded(tagId);
            }
        }

        public void RemoveMusicTag(int categoryId, int tagId)
        {
            if (tagsPlayer != null)
            {
                bool hadFiles = true;
                bool mustChange = tagsPlayer.RemoveTag(categoryId, tagId, out hadFiles);
                HandleTagChange(mustChange, hadFiles);
            }
            if (ProjectCallbacks != null)
            {
                ProjectCallbacks.MusicTagRemoved(tagId);
            }
        }

        public void RemoveAllMusicTags()
        {
            DoRemoveAllMusicTags(true);
        }

        private Data.TagCategoryCombination m_TagCategoryCombination = Data.TagCategoryCombination.UseAnyTag;

        public void SetMusicTagCategoriesCombination(Data.TagCategoryCombination categoryCombination)
        {
            m_TagCategoryCombination = categoryCombination;
            if (tagsPlayer != null)
            {
                bool hadFiles = true;
                bool mustChange = tagsPlayer.SetCategoriesCombination(categoryCombination, out hadFiles);
                HandleTagChange(mustChange, hadFiles);
            }
            if (ProjectCallbacks != null)
            {
                ProjectCallbacks.MusicTagCategoriesCombinationChanged(categoryCombination);
            }
        }

        private bool m_PlayMusicOnAllSpeakers = false;

        public void SetPlayMusicOnAllSpeakers(bool onAllSpeakers)
        {
            m_PlayMusicOnAllSpeakers = onAllSpeakers;
            if (m_ActiveMusicPlayer != null)
                m_ActiveMusicPlayer.SetMusicOnAllSpeakers(onAllSpeakers);
            if (ProjectCallbacks != null)
            {
                ProjectCallbacks.MusicOnAllSpeakersChanged(onAllSpeakers);
            }
        }

        private bool m_FadeOnPreviousNext = false;
        private bool m_CrossFadeOnPreviousNext = false;
        private int m_FadeTimeOnPreviousNext = 0;

        public void SetFadingOnPreviousNext(bool fade, bool crossFade, int fadeTime)
        {
            m_FadeOnPreviousNext = fade;
            m_CrossFadeOnPreviousNext = crossFade;
            m_FadeTimeOnPreviousNext = fadeTime;
            if (ProjectCallbacks != null)
            {
                ProjectCallbacks.PreviousNextFadingChanged(fade, crossFade, fadeTime);
            }
        }

        public void SetMusicByTagsElementPlayed(IMusicByTags element)
        {
            if (tagsPlayer == null)
            {
                tagsPlayer = new TagsMusicPlayer();
                tagsPlayer.SetFading(m_MusicTagsFadeTime, m_MusicTagsFadeOnlyOnChange);
            }
            tagsPlayer.SetMusicByTagsElementPlayed(element);
        }

        private int m_MusicTagsFadeTime = 0;
        private bool m_MusicTagsFadeOnlyOnChange = false;

        public void SetMusicTagFading(int fadeTime, bool fadeOnlyOnChange)
        {
            m_MusicTagsFadeTime = fadeTime;
            m_MusicTagsFadeOnlyOnChange = fadeOnlyOnChange;
            if (tagsPlayer != null)
            {
                tagsPlayer.SetFading(fadeTime, fadeOnlyOnChange);
            }
            if (ProjectCallbacks != null)
            {
                ProjectCallbacks.MusicTagsFadingChanged(fadeTime, fadeOnlyOnChange);
            }
        }

        private void HandleTagChange(bool mustChangeTagMusic, bool wasPlayingTagMusic)
        {
            if (mustChangeTagMusic)
            {
                if (tagsPlayer.MusicList.GetElements().Count == 0)
                {
                    if (m_ActiveMusicPlayer != null)
                    {
                        m_ActiveMusicPlayer.ChangeRandomList(tagsPlayer.MusicList);
                    }
                    // and a new title must be selected, because the old one
                    // isn't eligible any more
                    NextMusicTitle();
                }
                /*
                // either the currently played title isn't eligible any more
                // or no title was playing and now there are titles
                if (wasPlayingTagMusic)
                {
                    // music list has changed
                    if (m_ActiveMusicPlayer != null)
                    {
                        m_ActiveMusicPlayer.ChangeRandomList(tagsPlayer.MusicList);
                    }
                    // and a new title must be selected, because the old one
                    // isn't eligible any more
                    NextMusicTitle();
                }
                */
                else
                {
                    // no tag music was playing, but now we have files
                    // --> start the player
                    StartElement(tagsPlayer.ModeElement);
                }
            }
            else
            {
                // either the currently played title is still eligible
                // or there are still no titles to play
                if (wasPlayingTagMusic && m_ActiveMusicPlayer != null)
                {
                    // music list has changed
                    m_ActiveMusicPlayer.ChangeRandomList(tagsPlayer.MusicList);
                }
            }
        }

        private void DoRemoveAllMusicTags(bool stopMusic)
        {
            if (tagsPlayer != null)
            {
                bool hadFiles = true;
                bool mustChange = tagsPlayer.RemoveAllTags(out hadFiles);
                HandleTagChange(mustChange, hadFiles);
            }
            if (ProjectCallbacks != null)
            {
                ProjectCallbacks.AllMusicTagsRemoved();
            }
        }

        private ManualResetEvent StopMusic(int crossFadeMusicTime)
        {
            List<StartElementPlayer> copy = new List<StartElementPlayer>();
            foreach (IElement key in m_Players.Keys)
            {
                copy.AddRange(m_Players[key]);
            }
            ManualResetEvent mre = null;
            foreach (StartElementPlayer player in copy)
            {
                mre = player.StopMusic(crossFadeMusicTime);
            }
            return mre;
        }

        private List<ManualResetEvent> StopSounds(int fadeTime)
        {
            List<StartElementPlayer> copy = new List<StartElementPlayer>();
            foreach (IElement key in m_Players.Keys)
            {
                copy.AddRange(m_Players[key]);
            }
            List<ManualResetEvent> events = new List<ManualResetEvent>();
            foreach (StartElementPlayer player in copy)
            {
                ManualResetEvent mre = player.StopSounds(fadeTime);
                if (mre != null)
                    events.Add(mre);
            }
            return events;
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
            return TriggerElement(element);
        }

        private bool TriggerElement(IModeElement element)
        {
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

        private void DoStartElement(IElement element, StartElementPlayer player, Int32 musicFadeInTime)
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
            player.Start(musicFadeInTime);
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

        private ModeElementPlayer m_NextModeElementPlayer;

        private void StartElement(IModeElement element)
        {
            if (m_NextModeElementPlayer != null)
            {
                m_NextModeElementPlayer.Stop();
                m_NextModeElementPlayer = null;
            }

            List<ManualResetEvent> stoppedEvents = new List<ManualResetEvent>();
            int fadeMusicTime = 0;
            if (element.Trigger != null && (element.Trigger.StopMusic || element.AlwaysStartsMusic()))
            {
                if (element.Trigger.CrossFadeMusic)
                    fadeMusicTime = element.Trigger.FadeMusicTime * 2; // because it will be calculated / 2 later again
                else if (element.Trigger.FadeMusic)
                    fadeMusicTime = element.Trigger.FadeMusicTime;
                var stoppedEvent = StopMusic(fadeMusicTime);
                if (stoppedEvent != null)
                    stoppedEvents.Add(stoppedEvent);
                else
                    fadeMusicTime = 0;
            }
            int fadeSoundsTime = 0;
            if (element.Trigger != null && element.Trigger.StopSounds)
            {
                if (element.Trigger.FadeSounds)
                    fadeSoundsTime = element.Trigger.FadeSoundTime;
                var stoppedEvents2 = StopSounds(fadeSoundsTime);
                stoppedEvents.AddRange(stoppedEvents2);
                if (stoppedEvents2.Count == 0)
                    fadeSoundsTime = 0;
            }
            ModeElementPlayer player = new ModeElementPlayer(element, ProjectCallbacks, player2 => PlayerStopped(player2, element), m_PlayMusicOnAllSpeakers);
            if (element.Trigger != null && element.Trigger.CrossFadeMusic && element.Trigger.FadeMusicTime > 0)
            {
                m_AllowTwoMusicPlayers = true;
                DoStartElement(element, player, element.Trigger.FadeMusicTime);
            }
            else if (stoppedEvents.Count > 0)
            {
                m_NextModeElementPlayer = player;
                int maxFadeTime = Math.Max(fadeMusicTime / 2, fadeSoundsTime);
                if (stoppedEvents.Count > 1)
                {
                    HashSet<ManualResetEvent> withoutDuplicates = new HashSet<ManualResetEvent>();
                    for (int i = 0; i < stoppedEvents.Count; ++i)
                        withoutDuplicates.Add(stoppedEvents[i]);
                    stoppedEvents.Clear();
                    stoppedEvents.AddRange(withoutDuplicates);
                }
                ThreadPool.QueueUserWorkItem((Object state) =>
                {
                    bool timedOut = !WaitHandle.WaitAll(stoppedEvents.ToArray(), maxFadeTime + 100);
                    if (timedOut && !player.Stopped)
                    {
                        m_NextModeElementPlayer = null;
                        DoStartElement(element, player, fadeMusicTime);
                    }
                },
                null);
            }
            else
            {
                DoStartElement(element, player, 0);
            }
        }

        public void PlayElement(IElement element)
        {
            SingleElementPlayer player = new SingleElementPlayer(element, ElementCallbacks, player2 => PlayerStopped(player2, element), m_PlayMusicOnAllSpeakers);
            DoStartElement(element, player, 0);
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
                    if (tagsPlayer != null && tagsPlayer.ModeElement == element)
                    {
                        DoRemoveAllMusicTags(false);
                    }
                }
            }
            if (nrOfActivePlayers <= 0)
                stoppingEvent.Set();
        }

        internal IProjectPlayingCallbacks ProjectCallbacks { get; set; }
        internal IElementPlayingCallbacks ElementCallbacks { get; set; }

        internal int MusicVolume { get; set; }
        internal int SoundVolume { get; set; }

        private bool m_AllowTwoMusicPlayers = false;

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
                    if (!m_AllowTwoMusicPlayers)
                    {
                        m_ActiveMusicPlayer.Stop();
                    }
                    else
                        m_AllowTwoMusicPlayers = false;
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
