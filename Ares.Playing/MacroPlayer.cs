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
    class MacroPlayers : IProjectPlayingCallbacks
    {
        private static MacroPlayers sInstance = null;
        
        public static MacroPlayers Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new MacroPlayers();
                return sInstance;
            }
        }

        private Dictionary<int, List<EventWaitHandle>> m_WaitingPlayers = new Dictionary<int, List<EventWaitHandle>>();
        private HashSet<int> m_RunningElements = new HashSet<int>();

        public void AddWaitingPlayer(EventWaitHandle player, int elementId)
        {
            lock (m_LockObject)
            {
                if (!m_WaitingPlayers.ContainsKey(elementId))
                {
                    m_WaitingPlayers[elementId] = new List<EventWaitHandle>();
                }
                m_WaitingPlayers[elementId].Add(player);
            }
        }

        public void ClearWaitingPlayers()
        {
            lock (m_LockObject)
            {
                m_WaitingPlayers.Clear();
            }
        }

        public void RemoveWaitingPlayer(EventWaitHandle player, int elementId)
        {
            lock (m_LockObject)
            {
                RemoveWaitingPlayerNoLock(player, elementId);
            }
        }

        private void RemoveWaitingPlayerNoLock(EventWaitHandle player, int elementId)
        {
            if (m_WaitingPlayers.ContainsKey(elementId))
            {
                m_WaitingPlayers[elementId].Remove(player);
            }
        }

        public bool IsElementRunning(int elementId)
        {
            lock (m_LockObject)
            {
                return m_RunningElements.Contains(elementId);
            }
        }

        private MacroPlayers()
        {
            PlayingModule.AddCallbacks(this);
        }

        private Object m_LockObject = new Int32();

        private void Signal(int elementId)
        {
            List<EventWaitHandle> waitingPlayers = null;
            lock (m_LockObject)
            {
                if (m_WaitingPlayers.ContainsKey(elementId))
                {
                    waitingPlayers = new List<EventWaitHandle>(m_WaitingPlayers[elementId]);
                }
            }
            if (waitingPlayers != null)
            {
                foreach (EventWaitHandle player in waitingPlayers)
                {
                    player.Set();
                }
            }
        }

        public void ModeChanged(IMode newMode)
        {
        }

        public void ModeElementStarted(IModeElement element)
        {
            lock (m_LockObject)
            {
                m_RunningElements.Add(element.Id);
            }
            Signal(element.Id);
        }

        public void ModeElementFinished(IModeElement element)
        {
            lock (m_LockObject)
            {
                m_RunningElements.Remove(element.Id);
            }
            Signal(element.Id);
        }

        public void SoundStarted(int elementId)
        {
            lock (m_LockObject)
            {
                m_RunningElements.Add(elementId);
            }
            Signal(elementId);
        }

        public void SoundFinished(int elementId)
        {
            lock (m_LockObject)
            {
                m_RunningElements.Remove(elementId);
            }
            Signal(elementId);
        }

        public void MusicStarted(int elementId)
        {
            lock (m_LockObject)
            {
                m_RunningElements.Add(elementId);
            }
            Signal(elementId);
        }

        public void MusicFinished(int elementId)
        {
            lock (m_LockObject)
            {
                m_RunningElements.Remove(elementId);
            }
            Signal(elementId);
        }

        public void VolumeChanged(VolumeTarget target, int newValue)
        {
        }

        public void MusicPlaylistStarted(int elementId)
        {
        }

        public void MusicPlaylistFinished(int elementId)
        {
        }

        public void MusicRepeatChanged(bool repeat)
        {
        }

        public void MusicOnAllSpeakersChanged(bool onAllSpeakers)
        {
        }

        public void PreviousNextFadingChanged(bool fade, bool crossFade, int fadeTime)
        {
        }

        public void ErrorOccurred(int elementId, string errorMessage)
        {
        }

        public void AddMessage(MessageType messageType, String message)
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

        public void MusicTagCategoriesCombinationChanged(Data.TagCategoryCombination categoryCombination)
        {
        }

        public void MusicTagsChanged(System.Collections.Generic.ICollection<int> newTags, Data.TagCategoryCombination categoryCombination, int fadeTime)
        {
        }

        public void MusicTagsFadingChanged(int fadeTime, bool fadeOnlyOnChange)
        {
        }
    }

    class MacroPlayer : ElementPlayerBase, IMacroCommandVisitor
    {
        public enum MacroPlayerState
        {
            NotStarted,
            Running,
            WaitingForTimeout,
            WaitingForCondition,
            Finished
        }

        public MacroPlayerState State { get; set; }

        public MacroPlayer(IMacro macro, WaitHandle stoppedEvent, IElementPlayerClient client)
            : base(stoppedEvent, client)
        {
            m_Macro = macro;
            State = MacroPlayerState.NotStarted;
            m_ConditionEvent = new AutoResetEvent(false);
        }

        public void Start()
        {
            State = MacroPlayerState.Running;
            m_Macro.VisitMacro(this);
            State = MacroPlayerState.Finished;
            if (Client != null)
            {
                Client.SubPlayerFinished(this, false, false);
            }
        }

        private IMacro m_Macro;
        private EventWaitHandle m_ConditionEvent;

        private bool IsConditionFulfilled(IMacroCondition condition)
        {
            if (condition == null)
                return true;
            if (condition.Conditional == null)
                return true;
            switch (condition.ConditionType)
            {
                case MacroConditionType.ElementNotRunning:
                    return !MacroPlayers.Instance.IsElementRunning(condition.ConditionalId);
                case MacroConditionType.ElementRunning:
                    return MacroPlayers.Instance.IsElementRunning(condition.ConditionalId);
                case MacroConditionType.None:
                default:
                    return true;
            }
        }

        public void VisitStartCommand(IStartCommand startCommand)
        {
            if (Client.Stopped)
                return;
            if (IsConditionFulfilled(startCommand.Condition))
            {
                if (startCommand.StartedElement != null && (!startCommand.StartedElement.IsEndless() || !MacroPlayers.Instance.IsElementRunning(startCommand.StartedElementId)))
                {
                    PlayingModule.ProjectPlayer.SwitchElement(startCommand.StartedElementId);
                }
            }
        }

        public void VisitStopCommand(IStopCommand stopCommand)
        {
            if (Client.Stopped)
                return;
            if (IsConditionFulfilled(stopCommand.Condition))
            {
                if (MacroPlayers.Instance.IsElementRunning(stopCommand.StoppedElementId))
                {
                    PlayingModule.ProjectPlayer.SwitchElement(stopCommand.StoppedElementId);
                }
            }
        }

        public void VisitWaitTimeCommand(IWaitTimeCommand waitTimeCommand)
        {
            if (Client.Stopped)
                return;
            if (IsConditionFulfilled(waitTimeCommand.Condition))
            {
                State = MacroPlayerState.WaitingForTimeout;
                bool stopped = StoppedEvent.WaitOne(waitTimeCommand.TimeInMillis);
                State = MacroPlayerState.Running;
            }
        }


        public void VisitWaitConditionCommand(IWaitConditionCommand waitConditionCommand)
        {
            if (Client.Stopped)
                return;
            if (IsConditionFulfilled(waitConditionCommand.Condition))
            {
                while (!IsConditionFulfilled(waitConditionCommand.AwaitedCondition))
                {
                    State = MacroPlayerState.WaitingForCondition;
                    MacroPlayers.Instance.AddWaitingPlayer(m_ConditionEvent, waitConditionCommand.AwaitedCondition.ConditionalId);
                    WaitHandle[] handles = new WaitHandle[] { m_ConditionEvent, StoppedEvent };
                    
                    int signaled = WaitHandle.WaitAny(handles);
                    
                    MacroPlayers.Instance.RemoveWaitingPlayer(m_ConditionEvent, waitConditionCommand.AwaitedCondition.ConditionalId);
                    State = MacroPlayerState.Running;
                    if (signaled == 1)
                    {
                        // stop event signaled
                        break;
                    }
                }
            }
        }

        public void VisitTagCommand(ITagCommand tagCommand)
        {
            if (Client.Stopped)
                return;
            if (IsConditionFulfilled(tagCommand.Condition))
            {
                if (tagCommand.CommandType == MacroCommandType.AddTag)
                    PlayingModule.ProjectPlayer.AddMusicTag(tagCommand.CategoryId, tagCommand.TagId);
                else
                    PlayingModule.ProjectPlayer.RemoveMusicTag(tagCommand.CategoryId, tagCommand.TagId);
            }
        }

        public void VisitRemoveAllTagsCommand(IRemoveAllTagsCommand tagCommand)
        {
            if (Client.Stopped)
                return;
            if (IsConditionFulfilled(tagCommand.Condition))
            {
                PlayingModule.ProjectPlayer.RemoveAllMusicTags();
            }
        }

        public override void VisitFileElement(IFileElement fileElement)
        {
        }

        public override void VisitWebRadioElement(IWebRadioElement webRadio)
        {
        }

        public override void VisitLightEffects(ILightEffects lightEffects)
        {
            throw new NotImplementedException();
        }

        public override bool StopMusic(int crossFadeMusicTime)
        {
            return false;
        }

        public override bool StopSounds(int fadeTime)
        {
            return false;
        }

        public override void SetSoundVolume(int volume)
        {
        }
    }
}
