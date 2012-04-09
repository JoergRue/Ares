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

import android.preference.PreferenceManager;
import android.support.v4.app.FragmentActivity;
import android.view.KeyEvent;
import ares.controllers.control.Control;

public class ControllerActivity extends FragmentActivity {
	
	public void onStart() {
		super.onStart();
		MessageHandler.getInstance().register();
	}
	
	public void onStop() {
		MessageHandler.getInstance().unregister();
		super.onStop();
	}

	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (!Control.getInstance().isConnected() || (keyCode != KeyEvent.KEYCODE_VOLUME_DOWN && keyCode != KeyEvent.KEYCODE_VOLUME_UP)) {
			return super.onKeyDown(keyCode, event);
		}
		String volumeSetting = PreferenceManager.getDefaultSharedPreferences(getBaseContext()).getString("volume_keys", "Overall");
		if (volumeSetting.equals("None")) {
			return super.onKeyDown(keyCode, event);
		}
		int currentVolume = PlayingState.getInstance().getOverallVolume();
		int index = 2;
		if (volumeSetting.equals("Music")) {
			index = 1;
			currentVolume = PlayingState.getInstance().getMusicVolume();
		}
		else if (volumeSetting.equals("Sounds")) {
			index = 0;
			currentVolume = PlayingState.getInstance().getSoundVolume();
		}
		if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
			Control.getInstance().setVolume(index, currentVolume > 10 ? currentVolume - 10 : 0);
			return true;
		}
		else if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
			Control.getInstance().setVolume(index, currentVolume < 90 ? currentVolume + 10 : 100);
			return true;
		}
		else return super.onKeyDown(keyCode, event);
	}
	
	public static final String ANIMATION_TYPE = "AnimationType";
	public static final int ANIM_MOVE_UP = 100;
	public static final int ANIM_MOVE_DOWN = 101;
	public static final int ANIM_MOVE_LEFT = 102;
	public static final int ANIM_MOVE_RIGHT = 103;
	public static final int ANIM_NONE = 104;
	
	public void onBackPressed() {
		int animation = getIntent().getIntExtra(ANIMATION_TYPE, ANIM_NONE);
		super.onBackPressed();
		switch (animation) {
		case ANIM_MOVE_UP:
			overridePendingTransition(R.anim.slide_from_bottom, R.anim.slide_to_top);
			break;
		case ANIM_MOVE_DOWN:
			overridePendingTransition(R.anim.slide_from_top, R.anim.slide_to_bottom);
			break;
		case ANIM_MOVE_LEFT:
			overridePendingTransition(R.anim.slide_from_right, R.anim.slide_to_left);
			break;
		case ANIM_MOVE_RIGHT:
			overridePendingTransition(R.anim.slide_from_left, R.anim.slide_to_right);
			break;
		case ANIM_NONE:
			break;
		default:
			break;
		}
	}
}
