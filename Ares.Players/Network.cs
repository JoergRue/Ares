/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ares.Data;
using Ares.Playing;
using Ares.Settings;

namespace Ares.Players
{
    public interface INetworkClient
    {
        void KeyReceived(int key);
        void VolumeReceived(Ares.Playing.VolumeTarget target, int value);
        void ClientDataChanged(bool listenAgainAfterDisconnect);
        void ClientConnected();
        String GetProjectsDirectory();
        Ares.Settings.RecentFiles GetLastUsedProjects();
        void ProjectShallChange(String newProjectFile);
        Ares.Data.IProject GetCurrentProject();
        void PlayOtherMusic(Int32 elementId);
        void SwitchElement(Int32 elementId);
        void SetMusicRepeat(bool repeat);
        void SwitchTag(Int32 categoryId, Int32 tagId, bool tagIsActive);
        void DeactivateAllTags();
        void SetTagCategoryCombination(Data.TagCategoryCombination categoryCombination);
        void SetMusicTagsFading(Int32 fadeTime, bool onlyOnChange);
        void SetPlayMusicOnAllSpeakers(bool onAllSpeakers);
        void SetFadingOnPreviousNext(int option, int fadeTime);
        void ChangeMode(String title);
    }

    public interface IUDPBroadcast
    {
        void InitConnectionData();
        void StartUdpBroadcast();
        bool SendUdpPacket();
        void StopUdpBroadcast();
    }

    public interface INetwork
    {
        void ListenForClient();
        void StopListenForClient();

        bool ClientConnected { get; }
        String ClientName { get; }
        void DisconnectClient(bool listenAgain);
        void Shutdown();
        
        void ErrorOccurred(int elementId, string errorMessage);
        void InformClientOfPossibleTags(int languageId, Ares.Data.IProject project);
        void InformClientOfProject(Ares.Data.IProject newProject);
        void InformClientOfEverything(int overallVolume, int musicVolume, int soundVolume, Ares.Data.IMode mode, MusicInfo music,
            System.Collections.Generic.IList<Ares.Data.IModeElement> elements, Ares.Data.IProject project, Int32 musicListId, bool musicRepeat,
            int tagLanguageId, System.Collections.Generic.IList<int> activeTags, Data.TagCategoryCombination categoryCombination, int fadeTime, bool fadeOnlyOnChange,
            bool musicOnAllChannels, int fadeOnPreviousNextOption, int fadeOnPreviousNextTime);
        void InformClientOfVolume(Ares.Playing.VolumeTarget target, int value);
        void InformClientOfFading(int fadeTime, bool fadeOnlyOnChange);
        void InformClientOfImportProgress(int percent, String additionalInfo);
    }

    public interface INetworks : INetwork, IUDPBroadcast
    { }

    public class Networks : INetworks
    {
        private INetworkClient mClient;
        private INetwork[] mNetworks = new INetwork[2];

        public static readonly int PLAYER_VERSION = 4;

        public bool ClientConnected
        {
            get
            {
                foreach (INetwork network in mNetworks)
                    if (network != null && network.ClientConnected) return true;
                return false;
            }
        }

        public string ClientName
        {
            get
            {
                String name = "";
                foreach (INetwork network in mNetworks)
                {
                    if (network != null && network.ClientConnected)
                    {
                        String n = network.ClientName;
                        if (!String.IsNullOrEmpty(n))
                        {
                            if (name.Length > 0) name += ", ";
                            name += n;
                        }
                    }
                }
                return name;
            }
        }

        private void SetUseCustomIpNetwork(bool useIt)
        {
            if (mNetworks[0] != null && !useIt)
            {
                if (mNetworks[0].ClientConnected) mNetworks[0].DisconnectClient(false);
                mNetworks[0].Shutdown();
                mNetworks[0] = null;
            }
            if (mNetworks[0] == null && useIt)
            {
                mNetworks[0] = new CustomIpNetwork(mClient);
            }
        }

        private void SetUseWebNetwork(bool useIt)
        {
            if (mNetworks[1] != null && !useIt)
            {
                if (mNetworks[1].ClientConnected) mNetworks[1].DisconnectClient(false);
                mNetworks[1].Shutdown();
                mNetworks[1] = null;
            }
            if (mNetworks[1] == null && useIt)
            {
				#if !ANDROID
                mNetworks[1] = new Ares.Players.Web.WebNetwork(mClient);
				#endif
            }
        }

        public void ListenForClient()
        {
            foreach (INetwork network in mNetworks) if (network != null) network.ListenForClient();
        }

        public void StopListenForClient()
        {
            foreach (INetwork network in mNetworks) if (network != null) network.StopListenForClient();
        }

        public void DisconnectClient(bool listenAgain)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.DisconnectClient(listenAgain);
            if (listenAgain)
                StartUdpBroadcast();
        }

        public void Shutdown()
        {
            foreach (INetwork network in mNetworks) if (network != null) network.Shutdown();
        }

