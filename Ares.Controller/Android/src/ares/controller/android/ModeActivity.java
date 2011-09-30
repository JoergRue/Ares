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

import android.content.Context;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.TextView;
import android.widget.ToggleButton;
import android.widget.GridView;
import ares.controllers.control.Control;
import ares.controllers.data.Command;
import ares.controllers.data.KeyStroke;
import ares.controllers.data.Mode;

public class ModeActivity extends ModeLikeActivity {
	
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.button_table);
        registerGestures();
    	GridView buttonGrid = (GridView)findViewById(R.id.buttonGrid);
    	mAdapter = new ButtonAdapter(this, getMode());
    	buttonGrid.setAdapter(mAdapter);
        TextView title = ((TextView)findViewById(R.id.title));
        title.setText(Control.getInstance().getConfiguration().getModes().get(getMode()).getTitle());
    }
    
    protected void onDestroy() {
    	for (Command command : Control.getInstance().getConfiguration().getModes().get(getMode()).getCommands()) {
    		CommandButtonMapping.getInstance().unregisterButton(command.getId());
    	}
    	super.onDestroy();
    }
    
    private ButtonAdapter mAdapter;
    
    private static boolean sCommandsActive = true;
    
    public static void setCommandsActive(boolean active) {
    	sCommandsActive = active;
    }
    
    private class CommandSender implements OnCheckedChangeListener {
		public void onCheckedChanged(CompoundButton button, boolean checked) {
			boolean active = CommandButtonMapping.getInstance().isCommandActive(mId);
			if (button.isChecked() != active) {
				button.setChecked(active);
				button.setSelected(active);
			}
			if (!sCommandsActive)
				return;
			Control.getInstance().sendKey(mModeKey);
			Control.getInstance().sendKey(mCommandKey);
		}
		
		public CommandSender(Mode mode, int index, int id) {
			mModeKey = mode.getKeyStroke();
			mCommandKey = mode.getCommands().get(index).getKeyStroke();
			mId = id;
		}

		private KeyStroke mModeKey;
		private KeyStroke mCommandKey;
		private int mId;
    }
    
    private class ButtonAdapter extends BaseAdapter {
    	
    	private Context mContext;
    	private Mode mMode;
    	
    	public ButtonAdapter(Context c, int mode) {
    		mContext = c;
    		mMode = Control.getInstance().getConfiguration().getModes().get(mode);
    	}
    	
		public int getCount() {
			return mMode.getCommands().size();
		}

		public Object getItem(int position) {
			return null;
		}

		public long getItemId(int position) {
			return 0;
		}
		
		private HashMap<Integer, ToggleButton> mButtons = new HashMap<Integer, ToggleButton>(); 

		public View getView(int position, View convertView, ViewGroup parent) {
			ToggleButton button;
			if (convertView == null) {
				if (mButtons.containsKey(position)) {
					return mButtons.get(position);
				}
				else {
					button = new ToggleButton(mContext);
					Command command = mMode.getCommands().get(position);
					//button.setLayoutParams(new GridView.LayoutParams(80, 20));
					button.setPadding(5, 5, 5, 5);
					button.setText(command.getTitle());
					button.setTextOn(command.getTitle());
					button.setTextOff(command.getTitle());
					CommandButtonMapping.getInstance().registerButton(command.getId(), button);
					button.setOnCheckedChangeListener(new CommandSender(mMode, position, command.getId()));
					mButtons.put(position, button);
					return button;
				}
			}
			else {
				return (ToggleButton)convertView;
			}
		}
    	
    }
}
