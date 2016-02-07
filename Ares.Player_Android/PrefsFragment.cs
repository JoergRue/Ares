
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

