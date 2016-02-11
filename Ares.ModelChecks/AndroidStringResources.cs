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
using Ares.ModelInfo_Android;

namespace Ares.ModelInfo
{
	class StringResources
	{
		public static String Ares
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.Ares);}
		}
		public static String FileNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.FileNotFound);}
		}
		public static String ImportOverwrite
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ImportOverwrite);}
		}
		public static String InvalidImportFile
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.InvalidImportFile);}
		}
		public static String CannotPlayURLS
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.CannotPlayURLS);}
		}
		public static String ErrorReadingPlaylist
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ErrorReadingPlaylist);}
		}
		public static String PlaylistNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.PlaylistNotFound);}
		}
		public static String ConditionElementNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ConditionElementNotFound);}
		}
		public static String AwaitedElementNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.AwaitedElementNotFound);}
		}
		public static String StartedElementNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.StartedElementNotFound);}
		}
		public static String StoppedElementNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.StoppedElementNotFound);}
		}
		public static String ReferencedElementNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ReferencedElementNotFound);}
		}
		public static String TagNotFound
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.TagNotFound);}
		}
	}
}
