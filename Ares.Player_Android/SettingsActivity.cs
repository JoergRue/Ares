
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Ares.Player_Android
{
	[Activity(Label = "Ares Player", Icon = "@mipmap/icon", Theme = "@android:style/Theme.Holo.DialogWhenLarge", ExcludeFromRecents = true)]			
	public class SettingsActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			FragmentManager.BeginTransaction().Replace(Android.Resource.Id.Content, new PrefsFragment()).Commit();
		}
	}
}

