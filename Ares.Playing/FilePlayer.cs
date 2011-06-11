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
                lock (m_Mutex)
                {
                    m_RunningStreams[channel] = new Action(() =>
                    {
                        Bass.BASS_StreamFree(channel); // no error handling, unimportant
                        callback(file.Id, channel);
                    });
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
                        }
                        return 0;
                    }
                }
                float volume = file.Volume / 100.0f;
                if (file.Effects != null)
                {
                    volume = volume * file.Effects.Volume / 100.0f;
                }
                if (file.Effects != null && file.Effects.FadeInTime != 0)
                {

                    if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, 0.0f))
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                        return 0;
                    }
                    if (!Bass.BASS_ChannelSlideAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, volume, file.Effects.FadeInTime))
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                        return 0;
                    }
                }
                else
                {
                    if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, volume))
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                        return 0;
                    }
                }
                if (file.Effects != null && file.Effects.FadeOutTime != 0)
                {
                    long totalLength = Bass.BASS_ChannelGetLength(channel);
                    if (totalLength == -1)
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                        return 0;
                    }
                    long fadeOutLength = Bass.BASS_ChannelSeconds2Bytes(channel, 0.001 * file.Effects.FadeOutTime);
                    if (fadeOutLength == -1)
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                        return 0;
                    }
                    if (fadeOutLength > totalLength)
                    {
                        fadeOutLength = totalLength;
                    }
                    if (Bass.BASS_ChannelSetSync(channel, BASSSync.BASS_SYNC_POS, totalLength - fadeOutLength, m_FadeOutSync, new IntPtr(file.Effects.FadeOutTime)) == 0)
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                        return 0;
                    }
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
                    }
                    return 0;
                }
                return channel;
            }
        }

        public void StopFile(int handle)
        {
            StopFile(handle, false, 0);
        }

        public void StopFile(int handle, bool fadeOut, int fadeOutTime)
        {
            if (!fadeOut || fadeOutTime == 0)
            {
                Bass.BASS_ChannelStop(handle); // no error handling, unimportant
                FileFinished(handle);
            }
            else
            {
                long pos = Bass.BASS_ChannelGetPosition(handle);
                long length = Bass.BASS_ChannelGetLength(handle);
                long remaining = length - pos;
                long fadeOutLength = Bass.BASS_ChannelSeconds2Bytes(handle, 0.001 * fadeOutTime);
                if (pos == -1 || length == -1 || fadeOutLength == -1 || remaining <= 0)
                {
                    // on error, just stop the file
                    Bass.BASS_ChannelStop(handle);
                    FileFinished(handle);
                    return;
                }
                if (fadeOutLength > remaining)
                {
                    fadeOutLength = remaining;
                }
                if (Bass.BASS_ChannelSetSync(handle, BASSSync.BASS_SYNC_POS, pos + fadeOutLength, m_StopSync, new IntPtr(0)) == 0)
                {
                    // on error, just stop the file
                    Bass.BASS_ChannelStop(handle);
                    FileFinished(handle);
                    return;
                }
                FadeOut(handle, fadeOutTime);
            }
        }

        public void SetVolume(int handle, int volume)
        {
            if (volume < 0 || volume > 100)
                throw new ArgumentException("Invalid volume!");
            bool error = false;
            lock (m_Mutex)
            {
                if (!Un4seen.Bass.Bass.BASS_ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_VOL, volume / 100.0f))
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
            m_FadeOutSync = new SYNCPROC(FadeOutSync);
            m_StopSync = new SYNCPROC(StopSync);
            m_RunningStreams = new Dictionary<int, Action>();
        }

        public void Dispose()
        {
        }

        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            FileFinished(channel);
        }

        private void StopSync(int handle, int channel, int data, IntPtr user)
        {
            Bass.BASS_ChannelStop(channel);
            FileFinished(channel);
        }

        private void FadeOutSync(int handle, int channel, int data, IntPtr user)
        {
            FadeOut(channel, user.ToInt32());
        }

        private void FadeOut(int channel, int time)
        {
            Bass.BASS_ChannelSlideAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, 0.0f, time);
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
                }
            }
            if (endAction != null)
            {
                endAction();
            }
        }

        private SYNCPROC m_EndSync;
        private SYNCPROC m_FadeOutSync;
        private SYNCPROC m_StopSync;

        private Dictionary<int, Action> m_RunningStreams;
        private Object m_Mutex = new Int16();

    }
}
