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
package ares.controller.network;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.InetAddress;
import java.net.Socket;
import java.nio.charset.Charset;
import java.text.NumberFormat;
import java.util.HashMap;

import javax.swing.KeyStroke;
import javax.swing.Timer;

import ares.controller.messages.Messages;
import ares.controller.messages.Message.MessageType;
import ares.controller.util.Localization;

public final class ControlConnection {

  private InetAddress address;
  private int port;
  
  private Socket socket;
  
  private Timer pingTimer;
  
  private INetworkClient networkClient;
  
  public ControlConnection(ServerInfo server, INetworkClient client) {
    address = server.getAddress();
    port = server.getPort();
    networkClient = client;
  }
  
  public void dispose() {
    if (socket != null) {
      disconnect(true);
    }
  }
  
  public void connect() {
    if (socket != null) return;
    try {
      String hostName = InetAddress.getLocalHost().getHostName();
      NumberFormat format = NumberFormat.getIntegerInstance();
      format.setMinimumIntegerDigits(4);
      format.setMaximumIntegerDigits(4);
      format.setGroupingUsed(false);
      String nameLength = format.format(hostName.length());
      String textToSend = nameLength + hostName;
      socket = new Socket(address, port);
      Messages.addMessage(MessageType.Debug, Localization.getString("ControlConnection.SendingInfo") + textToSend); //$NON-NLS-1$
      socket.getOutputStream().write(textToSend.getBytes("UTF8")); //$NON-NLS-1$
      Thread listenThread = new Thread(new Runnable() {
		public void run() {
			listenForStatusUpdates();
		}
      });
      continueListen = true;
      listenThread.start();
      Messages.addMessage(MessageType.Info, Localization.getString("ControlConnection.ConnectedWith") + address + ":" + port); //$NON-NLS-1$ //$NON-NLS-2$
      pingTimer = new Timer(5000, new ActionListener() {
        public void actionPerformed(ActionEvent e) {
          sendPing();
        }
      });
      pingTimer.start();
    }
    catch (IOException e) {
      Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
      networkClient.connectionFailed();
    }
  }
  
  public void disconnect(boolean informServer) {
    if (socket != null) {
      Messages.addMessage(MessageType.Info, Localization.getString("ControlConnection.ClosingConnection")); //$NON-NLS-1$
      pingTimer.stop();
      pingTimer = null;
      try {
        if (socket != null && !socket.isClosed()) {
          if (socket.isConnected() && informServer) {
            socket.getOutputStream().write(1);
            socket.getOutputStream().flush();
          }
          socket.close();
        }
      }
      catch (IOException e) {
        Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
      }
      socket = null;
    }
  }
  
  private boolean continueListen = true;
  
  private static String readString(InputStream stream) throws IOException {
	  int length = stream.read();
	  length *= (1 << 8);
	  length += stream.read();
	  byte[] bytes = new byte[length];
	  stream.read(bytes);
	  return new String(bytes, 0, length, Charset.forName("UTF8")); //$NON-NLS-1$
  }
  
  private void listenForStatusUpdates()
  {
	  boolean goOn = true;
	  while (goOn) {
		  try {
			  InputStream stream = socket.getInputStream();
			  if (stream.available() < 3) {
                try {
					Thread.sleep(50);
				} catch (InterruptedException e) {
				}
			  }
			  if (stream.available() >= 3) {
				  int selector = stream.read(); 
				  switch (selector) {
				  case 0:
				  {
					  networkClient.modeChanged(readString(stream));
					  break;
				  }
				  case 1:
				  {
					  int id = stream.read();
					  id *= (1 << 8);
					  id += stream.read();
					  boolean active = stream.read() == 1;
					  if (active) {
						  networkClient.modeElementStarted(id);
					  }
					  else {
						  networkClient.modeElementStopped(id);
					  }
					  break;
				  }
				  case 2:
				  {
					  networkClient.musicChanged(readString(stream));
					  break;
				  }
				  case 3:
				  {
					  ares.controller.messages.Messages.addMessage(MessageType.Error, readString(stream));
					  break;
				  }
				  case 4:
					  int index = stream.read();
					  int volume = stream.read();
					  networkClient.volumeChanged(index, volume);
					  break;
				  case 5:
					  stream.read();
					  stream.read();
					  networkClient.disconnect();
					  synchronized (this) {
						  continueListen = false;
					  }
					  break;
				  default:
					  break;
				  }
			  }
		  }
		  catch (IOException e) {
			  ares.controller.messages.Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
		      networkClient.connectionFailed();
			  break;
		  }
		  synchronized(this) {
			  goOn = continueListen;
		  }
	  }
  }
  
