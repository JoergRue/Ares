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
using Un4seen.Bass;

namespace Ares.Playing
{
    class FilePlayer : IFilePlayer
    {
        public int PlayFile(ISoundFile file, PlayingFinished callback, bool loop)
        {
            int channel = 0;
            channel = Bass.BASS_StreamCreateFile(file.Path, 0, 0, BASSFlag.BASS_DEFAULT);
            if (channel == 0)
            {
                ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                return 0;
            }
            else
            {
                int volumeEffect = Bass.BASS_ChannelSetFX(channel, BASSFXType.BASS_FX_BFX_VOLUME, 1);
                if (volumeEffect == 0)
                {
                    ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                    return 0;
                }
                lock (m_Mutex)
                {
                    m_RunningStreams[channel] = new Action(() =>
                    {
                        Bass.BASS_StreamFree(channel); // no error handling, unimportant
                        callback(file.Id, channel);
                    });
                    m_RunningVolumeEffects[channel] = volumeEffect;
                }
                if (!loop)
                {
                    if (Bass.BASS_ChannelSetSync(channel, BASSSync.BASS_SYNC_END, 0, m_EndSync, IntPtr.Zero) == 0)
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                        lock (m_Mutex)
                        {
                            Bass.BASS_StreamFree(channel);
                            m_RunningStreams.Remove(channel);
                            m_RunningVolumeEffects.Remove(channel);
                        }
                        return 0;
                    }
                }
                float volume = file.Volume / 100.0f;
                if (file.Effects != null)
                {
                    volume = volume * file.Effects.Volume / 100.0f;
                }
                Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME vol = new Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME(volume);
                if (!Bass.BASS_FXSetParameters(volumeEffect, vol))
                {
                    ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                    return 0;
                }
                if (loop)
                {
                    Bass.BASS_ChannelFlags(channel, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                }
                if (!Bass.BASS_ChannelPlay(channel, false))
                {
                    ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                    lock (m_Mutex)
                    {
                        Bass.BASS_StreamFree(channel);
                        m_RunningStreams.Remove(channel);
                        m_RunningVolumeEffects.Remove(channel);
                    }
                    return 0;
                }
                return channel;
            }
        }

        public void StopFile(int handle)
        {
            Bass.BASS_ChannelStop(handle); // no error handling, unimportant
            FileFinished(handle);
        }

        public void SetVolume(int handle, int volume)
        {
            if (volume < 0 || volume > 100)
                throw new ArgumentException("Invalid volume!");
            bool error = false;
            lock (m_Mutex)
            {
                if (!m_RunningVolumeEffects.ContainsKey(handle))
                    return;
                Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME vol = new Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME(volume / 100.0f);
                if (!Bass.BASS_FXSetParameters(m_RunningVolumeEffects[handle], vol))
                {
                    error = true;
                }
            }
            if (error)
            {
                ErrorHandling.BassErrorOccurred(-1, StringResources.SetVolumeError);
            }
        }

        public FilePlayer()
        {
            m_EndSync = new SYNCPROC(EndSync);
            m_RunningStreams = new Dictionary<int, Action>();
            m_RunningVolumeEffects = new Dictionary<int, int>();
        }

        public void Dispose()
        {
        }

        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            FileFinished(channel);
        }

        private void FileFinished(int channel)
        {
            Action endAction = null;
            lock (m_Mutex)
            {
                if (m_RunningStreams.ContainsKey(channel))
                {
                    endAction = m_RunningStreams[channel];
                    m_RunningStreams.Remove(channel);
                    m_RunningVolumeEffects.Remove(channel);
                }
            }
            if (endAction != null)
            {
                endAction();
            }
        }

        private SYNCPROC m_EndSync;

        private Dictionary<int, Action> m_RunningStreams;
        private Dictionary<int, int> m_RunningVolumeEffects;
        private Object m_Mutex = new Int16();

    }
}
