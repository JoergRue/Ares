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

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;

public class MainActivity extends ControllerActivity {
	
	static class StrictModeWrapper {

		static 
		{
		    try
		    {
		        Class.forName("android.os.StrictMode");
		    }
		    catch (Exception e)
		    {
		        throw new RuntimeException(e);
		    }
		}
		
		public static void checkAvailable() {}
		
		public void setState() {
	         android.os.StrictMode.setThreadPolicy(new android.os.StrictMode.ThreadPolicy.Builder()
             .penaltyLog()
             .build());			
		}
	}
	
	private static Boolean sStrictModeAvailable = null;

    private void setStrictModeState() {
        if (sStrictModeAvailable == null) {
            try {
                StrictModeWrapper.checkAvailable();
                sStrictModeAvailable = true;
            } 
            catch (Throwable t) {
            	sStrictModeAvailable = false;
            }
        }

        if (sStrictModeAvailable == true) {
            (new StrictModeWrapper()).setState();
        }
    }

    private void setMessageFilter() {
		String messageSetting = PreferenceManager.getDefaultSharedPreferences(getBaseContext()).getString("message_level", "Error");
		MessageHandler.getInstance().setFilter(messageSetting);
	}
	
	public void onCreate(Bundle savedInstanceState) {
		
		setStrictModeState();
		
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);

		Intent intent = getIntent();
		if (intent.hasExtra("UDPPort")) {
			int newPort = intent.getIntExtra("UDPPort", 0);
			if (newPort != 0) {
				Log.d("Ares Controller", "Received new port by intent");
				SharedPreferences.Editor editor = PreferenceManager.getDefaultSharedPreferences(this).edit();
				editor.putString("udp_port", "" + newPort);
				editor.commit();
			}
		}
		
		MessageHandler.getInstance().setContext(getApplicationContext());
        setMessageFilter();
	}

    public Dialog onCreateDialog(int id) {
    	ControlFragment controlFragment = (ControlFragment)getSupportFragmentManager().findFragmentById(R.id.control_fragment);
    	if (controlFragment == null)
    	{
    		controlFragment = (ControlFragment)getSupportFragmentManager().findFragmentById(R.id.controlFragmentContainer);
    	}
    	if (controlFragment == null) {
    		Log.e(getClass().getName(), "Could not find control fragment!");
    		return null;
    	}
    	
    	switch (id) {
    	case ControlFragment.ABOUT:
    		return controlFragment.showAboutDialog();
    	case ControlFragment.CONNECT:
    		return controlFragment.showPlayerSelection();
		default:
			return null;
    	}
    }
    
    public synchronized void onActivityResult(final int requestCode, int resultCode, final Intent data) {
    	ControlFragment controlFragment = (ControlFragment)getSupportFragmentManager().findFragmentById(R.id.control_fragment);
    	if (controlFragment == null)
    	{
    		controlFragment = (ControlFragment)getSupportFragmentManager().findFragmentById(R.id.controlFragmentContainer);
    	}
    	if (controlFragment == null) {
    		Log.e(getClass().getName(), "Could not find control fragment!");
    		return;
    	}
    	if (requestCode == ControlFragment.REQUEST_OPEN) {
    		if (resultCode == Activity.RESULT_OK) {
    			String filePath = data.getStringExtra(FileDialog.RESULT_PATH);
    			controlFragment.openProject(filePath, this);
    		}
    	}
    	else if (requestCode == ControlFragment.REQUEST_PREFS) {
    		setMessageFilter();
    		controlFragment.preferencesChanged();
    	}
    }
    
}