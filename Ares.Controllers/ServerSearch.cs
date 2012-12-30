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
    [Serializable]
    public sealed class ServerInfo
    {
        public String Name { get; private set; }

        public IPEndPoint EndPoint { get; private set; }

        public ServerInfo(String name, IPEndPoint endPoint)
        {
            Name = name;
            EndPoint = endPoint;
        }
    }

    public class ServerEventArgs : EventArgs
    {
        public ServerEventArgs(ServerInfo server)
        {
            Server = server;
        }

        public ServerInfo Server { get; private set; }
    }

    public sealed class ServerSearch : IDisposable
    {
        private int m_Port;

        private bool m_Listening;
        private bool m_ContinueThread;
        private System.Threading.Thread m_ListenerThread;
        private Object m_SyncObject = new Int32();

        public event EventHandler<ServerEventArgs> ServerFound;

        public ServerSearch(int port)
        {
            m_Port = port;
            m_Listening = false;
            m_ContinueThread = true;
        }

        public void Dispose()
        {
            bool isListening = false;
            lock (m_SyncObject)
            {
                isListening = m_Listening;
            }
            if (isListening)
            {
                StopSearch();
            }
        }

        private HashSet<String> m_FoundServers = new HashSet<string>();

        public void StartSearch()
        {
            lock (m_SyncObject)
            {
                if (m_Listening) return;
                m_Listening = true;
            }
            Messages.AddMessage(MessageType.Info, StringResources.StartServerSearch);
            m_ContinueThread = true;
            m_FoundServers.Clear();
            m_ListenerThread = new System.Threading.Thread(new System.Threading.ThreadStart(Listen));
            m_ListenerThread.Start();
        }

        public void StopSearch()
        {
            lock (m_SyncObject)
            {
                if (!m_Listening) return;
                m_ContinueThread = false;
            }
            Messages.AddMessage(MessageType.Info, StringResources.StopServerSearch);
            try
            {
                if (m_ListenerThread.IsAlive)
                    m_ListenerThread.Join();
            }
            catch (System.Threading.ThreadStateException)
            {
            }
            catch (System.Threading.ThreadInterruptedException)
            {
            }
            m_ListenerThread = null;
        }

        public bool IsSearching
        {
            get
            {
                lock (m_SyncObject)
                {
                    return m_Listening;
                }
            }
        }

        private void Listen()
        {
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, m_Port);
                socket.Bind(endPoint);
                socket.ReceiveTimeout = 50;
                bool goOn = true;
                lock (m_SyncObject)
                {
                    goOn = m_ContinueThread;
                }
                while (goOn)
                {
                    LookForDatagram(socket);
                    try
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    catch (System.Threading.ThreadInterruptedException)
                    {
                    }
                    lock (m_SyncObject)
                    {
                        goOn = m_ContinueThread;
                    }
                }
            }
            catch (SocketException e)
            {
                Messages.AddMessage(MessageType.Error, e.Message);
            }
            finally
            {
                if (socket != null)
                    socket.Close();
                lock (m_SyncObject)
                {
                    m_Listening = false;
                }
            }
        }

        public const int NEEDED_SERVER_VERSION = 2;

        public static ServerInfo GetServerInfo(String text, String token)
        {
            String[] tokens = text.Split(new String[] { token }, StringSplitOptions.None);
            int neededTokens = (token == "|") ? 4 : 3;
            if (tokens.Length < neededTokens)
                return null;
            String name = tokens[0];
            int port = 0;
            if (!Int32.TryParse(tokens[1], out port))
                return null;
            IPAddress address = null;
            if (!IPAddress.TryParse(tokens[2], out address))
                return null;
            if (neededTokens == 4)
            {
                int version = 0;
                if (!Int32.TryParse(tokens[3], out version))
                    return null;
                if (version != NEEDED_SERVER_VERSION)
                    return null;
            }
            return new ServerInfo(name, new IPEndPoint(address, port));
        }

        private void LookForDatagram(Socket socket)
        {
            try
            {
                if (socket.Available > 0)
                {
                    byte[] bytes = new byte[600];
                    int count = socket.Receive(bytes);
                    String receivedData = System.Text.Encoding.UTF8.GetString(bytes, 0, count);
                    Messages.AddMessage(MessageType.Debug, StringResources.UDPReceived + receivedData);
                    ServerInfo server = GetServerInfo(receivedData, "|");
                    if (server == null)
                    {
                        Messages.AddMessage(MessageType.Debug, StringResources.WrongPlayerIgnored);
                    }
                    else if (!m_FoundServers.Contains(server.Name))
                    {
                        m_FoundServers.Add(server.Name);
                        UIThreadDispatcher.DispatchToUIThread(() =>
                            {
                                FireServerFound(server);
                            });
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode != SocketError.TimedOut)
                    throw;
            }
            catch (System.Text.DecoderFallbackException e)
            {
                Messages.AddMessage(MessageType.Warning, e.Message);
            }
        }

        private void FireServerFound(ServerInfo server)
        {
            if (ServerFound != null)
                ServerFound(this, new ServerEventArgs(server));
        }
    }
}
