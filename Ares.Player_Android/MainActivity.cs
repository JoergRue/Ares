using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace Ares.Player_Android
{
	[Activity (Label = "Ares Player", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		bool runs = false;

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
			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += delegate {
				if (!runs)
				{
					StartService(new Intent("de.joerg_ruedenauer.ares.PlayerService"));	
					runs = true;
				}
				else
				{
					StopService(new Intent("de.joerg_ruedenauer.ares.PlayerService"));
					runs = false;
				}
			};
		}
	}
}
