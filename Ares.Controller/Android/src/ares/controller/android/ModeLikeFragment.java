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
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import ares.controllers.control.Control;

public abstract class ModeLikeFragment extends Fragment {
	
	public abstract void projectLoaded();

	protected boolean isOnXLargeScreen() {
    	return getActivity().findViewById(R.id.modeFragmentContainer) != null;
	}

	protected void registerGestures()	{
		registerGestures(R.id.modeRootLayout, R.id.modeMainLayout);
	}
	
	private void registerGestures(int rootId, int mainId) {
		final GestureLibrary gesturelib = GestureLibraries.fromRawResource(getActivity(), R.raw.gestures);
		gesturelib.load();
		LinearLayout layout = (LinearLayout) getActivity().findViewById(rootId);
		GestureOverlayView gestureOverlay = new GestureOverlayView(getActivity());
		gestureOverlay.setUncertainGestureColor(Color.TRANSPARENT);
		gestureOverlay.setGestureColor(Color.TRANSPARENT);
		layout.addView(gestureOverlay, new LinearLayout.LayoutParams(LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT, 1));
		ViewGroup mainViewGroup = (ViewGroup) getActivity().findViewById(mainId);
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

	public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        if (!isOnXLargeScreen())
    	{
        	mMode = getActivity().getIntent().getIntExtra(ModeLikeActivity.MODE_INDEX, 0);
    	}
	}
	
	public void setMode(int mode) {
		mMode = mode;
	}
	
    private static final int SHOW_MODES = 0;
    private static final int SHOW_MAIN = 1;
    
    public void onCreateOptionsMenu(android.view.Menu menu, MenuInflater inflater) {
    	if (!isOnXLargeScreen())
    	{
    		menu.add(Menu.NONE, SHOW_MAIN, Menu.NONE, R.string.show_main);
        	menu.add(Menu.NONE, SHOW_MODES, Menu.NONE, R.string.ShowModes);
    	}
    	super.onCreateOptionsMenu(menu, inflater);
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
    	if (!isOnXLargeScreen()) {
    		getActivity().overridePendingTransition(R.anim.slide_from_left, R.anim.slide_to_right);
    	}
    }
    
    private void showNextMode() {
    	int mode = mMode + 1;
    	if (mode == Control.getInstance().getConfiguration().getModes().size()) {
    		mode = -1;
    	}
    	showMode(mode, ControllerActivity.ANIM_MOVE_RIGHT);
    	if (!isOnXLargeScreen()) {
    		getActivity().overridePendingTransition(R.anim.slide_from_right, R.anim.slide_to_left);
    	}
    }
    
    private void showModes() {
    	if (isOnXLargeScreen())
    		return;
		Intent intent = new Intent(getActivity().getBaseContext(), ModesActivity.class);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, ControllerActivity.ANIM_MOVE_DOWN);
		startActivity(intent);
		if (!isOnXLargeScreen()) {
    		getActivity().overridePendingTransition(R.anim.slide_from_bottom, R.anim.slide_to_top);
		}
    }
    
    private void showMainControls() {
    	if (isOnXLargeScreen())
    		return;
    	Intent intent = new Intent(getActivity().getBaseContext(), MainActivity.class);
    	intent.putExtra(ControllerActivity.ANIMATION_TYPE, ControllerActivity.ANIM_MOVE_UP);
    	startActivity(intent);    	
    	if (!isOnXLargeScreen()) {
    		getActivity().overridePendingTransition(R.anim.slide_from_top, R.anim.slide_to_bottom);
    	}
    }
    
    private void showMode(int mode, int backAnimation) {
    	if (!isOnXLargeScreen()) {
	    	if (mode == -1) {
	    		// special case: music list
				Intent intent = new Intent(getActivity().getBaseContext(), MusicListActivity.class);
				intent.putExtra(ModeLikeActivity.MODE_INDEX, mode);
				intent.putExtra(ControllerActivity.ANIMATION_TYPE, backAnimation);
				startActivity(intent);		    	    		
	    	}
	    	else {
				Intent intent = new Intent(getActivity().getBaseContext(), ModeActivity.class);
				intent.putExtra(ModeLikeActivity.MODE_INDEX, mode);
				intent.putExtra(ControllerActivity.ANIMATION_TYPE, backAnimation);
				startActivity(intent);		    	
	    	}
    	}
    	else {
    		ModeLikeFragment fragment = (mode == -1) ? new MusicListFragment() : new ModeFragment();
			fragment.setMode(mode);
			FragmentTransaction transaction = getFragmentManager().beginTransaction();
			transaction.replace(R.id.modeFragmentContainer, fragment);
			transaction.addToBackStack(null);
			transaction.commit();
    	}
    }
    
    protected int getMode() {
    	return mMode;
    }
    
    private int mMode;
}
