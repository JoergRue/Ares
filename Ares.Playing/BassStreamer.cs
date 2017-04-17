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

namespace Ares.Playing
{
    class BassStreamer : IStreamer
    {

		#if MONO
		#if !ANDROID
		private String FindExecutableDirectory(String executableName)
		{
			try
			{
				System.Diagnostics.Process whichProcess = new System.Diagnostics.Process();
				whichProcess.StartInfo.FileName = "which";
				whichProcess.StartInfo.Arguments = executableName;
				whichProcess.StartInfo.CreateNoWindow = true;
				whichProcess.StartInfo.RedirectStandardOutput = true;
				whichProcess.StartInfo.UseShellExecute = false;
				whichProcess.Start();
				String path = whichProcess.StandardOutput.ReadToEnd().Trim();
				whichProcess.WaitForExit();
				if (path.EndsWith(executableName))
				{
					ErrorHandling.AddMessage(MessageType.Debug, "Found " + executableName + " in " + path);
					return path.Substring(0, path.Length - executableName.Length);
				}
				else
				{
					ErrorHandling.AddMessage(MessageType.Debug, "which " + executableName + " returned " + path);
					return String.Empty;
				}
			}
			catch (Exception ex)
			{
				ErrorHandling.AddMessage(MessageType.Error, ex.Message);
				return String.Empty;
			}
		}
		#endif
		#endif

