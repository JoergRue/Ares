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
using Android.Content;
using Android.Util;
using Android.Preferences;
using Android.App;
using Android.OS;

namespace Ares.Player_Android
{
	public class FolderPreference : Preference
	{
		private String mFolderType;
		private Settings.IFolder mFolder;

		public Fragment ParentFragment { get; set; }

		public int FolderId { get; set; }

		public void OnActivityResult(Intent data)
		{
			String serialized = data.GetStringExtra(FolderBrowserActivity.FOLDER_EXTRA);
			Settings.IFolder folder = Settings.FolderFactory.CreateFromSerialization(serialized);
			if (folder != null && CallChangeListener(folder.Serialize()))
			{
				mFolder = folder;
				PersistString(mFolder.Serialize());
				Summary = mFolder.IOName;
			}
		}

		public FolderPreference(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			Init(context, attrs);
		}

		public FolderPreference(Context context)
			: base(context)
		{
			mFolderType = "Unknown";
		}

		public FolderPreference(Context context, IAttributeSet attrs, int defStyle)
			: base(context, attrs, defStyle)
		{
			Init(context, attrs);
		}

		public FolderPreference(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		private void Init(Context context, IAttributeSet attrs)
		{
			var ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.FolderPreference);
			mFolderType = ta.GetString(Resource.Styleable.FolderPreference_folderType);
			ta.Recycle();
			mFolder = RetrieveDefaultValue();
			Summary = mFolder != null ? mFolder.IOName : String.Empty;
		}

		protected override void OnClick()
		{
			Intent intent = new Intent(this.Context, typeof(FolderBrowserActivity));
			if (mFolder != null)
			{
				intent.PutExtra(FolderBrowserActivity.FOLDER_EXTRA, mFolder.Serialize());
			}
			else
			{
				var folder = Settings.FolderFactory.CreateFileSystemFolder("/");
				intent.PutExtra(FolderBrowserActivity.FOLDER_EXTRA, folder.Serialize());
			}
			ParentFragment.StartActivityForResult(intent, FolderId);
		}

		protected override void OnSetInitialValue(bool restorePersistedValue, Java.Lang.Object defaultValue)
		{
			if (restorePersistedValue)
			{
				mFolder = Settings.FolderFactory.CreateFromSerialization(GetPersistedString(RetrieveDefaultValue().Serialize()));
			}
			else
			{
				mFolder = RetrieveDefaultValue();
				if (mFolder != null)
				{
					PersistString(mFolder.Serialize());
				}
			}
			Summary = mFolder != null ? mFolder.IOName : String.Empty;
		}

		protected override Java.Lang.Object OnGetDefaultValue(Android.Content.Res.TypedArray a, int index)
		{
			return RetrieveDefaultValue().Serialize();
		}

		private Settings.IFolder RetrieveDefaultValue()
		{
			if (mFolderType == "Music")
			{
				return Ares.Settings.Settings.GetDefaultMusicDirectory();
			}
			else if (mFolderType == "Sounds")
			{
				return Ares.Settings.Settings.GetDefaultSoundDirectory();
			}
			else if (mFolderType == "Projects")
			{
				return Ares.Settings.Settings.GetDefaultProjectDirectory();
			}
			else
			{
				return null;
			}
		}

		private class SavedState : BaseSavedState {

			public  String Value { get; set; }

			public SavedState(IParcelable superState) 
				: base(superState) {
			}

			public SavedState(Parcel source) 
				: base(source)
			{
				Value = source.ReadString();
			}

			public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags) {
				base.WriteToParcel(dest, flags);
				dest.WriteString(Value);
			}

			private class MyCreator : Java.Lang.Object, IParcelableCreator
			{
				public Java.Lang.Object CreateFromParcel(Parcel p) {
					return new SavedState(p);
				}

				public Java.Lang.Object[] NewArray(int size) {
					return new SavedState[size];
				}
			}

			public static IParcelableCreator CREATOR = new MyCreator();
		}

		protected override IParcelable OnSaveInstanceState()
		{
			var superState = base.OnSaveInstanceState();
			if (this.Persistent)
				return superState;
			var myState = new SavedState(superState);
			myState.Value = mFolder != null ? mFolder.Serialize() : String.Empty;
			return myState;
		}

		protected override void OnRestoreInstanceState(IParcelable state)
		{
			if (state == null || !(state is SavedState))
			{
				base.OnRestoreInstanceState(state);
				return;
			}
			SavedState myState = state as SavedState;
			mFolder = Settings.FolderFactory.CreateFromSerialization(myState.Value);
			base.OnRestoreInstanceState(myState.SuperState);
		}
	}
}

