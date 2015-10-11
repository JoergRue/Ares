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

import java.awt.event.KeyEvent;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

import ares.controllers.data.Command;
import ares.controllers.data.Configuration;
import ares.controllers.data.KeyStroke;
import ares.controllers.data.Mode;
import ares.controllers.data.TitledElement;
import ares.controllers.messages.Messages;
import ares.controllers.messages.Message.MessageType;
import ares.controllers.util.Localization;
import ares.controllers.util.UIThreadDispatcher;

public final class ControlConnection {

  private InetAddress address;
  private int port;
  
  private Socket socket;
  
  private INetworkClient networkClient;
  
  private Timer pingTimer;
  
  private Timer reconnectTimer;
  
  private Timer watchDogTimer;
  
  private enum State { NotConnected, Connected, ConnectionFailure };
  
  private State state;
  
  public ControlConnection(ServerInfo server, INetworkClient client) {
    address = server.getAddress();
    port = server.getTcpPort();
    networkClient = client;
    state = State.NotConnected;
  }
  
  public void dispose() {
    if (socket != null) {
      disconnect(true);
    }
  }
  
  private Thread listenThread;
  
  private boolean checkedVersion = false;
  
  private void doConnect(int timeout) throws SocketTimeoutException, IOException {
      String hostName = InetAddress.getLocalHost().getHostName();
      if (hostName.equals("localhost")) {
    	 String vendor = System.getProperties().getProperty("java.vendor");
    	 if (vendor != null && vendor.contains("Android")) {
    		 hostName = "Android Controller";
    	 }
      }
      NumberFormat format = NumberFormat.getIntegerInstance();
      format.setMinimumIntegerDigits(4);
      format.setMaximumIntegerDigits(4);
      format.setGroupingUsed(false);
      String nameLength = format.format(hostName.length());
      String textToSend = nameLength + hostName;
      socket = new Socket();
      socket.connect(new InetSocketAddress(address, port), timeout);
      Messages.addMessage(MessageType.Debug, Localization.getString("ControlConnection.SendingInfo") + textToSend); //$NON-NLS-1$
      socket.getOutputStream().write(textToSend.getBytes("UTF8")); //$NON-NLS-1$	  
      checkedVersion = false;
      listenThread = new Thread(new Runnable() {
  		public void run() {
  			listenForStatusUpdates();
  		}
	  });
	  continueListen = true;
	  listenThread.start();
	  pingTimer = new Timer("PingTimer"); //$NON-NLS-1$
	  pingTimer.scheduleAtFixedRate(new TimerTask() {
	    public void run() {
		  UIThreadDispatcher.dispatchToUIThread(new Runnable() {
			public void run() {
				sendPing();
			}
		  });
	    }
	  }, 5000, 5000);
	  watchDogTimer = new Timer("WatchdogTimer"); //$NON-NLS-1$
	  watchDogTimer.schedule(new TimerTask() {
		  public void run() {
			  Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoPingFailure")); //$NON-NLS-1$
			  handleConnectionFailure(false);
		  }
	  }, 25000);
	  state = State.Connected;
      Messages.addMessage(MessageType.Info, Localization.getString("ControlConnection.ConnectedWith") + address + ":" + port); //$NON-NLS-1$ //$NON-NLS-2$
  }
  
  public void connect() {
    if (socket != null) return;
    try {
      doConnect(5000);
    }
    catch (SocketTimeoutException e) {
        Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
        networkClient.connectionFailed();    	
    }
    catch (IOException e) {
      Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
      networkClient.connectionFailed();
    }
  }
  
  private boolean tryReconnect() {
	  if (state != State.ConnectionFailure)
		  return false;
	  try {
		  doConnect(2000);
		  if (reconnectTimer != null) {
			  reconnectTimer.cancel();
			  reconnectTimer = null;
		  }
		  return true;
	  }
	  catch (SocketTimeoutException e) {
		  return false;
	  }
	  catch (IOException e) {
		  return false;
	  }
  }
  
