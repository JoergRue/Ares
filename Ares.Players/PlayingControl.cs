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

        public void KeyReceived(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.Escape)
            {
                PlayingModule.ProjectPlayer.StopAll();
            }
            else if (key == System.Windows.Forms.Keys.Up)
            {
                int value = ModifyVolume(ref m_GlobalVolume, true);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Both, value);
            }
            else if (key == System.Windows.Forms.Keys.Down)
            {
                int value = ModifyVolume(ref m_GlobalVolume, false);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Both, value);
            }
            else if (key == System.Windows.Forms.Keys.PageUp)
            {
                int value = ModifyVolume(ref m_SoundVolume, true);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Sounds, value);
            }
            else if (key == System.Windows.Forms.Keys.PageDown)
            {
                int value = ModifyVolume(ref m_SoundVolume, false);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Sounds, value);
            }
            else if (key == System.Windows.Forms.Keys.Insert)
            {
                int value = ModifyVolume(ref m_MusicVolume, true);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Music, value);
            }
            else if (key == System.Windows.Forms.Keys.Delete)
            {
                int value = ModifyVolume(ref m_MusicVolume, false);
                PlayingModule.ProjectPlayer.SetVolume(VolumeTarget.Music, value);
            }
            else if (key == System.Windows.Forms.Keys.Right)
            {
                PlayingModule.ProjectPlayer.NextMusicTitle();
            }
            else if (key == System.Windows.Forms.Keys.Left)
            {
                PlayingModule.ProjectPlayer.PreviousMusicTitle();
            }
            else
            {
                PlayingModule.ProjectPlayer.KeyReceived((int)key);
            }
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
    }
}
