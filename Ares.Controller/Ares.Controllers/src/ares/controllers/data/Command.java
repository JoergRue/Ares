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
package ares.controllers.data;

public class Command {

  private String title;
  private KeyStroke keyStroke;
  private int id;
  
  public Command(String title, int id, KeyStroke keyStroke) {
    this.title = title;
    this.keyStroke = keyStroke;
    this.id = id;
  }
  
  public int getId() {
	  return id;
  }
  
  public String getTitle() {
    return title;
  }
  
  public KeyStroke getKeyStroke() {
    return keyStroke;
  }
  
}
