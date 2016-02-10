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
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Linq;

namespace Ares.Player_Android
{
	[Activity (Label = "Ares Player", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			#if DEBUG
			Xamarin.Insights.Initialize (Xamarin.Insights.DebugModeKey, this);
			#else
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);
			#endif
			base.OnCreate (savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this);
			if (prefs.GetBoolean("FirstStart", true))
			{
				var editor = prefs.Edit();
				editor.PutBoolean("FirstStart", false);
				editor.Commit();
				ShowUsageHint();
			}
		}

		public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.OptionsMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Resource.Id.menu_about:
				ShowAboutDialog();
				break;
			case Resource.Id.menu_settings:
				ShowSettings();
				break;
			case Resource.Id.menu_usage:
				ShowUsageHint();
				break;
			}
			return base.OnOptionsItemSelected(item);
		}

		private void ShowSettings()
		{
			StartActivity(typeof(SettingsActivity));
			var serviceFragment = (ServiceFragment)FragmentManager.FindFragmentById(Resource.Id.service_fragment);
		}

		private void ShowAboutDialog()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			var pinfo = PackageManager.GetPackageInfo(PackageName, 0);
			builder.SetMessage(string.Format(Resources.GetString(Resource.String.aboutInfo), pinfo.VersionName))
				.SetCancelable(true)
				.SetIcon(Resource.Drawable.Ares)
				.SetNeutralButton(Android.Resource.String.Ok, (sender, EventArgs) => {
			});
			builder.Show();
		}

		private void ShowUsageHint()
		{
			UsageDialogFragment dialogFragment = new UsageDialogFragment();
			dialogFragment.Show(FragmentManager, "UsageDialogFragment");
		}
	}
}
