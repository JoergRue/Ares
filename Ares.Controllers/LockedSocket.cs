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
using System.Net;
using System.Net.Sockets;

namespace Ares.Controllers
{
    class LockedSocket
    {
        private Socket m_Socket;

        private Object m_SyncObject = new Int32();

        public LockedSocket(Socket socket)
        {
            m_Socket = socket;
        }

        public bool Connected
        {
            get
            {
                lock (m_SyncObject)
                {
                    return m_Socket != null && m_Socket.Connected;
                }
            }
        }

        public void Disconnect(bool informServer)
        {
            lock (m_SyncObject)
            {
                if (m_Socket != null && m_Socket.Connected)
                {
                    if (informServer)
                    {
                        m_Socket.Send(new byte[] { 1 });
                    }
                    m_Socket.Close();
                    m_Socket = null;
                }
            }
        }

        public void Close()
        {
            lock (m_SyncObject)
            {
                if (m_Socket != null)
                {
                    m_Socket.Close();
                    m_Socket = null;
                }
            }
        }

        public bool Connect(IPEndPoint endPoint, int timeout)
        {
            System.Threading.Monitor.Enter(m_SyncObject);
            try
            {
                if (m_Socket != null)
                {
                    IAsyncResult result = m_Socket.BeginConnect(endPoint, null, null);
                    System.Threading.Monitor.Exit(m_SyncObject);
                    try
                    {
                        result.AsyncWaitHandle.WaitOne(timeout, true);
                    }
                    finally
                    {
                        System.Threading.Monitor.Enter(m_SyncObject);
                    }
                    if (m_Socket == null)
                        return false;
                    else if (!m_Socket.Connected)
                    {
                        m_Socket.Close();
                        m_Socket = null;
                        return false;
                    }
                    else
                    {
                        m_Socket.EndConnect(result);
                        return true;
                    }
                }
                else
                    return false;
            }
            finally
            {
                System.Threading.Monitor.Exit(m_SyncObject);
            }
        }


        public int ReceiveTimeout
        {
            set
            {
                lock (m_SyncObject)
                {
                    if (m_Socket != null)
                        m_Socket.ReceiveTimeout = value;
                }
            }
        }

        public void Send(Byte[] bytes)
        {
            lock (m_SyncObject)
            {
                if (m_Socket != null)
                    m_Socket.Send(bytes);
            }
        }

        public int Available
        {
            get
            {
                lock (m_SyncObject)
                {
                    if (m_Socket != null)
                        return m_Socket.Available;
                    else
                        return 0;
                }
            }
        }

        public int Receive(Byte[] bytes)
        {
            lock (m_SyncObject)
            {
                if (m_Socket != null)
                    return m_Socket.Receive(bytes);
                else
                    return 0;
            }
        }

        public int Receive(Byte[] bytes, int offset, int length)
        {
            lock (m_SyncObject)
            {
                if (m_Socket != null)
                    return m_Socket.Receive(bytes, offset, length, SocketFlags.None);
                else
                    return 0;
            }
        }

        public bool Receive(Byte[] bytes, int timeout)
        {
            System.Threading.Monitor.Enter(m_SyncObject);
            try
            {
                if (m_Socket != null)
                {
                    IAsyncResult result = m_Socket.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, null, null);
                    if (result == null)
                        return false;
                    System.Threading.Monitor.Exit(m_SyncObject);
                    bool success = false;
                    try
                    {
                        success = result.AsyncWaitHandle.WaitOne(timeout, true);
                    }
                    finally
                    {
                        System.Threading.Monitor.Enter(m_SyncObject);
                    }
                    if (m_Socket != null)
                        m_Socket.EndReceive(result);
                    return success;
                }
                else
                    return false;
            }
            finally
            {
                System.Threading.Monitor.Exit(m_SyncObject);
            }
        }

    }
}
