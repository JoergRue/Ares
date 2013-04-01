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
 */
package ares.controller.android;

import android.content.Intent;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;

public class TagsActivity extends ModeLikeActivity {

	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.tags_activity);
	}

    private void setMessageFilter() {
		String messageSetting = PreferenceManager.getDefaultSharedPreferences(getBaseContext()).getString("message_level", "Error");
		MessageHandler.getInstance().setFilter(messageSetting);
	}

    public synchronized void onActivityResult(final int requestCode, int resultCode, final Intent data) {
    	TagsFragment tagsFragment = (TagsFragment)getSupportFragmentManager().findFragmentById(R.id.tags_fragment);
    	if (tagsFragment == null) {
    		Log.e(getClass().getName(), "Could not find tags fragment!");
    		return;
    	}
    	if (requestCode == ControlFragment.REQUEST_PREFS) {
    		setMessageFilter();
    		tagsFragment.preferencesChanged();
    	}
    }
    
}
