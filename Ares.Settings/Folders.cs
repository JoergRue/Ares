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

