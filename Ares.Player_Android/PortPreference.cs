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
using Android.Preferences;
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.OS;

namespace Ares.Player_Android
{
	public class PortPreference : DialogPreference
	{
		private int mValue;
		private int mDefaultValue = 8009;

		public PortPreference(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			Init(context, attrs);
		}

		public PortPreference(Context context, IAttributeSet attrs, int defaultStyle)
			: base(context, attrs, defaultStyle)
		{
			Init(context, attrs);
		}

		public PortPreference(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		private void Init(Context context, IAttributeSet attrs)
		{
			var ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.PortPreference);
			string portType = ta.GetString(Resource.Styleable.PortPreference_portType);
			ta.Recycle();
			if (portType == "udp")
				mDefaultValue = 8009;
			else if (portType == "tcp")
				mDefaultValue = 11112;

			PositiveButtonText = context.Resources.GetString(Android.Resource.String.Ok);
			NegativeButtonText = context.Resources.GetString(Android.Resource.String.Cancel);
			DialogIcon = null;
		}

		private Android.Widget.NumberPicker mNumberPicker;

		protected override void OnPrepareDialogBuilder(Android.App.AlertDialog.Builder builder)
		{
			base.OnPrepareDialogBuilder(builder);

			mNumberPicker = new NumberPicker(Context);
			mNumberPicker.MinValue = 1000;
			mNumberPicker.MaxValue = 20000;
			mNumberPicker.Value = mValue;
			mNumberPicker.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);

			var linearLayout = new LinearLayout(Context);
			linearLayout.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
			linearLayout.SetGravity(Android.Views.GravityFlags.Center);
			linearLayout.AddView(mNumberPicker);

			builder.SetView(linearLayout);
		}

		protected override void OnDialogClosed(bool positiveResult)
		{
			base.OnDialogClosed(positiveResult);
			if (positiveResult && mNumberPicker != null)
			{
				int value = mNumberPicker.Value;
				if (CallChangeListener(value))
				{
					mValue = value;
					PersistInt(mValue);
					Summary = mValue.ToString();
				}
			}
		}

		protected override void OnSetInitialValue(bool restorePersistedValue, Java.Lang.Object defaultValue)
		{
			if (restorePersistedValue)
			{
				mValue = GetPersistedInt(mDefaultValue);
			}
			else
			{
				mValue = mDefaultValue;
				PersistInt(mValue);
			}
			Summary = mValue.ToString();
		}

		protected override Java.Lang.Object OnGetDefaultValue(Android.Content.Res.TypedArray a, int index)
		{
			mDefaultValue = a.GetInteger(index, mDefaultValue);
			return mDefaultValue;
		}

		private class SavedState : BaseSavedState {

			public int Value { get; set; }

			public SavedState(IParcelable superState) 
				: base(superState) {
			}

			public SavedState(Parcel source) 
				: base(source)
			{
				Value = source.ReadInt();
			}

			public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags) {
				base.WriteToParcel(dest, flags);
				dest.WriteInt(Value);
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
			myState.Value = mValue;
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
			mValue = myState.Value;
			base.OnRestoreInstanceState(myState.SuperState);
		}
	}
}

