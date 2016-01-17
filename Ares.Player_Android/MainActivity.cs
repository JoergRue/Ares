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
		}
	}
}
