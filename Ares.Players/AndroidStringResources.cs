/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using Ares.Players_Android;

namespace Ares.Players
{
	class StringResources
	{
		public static String ClientConnected { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.ClientConnected); } }
		public static String ClientDisconnected { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.ClientDisconnected); } }
		public static String ClientLengthReceived { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.ClientLengthReceived); } }
		public static String ClientListenError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.ClientListenError); } }
		public static String ClientSendError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.ClientSendError); } }
		public static String CommandReceived { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.CommandReceived); } }
		public static String DisconnectingClient { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.DisconnectingClient); } }
		public static String InvalidKeyCode { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.InvalidKeyCode); } }
		public static String KeyListenError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.KeyListenError); } }
		public static String KeyReceived { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.KeyReceived); } }
		public static String LengthTooHigh { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.LengthTooHigh); } }
		public static String NoClientID { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoClientID); } }
		public static String PingReceived { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.PingReceived); } }
		public static String PingTimeout { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.PingTimeout); } }
		public static String PlayError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.PlayError); } }
		public static String StartBroadcast { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.StartBroadcast); } }
		public static String StopBroadcast { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.StopBroadcast); } }
		public static String TagsDbError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.TagsDbError); } }
		public static String UDPError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.UDPError); } }
		public static String UDPSending { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.UDPSending); } }
		public static String VolumeCommandReceived { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.VolumeCommandReceived); } }
		public static String VolumeOutOfRange { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.VolumeOutOfRange); } }
		public static String WrongClientLength { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.WrongClientLength); } }
	}
}