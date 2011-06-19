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

import java.util.ArrayList;

import android.content.Context;
import android.content.Intent;
import android.gesture.Gesture;
import android.gesture.GestureLibraries;
import android.gesture.GestureLibrary;
import android.gesture.GestureOverlayView;
import android.gesture.Prediction;
import android.gesture.GestureOverlayView.OnGesturePerformedListener;
import android.graphics.Color;
import android.os.Bundle;
import android.view.Menu;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.BaseAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.ToggleButton;
import android.widget.GridView;
import android.widget.LinearLayout.LayoutParams;
import ares.controllers.control.Control;
import ares.controllers.data.Command;
import ares.controllers.data.KeyStroke;
import ares.controllers.data.Mode;

public class ModeActivity extends ControllerActivity {
	
	public static final String MODE_INDEX = "ModeIndex";

	private void registerGestures()	{
		final GestureLibrary gesturelib = GestureLibraries.fromRawResource(this, R.raw.gestures);
		gesturelib.load();
		LinearLayout layout = (LinearLayout) findViewById(R.id.rootLayout);
		GestureOverlayView gestureOverlay = new GestureOverlayView(this);
		gestureOverlay.setUncertainGestureColor(Color.TRANSPARENT);
		gestureOverlay.setGestureColor(Color.TRANSPARENT);
		layout.addView(gestureOverlay, new LinearLayout.LayoutParams(LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT, 1));
		ViewGroup mainViewGroup = (ViewGroup) findViewById(R.id.mainLayout);
		layout.removeView(mainViewGroup);
		gestureOverlay.addView(mainViewGroup);
		gestureOverlay.addOnGesturePerformedListener(new OnGesturePerformedListener() {			
			@Override
			public void onGesturePerformed(GestureOverlayView overlay, Gesture gesture) {
				ArrayList<Prediction> predictions = gesturelib.recognize(gesture);
				for (Prediction prediction : predictions) {
					if (prediction.score > 1.0) {
						if (prediction.name.equals("point_up")) {
							showMainControls();
						}
						else if (prediction.name.equals("point_down")) {
							showModes();
						}
						else if (prediction.name.equals("swipe_left")) {
							showNextMode();
						}
						else if (prediction.name.equals("swipe_right")) {
							showPreviousMode();
						}
						break;
					}
				}
			}
		});
	}

	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.button_table);
        registerGestures();
    	GridView buttonGrid = (GridView)findViewById(R.id.buttonGrid);
    	mMode = getIntent().getIntExtra(MODE_INDEX, 0);
    	mAdapter = new ButtonAdapter(this, mMode);
    	buttonGrid.setAdapter(mAdapter);
        TextView title = ((TextView)findViewById(R.id.title));
        title.setText(Control.getInstance().getConfiguration().getModes().get(mMode).getTitle());
    }
    
    private static final int SHOW_MODES = 0;
    private static final int SHOW_MAIN = 1;
    
    public boolean onCreateOptionsMenu(android.view.Menu menu) {
    	menu.add(Menu.NONE, SHOW_MAIN, Menu.NONE, R.string.show_main);
    	menu.add(Menu.NONE, SHOW_MODES, Menu.NONE, R.string.ShowModes);
    	return super.onCreateOptionsMenu(menu);
    }

    public boolean onOptionsItemSelected(android.view.MenuItem menuItem) {
    	switch (menuItem.getItemId())
    	{
    	case SHOW_MODES:
    		showModes();
    		break;
    	case SHOW_MAIN:
    		showMainControls();
        }
    	return super.onOptionsItemSelected(menuItem);
    }
    
    protected void onDestroy() {
    	for (Command command : Control.getInstance().getConfiguration().getModes().get(mMode).getCommands()) {
    		CommandButtonMapping.getInstance().unregisterButton(command.getId());
    	}
    	super.onDestroy();
    }
    
    private void showPreviousMode() {
    	int mode = mMode - 1;
    	if (mode == -1) {
    		mode = Control.getInstance().getConfiguration().getModes().size() - 1;
    	}
    	showMode(mode, ControllerActivity.ANIM_MOVE_LEFT);
    	overridePendingTransition(R.anim.slide_from_left, R.anim.slide_to_right);
    }
    
    private void showNextMode() {
    	int mode = mMode + 1;
    	if (mode == Control.getInstance().getConfiguration().getModes().size()) {
    		mode = 0;
    	}
    	showMode(mode, ControllerActivity.ANIM_MOVE_RIGHT);
    	overridePendingTransition(R.anim.slide_from_right, R.anim.slide_to_left);
    }
    
    private void showModes() {
		Intent intent = new Intent(getBaseContext(), ModesActivity.class);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, ControllerActivity.ANIM_MOVE_DOWN);
		startActivity(intent);
		overridePendingTransition(R.anim.slide_from_bottom, R.anim.slide_to_top);
    }
    
    private void showMainControls() {
    	Intent intent = new Intent(getBaseContext(), MainActivity.class);
    	intent.putExtra(ControllerActivity.ANIMATION_TYPE, ControllerActivity.ANIM_MOVE_UP);
    	startActivity(intent);    	
    	overridePendingTransition(R.anim.slide_from_top, R.anim.slide_to_bottom);
    }
    
    private void showMode(int mode, int backAnimation) {
		Intent intent = new Intent(getBaseContext(), ModeActivity.class);
		intent.putExtra(ModeActivity.MODE_INDEX, mode);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, backAnimation);
		startActivity(intent);		    	
    }
    
    private int mMode;
    private ButtonAdapter mAdapter;
    
    private class CommandSender implements OnClickListener {
		public void onClick(View v) {
			Control.getInstance().sendKey(mModeKey);
			Control.getInstance().sendKey(mCommandKey);
		}
		
		public CommandSender(Mode mode, int index) {
			mModeKey = mode.getKeyStroke();
			mCommandKey = mode.getCommands().get(index).getKeyStroke();
		}

		private KeyStroke mModeKey;
		private KeyStroke mCommandKey;
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

		public View getView(int position, View convertView, ViewGroup parent) {
			ToggleButton button;
			if (convertView == null) {
				button = new ToggleButton(mContext);
				Command command = mMode.getCommands().get(position);
				//button.setLayoutParams(new GridView.LayoutParams(80, 20));
				button.setPadding(5, 5, 5, 5);
				button.setText(command.getTitle());
				button.setTextOn(command.getTitle());
				button.setTextOff(command.getTitle());
				button.setChecked(CommandButtonMapping.getInstance().isCommandActive(command.getId()));
				CommandButtonMapping.getInstance().registerButton(command.getId(), button);
				button.setOnClickListener(new CommandSender(mMode, position));
			}
			else {
				button = (ToggleButton)convertView;
			}
			return button;
		}
    	
    }
}
