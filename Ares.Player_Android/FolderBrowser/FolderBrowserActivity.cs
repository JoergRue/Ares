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
using Android.Views;
using Android.Widget;
using Ares.Settings;
using System.Threading.Tasks;
using System.Threading;
using Android.Preferences;

namespace Ares.Player_Android
{
	[Activity (Label = "Ares Player", Icon = "@mipmap/icon", Theme = "@android:style/Theme.Holo.DialogWhenLarge", ExcludeFromRecents = true)]			
	public class FolderBrowserActivity : Activity
	{

		public const string FOLDER_EXTRA = "folder";

		class Authenticator : Jcifs.Smb.NtlmAuthenticator, Android.App.Application.IActivityLifecycleCallbacks
		{
			private Activity mCurrentActivity;
			private AutoResetEvent mEvent = new AutoResetEvent(false);

			private bool dialogDismissed = false;
			private Jcifs.Smb.NtlmPasswordAuthentication mAuth = null;

			protected override Jcifs.Smb.NtlmPasswordAuthentication NtlmPasswordAuthentication {
				get {
					if (RequestingURL != null && RequestingURL == "LAST_USED_AUTH")
					{
						var prefs = PreferenceManager.GetDefaultSharedPreferences(mCurrentActivity);
						String user = prefs.GetString("LastSmbUser", "");
						if (user != "")
						{
							return new Jcifs.Smb.NtlmPasswordAuthentication(prefs.GetString("LastSmbDomain", ""),
								user, prefs.GetString("LastSmbPassword", ""));
						}
					}
					Android.Util.Log.Debug("FolderBrowser", "AuthFailure Hook");
					dialogDismissed = false;
					mAuth = null;
					mCurrentActivity.RunOnUiThread(() => {
						if (mCurrentActivity == null)
						{
							mAuth = null;
							dialogDismissed = true;
							mEvent.Set();
							return;
						}
						var transaction = mCurrentActivity.FragmentManager.BeginTransaction();
						var prev = mCurrentActivity.FragmentManager.FindFragmentByTag("smbAuthDialog");
						if (prev != null)
							transaction.Remove(prev);
						transaction.AddToBackStack(null);
						AuthDialog dialog = new AuthDialog();
						dialog.OkCallback = (userName, password, domain) =>
						{
							mAuth = new Jcifs.Smb.NtlmPasswordAuthentication(domain, userName, password);
							var prefs = PreferenceManager.GetDefaultSharedPreferences(mCurrentActivity);
							var editor = prefs.Edit();
							editor.PutString("LastSmbUser", userName);
							editor.PutString("LastSmbDomain", domain);
							editor.PutString("LastSmbPassword", password);
							editor.Commit();
							dialogDismissed = true;
							mEvent.Set();
						};
						dialog.CancelCallback = () =>
						{
							mAuth = null;
							dialogDismissed = true;
							mEvent.Set();
						};
						dialog.Show(transaction, "smbAuthDialog");
					});
					while (!dialogDismissed)
						mEvent.WaitOne();
					Android.Util.Log.Debug("FolderBrowser", "AuthFailure Hook Returns" + mAuth != null ? " with auth" : " with no auth");
					return mAuth;
				}
			}

			public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
			{
				mCurrentActivity = activity;
			}

			public void OnActivityDestroyed(Activity activity)
			{
				if (activity == mCurrentActivity)
					mCurrentActivity = null;
			}

			public void OnActivityPaused(Activity activity)
			{
				if (activity == mCurrentActivity)
					mCurrentActivity = null;
			}

			public void OnActivityResumed(Activity activity)
			{
				mCurrentActivity = activity;
			}

			public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
			{
			}

			public void OnActivityStarted(Activity activity)
			{
			}

			public void OnActivityStopped(Activity activity)
			{
			}

			public Authenticator(Application owner)
			{
				owner.RegisterActivityLifecycleCallbacks(this);
			}
		}

		private Authenticator mAuthenticator;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			mAuthenticator = new Authenticator(this.Application);
			#if DEBUG
			Xamarin.Insights.Initialize(Xamarin.Insights.DebugModeKey, this);
			#else
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);
			#endif
			SetContentView(Resource.Layout.FolderBrowser);
			selectButton = FindViewById<Button>(Resource.Id.selectButton);
			selectButton.Click += delegate {
				Intent returnIntent = new Intent();
				returnIntent.PutExtra(FOLDER_EXTRA, currentFolder.Serialize());
				SetResult(Result.Ok, returnIntent);
				Finish();
			};
			upButton = FindViewById<Button>(Resource.Id.upButton);
			upButton.Click += async delegate {
				try
				{
					IFolder newFolder = await currentFolder.GetParentFolder();
					await SetFolder(newFolder);
				}
				catch (Exception ex)
				{
					Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
				}
			};
			newButton = FindViewById<Button>(Resource.Id.createButton);
			newButton.Click += delegate {
				CreateFolder();
			};
			switchTypeButton = FindViewById<Button>(Resource.Id.switchTypeButton);
			switchTypeButton.Click += async delegate {
				await SwitchFolderType();
			};
			currentFolderView = FindViewById<TextView>(Resource.Id.currentFolderView);

