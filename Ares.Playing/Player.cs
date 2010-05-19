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
            set; 
        }

        public string Path
        {
            get;
            set;
        }

        public SoundFileType SoundFileType
        {
            get;
            set;
        }

        public SoundFile(IFileElement element)
        {
            Id = element.Id;
            Path = element.FilePath;
            SoundFileType = element.SoundFileType == Data.SoundFileType.Music ? SoundFileType.Music : SoundFileType.SoundEffect;
        }
    }

    interface IElementPlayerClient
    {
        void PlayFile(IFileElement fileElement, Action afterPlayed);

        bool Stopped { get; }

        void SubPlayerStarted();
        void SubPlayerFinished();

    }

    class ElementPlayer : IElementVisitor, IElementPlayerClient
    {
        // Interface IElementPlayerClient -- for sub-players 
        // in parallel containers
        // Mostly just pass through to our own client

        public void PlayFile(IFileElement element, Action afterPlayed)
        {
            m_Client.PlayFile(element, afterPlayed);
        }

        public bool Stopped { get { return m_Client.Stopped; } }

        public void SubPlayerStarted()
        {
            m_Client.SubPlayerStarted();
            Interlocked.Increment(ref m_ActiveSubPlayers);
        }

        public void SubPlayerFinished()
        {
            m_Client.SubPlayerFinished();
            if (Interlocked.Decrement(ref m_ActiveSubPlayers) == 0)
            {
                // done with the parallel container
                Next();
            }
        }

        // Interface IElementVisitor

        public void VisitFileElement(IFileElement fileElement)
        {
            m_Client.PlayFile(fileElement, () => Next());
        }

        public void VisitSequentialContainer(IElementContainer<ISequentialElement> sequentialContainer)
        {
            foreach (ISequentialElement element in sequentialContainer.GetElements())
            {
                m_ElementStack.Add(new StackElement(element));
            }
            Next();
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IParallelElement element in parallelContainer.GetElements())
            {
                ElementPlayer subPlayer = new ElementPlayer(this, m_StoppedEvent, element);
                m_Client.SubPlayerStarted();
                Interlocked.Increment(ref m_ActiveSubPlayers);
                subPlayer.Start();
            }
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
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
                        m_ElementStack.Add(new StackElement(element));
                        break;
                    }
                }
            }
            Next();
        }

        // Interface for the mode player

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(state => Next());
        }

        private void Next()
        {
            if (m_Client.Stopped)
            {
                return;
            }
            Monitor.Enter(syncObject);
            if (m_ElementStack.Count == 0)
            {
                // finished, nothing to play any more
                Monitor.Exit(syncObject);
                m_Client.SubPlayerFinished();
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

        private void Delay(IDelayableElement element, IElement original)
        {
            ProcessAfterDelay(original, element.FixedStartDelay, element.MaximumRandomStartDelay);
        }

        private void Repeat(IRepeatableElement element, IElement original)
        {
            ProcessAfterDelay(original, element.FixedIntermediateDelay, element.MaximumRandomIntermediateDelay);
        }

        private void ProcessAfterDelay(IElement element, TimeSpan fixedDelay, TimeSpan randomDelay)
        {
                TimeSpan delay = fixedDelay;
                if (randomDelay.Ticks > 0)
                {
                    int millis = (int) (randomDelay.Ticks / TimeSpan.TicksPerMillisecond);
                    delay = TimeSpan.FromTicks(delay.Ticks + PlayingModule.Randomizer.Next(millis) * TimeSpan.TicksPerMillisecond);
                }
                if (delay.Ticks > 0)
                {
                    m_RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(m_StoppedEvent, (Object state, bool timedOut) =>
                    {
                        m_RegisteredWaitHandle.Unregister(null);
                        if (timedOut && !m_Client.Stopped)
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

        private void Process(IElement element)
        {
            element.Visit(this);
        }


        public ElementPlayer(IElementPlayerClient client, WaitHandle stoppedEvent, IElement startElement)
        {
            m_ElementStack = new List<StackElement>();
            m_StoppedEvent = stoppedEvent;
            m_Client = client;
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
        
        private RegisteredWaitHandle m_RegisteredWaitHandle;
        private WaitHandle m_StoppedEvent;

        private IElementPlayerClient m_Client;

        private int m_ActiveSubPlayers;
    }

    class ModeElementPlayer : IElementPlayerClient
    {
        public void PlayFile(IFileElement fileElement, Action afterPlayed)
        {
            SoundFile soundFile = new SoundFile(fileElement);
            int handle = PlayingModule.FilePlayer.PlayFile(soundFile, (id, handle2) =>
            {
                if (m_Callbacks != null)
                {
                    if (fileElement.SoundFileType == Data.SoundFileType.Music)
                    {
                        m_Callbacks.MusicFinished(id);
                    }
                    else
                    {
                        m_Callbacks.SoundFinished(id);
                    }
                }
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
            if (Interlocked.Decrement(ref m_SubPlayers) == 0 &&  m_Mode.IsPlaying)
            {
                m_Mode.IsPlaying = false;
                Monitor.Exit(syncObject);
                if (m_Callbacks != null)
                {
                    m_Callbacks.ModeElementFinished(m_Mode);
                }
            }
            else
            {
                Monitor.Exit(syncObject);
            }
        }

        public void Start()
        {
            if (m_Callbacks != null)
            {
                m_Callbacks.ModeElementStarted(m_Mode);
            }
            m_Mode.IsPlaying = true;
            if (m_Mode.StartElement != null)
            {
                ElementPlayer subPlayer = new ElementPlayer(this, m_StoppedEvent, m_Mode.StartElement);
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
            if (m_Mode.IsPlaying)
            {
                m_Mode.IsPlaying = false;
                Monitor.Exit(syncObject);
                if (m_Callbacks != null)
                {
                    m_Callbacks.ModeElementFinished(m_Mode);
                }
            }
            else
            {
                Monitor.Exit(syncObject);
            }
        }

        public ModeElementPlayer(IModeElement modeElement, IPlayingCallbacks callbacks)
        {
            m_CurrentFiles = new List<int>();
            m_SubPlayers = 0;
            m_Mode = modeElement;
            m_Callbacks = callbacks;
            m_Stopped = false;
            m_StoppedEvent = new ManualResetEvent(false);
        }

        private List<Int32> m_CurrentFiles;
        private int m_SubPlayers;
        private IModeElement m_Mode;
        private IPlayingCallbacks m_Callbacks;

        private bool m_Stopped;
        private ManualResetEvent m_StoppedEvent;

        private Object syncObject = new Int16();

    }

    class Player : IPlayer
    {
        public void SetProject(Data.IProject project)
        {
            m_Project = project;
            if (m_Project.GetModes().Count > 0)
            {
                m_ActiveMode = m_Project.GetModes()[0];
                if (Callbacks != null)
                {
                    Callbacks.ModeChanged(m_ActiveMode);
                }
            }
            // TODO: set volume
        }

        public void KeyReceived(int keyCode)
        {
            if (m_Project == null)
                return;
            IMode mode = m_Project.GetMode(keyCode);
            if (mode != null)
            {
                m_ActiveMode = mode;
                if (Callbacks != null)
                {
                    Callbacks.ModeChanged(mode);
                }
            }
            else if (m_ActiveMode != null)
            {
                TriggerElement(keyCode);
            }
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

        private void StopElement(IModeElement element)
        {
            if (m_Players.ContainsKey(element))
            {
                m_Players[element].Stop();
                m_Players.Remove(element);
            }
        }

        private void StartElement(IModeElement element)
        {
            if (m_Players.ContainsKey(element))
            {
                m_Players[element].Stop();
                m_Players.Remove(element);
            }
            ModeElementPlayer player = new ModeElementPlayer(element, Callbacks);
            m_Players[element] = player;
            player.Start();
        }

        internal IPlayingCallbacks Callbacks { get; set; }

        public Player()
        {
            m_Players = new Dictionary<IModeElement, ModeElementPlayer>();
        }

        private IProject m_Project;
        private IMode m_ActiveMode;

        private IDictionary<IModeElement, ModeElementPlayer> m_Players;
    }
}