        public void ErrorOccurred(int elementId, string errorMessage)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.ErrorOccurred(elementId, errorMessage);
        }

        public void InformClientOfPossibleTags(int languageId, IProject project)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.InformClientOfPossibleTags(languageId, project);
        }

        public void InformClientOfProject(IProject newProject)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.InformClientOfProject(newProject);
        }

        public void InformClientOfEverything(int overallVolume, int musicVolume, int soundVolume, IMode mode, MusicInfo music, IList<IModeElement> elements, IProject project, int musicListId, bool musicRepeat, int tagLanguageId, IList<int> activeTags, TagCategoryCombination categoryCombination, int fadeTime, bool fadeOnlyOnChange, bool musicOnAllChannels, int fadeOnPreviousNextOption, int fadeOnPreviousNextTime)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.InformClientOfEverything(overallVolume, musicVolume, soundVolume, mode, music, elements, project, musicListId, musicRepeat, tagLanguageId, activeTags, categoryCombination, fadeTime, fadeOnlyOnChange, musicOnAllChannels, fadeOnPreviousNextOption, fadeOnPreviousNextTime);
        }

        public void InformClientOfVolume(VolumeTarget target, int value)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.InformClientOfVolume(target, value);
        }

        public void InformClientOfFading(int fadeTime, bool fadeOnlyOnChange)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.InformClientOfFading(fadeTime, fadeOnlyOnChange);
        }

        public void InformClientOfImportProgress(int percent, String additionalInfo)
        {
            foreach (INetwork network in mNetworks) if (network != null) network.InformClientOfImportProgress(percent, additionalInfo);
        }

        public Networks(INetworkClient client, bool useLegacy, bool useWeb)
        {
            mClient = client;
            SetUseCustomIpNetwork(useLegacy);
            SetUseWebNetwork(useWeb);
        }

        private Byte[] udpPacket;

        private String udpString;

        public void InitConnectionData()
        {
            int tcpPort = Settings.Settings.Instance.TcpPort;
            String ipAddress = Settings.Settings.Instance.IPAddress;
            StringBuilder str = new StringBuilder();
			#if !ANDROID
            String machineName = Dns.GetHostName();
			#else
			String machineName = Settings.Settings.Instance.PlayerName;
			#endif
            str.Append(machineName);
            str.Append("|");
            str.Append(tcpPort);
            if (!String.IsNullOrEmpty(ipAddress))
            {
                str.Append("|");
                System.Net.IPAddress address = null;
                if (System.Net.IPAddress.TryParse(ipAddress, out address) && address != null && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    int index = ipAddress.IndexOf('%');
                    if (index != -1)
                        ipAddress = ipAddress.Substring(0, index);
                    str.Append(ipAddress);
                }
                else
                {
                    str.Append(ipAddress);
                }
            }
            str.Append("|");
            str.Append(PLAYER_VERSION);
            str.Append("|");
            str.Append(Settings.Settings.Instance.UseLegacyNetwork ? "true" : "false");
            str.Append("|");
            str.Append(Settings.Settings.Instance.UseWebNetwork ? "true" : "false");
            str.Append("|");
            str.Append(Settings.Settings.Instance.WebTcpPort);
            udpString = str.ToString();
            udpPacket = System.Text.Encoding.UTF8.GetBytes(udpString);
        }

        private static int udpPacketCount = 0;

        public bool SendUdpPacket()
        {
            int port = Settings.Settings.Instance.UdpPort;
            if (udpClient == null)
            {
                return true;
            }
            try
            {
                ++udpPacketCount;
                if (udpPacketCount == 1)
                {
                    Messages.AddMessage(MessageType.Debug, String.Format(StringResources.UDPSending, udpString));
                }
                lock (syncObject)
                {
                    if (udpClient == null)
                        return true;
                    udpClient.Send(udpPacket, udpPacket.Length, new IPEndPoint(IPAddress.Parse("255.255.255.255"), port));
                }
                return true;
            }
            catch (SocketException e)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.UDPError + e.Message);
                return false;
            }
        }

        public void StartUdpBroadcast()
        {
            Messages.AddMessage(MessageType.Info, StringResources.StartBroadcast);
            if (udpClient != null)
                StopUdpBroadcast();
            lock (syncObject)
            {
                udpClient = new UdpClient();
                udpClient.EnableBroadcast = true;
                udpPacketCount = 0;
            }
        }

        public void StopUdpBroadcast()
        {
            Messages.AddMessage(MessageType.Info, StringResources.StopBroadcast);
            lock (syncObject)
            {
                if (udpClient == null)
                    return;
                udpClient.Close();
                udpClient = null;
            }
        }

        private UdpClient udpClient = null;

        private Object syncObject = new Int32();
    }

    public class CustomIpNetwork : INetwork, Ares.Playing.IProjectPlayingCallbacks
    {
        public CustomIpNetwork(INetworkClient client)
        {
            networkClient = client;
            ClientConnected = false;
            ClientName = String.Empty;
        }

        public void ListenForClient()
        {
            lock (syncObject)
            {
                if (forceShutdown)
                    return;
                continueListenForClients = true;
            }

            String ipAddress = Settings.Settings.Instance.IPAddress;
            tcpListenAddress = IPAddress.Parse(ipAddress);

            System.Threading.Thread listenThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenInThread));
            listenThread.Start();
        }

        public void StopListenForClient()
        {
            bool listenerRunning = false;
            lock (syncObject)
            {
                continueListenForClients = false;
                listenerRunning = listenerThreadRunning;
            }
            if (listenerRunning)
            {
                listenerThreadStoppedEvent.WaitOne(500);
            }
        }

        public void Shutdown()
        {
            bool listenerRunning = false;
            lock (syncObject)
            {
                continueListenForClients = false;
                forceShutdown = true;
                listenerRunning = listenerThreadRunning;
            }
            if (listenerRunning)
            {
                listenerThreadStoppedEvent.WaitOne(500);
            }
        }

        public void InformClientOfVolume(Ares.Playing.VolumeTarget target, int value)
        {
            InformVolume(target, value);
        }

        public void InformClientOfProject(Ares.Data.IProject newProject)
        {
            InformProjectModel(newProject);
        }

        public void InformClientOfMusicList(Int32 musicListId)
        {
            InformMusicList(musicListId);
        }

        public void InformClientOfPossibleTags(int languageId, Ares.Data.IProject project)
        {
            InformPossibleTags(languageId, project);
        }

        public void InformClientOfActiveTags(System.Collections.Generic.IList<int> activeTags)
        {
            InformActiveTags(activeTags);
        }

        public void InformClientOfTagCategoryCombination(Data.TagCategoryCombination categoryCombination)
        {
            InformCategoryCombinationChanged(categoryCombination);
        }

        public void InformClientOfFading(int fadeTime, bool fadeOnlyOnChange)
        {
            InformFading(fadeTime, fadeOnlyOnChange);
        }

        public void InformClientOfEverything(int overallVolume, int musicVolume, int soundVolume, Ares.Data.IMode mode, MusicInfo music,
            System.Collections.Generic.IList<Ares.Data.IModeElement> elements, Ares.Data.IProject project, Int32 musicListId, bool musicRepeat,
            int tagLanguageId, System.Collections.Generic.IList<int> activeTags, Data.TagCategoryCombination categoryCombination, int fadeTime, bool fadeOnlyOnChange,
            bool musicOnAllChannels, int fadeOnPreviousNextOption, int fadeOnPreviousNextTime)
        {
            InformProjectModel(project);
            InformPossibleTags(tagLanguageId, project);
            InformVolume(Playing.VolumeTarget.Both, overallVolume);
            InformVolume(Playing.VolumeTarget.Music, musicVolume);
            InformVolume(Playing.VolumeTarget.Sounds, soundVolume);
            InformMusicChange(music);
            InformModeChange(mode);
            InformAllElementsStopped();
            foreach (Ares.Data.IModeElement element in elements) {
                InformModeElementChange(element, true);
            }
            InformMusicList(musicListId);
            InformRepeatChanged(musicRepeat);
            InformMusicOnAllChannelsChanged(musicOnAllChannels);
            InformActiveTags(activeTags);
            InformCategoryCombinationChanged(categoryCombination);
            m_MusicTagsFadeOnlyOnChange = fadeOnlyOnChange;
            InformFading(fadeTime, fadeOnlyOnChange);
            InformPreviousNextFading(fadeOnPreviousNextOption, fadeOnPreviousNextTime);
        }

        public void InformClientOfImportProgress(int percent, String additionalInfo)
        {
            InformImportProgress(percent, additionalInfo);
        }

        public void ListenInThread()
        {
            bool goOn = true;
            lock (syncObject)
            {
                listenerThreadRunning = true;
            }
            TcpListener listener = new TcpListener(tcpListenAddress, Settings.Settings.Instance.TcpPort);
            try
            {
                listener.Start(1);
                while (goOn)
                {
                    if (listener.Pending())
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        if (InitConnection(client))
                        {
                            StopListenForClient();
                            break;
                        }
                    }
                    lock (syncObject)
                    {
                        goOn = continueListenForClients;
                    }
                    if (goOn) System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.ClientListenError + e.Message);
            }
            finally
            {
                listener.Stop();
                lock (syncObject)
                {
                    listenerThreadRunning = false;
                }
                listenerThreadStoppedEvent.Set();
            }
        }

        private static bool ReadFromStream(NetworkStream stream, Byte[] buffer, int size, int timeout)
        {
            int read = 0;
            int time = 0;
            while (read < size)
            {
                while (!stream.DataAvailable)
                {
                    if (time > timeout) return false;
                    time += 5;
                    System.Threading.Thread.Sleep(5);
                }
                buffer[read++] = (byte)stream.ReadByte();
            }
            return true;
        }

        private System.Timers.Timer m_WatchdogTimer;
        private System.Timers.Timer m_PingTimer;

        private bool InitConnection(TcpClient aClient)
        {
            Byte[] buffer = new Byte[1000];
            NetworkStream stream = aClient.GetStream();
            bool success = ReadFromStream(stream, buffer, 4, 2000);
            if (!success) return false;
            int length = 0;
            String s = System.Text.Encoding.UTF8.GetString(buffer, 0, 4);
            Messages.AddMessage(MessageType.Debug, StringResources.ClientLengthReceived + s);
            success = Int32.TryParse(s, out length);
            if (!success)
            {
                Messages.AddMessage(MessageType.Debug, StringResources.WrongClientLength);
                return false;
            }
            if (length > 1000)
            {
                Messages.AddMessage(MessageType.Debug, StringResources.LengthTooHigh);
                return false;
            }
            success = ReadFromStream(stream, buffer, length, 2000);
            if (!success)
            {
                Messages.AddMessage(MessageType.Debug, StringResources.NoClientID);
                return false;
            }
            ClientName = System.Text.Encoding.UTF8.GetString(buffer, 0, length);
            lock (syncObject)
            {
                client = aClient;
                Ares.Playing.PlayingModule.AddCallbacks(this);
                ClientConnected = true;
            }
            Messages.AddMessage(MessageType.Info, String.Format(StringResources.ClientConnected, ClientName));
            networkClient.ClientConnected();
            continueListenForCommands = true;
            System.Threading.Thread commandThread = new System.Threading.Thread(ListenForCommands);
            commandThread.Start();
            m_WatchdogTimer = new System.Timers.Timer(50000);
            m_WatchdogTimer.Elapsed += new System.Timers.ElapsedEventHandler(watchdogTimer_Elapsed);
            m_WatchdogTimer.Start();
            m_PingTimer = new System.Timers.Timer(10000);
            m_PingTimer.Elapsed += new System.Timers.ElapsedEventHandler(pingTimer_Elapsed);
            m_PingTimer.AutoReset = true;
            m_PingTimer.Start();
            return true;
        }

        void pingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendPing();
        }

        void watchdogTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Messages.AddMessage(MessageType.Warning, StringResources.PingTimeout);
            DoDisconnect(true);
        }

        public void DisconnectClient(bool listenAgain)
        {
            try
            {
                InformDisconnect();
                DoDisconnect(listenAgain);

            }
            catch (System.IO.IOException)
            {
                lock (syncObject)
                {
                    client = null;
                    ClientConnected = false;
                }
                if (listenAgain)
                {
                    ListenForClient();
                }
            }
        }

        private void DoDisconnect(bool listenAgain)
        {
            lock (syncObject)
            {
                if (m_WatchdogTimer != null)
                {
                    m_WatchdogTimer.Stop();
                    m_WatchdogTimer.Dispose();
                    m_WatchdogTimer = null;
                }
                if (m_PingTimer != null)
                {
                    m_PingTimer.Stop();
                    m_PingTimer.Dispose();
                    m_PingTimer = null;
                }
                continueListenForCommands = false;
                if (client != null)
                {
                    if (client.Client != null)
                    {
                        client.Client.Close();
                    }
                    client.Close();
                    client = null;
                }
                Ares.Playing.PlayingModule.RemoveCallbacks(this);
            }
            ClientConnected = false;
            Messages.AddMessage(MessageType.Info, StringResources.ClientDisconnected);
            networkClient.ClientDataChanged(listenAgain);
            if (listenAgain)
            {
                ListenForClient();
            }
        }

        private int[][] m_BytesToKeys;
        private Dictionary<int, byte[]> m_KeysToBytes;

        private void PrepareKeysMaps()
        {
            m_BytesToKeys = new int[4][];
            m_BytesToKeys[0] = null;

            m_BytesToKeys[1] = new int[13];
            m_BytesToKeys[1][0] = 0;
            m_BytesToKeys[1][1] = (int)Ares.Data.Keys.F1;
            m_BytesToKeys[1][2] = (int)Ares.Data.Keys.F2;
            m_BytesToKeys[1][3] = (int)Ares.Data.Keys.F3;
            m_BytesToKeys[1][4] = (int)Ares.Data.Keys.F4;
            m_BytesToKeys[1][5] = (int)Ares.Data.Keys.F5;
            m_BytesToKeys[1][6] = (int)Ares.Data.Keys.F6;
            m_BytesToKeys[1][7] = (int)Ares.Data.Keys.F7;
            m_BytesToKeys[1][8] = (int)Ares.Data.Keys.F8;
            m_BytesToKeys[1][9] = (int)Ares.Data.Keys.F9;
            m_BytesToKeys[1][10] = (int)Ares.Data.Keys.F10;
            m_BytesToKeys[1][11] = (int)Ares.Data.Keys.F11;
            m_BytesToKeys[1][12] = (int)Ares.Data.Keys.F12;

            m_BytesToKeys[2] = new int[10];
            m_BytesToKeys[2][0] = (int)Ares.Data.Keys.NumPad0;
            m_BytesToKeys[2][1] = (int)Ares.Data.Keys.NumPad1;
            m_BytesToKeys[2][2] = (int)Ares.Data.Keys.NumPad2;
            m_BytesToKeys[2][3] = (int)Ares.Data.Keys.NumPad3;
            m_BytesToKeys[2][4] = (int)Ares.Data.Keys.NumPad4;
            m_BytesToKeys[2][5] = (int)Ares.Data.Keys.NumPad5;
            m_BytesToKeys[2][6] = (int)Ares.Data.Keys.NumPad6;
            m_BytesToKeys[2][7] = (int)Ares.Data.Keys.NumPad7;
            m_BytesToKeys[2][8] = (int)Ares.Data.Keys.NumPad8;
            m_BytesToKeys[2][9] = (int)Ares.Data.Keys.NumPad9;

            m_BytesToKeys[3] = new int[22];
            m_BytesToKeys[3][0] = (int)Ares.Data.Keys.Insert;
            m_BytesToKeys[3][1] = (int)Ares.Data.Keys.Delete;
            m_BytesToKeys[3][2] = (int)Ares.Data.Keys.Home;
            m_BytesToKeys[3][3] = (int)Ares.Data.Keys.End;
            m_BytesToKeys[3][4] = (int)Ares.Data.Keys.PageUp;
            m_BytesToKeys[3][5] = (int)Ares.Data.Keys.PageDown;
            m_BytesToKeys[3][6] = (int)Ares.Data.Keys.Left;
            m_BytesToKeys[3][7] = (int)Ares.Data.Keys.Right;
            m_BytesToKeys[3][8] = (int)Ares.Data.Keys.Up;
            m_BytesToKeys[3][9] = (int)Ares.Data.Keys.Down;
            m_BytesToKeys[3][10] = (int)Ares.Data.Keys.Space;
            m_BytesToKeys[3][11] = (int)Ares.Data.Keys.Return;
            m_BytesToKeys[3][12] = (int)Ares.Data.Keys.Escape;
            m_BytesToKeys[3][13] = (int)Ares.Data.Keys.Tab;
            m_BytesToKeys[3][14] = (int)Ares.Data.Keys.Back;
            m_BytesToKeys[3][15] = (int)Ares.Data.Keys.OemPeriod;
            m_BytesToKeys[3][16] = (int)Ares.Data.Keys.OemSemicolon;
            m_BytesToKeys[3][17] = (int)Ares.Data.Keys.Oem4;
            m_BytesToKeys[3][18] = (int)Ares.Data.Keys.OemComma;
            m_BytesToKeys[3][19] = (int)Ares.Data.Keys.Oem2;
            m_BytesToKeys[3][20] = (int)Ares.Data.Keys.OemMinus;
            m_BytesToKeys[3][21] = (int)Ares.Data.Keys.Oem3;

            m_KeysToBytes = new Dictionary<int, byte[]>();
            for (byte i = 1; i < m_BytesToKeys.Length; ++i)
            {
                for (byte j = 0; j < m_BytesToKeys[i].Length; ++j)
                {
                    byte[] bytes = new byte[2];
                    bytes[0] = i;
                    bytes[1] = j;
                    m_KeysToBytes[m_BytesToKeys[i][j]] = bytes;
                }
            }
        }

        private int MapKeyCodeToKey(Byte[] keyCode)
        {
            try
            {
                if (keyCode[0] == 0)
                {
                    String s = System.Text.Encoding.ASCII.GetString(keyCode, 1, 1);
                    return (int)s[0];
                }
                if (m_BytesToKeys == null)
                {
                    PrepareKeysMaps();
                }
                if (keyCode[0] < m_BytesToKeys.Length)
                {
                    int[] subArray = m_BytesToKeys[keyCode[0]];
                    if (keyCode[1] < subArray.Length)
                    {
                        return subArray[keyCode[1]];
                    }
                }
                return (int)Ares.Data.Keys.Escape;
            }
            catch (Exception e)
            {
                Messages.AddMessage(MessageType.Error, String.Format(StringResources.InvalidKeyCode, e.Message));
                return (int)Ares.Data.Keys.Escape;
            }
        }

        private byte[] MapKeyToKeyCode(int key)
        {
            if (key == 0)
            {
                return new byte[] { 0, 0 };
            }
            if (m_KeysToBytes == null)
            {
                PrepareKeysMaps();
            }
            if (m_KeysToBytes.ContainsKey(key))
            {
                return m_KeysToBytes[key];
            }
            else
            {
                char[] chars = new char[1];
                chars[0] = (char)key;
                byte[] bytes = new byte[2];
                bytes[0] = 0;
                bytes[1] = System.Text.Encoding.ASCII.GetBytes(chars)[0];
                return bytes;
            }
        }

        private bool ReadInt32(out Int32 value)
        {
            Byte[] data = new Byte[4];
            lock (syncObject)
            {
                bool success = client != null && client.Connected && ReadFromStream(client.GetStream(), data, 4, 500);
                if (success)
                {
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data);
                    value = BitConverter.ToInt32(data, 0);
                }
                else
                {
                    value = -1;
                }
                return success;
            }
        }

        private void ListenForCommands()
        {
            try
            {
                bool goOn = true;
                bool disconnect = false;
                while (goOn && client != null)
                {
                    int command = -1;
                    lock (syncObject)
                    {
                        if (client != null && client.Connected && client.GetStream().DataAvailable)
                        {
                            client.GetStream().ReadTimeout = 100;
                            try
                            {
                                command = client.GetStream().ReadByte();
                            }
                            catch (System.IO.IOException)
                            {
                                command = -1;
                            }
                        }
                    }
                    if (command != -1)
                    {
                        Messages.AddMessage(MessageType.Debug, String.Format(StringResources.CommandReceived, command));
                    }
                    if (command == 0)
                    {
                        Byte[] keyCode = new Byte[2];
                        bool success = false;
                        lock (syncObject)
                        {
                            success = client != null && client.Connected && ReadFromStream(client.GetStream(), keyCode, 2, 500);
                        }
                        if (success)
                        {
                            Messages.AddMessage(MessageType.Debug, String.Format(StringResources.KeyReceived, keyCode[0], keyCode[1]));
                            networkClient.KeyReceived(MapKeyCodeToKey(keyCode));
                        }
                    }
                    else if (command == 1)
                    {
                        Messages.AddMessage(MessageType.Debug, StringResources.DisconnectingClient);
                        disconnect = true;
                        break;
                    }
                    else if (command == 2)
                    {
                        Byte[] data = new Byte[2];
                        bool success = false;
                        lock (syncObject)
                        {
                            success = client != null && client.Connected && ReadFromStream(client.GetStream(), data, 2, 500);
                        }
                        if (success)
                        {
                            Ares.Playing.VolumeTarget target = (Ares.Playing.VolumeTarget)data[0];
                            int vol = (int)data[1];
                            Messages.AddMessage(MessageType.Debug, String.Format(StringResources.VolumeCommandReceived, target, vol));
                            if (vol < 0 || vol > 100)
                            {
                                Messages.AddMessage(MessageType.Warning, StringResources.VolumeOutOfRange);
                            }
                            else
                            {
                                networkClient.VolumeReceived(target, vol);
                            }
                        }
                    }
                    else if (command == 5)
                    {
                        // this is ping
                        Messages.AddMessage(MessageType.Debug, StringResources.PingReceived);
                        if (m_WatchdogTimer != null)
                        {
                            m_WatchdogTimer.Stop();
                            m_WatchdogTimer.Interval = 50000;
                            m_WatchdogTimer.Start();
                        }
                    }
                    else if (command == 6)
                    {
                        Byte[] data = new Byte[2];
                        String projectName = String.Empty;
                        bool success = false;
                        lock (syncObject)
                        {
                            success = client != null && client.Connected && ReadFromStream(client.GetStream(), data, 2, 500);
                            if (success)
                            {
                                int length = data[0] * (1 << 8);
                                length += data[1];
                                Byte[] bytes = new Byte[length];
                                success = client != null && client.Connected && ReadFromStream(client.GetStream(), bytes, length, 500);
                                if (success)
                                {
                                    projectName = System.Text.Encoding.UTF8.GetString(bytes);
                                }
                            }
                        }
                        if (success && !String.IsNullOrEmpty(projectName) && networkClient != null)
                        {
                            networkClient.ProjectShallChange(projectName);
                        }
                    }
                    else if (command == 7)
                    {
                        Int32 newMusicId = -1;
                        bool success = ReadInt32(out newMusicId);
                        if (success && newMusicId != -1)
                        {
                            networkClient.PlayOtherMusic(newMusicId);
                        }
                    }
                    else if (command == 8)
                    {
                        Int32 elementId = -1;
                        bool success = ReadInt32(out elementId);
                        if (success && elementId != -1)
                        {
                            networkClient.SwitchElement(elementId);
                        }
                    }
                    else if (command == 9)
                    {
                        Int32 repeatValue = 0;
                        bool success = ReadInt32(out repeatValue);
                        if (success)
                        {
                            networkClient.SetMusicRepeat(repeatValue == 1);
                        }
                    }
                    else if (command == 10)
                    {
                        Int32 categoryId = -1;
                        Int32 tagId = -1;
                        Int32 onOff = -1;
                        bool success = ReadInt32(out categoryId);
                        if (success)
                        {
                            success = ReadInt32(out tagId);
                        }
                        if (success)
                        {
                            success = ReadInt32(out onOff);
                        }
                        if (success)
                        {
                            networkClient.SwitchTag(categoryId, tagId, onOff == 1);
                        }
                    }
                    else if (command == 11)
                    {
                        networkClient.DeactivateAllTags();
                    }
                    else if (command == 12)
                    {
                        int combination = -1;
                        bool success = ReadInt32(out combination);
                        if (success)
                        {
                            if (combination < (int)Data.TagCategoryCombination.UseAnyTag || combination > (int)Data.TagCategoryCombination.UseAllTags)
                                combination = 0;
                            networkClient.SetTagCategoryCombination((Data.TagCategoryCombination)combination);
                        }
                    }
                    else if (command == 13)
                    {
                        InformPossibleProjects();
                    }
                    else if (command == 14)
                    {
                        Int32 fadeTime = 0;
                        bool success = ReadInt32(out fadeTime);
                        Int32 onlyOnChange = 0;
                        success = ReadInt32(out onlyOnChange) && success;
                        if (success)
                        {
                            networkClient.SetMusicTagsFading(fadeTime, onlyOnChange == 1);
                        }
                    }
                    else if (command == 15)
                    {
                        Int32 onAllSpeakers = -1;
                        bool success = ReadInt32(out onAllSpeakers);
                        if (success)
                        {
                            networkClient.SetPlayMusicOnAllSpeakers(onAllSpeakers == 1);
                        }
                    }
                    else if (command == 16)
                    {
                        Int32 fadingOption = 0;
                        bool success = ReadInt32(out fadingOption);
                        Int32 fadingTime = 0;
                        if (success)
                            success = ReadInt32(out fadingTime);
                        if (success)
                        {
                            if (fadingOption < 0 || fadingOption > 2)
                                fadingOption = 0;
                            if (fadingTime < 0 || fadingTime > 30000)
                                fadingTime = 0;
                            networkClient.SetFadingOnPreviousNext(fadingOption, fadingTime);
                        }
                    }
                    lock (syncObject)
                    {
                        goOn = continueListenForCommands;
                    }
                    if (goOn)
                    {
                        System.Threading.Thread.Sleep(5);
                    }
                }
                if (disconnect)
                {
                    DoDisconnect(true);
                }
            }
            catch (Exception e)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.KeyListenError + e.Message);
                DoDisconnect(true);
            }
        }

        private void WriteToClient(byte[] package, int offset, int length)
        {
            lock (syncObject)
            {
                try
                {
                    if (client != null && client.Connected)
                    {
                        client.GetStream().Write(package, offset, length);
                    }
                }
                catch (InvalidOperationException)
                {
                    Messages.AddMessage(MessageType.Error, StringResources.ClientSendError);
                    DoDisconnect(true);
                }
            }
        }

        private void InformModeChange(Ares.Data.IMode mode)
        {
            if (ClientConnected)
            {
                String title = mode != null ? mode.Title : String.Empty;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(title);
                byte[] package = new byte[3 + bytes.Length];
                package[0] = 0;
                package[1] = (byte)(bytes.Length / (1 << 8));
                package[2] = (byte)(bytes.Length % (1 << 8));
                Array.Copy(bytes, 0, package, 3, bytes.Length);
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformModeElementChange(Ares.Data.IModeElement element, bool isActive)
        {
            if (ClientConnected)
            {
                byte[] bytes = new byte[4];
                bytes[0] = 1;
                bytes[1] = (byte)(element.Id / (1 << 8));
                bytes[2] = (byte)(element.Id % (1 << 8));
                bytes[3] = (byte)(isActive ? 1 : 0);
                WriteToClient(bytes, 0, bytes.Length);
            }
        }

        private void InformMusicChange(MusicInfo music)
        {
            if (ClientConnected)
            {
                byte[] bytes1 = System.Text.Encoding.UTF8.GetBytes(music.LongTitle);
                byte[] bytes2 = System.Text.Encoding.UTF8.GetBytes(music.ShortTitle);
                byte[] package = new byte[5 + bytes1.Length + bytes2.Length];
                package[0] = 2;
                package[1] = (byte)(bytes1.Length / (1 << 8));
                package[2] = (byte)(bytes1.Length % (1 << 8));
                Array.Copy(bytes1, 0, package, 3, bytes1.Length);
                package[3 + bytes1.Length] = (byte)(bytes2.Length / (1 << 8));
                package[3 + bytes1.Length + 1] = (byte)(bytes2.Length % (1 << 8));
                Array.Copy(bytes2, 0, package, 3 + bytes1.Length + 2, bytes2.Length);
                WriteToClient(package, 0, package.Length);
            }
        }

        private void SendStringAndInt32(byte commandId, byte subCommandId, String s, Int32 i)
        {
            byte[] sa = System.Text.Encoding.UTF8.GetBytes(s);
            byte[] ia = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ia);
            byte[] elementPackage = new byte[3 + ia.Length + 2 + sa.Length];
            elementPackage[0] = commandId;
            elementPackage[1] = subCommandId;
            elementPackage[2] = 0;
            Array.Copy(ia, 0, elementPackage, 3, ia.Length);
            elementPackage[3 + ia.Length] = (byte)(sa.Length / (1 << 8));
            elementPackage[3 + ia.Length + 1] = (byte)(sa.Length % (1 << 8));
            Array.Copy(sa, 0, elementPackage, 3 + ia.Length + 2, sa.Length);
            WriteToClient(elementPackage, 0, elementPackage.Length);
        }

        private void InformMusicList(Int32 musicListId)
        {
            if (ClientConnected)
            {
                // start packet
                byte[] package = new byte[3];
                package[0] = 8;
                package[1] = 0;
                package[2] = 0;
                WriteToClient(package, 0, package.Length);
                Ares.Data.IElement element = musicListId != -1 ? Ares.Data.DataModule.ElementRepository.GetElement(musicListId) : null;
                Ares.Data.IMusicList musicList = (element != null && element is Ares.Data.IMusicList) ? element as Ares.Data.IMusicList : null;
                // one packet each for each music title
                if (musicList != null)
                {
                    foreach (Ares.Data.IFileElement fileElement in musicList.GetFileElements())
                    {
                        SendStringAndInt32(8, 1, fileElement.Title, fileElement.Id);
                    }
                }
                // end packet
                package[1] = 2;
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformPossibleTags(int languageId, Ares.Data.IProject project)
        {
            if (ClientConnected)
            {
                try
                {
                    var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId);
                    var categories = dbRead.GetAllCategories();
                    HashSet<int> hiddenCategories = project != null ? project.GetHiddenTagCategories() : new HashSet<int>();
                    HashSet<int> hiddenTags = project != null ? project.GetHiddenTags() : new HashSet<int>();
                    // start packet for categories
                    byte[] package = new byte[3];
                    package[0] = 11;
                    package[1] = 0;
                    package[2] = 0;
                    WriteToClient(package, 0, package.Length);
                    foreach (var category in categories)
                    {
                        if (hiddenCategories.Contains(category.Id))
                            continue;
                        // category packet
                        SendStringAndInt32(11, 1, category.Name, category.Id);
                        // all tags for the category
                        var tags = dbRead.GetAllTags(category.Id);
                        foreach (var tag in tags)
                        {
                            if (hiddenTags.Contains(tag.Id))
                                continue;
                            // tag packet
                            SendStringAndInt32(11, 2, tag.Name, tag.Id);
                        }
                    }
                    // end packet
                    package[1] = 3;
                    WriteToClient(package, 0, package.Length);
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    Messages.AddMessage(MessageType.Error, String.Format(StringResources.TagsDbError, ex.Message));
                }
            }
        }

        private void InformActiveTags(System.Collections.Generic.IList<Int32> tags)
        {
            if (ClientConnected)
            {
                // start packet for tags
                byte[] package = new byte[3];
                package[0] = 12;
                package[1] = 0;
                package[2] = 0;
                WriteToClient(package, 0, package.Length);
                foreach (Int32 tagId in tags)
                {
                    byte[] ia = BitConverter.GetBytes(tagId);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(ia);
                    byte[] elementPackage = new byte[3 + ia.Length];
                    elementPackage[0] = 12;
                    elementPackage[1] = 1;
                    elementPackage[2] = 0;
                    Array.Copy(ia, 0, elementPackage, 3, ia.Length);
                    WriteToClient(elementPackage, 0, elementPackage.Length);
                }
                // end packet
                package[1] = 2;
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformTagChanged(Int32 tagId, bool active)
        {
            if (ClientConnected)
            {
                byte[] ia = BitConverter.GetBytes(tagId);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(ia);
                byte[] package = new byte[3 + ia.Length];
                package[0] = 13;
                package[1] = (byte) (active ? 1 : 0);
                package[2] = 0;
                Array.Copy(ia, 0, package, 3, ia.Length);
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformCategoryCombinationChanged(Data.TagCategoryCombination categoryCombination)
        {
            if (ClientConnected)
            {
                byte[] package = new byte[3];
                package[0] = 14;
                package[1] = (byte)(categoryCombination);
                package[2] = 0;
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformFading(int fadeTime, bool fadeOnlyOnChange)
        {
            if (ClientConnected)
            {
                byte[] ia = BitConverter.GetBytes(fadeTime);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(ia);
                byte[] package = new byte[3 + ia.Length];
                package[0] = 17;
                package[1] = (byte)(fadeOnlyOnChange ? 1 : 0);
                package[2] = 0;
                Array.Copy(ia, 0, package, 3, ia.Length);
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformError(String errorMessage)
        {
            if (ClientConnected)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(errorMessage);
                byte[] package = new byte[3 + bytes.Length];
                package[0] = 3;
                package[1] = (byte)(bytes.Length / (1 << 8));
                package[2] = (byte)(bytes.Length % (1 << 8));
                Array.Copy(bytes, 0, package, 3, bytes.Length);
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformVolume(Ares.Playing.VolumeTarget target, int value)
        {
            if (ClientConnected)
            {
                byte[] bytes = new byte[3];
                bytes[0] = 4;
                bytes[1] = (byte)target;
                bytes[2] = (byte)value;
                WriteToClient(bytes, 0, bytes.Length);
            }
        }

        private void InformDisconnect()
        {
            if (ClientConnected)
            {
                byte[] bytes = new byte[3];
                bytes[0] = 5;
                bytes[1] = 0;
                bytes[2] = 0;
                WriteToClient(bytes, 0, bytes.Length);
            }
        }

        private void InformProjectChange(String newProject)
        {
            if (ClientConnected)
            {
                if (newProject == null)
                    newProject = String.Empty;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newProject);
                byte[] package = new byte[3 + bytes.Length];
                package[0] = 6;
                package[1] = (byte)(bytes.Length / (1 << 8));
                package[2] = (byte)(bytes.Length % (1 << 8));
                Array.Copy(bytes, 0, package, 3, bytes.Length);
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformAllElementsStopped()
        {
            if (ClientConnected)
            {
                byte[] package = new byte[3];
                package[0] = 7;
                package[1] = 0;
                package[2] = 0;
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformRepeatChanged(bool repeat)
        {
            if (ClientConnected)
            {
                byte[] package = new byte[3];
                package[0] = 10;
                package[1] = (byte)(repeat ? 1 : 0);
                package[2] = 0;
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformMusicOnAllChannelsChanged(bool onAllSpeakers)
        {
            if (ClientConnected)
            {
                byte[] package = new byte[3];
                package[0] = 18;
                package[1] = (byte)(onAllSpeakers ? 1 : 0);
                package[2] = 0;
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformPreviousNextFading(int fadingOption, int fadingTime)
        {
            if (ClientConnected)
            {
                byte[] ia = BitConverter.GetBytes(fadingTime);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(ia);
                byte[] package = new byte[3 + ia.Length];
                package[0] = 19;
                package[1] = (byte)(fadingOption);
                package[2] = 0;
                Array.Copy(ia, 0, package, 3, ia.Length);
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformImportProgress(int percent, String additionalInfo)
        {
            if (ClientConnected)
            {
                SendStringAndInt32(20, 0, additionalInfo, percent);
            }
        }

		#if ANDROID
		private void GetProjectsFromFolder(String folderPath, out String[] files1, out String[] files2)
		{
			String[] res1 = null;
			String[] res2 = null;
			try 
			{
				var folderTask = Ares.Settings.FolderFactory.CreateFromSerialization(folderPath);
				var task2 = folderTask.ContinueWith((firstTask) =>
					{
						var filesTask = firstTask.Result.GetProjectNames();
						return filesTask;
					}).Unwrap();
				var task3 = task2.ContinueWith((secondTask) =>
					{
						var files = secondTask.Result;
						var list1 = new List<String>();
						var list2 = new List<String>();
						foreach (var file in files)
						{
							if (file.DisplayName.EndsWith(".ares", StringComparison.InvariantCultureIgnoreCase))
								list1.Add(file.DisplayName);
							else
								list2.Add(file.DisplayName);
						}
						if (Ares.Settings.Settings.Instance.MusicDirectory.IsSmbFile())
						{
							// can't open packed projects if music is on a share because the tags db can't be changed
							list2.Clear();
						}
						res1 = list1.ToArray();
						res2 = list2.ToArray();
					});
				task3.Wait();
				files1 = res1;
				files2 = res2;
			}
			catch (System.IO.IOException)
			{
				files1 = new string[0];
				files2 = new string[0];
			}
		}
		#else
		private void GetProjectsFromFolder(String folderPath, out String[] files1, out String[] files2)
		{
			files1 = System.IO.Directory.GetFiles(folderPath, "*.ares");
			files2 = System.IO.Directory.GetFiles(folderPath, "*.apkg");
		}
		#endif

        private void InformPossibleProjects()
        {
            if (ClientConnected)
            {
                String directory = networkClient.GetProjectsDirectory();
				String[] files1 = null;
				String[] files2 = null;
				GetProjectsFromFolder(directory, out files1, out files2);
                String[] files = new String[files1.Length + files2.Length];
                for (int i = 0; i < files1.Length; ++i)
                {
                    files[i] = System.IO.Path.GetFileName(files1[i]);
                }
                for (int i = 0; i < files2.Length; ++i)
                {
                    files[i + files1.Length] = System.IO.Path.GetFileName(files2[i]);
                }
                Array.Sort(files, StringComparer.CurrentCultureIgnoreCase);
                // start packet for project files
                byte[] package = new byte[3];
                package[0] = 15;
                package[1] = 0;
                package[2] = 0;
                WriteToClient(package, 0, package.Length);
                foreach (String file in files)
                {
                    // category packet
                    SendStringAndInt32(15, 1, file, 0);
                }
                // end packet
                package[1] = 2;
                WriteToClient(package, 0, package.Length);
            }
        }

        private void InformProjectModel()
        {
            Ares.Data.IProject project = networkClient.GetCurrentProject();
            InformProjectModel(project);
        }

        private void InformProjectModel(Ares.Data.IProject project)
        {
            if (ClientConnected)
            {
                // start packet for project
                if (project == null)
                {
                    byte[] package = new byte[3];
                    package[0] = 16;
                    package[1] = 0;
                    package[2] = 1;
                    WriteToClient(package, 0, package.Length);
                    return;
                }
                String filePath = project.FileName;
                String fileName = System.IO.Path.GetFileName(filePath);
                byte[] pa = System.Text.Encoding.UTF8.GetBytes(fileName);
                byte[] na = System.Text.Encoding.UTF8.GetBytes(project.Title);
                byte[] startPackage = new byte[3 + 2 + na.Length + 2 + pa.Length];
                startPackage[0] = 16;
                startPackage[1] = 0;
                startPackage[2] = 0;
                startPackage[3] = (byte)(na.Length / (1 << 8));
                startPackage[3 + 1] = (byte)(na.Length % (1 << 8));
                Array.Copy(na, 0, startPackage, 3 + 2, na.Length);
                startPackage[3 + 2 + na.Length] = (byte)(pa.Length / (1 << 8));
                startPackage[3 + 2 + na.Length + 1] = (byte)(pa.Length % (1 << 8));
                Array.Copy(pa, 0, startPackage, 3 + 2 + na.Length + 2, pa.Length);
                WriteToClient(startPackage, 0, startPackage.Length);
                foreach (Ares.Data.IMode mode in project.GetModes())
                {
                    // mode package
                    byte[] sa = System.Text.Encoding.UTF8.GetBytes(mode.Title);
                    byte[] modePackage = new byte[3 + 2 + sa.Length + 2];
                    modePackage[0] = 16;
                    modePackage[1] = 1;
                    modePackage[2] = 0;
                    modePackage[3] = (byte)(sa.Length / (1 << 8));
                    modePackage[3 + 1] = (byte)(sa.Length % (1 << 8));
                    Array.Copy(sa, 0, modePackage, 3 + 2, sa.Length);
                    Array.Copy(MapKeyToKeyCode(mode.KeyCode), 0, modePackage, 3 + 2 + sa.Length, 2);
                    WriteToClient(modePackage, 0, modePackage.Length);
                    foreach (Ares.Data.IModeElement modeElement in mode.GetElements())
                    {
                        if (!modeElement.IsVisibleInPlayer)
                            continue;
                        // element package
                        byte[] sa2 = System.Text.Encoding.UTF8.GetBytes(modeElement.Title);
                        byte[] ia = BitConverter.GetBytes(modeElement.Id);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(ia);
                        byte[] keyCode = modeElement.Trigger != null && modeElement.Trigger.TriggerType == Ares.Data.TriggerType.Key ?
                            MapKeyToKeyCode(((Ares.Data.IKeyTrigger)modeElement.Trigger).KeyCode) : new byte[] { 0, 0 };
                        byte[] elementPackage = new byte[3 + 2 + sa2.Length + ia.Length + 2];
                        elementPackage[0] = 16;
                        elementPackage[1] = 2;
                        elementPackage[2] = 0;
                        elementPackage[3] = (byte)(sa2.Length / (1 << 8));
                        elementPackage[3 + 1] = (byte)(sa2.Length % (1 << 8));
                        Array.Copy(sa2, 0, elementPackage, 3 + 2, sa2.Length);
                        Array.Copy(ia, 0, elementPackage, 3 + 2 + sa2.Length, ia.Length);
                        Array.Copy(keyCode, 0, elementPackage, 3 + 2 + sa2.Length + ia.Length, 2);
                        WriteToClient(elementPackage, 0, elementPackage.Length);
                    }
                }
                // end package
                byte[] endPackage = new byte[3];
                endPackage[0] = 16;
                endPackage[1] = 3;
                endPackage[2] = 1;
                WriteToClient(endPackage, 0, endPackage.Length);
            }
        }

        private void SendPing()
        {
            if (ClientConnected)
            {
                byte[] package = new byte[3];
                package[0] = 9;
                package[1] = (byte)(Networks.PLAYER_VERSION / (1 << 8));
                package[2] = (byte)(Networks.PLAYER_VERSION % (1 << 8));
                try
                {
                    WriteToClient(package, 0, package.Length);
                }
                catch (Exception)
                {
                }
            }
        }

        private IPAddress tcpListenAddress = null;

        private bool listenerThreadRunning;

        private System.Threading.AutoResetEvent listenerThreadStoppedEvent = new System.Threading.AutoResetEvent(false);

        private bool continueListenForClients = true;

        private bool continueListenForCommands = true;

        private bool forceShutdown = false;

        public bool ClientConnected { get; set; }

        public String ClientName { get; set; }

        private TcpClient client = null;

        private INetworkClient networkClient;

        private Object syncObject = new Int32();

        public void ModeChanged(Data.IMode newMode)
        {
            try
            {
                InformModeChange(newMode);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void ModeElementStarted(Data.IModeElement element)
        {
            try
            {
                InformModeElementChange(element, true);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void ModeElementFinished(Data.IModeElement element)
        {
            try
            {
                InformModeElementChange(element, false);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void SoundStarted(int elementId)
        {
        }

        public void SoundFinished(int elementId)
        {
        }

        private int m_CurrentMusicId = -1;

        public void MusicStarted(int elementId)
        {
            try
            {
                m_CurrentMusicId = elementId;
                InformMusicChange(MusicInfo.GetInfo(elementId));
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicFinished(int elementId)
        {
            try
            {
                if (m_CurrentMusicId == elementId)
                {
                    m_CurrentMusicId = -1;
                    InformMusicChange(MusicInfo.GetInfo(-1));
                }
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicPlaylistStarted(int elementId)
        {
            InformMusicList(elementId);
        }

        public void MusicPlaylistFinished(int elementId)
        {
            InformMusicList(-1);
        }

        public void MusicRepeatChanged(bool repeat)
        {
            try
            {
                InformRepeatChanged(repeat);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicOnAllSpeakersChanged(bool onAllSpeakers)
        {
            try
            {
                InformMusicOnAllChannelsChanged(onAllSpeakers);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void PreviousNextFadingChanged(bool fade, bool crossFade, int fadeTime)
        {
            try
            {
                InformPreviousNextFading(fade ? (crossFade ? 2 : 1) : 0, fadeTime);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicTagAdded(int tagId)
        {
            try
            {
                InformTagChanged(tagId, true);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicTagRemoved(int tagId)
        {
            try
            {
                InformTagChanged(tagId, false);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }

        }

        public void AllMusicTagsRemoved()
        {
            try
            {
                InformActiveTags(new System.Collections.Generic.List<int>());
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicTagsChanged(ICollection<int> newTags, Data.TagCategoryCombination categoryCombination, int fadeTime)
        {
            try
            {
                InformActiveTags(new List<int>(newTags));
                InformCategoryCombinationChanged(categoryCombination);
                InformFading(fadeTime, m_MusicTagsFadeOnlyOnChange);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicTagCategoriesCombinationChanged(Data.TagCategoryCombination categoryCombination)
        {
            try
            {
                InformCategoryCombinationChanged(categoryCombination);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicTagsFadingChanged(int fadeTime, bool fadeOnlyOnChange)
        {
            try
            {
                m_MusicTagsFadeOnlyOnChange = fadeOnlyOnChange;
                InformFading(fadeTime, fadeOnlyOnChange);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        private bool m_MusicTagsFadeOnlyOnChange;

        public void VolumeChanged(Ares.Playing.VolumeTarget target, int newValue)
        {
            /*
            try
            {
                InformVolume(target, newValue);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Error, e.Message);
                DoDisconnect(true);
            }
             */
            // Is informed separately by the player when it updates itself
        }

        public void ErrorOccurred(int elementId, string errorMessage)
        {
            try
            {
                if (elementId != -1)
                {
                    Ares.Data.IElement element = Ares.Data.DataModule.ElementRepository.GetElement(elementId);
                    String message = String.Format(StringResources.PlayError, element.Title, errorMessage);
                    InformError(message);
                }
                else
                {
                    InformError(errorMessage);
                }
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }

        public void AddMessage(Ares.Playing.MessageType messageType, String message)
        {
            if (messageType == Ares.Playing.MessageType.Error)
                ErrorOccurred(-1, message);
        }
    }
}
