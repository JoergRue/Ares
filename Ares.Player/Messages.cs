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
using System.Collections.Generic;
using System.Text;

namespace Ares.Player
{
    enum MessageType
    {
        Debug = 0, Info = 1, Warning = 2, Error = 3
    }

    struct Message
    {
        public MessageType Type { get; set; }
        public String Text { get; set; }

        public Message(MessageType type, String text)
            : this()
        {
            Type = type;
            Text = text;
        }
    }

    delegate void MessageReceivedHandler(Message m);

    sealed class Messages
    {
        public static Messages Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new Messages();
                }
                return sInstance;
            }
        }

        private static Messages sInstance = null;

        private List<Message> mMessages = null;

        public IList<Message> GetAllMessages() { lock (this) { return new List<Message>(mMessages); } }

        private Messages()
        {
            mMessages = new List<Message>();
        }

        public event MessageReceivedHandler MessageReceived;

        private void AddMessage(Message m)
        {
            lock (this)
            {
                mMessages.Add(m);
            }
            if (MessageReceived != null)
            {
                MessageReceived(m);
            }
        }

        public static void AddMessage(MessageType type, String text)
        {
            Instance.AddMessage(new Message(type, text));
        }
    }
}
