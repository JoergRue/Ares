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
using Android.App;
using Android.Widget;

namespace Ares.Player_Android
{
	public class AuthDialog : DialogFragment
	{
		public AuthDialog()
		{
		}

		public Action<String, String, String> OkCallback { get; set; }
		public Action CancelCallback { get; set; }

		public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
		{
			var builder = new AlertDialog.Builder(Activity);
			var inflater = Activity.LayoutInflater;
			var view = inflater.Inflate(Resource.Layout.SmbAuth, null);
			var userNameView = view.FindViewById<EditText>(Resource.Id.userNameEdit);
			var passwordView = view.FindViewById<EditText>(Resource.Id.passwordEdit);
			var domainView = view.FindViewById<EditText>(Resource.Id.domainEdit);
			builder.SetView(view)
				.SetPositiveButton(Android.Resource.String.Ok, (sender, args) => {
					if (OkCallback != null)
						OkCallback(userNameView.Text, passwordView.Text, domainView.Text);
					Dismiss();
			})
				.SetNegativeButton(Android.Resource.String.Cancel, (sender, args) => {
					if (CancelCallback != null)
						CancelCallback();
					Dismiss();
			})
				.SetTitle(Resource.String.authDialogTitle);
			return builder.Create();
		}
	}
}

