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
package ares.controller.gui;

import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import javax.swing.AbstractButton;

import ares.controller.control.ModeCommandAction;

class CommandButtonMapping {
	
	private CommandButtonMapping()
	{
	}
	
	private static CommandButtonMapping sInstance = null;
	
	public static CommandButtonMapping getInstance() {
		if (sInstance == null) {
			sInstance = new CommandButtonMapping();
		}
		return sInstance;
	}
	
	public void registerButton(int id, AbstractButton button) {
		mButtons.put(id, button);
		if (mActiveCommands.contains(id)) {
			button.setSelected(true);
		}
	}
	
	public void unregisterButton(int id) {
		mButtons.remove(id);
	}
	
	public boolean isCommandActive(int id) {
		return mActiveCommands.contains(id);
	}
	
	public void commandStateChanged(int id, boolean active) {
		if (mActiveCommands.contains(id) && !active) {
			mActiveCommands.remove(id);
		}
		else if (!mActiveCommands.contains(id) && active) {
			mActiveCommands.add(id);
		}
		if (mButtons.containsKey(id)) {
			ModeCommandAction.setCommandsActive(false);
			mButtons.get(id).setSelected(active);
			ModeCommandAction.setCommandsActive(true);
		}
	}

	private Map<Integer, AbstractButton> mButtons = new HashMap<Integer, AbstractButton>();
	private Set<Integer> mActiveCommands = new HashSet<Integer>();
}
