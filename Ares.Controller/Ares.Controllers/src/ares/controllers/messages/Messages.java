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
package ares.controllers.messages;

import java.util.ArrayList;
import java.util.List;

import ares.controllers.util.AbstractObservable;
import ares.controllers.util.UIThreadDispatcher;

public final class Messages extends AbstractObservable<IMessageListener> {
  
  private List<Message> messages;
  
  private Messages() {
    messages = new ArrayList<Message>();
  }
  
  private static Messages sInstance = null;
  
  public static Messages getInstance() {
    if (sInstance == null) {
      sInstance = new Messages();
    }
    return sInstance;
  }
  
  public void clear() {
    messages.clear();
  }
  
  public List<Message> getMessages() {
    List<Message> result = new ArrayList<Message>();
    result.addAll(messages);
    return result;
  }
  
  private void fireMessageAdded(final Message message) {
    List<IMessageListener> observersCopy = new ArrayList<IMessageListener>();
    observersCopy.addAll(observers);
    for (IMessageListener listener : observersCopy) {
      listener.messageAdded(message);
    }
  }
  
  public void addMessage(final Message message) {
    UIThreadDispatcher.dispatchToUIThread(new Runnable() {
      public void run() {
        messages.add(message);
        fireMessageAdded(message);
      }
    });
  }
  
  public static void addMessage(Message.MessageType type, String text) {
    getInstance().addMessage(new Message(type, text));
  }
  
}
