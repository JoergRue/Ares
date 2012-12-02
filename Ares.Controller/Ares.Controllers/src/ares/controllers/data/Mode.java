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

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public final class Mode extends Command {
  
  private Map<Integer, Command> commands;

  public Mode(String title, int id, KeyStroke keyStroke) {
    super(title, id, keyStroke, true);
    commands = new HashMap<Integer, Command>();
  }
  
  public void addCommand(Command command) {
    commands.put(command.getId(), command);
  }
  
  public String getTitle(int id) {
	  if (commands.containsKey(id)) {
		  return commands.get(id).getTitle();
	  }
	  else {
		  return null;
	  }
  }
  
  public List<Command> getCommands() {
	  return new ArrayList<Command>(commands.values());
  }
  
  public boolean containsKeyStroke(KeyStroke keyStroke) {
	if (keyStroke == null)
		return false;
    for(Command command : commands.values()) {
      if (command.getKeyStroke() != null && command.getKeyStroke().equals(keyStroke)) return true;
    }
    return false;
  }
  
}
