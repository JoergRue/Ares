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

namespace Ares.Settings
{
	public interface IFolder
	{
		String DisplayName { get; }
		List<String> GetSubFolderNames();
		IFolder GetSubFolder(String name);
		bool HasParentFolder();
		IFolder GetParentFolder();
		IFolder CreateAndGetSubFolder(String name);

		String Serialize();

		String IOName { get; }
	}

	public class FolderFactory
	{
		public const String FILE_SYSTEM_FOLDER_TAG = "FileSystemFolder:";

		public static IFolder CreateFromSerialization(String serializedForm)
		{
			if (String.IsNullOrEmpty(serializedForm))
				return null;
			if (serializedForm.StartsWith(FILE_SYSTEM_FOLDER_TAG))
				return new FileSystemFolder(serializedForm.Substring(FILE_SYSTEM_FOLDER_TAG.Length));
			return null;
		}

		public static IFolder CreateFileSystemFolder(String path)
		{
			return new FileSystemFolder(path);
		}
	}

	class FileSystemFolder : IFolder
	{
		public String DisplayName { get { return System.IO.Path.GetFullPath(mPath); } }

		public String IOName { get { return DisplayName; } }

		public List<String> GetSubFolderNames()
		{
			try 
			{
				return new List<String>(System.IO.Directory.GetDirectories(mPath));
			}
			catch (System.IO.IOException)
			{
				return new List<String>();
			}
		}

		public IFolder GetSubFolder(String name)
		{
			String path = System.IO.Path.Combine(mPath, name);
			return new FileSystemFolder(path);
		}

		public bool HasParentFolder()
		{
			return System.IO.Path.GetPathRoot(mPath) != mPath;
		}

		public IFolder GetParentFolder()
		{
			return new FileSystemFolder(System.IO.Directory.GetParent(mPath).FullName);
		}

		public IFolder CreateAndGetSubFolder(String name)
		{
			String path = System.IO.Path.Combine(mPath, name);
			System.IO.Directory.CreateDirectory(path);
			return new FileSystemFolder(path);
		}

		public FileSystemFolder(String path)
		{
			mPath = path;
		}

		public String Serialize()
		{
			return FolderFactory.FILE_SYSTEM_FOLDER_TAG + mPath;
		}

		private String mPath;
	}
}