        public void BeginStreaming(StreamingParameters parameters)
        {
#if !ANDROID
            ErrorSent = false;
            if (IsStreaming)
                EndStreaming();

            m_MixerChannel = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Mixer_StreamCreate(44100, 2, Un4seen.Bass.BASSFlag.BASS_MIXER_RESUME | Un4seen.Bass.BASSFlag.BASS_MIXER_NONSTOP);
            if (m_MixerChannel == 0)
            {
                HandleBassError();
                return;
            }
            Un4seen.Bass.Misc.IBaseEncoder encoder = null;
#if !MONO
            switch (parameters.Encoding)
            {
                case StreamEncoding.Wav:
                    encoder = new Un4seen.Bass.Misc.EncoderWAV(m_MixerChannel);
                    break;
                case StreamEncoding.Wma:
                    Un4seen.Bass.Misc.EncoderWMA wmaEnc = new Un4seen.Bass.Misc.EncoderWMA(m_MixerChannel);
                    wmaEnc.WMA_Bitrate = (int)parameters.Bitrate;
                    encoder = wmaEnc;
                    break;
                case StreamEncoding.Ogg:
                    Un4seen.Bass.Misc.EncoderOGG oggEnc = new Un4seen.Bass.Misc.EncoderOGG(m_MixerChannel);
                    oggEnc.OGG_Bitrate = (int)parameters.Bitrate;
                    oggEnc.OGG_TargetSampleRate = (int)Un4seen.Bass.Misc.EncoderOGG.SAMPLERATE.Hz_44100;
                    oggEnc.OGG_UseQualityMode = false; //oggEnc.OGG_Bitrate == 128;
                    oggEnc.OGG_MaxBitrate = oggEnc.OGG_Bitrate;
                    oggEnc.OGG_MinBitrate = oggEnc.OGG_Bitrate;
                    encoder = oggEnc;
                    break;
                case StreamEncoding.Lame:
                    Un4seen.Bass.Misc.EncoderLAME lameEnc = new Un4seen.Bass.Misc.EncoderLAME(m_MixerChannel);
                    lameEnc.LAME_Bitrate = (int)parameters.Bitrate;
                    lameEnc.LAME_Mode = Un4seen.Bass.Misc.EncoderLAME.LAMEMode.Stereo;
                    lameEnc.LAME_TargetSampleRate = (int)Un4seen.Bass.Misc.EncoderLAME.SAMPLERATE.Hz_44100;
                    lameEnc.LAME_Quality = Un4seen.Bass.Misc.EncoderLAME.LAMEQuality.Quality;
                    encoder = lameEnc;
                    break;
                case StreamEncoding.Opus:
                    Un4seen.Bass.Misc.EncoderOPUS opusEnc = new Un4seen.Bass.Misc.EncoderOPUS(m_MixerChannel);
                    opusEnc.OPUS_Bitrate = (int)parameters.Bitrate;
                    encoder = opusEnc;
                    break;
                default:
                    Un4seen.Bass.Bass.BASS_StreamFree(m_MixerChannel);
                    m_MixerChannel = 0;
                    return;
            }
#else
			String encoderName = String.Empty;
			Un4seen.Bass.Misc.EncoderCMDLN theEnc = null;
			switch (parameters.Encoding) {
			case StreamEncoding.Lame: {
					encoderName = "lame";
					theEnc = new Un4seen.Bass.Misc.EncoderMP3 (m_MixerChannel);
					theEnc.CMDLN_EncoderType = Un4seen.Bass.BASSChannelType.BASS_CTYPE_STREAM_MP3;
					theEnc.CMDLN_CBRString = "-r -h -b ${bps} - -";
					theEnc.CMDLN_VBRString = "-r -h -v - -";
					break;
				}
			case StreamEncoding.Ogg: {
					encoderName = "oggenc";
					theEnc = new Un4seen.Bass.Misc.EncoderCMDLN (m_MixerChannel);
					theEnc.CMDLN_EncoderType = Un4seen.Bass.BASSChannelType.BASS_CTYPE_STREAM_OGG;
					theEnc.CMDLN_CBRString = "-r -";
					theEnc.CMDLN_VBRString = "-r -";
					break;
				}
			case StreamEncoding.Opus: {
					encoderName = "opusenc";
					theEnc = new Un4seen.Bass.Misc.EncoderCMDLN (m_MixerChannel);
					theEnc.CMDLN_EncoderType = Un4seen.Bass.BASSChannelType.BASS_CTYPE_STREAM_OPUS;
					theEnc.CMDLN_CBRString = "--raw - -";
					theEnc.CMDLN_VBRString = "--raw - -";
					break;
				}
			default:
				Un4seen.Bass.Bass.BASS_StreamFree (m_MixerChannel);
				m_MixerChannel = 0;
				return;
			}
			theEnc.CMDLN_Bitrate = (int)parameters.Bitrate;
			theEnc.EncoderDirectory = FindExecutableDirectory (encoderName);
			theEnc.CMDLN_Executable = encoderName;
			theEnc.CMDLN_SupportsSTDOUT = true;
			if (!theEnc.EncoderExists) {
				Un4seen.Bass.Bass.BASS_StreamFree (m_MixerChannel);
				m_MixerChannel = 0;
				ErrorHandling.AddMessage (MessageType.Error, String.Format (StringResources.EncoderNotFound, encoderName));
				return;
			}
			encoder = theEnc;
#endif
			encoder.InputFile = null;
            encoder.OutputFile = null;
            Un4seen.Bass.Misc.IStreamingServer server = null;
            try
            {
                switch (parameters.Streamer)
                {
                    case StreamerType.Icecast:
                        {
                            Un4seen.Bass.Misc.ICEcast iceCast = new Un4seen.Bass.Misc.ICEcast(encoder, true);
                            iceCast.Username = parameters.Username;
                            iceCast.Password = parameters.Password;
                            iceCast.ServerAddress = parameters.ServerAddress;
                            iceCast.ServerPort = parameters.ServerPort;
                            iceCast.StreamName = parameters.StreamName;
                            iceCast.PublicFlag = false;
                            iceCast.MountPoint = parameters.Encoding == StreamEncoding.Ogg ? "/" + parameters.StreamName + ".ogg" :
                                (parameters.Encoding == StreamEncoding.Opus ? "/" + parameters.StreamName + ".opus" : "/" + parameters.StreamName);
                            server = iceCast;
                            break;
                        }
                    case StreamerType.Shoutcast:
                        {
                            Un4seen.Bass.Misc.SHOUTcast shoutCast = new Un4seen.Bass.Misc.SHOUTcast(encoder, true);
#if !MEDIA_PORTAL
                            shoutCast.Username = parameters.Username;
#endif
                            shoutCast.Password = parameters.Password;
                            shoutCast.ServerAddress = parameters.ServerAddress;
                            shoutCast.ServerPort = parameters.ServerPort;
                            shoutCast.StationName = parameters.StreamName;
                            shoutCast.PublicFlag = false;
                            server = shoutCast;
                            break;
                        }
                    default:
                        Un4seen.Bass.Bass.BASS_StreamFree(m_MixerChannel);
                        m_MixerChannel = 0;
                        return;
                }
                m_BroadCast = new Un4seen.Bass.Misc.BroadCast(server);
                m_BroadCast.AutoReconnect = true;
                m_BroadCast.Notification += new Un4seen.Bass.Misc.BroadCastEventHandler(m_BroadCast_Notification);
                m_BroadCast.AutoConnect();
                if (!Un4seen.Bass.Bass.BASS_ChannelPlay(m_MixerChannel, false))
                {
                    HandleBassError();
                }
                else
                {
                    IsStreaming = true;
                }
            }
            catch (ArgumentException ex)
            {
                ErrorHandling.ErrorOccurred(-1, String.Format(StringResources.StreamingError, ex.Message));
            }
#endif
        }

#if !ANDROID
        void m_BroadCast_Notification(object sender, Un4seen.Bass.Misc.BroadCastEventArgs e)
        {
            lock (m_SyncObject)
            {
                if (m_BroadCast == null)
                    return;
                if (e.EventType == Un4seen.Bass.Misc.BroadCastEventType.Connected)
                {
                    ErrorHandling.AddMessage(MessageType.Info, "Connected to Icecast Server");
                }
                else if (e.EventType == Un4seen.Bass.Misc.BroadCastEventType.ConnectionError)
                {
                    if (!ErrorSent)
                    {
                        ErrorHandling.AddMessage(MessageType.Error, "Could not connect to Icecast Server");
                        ErrorSent = true;
                    }
                    else
                    {
                        ErrorHandling.AddMessage(MessageType.Debug, "Could not connect to Icecast Server");
                    }
                }
                else if (e.EventType == Un4seen.Bass.Misc.BroadCastEventType.ConnectionLost)
                {
                    if (!ErrorSent)
                    {
                        ErrorHandling.AddMessage(MessageType.Info, "Connection to Icecast Server lost");
                    }
                    else
                    {
                        ErrorHandling.AddMessage(MessageType.Debug, "Connection to Icecast Server lost");
                        ErrorSent = true;
                    }
					ErrorHandling.AddMessage(MessageType.Debug, m_BroadCast.Server.LastErrorMessage);
                }
                else if (e.EventType == Un4seen.Bass.Misc.BroadCastEventType.Disconnected)
                {
                    ErrorHandling.AddMessage(MessageType.Info, "Disconnected from Icecast Server");
                }
            }
        }
#endif

