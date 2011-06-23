/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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
package ares.controller.android;

import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import android.widget.ToggleButton;


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
	
	public void registerButton(int id, ToggleButton button) {
		mButtons.put(id, button);
		if (mActiveCommands.contains(id)) {
			ModeActivity.setCommandsActive(false);
			button.setChecked(true);
			button.setSelected(true);
			ModeActivity.setCommandsActive(true);
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
			ModeActivity.setCommandsActive(false);
			ToggleButton button = mButtons.get(id);
			if (button.isChecked() != active) {
				button.setChecked(active);
				button.setSelected(active);
				button.invalidate();
			}
			ModeActivity.setCommandsActive(true);
		}
	}
	
	public void allCommandsInactive() {
		ModeActivity.setCommandsActive(false);
		for (int id : mActiveCommands) {
			if (mButtons.containsKey(id)) {
				mButtons.get(id).setChecked(false);
			}
		}
		ModeActivity.setCommandsActive(true);
		mActiveCommands.clear();
	}

	private Map<Integer, ToggleButton> mButtons = new HashMap<Integer, ToggleButton>();
	private Set<Integer> mActiveCommands = new HashSet<Integer>();
}
