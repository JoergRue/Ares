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

namespace Ares.Controllers
{
    public class Control
    {
        private static Control s_Instance;

        public static Control Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new Control();
                return s_Instance;
            }
        }

        private Control()
        {
            FileName = String.Empty;
            FilePath = String.Empty;
            m_Project = null;
            m_Connection = null;
            ServerName = String.Empty;
        }

        public String FileName { get; private set; }

        public String FilePath { get; private set; }

        private Configuration m_Project;

        public Configuration Project 
        {
            get
            {
                return m_Project;
            }
        }

        public void SetProject(Configuration project, String fileName)
        {
            m_Project = project;
            FileName = fileName;
        }

        public String ServerName { get; private set; }

        public static readonly String REMOTE_FILE_TAG = "remote://";

        public void OpenFile(String path)
        {
            FilePath = path;
            if (path.StartsWith(REMOTE_FILE_TAG))
                path = path.Substring(REMOTE_FILE_TAG.Length);
            if (IsConnected)
            {
                m_Connection.SendProjectOpenRequest(path, !m_IsLocalPlayer);
            }
            if (Project != null)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                FilePath = file.FullName;
                if (IsConnected)
                {
                    SendKey(System.Windows.Forms.Keys.Escape);
                    m_Connection.SendProjectOpenRequest(FilePath, !m_IsLocalPlayer);
                }
            }
        }

        private ControlConnection m_Connection;

        private bool m_IsLocalPlayer;

        public void Connect(ServerInfo server, INetworkClient client, bool isLocalPlayer)
        {
            if (m_Connection != null)
                Disconnect(true);
            m_Connection = new ControlConnection(server, client);
            m_Connection.Connect();
            ServerName = server.Name;
            m_IsLocalPlayer = isLocalPlayer;
            if (m_Connection != null && !String.IsNullOrEmpty(FilePath))
            {
                String path = FilePath;
                if (path.StartsWith(REMOTE_FILE_TAG))
                    path = path.Substring(REMOTE_FILE_TAG.Length);
                m_Connection.SendProjectOpenRequest(path, !m_IsLocalPlayer);
            }
        }

        public void Disconnect(bool informServer)
        {
            if (m_Connection != null)
            {
                if (m_Connection.Connected)
                {
                    m_Connection.Disconnect(informServer);
                }
                m_Connection.Dispose();
            }
            m_Connection = null;
            ServerName = String.Empty;
            m_IsLocalPlayer = false;
        }

        public bool IsConnected
        {
            get
            {
                return m_Connection != null;
            }
        }

        public void SendPing()
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SendPing();
            }
        }

        public void SendKey(System.Windows.Forms.Keys key)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SendKey(key);
            }
        }

        public void SendKey(byte[] keyCode)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SendKey(keyCode);
            }
        }

        public void SetVolume(int index, int value)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SetVolume(index, value);
            }
        }

        public void SetMusicTitle(int id)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SelectMusicElement(id);
            }
        }

        public void SwitchElement(int id)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SwitchElement(id);
            }
        }

        public void SetMusicRepeat(bool repeat)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SetMusicRepeat(repeat);
            }
        }

        public void SetMusicOnAllSpeakers(bool onAllSpeakers)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SetMusicOnAllSpeakers(onAllSpeakers);
            }
        }

        public void SwitchTag(int categoryId, int tagId, bool active)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SwitchMusicTag(categoryId, tagId, active);
            }
        }

        public void RemoveAllTags()
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.RemoveAllMusicTags();
            }
        }

        public void SetTagCategoriesCombination(int categoryCombination)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SetMusicTagCategoryCombination(categoryCombination);
            }
        }

        public void RequestProjectFiles()
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.RequestProjectFiles();
            }
        }

        public void SetTagFading(int fadeTime, bool onlyOnChange)
        {
            if (m_Connection == null || !m_Connection.Connected)
            {
                Messages.AddMessage(MessageType.Warning, StringResources.NoConnection);
            }
            else
            {
                m_Connection.SetTagFading(fadeTime, onlyOnChange);
            }

        }
    }
}
