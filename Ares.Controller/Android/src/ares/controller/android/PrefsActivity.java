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

public class PrefsActivity extends PreferenceActivity {

	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		addPreferencesFromResource(R.xml.prefs);
		Preference udpPortPreference = getPreferenceScreen().findPreference("udp_port");
		udpPortPreference.setOnPreferenceChangeListener(numberCheckListener);
	}

	Preference.OnPreferenceChangeListener numberCheckListener = new OnPreferenceChangeListener() {

	    @Override
	    public boolean onPreferenceChange(Preference preference, Object newValue) {
	        //Check that the string is an integer.
	        return numberCheck(newValue);
	    }
	};

	private boolean numberCheck(Object newValue) {
	    if( !newValue.toString().equals("")  &&  newValue.toString().matches("\\d\\d\\d?\\d?") ) {
	        return true;
	    }
	    else {
	    	Toast.makeText(this, String.format(getString(R.string.invalid_udp_port), newValue.toString()), Toast.LENGTH_SHORT).show();
	        return false;
	    }
	}}
