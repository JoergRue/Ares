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
using Ares.Playing_Android;

namespace Ares.Playing
{
	class StringResources
	{
		public static String BufLostError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BufLostError); } }
		public static String CodecError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.CodecError); } }
		public static String CreateError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.CreateError); } }
		public static String DeviceError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.DeviceError); } }
		public static String DriverError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.DriverError); } }
		public static String DxError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.DxError); } }
		public static String EndedError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.EndedError); } }
		public static String FileformError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.FileformError); } }
		public static String FileOpenError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.FileOpenError); } }
		public static String FormatError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.FormatError); } }
		public static String FreqError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.FreqError); } }
		public static String HandleError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.HandleError); } }
		public static String IllParamError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.IllParamError); } }
		public static String IllTypeError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.IllTypeError); } }
		public static String MemError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.MemError); } }
		public static String No3DError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.No3DError); } }
		public static String NoChanError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoChanError); } }
		public static String NoEaxError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoEaxError); } }
		public static String NoFxError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoFxError); } }
		public static String NoHwError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoHwError); } }
		public static String NoPlayError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoPlayError); } }
		public static String NotAudioError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NotAudioError); } }
		public static String NotFileError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NotFileError); } }
		public static String PlayingError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.PlayingError); } }
		public static String PositionError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.PositionError); } }
		public static String SpeakerError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.SpeakerError); } }
		public static String UnexpectedError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.UnexpectedError); } }
		public static String FilePlayingError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.FilePlayingError); } }
		public static String SetVolumeError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.SetVolumeError); } }
		public static String SetEffectError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.SetEffectError); } }
		public static String SpeakerNotAvailable { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.SpeakerNotAvailable); } }
		public static String StreamingError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.StreamingError); } }
		public static String TagsDbError { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.TagsDbError); } }
		public static String TaggedMusic { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.TaggedMusic); } }
		public static String BassAacLoadFail { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassAacLoadFail); } }
		public static String BassDeviceAlready { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassDeviceAlready); } }
		public static String BassDeviceDriver { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassDeviceDriver); } }
		public static String BassDeviceFormat { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassDeviceFormat); } }
		public static String BassDeviceInfo { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassDeviceInfo); } }
		public static String BassDeviceInvalid { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassDeviceInvalid); } }
		public static String BassFlacLoadFail { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassFlacLoadFail); } }
		public static String BassFxLoadFail { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassFxLoadFail); } }
		public static String BassInitFail { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassInitFail); } }
		public static String BassLoadFail { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassLoadFail); } }
		public static String BassNo3D { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassNo3D); } }
		public static String BassNoMem { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassNoMem); } }
		public static String BassOpusLoadFail { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassOpusLoadFail); } }
		public static String BassUnknown { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.BassUnknown); } }
		public static String DeviceDisabled { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.DeviceDisabled); } }
		public static String DeviceEnabled { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.DeviceEnabled); } }
		public static String NoDevice { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoDevice); } }
		public static String NoDeviceDriver { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.NoDeviceDriver); } }
		public static String Default { get { return global::Android.App.Application.Context.Resources.GetText(Resource.String.Default); } }
	}
}
