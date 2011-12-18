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
 package ares.controllers.control;

import ares.controllers.data.Configuration;
import ares.controllers.data.FileParser;
import ares.controllers.data.KeyStroke;
import ares.controllers.messages.Messages;
import ares.controllers.messages.Message.MessageType;
import ares.controllers.network.ControlConnection;
import ares.controllers.network.INetworkClient;
import ares.controllers.network.ServerInfo;
import ares.controllers.util.Localization;

public final class Control {
  
  private static Control sInstance = null;

  public static Control getInstance() {
    if (sInstance == null) {
      sInstance = new Control();
    }
    return sInstance;
  }
  
  private Control() {
    fileName = ""; //$NON-NLS-1$
    filePath = ""; //$NON-NLS-1$
    configuration = null;
    connection = null;
  }
  
  public String getFileName() {
    return fileName;
  }
  
  public String getFilePath() {
    return filePath;
  }
  
  public Configuration getConfiguration() {
    return configuration;
  }
  
  public void openFile(java.io.File file) {
    configuration = FileParser.parseFile(file);
    if (configuration != null) {
      this.filePath = file.getAbsolutePath();
      int pos = filePath.lastIndexOf(java.io.File.separator);
      if (pos == -1) {
        fileName = filePath;
      }
      else {
        fileName = filePath.substring(pos + 1);
      }
      if (isConnected()) {
    	  connection.sendProjectOpenRequest(filePath, !isLocalPlayer);
      }
    }
  }
  
  public void openFile(byte[] contents, String path) {
	 int pos = path.lastIndexOf("/");
	 String name = path;
	 if (pos != -1) {
		 name = path.substring(pos + 1);
	 }
	 configuration = FileParser.parseBytes(contents, name);
	 if (configuration != null) {
		  this.filePath = path;
		  this.fileName = name;
	      if (isConnected()) {
	    	  connection.sendProjectOpenRequest(fileName, !isLocalPlayer);
	      }		 
	 }
  }
  
  private String serverName = ""; //$NON-NLS-1$
  private boolean isLocalPlayer = false;
  public static final String DB_ROOT_ID = "dropbox:";
 
  
  public void connect(ServerInfo server, INetworkClient client, boolean isLocalPlayer) {
    if (connection != null) {
      disconnect(true);
    }
    connection = new ControlConnection(server, client);
    connection.connect();
    serverName = server.getName();
    this.isLocalPlayer = isLocalPlayer;
    if (getConfiguration() != null) {
    	if (getFilePath().startsWith(DB_ROOT_ID)) {
    		connection.sendProjectOpenRequest(getFileName(), !isLocalPlayer);
    	}
    	else {
    		connection.sendProjectOpenRequest(getFilePath(), !isLocalPlayer);
    	}
    }
  }
  
  public void disconnect(boolean informServer) {
    if (connection != null) {
      if (connection.isConnected()) {
        connection.disconnect(informServer);
      }
      connection.dispose();
      connection = null;
      serverName = ""; //$NON-NLS-1$
      isLocalPlayer = false;
    }
  }
  
  public String getServerName() {
	  return serverName;
  }
  
  public void sendPing() {
	    if (connection == null || !connection.isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("Control.noConnection")); //$NON-NLS-1$
	      }
	      else {
	        connection.sendPing();
	      }
  }
  
  public void sendKey(KeyStroke keyStroke) {
    if (connection == null || !connection.isConnected()) {
      Messages.addMessage(MessageType.Warning, Localization.getString("Control.noConnection")); //$NON-NLS-1$
    }
    else {
      connection.sendKey(keyStroke);
    }
  }
  
  public void setVolume(int index, int value) {
	    if (connection == null || !connection.isConnected()) {
	        Messages.addMessage(MessageType.Warning, Localization.getString("Control.noConnection")); //$NON-NLS-1$
	      }
	      else {
	        connection.setVolume(index, value);
	      }
  }
  
  public void setMusicTitle(int id) {
	  if (connection == null || !connection.isConnected()) {
		  Messages.addMessage(MessageType.Warning, Localization.getString("Control.noConnection")); //$NON-NLS-1$
	  }
	  else {
		  connection.selectMusicElement(id);
	  }
  }
  
  public void switchElement(int id) {
	  if (connection == null || !connection.isConnected()) {
		  Messages.addMessage(MessageType.Warning, Localization.getString("Control.noConnection")); //$NON-NLS-1$
	  }
	  else {
		  connection.switchElement(id);
	  }	  
  }
  
  public boolean isConnected() {
    return connection != null;
  }
  
  private String fileName;
  
  private String filePath;
  
  private Configuration configuration;
  
  private ControlConnection connection;
}
