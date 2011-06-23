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
import java.util.HashMap;

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
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.GridView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.LinearLayout.LayoutParams;
import ares.controllers.control.Control;

public class ModesActivity extends ControllerActivity {

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
							showMainControls(false);
						}
						else if (prediction.name.equals("point_down")) {
							showMainControls(true);
						}
						else if (prediction.name.equals("swipe_left")) {
							showFirstMode();
						}
						else if (prediction.name.equals("swipe_right")) {
							showLastMode();
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
    	mAdapter = new ButtonAdapter(this);
    	buttonGrid.setAdapter(mAdapter);
        TextView title = ((TextView)findViewById(R.id.title));
        title.setText(R.string.modes);
    }
    
    private ButtonAdapter mAdapter;
    
    protected void onRestart() {
    	super.onRestart();
    	mAdapter.notifyDataSetChanged();
    }
    
	private void showLastMode() {
		if (Control.getInstance().getConfiguration() == null)
			return;
		showMode(Control.getInstance().getConfiguration().getModes().size() - 1, ControllerActivity.ANIM_MOVE_LEFT);
		overridePendingTransition(R.anim.slide_from_left, R.anim.slide_to_right);
	}
	
	private void showFirstMode() {
		if (Control.getInstance().getConfiguration() == null)
			return;
		showMode(0, ControllerActivity.ANIM_MOVE_RIGHT);
		overridePendingTransition(R.anim.slide_from_right, R.anim.slide_to_left);
	}
	
	private void showMode(int index, int animation) {
		Intent intent = new Intent(getBaseContext(), ModeActivity.class);
		intent.putExtra(ModeActivity.MODE_INDEX, index);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, animation);
		startActivity(intent);		
	}

	private void showMainControls(boolean moveUp) {
		Intent intent = new Intent(getBaseContext(), MainActivity.class);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, moveUp ? ControllerActivity.ANIM_MOVE_UP : ControllerActivity.ANIM_MOVE_DOWN);
		startActivity(intent);
		if (moveUp) {
			overridePendingTransition(R.anim.slide_from_top, R.anim.slide_to_bottom);
		}
		else {
			overridePendingTransition(R.anim.slide_from_bottom, R.anim.slide_to_top);
		}
	}

	private class ModeSwitcher implements OnClickListener {
    	private int mMode;
    	
    	public ModeSwitcher(int mode) {
    		mMode = mode;
    	}
    	
		public void onClick(View v) {
			Intent intent = new Intent(getBaseContext(), ModeActivity.class);
			intent.putExtra(ModeActivity.MODE_INDEX, mMode);
			startActivity(intent);
		}

    }
    
	private class ButtonAdapter extends BaseAdapter {
    	
    	private Context mContext;
    	
    	public ButtonAdapter(Context c) {
    		mContext = c;
    	}
    	
		public int getCount() {
			return Control.getInstance().getConfiguration().getModes().size();
		}

		public Object getItem(int position) {
			return null;
		}

		public long getItemId(int position) {
			return 0;
		}
		
		private HashMap<Integer, Button> mButtons = new HashMap<Integer, Button>();

		public View getView(int position, View convertView, ViewGroup parent) {
			Button button;
			if (convertView == null) {
				if (mButtons.containsKey(position)) {
					return mButtons.get(position);
				}
				else {
					button = new Button(mContext);
					//button.setLayoutParams(new GridView.LayoutParams(80, 20));
					button.setPadding(5, 5, 5, 5);
					button.setText(Control.getInstance().getConfiguration().getModes().get(position).getTitle());
					button.setOnClickListener(new ModeSwitcher(position));
					mButtons.put(position, button);
					return button;
				}
			}
			else {
				return (Button)convertView;
			}
		}    	
    }
}
