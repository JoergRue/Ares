using System;
using System.Collections.Generic;
using Un4seen.Bass;

namespace Ares.Playing
{
    class FilePlayer : IFilePlayer
    {
        public int PlayFile(ISoundFile file, PlayingFinished callback)
        {
            int channel = 0;
            channel = Bass.BASS_StreamCreateFile(file.Path, 0, 0, BASSFlag.BASS_DEFAULT);
            if (channel == 0)
            {
                // TODO: better exception
                throw new Exception("Couldn't create stream from file");
            }
            else
            {
                int volumeEffect = Bass.BASS_ChannelSetFX(channel, BASSFXType.BASS_FX_BFX_VOLUME, 1);
                if (volumeEffect == 0)
                {
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error: " + error);
                }
                lock (m_Mutex)
                {
                    m_RunningStreams[channel] = new Action(() =>
                    {
                        Bass.BASS_StreamFree(channel);
                        callback(file.Id, channel);
                    });
                    m_RunningVolumeEffects[channel] = volumeEffect;
                }
                Bass.BASS_ChannelSetSync(channel, BASSSync.BASS_SYNC_END, 0, m_EndSync, IntPtr.Zero);
                Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME vol = new Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME(file.Volume / 100.0f);
                Bass.BASS_FXSetParameters(volumeEffect, vol);
                Bass.BASS_ChannelPlay(channel, false);
                return channel;
            }
        }

        public void StopFile(int handle)
        {
            Bass.BASS_ChannelStop(handle);
            FileFinished(handle);
        }

        public void SetVolume(int handle, int volume)
        {
            if (volume < 0 || volume > 100)
                throw new ArgumentException("Invalid volume!");
            lock (m_Mutex)
            {
                if (!m_RunningVolumeEffects.ContainsKey(handle))
                    return;
                Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME vol = new Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME(volume / 100.0f);
                Bass.BASS_FXSetParameters(m_RunningVolumeEffects[handle], vol);
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
