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
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import ares.controllers.control.Control;

public class ModeLikeActivity extends ControllerActivity {

	public static final String MODE_INDEX = "ModeIndex";

	protected void registerGestures()	{
		registerGestures(R.id.rootLayout, R.id.mainLayout);
	}
	
	protected void registerGestures(int rootId, int mainId) {
		final GestureLibrary gesturelib = GestureLibraries.fromRawResource(this, R.raw.gestures);
		gesturelib.load();
		LinearLayout layout = (LinearLayout) findViewById(rootId);
		GestureOverlayView gestureOverlay = new GestureOverlayView(this);
		gestureOverlay.setUncertainGestureColor(Color.TRANSPARENT);
		gestureOverlay.setGestureColor(Color.TRANSPARENT);
		layout.addView(gestureOverlay, new LinearLayout.LayoutParams(LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT, 1));
		ViewGroup mainViewGroup = (ViewGroup) findViewById(mainId);
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
    	mMode = getIntent().getIntExtra(MODE_INDEX, 0);
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
    
    private void showPreviousMode() {
    	int mode = mMode - 1;
    	if (mode == -2) {
    		mode = Control.getInstance().getConfiguration().getModes().size() - 1;
    	}
    	showMode(mode, ControllerActivity.ANIM_MOVE_LEFT);
    	overridePendingTransition(R.anim.slide_from_left, R.anim.slide_to_right);
    }
    
    private void showNextMode() {
    	int mode = mMode + 1;
    	if (mode == Control.getInstance().getConfiguration().getModes().size()) {
    		mode = -1;
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
    	if (mode == -1) {
    		// special case: music list
			Intent intent = new Intent(getBaseContext(), MusicListActivity.class);
			intent.putExtra(ModeLikeActivity.MODE_INDEX, mode);
			intent.putExtra(ControllerActivity.ANIMATION_TYPE, backAnimation);
			startActivity(intent);		    	    		
    	}
    	else {
			Intent intent = new Intent(getBaseContext(), ModeActivity.class);
			intent.putExtra(ModeLikeActivity.MODE_INDEX, mode);
			intent.putExtra(ControllerActivity.ANIMATION_TYPE, backAnimation);
			startActivity(intent);		    	
    	}
    }
    
    protected int getMode() {
    	return mMode;
    }
    
    private int mMode;
}
