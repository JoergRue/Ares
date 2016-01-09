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
		private readonly string cServiceIntentName = "de.joerg_ruedenauer.ares.PlayerService";
		private readonly string cServicePackageName = "de.joerg_ruedenauer.ares_player";
		private bool PlayerServiceRunning
		{
			get {
				var manager = (ActivityManager)GetSystemService(ActivityService);
				return manager.GetRunningServices(int.MaxValue).Any(service => service.Service.PackageName == cServicePackageName);
			}
		}

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
			button.Text = PlayerServiceRunning ? "Stop Service" : "Start Service";
			button.Click += delegate {
				if (!PlayerServiceRunning)
				{
					StartService(new Intent(cServiceIntentName));	
					button.Text = "Stop Service";
				}
				else
				{
					StopService(new Intent(cServiceIntentName));
					button.Text = "Start Service";
				}
			};
		}
	}
}
