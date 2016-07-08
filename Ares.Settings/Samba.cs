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
// #define DEBUG_SAMBA

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Jcifs.Smb;


namespace Ares.Settings
{

	class SmbOperations
	{
		public static T WithRetries<T>(Func<T> func, int retries = 2)
		{
			while (retries >= 0)
			{
				try
				{
					return func();
				}
				catch (SmbException)
				{
					if (retries == 0)
					{
						throw;
					}
				}
				catch (Java.IO.IOException)
				{
					if (retries == 0)
					{
						throw;
					}
				}
				--retries;
			}
			return default(T);
		}
	}

	// adapter because SmbInputStream is derived from Java.IO.InputStream,
	// which unfortunately isn't derived from System.IO.Stream
	class SambaInputStream : System.IO.Stream
	{
		private SmbFile mFile;
		private SmbFileInputStream mStream;
		private long mPos;

		public SambaInputStream(SmbFile smbFile)
		{
			if (smbFile == null)
				throw new ArgumentNullException("smbFile");
			mFile = smbFile;
			try 
			{
				mStream = new SmbFileInputStream(smbFile);
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
			mPos = 0;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (mStream != null)
			{
				mStream.Dispose();
			}
			mStream = null;
		}

		#region implemented abstract members of Stream
		public override void Flush()
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			try 
			{
				#if DEBUG_SAMBA
				Android.Util.Log.Debug("SambaInputStream", "Seeking with offset " + offset + " and origin " + origin);
				#endif
				long newPos = mPos;
				switch (origin)
				{
				case System.IO.SeekOrigin.Begin:
					if (offset < 0)
						throw new ArgumentOutOfRangeException("offset");
					newPos = offset;
					break;
				case SeekOrigin.Current:
					if (offset < -mPos)
						throw new ArgumentOutOfRangeException("offset");
					newPos = mPos + offset;
					break;
				case SeekOrigin.End:
					{
						long len = mFile.Length();
						if (offset > 0)
							throw new ArgumentOutOfRangeException("offset");
						newPos = len + offset;
					}
					break;
				default:
					break;
				}
				if (newPos > mPos)
				{
					mStream.Skip(newPos - mPos);
				}
				else if (newPos < mPos)
				{
					// SmbInputStream can't seek backwards; have to recreate
					mStream.Close();
					mStream = new SmbFileInputStream(mFile);
					mStream.Skip(newPos);
				}
				mPos = newPos;
				#if DEBUG_SAMBA
				Android.Util.Log.Debug("SambaInputStream", "New position is " + mPos);
				#endif
				return mPos;
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (Java.IO.IOException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		public override void SetLength(long value)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			try
			{
				int res = SmbOperations.WithRetries(() => 
					{
						return mStream.Read(buffer, offset, count);
					});
				// note: java.io.Stream.Read returns -1 on end of input, but
				// System.IO.Stream must return 0 in that case
				if (res == -1)
				{
					#if DEBUG_SAMBA
					Android.Util.Log.Debug("SambaInputStream", "End of stream reached at " + mPos + "(length is " + mFile.Length() + ")");
					#endif
					res = 0;
				}
				else
				{
					mPos += res;
					#if DEBUG_SAMBA
					Android.Util.Log.Debug("SambaInputStream", "Read " + res + " bytes from stream, new pos is " + mPos);
					#endif
				}
				return res;
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (Java.IO.IOException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			throw new NotSupportedException();
		}

		public override bool CanRead {
			get {
				return true;
			}
		}

		public override bool CanSeek {
			get {
				return true;
			}
		}

		public override bool CanWrite {
			get {
				return false;
			}
		}

		public override long Length {
			get {
				return mFile.Length();
			}
		}

		public override long Position {
			get {
				return mPos;
			}
			set {
				Seek(value, SeekOrigin.Begin);
			}
		}

		#endregion
	}

	// adapter because SmbOutputStream is derived from Java.IO.OutputStream,
	// which unfortunately isn't derived from System.IO.Stream
	class SambaOutputStream : System.IO.Stream
	{
		private SmbFile mFile;
		private SmbFileOutputStream mStream;

		public SambaOutputStream(SmbFile smbFile)
		{
			if (smbFile == null)
				throw new ArgumentNullException("smbFile");
			mFile = smbFile;
			try 
			{
				mStream = new SmbFileOutputStream(smbFile);
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (mStream != null)
			{
				mStream.Dispose();
			}
			mStream = null;
		}

		#region implemented abstract members of Stream
		public override void Flush()
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			try
			{
				int dummy = SmbOperations.WithRetries(() => { mStream.Flush(); return 0; });
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (Java.IO.IOException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (mStream == null)
				throw new ObjectDisposedException("SambaInputStream");
			try
			{
				int dummy = SmbOperations.WithRetries(() =>
					{
						mStream.Write(buffer, offset, count);
						return 0;
					});
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (Java.IO.IOException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		public override bool CanRead {
			get {
				return false;
			}
		}

		public override bool CanSeek {
			get {
				return false;
			}
		}

		public override bool CanWrite {
			get {
				return true;
			}
		}

		public override long Length {
			get {
				return mFile.Length();
			}
		}

		public override long Position {
			get {
				throw new NotSupportedException();
			}
			set {
				throw new NotSupportedException();
			}
		}

		#endregion
	}

	public static class SambaHelpers
	{
		public static bool IsSmbFile(this String filePath)
		{
			return filePath.StartsWith("smb://", StringComparison.InvariantCultureIgnoreCase);
		}

		public static String GetIOName(this SmbFile smbFile)
		{
			String path = smbFile.Path;
			if (smbFile.Principal != null && !String.IsNullOrEmpty(((NtlmPasswordAuthentication)smbFile.Principal).Username) && path.IndexOf('@') == -1)
			{
				// authentification is missing from URL -- add it
				var auth = (NtlmPasswordAuthentication)smbFile.Principal;
				String authStr = String.Empty;
				if (!String.IsNullOrEmpty(auth.Domain) && auth.Domain != "?")
				{
					authStr = auth.Domain + ";";
				}
				authStr += auth.Username;
				if (!String.IsNullOrEmpty(auth.Password))
				{
					authStr += ":" + auth.Password;
				}
				authStr += "@";
				path = "smb://" + authStr + path.Substring(6);
			}
			return path;
		}

		public static System.IO.Stream GetSambaInputStream(String smbUrl)
		{
			#if DEBUG_SAMBA
			Android.Util.Log.Debug("SmbClient", "Creating input stream for " + smbUrl);
			#endif
			return new SambaInputStream(new SmbFile(smbUrl));
		}

		public static System.IO.Stream GetSambaOutputStream(String smbUrl)
		{
			return new SambaOutputStream(new SmbFile(smbUrl));
		}

		public static bool FileExists(String smbUrl)
		{
			try
			{
				return SmbOperations.WithRetries(() =>
					{
						var smbFile = new SmbFile(smbUrl);
						return smbFile.Exists();
					});
			}
			catch (SmbException)
			{
				return false;
			}
			catch (Java.IO.IOException)
			{
				return false;
			}
		}

		public static String AppendFileName(String dir, String name)
		{
			return dir + "/" + name;
		}

		public static String GetDirectoryName(String fileUrl)
		{
			try
			{
				var file = new SmbFile(fileUrl);
				return file.Parent;
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		public static void CreateDirectory(String dirUrl)
		{
			try
			{
				var file = new SmbFile(dirUrl);
				if (!file.Exists())
				{
					file.Mkdirs();
				}
			}
			catch (SmbException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (Java.IO.IOException ex)
			{
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		private static void LogException(Exception ex)
		{
			Android.Util.Log.Debug("Ares", "Exception: " + ex.GetType().Name + ": " + ex.Message);
			Exception last = ex;
			Exception inner = ex.InnerException;
			while (inner != null)
			{
				Android.Util.Log.Debug("Ares", "    Inner: " + inner.GetType().Name + ":  " + inner.Message);
				last = inner;
				inner = inner.InnerException;
			}
			Android.Util.Log.Debug("Ares", "    Stack Trace: " + last.StackTrace);
		}

		public static byte[] GetFileContent(String smbUrl)
		{
			#if DEBUG_SAMBA
			Android.Util.Log.Debug("SmbClient", "Getting file content of " + smbUrl);
			#endif
			byte[] buffer = null;
			try
			{
				var file = new Jcifs.Smb.SmbFile(smbUrl);
				using (var sis = new System.IO.BufferedStream(new SambaInputStream(file)))
				{
					var length = file.Length();
					buffer = new byte[length];
					sis.Read(buffer, 0, (int)length); // will internally make several tries
				}
			}
			catch (Jcifs.Smb.SmbException ex)
			{
				LogException(ex);
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (Java.IO.IOException ex)
			{
				LogException(ex);
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (System.IO.IOException ex)
			{
				LogException(ex);
				throw;
			}
			return buffer;
		}
	}

	class SambaFile : IFile
	{
		private SmbFile mFile; 
		public String DisplayName { get { return mFile.Name; } }

		public String IOName { get { return mFile.GetIOName(); } }

		public SambaFile(SmbFile file)
		{
			mFile = file;
		}
	}

	class SmbFolder : IFolder
	{
		public FolderType FolderType
		{
			get 
			{
				return FolderType.SambaShare;
			}
		}

		public String DisplayName 
		{ 
			get 
			{
				return mSmbFile != null ? mSmbFile.UncPath : String.Empty;
			} 
		}

		public String IOName { 
			get 
			{
				if (mSmbFile == null)
					return String.Empty;
				return mSmbFile.GetIOName();
			} 
		}

		private static void LogException(SmbException ex)
		{
			Android.Util.Log.Debug("SmbClient", "Exception: " + ex.GetType().Name + ": " + ex.Message);		if (ex.RootCause != null)
				Android.Util.Log.Debug("SmbClient", "    Root Cause: " + ex.RootCause.Class.Name + ": " + ex.RootCause.Message);
			Exception inner = ex.InnerException;
			while (inner != null)
			{
				Android.Util.Log.Debug("SmbClient", "    Inner: " + inner.GetType().Name + ":  " + inner.Message);
				inner = inner.InnerException;
			}
		}

		public SmbFolder(String path)
		{
			try 
			{
				#if DEBUG_SAMBA
				Android.Util.Log.Debug("SmbClient", "Create new file with path: " + path);
				#endif
				mSmbFile = new SmbFile(path);
			}
			catch (SmbException ex)
			{
				LogException(ex);
				throw new System.IO.IOException(ex.Message, ex);
			}
			catch (Java.Net.MalformedURLException)
			{
				mSmbFile = new SmbFile("smb://");
			}
		}

		private SmbFolder(SmbFile smbFile)
		{
			#if DEBUG_SAMBA
			if (smbFile != null)
				Android.Util.Log.Debug("SmbClient", "Create file with smb path: " + smbFile.Path);
			#endif
			mSmbFile = smbFile;
		}

		private SmbFolder(SmbFile parent, String subPath)
		{
			try
			{
				if (!subPath.EndsWith("/"))
					subPath = subPath + "/";
				if (parent == null || String.IsNullOrEmpty(parent.Server))
				{
					#if DEBUG_SAMBA
					Android.Util.Log.Debug("SmbClient", "Create file with root path: " + "smb://" + subPath);
					#endif
					if (parent.Principal == null || String.IsNullOrEmpty(((NtlmPasswordAuthentication)parent.Principal).Username))
					{
						NtlmPasswordAuthentication auth = NtlmAuthenticator.RequestNtlmPasswordAuthentication("LAST_USED_AUTH", null);
						if (auth != null)
						{
							mSmbFile = new SmbFile("smb://" + subPath, auth);
						}
						else
						{
							mSmbFile = new SmbFile("smb://" + subPath, new NtlmPasswordAuthentication(String.Empty, "GUEST", String.Empty));	
						}
					}
					else
					{
						mSmbFile = new SmbFile("smb://" + subPath, (NtlmPasswordAuthentication)parent.Principal);
					}
				}
				else
				{
					#if DEBUG_SAMBA
					Android.Util.Log.Debug("SmbClient", "Create file with sub path: " + parent.Path + "/" + subPath);
					#endif
					mSmbFile = new SmbFile(parent, subPath);
				}
			}
			catch (SmbException ex)
			{
				LogException(ex);
				throw new System.IO.IOException(ex.Message, ex);
			}
		}

		public Task<List<String>> GetSubFolderNames()
		{
			if (mSmbFile == null)
				return Task.FromResult(new List<String>());

			return Task.Factory.StartNew(() => {
				var result = new List<String>();
				try
				{
					bool hasTriedLastAuth = false;
					SmbOperations.WithRetries(() =>
					{
						result.Clear();
						try 
						{
							#if DEBUG_SAMBA
							Android.Util.Log.Debug("SmbClient", "Getting sub folders");
							#endif

							var subList = mSmbFile.ListFiles();
							foreach (var element in subList)
							{
								try
								{
									if (!element.IsFile)
									{
										switch (element.Type)
										{
										case SmbFile.TypeFilesystem:
										case SmbFile.TypeWorkgroup:
										case SmbFile.TypeServer:
										case SmbFile.TypeShare:
											result.Add(element.Name);
											break;
										default:
											break;
										}
									}
								}
								catch (SmbException)
								{
								}
								catch (Java.IO.IOException)
								{
								}
							}
							return result;
						}
						catch (SmbAuthException ex)
						{
							NtlmPasswordAuthentication auth = null;
							if (!hasTriedLastAuth)
							{
								auth = NtlmAuthenticator.RequestNtlmPasswordAuthentication("LAST_USED_AUTH", ex);
								hasTriedLastAuth = true;
							}
							else
							{
								auth = NtlmAuthenticator.RequestNtlmPasswordAuthentication(mSmbFile.Path, ex);
							}
							if (auth != null)
							{
								mSmbFile = new SmbFile(mSmbFile.Path, auth);
								throw;
							}
							else
							{
								return result;
							}
						}
					}, 5);
				}
				catch (SmbException ex)
				{
					LogException(ex);
					throw new System.IO.IOException(ex.Message, ex);
				}
				catch (Java.IO.IOException ex)
				{
					throw new System.IO.IOException(ex.Message, ex);
				}
				return result;
			});
		}

		public Task<List<IFile>> GetProjectNames()
		{
			if (mSmbFile == null)
				return Task.FromResult(new List<IFile>());

			return Task.Factory.StartNew(() => {
				var result = new List<IFile>();
				try
				{
					SmbOperations.WithRetries(() =>
					{
						result.Clear();
						#if DEBUG_SAMBA
						Android.Util.Log.Debug("SmbClient", "Getting projects");
						#endif

						var subList = mSmbFile.ListFiles();
						foreach (var element in subList)
						{
							try
							{
								if (element.IsFile && (
									element.Name.EndsWith(".ares", StringComparison.InvariantCultureIgnoreCase) 
									|| element.Name.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase)))
								{
									#if DEBUG_SAMBA
									Android.Util.Log.Debug("SmbClient", "Found project " + element.Name);
									#endif
									result.Add(new SambaFile(element));
								}
							}
							catch (SmbException)
							{
							}
							catch (Java.IO.IOException)
							{
							}
						}
						return result;
					}, 3);
				}
				catch (SmbException ex)
				{
					LogException(ex);
					throw new System.IO.IOException(ex.Message, ex);
				}
				catch (Java.IO.IOException ex)
				{
					throw new System.IO.IOException(ex.Message, ex);
				}
				#if DEBUG_SAMBA
				Android.Util.Log.Debug("SmbClient", "Returning " + result.Count + " files.");
				#endif
				return result;
			});
		}

		public Task<IFolder> GetSubFolder(String name)
		{
			if (mSmbFile == null)
				return Task.FromResult((IFolder)this);

			return Task.Factory.StartNew(() => { return (IFolder)(new SmbFolder(mSmbFile, name)); });
		}

		public bool HasParentFolder()
		{
			return mSmbFile != null && !String.IsNullOrEmpty(mSmbFile.Server);
		}

		public Task<IFolder> GetParentFolder()
		{
			if (mSmbFile == null || String.IsNullOrEmpty(mSmbFile.Server) || String.IsNullOrEmpty(mSmbFile.Share))
				return Task.FromResult((IFolder)new SmbFolder((SmbFile)null));

			return Task.Factory.StartNew(() => {
				try {
					return (IFolder)(new SmbFolder(new SmbFile(mSmbFile.Parent, (NtlmPasswordAuthentication)mSmbFile.Principal)));
				}
				catch (SmbException ex) {
					LogException(ex);
					throw new System.IO.IOException(ex.Message, ex);
				}
			});
		}

		public Task<IFolder> CreateAndGetSubFolder(String name)
		{
			if (mSmbFile == null)
				return Task.FromResult((IFolder)this);

			return Task.Factory.StartNew(() => {
				try
				{
					var file = new SmbFile(mSmbFile, name);
					file.Mkdir();
				}
				catch (SmbException ex)
				{
					LogException(ex);
					throw new System.IO.IOException(ex.Message, ex);
				}
				catch (Java.IO.IOException ex)
				{
					throw new System.IO.IOException(ex.Message, ex);
				}
				return (IFolder)(new SmbFolder(mSmbFile, name));
			});
		}

		public String Serialize()
		{
			return FolderFactory.SMB_FOLDER_TAG + IOName;
		}

		private SmbFile mSmbFile;
	}

}

