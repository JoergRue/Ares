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
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

namespace Ares.MediaPortalPlugin
{
    public interface IFromControllerNetworkClient
    {
        void FromControllerKeyReceived(byte[] keyCode);
        void FromControllerVolumeReceived(int index, int value);
        void FromControllerClientDataChanged();
        String FromControllerGetProjectsDirectory();
        void FromControllerProjectShallChange(String newProjectFile);
        Ares.Controllers.Configuration FromControllerGetCurrentProject();
        void FromControllerPlayOtherMusic(Int32 elementId);
        void FromControllerSwitchElement(Int32 elementId);
        void FromControllerSetMusicRepeat(bool repeat);
        void FromControllerSwitchTag(Int32 categoryId, Int32 tagId, bool tagIsActive);
        void FromControllerDeactivateAllTags();
        void FromControllerSetTagCategoryOperator(bool operatorIsAnd);
        void FromControllerSetMusicTagsFading(Int32 fadeTime, bool onlyOnChange);
        void FromControllerSetPlayMusicOnAllSpeakers(bool onAllSpeakers);
    }

    public class FromControllerNetwork
    {
        public FromControllerNetwork(IFromControllerNetworkClient client)
        {
            networkClient = client;
            ClientConnected = false;
            ClientName = String.Empty;
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
                    MediaPortal.GUI.Library.Log.Debug(String.Format(StringResources.UDPSending, udpString));
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
                MediaPortal.GUI.Library.Log.Warn(StringResources.UDPError + e.Message);
                return false;
            }
        }

        public void StartUdpBroadcast()
        {
            MediaPortal.GUI.Library.Log.Info(StringResources.StartBroadcast);
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
            MediaPortal.GUI.Library.Log.Info(StringResources.StopBroadcast);
            lock (syncObject)
            {
                if (udpClient == null)
                    return;
                udpClient.Close();
                udpClient = null;
            }
        }

