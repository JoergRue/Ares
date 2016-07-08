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
using Ares.Data.Android;

namespace Ares.Data
{
	class StringResources
	{
		public static String ExpectedAttribute
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ExpectedAttribute);}
		}
		public static String ExpectedContent
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ExpectedContent);}
		}
		public static String ExpectedDouble
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ExpectedDouble);}
		}
		public static String ExpectedInteger		
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ExpectedInteger);}
		}
		public static String ExpectedElement
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.ExpectedElement);}
		}
		public static String FileNameMustBeSet
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.FileNameMustBeSet);}
		}
		public static String InvalidVolume
		{
			get { return global::Android.App.Application.Context.Resources.GetText (Resource.String.InvalidVolume);}
		}
	}
}
