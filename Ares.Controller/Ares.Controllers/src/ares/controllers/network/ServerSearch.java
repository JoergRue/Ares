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
package ares.controllers.network;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;
import java.util.StringTokenizer;

import ares.controllers.messages.Messages;
import ares.controllers.messages.Message.MessageType;
import ares.controllers.util.UIThreadDispatcher;
import ares.controllers.util.Localization;


public final class ServerSearch {

  private int port;
  private IServerListener callback;
  
  private boolean listening;
  private boolean continueThread;
  private Thread listenerThread;
  
  public ServerSearch(IServerListener listener, int port) {
    this.port = port;
    callback = listener;
    listening = false;
    continueThread = true;
  }
  
  public void dispose() {
    boolean isListening = false;
    synchronized(this) {
      isListening = listening;
    }
    if (isListening) {
      stopSearch();
    }
  }
  
  public void startSearch() {
    synchronized(this) {
      if (listening) return;
      listening = true;
    }
    Messages.addMessage(MessageType.Info, Localization.getString("ServerSearch.StartServerSearch")); //$NON-NLS-1$
    continueThread = true;
    listenerThread = new Thread(new ListenerThread());
    listenerThread.start();
  }
  
  public void stopSearch() {
    synchronized(this) {
      if (!listening) return;
      continueThread = false;
    }
    Messages.addMessage(MessageType.Info, Localization.getString("ServerSearch.StopServerSearch")); //$NON-NLS-1$
    try {
      if (listenerThread.isAlive()) listenerThread.join();
    }
    catch (InterruptedException e) {
      // ignored
    }
    listenerThread = null;
  }
  
  public boolean isSearching() {
    synchronized(this) {
      return listening;
    }
  }
  
  private class ListenerThread implements Runnable {
    
    public void run() {
      DatagramSocket socket = null;
      try {
        socket = new DatagramSocket(port);
        socket.setSoTimeout(50);
        boolean goOn = true;
        synchronized(this) {
          goOn = continueThread;
        }
        while (goOn) {
          lookForDatagram(socket);
          try {
            Thread.sleep(100);
          }
          catch (InterruptedException e) {
            // ignored
          }
          synchronized(this) {
            goOn = continueThread;
          }
        }
      }
      catch (SocketException ex) {
        Messages.addMessage(MessageType.Error, ex.getLocalizedMessage());
      }
      finally {
        if (socket != null) socket.close();
        synchronized(this) {
          listening = false;
        }
      }
    }
  }
  
  public static final int NEEDED_SERVER_VERSION = 3;
  
  public static ServerInfo getServerInfo(String text, String token) throws UnknownHostException {
      StringTokenizer tokenizer = new StringTokenizer(text, token); //$NON-NLS-1$
      int neededTokens = (token == "|") ? 4 : 3; //$NON-NLS-1$
      if (tokenizer.countTokens() < neededTokens) {
    	  return null;
      }
      String name = tokenizer.nextToken();
      int port = Integer.parseInt(tokenizer.nextToken());
      InetAddress address = InetAddress.getByName(tokenizer.nextToken());
      if (neededTokens == 4)
      {
    	  int version = Integer.parseInt(tokenizer.nextToken());
    	  if (version != NEEDED_SERVER_VERSION)
    		  return null;
      }
      return new ServerInfo(address, port, name);
  }
  
  private void lookForDatagram(DatagramSocket socket) {
    try {
      byte[] bytes = new byte[600];
      DatagramPacket packet = new DatagramPacket(bytes, 600);
      socket.receive(packet);
      byte[] receivedBytes = new byte[packet.getLength()];
      for (int i = 0; i < packet.getLength(); ++i) {
    	  receivedBytes[i] = bytes[i];
      }
      String receivedData = new String(receivedBytes, "UTF8"); //$NON-NLS-1$
      Messages.addMessage(MessageType.Debug, Localization.getString("ServerSearch.UDPReceived") + receivedData); //$NON-NLS-1$
      ServerInfo server1 = null;
      try {
    	  server1 = getServerInfo(receivedData, "|"); //$NON-NLS-1$
    	  if (server1 == null) {
    		  Messages.addMessage(MessageType.Debug, Localization.getString("ServerSearch.IgnoringPlayerWithWrongVersion")); //$NON-NLS-1$
    		  return;
    	  }
      }
      catch (NumberFormatException e) {
          Messages.addMessage(MessageType.Warning, Localization.getString("ServerSearch.InvalidPort")); //$NON-NLS-1$
          return;
        }
      catch (IllegalArgumentException e) {
        Messages.addMessage(MessageType.Warning, Localization.getString("ServerSearch.WrongUDPReceived")); //$NON-NLS-1$
        return;
      }
      catch (UnknownHostException e) {
        Messages.addMessage(MessageType.Warning, Localization.getString("ServerSearch.InvalidAddress")); //$NON-NLS-1$
      }
      final ServerInfo server = server1;
      UIThreadDispatcher.dispatchToUIThread(new Runnable() {
        public void run() {
          if (callback != null) {
            callback.serverFound(server);
          }
        }
      });
    }
    catch (SocketTimeoutException e) {
      return;
    }
    catch (IOException e) {
      Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
      return;
    }
  }
}
