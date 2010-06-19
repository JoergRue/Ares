using System;
using System.Collections.Generic;
using System.Threading;
using Ares.Data;

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

        public SoundFile(IFileElement element)
        {
            Id = element.Id;
            SoundFileType = element.SoundFileType == Data.SoundFileType.Music ? SoundFileType.Music : SoundFileType.SoundEffect;
            String dir = (SoundFileType == Playing.SoundFileType.Music) ? PlayingModule.ThePlayer.MusicPath : PlayingModule.ThePlayer.SoundPath;
            Path = System.IO.Path.Combine(dir, element.FilePath);
            Volume = SoundFileType == Playing.SoundFileType.Music ? PlayingModule.ThePlayer.MusicVolume : PlayingModule.ThePlayer.SoundVolume;
        }
    }

    interface IElementPlayerClient
    {
        int PlayFile(IFileElement fileElement, Action afterPlayed);

        bool Stopped { get; }

        void SubPlayerStarted();
        void SubPlayerFinished();
    }

    interface IMusicPlayer
    {
        void Next();
        void Previous();
        void Stop();
        void SetVolume(int volume);
    }

    abstract class ElementPlayerBase : IElementVisitor
    {
        public abstract void VisitFileElement(IFileElement fileElement);

        public virtual void VisitSequentialContainer(IElementContainer<ISequentialElement> sequentialContainer) { }

        public virtual void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer) { }

        public virtual void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer) { }

        public virtual void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList) { }

        public virtual void VisitRandomMusicList(IRandomBackgroundMusicList musicList) { }

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
            }
            if (element.SetsSoundVolume)
            {
                PlayingModule.ThePlayer.SetVolume(VolumeTarget.Sounds, element.MusicVolume);
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
    }

    abstract class MusicPlayer : ElementPlayerBase, IMusicPlayer
    {
        public override void VisitFileElement(IFileElement fileElement)
        {
            CurrentPlayedHandle = Client.PlayFile(fileElement, () =>
                {
                    bool stop = false;
                    lock (syncObject)
                    {
                        stop = shallStop;
                    }
                    if (stop)
                    {
                        Client.SubPlayerFinished();
                    }
                    else
                    {
                        PlayNext();
                    }
                });
            PlayingModule.ThePlayer.ActiveMusicPlayer = this;
        }

        public abstract void PlayNext();

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

        public void Stop()
        {
            lock (syncObject)
            {
                shallStop = true;
            }
            StopCurrentFile();
        }

        public void SetVolume(int volume)
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

        protected Object syncObject = new Int16();
        private bool shallStop = false;
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
            if (Client.Stopped)
            {
                Monitor.Exit(syncObject);
                Client.SubPlayerFinished();
                return;
            }
            ++m_Index;
            if (m_Index >= m_Container.GetElements().Count)
            {
                if (m_Container.Repeat)
                {
                    Monitor.Exit(syncObject);
                    Repeat(m_Container, m_Container);
                }
                else
                {
                    Monitor.Exit(syncObject);
                    Client.SubPlayerFinished();
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
        }

        public void Start()
        {
            if (m_Container.GetElements().Count > 0)
            {
                ThreadPool.QueueUserWorkItem(state => Process(m_Container));
            }
            else
            {
                Client.SubPlayerFinished();
            }
        }

        private ISequentialBackgroundMusicList m_Container;
        private int m_Index;
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
            if (Client.Stopped || !m_Container.Repeat)
            {
                Monitor.Exit(syncObject);
                Client.SubPlayerFinished();
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
        }

        private IRandomBackgroundMusicList m_Container;
        private List<IChoiceElement> m_LastElementsStack;
        private bool m_GoBack;
    }

    class ElementPlayer : ElementPlayerBase, IElementPlayerClient
    {
        // Interface IElementPlayerClient -- for sub-players 
        // in parallel containers
        // Mostly just pass through to our own client

        public int PlayFile(IFileElement element, Action afterPlayed)
        {
            return Client.PlayFile(element, afterPlayed);
        }

        public bool Stopped { get { return Client.Stopped; } }

        public void SubPlayerStarted()
        {
            Client.SubPlayerStarted();
            Interlocked.Increment(ref m_ActiveSubPlayers);
        }

        public void SubPlayerFinished()
        {
            Client.SubPlayerFinished();
            if (Interlocked.Decrement(ref m_ActiveSubPlayers) == 0)
            {
                // done with the parallel container or music player
                // note: there can't be several counts running in parallel,
                // because for parallel execution, new players are created
                Next();
            }
        }

        // Interface IElementVisitor

        public override void VisitFileElement(IFileElement fileElement)
        {
            Client.PlayFile(fileElement, () => Next());
        }

        public override void VisitSequentialContainer(IElementContainer<ISequentialElement> sequentialContainer)
        {
            foreach (ISequentialElement element in sequentialContainer.GetElements())
            {
                m_ElementStack.Add(new StackElement(element));
            }
            Next();
        }

        public override void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IParallelElement element in parallelContainer.GetElements())
            {
                ElementPlayer subPlayer = new ElementPlayer(this, StoppedEvent, element);
                Client.SubPlayerStarted();
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
                m_ElementStack.Add(new StackElement(element));
            }
            Next();
        }

        public override void VisitSequentialMusicList(ISequentialBackgroundMusicList list)
        {
            if (list.GetElements().Count == 0)
                return;
            SequentialMusicPlayer subPlayer = new SequentialMusicPlayer(list, StoppedEvent, this);
            Client.SubPlayerStarted();
            Interlocked.Increment(ref m_ActiveSubPlayers);
            subPlayer.Start();
        }

        public override void VisitRandomMusicList(IRandomBackgroundMusicList list)
        {
            if (list.GetElements().Count == 0)
                return;
            RandomMusicPlayer subPlayer = new RandomMusicPlayer(list, StoppedEvent, this);
            Client.SubPlayerStarted();
            Interlocked.Increment(ref m_ActiveSubPlayers);
            subPlayer.Start();
        }

        // Interface for the mode player

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(state => Next());
        }

        private void Next()
        {
            if (Client.Stopped)
            {
                return;
            }
            Monitor.Enter(syncObject);
            if (m_ElementStack.Count == 0)
            {
                // finished, nothing to play any more
                Monitor.Exit(syncObject);
                Client.SubPlayerFinished();
            }
            else
            {
                StackElement element = m_ElementStack[m_ElementStack.Count - 1];
                bool firstPlay = element.FirstPlay;
                IRepeatableElement repeatable = element.Element as IRepeatableElement;
                if (repeatable == null || !repeatable.Repeat)
                {
                    // will not be played again
                    m_ElementStack.RemoveAt(m_ElementStack.Count - 1);
                }
                else
                {
                    element.FirstPlay = false;
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
            m_ElementStack = new List<StackElement>();
            m_ElementStack.Add(new StackElement(startElement));
            m_ActiveSubPlayers = 0;
        }

        private class StackElement
        {
            public IElement Element { get; set; }
            public bool FirstPlay { get; set; }

            public StackElement(IElement element)
            {
                Element = element;
                FirstPlay = true;
            }
        }

        private List<StackElement> m_ElementStack;

        private Object syncObject = new Int16();
        
        private int m_ActiveSubPlayers;
    }

    abstract class StartElementPlayer : IElementPlayerClient
    {
        public int PlayFile(IFileElement fileElement, Action afterPlayed)
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
                    afterPlayed();
                });
            lock (syncObject)
            {
                m_CurrentFiles.Add(handle);
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

        public void SubPlayerStarted()
        {
            Interlocked.Increment(ref m_SubPlayers);
        }

        public void SubPlayerFinished()
        {
            Monitor.Enter(syncObject);
            if (Interlocked.Decrement(ref m_SubPlayers) == 0 && Playing)
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
                Interlocked.Increment(ref m_SubPlayers);
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
            m_SubPlayers = 0;
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
            m_SubPlayers = 0;
            m_Stopped = false;
            m_StoppedEvent = new ManualResetEvent(false);
            m_FinishedAction = finishedAction;
        }

        private int m_SubPlayers;
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
            m_Mode.IsPlaying = true;
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
                return m_Mode.IsPlaying;
            }
            set
            {
                m_Mode.IsPlaying = value;
            }
        }

        public ModeElementPlayer(IModeElement modeElement, IProjectPlayingCallbacks callbacks, Action<StartElementPlayer> finishedAction)
            : base(modeElement.StartElement, finishedAction)
        {
            m_Mode = modeElement;
            m_Callbacks = callbacks;
        }

        private IModeElement m_Mode;
        private IProjectPlayingCallbacks m_Callbacks;
    }

    class Player : IProjectPlayer, IElementPlayer
    {
        public void SetProject(Data.IProject project)
        {
            m_Project = project;
            if (m_Project.GetModes().Count > 0)
            {
                m_ActiveMode = m_Project.GetModes()[0];
                if (ProjectCallbacks != null)
                {
                    ProjectCallbacks.ModeChanged(m_ActiveMode);
                }
            }
        }

        public void KeyReceived(int keyCode)
        {
            if (m_Project == null)
                return;
            IMode mode = m_Project.GetMode(keyCode);
            if (mode != null)
            {
                m_ActiveMode = mode;
                if (ProjectCallbacks != null)
                {
                    ProjectCallbacks.ModeChanged(mode);
                }
            }
            else if (m_ActiveMode != null)
            {
                TriggerElement(keyCode);
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
            copy.AddRange(m_Players.Values);
            copy.ForEach(player => player.Stop());
            if (nrOfActivePlayers > 0)
            {
                stoppingEvent.WaitOne();
            }
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
                    ActiveMusicPlayer.SetVolume(MusicVolume);
                }
            }
            else if (target == VolumeTarget.Sounds)
            {
                SoundVolume = value;
            }
            else
            {
                Un4seen.Bass.Bass.BASS_SetVolume(value / 100.0f);
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

        private void TriggerElement(int keyCode)
        {
            IModeElement element = m_ActiveMode.GetTriggeredElement(keyCode);
            if (element.IsPlaying)
            {
                StopElement(element);
            }
            else
            {
                StartElement(element);
            }
        }

        private void DoStartElement(IElement element, StartElementPlayer player)
        {
            StartElementPlayer oldPlayer = null;
            lock (syncObject)
            {
                if (m_Players.ContainsKey(element))
                {
                    oldPlayer = m_Players[element];
                }
            }
            if (oldPlayer != null)
            {
                oldPlayer.Stop();
            }
            lock (syncObject)
            {
                m_Players[element] = player;
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
                    player = m_Players[element];
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
                m_Players.Remove(element);
                --nrOfActivePlayers;
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
            m_Players = new Dictionary<IElement, StartElementPlayer>();
            MusicVolume = 100;
            SoundVolume = 100;
        }

        private IProject m_Project;
        private IMode m_ActiveMode;

        private IDictionary<IElement, StartElementPlayer> m_Players;

        private AutoResetEvent stoppingEvent = new AutoResetEvent(false);
        private int nrOfActivePlayers;
        private Object syncObject = new Int16();
    }
}