        public void ListenForClient()
        {
            lock (syncObject)
            {
                if (forceShutdown)
                    return;
                continueListenForClients = true;
            }
            System.Threading.Thread listenThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenInThread));
            listenThread.Start();
        }

        public void StopListenForClient()
        {
            lock (syncObject)
            {
                continueListenForClients = false;
            }
        }

        public void Shutdown()
        {
            lock (syncObject)
            {
                continueListenForClients = false;
                forceShutdown = true;
            }
        }

        public void InformClientOfVolume(int volumeIndex, int value)
        {
            InformVolume(volumeIndex, value);
        }

        public void InformClientOfProject(Ares.Controllers.Configuration newProject)
        {
            InformProjectModel(newProject);
        }

        public void InformClientOfMusicList(List<Controllers.MusicListItem> newList)
        {
            InformMusicList(newList);
        }

        public void InformClientOfPossibleTags(List<Ares.Controllers.MusicTagCategory> categories, Dictionary<int, List<Ares.Controllers.MusicTag>> tagsPerCategory)
        {
            InformPossibleTags(categories, tagsPerCategory);
        }

        public void InformClientOfActiveTags(System.Collections.Generic.IList<int> activeTags)
        {
            InformActiveTags(activeTags);
        }

        public void InformClientOfTagCategoryOperator(bool operatorIsAnd)
        {
            InformCategoryOperatorChanged(operatorIsAnd);
        }

        public void InformClientOfFading(int fadeTime, bool fadeOnlyOnChange)
        {
            InformFading(fadeTime, fadeOnlyOnChange);
        }

        public void InformClientOfEverything(int overallVolume, int musicVolume, int soundVolume, String mode, String shortMusic, String longMusic,
            System.Collections.Generic.List<int> elements, Ares.Controllers.Configuration configuration, List<Ares.Controllers.MusicListItem> musicList, bool musicRepeat,
            List<Ares.Controllers.MusicTagCategory> categories, Dictionary<int, List<Ares.Controllers.MusicTag>> tagsPerCategory, 
            System.Collections.Generic.IList<int> activeTags, bool tagCategoryOperatorIsAnd, int fadeTime, bool fadeOnlyOnChange,
            bool musicOnAllChannels)
        {
            InformProjectModel(configuration);
            InformPossibleTags(categories, tagsPerCategory);
            InformVolume(2, overallVolume);
            InformVolume(1, musicVolume);
            InformVolume(0, soundVolume);
            InformMusicChange(shortMusic, longMusic);
            InformModeChange(mode);
            InformAllElementsStopped();
            foreach (int element in elements) {
                InformModeElementChange(element, true);
            }
            InformMusicList(musicList);
            InformRepeatChanged(musicRepeat);
            InformMusicOnAllChannelsChanged(musicOnAllChannels);
            InformActiveTags(activeTags);
            InformCategoryOperatorChanged(tagCategoryOperatorIsAnd);
            m_MusicTagsFadeOnlyOnChange = fadeOnlyOnChange;
            InformFading(fadeTime, fadeOnlyOnChange);
        }

        public void ListenInThread()
        {
            bool goOn = true;
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
                MediaPortal.GUI.Library.Log.Warn(StringResources.ClientListenError + e.Message);
            }
            finally
            {
                listener.Stop();
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
            MediaPortal.GUI.Library.Log.Debug(StringResources.ClientLengthReceived + s);
            success = Int32.TryParse(s, out length);
            if (!success)
            {
                MediaPortal.GUI.Library.Log.Debug(StringResources.WrongClientLength);
                return false;
            }
            if (length > 1000)
            {
                MediaPortal.GUI.Library.Log.Debug(StringResources.LengthTooHigh);
                return false;
            }
            success = ReadFromStream(stream, buffer, length, 2000);
            if (!success)
            {
                MediaPortal.GUI.Library.Log.Debug(StringResources.NoClientID);
                return false;
            }
            ClientName = System.Text.Encoding.UTF8.GetString(buffer, 0, length);
            lock (syncObject)
            {
                client = aClient;
                // Ares.Playing.PlayingModule.AddCallbacks(this);
                ClientConnected = true;
            }
            MediaPortal.GUI.Library.Log.Info(String.Format(StringResources.ClientConnected, ClientName));
            networkClient.FromControllerClientDataChanged();
            continueListenForCommands = true;
            System.Threading.Thread commandThread = new System.Threading.Thread(ListenForCommands);
            commandThread.Start();
            StopUdpBroadcast();
            m_WatchdogTimer = new System.Timers.Timer(25000);
            m_WatchdogTimer.Elapsed += new System.Timers.ElapsedEventHandler(watchdogTimer_Elapsed);
            m_WatchdogTimer.Start();
            m_PingTimer = new System.Timers.Timer(5000);
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
            MediaPortal.GUI.Library.Log.Warn(StringResources.PingTimeout);
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
                client = null;
                ClientConnected = false;
                if (listenAgain)
                {
                    StartUdpBroadcast();
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
                // Ares.Playing.PlayingModule.RemoveCallbacks(this);
            }
            ClientConnected = false;
            MediaPortal.GUI.Library.Log.Info(StringResources.ClientDisconnected);
            networkClient.FromControllerClientDataChanged();
            if (listenAgain)
            {
                StartUdpBroadcast();
                ListenForClient();
            }
        }

        private bool ReadInt32(out Int32 value)
        {
            Byte[] data = new Byte[4];
            lock (syncObject)
            {
                bool success = client != null && ReadFromStream(client.GetStream(), data, 4, 500);
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
                        if (client != null && client.GetStream().DataAvailable)
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
                        MediaPortal.GUI.Library.Log.Debug(String.Format(StringResources.CommandReceived, command));
                    }
                    if (command == 0)
                    {
                        Byte[] keyCode = new Byte[2];
                        bool success = false;
                        lock (syncObject)
                        {
                            success = client != null && ReadFromStream(client.GetStream(), keyCode, 2, 500);
                        }
                        if (success)
                        {
                            MediaPortal.GUI.Library.Log.Debug(String.Format(StringResources.KeyReceived, keyCode[0], keyCode[1]));
                            networkClient.FromControllerKeyReceived(keyCode);
                        }
                    }
                    else if (command == 1)
                    {
                        MediaPortal.GUI.Library.Log.Debug(StringResources.DisconnectingClient);
                        disconnect = true;
                        break;
                    }
                    else if (command == 2)
                    {
                        Byte[] data = new Byte[2];
                        bool success = false;
                        lock (syncObject)
                        {
                            success = client != null && ReadFromStream(client.GetStream(), data, 2, 500);
                        }
                        if (success)
                        {
                            int index = data[0];
                            int vol = (int)data[1];
                            MediaPortal.GUI.Library.Log.Debug(String.Format(StringResources.VolumeCommandReceived, index, vol));
                            if (vol < 0 || vol > 100)
                            {
                                MediaPortal.GUI.Library.Log.Warn(StringResources.VolumeOutOfRange);
                            }
                            else
                            {
                                networkClient.FromControllerVolumeReceived(index, vol);
                            }
                        }
                    }
                    else if (command == 5)
                    {
                        // this is ping
                        MediaPortal.GUI.Library.Log.Debug(StringResources.PingReceived);
                        if (m_WatchdogTimer != null)
                        {
                            m_WatchdogTimer.Stop();
                            m_WatchdogTimer.Interval = 7000;
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
                            success = client != null && ReadFromStream(client.GetStream(), data, 2, 500);
                            if (success)
                            {
                                int length = data[0] * (1 << 8);
                                length += data[1];
                                Byte[] bytes = new Byte[length];
                                success = client != null && ReadFromStream(client.GetStream(), bytes, length, 500);
                                if (success)
                                {
                                    projectName = System.Text.Encoding.UTF8.GetString(bytes);
                                }
                            }
                        }
                        if (success && !String.IsNullOrEmpty(projectName) && networkClient != null)
                        {
                            networkClient.FromControllerProjectShallChange(projectName);
                        }
                    }
                    else if (command == 7)
                    {
                        Int32 newMusicId = -1;
                        bool success = ReadInt32(out newMusicId);
                        if (success && newMusicId != -1)
                        {
                            networkClient.FromControllerPlayOtherMusic(newMusicId);
                        }
                    }
                    else if (command == 8)
                    {
                        Int32 elementId = -1;
                        bool success = ReadInt32(out elementId);
                        if (success && elementId != -1)
                        {
                            networkClient.FromControllerSwitchElement(elementId);
                        }
                    }
                    else if (command == 9)
                    {
                        Int32 repeatValue = 0;
                        bool success = ReadInt32(out repeatValue);
                        if (success)
                        {
                            networkClient.FromControllerSetMusicRepeat(repeatValue == 1);
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
                            networkClient.FromControllerSwitchTag(categoryId, tagId, onOff == 1);
                        }
                    }
                    else if (command == 11)
                    {
                        networkClient.FromControllerDeactivateAllTags();
                    }
                    else if (command == 12)
                    {
                        Int32 isAnd = -1;
                        bool success = ReadInt32(out isAnd);
                        if (success)
                        {
                            networkClient.FromControllerSetTagCategoryOperator(isAnd == 1);
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
                            networkClient.FromControllerSetMusicTagsFading(fadeTime, onlyOnChange == 1);
                        }
                    }
                    else if (command == 15)
                    {
                        Int32 onAllSpeakers = -1;
                        bool success = ReadInt32(out onAllSpeakers);
                        if (success)
                        {
                            networkClient.FromControllerSetPlayMusicOnAllSpeakers(onAllSpeakers == 1);
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
                MediaPortal.GUI.Library.Log.Warn(StringResources.KeyListenError + e.Message);
                DoDisconnect(true);
            }
        }

        private static readonly int PLAYER_VERSION = 2;

        public static readonly String MEDIA_PORTAL = " (MediaPortal)";

        public void InitConnectionData()
        {
            int tcpPort = Settings.Settings.Instance.TcpPort;
            String ipAddress = Settings.Settings.Instance.IPAddress;
            StringBuilder str = new StringBuilder();
            String machineName = Dns.GetHostName();
            str.Append(machineName);
            str.Append(MEDIA_PORTAL);
            str.Append("|");
            str.Append(tcpPort);
            if (!String.IsNullOrEmpty(ipAddress))
            {
                str.Append("|");
                str.Append(ipAddress);
                tcpListenAddress = IPAddress.Parse(ipAddress);
            }
            str.Append("|");
            str.Append(PLAYER_VERSION);
            udpString = str.ToString();
            udpPacket = System.Text.Encoding.UTF8.GetBytes(udpString);
        }

        private void InformModeChange(String mode)
        {
            if (ClientConnected)
            {
                String title = mode != null ? mode : String.Empty;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(title);
                byte[] package = new byte[3 + bytes.Length];
                package[0] = 0;
                package[1] = (byte)(bytes.Length / (1 << 8));
                package[2] = (byte)(bytes.Length % (1 << 8));
                Array.Copy(bytes, 0, package, 3, bytes.Length);
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
            }
        }

        private void InformModeElementChange(int id, bool isActive)
        {
            if (ClientConnected)
            {
                byte[] bytes = new byte[4];
                bytes[0] = 1;
                bytes[1] = (byte)(id / (1 << 8));
                bytes[2] = (byte)(id % (1 << 8));
                bytes[3] = (byte)(isActive ? 1 : 0);
                lock (syncObject)
                {
                    client.GetStream().Write(bytes, 0, bytes.Length);
                }
            }
        }

        private void InformMusicChange(String shortMusic, String longMusic)
        {
            if (ClientConnected)
            {
                byte[] bytes1 = System.Text.Encoding.UTF8.GetBytes(shortMusic);
                byte[] bytes2 = System.Text.Encoding.UTF8.GetBytes(longMusic);
                byte[] package = new byte[5 + bytes1.Length + bytes2.Length];
                package[0] = 2;
                package[1] = (byte)(bytes1.Length / (1 << 8));
                package[2] = (byte)(bytes1.Length % (1 << 8));
                Array.Copy(bytes1, 0, package, 3, bytes1.Length);
                package[3 + bytes1.Length] = (byte)(bytes2.Length / (1 << 8));
                package[3 + bytes1.Length + 1] = (byte)(bytes2.Length % (1 << 8));
                Array.Copy(bytes2, 0, package, 3 + bytes1.Length + 2, bytes2.Length);
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
            lock (syncObject)
            {
                client.GetStream().Write(elementPackage, 0, elementPackage.Length);
            }
        }

        private void InformMusicList(List<Controllers.MusicListItem> newList)
        {
            if (ClientConnected)
            {
                // start packet
                byte[] package = new byte[3];
                package[0] = 8;
                package[1] = 0;
                package[2] = 0;
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
                // one packet each for each music title
                if (newList != null)
                {
                    foreach (Controllers.MusicListItem item in newList)
                    {
                        SendStringAndInt32(8, 1, item.Title, item.Id);
                    }
                }
                // end packet
                package[1] = 2;
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
            }
        }

        private void InformPossibleTags(List<Ares.Controllers.MusicTagCategory> categories, Dictionary<int, List<Ares.Controllers.MusicTag>> tagsPerCategory)
        {
            if (ClientConnected)
            {
                // start packet for categories
                byte[] package = new byte[3];
                package[0] = 11;
                package[1] = 0;
                package[2] = 0;
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
                foreach (var category in categories)
                {
                    // category packet
                    SendStringAndInt32(11, 1, category.Title, category.Id);
                    // all tags for the category
                    var tags = tagsPerCategory[category.Id];
                    foreach (var tag in tags)
                    {
                        // tag packet
                        SendStringAndInt32(11, 2, tag.Title, tag.Id);
                    }
                }
                // end packet
                package[1] = 3;
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                    lock (syncObject)
                    {
                        client.GetStream().Write(elementPackage, 0, elementPackage.Length);
                    }
                }
                // end packet
                package[1] = 2;
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
            }
        }

        private void InformCategoryOperatorChanged(bool isAndOperator)
        {
            if (ClientConnected)
            {
                byte[] package = new byte[3];
                package[0] = 14;
                package[1] = (byte)(isAndOperator ? 1 : 0);
                package[2] = 0;
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
            }
        }

        private void InformVolume(int index, int value)
        {
            if (ClientConnected)
            {
                byte[] bytes = new byte[3];
                bytes[0] = 4;
                bytes[1] = (byte)index;
                bytes[2] = (byte)value;
                lock (syncObject)
                {
                    client.GetStream().Write(bytes, 0, bytes.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(bytes, 0, bytes.Length);
                }                
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
            }
        }

        private void InformPossibleProjects()
        {
            if (ClientConnected)
            {
                String directory = networkClient.FromControllerGetProjectsDirectory();
                String[] files1 = System.IO.Directory.GetFiles(directory, "*.ares");
                String[] files2 = System.IO.Directory.GetFiles(directory, "*.apkg");
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
                foreach (String file in files)
                {
                    // category packet
                    SendStringAndInt32(15, 1, file, 0);
                }
                // end packet
                package[1] = 2;
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
            }
        }

        private void InformProjectModel()
        {
            Ares.Controllers.Configuration project = networkClient.FromControllerGetCurrentProject();
            InformProjectModel(project);
        }

        private void InformProjectModel(Ares.Controllers.Configuration config)
        {
            if (ClientConnected)
            {
                // start packet for project
                if (config == null)
                {
                    byte[] package = new byte[3];
                    package[0] = 16;
                    package[1] = 0;
                    package[2] = 1;
                    lock (syncObject)
                    {
                        client.GetStream().Write(package, 0, package.Length);
                    }
                    return;
                }
                String filePath = config.FileName;
                String fileName = System.IO.Path.GetFileName(filePath);
                byte[] pa = System.Text.Encoding.UTF8.GetBytes(fileName);
                byte[] na = System.Text.Encoding.UTF8.GetBytes(config.Title);
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
                lock (syncObject)
                {
                    client.GetStream().Write(startPackage, 0, startPackage.Length);
                }
                foreach (Controllers.Mode mode in config.Modes)
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
                    Array.Copy(mode.KeyCode, 0, modePackage, 3 + 2 + sa.Length, 2);
                    lock (syncObject)
                    {
                        client.GetStream().Write(modePackage, 0, modePackage.Length);
                    }
                    foreach (Controllers.ModeElement modeElement in mode.Elements)
                    {
                        // element package
                        byte[] sa2 = System.Text.Encoding.UTF8.GetBytes(modeElement.Title);
                        byte[] ia = BitConverter.GetBytes(modeElement.Id);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(ia);
                        byte[] keyCode = modeElement.KeyCode;
                        byte[] elementPackage = new byte[3 + 2 + sa2.Length + ia.Length + 2];
                        elementPackage[0] = 16;
                        elementPackage[1] = 2;
                        elementPackage[2] = 0;
                        elementPackage[3] = (byte)(sa2.Length / (1 << 8));
                        elementPackage[3 + 1] = (byte)(sa2.Length % (1 << 8));
                        Array.Copy(sa2, 0, elementPackage, 3 + 2, sa2.Length);
                        Array.Copy(ia, 0, elementPackage, 3 + 2 + sa2.Length, ia.Length);
                        Array.Copy(keyCode, 0, elementPackage, 3 + 2 + sa2.Length + ia.Length, 2);
                        lock (syncObject)
                        {
                            client.GetStream().Write(elementPackage, 0, elementPackage.Length);
                        }
                    }
                }
                // end package
                byte[] endPackage = new byte[3];
                endPackage[0] = 16;
                endPackage[1] = 3;
                endPackage[2] = 1;
                lock (syncObject)
                {
                    client.GetStream().Write(endPackage, 0, endPackage.Length);
                }
            }
        }

        private void SendPing()
        {
            if (ClientConnected)
            {
                byte[] package = new byte[3];
                package[0] = 9;
                package[1] = (byte)(PLAYER_VERSION / (1 << 8));
                package[2] = (byte)(PLAYER_VERSION % (1 << 8));
                try
                {
                    lock (syncObject)
                    {
                        if (client != null && client.Client != null)
                        {
                            client.GetStream().Write(package, 0, package.Length);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private Byte[] udpPacket;

        private String udpString;

        private IPAddress tcpListenAddress = null;

        private bool continueListenForClients = true;

        private bool continueListenForCommands = true;

        private bool forceShutdown = false;

        public bool ClientConnected { get; set; }

        public String ClientName { get; set; }

        private TcpClient client = null;

        private UdpClient udpClient = null;

        private IFromControllerNetworkClient networkClient;

        private Object syncObject = new Int32();

        public void ModeChanged(String newMode)
        {
            try
            {
                InformModeChange(newMode);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

        public void ModeElementStarted(int id)
        {
            try
            {
                InformModeElementChange(id, true);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

        public void ModeElementFinished(int id)
        {
            try
            {
                InformModeElementChange(id, false);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicChanged(String shortMusic, String longMusic)
        {
            try
            {
                InformMusicChange(shortMusic, longMusic);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicPlaylistChanged(List<Controllers.MusicListItem> newList)
        {
            InformMusicList(newList);
        }

        public void MusicRepeatChanged(bool repeat)
        {
            try
            {
                InformRepeatChanged(repeat);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
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
                MediaPortal.GUI.Library.Log.Warn(e.Message);
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
                MediaPortal.GUI.Library.Log.Warn(e.Message);
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
                MediaPortal.GUI.Library.Log.Warn(e.Message);
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
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicTagsChanged(ICollection<int> newTags, bool operatorIsAnd, int fadeTime)
        {
            try
            {
                InformActiveTags(new List<int>(newTags));
                InformCategoryOperatorChanged(operatorIsAnd);
                InformFading(fadeTime, m_MusicTagsFadeOnlyOnChange);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicTagCategoriesOperatorChanged(bool isAndOperator)
        {
            try
            {
                InformCategoryOperatorChanged(isAndOperator);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
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
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

        private bool m_MusicTagsFadeOnlyOnChange;

        public void VolumeChanged(int volumeIndex, int newValue)
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
                InformError(errorMessage);
            }
            catch (System.IO.IOException e)
            {
                MediaPortal.GUI.Library.Log.Warn(e.Message);
                DoDisconnect(true);
            }
        }

    }
}
