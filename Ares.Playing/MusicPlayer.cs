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
        void ChangeRandomList(IRandomBackgroundMusicList newList);
    }

    abstract class MusicPlayer : ElementPlayerBase, IMusicPlayer
    {
        public virtual void ChangeRandomList(IRandomBackgroundMusicList newList)
        {
        }

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

        public override bool StopMusic(int crossFadeMusicTime)
        {
            Stop(crossFadeMusicTime);
            return true;
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
            IList<IChoiceElement> elements = RandomMusicList.GetElements();
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

        protected IRandomBackgroundMusicList RandomMusicList 
        { 
            get 
            {
                lock (syncObject)
                {
                    return m_Container;
                }
            } 
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
                Repeat(RandomMusicList, element);
            }
            else if (m_GoBack && m_LastElementsStack.Count > 1)
            {
                IChoiceElement element = m_LastElementsStack[m_LastElementsStack.Count - 2];
                m_LastElementsStack.RemoveAt(m_LastElementsStack.Count - 1);
                m_GoBack = false;
                m_FixedNext = -1;
                Monitor.Exit(syncObject);
                Repeat(RandomMusicList, element);
            }
            else
            {
                IChoiceElement element = SelectRandomElement(m_Container);
                if (element != null)
                {
                    m_LastElementsStack.Add(element);
                    m_GoBack = false;
                    m_FixedNext = -1;
                    Monitor.Exit(syncObject);
                    Repeat(RandomMusicList, element);
                }
                else
                {
                    Monitor.Exit(syncObject);
                    if (PlayingModule.ThePlayer.ProjectCallbacks != null)
                    {
                        PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistFinished();
                    }
                    Client.SubPlayerFinished(this, stop);
                }
            }
        }

        protected override bool IsSingleFileList()
        {
            return ((IMusicList)RandomMusicList).GetFileElements().Count < 2;
        }

        public override void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            // called on first start
            PlayNext();
        }

        public void Start(int musicFadeInTime)
        {
            if (RandomMusicList.GetElements().Count == 0)
            {
                Client.SubPlayerFinished(this, false);
                return;
            }
            PlayingModule.ThePlayer.ActiveMusicPlayer = this; // once early to stop previous music player
            if (PlayingModule.ThePlayer.ProjectCallbacks != null)
            {
                PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistStarted(RandomMusicList.Id);
            }
            Monitor.Enter(syncObject);
            // Don't delay, start immediately (start delay was already processed by calling player)
            IChoiceElement element = SelectRandomElement(m_Container);
            m_LastElementsStack.Add(element);
            m_GoBack = false;
            Monitor.Exit(syncObject);
            Process(element, musicFadeInTime);
        }

        public override void ChangeRandomList(IRandomBackgroundMusicList newList)
        {
            IRandomBackgroundMusicList list;
            list = Ares.ModelInfo.Playlists.ExpandRandomMusicList(newList, (String error) =>
                {
                    ErrorHandling.ErrorOccurred(newList.Id, error);
                });
            if (PlayingModule.ThePlayer.ProjectCallbacks != null)
            {
                PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistFinished();
            }
            lock (syncObject)
            {
                m_Container = list;
            }
            if (PlayingModule.ThePlayer.ProjectCallbacks != null)
            {
                PlayingModule.ThePlayer.ProjectCallbacks.MusicPlaylistStarted(RandomMusicList.Id);
            }
        }

        public RandomMusicPlayer(IRandomBackgroundMusicList list, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_LastElementsStack = new List<IChoiceElement>();
            m_GoBack = false;
            m_FixedNext = -1;
            m_RepeatCount = 0;
            m_Container = Ares.ModelInfo.Playlists.ExpandRandomMusicList(list, (String error) =>
            {
                ErrorHandling.ErrorOccurred(list.Id, error);
            });
        }

        private List<IChoiceElement> m_LastElementsStack;
        private bool m_GoBack;
        private int m_FixedNext;
        private int m_RepeatCount;
        private IRandomBackgroundMusicList m_Container;
    }

    class TagsMusicPlayer : IProjectPlayingCallbacks, IDisposable
    {
        private HashSet<int> m_TagsSet = new HashSet<int>();
        private Dictionary<int, HashSet<int>> m_TagsSetByCategory = new Dictionary<int, HashSet<int>>();
        private IList<String> m_CurrentChoices;
        private Object m_SyncObject = new Int32();
        private String m_CurrentFile;
        private int m_CurrentFileId;
        private IRandomBackgroundMusicList m_MusicList;
        private IModeElement m_ModeElement;
        private bool m_MusicByTagsElementPlayed;

        private int m_FadeTime = 0;
        private bool m_FadeOnlyOnChange = false;

        public bool AddTag(int categoryId, int tag, out bool hadFiles)
        {
            hadFiles = m_CurrentChoices.Count > 0;

            m_TagsSet.Add(tag);
            if (!m_TagsSetByCategory.ContainsKey(categoryId))
            {
                m_TagsSetByCategory[categoryId] = new HashSet<int>();
            }
            m_TagsSetByCategory[categoryId].Add(tag);
            return RetrieveCurrentChoices();
        }

        public bool RemoveTag(int categoryId, int tag, out bool hadFiles)
        {
            hadFiles = m_CurrentChoices.Count > 0;

            m_TagsSet.Remove(tag);
            if (m_TagsSetByCategory.ContainsKey(categoryId))
            {
                m_TagsSetByCategory[categoryId].Remove(tag);
                if (m_TagsSetByCategory[categoryId].Count == 0)
                {
                    m_TagsSetByCategory.Remove(categoryId);
                }
            }
            if (m_TagsSet.Count == 0)
            {
                m_CurrentChoices.Clear();
                UpdateMusicList();
                return hadFiles;
            }
            else
            {
                return RetrieveCurrentChoices();
            }
        }

        public bool RemoveAllTags(out bool hadFiles)
        {
            hadFiles = m_CurrentChoices.Count > 0;
            m_TagsSet.Clear();
            m_TagsSetByCategory.Clear();
            m_CurrentChoices.Clear();
            UpdateMusicList();
            if (m_MusicByTagsElementPlayed)
            {
                m_MusicByTagsElementPlayed = false;
                return true;
            }
            else
            {
                return hadFiles;
            }
        }

        private bool m_CategoriesOperatorIsAnd = false;

        public bool SetCategoriesOperator(bool isAnd, out bool hadFiles)
        {
            hadFiles = m_CurrentChoices.Count > 0;

            if (m_CategoriesOperatorIsAnd == isAnd)
            {
                return false;
            }

            m_CategoriesOperatorIsAnd = isAnd;
            return RetrieveCurrentChoices();
        }

        public IModeElement ModeElement { get { return m_ModeElement; } }

        public IRandomBackgroundMusicList MusicList { get { return m_MusicList; } }

        private String m_Title;

        public TagsMusicPlayer()
        {
            m_CurrentFile = String.Empty;
            m_CurrentFileId = -1;
            m_Title = StringResources.TaggedMusic;
            m_MusicList = Ares.Data.DataModule.ElementFactory.CreateRandomBackgroundMusicList(m_Title);
            m_ModeElement = Ares.Data.DataModule.ElementFactory.CreateModeElement(m_Title, m_MusicList);
            m_CurrentChoices = new List<String>();
            ProjectCallbacks.Instance.AddCallbacks(this);
        }

        public void Dispose()
        {
            Ares.Data.DataModule.ElementRepository.DeleteElement(m_MusicList.Id);
            ProjectCallbacks.Instance.RemoveCallbacks(this);
        }

        public void SetFading(int fadeTime, bool fadeOnlyOnChange)
        {
            m_FadeTime = fadeTime;
            m_FadeOnlyOnChange = fadeOnlyOnChange;
            if (m_MusicList != null)
            {
                foreach (IFileElement element in ((IMusicList)m_MusicList).GetFileElements())
                {
                    element.Effects.FadeInTime = fadeOnlyOnChange ? 0 : m_FadeTime / 2;
                    element.Effects.FadeOutTime = fadeOnlyOnChange ? 0 : m_FadeTime / 2;
                }
            }
            if (m_ModeElement != null && m_ModeElement.Trigger != null)
            {
                m_ModeElement.Trigger.FadeMusic = m_FadeTime > 0;
                m_ModeElement.Trigger.FadeMusicTime = m_FadeTime;
            }
        }

        public void SetMusicByTagsElementPlayed(Ares.Data.IMusicByTags musicByTags)
        {
            m_TagsSet.Clear();
            m_TagsSet.UnionWith(musicByTags.GetAllTags());
            m_TagsSetByCategory.Clear();
            IDictionary<int, HashSet<int>> tagsByCategories = musicByTags.GetTags();
            foreach (int category in tagsByCategories.Keys)
            {
                HashSet<int> tags = new HashSet<int>();
                tags.UnionWith(tagsByCategories[category]);
                m_TagsSetByCategory[category] = tags;
            }
            m_CategoriesOperatorIsAnd = musicByTags.IsOperatorAnd;
            m_FadeTime = musicByTags.FadeTime;
            m_MusicByTagsElementPlayed = true;
        }

        private bool RetrieveCurrentChoices()
        {
            IList<String> choices = null;
            try
            {
                Ares.Tags.ITagsDBRead dbRead = Ares.Tags.TagsModule.GetTagsDB().ReadInterface;
                if (!m_CategoriesOperatorIsAnd)
                {
                    choices = dbRead.GetAllFilesWithAnyTag(m_TagsSet);
                }
                else
                {
                    choices = dbRead.GetAllFilesWithAnyTagInEachCategory(m_TagsSetByCategory);
                }
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                ErrorHandling.ErrorOccurred(-1, String.Format(StringResources.TagsDbError, ex.Message));
                choices = new List<String>();
            }
            List<String> newChoices = new List<string>();
            foreach (String file in choices)
            {
                newChoices.Add(file.Replace('\\', System.IO.Path.DirectorySeparatorChar));
            }
            choices = newChoices;
            bool mustChange = false;
            lock (m_SyncObject)
            {
                m_CurrentChoices = choices;
                // must change music if 
                // either a title is playing which is not in the current choice list
                if (!String.IsNullOrEmpty(m_CurrentFile) && !m_CurrentChoices.Contains(m_CurrentFile))
                {
                    mustChange = true;
                }
                // or no title is playing --> then always change, even if the new list has no titles
                else if (String.IsNullOrEmpty(m_CurrentFile))
                {
                    mustChange = true;
                }
                if (m_MusicByTagsElementPlayed)
                {
                    m_MusicByTagsElementPlayed = false;
                    mustChange = true;
                }
            }
            UpdateMusicList();
            return mustChange;
        }

        public static IRandomBackgroundMusicList CreateTagsMusicList(String title, IList<String> choices, int fadeTime)
        {
            var newMusicList = Ares.Data.DataModule.ElementFactory.CreateRandomBackgroundMusicList(title);
            foreach (String file in choices)
            {
                IFileElement fileElement = Ares.Data.DataModule.ElementFactory.CreateFileElement(file, Data.SoundFileType.Music);
                if (fadeTime > 0)
                {
                    fileElement.Effects.FadeInTime = fadeTime / 2;
                    fileElement.Effects.FadeOutTime = fadeTime / 2;
                }
                newMusicList.AddElement(fileElement);
            }
            return newMusicList;
        }

        private void UpdateMusicList()
        {
            var newMusicList = CreateTagsMusicList(m_Title, m_CurrentChoices, m_FadeOnlyOnChange ? 0 : m_FadeTime);
            var newModeElement = Ares.Data.DataModule.ElementFactory.CreateModeElement(m_Title, newMusicList);
            newModeElement.Trigger = Ares.Data.DataModule.ElementFactory.CreateNoTrigger();
            newModeElement.Trigger.StopMusic = true;
            newModeElement.Trigger.CrossFadeMusic = false;
            if (m_FadeTime > 0)
            {
                newModeElement.Trigger.FadeMusic = true;
                newModeElement.Trigger.FadeMusicTime = m_FadeTime;
            }
            IRandomBackgroundMusicList oldMusicList;
            lock (m_SyncObject)
            {
                oldMusicList = m_MusicList;
                m_MusicList = newMusicList;
                m_ModeElement = newModeElement;
            }
            if (oldMusicList != null)
            {
                foreach (IChoiceElement element in oldMusicList.GetElements())
                {
                    Ares.Data.DataModule.ElementRepository.DeleteElement(element.Id);
                }
                Ares.Data.DataModule.ElementRepository.DeleteElement(oldMusicList.Id);
            }
        }


        public void ModeChanged(IMode newMode)
        {
        }

        public void ModeElementStarted(IModeElement element)
        {
        }

        public void ModeElementFinished(IModeElement element)
        {
        }

        public void SoundStarted(int elementId)
        {
        }

        public void SoundFinished(int elementId)
        {
        }

        public void MusicStarted(int elementId)
        {
            m_CurrentFileId = elementId;
            IElement element = Ares.Data.DataModule.ElementRepository.GetElement(elementId);
            if (element != null && element is IFileElement)
            {
                m_CurrentFile = (element as IFileElement).FilePath;
            }
        }

        public void MusicFinished(int elementId)
        {
            if (m_CurrentFileId == elementId)
            {
                m_CurrentFileId = -1;
                m_CurrentFile = String.Empty;
            }
        }

        public void VolumeChanged(VolumeTarget target, int newValue)
        {
        }

        public void MusicPlaylistStarted(int elementId)
        {
        }

        public void MusicPlaylistFinished()
        {
        }

        public void MusicPlaylistChanged()
        {
        }

        public void MusicRepeatChanged(bool repeat)
        {
        }

        public void ErrorOccurred(int elementId, string errorMessage)
        {
        }

        public void MusicTagAdded(int tagId)
        {
        }

        public void MusicTagRemoved(int tagId)
        {
        }

        public void AllMusicTagsRemoved()
        {
        }

        public void MusicTagCategoriesOperatorChanged(bool isAndOperator)
        {
        }

        public void MusicTagsChanged(ICollection<int> newTags, bool isAndOperator, int fadeTime)
        {
        }

        public void MusicTagsFadingChanged(int fadeTime, bool fadeOnlyOnChange)
        {
        }

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