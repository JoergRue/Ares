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
                lock (m_Mutex)
                {
                    m_RunningStreams[channel] = new Action(() =>
                    {
                        Bass.BASS_StreamFree(channel);
                        callback(file.Id, channel);
                    });
                }
                Bass.BASS_ChannelSetSync(channel, BASSSync.BASS_SYNC_END, 0, m_EndSync, IntPtr.Zero);
                Bass.BASS_ChannelPlay(channel, false);
                return channel;
            }
        }

        public void StopFile(int handle)
        {
            Bass.BASS_ChannelStop(handle);
            FileFinished(handle);
        }

        public FilePlayer()
        {
            m_EndSync = new SYNCPROC(EndSync);
            m_RunningStreams = new Dictionary<int, Action>();
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
                }
            }
            if (endAction != null)
            {
                endAction();
            }
        }

        private SYNCPROC m_EndSync;

        private Dictionary<int, Action> m_RunningStreams;
        private Object m_Mutex = new Int16();

    }
}
