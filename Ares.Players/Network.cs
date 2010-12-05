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
        void KeyReceived(System.Windows.Forms.Keys key);
        void VolumeReceived(Ares.Playing.VolumeTarget target, int value);
        void ClientDataChanged();
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
                udpClient.Send(udpPacket, udpPacket.Length, new IPEndPoint(IPAddress.Parse("255.255.255.255"), port));
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
            if (udpClient != null)
                StopUdpBroadcast();
            Messages.AddMessage(MessageType.Info, StringResources.StartBroadcast);
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            udpPacketCount = 0;
        }

        public void StopUdpBroadcast()
        {
            if (udpClient == null)
                return;
            Messages.AddMessage(MessageType.Info, StringResources.StopBroadcast);
            udpClient.Close();
            udpClient = null;
        }

        public void ListenForClient()
        {
            lock (syncObject)
            {
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

        public void InformClientOfVolume(Ares.Playing.VolumeTarget target, int value)
        {
            InformVolume(target, value);
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
                Messages.AddMessage(MessageType.Error, StringResources.ClientListenError + e.Message);
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
            continueListenForKeys = true;
            System.Threading.Thread commandThread = new System.Threading.Thread(ListenForKeys);
            commandThread.Start();
            StopUdpBroadcast();
            return true;
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
                continueListenForKeys = false;
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

        private System.Windows.Forms.KeysConverter converter = new System.Windows.Forms.KeysConverter();

        private System.Windows.Forms.Keys MapKeyCodeToKey(Byte[] keyCode)
        {
            try
            {
                if (keyCode[0] == 0)
                {
                    String s = System.Text.Encoding.ASCII.GetString(keyCode, 1, 1);
                    return (System.Windows.Forms.Keys)converter.ConvertFromInvariantString(s);
                }
                else if (keyCode[0] == 1)
                {
                    switch (keyCode[1])
                    {
                        case 1: return System.Windows.Forms.Keys.F1;
                        case 2: return System.Windows.Forms.Keys.F2;
                        case 3: return System.Windows.Forms.Keys.F3;
                        case 4: return System.Windows.Forms.Keys.F4;
                        case 5: return System.Windows.Forms.Keys.F5;
                        case 6: return System.Windows.Forms.Keys.F6;
                        case 7: return System.Windows.Forms.Keys.F7;
                        case 8: return System.Windows.Forms.Keys.F8;
                        case 9: return System.Windows.Forms.Keys.F9;
                        case 10: return System.Windows.Forms.Keys.F10;
                        case 11: return System.Windows.Forms.Keys.F11;
                        case 12: return System.Windows.Forms.Keys.F12;
                    }
                }
                else if (keyCode[0] == 2)
                {
                    switch (keyCode[1])
                    {
                        case 0: return System.Windows.Forms.Keys.NumPad0;
                        case 1: return System.Windows.Forms.Keys.NumPad1;
                        case 2: return System.Windows.Forms.Keys.NumPad2;
                        case 3: return System.Windows.Forms.Keys.NumPad3;
                        case 4: return System.Windows.Forms.Keys.NumPad4;
                        case 5: return System.Windows.Forms.Keys.NumPad5;
                        case 6: return System.Windows.Forms.Keys.NumPad6;
                        case 7: return System.Windows.Forms.Keys.NumPad7;
                        case 8: return System.Windows.Forms.Keys.NumPad8;
                        case 9: return System.Windows.Forms.Keys.NumPad9;
                    }
                }
                else if (keyCode[0] == 3)
                {
                    switch (keyCode[1])
                    {
                        case 0: return System.Windows.Forms.Keys.Insert;
                        case 1: return System.Windows.Forms.Keys.Delete;
                        case 2: return System.Windows.Forms.Keys.Home;
                        case 3: return System.Windows.Forms.Keys.End;
                        case 4: return System.Windows.Forms.Keys.PageUp;
                        case 5: return System.Windows.Forms.Keys.PageDown;
                        case 6: return System.Windows.Forms.Keys.Left;
                        case 7: return System.Windows.Forms.Keys.Right;
                        case 8: return System.Windows.Forms.Keys.Up;
                        case 9: return System.Windows.Forms.Keys.Down;
                        case 10: return System.Windows.Forms.Keys.Space;
                        case 11: return System.Windows.Forms.Keys.Return;
                        case 12: return System.Windows.Forms.Keys.Escape;
                        case 13: return System.Windows.Forms.Keys.Tab;
                        case 14: return System.Windows.Forms.Keys.Back;
                        case 15: return System.Windows.Forms.Keys.OemPeriod;
                        case 16: return System.Windows.Forms.Keys.OemSemicolon;
                        case 17: return System.Windows.Forms.Keys.Oem4;
                        case 18: return System.Windows.Forms.Keys.Oemcomma;
                        case 19: return System.Windows.Forms.Keys.Oem2;
                        case 20: return System.Windows.Forms.Keys.OemMinus;
                        case 21: return System.Windows.Forms.Keys.Oem3;
                    }
                }
                return System.Windows.Forms.Keys.Escape;
            }
            catch (Exception e)
            {
                Messages.AddMessage(MessageType.Error, String.Format(StringResources.InvalidKeyCode, e.Message));
                return System.Windows.Forms.Keys.Escape;
            }
        }

        private void ListenForKeys()
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
                    }
                    lock (syncObject)
                    {
                        goOn = continueListenForKeys;
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
                Messages.AddMessage(MessageType.Error, StringResources.KeyListenError + e.Message);
                DoDisconnect(true);
            }
        }

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
            udpString = str.ToString();
            udpPacket = System.Text.Encoding.UTF8.GetBytes(udpString);
        }

        private void InformModeChange(Ares.Data.IMode mode)
        {
            if (ClientConnected)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(mode.Title);
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

        private void InformMusicChange(String newMusicTitle)
        {
            if (ClientConnected)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newMusicTitle);
                byte[] package = new byte[3 + bytes.Length];
                package[0] = 2;
                package[1] = (byte)(bytes.Length / (1 << 8));
                package[2] = (byte)(bytes.Length % (1 << 8));
                Array.Copy(bytes, 0, package, 3, bytes.Length);
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

        private Byte[] udpPacket;

        private String udpString;

        private IPAddress tcpListenAddress = null;

        private bool continueListenForClients = true;

        private bool continueListenForKeys = true;

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
                Messages.AddMessage(MessageType.Error, e.Message);
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
                Messages.AddMessage(MessageType.Error, e.Message);
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
                Messages.AddMessage(MessageType.Error, e.Message);
                DoDisconnect(true);
            }
        }

        public void SoundStarted(int elementId)
        {
        }

        public void SoundFinished(int elementId)
        {
        }

        public void MusicStarted(int elementId)
        {
            try
            {
                InformMusicChange(MusicInfo.GetInfo(elementId));
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Error, e.Message);
                DoDisconnect(true);
            }
        }

        public void MusicFinished(int elementId)
        {
            try
            {
                InformMusicChange(String.Empty);
            }
            catch (System.IO.IOException e)
            {
                Messages.AddMessage(MessageType.Error, e.Message);
                DoDisconnect(true);
            }
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
                Messages.AddMessage(MessageType.Error, e.Message);
                DoDisconnect(true);
            }
        }
    }
}
