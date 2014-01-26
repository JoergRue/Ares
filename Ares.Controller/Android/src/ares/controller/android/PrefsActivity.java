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


import android.os.Bundle;
import android.preference.Preference;
import android.preference.Preference.OnPreferenceChangeListener;
import android.preference.PreferenceActivity;
import android.widget.Toast;
import ares.controllers.network.ServerInfo;
import ares.controllers.network.ServerSearch;

public class PrefsActivity extends PreferenceActivity {

	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		addPreferencesFromResource(R.xml.prefs);
		Preference udpPortPreference = getPreferenceScreen().findPreference("udp_port");
		udpPortPreference.setOnPreferenceChangeListener(numberCheckListener);
		Preference playerConnectionPreference = getPreferenceScreen().findPreference("player_connection");
		playerConnectionPreference.setOnPreferenceChangeListener(connectionCheckListener);

		Preference fadingPreference = getPreferenceScreen().findPreference("tag_fading_time");
		fadingPreference.setOnPreferenceChangeListener(fadingChangeListener);
		
		Preference fadingPreference2 = getPreferenceScreen().findPreference("music_fading_time");
		fadingPreference2.setOnPreferenceChangeListener(fadingChangeListener);
	}
	
	Preference.OnPreferenceChangeListener fadingChangeListener = new OnPreferenceChangeListener() {
		public boolean onPreferenceChange(Preference preference, Object newValue) {
			return checkFadingTime(newValue);
		}
	};
	
	private boolean checkFadingTime(Object newValue) {
		if (newValue == null || newValue.toString().equals("")) {
			Toast.makeText(getApplicationContext(), getString(R.string.invalid_fading_time), Toast.LENGTH_LONG).show();
			return false;
		}
		try {
			int val = Integer.parseInt(newValue.toString());
			if (val < 0 || val > 50000) {
				Toast.makeText(getApplicationContext(), getString(R.string.invalid_fading_time), Toast.LENGTH_LONG).show();
				return false;
			}
			return true;
		}
		catch (NumberFormatException e) {
			Toast.makeText(getApplicationContext(), getString(R.string.invalid_fading_time), Toast.LENGTH_LONG).show();
			return false;
		}
	}
	
	Preference.OnPreferenceChangeListener numberCheckListener = new OnPreferenceChangeListener() {

	    @Override
	    public boolean onPreferenceChange(Preference preference, Object newValue) {
	        //Check that the string is an integer.
	        return numberCheck(newValue);
	    }
	};
	
	Preference.OnPreferenceChangeListener connectionCheckListener = new OnPreferenceChangeListener() {
		public boolean onPreferenceChange(Preference preference, Object newValue) {
			if (newValue.equals("auto"))
				return true;
			try {
				ServerInfo info = ServerSearch.getServerInfo(newValue.toString(), ",");
				if (info == null)
				{
					Toast.makeText(getApplicationContext(), getString(R.string.invalid_player_connection_format), Toast.LENGTH_LONG).show();
					return false;
				}
				return true;
			}
			catch (Exception e) {
				Toast.makeText(getApplicationContext(), getString(R.string.invalid_player_connection_format), Toast.LENGTH_LONG).show();
				return false;
			}
		}
	};

	private boolean numberCheck(Object newValue) {
	    if( !newValue.toString().equals("")  &&  newValue.toString().matches("\\d\\d\\d?\\d?") ) {
	        return true;
	    }
	    else {
	    	Toast.makeText(this, String.format(getString(R.string.invalid_udp_port), newValue.toString()), Toast.LENGTH_LONG).show();
	        return false;
	    }
	}}
