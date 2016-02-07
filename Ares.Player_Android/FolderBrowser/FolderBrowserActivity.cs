
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

namespace Ares.Player_Android
{
	[Activity (Label = "Ares Player", Icon = "@mipmap/icon", Theme = "@android:style/Theme.Holo.DialogWhenLarge", ExcludeFromRecents = true)]			
	public class FolderBrowserActivity : Activity
	{

		public const string FOLDER_EXTRA = "folder";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			#if DEBUG
			Xamarin.Insights.Initialize (Xamarin.Insights.DebugModeKey, this);
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
			upButton.Click += delegate {
				try 
				{
					IFolder newFolder = currentFolder.GetParentFolder();
					SetFolder(newFolder);
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
			currentFolderView = FindViewById<TextView>(Resource.Id.currentFolderView);

			folderList = FindViewById<ListView>(Resource.Id.folderList);
			folderList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) {
				try
				{
					IFolder newFolder = currentFolder.GetSubFolder(currentFolder.GetSubFolderNames()[e.Position]);
					SetFolder(newFolder);
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

			SetFolder(FolderFactory.CreateFromSerialization(serializedFolder));
		}

		private void SetFolder(IFolder folder)
		{
			currentFolder = folder;
			currentFolderView.Text = folder.DisplayName;
			upButton.Enabled = folder.HasParentFolder();
			subFolders.Clear();
			subFolders.AddAll(currentFolder.GetSubFolderNames());
		}

		private void CreateFolder()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle(Resource.String.createDir);

			EditText input = new EditText(this);
			input.InputType = Android.Text.InputTypes.ClassText;
			builder.SetView(input);

			builder.SetPositiveButton(Android.Resource.String.Ok, (sender, eventArgs) =>
				{
					String text = input.Text;
					CreateFolder(text);
				}
			);
			builder.SetNegativeButton(Android.Resource.String.Cancel, (sender, eventArgs) =>
				{
				});

			builder.Show();
		}

		private void CreateFolder(String name)
		{
			try 
			{
				IFolder newFolder = currentFolder.CreateAndGetSubFolder(name);
				SetFolder(newFolder);
			}
			catch (Exception ex)
			{
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}
		}

		private ArrayAdapter<String> subFolders;
		private Button selectButton;
		private Button upButton;
		private Button newButton;
		private TextView currentFolderView;
		private ListView folderList;

		private IFolder currentFolder;
	}
}

