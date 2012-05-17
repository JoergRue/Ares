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
    public enum MessageType
    {
        Error,
        Warning,
        Info,
        Debug
    }

    public sealed class Message
    {
        public MessageType Type { get; private set; }

        public String Text { get; private set; }

        public Message(MessageType messageType, String message)
        {
            Type = messageType;
            Text = message;
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Message message)
        {
            Message = message;
        }

        public Message Message { get; private set; }

    }

    public sealed class Messages
    {
        private List<Message> m_Messages;

        private Messages()
        {
            m_Messages = new List<Message>();
        }

        private static Messages s_Instance;

        public static Messages Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new Messages();
                return s_Instance;
            }
        }

        public void Clear()
        {
            m_Messages.Clear();
        }

        public List<Message> GetMessages()
        {
            return new List<Message>(m_Messages);
        }

        public event EventHandler<MessageEventArgs> MessageAdded;

        public void AddMessage(Message message)
        {
            UIThreadDispatcher.DispatchToUIThread(() =>
                {
                    m_Messages.Add(message);
                    if (MessageAdded != null)
                        MessageAdded(this, new MessageEventArgs(message));
                });
        }

        public static void AddMessage(MessageType messageType, String text)
        {
            Instance.AddMessage(new Message(messageType, text));
        }
    }
}