        public void EndStreaming()
        {
            if (!IsStreaming)
                return;
	#if !ANDROID
            lock (m_SyncObject)
            {
                if (m_BroadCast != null)
				{
                    m_BroadCast.Disconnect();
				}
                m_BroadCast = null;
            }
	#endif
            if (m_MixerChannel != 0)
                Un4seen.Bass.Bass.BASS_StreamFree(m_MixerChannel);
            m_MixerChannel = 0;
            IsStreaming = false;
            ErrorSent = false;
        }

        public bool IsStreaming { get; set; }

        private bool ErrorSent { get; set; }
        
        public bool AddChannel(int channel)
        {
            if (!IsStreaming)
                return false;
            bool result = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Mixer_StreamAddChannel(m_MixerChannel, channel, Un4seen.Bass.BASSFlag.BASS_MIXER_DOWNMIX | Un4seen.Bass.BASSFlag.BASS_MIXER_NORAMPIN | Un4seen.Bass.BASSFlag.BASS_STREAM_AUTOFREE);
            if (!result)
                HandleBassError();
            return result;
        }

        private Object m_SyncObject = new object();

        public void RemoveChannel(int channel)
        {
            if (!IsStreaming)
                return;

            bool result = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Mixer_ChannelRemove(channel);
        }

        private void HandleBassError()
        {
            ErrorHandling.BassErrorOccurred(0, StringResources.StreamingError);
        }

        public static BassStreamer Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new BassStreamer();
                    /*
                    StreamingParameters streamingParams = new StreamingParameters();
                    streamingParams.StreamName = "Ares";
                    streamingParams.ServerPort = 8000;
                    streamingParams.Password = "ares";
                    streamingParams.ServerAddress = "192.168.2.104";
                    streamingParams.Username = "";
                    streamingParams.Streamer = StreamerType.Icecast;
                    streamingParams.Encoding = StreamEncoding.Ogg;
                    s_Instance.BeginStreaming(streamingParams);
                     */
                }

                return s_Instance;
            }
        }

        private int m_MixerChannel;
#if !ANDROID
        private Un4seen.Bass.Misc.BroadCast m_BroadCast;
#endif
        private static BassStreamer s_Instance;
    }
}