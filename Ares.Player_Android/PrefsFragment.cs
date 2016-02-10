/*
 Copyright (c) 2016  [Joerg Ruedenauer]
 
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace Ares.Player_Android
{
	public class PrefsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
	{
		private FolderPreference[] mFolderPrefs = new FolderPreference[3];
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			AddPreferencesFromResource(Resource.Xml.preferences);
			mFolderPrefs[0] = (FolderPreference)FindPreference("musicFolder");
			mFolderPrefs[1] = (FolderPreference)FindPreference("soundFolder");
			mFolderPrefs[2] = (FolderPreference)FindPreference("projectFolder");
			for (int i = 0; i < mFolderPrefs.Length; ++i)
			{
				var pref = mFolderPrefs[i];
				pref.ParentFragment = this;
				pref.FolderId = i;
			}
		}

		public override void OnResume()
		{
			base.OnResume();
			PreferenceScreen.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
		}

		public override void OnPause()
		{
			base.OnPause();
			PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
		}

		private bool PlayerServiceRunning
		{
			get {
				var manager = (ActivityManager)Activity.GetSystemService(Activity.ActivityService);
				return manager.GetRunningServices(int.MaxValue).Any(service => service.Service.PackageName == ServiceFragment.cServicePackageName);
			}
		}

		public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
		{
			Settings.Settings.Instance.Read(Activity);
			if (PlayerServiceRunning)
			{
				Activity.StopService(new Intent(ServiceFragment.cServiceIntentName));
				Activity.StartService(new Intent(ServiceFragment.cServiceIntentName));	
			}
		}


		public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok && requestCode >= 0 && requestCode < mFolderPrefs.Length)
			{
				mFolderPrefs[requestCode].OnActivityResult(data);
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			return base.OnCreateView(inflater, container, savedInstanceState);
		}
	}
}