  private void doDisconnect(boolean stopListenThread) {
	if (pingTimer != null) {
		pingTimer.cancel();
		pingTimer = null;
	}
	if (watchDogTimer != null) {
		watchDogTimer.cancel();
		watchDogTimer = null;
	}
	if (stopListenThread) {
		synchronized(this) {
			continueListen = false;
		}
		try {
			if (listenThread != null) 
				listenThread.join();
			listenThread = null;
		}
		catch (InterruptedException e) {
		}
	}
	checkedVersion = false;
  }
  
  public void disconnect(boolean informServer) {
	if (state == State.ConnectionFailure)
	{
		if (reconnectTimer != null) {
			reconnectTimer.cancel();
			reconnectTimer = null;
		}
		return;
	}
	else if (state == State.NotConnected)
	{
		return;
	}
	else if (informServer && reconnectTimer != null) {
		reconnectTimer.cancel();
		reconnectTimer = null;
	}
    state = State.NotConnected;
	doDisconnect(true);
    if (socket != null) {
      Messages.addMessage(MessageType.Info, Localization.getString("ControlConnection.ClosingConnection")); //$NON-NLS-1$
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
    state = State.NotConnected;
  }
  
  private void handleConnectionFailure(boolean stopListenThread) {
	doDisconnect(stopListenThread);
	try {
		if (socket != null) {
			socket.close();
		}
	}
	catch (IOException e) {
	}
	socket = null;
    reconnectTimer = new Timer("ReconnectTimer"); //$NON-NLS-1$
	reconnectTimer.scheduleAtFixedRate(new TimerTask() {
       public void run() {
			tryReconnect();
	    }
	}, 200, 3000);
	state = State.ConnectionFailure;
  }
  
  private boolean continueListen = true;
  
  private static String readString(InputStream stream) throws IOException {
	  int length = stream.read();
	  length *= (1 << 8);
	  length += stream.read();
	  byte[] bytes = new byte[length];
	  stream.read(bytes);
	  return new String(bytes, "UTF8"); //$NON-NLS-1$
  }
  
  private static int readInt32(InputStream stream) throws IOException {
	  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.allocate(4);
	  stream.read(buffer.array());
	  return buffer.getInt(0);
  }
  
  private static TitledElement readTitledElement(InputStream stream) throws IOException {
	  int id = readInt32(stream);
	  return new TitledElement(readString(stream), id); 	  
  }
  
  private void listenForStatusUpdates()
  {
	  boolean goOn = true;
	  while (goOn) {
		  try {
			  if (socket == null) {
				  handleConnectionFailure(false);
				  goOn = false;
				  break;
			  }
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
					  networkClient.musicChanged(readString(stream), readString(stream));
					  break;
				  }
				  case 3:
				  {
					  ares.controllers.messages.Messages.addMessage(MessageType.Error, readString(stream));
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
				  case 6:
				  {
					  // networkClient.projectChanged(readString(stream));
					  break;
				  }
				  case 7:
				  {
					  stream.read();
					  stream.read();
					  networkClient.allModeElementsStopped();
					  break;
				  }
				  case 8:
				  {
					  int subcommand = stream.read();
					  stream.read();
					  if (subcommand == 0) {
						  currentMusicList = new ArrayList<TitledElement>();
					  }
					  else if (subcommand == 1) {
						  currentMusicList.add(readTitledElement(stream));
					  }
					  else if (subcommand == 2) {
						  networkClient.musicListChanged(currentMusicList);
					  }
					  break;
				  }
				  case 9:
				  {
    				    // ping
					    int version = stream.read();
						version *= (1 << 8);
						version += stream.read();
						if (watchDogTimer != null) {
							watchDogTimer.cancel();
						}
						if (!checkedVersion && version != ServerSearch.NEEDED_SERVER_VERSION) {
							Messages.addMessage(MessageType.Error, Localization.getString("ControlConnection.PlayerHasWrongVersion")); //$NON-NLS-1$
							networkClient.connectionFailed();
							break;
						}
						else {
							checkedVersion = true;
						}
						watchDogTimer = new Timer("WatchdogTimer"); //$NON-NLS-1$
						watchDogTimer.schedule(new TimerTask() {
							  public void run() {
								  Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoPingFailure")); //$NON-NLS-1$
								  handleConnectionFailure(false);
							  }
						}, 7000);
						break;
				  }
				  case 10:
				  {
					  // repeat music
					  int val = stream.read();
					  stream.read();
					  boolean isRepeat = (val == 1);
					  networkClient.musicRepeatChanged(isRepeat);
					  break;
				  }
				  case 11:
				  {
					  // new tags
					  int subcommand = stream.read();
					  stream.read();
					  if (subcommand == 0)
					  {
						  // start list
						  currentCategoryList = new ArrayList<TitledElement>();
						  currentTags = new HashMap<Integer, List<TitledElement>>();
						  currentTagList = new ArrayList<TitledElement>();
					  }
					  else if (subcommand == 1)
					  {
						  // new category
						  TitledElement category = readTitledElement(stream);
						  currentCategoryList.add(category);
						  currentTagList = new ArrayList<TitledElement>();
						  currentTags.put(category.getId(), currentTagList);
					  }
					  else if (subcommand == 2)
					  {
						  // new tag
						  currentTagList.add(readTitledElement(stream));
					  }
					  else if (subcommand == 3)
					  {
						  // done
						  networkClient.tagsChanged(currentCategoryList, currentTags);
					  }
					  break;
				  }
				  case 12:
				  {
					  // new active tags
					  int subcommand = stream.read();
					  stream.read();
					  if (subcommand == 0)
					  {
						  currentActiveTagList = new ArrayList<Integer>();
					  }
					  else if (subcommand == 1)
					  {
						  currentActiveTagList.add(readInt32(stream));
					  }
					  else if (subcommand == 2)
					  {
						  networkClient.activeTagsChanged(currentActiveTagList);
					  }
					  break;
				  }
				  case 13:
				  {
					  boolean isActive = stream.read() == 1;
					  stream.read();
					  int tagId = readInt32(stream);
					  networkClient.tagSwitched(tagId, isActive);
					  break;
				  }
				  case 14:
				  {
					  int newCombination = stream.read();
					  if (newCombination < 0 || newCombination > 2)
						  newCombination = 0;
					  INetworkClient.CategoryCombination categoryCombination = INetworkClient.CategoryCombination.values()[newCombination];
					  stream.read();
					  networkClient.tagCategoryCombinationChanged(categoryCombination);
					  break;
				  }
				  case 15:
				  {
					  // project files
					  int subCommand = stream.read();
					  stream.read();
					  if (subCommand == 0) {
						  currentProjectFileList = new ArrayList<String>();
					  }
					  else if (subCommand == 1) {
						  readInt32(stream);
						  currentProjectFileList.add(readString(stream));
					  }
					  else if (subCommand == 2) {
						  networkClient.projectFilesRetrieved(currentProjectFileList);
					  }
					  break;
				  }
				  case 16:
				  {
					  // new project structure
					  int subcommand = stream.read();
					  int second = stream.read();
					  if (subcommand == 0) {
						  if (second == 1) {
							  currentConfiguration = null;
							  networkClient.configurationChanged(currentConfiguration, "");
						  }
						  else {
							  currentConfiguration = new Configuration();
							  currentConfiguration.setTitle(readString(stream));
							  currentConfigFileName = readString(stream);
						  }
					  }
					  else if (subcommand == 1) {
						  String name = readString(stream);
						  byte[] bytes = new byte[2];
						  bytes[0] = (byte)stream.read();
						  bytes[1] = (byte)stream.read();
						  KeyStroke stroke = MapBytesToKeyStroke(bytes, 0);
						  currentMode = new Mode(name, stroke != null ? stroke.getKeyCode() : 0, stroke);
						  if (currentConfiguration != null)
							  currentConfiguration.addMode(currentMode);
					  }
					  else if (subcommand == 2) {
						  String name = readString(stream);
						  int id = readInt32(stream);
						  byte[] bytes = new byte[2];
						  bytes[0] = (byte)stream.read();
						  bytes[1] = (byte)stream.read();
						  KeyStroke stroke = MapBytesToKeyStroke(bytes, 0);
						  if (currentMode != null)
							  currentMode.addCommand(new Command(name, id, stroke));
					  }
					  else if (subcommand == 3) {
						  networkClient.configurationChanged(currentConfiguration, currentConfigFileName);
					  }
					  break;
				  }
				  case 17:
				  {
					  boolean fadeOnlyOnChange = stream.read() == 1;
					  stream.read();
					  int fadeTime = readInt32(stream);
					  networkClient.musicTagFadingChanged(fadeTime, fadeOnlyOnChange);
					  break;
				  }
				  case 18:
				  {
					  boolean onAllSpeakers = stream.read() == 1;
					  stream.read();
					  networkClient.musicOnAllSpeakersChanged(onAllSpeakers);
					  break;
				  }
				  case 19:
				  {
					  int fadingOption = stream.read();
					  if (fadingOption < 0 || fadingOption > 2)
						  fadingOption = 0;
					  stream.read();
					  int fadingTime = readInt32(stream);
					  if (fadingTime < 0 || fadingTime > 30000)
						  fadingTime = 0;
					  networkClient.musicFadingChanged(fadingOption, fadingTime);
					  break;
				  }
				  default:
					  break;
				  }
			  }
		  }
		  catch (IOException e) {
			  ares.controllers.messages.Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
			  handleConnectionFailure(false);
			  break;
		  }
		  synchronized(this) {
			  goOn = continueListen;
		  }
	  }
  }
  
  private ArrayList<TitledElement> currentMusicList = new ArrayList<TitledElement>();
  
  private ArrayList<TitledElement> currentCategoryList = new ArrayList<TitledElement>();
  private HashMap<Integer, List<TitledElement>> currentTags = new HashMap<Integer, List<TitledElement>>();
  private ArrayList<TitledElement> currentTagList = new ArrayList<TitledElement>();
  private ArrayList<Integer> currentActiveTagList = new ArrayList<Integer>();
  
  private ArrayList<String> currentProjectFileList = new ArrayList<String>();
  
  private Configuration currentConfiguration = null;
  private Mode currentMode = null;
  private String currentConfigFileName = "";
  
  public boolean isConnected() {
    return state != State.NotConnected;
  }
  
  public void sendKey(KeyStroke keyStroke) {
    if (!isConnected()) {
      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
      return;
    }
    if (state == State.ConnectionFailure) {
    	if (!tryReconnect()) {
    	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
    	      return;    		
    	}
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
        Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
        handleConnectionFailure(true);
      }
    }
    else 
    {
      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.UnsupportedKeyStroke") + keyStroke); //$NON-NLS-1$
    }
  }
  
  public void setVolume(int index, int value) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
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
          Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
      }	  
  }
  
  public void sendPing() {
    if (state != State.Connected) {
      return;
    }
    try {
      Messages.addMessage(MessageType.Debug, Localization.getString("ControlConnection.SendingPing")); //$NON-NLS-1$
		if (socket != null)
      		socket.getOutputStream().write(5);
    }
    catch (IOException e) {
      Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
      handleConnectionFailure(true);
    }
  }
  
  public void sendProjectOpenRequest(String projectName, boolean stripPath) {
	  if (state != State.Connected) {
		  return;
	  }
	  if (stripPath) {
		  File file = new File(projectName);
		  projectName = file.getName();
	  }
	  try {
		  byte[] utf8Name = projectName.getBytes("UTF8"); //$NON-NLS-1$
		  byte[] bytes = new byte[3 + utf8Name.length];
		  bytes[0] = 6;
		  bytes[1] = (byte)(utf8Name.length / (1 << 8));
		  bytes[2] = (byte)(utf8Name.length % (1 << 8));
		  for (int i = 0; i < utf8Name.length; ++i) {
			  bytes[3 + i] = utf8Name[i];
		  }
		  socket.getOutputStream().write(bytes);
	  }
      catch (IOException e) {
          Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
      }	  	  
  }
  
  public void selectMusicElement(int elementId) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 4];
		  bytes[0] = 7;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, elementId);
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }
  }
  
  public void switchElement(int elementId) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 4];
		  bytes[0] = 8;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, elementId);
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }
  }
  
  public void setMusicRepeat(boolean repeat) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 4];
		  bytes[0] = 9;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, repeat ? 1 : 0);
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }	  
  }
  
  public void setMusicOnAllSpeakers(boolean onAllSpeakers) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 4];
		  bytes[0] = 15;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, onAllSpeakers ? 1 : 0);
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }	  	  
  }
  
  public void setMusicFading(int fadingOption, int fadingTime) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 4 + 4];
		  bytes[0] = 16;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, fadingOption);
		  buffer.putInt(1 + 4, fadingTime);
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }	  	  	  
  }
  
  public void switchTag(int categoryId, int tagId, boolean active) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 4 + 4 + 4];
		  bytes[0] = 10;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, categoryId);
		  buffer.putInt(1 + 4, tagId);
		  buffer.putInt(1 + 4 + 4, active ? 1 : 0);
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }
  }
  
  public void removeAllTags() {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1];
		  bytes[0] = 11;
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }	  
  }
  
  public void setTagCategoryCombination(INetworkClient.CategoryCombination categoryCombination) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 4];
		  bytes[0] = 12;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, categoryCombination.ordinal());
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }	  
  }
  
  public void requestProjectFiles() {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1];
		  bytes[0] = 13;
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }	  	  
  }
  
  public void setMusicTagsFading(int fadeTime, boolean onlyOnChange) {
	    if (!isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	        return;
	      }
	      if (state == State.ConnectionFailure) {
	      	if (!tryReconnect()) {
	      	      Messages.addMessage(MessageType.Warning, Localization.getString("ControlConnection.NoConnection")); //$NON-NLS-1$
	      	      return;    		
	      	}
	      }
	  try {
		  byte[] bytes = new byte[1 + 2 * 4];
		  bytes[0] = 14;
		  java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(bytes);
		  buffer.putInt(1, fadeTime);
		  buffer.putInt(5, onlyOnChange ? 1 : 0);
		  socket.getOutputStream().write(bytes);
	  }
	  catch (IOException e) {
		  Messages.addMessage(MessageType.Warning, e.getLocalizedMessage());
          handleConnectionFailure(true);
	  }	  	  	  
  }
  
  private HashMap<KeyStroke, byte[]> commandMap = null;
  private HashMap<Byte, HashMap<Byte, KeyStroke>> inverseCommandMap = null;
  
  private void createCommandMap() {
    commandMap = new HashMap<KeyStroke, byte[]>();
    inverseCommandMap = new HashMap<Byte, HashMap<Byte, KeyStroke>>();
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
    for (KeyStroke stroke : commandMap.keySet()) {
    	byte[] bytes = commandMap.get(stroke);
    	if (!inverseCommandMap.containsKey(bytes[0])) {
    		inverseCommandMap.put(bytes[0], new HashMap<Byte, KeyStroke>());
    	}
    	inverseCommandMap.get(bytes[0]).put(bytes[1], stroke);
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
  
  private KeyStroke MapBytesToKeyStroke(byte[] bytes, int index) {
	  if (bytes == null || bytes.length < index + 2) {
		  return null;
	  }
	  if (bytes[index] == 0 && bytes[index + 1] == 0) {
		  return null;
	  }
	  if (inverseCommandMap == null) {
		  createCommandMap();
	  }
	  if (inverseCommandMap.containsKey(bytes[index])) {
		  HashMap<Byte, KeyStroke> subMap = inverseCommandMap.get(bytes[index]);
		  if (subMap.containsKey(bytes[index + 1])) {
			  return subMap.get(bytes[index + 1]);
		  }
	  }
	  return null;
  }
  
  
}
