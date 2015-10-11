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

import java.io.Serializable;
import java.net.InetAddress;

public final class ServerInfo implements Serializable {

  private InetAddress address;
  private int tcpPort;
  private int webPort;
  private String name;
  
  private boolean hasWebServer;
  private boolean hasTcpServer;
  
  public ServerInfo(InetAddress address, boolean hasTcp, int tcpPort, boolean hasWeb, int webPort, String name) {
    this.address = address;
    this.hasTcpServer = hasTcp;
    this.tcpPort = tcpPort;
    this.hasWebServer = hasWeb;
    this.webPort = webPort;
    this.name = name;
  }
  
  public InetAddress getAddress() {
    return address;
  }
  
  public boolean hasTcpServer() {
	  return hasTcpServer;
  }
  
  public int getTcpPort() {
    return tcpPort;
  }
  
  public boolean hasWebServer() {
	  return hasWebServer;
  }
  
  public int getWebPort() {
	  return webPort;
  }
  
  public String getName() { 
    return name;
  }
}
