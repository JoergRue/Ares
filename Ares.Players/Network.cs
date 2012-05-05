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

namespace Ares.Players
{
    public interface INetworkClient
    {
        void KeyReceived(int key);
        void VolumeReceived(Ares.Playing.VolumeTarget target, int value);
        void ClientDataChanged();
        void ProjectShallChange(String newProjectFile);
        void PlayOtherMusic(Int32 elementId);
        void SwitchElement(Int32 elementId);
    }

    public class Network : Ares.Playing.IProjectPlayingCallbacks
    {
        public Network(INetworkClient client)
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

        public void InformClientOfVolume(Ares.Playing.VolumeTarget target, int value)
        {
            InformVolume(target, value);
        }

        public void InformClientOfProject(String newProject)
        {
            InformProjectChange(newProject);
        }

        public void InformClientOfMusicList(Int32 musicListId)
        {
            InformMusicList(musicListId);
        }

        public void InformClientOfEverything(int overallVolume, int musicVolume, int soundVolume, Ares.Data.IMode mode, MusicInfo music,
            System.Collections.Generic.IList<Ares.Data.IModeElement> elements, String projectName, Int32 musicListId)
        {
            InformProjectChange(projectName);
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
                Messages.AddMessage(MessageType.Warning, StringResources.ClientListenError + e.Message);
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
            networkClient.ClientDataChanged();
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
                Ares.Playing.PlayingModule.RemoveCallbacks(this);
            }
            ClientConnected = false;
            Messages.AddMessage(MessageType.Info, StringResources.ClientDisconnected);
            networkClient.ClientDataChanged();
            if (listenAgain)
            {
                StartUdpBroadcast();
                ListenForClient();
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
                else if (keyCode[0] == 1)
                {
                    switch (keyCode[1])
                    {
                        case 1: return (int)Keys.F1;
                        case 2: return (int)Keys.F2;
                        case 3: return (int)Keys.F3;
                        case 4: return (int)Keys.F4;
                        case 5: return (int)Keys.F5;
                        case 6: return (int)Keys.F6;
                        case 7: return (int)Keys.F7;
                        case 8: return (int)Keys.F8;
                        case 9: return (int)Keys.F9;
                        case 10: return (int)Keys.F10;
                        case 11: return (int)Keys.F11;
                        case 12: return (int)Keys.F12;
                    }
                }
                else if (keyCode[0] == 2)
                {
                    switch (keyCode[1])
                    {
                        case 0: return (int)Keys.NumPad0;
                        case 1: return (int)Keys.NumPad1;
                        case 2: return (int)Keys.NumPad2;
                        case 3: return (int)Keys.NumPad3;
                        case 4: return (int)Keys.NumPad4;
                        case 5: return (int)Keys.NumPad5;
                        case 6: return (int)Keys.NumPad6;
                        case 7: return (int)Keys.NumPad7;
                        case 8: return (int)Keys.NumPad8;
                        case 9: return (int)Keys.NumPad9;
                    }
                }
                else if (keyCode[0] == 3)
                {
                    switch (keyCode[1])
                    {
                        case 0: return (int)Keys.Insert;
                        case 1: return (int)Keys.Delete;
                        case 2: return (int)Keys.Home;
                        case 3: return (int)Keys.End;
                        case 4: return (int)Keys.PageUp;
                        case 5: return (int)Keys.PageDown;
                        case 6: return (int)Keys.Left;
                        case 7: return (int)Keys.Right;
                        case 8: return (int)Keys.Up;
                        case 9: return (int)Keys.Down;
                        case 10: return (int)Keys.Space;
                        case 11: return (int)Keys.Return;
                        case 12: return (int)Keys.Escape;
                        case 13: return (int)Keys.Tab;
                        case 14: return (int)Keys.Back;
                        case 15: return (int)Keys.OemPeriod;
                        case 16: return (int)Keys.OemSemicolon;
                        case 17: return (int)Keys.Oem4;
                        case 18: return (int)Keys.OemComma;
                        case 19: return (int)Keys.Oem2;
                        case 20: return (int)Keys.OemMinus;
                        case 21: return (int)Keys.Oem3;
                    }
                }
                return (int)Keys.Escape;
            }
            catch (Exception e)
            {
                Messages.AddMessage(MessageType.Error, String.Format(StringResources.InvalidKeyCode, e.Message));
                return (int)Keys.Escape;
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
                        Messages.AddMessage(MessageType.Debug, String.Format(StringResources.CommandReceived, command));
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
                            success = client != null && ReadFromStream(client.GetStream(), data, 2, 500);
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
                            networkClient.ProjectShallChange(projectName);
                        }
                    }
                    else if (command == 7)
                    {
                        Byte[] data = new Byte[4];
                        bool success = false;
                        Int32 newMusicId = -1;
                        lock (syncObject)
                        {
                            success = client != null && ReadFromStream(client.GetStream(), data, 4, 500);
                            if (success)
                            {
                                if (BitConverter.IsLittleEndian)
                                    Array.Reverse(data);
                                newMusicId = BitConverter.ToInt32(data, 0);
                            }
                        }
                        if (success && newMusicId != -1)
                        {
                            networkClient.PlayOtherMusic(newMusicId);
                        }
                    }
                    else if (command == 8)
                    {
                        Byte[] data = new Byte[4];
                        bool success = false;
                        Int32 elementId = -1;
                        lock (syncObject)
                        {
                            success = client != null && ReadFromStream(client.GetStream(), data, 4, 500);
                            if (success)
                            {
                                if (BitConverter.IsLittleEndian)
                                    Array.Reverse(data);
                                elementId = BitConverter.ToInt32(data, 0);
                            }
                        }
                        if (success && elementId != -1)
                        {
                            networkClient.SwitchElement(elementId);
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

        private static readonly int PLAYER_VERSION = 1;

        public void InitConnectionData()
        {
            int tcpPort = Settings.Settings.Instance.TcpPort;
            String ipAddress = Settings.Settings.Instance.IPAddress;
            StringBuilder str = new StringBuilder();
            String machineName = Dns.GetHostName();
            str.Append(machineName);
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(bytes, 0, bytes.Length);
                }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
            }
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
                lock (syncObject)
                {
                    client.GetStream().Write(package, 0, package.Length);
                }
                Ares.Data.IElement element = musicListId != -1 ? Ares.Data.DataModule.ElementRepository.GetElement(musicListId) : null;
                Ares.Data.IMusicList musicList = (element != null && element is Ares.Data.IMusicList) ? element as Ares.Data.IMusicList : null;
                // one packet each for each music title
                if (musicList != null)
                {
                    foreach (Ares.Data.IFileElement fileElement in musicList.GetFileElements())
                    {
                        byte[] title = System.Text.Encoding.UTF8.GetBytes(fileElement.Title);
                        byte[] elementId = BitConverter.GetBytes(fileElement.Id);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(elementId);
                        byte[] elementPackage = new byte[3 + elementId.Length + 2 + title.Length];
                        elementPackage[0] = 8;
                        elementPackage[1] = 1;
                        elementPackage[2] = 0;
                        Array.Copy(elementId, 0, elementPackage, 3, elementId.Length);
                        elementPackage[3 + elementId.Length] = (byte)(title.Length / (1 << 8));
                        elementPackage[3 + elementId.Length + 1] = (byte)(title.Length % (1 << 8));
                        Array.Copy(title, 0, elementPackage, 3 + elementId.Length + 2, title.Length);
                        lock (syncObject)
                        {
                            client.GetStream().Write(elementPackage, 0, elementPackage.Length);
                        }
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

        private void InformVolume(Ares.Playing.VolumeTarget target, int value)
        {
            if (ClientConnected)
            {
                byte[] bytes = new byte[3];
                bytes[0] = 4;
                bytes[1] = (byte)target;
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

        public void MusicPlaylistFinished()
        {
            InformMusicList(-1);
        }

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
                Ares.Data.IElement element = Ares.Data.DataModule.ElementRepository.GetElement(elementId);
                String message = String.Format(StringResources.PlayError, element.Title, errorMessage);
                InformError(message);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                DoDisconnect(true);
            }
        }
    }
}