			folderList = FindViewById<ListView>(Resource.Id.folderList);
			folderList.ItemClick += async delegate(object sender, AdapterView.ItemClickEventArgs e) {
				try
				{
					var folderNames = await currentFolder.GetSubFolderNames();
					IFolder newFolder = await currentFolder.GetSubFolder(folderNames[e.Position]);
					await SetFolder(newFolder);
				}
				catch (Exception ex)
				{
					Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
				}
			};
			subFolders = new ArrayAdapter<string>(this, Resource.Layout.FolderListItem);
			folderList.Adapter = subFolders;

			Intent intent = this.Intent;
			String serializedFolder = intent.GetStringExtra(FOLDER_EXTRA);

			DoSetFolder(serializedFolder);
		}

		protected override void OnStop()
		{
			Jcifs.Smb.NtlmAuthenticator.SetDefault(null);
			base.OnStop();
		}

		protected override void OnStart()
		{
			base.OnStart();
			Jcifs.Smb.NtlmAuthenticator.SetDefault(mAuthenticator);
		}

		private async void DoSetFolder(String serializedFolder) 
		{
			var folder = await FolderFactory.CreateFromSerialization(serializedFolder);
			await SetFolder(folder);
		}

		private async Task SetFolder(IFolder folder)
		{
			if (folder.FolderType == FolderType.SambaShare && String.IsNullOrEmpty(folder.DisplayName))
			{
				SwitchToSmb();
				return;
			}
			currentFolder = folder;
			currentFolderView.Text = folder.DisplayName;
			upButton.Enabled = folder.HasParentFolder();
			subFolders.Clear();
			var dialog = new ProgressDialog(this);
			dialog.SetMessage(Resources.GetString(Resource.String.gettingDirs));
			dialog.Indeterminate = true;
			dialog.SetCancelable(false);
			dialog.Show();
			try 
			{
				subFolders.AddAll(await currentFolder.GetSubFolderNames());
				dialog.Dismiss();
			}
			catch (Exception ex)
			{
				dialog.Dismiss();
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}
			switch (currentFolder.FolderType)
			{
			case FolderType.SambaShare:
				switchTypeButton.Text = Resources.GetString(Resource.String.local);
				break;
			case FolderType.FileSystem:
			default:
				switchTypeButton.Text = Resources.GetString(Resource.String.network);
				break;
			}
		}

		private void CreateFolder()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle(Resource.String.createDir);

			EditText input = new EditText(this);
			input.InputType = Android.Text.InputTypes.ClassText;
			builder.SetView(input);

			builder.SetPositiveButton(Android.Resource.String.Ok, async (sender, eventArgs) =>
				{
					String text = input.Text;
					await CreateFolder(text);
				}
			);
			builder.SetNegativeButton(Android.Resource.String.Cancel, (sender, eventArgs) =>
				{
				});

			builder.Show();
		}

		private void SwitchToSmb()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle(Resource.String.selectServer);

			EditText input = new EditText(this);
			input.InputType = Android.Text.InputTypes.ClassText;
			input.Text = PreferenceManager.GetDefaultSharedPreferences(this).GetString("LastSmbServer", "");
			builder.SetView(input);

			builder.SetPositiveButton(Android.Resource.String.Ok, async (sender, eventArgs) =>
				{
					String text = input.Text;
					var editor = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
					editor.PutString("LastSmbServer", text);
					editor.Commit();
					try
					{
						IFolder newFolder = await FolderFactory.CreateRootFolder(FolderType.SambaShare);
						newFolder = await newFolder.GetSubFolder(text);
						await SetFolder(newFolder);
					}
					catch (Exception ex)
					{
						Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
					}
				}
			);
			builder.SetNegativeButton(Android.Resource.String.Cancel, async (sender, eventArgs) =>
				{
					await SwitchToLocal();
				});

			builder.Show();
		}

		private async Task SwitchToLocal()
		{
			try
			{
				IFolder newFolder = await FolderFactory.CreateRootFolder(FolderType.FileSystem);
				await SetFolder(newFolder);
			}
			catch (Exception ex)
			{
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}
		}

		private async Task CreateFolder(String name)
		{
			try 
			{
				IFolder newFolder = await currentFolder.CreateAndGetSubFolder(name);
				await SetFolder(newFolder);
			}
			catch (Exception ex)
			{
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}
		}

		private async Task SwitchFolderType()
		{
			var folderType = currentFolder.FolderType;
			switch (folderType)
			{
			case FolderType.FileSystem:
				SwitchToSmb();
				break;
			case FolderType.SambaShare:
			default:
				await SwitchToLocal();
				break;
			}
		}

		private ArrayAdapter<String> subFolders;
		private Button selectButton;
		private Button upButton;
		private Button newButton;
		private Button switchTypeButton;
		private TextView currentFolderView;
		private ListView folderList;

		private IFolder currentFolder;
	}
}

