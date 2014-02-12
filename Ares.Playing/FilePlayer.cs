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
        public int PlayFile(ISoundFile file, int fadeInTime, PlayingFinished callback, bool loop)
        {
            int channel = 0;
            BASSFlag speakerFlag = GetSpeakerFlag(file);
            BASSFlag decodeFlag = BASSFlag.BASS_STREAM_DECODE;
            if (file.SoundFileType == SoundFileType.Music && file.Effects.SpeakerAssignment.Active)
            {
                switch (file.Effects.SpeakerAssignment.Assignment)
                {
                    case Data.SpeakerAssignment.AllSpeakers:
                    case Data.SpeakerAssignment.BothCenterRears:
                    case Data.SpeakerAssignment.BothFronts:
                    case Data.SpeakerAssignment.BothRears:
                    case Data.SpeakerAssignment.CenterAndSubwoofer:
                    case Data.SpeakerAssignment.Default:
                        break;
                    default:
                        decodeFlag |= BASSFlag.BASS_SAMPLE_MONO;
                        break;
                }
            }
#if MONO
			byte[] buffer = null;
			long length = 0;
			try 
			{
				System.IO.FileStream fs = System.IO.File.OpenRead(file.Path);
				length = fs.Length;
				buffer = new byte[length];
				fs.Read (buffer, 0, (int)length);
				fs.Close ();
			}
			catch (System.IO.IOException e)
			{
				ErrorHandling.ErrorOccurred(file.Id, e.Message);
				return 0;
			}
			System.Runtime.InteropServices.GCHandle gcHandle = System.Runtime.InteropServices.GCHandle.Alloc(buffer, System.Runtime.InteropServices.GCHandleType.Pinned);
			channel = Bass.BASS_StreamCreateFile(gcHandle.AddrOfPinnedObject(), 0L, length, decodeFlag);
#else
            channel = Bass.BASS_StreamCreateFile(file.Path, 0, 0, decodeFlag);
#endif
            if (channel == 0)
            {
#if MONO
				gcHandle.Free();
#endif
                ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                return 0;
            }
            RunningFileInfo info = new RunningFileInfo();
            bool isStreaming = BassStreamer.Instance.IsStreaming;
            bool useMultiSpeakerChannels = false;
            int speakers = 2;
            int origChannel = channel;
            if (!isStreaming 
                && file.Effects.SpeakerAssignment.Active && file.Effects.SpeakerAssignment.Assignment == Data.SpeakerAssignment.AllSpeakers 
                && !file.Effects.Balance.Active && !file.Effects.Pitch.Active && !file.Effects.Tempo.Active)
            {
                speakers = Bass.BASS_GetInfo().speakers;
                if (speakers > 2)
                {
                    useMultiSpeakerChannels = true;
                }
            }

            Un4seen.Bass.BASSFlag flags = BASSFlag.BASS_DEFAULT;
            if (isStreaming)
                flags = BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_STREAM_DECODE;
            else if (useMultiSpeakerChannels)
                flags = BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_STREAM_DECODE;
            else
                flags = BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_FX_FREESOURCE | speakerFlag;
            channel = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(channel, flags);
            if (channel == 0)
            {
#if MONO
				gcHandle.Free();
#endif
                ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                return 0;
            }
            bool result = true;
            if (useMultiSpeakerChannels)
            {
                int splitStream = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Split_StreamCreate(channel, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SPEAKER_FRONT, null);
                if (splitStream == 0)
                {
                    result = false;
                }
                else
                {
                    int splitStream2 = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Split_StreamCreate(channel, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SPEAKER_REAR, null);
                    if (splitStream2 == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        Bass.BASS_ChannelSetLink(splitStream, splitStream2);
                        info.LinkedChannels = new List<int>();
                        info.LinkedChannels.Add(splitStream2);
                    }
                    if (result && speakers > 4)
                    {
                        int splitStream3 = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Split_StreamCreate(channel, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SPEAKER_CENLFE, null);
                        if (splitStream3 == 0)
                        {
                            result = false;
                        }
                        else
                        {
                            Bass.BASS_ChannelSetLink(splitStream, splitStream3);
                            info.LinkedChannels.Add(splitStream3);
                        }
                    }
                    if (result && speakers > 6)
                    {
                        int splitStream4 = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Split_StreamCreate(channel, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SPEAKER_REAR2, null);
                        if (splitStream4 == 0)
                        {
                            result = false;
                        }
                        else
                        {
                            Bass.BASS_ChannelSetLink(splitStream, splitStream4);
                            info.LinkedChannels.Add(splitStream4);
                        }
                    }
                    if (result)
                        channel = splitStream;
                }
            }
            if (result)
            {
                lock (m_Mutex)
                {
                    info.EndAction = new Action(() =>
                    {
                        callback(file.Id, channel);
                    });
                    info.Volume = file.Volume;
                }
                if (!loop)
                {
                    int sync = Bass.BASS_ChannelSetSync(channel, BASSSync.BASS_SYNC_END, 0, m_EndSync, IntPtr.Zero);
                    if (sync == 0)
                    {
#if MONO
						gcHandle.Free();
#endif
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                        return 0;
                    }
                    else
                    {
                        m_NotLoops[channel] = sync;
                    }
                }
                if (!SetStartVolume(file, fadeInTime, channel, info))
                {
                    return 0;
                }
                info.CrossFade = false;
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
                    if (loop)
                    {
                        m_Loops[file.Id] = file;
                        if (Bass.BASS_ChannelSetSync(channel, BASSSync.BASS_SYNC_FREE, 0, m_EndSync2, new IntPtr(file.Id)) == 0)
                        {
                            ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                            return 0;
                        }
                        if (Bass.BASS_ChannelSetSync(channel, BASSSync.BASS_SYNC_POS, totalLength, m_LoopSync, new IntPtr(file.Id)) == 0)
                        {
                            ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                            return 0;
                        }
                    }
                    else
                    {
                        info.CrossFade = file.Effects.CrossFading;
                    }
                }
                if (file.Effects != null && file.Effects.Pitch.Active)
                {
                    float pitchValue = DetermineIntEffectValue(file.Effects.Pitch);
                    if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, pitchValue))
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                        return 0;
                    }
                }
                if (file.Effects != null && file.Effects.Tempo.Active)
                {
                    float tempoValue = DetermineIntEffectValue(file.Effects.Tempo);
                    if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_TEMPO, tempoValue))
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                        return 0;
                    }
                }
                if (file.Effects != null && file.Effects.Balance.Active && !useMultiSpeakerChannels)
                {
                    SetBalanceEffect(channel, file.Id, file.Effects.Balance);
                }
                if (file.Effects != null && file.Effects.VolumeDB.Active)
                {
                    float volumeDB = DetermineIntEffectValue(file.Effects.VolumeDB);
                    float linear = (float)Math.Pow(10d, volumeDB / 20);
                    int volFx = Bass.BASS_ChannelSetFX(channel, BASSFXType.BASS_FX_BFX_VOLUME, 1);
                    if (volFx == 0)
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                        return 0;
                    }
                    Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME fxVol = new Un4seen.Bass.AddOn.Fx.BASS_BFX_VOLUME(linear, Un4seen.Bass.AddOn.Fx.BASSFXChan.BASS_BFX_CHANALL);
                    if (!Bass.BASS_FXSetParameters(volFx, fxVol))
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                        return 0;
                    }
                    if (info.LinkedChannels != null) foreach (int splitStream2 in info.LinkedChannels)
                        {
                            int volFx2 = splitStream2 != 0 ? Bass.BASS_ChannelSetFX(splitStream2, BASSFXType.BASS_FX_BFX_VOLUME, 1) : 0;
                            if (splitStream2 != 0 && volFx2 == 0)
                            {
                                ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                                return 0;
                            }
                            if (volFx2 != 0 && !Bass.BASS_FXSetParameters(volFx2, fxVol))
                            {
                                ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                                return 0;
                            }
                        }
                }
                if (file.Effects != null && file.Effects.Reverb.Active)
                {
                    float linearLevel = (float)Math.Pow(10d, file.Effects.Reverb.Level / 20);
                    int reverbFx = Bass.BASS_ChannelSetFX(channel, BASSFXType.BASS_FX_BFX_REVERB, 1);
                    if (reverbFx == 0)
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                        return 0;
                    }
                    Un4seen.Bass.AddOn.Fx.BASS_BFX_REVERB fxReverb = new Un4seen.Bass.AddOn.Fx.BASS_BFX_REVERB(linearLevel, file.Effects.Reverb.Delay);
                    if (!Bass.BASS_FXSetParameters(reverbFx, fxReverb))
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                        return 0;
                    }
                    if (info.LinkedChannels != null) foreach (int splitStream2 in info.LinkedChannels)
                        {
                            int reverbFx2 = splitStream2 != 0 ? Bass.BASS_ChannelSetFX(splitStream2, BASSFXType.BASS_FX_BFX_REVERB, 1) : 0;
                            if (splitStream2 != 0 && reverbFx2 == 0)
                            {
                                ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                                return 0;
                            }
                            if (reverbFx2 != 0 && !Bass.BASS_FXSetParameters(reverbFx2, fxReverb))
                            {
                                ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetEffectError);
                                return 0;
                            }
                        }
                }
                if (loop)
                {
                    Bass.BASS_ChannelFlags(channel, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                }
                if (isStreaming)
                {
                    result = BassStreamer.Instance.AddChannel(channel);
                }
                else
                {
                    result = Bass.BASS_ChannelPlay(channel, false);
                }
                if (!result)
                {
                    ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                    Bass.BASS_StreamFree(channel);
#if MONO
                    gcHandle.Free();
#endif
                    return 0;
                }
                lock (m_Mutex)
                {
                    m_RunningFiles[channel] = info;
#if MONO
                    m_GCHandles[channel] = gcHandle;
#endif
                }
                return channel;
            }
            else
            {
                ErrorHandling.BassErrorOccurred(file.Id, StringResources.FilePlayingError);
                return 0;
            }
        }

        public void StopFile(int handle)
        {
            StopFile(handle, false, 0, false);
        }

        public void StopFile(int handle, bool fadeOut, int fadeOutTime, bool isCrossFade)
        {
            if (!fadeOut || fadeOutTime == 0)
            {
                Bass.BASS_ChannelStop(handle); // no error handling, unimportant
                FileFinished(handle, true);
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
                    FileFinished(handle, true);
                    return;
                }
                if (fadeOutLength > remaining)
                {
                    fadeOutLength = remaining;
                }
                // if (Bass.BASS_ChannelSetSync(handle, BASSSync.BASS_SYNC_POS, pos + fadeOutLength, m_StopSync, new IntPtr(0)) == 0)
                if (Bass.BASS_ChannelSetSync(handle, BASSSync.BASS_SYNC_SLIDE | BASSSync.BASS_SYNC_ONETIME, 0, m_StopSync, new IntPtr(handle)) == 0)
                {
                    // on error, just stop the file
                    Bass.BASS_ChannelStop(handle);
                    FileFinished(handle, true);
                    return;
                }
                FadeOut(handle, fadeOutTime);
                if (isCrossFade)
                {
                    // call finished immediately to start the next file
                    FileFinished(handle, false);
                }
            }
        }

        private static float DetermineVolume(Data.IEffects effects, float baseVolume, out float specificVolume)
        {
            if (!effects.HasRandomVolume)
            {
                specificVolume = effects.Volume / 100.0f;
            }
            else
            {
                specificVolume = (float)(0.01 * ((PlayingModule.Randomizer.NextDouble() * (effects.MaxRandomVolume - effects.MinRandomVolume) + effects.MinRandomVolume)));
            }
            return baseVolume * specificVolume;
        }

        private static float DetermineIntEffectValue(Data.IIntEffect effect)
        {
            if (!effect.Random)
            {
                return effect.FixValue;
            }
            else
            {
                return (float)(PlayingModule.Randomizer.NextDouble() * (effect.MaxRandomValue - effect.MinRandomValue) + effect.MinRandomValue);
            }
        }

        private static BASSFlag GetSpeakerFlag(ISoundFile file)
        {
            if (file.Effects == null || !file.Effects.SpeakerAssignment.Active)
                return BASSFlag.BASS_DEFAULT;
            Data.SpeakerAssignment assignment = file.Effects.SpeakerAssignment.Assignment;
            int speakers = Bass.BASS_GetInfo().speakers;
            while (true)
            {
                if (file.Effects.SpeakerAssignment.Random)
                {
                    assignment = (Data.SpeakerAssignment)(PlayingModule.Randomizer.Next(1, speakers + 1));
                }
                BASSFlag flag = BASSFlag.BASS_DEFAULT;
                int neededNrOfSpeakers = 2;
                switch (assignment)
                {
                    case Data.SpeakerAssignment.LeftFront:
                        flag = BASSFlag.BASS_SPEAKER_FRONTLEFT;
                        break;
                    case Data.SpeakerAssignment.RightFront:
                        flag = BASSFlag.BASS_SPEAKER_FRONTRIGHT;
                        break;
                    case Data.SpeakerAssignment.LeftBack:
                        flag = BASSFlag.BASS_SPEAKER_REARLEFT;
                        neededNrOfSpeakers = 5;
                        break;
                    case Data.SpeakerAssignment.RightBack:
                        flag = BASSFlag.BASS_SPEAKER_REARRIGHT;
                        neededNrOfSpeakers = 5;
                        break;
                    case Data.SpeakerAssignment.Center:
                        flag = BASSFlag.BASS_SPEAKER_CENTER;
                        neededNrOfSpeakers = 5;
                        break;
                    case Data.SpeakerAssignment.LeftCenterBack:
                        flag = BASSFlag.BASS_SPEAKER_REAR2LEFT;
                        neededNrOfSpeakers = 7;
                        break;
                    case Data.SpeakerAssignment.RightCenterBack:
                        flag = BASSFlag.BASS_SPEAKER_REAR2RIGHT;
                        neededNrOfSpeakers = 7;
                        break;
                    case Data.SpeakerAssignment.Subwoofer:
                        flag = BASSFlag.BASS_SPEAKER_LFE;
                        neededNrOfSpeakers = 5;
                        break;
                    case Data.SpeakerAssignment.BothFronts:
                        flag = BASSFlag.BASS_SPEAKER_FRONT;
                        break;
                    case Data.SpeakerAssignment.BothRears:
                        flag = BASSFlag.BASS_SPEAKER_REAR;
                        neededNrOfSpeakers = 5;
                        break;
                    case Data.SpeakerAssignment.BothCenterRears:
                        flag = BASSFlag.BASS_SPEAKER_REAR2;
                        neededNrOfSpeakers = 7;
                        break;
                    case Data.SpeakerAssignment.CenterAndSubwoofer:
                        flag = BASSFlag.BASS_SPEAKER_CENLFE;
                        neededNrOfSpeakers = 5;
                        break;
                    case Data.SpeakerAssignment.AllSpeakers:
                        flag = BASSFlag.BASS_DEFAULT;
                        break;
                    default:
                        break;
                }
                if (flag == BASSFlag.BASS_DEFAULT)
                    return BASSFlag.BASS_DEFAULT;
                if (neededNrOfSpeakers > speakers)
                {
                    if (file.Effects.SpeakerAssignment.Random)
                    {
                        // just choose another speaker
                        continue;
                    }
                    else
                    {
                        ErrorHandling.BassErrorOccurred(file.Id, StringResources.SpeakerNotAvailable);
                        return BASSFlag.BASS_DEFAULT;
                    }
                }
                else
                {
                    return flag;
                }
            }
        }

        private bool SetBalanceEffect(int channel, int id, Data.IBalanceEffect effect)
        {
            if (!effect.IsPanning)
            {
                float balanceValue = DetermineIntEffectValue(effect);
                // balance is stored as [-10..10], but must be set as [-1..1]
                if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_PAN, balanceValue / 10.0f))
                {
                    ErrorHandling.BassErrorOccurred(id, StringResources.SetEffectError);
                    return false;
                }
                return true;
            }
            else
            {
                long totalLength = Bass.BASS_ChannelGetLength(channel);
                if (totalLength == -1)
                {
                    ErrorHandling.BassErrorOccurred(id, StringResources.SetEffectError);
                    return false;
                }
                double totalLengthSeconds = Bass.BASS_ChannelBytes2Seconds(channel, totalLength);
                if (totalLengthSeconds < 0)
                {
                    ErrorHandling.BassErrorOccurred(id, StringResources.SetEffectError);
                    return false;
                }
                // first set to start value
                // balance is stored as [-10..10], but must be set as [-1..1]
                if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_PAN, effect.PanningStart / 10.0f))
                {
                    ErrorHandling.BassErrorOccurred(id, StringResources.SetEffectError);
                    return false;
                }
                // now slide over all time to end value
                if (!Bass.BASS_ChannelSlideAttribute(channel, BASSAttribute.BASS_ATTRIB_PAN, effect.PanningEnd / 10.0f, (int)(totalLengthSeconds * 1000)))
                {
                    ErrorHandling.BassErrorOccurred(id, StringResources.SetEffectError);
                    return false;
                }
                return true;
            }
        }

        public void SetVolume(int handle, int volume)
        {
            if (volume < 0 || volume > 100)
                throw new ArgumentException("Invalid volume!");
            bool error = false;
            lock (m_Mutex)
            {
                var info = m_RunningFiles.ContainsKey(handle) ? m_RunningFiles[handle] : null;
                float specificVolume = info != null ? info.Volume : 0.01f;
                specificVolume *= volume * 0.01f;
                if (!Un4seen.Bass.Bass.BASS_ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_VOL, specificVolume))
                {
                    error = true;
                }
                if (info != null && info.LinkedChannels != null) foreach (int secondChannel in info.LinkedChannels)
                        if (secondChannel != 0 && !Un4seen.Bass.Bass.BASS_ChannelSetAttribute(secondChannel, BASSAttribute.BASS_ATTRIB_VOL, specificVolume))
                        {
                            error = true;
                        }
            }
            if (error)
            {
                ErrorHandling.BassErrorOccurred(-1, StringResources.SetVolumeError);
            }
        }

        public void SetRepeatFile(int handle, bool repeat)
        {
            if (!m_NotLoops.ContainsKey(handle))
                return; // file is looped anyway
            if (repeat)
            {
                Bass.BASS_ChannelFlags(handle, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                Bass.BASS_ChannelRemoveSync(handle, m_NotLoops[handle]);
            }
            else
            {
                Bass.BASS_ChannelFlags(handle, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
                int sync = Bass.BASS_ChannelSetSync(handle, BASSSync.BASS_SYNC_END, 0, m_EndSync, IntPtr.Zero);
                m_NotLoops[handle] = sync;
            }
        }

        private System.Collections.Generic.Dictionary<int, ISoundFile> m_Loops = new Dictionary<int, ISoundFile>();
        private System.Collections.Generic.Dictionary<int, int> m_NotLoops = new Dictionary<int, int>(); // channel to sync

        private bool SetStartVolume(ISoundFile file, int fadeInTime, int channel, RunningFileInfo info)
        {
            float volume = file.Volume / 100.0f;
            float specificVolume = 1.0f;
            if (file.Effects != null)
            {
                volume = DetermineVolume(file.Effects, volume, out specificVolume);
            }
            info.Volume = specificVolume;
            if ((file.Effects != null && file.Effects.FadeInTime != 0) || fadeInTime > 0)
            {

                if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, 0.0f))
                {
                    ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                    return false;
                }
                if (info.LinkedChannels != null) foreach (int secondChannel in info.LinkedChannels)
                        if (secondChannel != 0 && !Bass.BASS_ChannelSetAttribute(secondChannel, BASSAttribute.BASS_ATTRIB_VOL, 0.0f))
                        {
                            ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                            return false;
                        }
                int maxFadeInTime = file.Effects != null ? Math.Max(file.Effects.FadeInTime, fadeInTime) : fadeInTime;
                if (!Bass.BASS_ChannelSlideAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, volume, maxFadeInTime))
                {
                    ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                    return false;
                }
                if (info.LinkedChannels != null) foreach (int secondChannel in info.LinkedChannels)
                        if (secondChannel != 0 && !Bass.BASS_ChannelSlideAttribute(secondChannel, BASSAttribute.BASS_ATTRIB_VOL, volume, maxFadeInTime))
                        {
                            ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                            return false;
                        }
            }
            else
            {
                if (!Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, volume))
                {
                    ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                    return false;
                }
                if (info.LinkedChannels != null) foreach (int secondChannel in info.LinkedChannels)
                        if (secondChannel != 0 && !Bass.BASS_ChannelSetAttribute(secondChannel, BASSAttribute.BASS_ATTRIB_VOL, volume))
                        {
                            ErrorHandling.BassErrorOccurred(file.Id, StringResources.SetVolumeError);
                            return false;
                        }
            }
            return true;
        }

        public FilePlayer()
        {
            m_EndSync = new SYNCPROC(EndSync);
            m_EndSync2 = new SYNCPROC(EndSync2);
            m_FadeOutSync = new SYNCPROC(FadeOutSync);
            m_LoopSync = new SYNCPROC(LoopSync);
            m_StopSync = new SYNCPROC(StopSync);
            m_RunningFiles = new Dictionary<int, RunningFileInfo>();
#if MONO
            m_GCHandles = new Dictionary<int, System.Runtime.InteropServices.GCHandle>();
#endif
        }

        public void Dispose()
        {
        }

        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            if (m_NotLoops.ContainsKey(user.ToInt32()))
                m_NotLoops.Remove(user.ToInt32());
            FileFinished(channel, true);
        }

        private void EndSync2(int handle, int channel, int data, IntPtr user)
        {
            m_Loops.Remove(user.ToInt32());
        }

        private void StopSync(int handle, int channel, int data, IntPtr user)
        {
            if (user.ToInt32() == channel)
            {
                if (data == 2) // BASS_SLIDE_VOL
                {
                    float val = 1.0f;
                    if (Bass.BASS_ChannelGetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, ref val))
                    {
                        if (val < 0.001)
                        {
                            Bass.BASS_ChannelStop(channel);
                            FileFinished(channel, true);
                        }
                    }
                }
            }
        }

        private void FadeOutSync(int handle, int channel, int data, IntPtr user)
        {
            FadeOut(channel, user.ToInt32());
            bool crossFade = false;
            lock (m_Mutex)
            {
                if (m_RunningFiles.ContainsKey(channel))
                {
                    var info = m_RunningFiles[channel];
                    crossFade = info.CrossFade;
                }
            }
            if (crossFade)
            {
                CallEndAction(channel);
            }
        }

        private void LoopSync(int handle, int channel, int data, IntPtr user)
        {
            int id = user.ToInt32();
            ISoundFile file = null;
            RunningFileInfo info = null;
            lock (m_Mutex)
            {
                file = m_Loops[id];
                info = m_RunningFiles.ContainsKey(channel) ? m_RunningFiles[channel] : null;
            }
            if (info != null)
            {
                SetStartVolume(file, 0, channel, info);
            }
        }

        private void FadeOut(int channel, int time)
        {
            Bass.BASS_ChannelSlideAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, 0.0f, time);
            RunningFileInfo info = null;
            lock (m_Mutex)
            {
                info = m_RunningFiles.ContainsKey(channel) ? m_RunningFiles[channel] : null;
            }
            if (info != null)
            {
                if (info.LinkedChannels != null) foreach (int secondChannel in info.LinkedChannels)
                    {
                        Bass.BASS_ChannelSlideAttribute(secondChannel, BASSAttribute.BASS_ATTRIB_VOL, 0.0f, time);
                    }
            }
        }

        private void CallEndAction(int channel)
        {
            Action endAction = null;
            lock (m_Mutex)
            {
                if (m_RunningFiles.ContainsKey(channel))
                {
                    var info = m_RunningFiles[channel];
                    endAction = info.EndAction;
                    info.EndAction = null; // call only once
                }
            }
            if (endAction != null)
            {
                endAction();
            }
        }

        private void FileFinished(int channel, bool deleteMemory)
        {
            Action endAction = null;
            lock (m_Mutex)
            {
                if (m_RunningFiles.ContainsKey(channel))
                {
                    var info = m_RunningFiles[channel];
                    if (info.EndAction != null)
                    {
                        endAction = info.EndAction;
                    }
                    m_RunningFiles.Remove(channel);
                }
            }

            if (BassStreamer.Instance.IsStreaming)
                BassStreamer.Instance.RemoveChannel(channel);

            if (endAction != null)
            {
                endAction();
            }
#if MONO
            if (deleteMemory)
            {
				lock (m_Mutex)
				{
	                if (m_GCHandles.ContainsKey(channel))
	                {
	                    m_GCHandles[channel].Free();
	                    m_GCHandles.Remove(channel);
	                }
				}
            }
#endif
        }

        private SYNCPROC m_EndSync;
        private SYNCPROC m_EndSync2;
        private SYNCPROC m_FadeOutSync;
        private SYNCPROC m_LoopSync;
        private SYNCPROC m_StopSync;

        private class RunningFileInfo
        {
            public Action EndAction { get; set; }
            public float Volume { get; set; }
            public List<int> LinkedChannels { get; set; }
            public bool CrossFade { get; set; }
        }

        private Dictionary<int, RunningFileInfo> m_RunningFiles;
#if MONO
        private Dictionary<int, System.Runtime.InteropServices.GCHandle> m_GCHandles { get; set; }
#endif
        private Object m_Mutex = new Int16();

    }
}
