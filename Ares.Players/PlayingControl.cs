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
using System.Linq;
using System.Text;

using Ares.Playing;
using Ares.Data;

namespace Ares.Players
{
    public class PlayingControl : IProjectPlayingCallbacks, IDisposable
    {
        public PlayingControl()
        {
            PlayingModule.AddCallbacks(this);
        }

        public void Dispose()
        {
            PlayingModule.RemoveCallbacks(this);
        }

        public void UpdateDirectories()
        {
            Settings.Settings settings = Settings.Settings.Instance;
            PlayingModule.ProjectPlayer.SetMusicPath(settings.MusicDirectory);
            PlayingModule.ProjectPlayer.SetSoundPath(settings.SoundDirectory);
        }

        private int ModifyVolume(ref int volume, bool up)
        {
            lock (syncObject)
            {
                if (up)
                {
                    volume = volume < 95 ? volume + 5 : 100;
                }
                else
                {
                    volume = volume > 5 ? volume - 5 : 0;
                }
                return volume;
            }
        }

        public bool KeyReceived(int key)
        {
            if (key == (int)Keys.Escape)
            {
                PlayingModule.ProjectPlayer.StopAll();
                if (Settings.Settings.Instance.UseStreaming)
                {
                    PlayingModule.Streamer.EndStreaming();
                }
                return true;
            }
            else if (key == (int)Keys.Up)
            {
                int value = ModifyVolume(ref m_GlobalVolume, true);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Both, value);
                return true;
            }
            else if (key == (int)Keys.Down)
            {
                int value = ModifyVolume(ref m_GlobalVolume, false);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Both, value);
                return true;
            }
            else if (key == (int)Keys.PageUp)
            {
                int value = ModifyVolume(ref m_SoundVolume, true);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Sounds, value);
                return true;
            }
            else if (key == (int)Keys.PageDown)
            {
                int value = ModifyVolume(ref m_SoundVolume, false);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Sounds, value);
                return true;
            }
            else if (key == (int)Keys.Insert)
            {
                int value = ModifyVolume(ref m_MusicVolume, true);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Music, value);
                return true;
            }
            else if (key == (int)Keys.Delete)
            {
                int value = ModifyVolume(ref m_MusicVolume, false);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Music, value);
                return true;
            }
            else if (key == (int)Keys.Right)
            {
                PlayingModule.ProjectPlayer.NextMusicTitle();
                return true;
            }
            else if (key == (int)Keys.Left)
            {
                PlayingModule.ProjectPlayer.PreviousMusicTitle();
                return true;
            }
            else
            {
                if (Settings.Settings.Instance.UseStreaming && !PlayingModule.Streamer.IsStreaming)
                {
                    PlayingModule.Streamer.BeginStreaming(CreateStreamingParameters());
                }
                return PlayingModule.ProjectPlayer.KeyReceived(key);
            }
        }

        public void SelectMusicElement(Int32 elementId)
        {
            if (Settings.Settings.Instance.UseStreaming && !PlayingModule.Streamer.IsStreaming)
            {
                PlayingModule.Streamer.BeginStreaming(CreateStreamingParameters());
            }
            PlayingModule.ProjectPlayer.SetMusicTitle(elementId);
        }

        public bool SwitchElement(Int32 elementId)
        {
            if (Settings.Settings.Instance.UseStreaming && !PlayingModule.Streamer.IsStreaming)
            {
                PlayingModule.Streamer.BeginStreaming(CreateStreamingParameters());
            }
            return PlayingModule.ProjectPlayer.SwitchElement(elementId);
        }

        public bool SetMode(IMode mode)
        {
            return PlayingModule.ProjectPlayer.SetMode(mode.Title);
        }

        public bool SetMode(String title)
        {
            return PlayingModule.ProjectPlayer.SetMode(title);
        }

        public void SetRepeatCurrentMusic(bool repeat)
        {
            PlayingModule.ProjectPlayer.RepeatCurrentMusic = repeat;
        }

        public void AddMusicTag(int categoryId, int tagId)
        {
            PlayingModule.ProjectPlayer.AddMusicTag(categoryId, tagId);
        }

        public void RemoveMusicTag(int categoryId, int tagId)
        {
            PlayingModule.ProjectPlayer.RemoveMusicTag(categoryId, tagId);
        }

        public void RemoveAllMusicTags()
        {
            PlayingModule.ProjectPlayer.RemoveAllMusicTags();
        }

        public void SetMusicTagCategoriesCombination(Data.TagCategoryCombination categoryCombination)
        {
            PlayingModule.ProjectPlayer.SetMusicTagCategoriesCombination(categoryCombination);
        }

        public void SetMusicTagFading(int fadeTime, bool fadeOnlyChange)
        {
            PlayingModule.ProjectPlayer.SetMusicTagFading(fadeTime, fadeOnlyChange);
        }

        public void SetPlayMusicOnAllSpeakers(bool onAllSpeakers)
        {
            PlayingModule.ProjectPlayer.SetPlayMusicOnAllSpeakers(onAllSpeakers);
        }

        public void SetFadingOnPreviousNext(bool fadeMusic, bool crossFadeMusic, int fadeTimeMs)
        {
            PlayingModule.ProjectPlayer.SetFadingOnPreviousNext(fadeMusic, crossFadeMusic, fadeTimeMs);
        }

        private Playing.StreamingParameters CreateStreamingParameters()
        {
            Playing.StreamingParameters result = new StreamingParameters();
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            result.Encoding = (StreamEncoding)settings.StreamingEncoder;
            result.Password = settings.StreamingPassword;
            result.ServerAddress = settings.StreamingServerAddress;
            result.ServerPort = settings.StreamingServerPort;
            result.Streamer = StreamerType.Icecast;
            result.StreamName = settings.StreamingStreamName;
            result.Bitrate = (StreamingBitrate)settings.StreamingBitrate;
            result.Username = settings.StreamingUserName;
            return result;
        }

        private Object syncObject = new Int16();

        private IMode m_CurrentMode;

        public IMode CurrentMode
        {
            get
            {
                lock (syncObject)
                {
                    return m_CurrentMode;
                }
            }
        }

        private List<IModeElement> m_ModeElements = new List<IModeElement>();

        public IList<IModeElement> CurrentModeElements
        {
            get
            {
                lock (syncObject)
                {
                    List<IModeElement> copy = new List<IModeElement>(m_ModeElements);
                    return copy;
                }
            }
        }

        private List<int> m_SoundElements = new List<int>();

        public IList<int> CurrentSoundElements
        {
            get
            {
                lock (syncObject)
                {
                    List<int> copy = new List<int>(m_SoundElements);
                    return copy;
                }
            }
        }

        private int m_MusicElement = -1;

        public int CurrentMusicElement
        {
            get
            {
                lock (syncObject)
                {
                    return m_MusicElement;
                }
            }
        }

        private int m_GlobalVolume = 100;

        public int GlobalVolume
        {
            get
            {
                lock (syncObject)
                {
                    return m_GlobalVolume;
                }
            }

            set
            {
                lock (syncObject)
                {
                    m_GlobalVolume = value;
                }
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Both, value);
            }
        }

        private int m_MusicVolume = 100;

        public int MusicVolume
        {
            get
            {
                lock (syncObject)
                {
                    return m_MusicVolume;
                }
            }

            set
            {
                lock (syncObject)
                {
                    m_MusicVolume = value;
                }
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Music, value);
            }
        }

        private int m_SoundVolume = 100;

        public int SoundVolume
        {
            get
            {
                lock (syncObject)
                {
                    return m_SoundVolume;
                }
            }

            set
            {
                lock (syncObject)
                {
                    m_SoundVolume = value;
                }
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Sounds, value);
            }
        }

        private bool m_MusicRepeat = false;
        private bool m_MusicOnAllSpeakers = false;
        private int m_FadeOnPreviousNextOption = 0;
        private int m_FadeOnPreviousNextTime = 0;

        public bool MusicRepeat
        {
            get
            {
                return m_MusicRepeat;
            }
        }

        public bool MusicOnAllSpeakers
        {
            get
            {
                return m_MusicOnAllSpeakers;
            }
        }

        public int FadingOnPreviousNext
        {
            get
            {
                return m_FadeOnPreviousNextOption;
            }
        }

        public int FadeOnPreviousNextTime
        {
            get
            {
                return m_FadeOnPreviousNextTime;
            }
        }

        public void ModeChanged(Data.IMode newMode)
        {
            lock (syncObject)
            {
                m_CurrentMode = newMode;
            }
        }

        public void ModeElementStarted(Data.IModeElement element)
        {
            lock (syncObject)
            {
                m_ModeElements.Add(element);
            }
        }

        public void ModeElementFinished(Data.IModeElement element)
        {
            lock (syncObject)
            {
                m_ModeElements.Remove(element);
            }
        }

        public void SoundStarted(int elementId)
        {
            lock (syncObject)
            {
                m_SoundElements.Add(elementId);
            }
        }

        public void SoundFinished(int elementId)
        {
            lock (syncObject)
            {
                m_SoundElements.Remove(elementId);
            }
        }

        public void MusicStarted(int elementId)
        {
            lock (syncObject)
            {
                m_MusicElement = elementId;
            }
        }

        public void MusicFinished(int elementId)
        {
            lock (syncObject)
            {
                if (m_MusicElement == elementId)
                    m_MusicElement = -1;
            }
        }

        public void VolumeChanged(VolumeTarget target, int newValue)
        {
            lock (syncObject)
            {
                if (target == VolumeTarget.Music)
                {
                    m_MusicVolume = newValue;
                }
                else
                {
                    m_SoundVolume = newValue;
                }
            }
        }

        private Int32 m_CurrentMusicList = -1;

        public Int32 CurrentMusicList
        {
            get
            {
                lock (syncObject)
                {
                    return m_CurrentMusicList;
                }
            }
        }

        public void MusicPlaylistStarted(int elementId)
        {
            lock (syncObject)
            {
                m_CurrentMusicList = elementId;
            }
        }

        public void MusicPlaylistFinished(int elementId)
        {
            lock (syncObject)
            {
                if (m_CurrentMusicList == elementId)
                    m_CurrentMusicList = -1;
            }
        }

        public void MusicRepeatChanged(bool repeat)
        {
            lock (syncObject)
            {
                m_MusicRepeat = repeat;
            }
        }

        public void MusicOnAllSpeakersChanged(bool onAllSpeakers)
        {
            lock (syncObject)
            {
                m_MusicOnAllSpeakers = onAllSpeakers;
            }
        }

        public void PreviousNextFadingChanged(bool fade, bool crossFade, int fadeTime)
        {
            lock (syncObject)
            {
                m_FadeOnPreviousNextOption = fade ? (crossFade ? 2 : 1) : 0;
                m_FadeOnPreviousNextTime = fadeTime;
            }
        }

        private HashSet<int> m_CurrentMusicTags = new HashSet<int>();

        public HashSet<int> GetCurrentMusicTags()
        {
            lock (syncObject)
            {
                return new HashSet<int>(m_CurrentMusicTags);
            }
        }

        public void MusicTagAdded(int tagId)
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Add(tagId);
            }
        }

        public void MusicTagRemoved(int tagId)
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Remove(tagId);
            }
        }

        public void AllMusicTagsRemoved()
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Clear();
            }
        }

        public void MusicTagsChanged(ICollection<int> newTags, Data.TagCategoryCombination categoryCombination, int fadeTime)
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Clear();
                m_CurrentMusicTags.UnionWith(newTags);
                m_MusicTagCategoriesCombination = categoryCombination;
            }
        }

        private int m_MusicTagsFadeTime;
        private bool m_MusicTagsFadeOnlyOnChange;

        public void MusicTagsFadingChanged(int fadeTime, bool fadeOnlyOnChange)
        {
            lock (syncObject)
            {
                m_MusicTagsFadeTime = fadeTime;
                m_MusicTagsFadeOnlyOnChange = fadeOnlyOnChange;
            }
        }

        public void GetMusicTagsFading(out int fadeTime, out bool fadeOnlyOnChange)
        {
            lock (syncObject)
            {
                fadeTime = m_MusicTagsFadeTime;
                fadeOnlyOnChange = m_MusicTagsFadeOnlyOnChange;
            }
        }

        private Data.TagCategoryCombination m_MusicTagCategoriesCombination = Data.TagCategoryCombination.UseAnyTag;

        public Data.TagCategoryCombination GetMusicTagCategoriesCombination()
        {
            lock (syncObject)
            {
                return m_MusicTagCategoriesCombination;
            }
        }

        public void MusicTagCategoriesCombinationChanged(Data.TagCategoryCombination categoryCombination)
        {
            lock (syncObject)
            {
                m_MusicTagCategoriesCombination = categoryCombination;
            }
        }

        public void ErrorOccurred(int elementId, String errorMessage)
        {
            Ares.Data.IElement element = Ares.Data.DataModule.ElementRepository.GetElement(elementId);
            if (element != null)
            {
                String message = String.Format(StringResources.PlayError, element.Title, errorMessage);
                Messages.AddMessage(MessageType.Error, message);
            }
            else
            {
                Messages.AddMessage(MessageType.Error, errorMessage);
            }
        }

        public void AddMessage(Playing.MessageType messageType, String message)
        {
            Messages.AddMessage((MessageType)(int)messageType, message);
        }
    }
}
