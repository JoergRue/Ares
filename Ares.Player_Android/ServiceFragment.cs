
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

namespace Ares.Player_Android
{
	public class ServiceFragment : Fragment
	{
		public static readonly string cServiceIntentName = "de.joerg_ruedenauer.ares.PlayerService";
		public static readonly string cServicePackageName = "de.joerg_ruedenauer.ares_player";

		private class ServiceStateUpdateReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{
				var fragment = ((Activity)context).FragmentManager.FindFragmentById(Resource.Id.service_fragment);
				if (fragment != null)
				{
					((ServiceFragment)fragment).UpdateServiceState();
				}
			}
		}

		private class PlayerServiceConnection : Java.Lang.Object, IServiceConnection
		{
			public PlayerServiceConnection(ServiceFragment fragment)
			{
				mFragment = fragment;
			}

			public void OnServiceConnected(ComponentName name, IBinder service)
			{
				PlayerService.ServiceBinder binder = service as PlayerService.ServiceBinder;
				if (binder != null)
				{
					mFragment.Binder = binder;
					mFragment.IsBound = true;
					mFragment.UpdateServiceState();
				}
			}
			public void OnServiceDisconnected(ComponentName name)
			{
				mFragment.IsBound = false;
			}


			private ServiceFragment mFragment;
		}
			

		public override void OnStart()
		{
			base.OnStart();
			if (PlayerServiceRunning)
			{
				RegisterStateReceiver();
			}
		}

		public override void OnStop()
		{
			if (PlayerServiceRunning)
			{
				UnregisterStateReceiver();
			}
			base.OnStop();
		}

		private void RegisterStateReceiver()
		{
			var intentFilter = new IntentFilter(PlayerService.StateChangedAction);
			stateUpdateReceiver = new ServiceStateUpdateReceiver();
			Activity.RegisterReceiver(stateUpdateReceiver, intentFilter);
			Activity.BindService(playerServiceIntent, serviceConnection, Bind.AutoCreate);
		}

		private void UnregisterStateReceiver()
		{
			if (IsBound)
			{
				Activity.UnbindService(serviceConnection);
				IsBound = false;
			}
			if (stateUpdateReceiver != null)
			{
				Activity.UnregisterReceiver(stateUpdateReceiver);
			}
		}

		private ServiceStateUpdateReceiver stateUpdateReceiver;
		private Intent playerServiceIntent;
		private PlayerServiceConnection serviceConnection;
		public PlayerService.ServiceBinder Binder { get; set; }
		public bool IsBound { get; set; }

		public void UpdateServiceState()
		{
			if (IsBound && Activity != null)
			{
				Activity.RunOnUiThread(() => DoUpdateServiceState());
			}
		}

		private void DoUpdateServiceState()
		{
			bool serviceRunning = PlayerServiceRunning;
			serviceButton.Text = serviceRunning ? Resources.GetString(Resource.String.stop) : Resources.GetString(Resource.String.start);
			serviceStateView.Text = String.Format(Resources.GetString(Resource.String.service_status), 
				Resources.GetString(serviceRunning ? Resource.String.running : Resource.String.stopped));
			if (IsBound)
			{
				var service = Binder.Service;
				if (service.HasController)
				{
					controllerStateView.Text = Resources.GetString(Resource.String.connected_with) + service.ControllerName;
					controllerButton.Enabled = false;
					controllerButton.Visibility = ViewStates.Invisible;
				}
				else
				{
					controllerStateView.Text = Resources.GetString(Resource.String.not_connected);
					controllerButton.Enabled = true;
					controllerButton.Visibility = ViewStates.Visible;
				}
				if (service.HasProject)
				{
					projectView.Text = service.ProjectTitle;
				}
				else
				{
					projectView.Text = Resources.GetString(Resource.String.no_project);
				}
				if (messagesButton != null)
					messagesButton.Enabled = true;
			}
			else
			{
				controllerStateView.Text = Resources.GetString(Resource.String.not_connected);
				controllerButton.Enabled = false;
				controllerButton.Visibility = ViewStates.Visible;
				projectView.Text = Resources.GetString(Resource.String.no_project);
				if (messagesButton != null)
					messagesButton.Enabled = false;
			}
		}

		private bool PlayerServiceRunning
		{
			get {
				var manager = (ActivityManager)Activity.GetSystemService(Activity.ActivityService);
				return manager.GetRunningServices(int.MaxValue).Any(service => service.Service.PackageName == cServicePackageName);
			}
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			playerServiceIntent = new Intent(cServiceIntentName);
			serviceConnection = new PlayerServiceConnection(this);
		}

		private void StartService()
		{
			Activity.StartService(new Intent(cServiceIntentName));	
			serviceButton.Text = Resources.GetString(Resource.String.stop);
			serviceStateView.Text = String.Format(Resources.GetString(Resource.String.service_status), 
				Resources.GetString(Resource.String.running));
			RegisterStateReceiver();
		}

		private void StopService()
		{
			UnregisterStateReceiver();
			Activity.StopService(new Intent(cServiceIntentName));
			serviceButton.Text = Resources.GetString(Resource.String.start);
			serviceStateView.Text = String.Format(Resources.GetString(Resource.String.service_status), 
				Resources.GetString(Resource.String.stopped));
			controllerStateView.Text = Resources.GetString(Resource.String.not_connected);
			controllerButton.Enabled = false;
			controllerButton.Visibility = ViewStates.Visible;
			projectView.Text = Resources.GetString(Resource.String.no_project);
			if (messagesButton != null)
			{
				messagesButton.Enabled = false;
			}
		}

		TextView serviceStateView;
		Button serviceButton;
		TextView controllerStateView;
		Button controllerButton;
		TextView projectView;
		Button messagesButton;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.ServiceFragment, container, false);

			serviceStateView = view.FindViewById<TextView>(Resource.Id.serviceView);
			serviceButton = view.FindViewById<Button> (Resource.Id.serviceButton);

			serviceButton.Click += delegate {
				if (!PlayerServiceRunning)
				{
					StartService();
				}
				else
				{
					StopService();
				}
			};

			controllerStateView = view.FindViewById<TextView>(Resource.Id.controllerVIew);
			controllerButton = view.FindViewById<Button>(Resource.Id.controllerButton);
			controllerButton.Click += delegate {
				if (IsControllerInstalled())
				{
					Intent launchIntent = Activity.PackageManager.GetLaunchIntentForPackage(controllerPackage);
					StartActivity(launchIntent);
				}
				else
				{
					try 
					{
						StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=" + controllerPackage)));
					} 
					catch (Android.Content.ActivityNotFoundException) 
					{
						StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + controllerPackage)));
					}
				}
			};
			projectView = view.FindViewById<TextView>(Resource.Id.projectView);

			messagesButton = view.FindViewById<Button>(Resource.Id.messagesButton);
			if (messagesButton != null)
			{
				messagesButton.Click += delegate {
					Activity.StartActivity(typeof(MessagesActivity));
				};
			}

			DoUpdateServiceState();
			if (!PlayerServiceRunning)
			{
				StartService();
			}

			return view;
		}

		private const String controllerPackage = "ares.controller.android";

		private bool IsControllerInstalled()
		{
			return IsAppInstalled(controllerPackage);
		}

		private bool IsAppInstalled(String package)
		{
			var pm = Activity.PackageManager;
			bool installed = false;
			try
			{
				pm.GetPackageInfo(package, Android.Content.PM.PackageInfoFlags.Activities);
				installed = true;
			}
			catch (Android.Content.PM.PackageManager.NameNotFoundException)
			{
				installed = false;
			}
			return installed;
		}
	}
}

