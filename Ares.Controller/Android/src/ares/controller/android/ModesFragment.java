/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
 */package ares.controller.android;

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
import android.support.v4.app.FragmentTransaction;
import android.view.LayoutInflater;
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

public class ModesFragment extends ConnectedFragment {
	
	protected void handleConnectionOnStart() {
		boolean connected = Control.getInstance().isConnected();
        if (connected) {
        	// everything ok
        	// Log.d("ConnectedFragment", "Already connected");
        }
        else if (!isOnXLargeScreen()) {
        	// not connected, not in control fragment, not in main activity
        	// switch to main activity so that control fragment is displayed
        	// and connection can be restored
        	Intent intent = new Intent(getActivity().getBaseContext(), MainActivity.class);
        	startActivity(intent);    	
        }
        // else control fragment is displayed and will manage the connection
	}

	protected void onDisconnect(boolean startServerSearch) {
		super.onDisconnect(startServerSearch);
		if (!isOnXLargeScreen()) {
        	// not connected, not in control fragment, not in main activity
        	// switch to main activity so that control fragment is displayed
        	// and connection can be restored
        	Intent intent = new Intent(getActivity().getBaseContext(), MainActivity.class);
        	startActivity(intent);    	
        }
	}

	private void registerGestures()	{
		final GestureLibrary gesturelib = GestureLibraries.fromRawResource(getActivity(), R.raw.gestures);
		gesturelib.load();
		LinearLayout layout = (LinearLayout) getActivity().findViewById(R.id.rootLayout);
		GestureOverlayView gestureOverlay = new GestureOverlayView(getActivity());
		gestureOverlay.setUncertainGestureColor(Color.TRANSPARENT);
		gestureOverlay.setGestureColor(Color.TRANSPARENT);
		layout.addView(gestureOverlay, new LinearLayout.LayoutParams(LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT, 1));
		ViewGroup mainViewGroup = (ViewGroup) getActivity().findViewById(R.id.mainLayout);
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

    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
    	View view = inflater.inflate(R.layout.button_table, container, false);
    	return view;
    }
    
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        if (!isOnXLargeScreen()) {
        	registerGestures();
        }
    	GridView buttonGrid = (GridView)getActivity().findViewById(R.id.buttonGrid);
    	mAdapter = new ButtonAdapter(getActivity());
    	buttonGrid.setAdapter(mAdapter);
        TextView title = ((TextView)getActivity().findViewById(R.id.title));
        title.setText(R.string.modes);
    }

    private ButtonAdapter mAdapter;
    
    public void onResume() {
    	super.onResume();
    	if (mAdapter != null) {
    		mAdapter.notifyDataSetChanged();
    	}
    }
    
    public void projectLoaded() {
    	if (mAdapter != null) {
    		mAdapter.notifyDataSetChanged();
    	}
    	else {
        	GridView buttonGrid = (GridView)getActivity().findViewById(R.id.buttonGrid);
        	mAdapter = new ButtonAdapter(getActivity());
        	buttonGrid.setAdapter(mAdapter);    		
    	}
    }
    
	private void showLastMode() {
		if (Control.getInstance().getConfiguration() == null)
			return;
		showMode(Control.getInstance().getConfiguration().getModes().size() - 1, ControllerActivity.ANIM_MOVE_LEFT);
		if (!isOnXLargeScreen()) { 
			getActivity().overridePendingTransition(R.anim.slide_from_left, R.anim.slide_to_right);
		}
	}
	
	private void showFirstMode() {
		if (Control.getInstance().getConfiguration() == null)
			return;
		showMode(-1, ControllerActivity.ANIM_MOVE_RIGHT);
		if (!isOnXLargeScreen()) { 
			getActivity().overridePendingTransition(R.anim.slide_from_right, R.anim.slide_to_left);
		}
	}
	
	private void showMode(int index, int animation) {
		if (!isOnXLargeScreen()) {
	    	if (index == -1) {
	    		// special case: music list
				Intent intent = new Intent(getActivity().getBaseContext(), MusicListActivity.class);
				intent.putExtra(ModeLikeActivity.MODE_INDEX, index);
				intent.putExtra(ControllerActivity.ANIMATION_TYPE, animation);
				startActivity(intent);		    	    		
	    	}
	    	else {
				Intent intent = new Intent(getActivity().getBaseContext(), ModeActivity.class);
				intent.putExtra(ModeLikeActivity.MODE_INDEX, index);
				intent.putExtra(ControllerActivity.ANIMATION_TYPE, animation);
				startActivity(intent);		
	    	}
		}
		else {
    		ModeLikeFragment fragment = (index == -1) ? new MusicListFragment() : new ModeFragment();
			fragment.setMode(index);
			FragmentTransaction transaction = getFragmentManager().beginTransaction();
			transaction.replace(R.id.modeFragmentContainer, fragment);
			transaction.addToBackStack(null);
			transaction.commit();
		}
	}

	private void showMainControls(boolean moveUp) {
		if (isOnXLargeScreen())
			return;
		Intent intent = new Intent(getActivity().getBaseContext(), MainActivity.class);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, moveUp ? ControllerActivity.ANIM_MOVE_UP : ControllerActivity.ANIM_MOVE_DOWN);
		startActivity(intent);
		if (moveUp) {
			getActivity().overridePendingTransition(R.anim.slide_from_top, R.anim.slide_to_bottom);
		}
		else {
			getActivity().overridePendingTransition(R.anim.slide_from_bottom, R.anim.slide_to_top);
		}
	}

	private class ModeSwitcher implements OnClickListener {
    	private int mMode;
    	
    	public ModeSwitcher(int mode) {
    		mMode = mode;
    	}
    	
		public void onClick(View v) {
			if (!isOnXLargeScreen()) {
		    	if (mMode == -1) {
		    		// special case: music list
					Intent intent = new Intent(getActivity().getBaseContext(), MusicListActivity.class);
					intent.putExtra(ModeLikeActivity.MODE_INDEX, mMode);
					startActivity(intent);		    	    		
		    	}
		    	else {
					Intent intent = new Intent(getActivity().getBaseContext(), ModeActivity.class);
					intent.putExtra(ModeLikeActivity.MODE_INDEX, mMode);
					startActivity(intent);
		    	}
			}
			else {
	    		ModeLikeFragment fragment = (mMode == -1) ? new MusicListFragment() : new ModeFragment();
				fragment.setMode(mMode);
				FragmentTransaction transaction = getFragmentManager().beginTransaction();
				transaction.replace(R.id.modeFragmentContainer, fragment);
				transaction.addToBackStack(null);
				transaction.commit();
			}
		}

    }
    
	private class ButtonAdapter extends BaseAdapter {
    	
    	private Context mContext;
    	
    	public ButtonAdapter(Context c) {
    		mContext = c;
    	}
    	
		public int getCount() {
			if (Control.getInstance().getConfiguration() == null)
				return 0;
			else 
				return Control.getInstance().getConfiguration().getModes().size() + 1;
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
					if (position == 0) {
						button.setText(R.string.musicList);
					}
					else {
						button.setText(Control.getInstance().getConfiguration().getModes().get(position - 1).getTitle());
					}
					button.setOnClickListener(new ModeSwitcher(position - 1));
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
