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

import java.awt.event.KeyEvent;

import javax.swing.JComponent;
import javax.swing.KeyStroke;

import ares.controllers.data.Configuration;
import ares.controllers.data.FileParser;
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
    }
  }
  
  public void connect(ServerInfo server, INetworkClient client) {
    if (connection != null) {
      disconnect(true);
    }
    connection = new ControlConnection(server, client);
    connection.connect();
  }
  
  public void disconnect(boolean informServer) {
    if (connection != null) {
      if (connection.isConnected()) {
        connection.disconnect(informServer);
      }
      connection.dispose();
      connection = null;
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
  
  public boolean isConnected() {
    return connection != null;
  }
  
  private void addKeyAction(JComponent component, KeyStroke key) {
    component.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(key, key.toString());
    component.getActionMap().put(key.toString(), new KeyAction(key));
  }
  
  private static KeyStroke getKeyStroke(int keyCode) {
    return KeyStroke.getKeyStroke(keyCode, 0);
  }
  
  public void addAlwaysAvailableKeys(JComponent component) {
    addKeyAction(component, getKeyStroke(KeyEvent.VK_UP));
    addKeyAction(component, getKeyStroke(KeyEvent.VK_DOWN));
    addKeyAction(component, getKeyStroke(KeyEvent.VK_LEFT));
    addKeyAction(component, getKeyStroke(KeyEvent.VK_RIGHT));
    addKeyAction(component, getKeyStroke(KeyEvent.VK_PAGE_UP));
    addKeyAction(component, getKeyStroke(KeyEvent.VK_PAGE_DOWN));
    addKeyAction(component, getKeyStroke(KeyEvent.VK_INSERT));
    addKeyAction(component, getKeyStroke(KeyEvent.VK_DELETE));    
    KeyStroke escapeStroke = getKeyStroke(KeyEvent.VK_ESCAPE);
    component.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(escapeStroke, escapeStroke.toString());
    component.getActionMap().put(escapeStroke.toString(), new KeyAction(escapeStroke));
  }
  
  private String fileName;
  
  private String filePath;
  
  private Configuration configuration;
  
  private ControlConnection connection;
}
