
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
	[Activity (Label = "Ares Player", Icon = "@mipmap/icon")]		
	public class MessagesActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			#if DEBUG
			Xamarin.Insights.Initialize (Xamarin.Insights.DebugModeKey, this);
			#else
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);
			#endif
			SetContentView(Resource.Layout.Messages);
		}
	}
}