  public boolean isConnected() {
    return socket != null;
  }
  
  public void sendKey(KeyStroke keyStroke) {
    if (socket == null) {
      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
      return;
    }
    byte[] bytes = new byte[3];
    bytes[0] = 0;
    if (MapKeyStrokeToBytes(keyStroke, bytes))
    {
      Messages.addMessage(MessageType.Info, Localization.getString("ControlConnection.SendingKeystroke") + keyStroke); //$NON-NLS-1$
      Messages.addMessage(MessageType.Debug, Localization.getString("ControlConnection.SendingBytes") + bytes[0] + " " + bytes[1] + " " + bytes[2]); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
      try {
        socket.getOutputStream().write(bytes);
      }
      catch (IOException e) {
        Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
        networkClient.connectionFailed();
      }
    }
    else 
    {
      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.UnsupportedKeyStroke") + keyStroke); //$NON-NLS-1$
    }
  }
  
  public void setVolume(int index, int value) {
	  if (socket == null) {
		  Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
		  return;
	  }
	  byte[] bytes = new byte[3];
	  bytes[0] = 2;
	  bytes[1] = (byte)index;
	  bytes[2] = (byte)value;	  
	  Messages.addMessage(MessageType.Debug, Localization.getString("ControlConnection.SendingBytes") + bytes[0] + " " + bytes[1] + " " + bytes[2]); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$)
	  try {
		  socket.getOutputStream().write(bytes);
	  }
      catch (IOException e) {
          Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
          networkClient.connectionFailed();
      }	  
  }
  
  private void sendPing() {
    if (socket == null) {
      return;
    }
    try {
      Messages.addMessage(MessageType.Debug, Localization.getString("ControlConnection.SendingPing")); //$NON-NLS-1$
      socket.getOutputStream().write(5);
    }
    catch (IOException e) {
      Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
      networkClient.connectionFailed();
    }
  }
  
  private HashMap<KeyStroke, byte[]> commandMap = null;
  
  private void createCommandMap() {
    commandMap = new HashMap<KeyStroke, byte[]>();
    try {
      KeyStroke keyStroke;
      byte[] bytes;
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_A, 0);
      bytes = new byte[2];
      bytes[0] = 0;
      bytes[1] = "A".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_B, 0);
      bytes[0] = 0;
      bytes[1] = "B".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_C, 0);
      bytes[0] = 0;
      bytes[1] = "C".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_D, 0);
      bytes[0] = 0;
      bytes[1] = "D".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_E, 0);
      bytes[0] = 0;
      bytes[1] = "E".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F, 0);
      bytes[0] = 0;
      bytes[1] = "F".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_G, 0);
      bytes[0] = 0;
      bytes[1] = "G".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_H, 0);
      bytes[0] = 0;
      bytes[1] = "H".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_I, 0);
      bytes[0] = 0;
      bytes[1] = "I".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_J, 0);
      bytes[0] = 0;
      bytes[1] = "J".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_K, 0);
      bytes[0] = 0;
      bytes[1] = "K".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_L, 0);
      bytes[0] = 0;
      bytes[1] = "L".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_M, 0);
      bytes[0] = 0;
      bytes[1] = "M".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_N, 0);
      bytes[0] = 0;
      bytes[1] = "N".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_O, 0);
      bytes[0] = 0;
      bytes[1] = "O".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_P, 0);
      bytes[0] = 0;
      bytes[1] = "P".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_Q, 0);
      bytes[0] = 0;
      bytes[1] = "Q".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_R, 0);
      bytes[0] = 0;
      bytes[1] = "R".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_S, 0);
      bytes[0] = 0;
      bytes[1] = "S".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_T, 0);
      bytes[0] = 0;
      bytes[1] = "T".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_U, 0);
      bytes[0] = 0;
      bytes[1] = "U".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_V, 0);
      bytes[0] = 0;
      bytes[1] = "V".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_W, 0);
      bytes[0] = 0;
      bytes[1] = "W".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_X, 0);
      bytes[0] = 0;
      bytes[1] = "X".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_Y, 0);
      bytes[0] = 0;
      bytes[1] = "Y".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_Z, 0);
      bytes[0] = 0;
      bytes[1] = "Z".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_1, 0);
      bytes[0] = 0;
      bytes[1] = "1".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_2, 0);
      bytes[0] = 0;
      bytes[1] = "2".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_3, 0);
      bytes[0] = 0;
      bytes[1] = "3".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_4, 0);
      bytes[0] = 0;
      bytes[1] = "4".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_5, 0);
      bytes[0] = 0;
      bytes[1] = "5".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_6, 0);
      bytes[0] = 0;
      bytes[1] = "6".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_7, 0);
      bytes[0] = 0;
      bytes[1] = "7".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_8, 0);
      bytes[0] = 0;
      bytes[1] = "8".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_9, 0);
      bytes[0] = 0;
      bytes[1] = "9".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_0, 0);
      bytes[0] = 0;
      bytes[1] = "0".getBytes("ASCII")[0]; //$NON-NLS-1$ //$NON-NLS-2$
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F1, 0);
      bytes[0] = 1;
      bytes[1] = 1;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F2, 0);
      bytes[0] = 1;
      bytes[1] = 2;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F3, 0);
      bytes[0] = 1;
      bytes[1] = 3;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F4, 0);
      bytes[0] = 1;
      bytes[1] = 4;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F5, 0);
      bytes[0] = 1;
      bytes[1] = 5;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F6, 0);
      bytes[0] = 1;
      bytes[1] = 6;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F7, 0);
      bytes[0] = 1;
      bytes[1] = 7;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F8, 0);
      bytes[0] = 1;
      bytes[1] = 8;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F9, 0);
      bytes[0] = 1;
      bytes[1] = 9;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F10, 0);
      bytes[0] = 1;
      bytes[1] = 10;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F11, 0);
      bytes[0] = 1;
      bytes[1] = 11;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_F12, 0);
      bytes[0] = 1;
      bytes[1] = 12;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD0, 0);
      bytes[0] = 2;
      bytes[1] = 0;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD1, 0);
      bytes[0] = 2;
      bytes[1] = 1;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD2, 0);
      bytes[0] = 2;
      bytes[1] = 2;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD3, 0);
      bytes[0] = 2;
      bytes[1] = 3;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD4, 0);
      bytes[0] = 2;
      bytes[1] = 4;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD5, 0);
      bytes[0] = 2;
      bytes[1] = 5;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD6, 0);
      bytes[0] = 2;
      bytes[1] = 6;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD7, 0);
      bytes[0] = 2;
      bytes[1] = 7;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD8, 0);
      bytes[0] = 2;
      bytes[1] = 8;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_NUMPAD9, 0);
      bytes[0] = 2;
      bytes[1] = 9;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_INSERT, 0);
      bytes[0] = 3;
      bytes[1] = 0;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_DELETE, 0);
      bytes[0] = 3;
      bytes[1] = 1;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_HOME, 0);
      bytes[0] = 3;
      bytes[1] = 2;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_END, 0);
      bytes[0] = 3;
      bytes[1] = 3;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_PAGE_UP, 0);
      bytes[0] = 3;
      bytes[1] = 4;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_PAGE_DOWN, 0);
      bytes[0] = 3;
      bytes[1] = 5;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_LEFT, 0);
      bytes[0] = 3;
      bytes[1] = 6;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_RIGHT, 0);
      bytes[0] = 3;
      bytes[1] = 7;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_UP, 0);
      bytes[0] = 3;
      bytes[1] = 8;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_DOWN, 0);
      bytes[0] = 3;
      bytes[1] = 9;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_SPACE, 0);
      bytes[0] = 3;
      bytes[1] = 10;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_ENTER, 0);
      bytes[0] = 3;
      bytes[1] = 11;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0);
      bytes[0] = 3;
      bytes[1] = 12;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_TAB, 0);
      bytes[0] = 3;
      bytes[1] = 13;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_BACK_SPACE, 0);
      bytes[0] = 3;
      bytes[1] = 14;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_PERIOD, 0);
      bytes[0] = 3;
      bytes[1] = 15;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_SEMICOLON, 0);
      bytes[0] = 3;
      bytes[1] = 16;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_COLON, 0);
      bytes[0] = 3;
      bytes[1] = 17;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_COMMA, 0);
      bytes[0] = 3;
      bytes[1] = 18;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_UNDERSCORE, 0);
      bytes[0] = 3;
      bytes[1] = 19;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_MINUS, 0);
      bytes[0] = 3;
      bytes[1] = 20;
      commandMap.put(keyStroke, bytes);
      bytes = new byte[2];
      keyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_SLASH, 0);
      bytes[0] = 3;
      bytes[1] = 21;
      commandMap.put(keyStroke, bytes);
    }
    catch (UnsupportedEncodingException e) {
      Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
    }
  }

  private boolean MapKeyStrokeToBytes(KeyStroke keyStroke, byte[] bytes) {
    if (commandMap == null) {
      createCommandMap();
    }
    if (commandMap.containsKey(keyStroke)) {
      byte[] commandBytes = commandMap.get(keyStroke);
      bytes[1] = commandBytes[0];
      bytes[2] = commandBytes[1];
      return true;
    }
    else
      return false;
  }
  
  
}
