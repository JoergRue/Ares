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
using System.Threading.Tasks;
using Jcifs.Smb;

namespace Ares.Settings
{
	public enum FolderType
	{
		FileSystem,
		SambaShare
	}

	public interface IFile
	{
		String DisplayName { get; }
		String IOName { get; }
	}

	public interface IFolder
	{
		FolderType FolderType { get; }
		String DisplayName { get; }
		Task<List<String>> GetSubFolderNames();
		Task<IFolder> GetSubFolder(String name);
		bool HasParentFolder();
		Task<IFolder> GetParentFolder();
		Task<IFolder> CreateAndGetSubFolder(String name);
		Task<List<IFile>> GetProjectNames();

		String Serialize();

		String IOName { get; }
	}

	public class FolderFactory
	{
		public const String FILE_SYSTEM_FOLDER_TAG = "FileSystemFolder:";

		public const String SMB_FOLDER_TAG = "SmbFolder:";

		public static Task<IFolder> CreateFromSerialization(String serializedForm)
		{
			if (String.IsNullOrEmpty(serializedForm))
				return Task.FromResult<IFolder>(null);
			if (serializedForm.StartsWith(FILE_SYSTEM_FOLDER_TAG))
				return Task.Factory.StartNew(() => {
					return (IFolder)(new FileSystemFolder(serializedForm.Substring(FILE_SYSTEM_FOLDER_TAG.Length)));
				});
			if (serializedForm.StartsWith(SMB_FOLDER_TAG))
				return Task.Factory.StartNew(() => {
					return (IFolder)(new SmbFolder(serializedForm.Substring(SMB_FOLDER_TAG.Length)));
				});
			return Task.FromResult<IFolder>(null);
		}

		public static Task<IFolder> CreateFileSystemFolder(String path)
		{
			return Task.Factory.StartNew(() => { return (IFolder)(new FileSystemFolder(path)); });
		}

		public static Task<IFolder> CreateSmbFolder(String path)
		{
			return Task.Factory.StartNew(() => { return (IFolder)(new SmbFolder(path)); });
		}

		public static IFile CreateFile(String ioName)
		{
			if (ioName.IsSmbFile())
			{
				try
				{
					return new SambaFile(new SmbFile(ioName));
				}
				catch (SmbException ex)
				{
					throw new System.IO.IOException(ex.Message, ex);
				}
			}
			else
			{
				return new FileSystemFile(ioName);
			}
		}

		public static Task<IFolder> CreateRootFolder(FolderType folderType)
		{
			switch (folderType)
			{
			case FolderType.SambaShare:
				return CreateSmbFolder("smb://");
			case FolderType.FileSystem:
			default:
				return CreateFileSystemFolder("/");
			}
		}
	}

	class FileSystemFile : IFile
	{
		private string mPath;

		public String DisplayName { get { return System.IO.Path.GetFileName(mPath); } }

		public String IOName { get { return mPath; } }

		public FileSystemFile(String path)
		{
			mPath = path;
		}
	}

	class FileSystemFolder : IFolder
	{
		public FolderType FolderType
		{
			get 
			{
				return FolderType.FileSystem;
			}
		}

		public String DisplayName { get { return System.IO.Path.GetFullPath(mPath); } }

		public String IOName { get { return DisplayName; } }

		public Task<List<String>> GetSubFolderNames()
		{
			return Task.Factory.StartNew(() => {
				try
				{
					var result = new List<String>();
					foreach (var dir in System.IO.Directory.GetDirectories(mPath))
					{
						result.Add(System.IO.Path.GetFileName(dir));
					}
					return result;
				}
				catch (System.IO.IOException)
				{
					return new List<String>();
				}
			});
		}

		public Task<List<IFile>> GetProjectNames() 
		{
			return Task.Factory.StartNew(() => {
				var result = new List<IFile>();
				try
				{
					foreach (var file in System.IO.Directory.GetFiles(mPath))
					{
						if (file.EndsWith(".ares", StringComparison.InvariantCultureIgnoreCase) ||
						    file.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
						{
							result.Add(new FileSystemFile(file));
						}
					}
					return result;
				}
				catch (System.IO.IOException)
				{
					return result;
				}
			});
		}

		public Task<IFolder> GetSubFolder(String name)
		{
			String path = System.IO.Path.Combine(mPath, name);
			return Task.FromResult((IFolder)(new FileSystemFolder(path)));
		}

		public bool HasParentFolder()
		{
			return System.IO.Path.GetPathRoot(mPath) != mPath;
		}

		public Task<IFolder> GetParentFolder()
		{
			return Task.Factory.StartNew(() => {
				return (IFolder)(new FileSystemFolder(System.IO.Directory.GetParent(mPath).FullName));
			});
		}

		public Task<IFolder> CreateAndGetSubFolder(String name)
		{
			return Task.Factory.StartNew(() => {
				String path = System.IO.Path.Combine(mPath, name);
				System.IO.Directory.CreateDirectory(path);
				return (IFolder)(new FileSystemFolder(path));
			});
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

