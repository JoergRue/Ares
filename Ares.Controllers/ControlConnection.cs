/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Ares.Controllers
{
    public class MusicListItem
    {
        public int Id { get; set; }
        public String Title { get; set; }
    }

    public interface INetworkClient
    {
        void ModeChanged(String newMode);
        void ModeElementStarted(int element);
        void ModeElementStopped(int element);
        void AllModeElementsStopped();
        void VolumeChanged(int index, int volume);
        void MusicChanged(String newMusic, String shortTitle);
        void ProjectChanged(String newTitle);
        void MusicListChanged(List<MusicListItem> newList);

        void Disconnect();
        void ConnectionFailed();
    }


    public sealed class ControlConnection : IDisposable
    {
        private IPEndPoint m_EndPoint;
        private LockedSocket m_Socket;

        private INetworkClient m_NetworkClient;

        private System.Timers.Timer m_PingTimer;
        private System.Timers.Timer m_ReconnectTimer;
        private System.Timers.Timer m_WatchDogTimer;

        private enum State { NotConnected, Connected, ConnectionFailure };
        private State m_State;

        private System.Threading.Thread m_ListenThread;
        private bool m_ContinueListen;
        private Object m_SyncObject = new Int32();
        
        private bool m_CheckedVersion = false;

        private List<MusicListItem> m_CurrentMusicList = new List<MusicListItem>();

        public ControlConnection(ServerInfo server, INetworkClient client)
        {
            m_EndPoint = server.EndPoint;
            m_NetworkClient = client;
            m_State = State.NotConnected;
        }

        public void Dispose()
        {
            if (m_Socket != null)
                Disconnect(true);
        }

        private bool DoConnect(int timeout)
        {
            String hostName = System.Environment.MachineName;
            String textToSend = String.Format("{0:0000}{1}", hostName.Length, hostName);
            m_Socket = new LockedSocket(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            bool result = m_Socket.Connect(m_EndPoint, timeout);
            if (!result)
            {
                Messages.AddMessage(MessageType.Debug, StringResources.ConnectTimeout);
                return false;
            }
            m_Socket.ReceiveTimeout = RECEIVE_TIMEOUT;
            Messages.AddMessage(MessageType.Debug, StringResources.SendingControlInfo + textToSend);
            m_Socket.Send(System.Text.Encoding.UTF8.GetBytes(textToSend));
            m_CheckedVersion = false;
            
            m_ListenThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenForStatusUpdates));
            m_ContinueListen = true;
            m_ListenThread.Start();
            
            m_PingTimer = new System.Timers.Timer(5000);
            m_PingTimer.AutoReset = true;
            m_PingTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_PingTimer_Elapsed);
            m_PingTimer.Start();

            m_WatchDogTimer = new System.Timers.Timer(25000);
            m_WatchDogTimer.AutoReset = false;
            m_WatchDogTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_WatchDogTimer_Elapsed);
            m_WatchDogTimer.Start();

            m_State = State.Connected;
            Messages.AddMessage(MessageType.Info, String.Format(StringResources.ConnectedWith, m_EndPoint.ToString()));
            return true;
        }

        void m_WatchDogTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Messages.AddMessage(MessageType.Warning, StringResources.NoPingFailure);
            HandleConnectionFailure(false);
        }

        void m_PingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendPing();
        }

        public void Connect()
        {
            if (m_Socket != null)
                return;
            try
            {
                bool success = DoConnect(5000);
                if (!success)
                    m_NetworkClient.ConnectionFailed();
            }
            catch (SocketException e)
            {
                Messages.AddMessage(MessageType.Error, e.Message);
                m_NetworkClient.ConnectionFailed();
            }
        }

        private bool TryReconnect()
        {
            if (m_State != State.ConnectionFailure)
                return false;
            try
            {
                bool result = DoConnect(2000);
                if (!result)
                    return false;
                if (m_ReconnectTimer != null)
                {
                    m_ReconnectTimer.Stop();
                    m_ReconnectTimer.Dispose();
                    m_ReconnectTimer = null;
                }
                return true;
            }
            catch (SocketException )
            {
                return false;
            }
        }

        private void DoDisconnect(bool stopListenThread)
        {
            if (m_PingTimer != null)
            {
                m_PingTimer.Stop();
                m_PingTimer.Dispose();
                m_PingTimer = null;
            }
            if (m_WatchDogTimer != null)
            {
                m_WatchDogTimer.Stop();
                m_WatchDogTimer.Dispose();
                m_WatchDogTimer = null;
            }
            if (stopListenThread)
            {
                lock (m_SyncObject)
                {
                    m_ContinueListen = false;
                }
                try
                {
                    if (m_ListenThread != null)
                        m_ListenThread.Join();
                    m_ListenThread = null;
                }
                catch (System.Threading.ThreadInterruptedException)
                {
                }
            }
            m_CheckedVersion = false;
        }

        public void Disconnect(bool informServer)
        {
            if (m_State == State.ConnectionFailure)
            {
                if (m_ReconnectTimer != null)
                {
                    m_ReconnectTimer.Stop();
                    m_ReconnectTimer.Dispose();
                    m_ReconnectTimer = null;
                }
                return;
            }
            else if (m_State == State.NotConnected)
                return;
            else if (informServer && m_ReconnectTimer != null)
            {
                m_ReconnectTimer.Stop();
                m_ReconnectTimer.Dispose();
                m_ReconnectTimer = null;
            }
            m_State = State.NotConnected;
            DoDisconnect(true);
            if (m_Socket.Connected)
            {
                Messages.AddMessage(MessageType.Info, StringResources.ClosingConnection);
                try
                {
                    m_Socket.Disconnect(informServer);
                }
                catch (SocketException e)
                {
                    Messages.AddMessage(MessageType.Error, e.Message);
                }
            }
            m_State = State.NotConnected;
        }

        private void HandleConnectionFailure(bool stopListenThread)
        {
            DoDisconnect(stopListenThread);
            try
            {
                m_Socket.Close();
            }
            catch (SocketException)
            {
            }

            m_ReconnectTimer = new System.Timers.Timer(3000);
            m_ReconnectTimer.AutoReset = true;
            m_ReconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_ReconnectTimer_Elapsed);
            m_ReconnectTimer.Start();

            m_State = State.ConnectionFailure;
        }

        void m_ReconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TryReconnect();
        }

        const int RECEIVE_TIMEOUT = 8000;

        private String ReadString(byte[] firstBytes)
        {
            int length = firstBytes[1];
            length *= (1 << 8);
            length += firstBytes[2];
            byte[] data = new byte[length];
            bool result = m_Socket.Receive(data, RECEIVE_TIMEOUT);
            if (!result)
            {
                HandleConnectionFailure(false);
                return String.Empty;
            }
            return System.Text.Encoding.UTF8.GetString(data);
        }

        private void ListenForStatusUpdates()
        {
            bool goOn = true;
            while (goOn)
            {
                try
                {
                    if (m_Socket.Available < 3)      
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(50);
                        }
                        catch (System.Threading.ThreadInterruptedException)
                        {
                        }
                    }
                    if (m_Socket.Available >= 3)
                    {
                        byte[] buffer = new byte[3];
                        int count = m_Socket.Receive(buffer);
                        if (count < 3)
                            break;
                        switch (buffer[0])
                        {
                            case 0:
                                {
                                    m_NetworkClient.ModeChanged(ReadString(buffer));
                                    break;
                                }
                            case 1:
                                {
                                    int id = buffer[1];
                                    id *= (1 << 8);
                                    id += buffer[2];
                                    bool active = false;
                                    byte[] b = new byte[1];
                                    count = m_Socket.Receive(b);
                                    if (count == 0)
                                        break;
                                    active = b[0] == 1;
                                    if (active)
                                    {
                                        m_NetworkClient.ModeElementStarted(id);
                                    }
                                    else
                                    {
                                        m_NetworkClient.ModeElementStopped(id);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    String longName = ReadString(buffer);
                                    count = m_Socket.Receive(buffer, 1, 2);
                                    if (count < 2)
                                        break;
                                    String shortName = ReadString(buffer);
                                    m_NetworkClient.MusicChanged(longName, shortName);
                                    break;
                                }
                            case 3:
                                {
                                    Messages.AddMessage(MessageType.Error, ReadString(buffer));
                                    break;
                                }
                            case 4:
                                {
                                    int index = buffer[1];
                                    int volume = buffer[2];
                                    m_NetworkClient.VolumeChanged(index, volume);
                                    break;
                                }
                            case 5:
                                {
                                    m_NetworkClient.Disconnect();
                                    lock (m_SyncObject)
                                    {
                                        m_ContinueListen = false;
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    m_NetworkClient.ProjectChanged(ReadString(buffer));
                                    break;
                                }
                            case 7:
                                {
                                    m_NetworkClient.AllModeElementsStopped();
                                    break;
                                }
                            case 8:
                                {
                                    int subcommand = buffer[1];
                                    if (subcommand == 0)
                                    {
                                        m_CurrentMusicList.Clear();
                                    }
                                    else if (subcommand == 1)
                                    {
                                        byte[] bytes = new byte[4];
                                        bool success = m_Socket.Receive(bytes, RECEIVE_TIMEOUT);
                                        if (!success)
                                        {
                                            HandleConnectionFailure(false);
                                            break;
                                        }
                                        count = m_Socket.Receive(buffer, 1, 2);
                                        if (count < 2)
                                            break;
                                        String title = ReadString(buffer);
                                        if (BitConverter.IsLittleEndian)
                                            Array.Reverse(bytes);
                                        MusicListItem item = new MusicListItem();
                                        item.Id =  BitConverter.ToInt32(bytes, 0);
                                        item.Title = title;
                                        m_CurrentMusicList.Add(item);
                                    }
                                    else if (subcommand == 2)
                                    {
                                        m_NetworkClient.MusicListChanged(new List<MusicListItem>(m_CurrentMusicList));
                                    }
                                    break;
                                }
                            case 9:
                                {
                                    int version = buffer[1];
                                    version *= (1 << 8);
                                    version += buffer[2];
                                    if (m_WatchDogTimer != null)
                                    {
                                        m_WatchDogTimer.Stop();
                                        m_WatchDogTimer.Dispose();
                                    }
                                    if (!m_CheckedVersion && version != ServerSearch.NEEDED_SERVER_VERSION)
                                    {
                                        Messages.AddMessage(MessageType.Error, StringResources.PlayerHasWrongVersion);
                                        m_NetworkClient.ConnectionFailed();
                                        break;
                                    }
                                    else
                                    {
                                        m_CheckedVersion = true;
                                    }
                                    m_WatchDogTimer = new System.Timers.Timer(7000);
                                    m_WatchDogTimer.AutoReset = false;
                                    m_WatchDogTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_WatchDogTimer_Elapsed);
                                    m_WatchDogTimer.Start();
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
                catch (SocketException e)
                {
                    Messages.AddMessage(MessageType.Warning, e.Message);
                    HandleConnectionFailure(false);
                    break;
                }
                lock (m_SyncObject)
                {
                    goOn = m_ContinueListen;
                }
            }
        }

        public bool Connected
        {
            get
            {
                return m_State != State.NotConnected;
            }
        }

        public void SendKey(System.Windows.Forms.Keys key)
        {
            if (!Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                return;
            }
            if (m_State == State.ConnectionFailure)
            {
                if (!TryReconnect())
                {
                    Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                    return;
                }
            }
            byte[] bytes = new byte[3];
            bytes[0] = 0;
            if (MapKeysToBytes(key, bytes))
            {
                Messages.AddMessage(MessageType.Info, StringResources.SendingKey + key);
                Messages.AddMessage(MessageType.Debug, String.Format(StringResources.SendingBytes, bytes[0], bytes[1], bytes[2]));
                try
                {
                    m_Socket.Send(bytes);
                }
                catch (SocketException e)
                {
                    Messages.AddMessage(MessageType.Warning, e.Message);
                    HandleConnectionFailure(true);
                }
            }
            else
            {
                Messages.AddMessage(MessageType.Warning, String.Format(StringResources.UnsupportedKey, key));
            }
        }

        public void SetVolume(int index, int value)
        {
            if (!Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                return;
            }
            if (m_State == State.ConnectionFailure)
            {
                if (!TryReconnect())
                {
                    Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                    return;
                }
            }
            byte[] bytes = new byte[3];
            bytes[0] = 2;
            bytes[1] = (byte)index;
            bytes[2] = (byte)value;
            Messages.AddMessage(MessageType.Debug, String.Format(StringResources.SendingBytes, bytes[0], bytes[1], bytes[2]));
            try
            {
                m_Socket.Send(bytes);
            }
            catch (SocketException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
                HandleConnectionFailure(true);
            }
        }

        public void SendPing()
        {
            if (m_State != State.Connected)
                return;
            try
            {
                Messages.AddMessage(MessageType.Debug, StringResources.SendingPing);
                m_Socket.Send(new byte[] { 5 });
            }
            catch (SocketException ex)
            {
                Messages.AddMessage(MessageType.Warning, ex.Message);
                HandleConnectionFailure(true);
            }
        }

        public void SendProjectOpenRequest(String projectName, bool stripPath)
        {
            if (m_State != State.Connected)
                return;
            if (stripPath)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(projectName);
                projectName = file.Name;
            }
            try
            {
                byte[] utf8Name = System.Text.Encoding.UTF8.GetBytes(projectName);
                byte[] bytes = new byte[3 + utf8Name.Length];
                bytes[0] = 6;
                bytes[1] = (byte)(utf8Name.Length / (1 << 8));
                bytes[2] = (byte)(utf8Name.Length % (1 << 8));
                Array.Copy(utf8Name, 0, bytes, 3, utf8Name.Length);
                m_Socket.Send(bytes);
            }
            catch (SocketException ex)
            {
                Messages.AddMessage(MessageType.Warning, ex.Message);
                HandleConnectionFailure(true);
            }
        }

        public void SelectMusicElement(int elementId)
        {
            if (!Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                return;
            }
            if (m_State == State.ConnectionFailure)
            {
                if (!TryReconnect())
                {
                    Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                    return;
                }
            }
            try
            {
                byte[] bytes = new byte[1 + 4];
                bytes[0] = 7;
                byte[] idBytes = BitConverter.GetBytes(elementId);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(idBytes);
                Array.Copy(idBytes, 0, bytes, 1, 4);
                m_Socket.Send(bytes);
            }
            catch (SocketException ex)
            {
                Messages.AddMessage(MessageType.Warning, ex.Message);
                HandleConnectionFailure(true);
            }
        }

        public void SwitchElement(int elementId)
        {
            if (!Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                return;
            }
            if (m_State == State.ConnectionFailure)
            {
                if (!TryReconnect())
                {
                    Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
                    return;
                }
            }
            try
            {
                byte[] bytes = new byte[1 + 4];
                bytes[0] = 8;
                byte[] idBytes = BitConverter.GetBytes(elementId);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(idBytes);
                Array.Copy(idBytes, 0, bytes, 1, 4);
                m_Socket.Send(bytes);
            }
            catch (SocketException ex)
            {
                Messages.AddMessage(MessageType.Warning, ex.Message);
                HandleConnectionFailure(true);
            }
        }

        private Dictionary<System.Windows.Forms.Keys, byte[]> m_CommandMap;

        private void CreateCommandMap()
        {
            m_CommandMap = new Dictionary<System.Windows.Forms.Keys, byte[]>();
            byte[] bytes = null;
            char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int i = 0 ; i < chars.Length; ++i)
            {
                bytes = new byte[2];
                bytes[0] = 0;
                bytes[1] = System.Text.Encoding.ASCII.GetBytes(new char[] { chars[i] })[0];
                m_CommandMap[System.Windows.Forms.Keys.A + i] = bytes;
            }
            chars = "0123456789".ToCharArray();
            for (int i = 0; i < chars.Length; ++i)
            {
                bytes = new byte[2];
                bytes[0] = 0;
                bytes[1] = System.Text.Encoding.ASCII.GetBytes(new char[] { chars[i] })[0];
                m_CommandMap[System.Windows.Forms.Keys.D0 + i] = bytes;
            }
            for (int i = 0; i < 12; ++i)
            {
                bytes = new byte[2];
                bytes[0] = 1;
                bytes[1] = (byte)(i + 1);
                m_CommandMap[System.Windows.Forms.Keys.F1 + i] = bytes;
            }
            for (int i = 0; i <= 9; ++i)
            {
                bytes = new byte[2];
                bytes[0] = 2;
                bytes[1] = (byte)i;
                m_CommandMap[System.Windows.Forms.Keys.NumPad0 + i] = bytes;
            }
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 0;
            m_CommandMap[System.Windows.Forms.Keys.Insert] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 1;
            m_CommandMap[System.Windows.Forms.Keys.Delete] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 2;
            m_CommandMap[System.Windows.Forms.Keys.Home] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 3;
            m_CommandMap[System.Windows.Forms.Keys.End] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 4;
            m_CommandMap[System.Windows.Forms.Keys.PageUp] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 5;
            m_CommandMap[System.Windows.Forms.Keys.PageDown] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 6;
            m_CommandMap[System.Windows.Forms.Keys.Left] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 7;
            m_CommandMap[System.Windows.Forms.Keys.Right] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 8;
            m_CommandMap[System.Windows.Forms.Keys.Up] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 9;
            m_CommandMap[System.Windows.Forms.Keys.Down] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 10;
            m_CommandMap[System.Windows.Forms.Keys.Space] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 11;
            m_CommandMap[System.Windows.Forms.Keys.Enter] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 12;
            m_CommandMap[System.Windows.Forms.Keys.Escape] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 13;
            m_CommandMap[System.Windows.Forms.Keys.Tab] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 14;
            m_CommandMap[System.Windows.Forms.Keys.Back] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 15;
            m_CommandMap[System.Windows.Forms.Keys.OemPeriod] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 16;
            m_CommandMap[System.Windows.Forms.Keys.OemSemicolon] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 17;
            m_CommandMap[System.Windows.Forms.Keys.Oem4] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 18;
            m_CommandMap[System.Windows.Forms.Keys.Oemcomma] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 19;
            m_CommandMap[System.Windows.Forms.Keys.Oem2] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 20;
            m_CommandMap[System.Windows.Forms.Keys.OemMinus] = bytes;
            bytes = new byte[2];
            bytes[0] = 3;
            bytes[1] = 21;
            m_CommandMap[System.Windows.Forms.Keys.Oem3] = bytes;
        }

        private bool MapKeysToBytes(System.Windows.Forms.Keys key, byte[] bytes)
        {
            if (m_CommandMap == null)
                CreateCommandMap();
            if (m_CommandMap.ContainsKey(key))
            {
                byte[] commandBytes = m_CommandMap[key];
                bytes[1] = commandBytes[0];
                bytes[2] = commandBytes[1];
                return true;
            }
            else
                return false;
        }
    }
}
