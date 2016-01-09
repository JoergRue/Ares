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
using Ares.Players;
using Ares.Playing;
using Ares.Settings;
using Ares.Data;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace Ares.Player_Android
{
	[Service]
	[IntentFilter(new string[]{"de.joerg_ruedenauer.ares.PlayerService"})]
	public class PlayerService : Service, INetworkClient
	{
		public PlayerService ()
		{
		}

		Notification.Builder mNotificationBuilder = null;
		readonly int mNotificationId = 1;

		private void InitNotification()
		{
			if (mNotificationBuilder == null)
			{
				mNotificationBuilder = new Notification.Builder(this);
				mNotificationBuilder.SetSmallIcon(Resource.Drawable.Ares);
				mNotificationBuilder.SetOngoing(true);
				mNotificationBuilder.SetContentTitle(Resources.GetString(Resource.String.service_running));
				TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
				stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
				Intent resultIntent = new Intent(this, typeof(MainActivity));
				stackBuilder.AddNextIntent(resultIntent);
				PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
				mNotificationBuilder.SetContentIntent(resultPendingIntent);
				mNotificationBuilder.SetPriority((int)NotificationPriority.Low);
			}
		}

		private void UpdateNotification(bool firstNotification)
		{
			InitNotification();
			String secondLine = m_Network.ClientConnected ? 
				String.Format(Resources.GetString(Resource.String.connected_with), m_Network.ClientName) : 
				Resources.GetString(Resource.String.not_connected);
			mNotificationBuilder.SetContentText(secondLine);
			var style = new Notification.BigTextStyle(mNotificationBuilder);
			String project = m_Project != null ? 
				String.Format(Resources.GetString(Resource.String.loaded_project), m_Project.Title) :
				Resources.GetString(Resource.String.no_project);
			style.BigText(secondLine + "\n" + project);
			mNotificationBuilder.SetStyle(style);
			mNotificationBuilder.SetProgress(0, 0, false);
			if (firstNotification)
			{
				StartForeground(mNotificationId, mNotificationBuilder.Build());
			}
			else
			{
				var nMgr = (NotificationManager)GetSystemService(NotificationService);
				var notification = mNotificationBuilder.Build();
				notification.Flags |= NotificationFlags.ForegroundService;
				nMgr.Notify(mNotificationId, notification);
			}
		}

		private void UpdateProgressNotification(int percent, String text)
		{
			InitNotification();
			mNotificationBuilder.SetProgress(100, percent, false);
			if (!String.IsNullOrEmpty(text))
			{
				var style = new Notification.BigTextStyle(mNotificationBuilder);
				style.BigText(Resources.GetString(Resource.String.importing_project) + "\n" + text);
				mNotificationBuilder.SetStyle(style);
			}
			else
			{
				mNotificationBuilder.SetContentText(Resources.GetString(Resource.String.importing_project));
				mNotificationBuilder.SetStyle(null);
			}
			var nMgr = (NotificationManager)GetSystemService(NotificationService);
			nMgr.Notify(mNotificationId, mNotificationBuilder.Build());
		}

		private void RemoveNotification()
		{
			if (mNotificationBuilder != null)
			{
				StopForeground(true);
				//var nMgr = (NotificationManager)GetSystemService(NotificationService);
				//nMgr.Cancel(mNotificationId);
			}
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			try 
			{
				m_BassInit = new BassInit(-1, (w) => { Toast.MakeText(this, w, ToastLength.Long).Show(); });
				Initialize();
				UpdateNotification(true);
			}
			catch (BassInitException ex) 
			{
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();	
			}
		}

		public override void OnDestroy ()
		{
			Shutdown();
			if (m_BassInit != null)
			{
				m_BassInit.Dispose();
			}

			RemoveNotification();

			base.OnDestroy();
		}

		public override StartCommandResult OnStartCommand (Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			return StartCommandResult.Sticky;
		}

		public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			return null;
		}

		private INetworks m_Network;
		private PlayingControl m_PlayingControl;
		private Ares.Playing.BassInit m_BassInit;

		private IProject m_Project;
		private int m_TagLanguageId;

		private System.Timers.Timer m_BroadcastTimer;
		private bool warnOnNetworkFail = true;

		private Object m_LockObject = new Object();

		private Handler m_Handler;

		private void Initialize()
		{
			m_Handler = new Handler();
			m_PlayingControl = new PlayingControl();
			ReadSettings();
			Settings.Settings.Instance.MessageFilterLevel = 1;
			Messages.Instance.MessageReceived += new MessageReceivedHandler(MessageReceived);
			if (Ares.Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
			{
				OpenProject(Ares.Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath, false);
			}
			bool foundAddress = false;
			bool foundIPv4Address = false;
			String ipv4Address = String.Empty;
			String ipAddress = String.Empty;
			foreach (System.Net.IPAddress address in System.Net.Dns.GetHostAddresses(String.Empty))
			{
				//if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
				//    continue;
				if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					foundIPv4Address = true;
					ipv4Address = address.ToString();
				}
				String s = address.ToString();
				if (s == Settings.Settings.Instance.IPAddress)
				{
					foundAddress = true;
					ipAddress = s;
				}
				else if (!foundAddress)
				{
					ipAddress = s; // take first one
				}
			}
			if (!foundAddress && foundIPv4Address)
			{
				ipAddress = ipv4Address; // prefer v4
			}
			if (!String.IsNullOrEmpty(ipAddress))
			{
				Settings.Settings.Instance.IPAddress = ipAddress;
			}
			else 
			{
				Toast.MakeText(this, Resource.String.no_ip_address, ToastLength.Long).Show();
				return;
			}
			m_Network = new Networks(this, Settings.Settings.Instance.UseLegacyNetwork, Settings.Settings.Instance.UseWebNetwork);
			m_Network.InitConnectionData();
			m_Network.StartUdpBroadcast();
			m_BroadcastTimer = new System.Timers.Timer(50);
			m_BroadcastTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_BroadcastTimer_Elapsed);
			m_BroadcastTimer.Enabled = true;
			m_Network.ListenForClient();
		}

		private void ShowToast(int id)
		{
			m_Handler.Post(() => {
				Toast.MakeText(this, id, ToastLength.Long).Show();
			});
		}

		private void ShowToast(String text)
		{
			m_Handler.Post(() => {
				Toast.MakeText(this, text, ToastLength.Long).Show();
			});
		}

		private void m_BroadcastTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (m_Network != null && !m_Network.SendUdpPacket())
			{
				if (warnOnNetworkFail)
				{
					warnOnNetworkFail = false;
					ShowToast(Resource.String.no_status_info_error);
				}
			}
		}

		private void Shutdown()
		{
			StopAllPlaying();
			if (m_Network != null)
			{
				if (m_Network.ClientConnected)
				{
					m_Network.DisconnectClient(false);
				}
				else
				{
					m_Network.StopListenForClient();
					System.Threading.Thread.Sleep(400);
					if (m_Network.ClientConnected)
					{
						m_Network.DisconnectClient(false);
					}
				}
				m_BroadcastTimer.Enabled = false;
				m_Network.StopUdpBroadcast();
				m_Network.Shutdown();
			}
			if (m_PlayingControl != null)
			{
				m_PlayingControl.Dispose();
			}
		}

		private void ReadSettings()
		{
			bool hasSettings = Ares.Settings.Settings.Instance.Initialize();
			if (!hasSettings)
			{
				Toast.MakeText(this, Resource.String.no_settings, ToastLength.Long).Show();
				SettingsChanged(true);
			}
			else
			{
				SettingsChanged(true);
			}
		}

		private void EnsureDirectoryExists(String directory)
		{
			if (!System.IO.Directory.Exists(directory))
			{
				try
				{
					System.IO.Directory.CreateDirectory(directory);
				}
				catch (Exception ex)
				{
					Toast.MakeText(this, String.Format(Resources.GetString(Resource.String.directory_error), directory, ex.Message), ToastLength.Long).Show();
				}
			}
		}

		private void EnsureDirectoriesExist(Ares.Settings.Settings settings)
		{
			EnsureDirectoryExists(settings.MusicDirectory);
			EnsureDirectoryExists(settings.SoundDirectory);
			EnsureDirectoryExists(settings.ProjectDirectory);
		}

		private void SettingsChanged(bool fundamentalChange)
		{
			Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
			if (fundamentalChange)
			{
				m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Escape);
				EnsureDirectoriesExist(settings);
				m_PlayingControl.UpdateDirectories();
				LoadTagsDB();
			}
			m_PlayingControl.GlobalVolume = settings.GlobalVolume;
			m_PlayingControl.MusicVolume = settings.MusicVolume;
			m_PlayingControl.SoundVolume = settings.SoundVolume;
			m_PlayingControl.SetMusicTagFading(settings.TagMusicFadeTime, settings.TagMusicFadeOnlyOnChange);
			m_PlayingControl.SetPlayMusicOnAllSpeakers(settings.PlayMusicOnAllSpeakers);
			m_PlayingControl.SetFadingOnPreviousNext(settings.ButtonMusicFadeMode != 0, settings.ButtonMusicFadeMode == 2, settings.ButtonMusicFadeTime);
			if (fundamentalChange)
			{
				if (m_Network != null && m_Network.ClientConnected)
				{
					m_Network.DisconnectClient(true);
				}
			}
		}

		private void LoadTagsDB()
		{
			try
			{
				Ares.Tags.ITagsDBFiles tagsDBFiles = Ares.Tags.TagsModule.GetTagsDB().FilesInterface;
				String path = System.IO.Path.Combine(Ares.Settings.Settings.Instance.MusicDirectory, tagsDBFiles.DefaultFileName);
				tagsDBFiles.OpenOrCreateDatabase(path);
			}
			catch (Ares.Tags.TagsDbException ex)
			{
				Toast.MakeText(this, String.Format(Resources.GetString(Resource.String.tags_db_error), ex.Message), ToastLength.Long).Show();
			}
		}

		private void StopAllPlaying()
		{
			Ares.Playing.PlayingModule.ProjectPlayer.StopAll();
			if (Ares.Playing.PlayingModule.Streamer.IsStreaming)
			{
				Ares.Playing.PlayingModule.Streamer.EndStreaming();
			}
		}

		private void OpenProjectFromController(String fileName)
		{
			String path = fileName;
			if (!System.IO.Path.IsPathRooted(path))
			{
				String oldPath = m_Project != null ? m_Project.FileName : Settings.Settings.Instance.ProjectDirectory;
				if (m_Project != null)
				{
					oldPath = System.IO.Directory.GetParent(oldPath).FullName;
				}
				path = oldPath + System.IO.Path.DirectorySeparatorChar + fileName;
			}
			if (System.IO.File.Exists(path) && !path.Equals(m_CurrentProjectPath, StringComparison.OrdinalIgnoreCase))
			{
				if (path.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
				{
					ImportProject(path, true);
				}
				else
				{
					OpenProject(path, true);
				}
			}
		}

		private void OpenProject(String filePath, bool onControllerRequest)
		{
			StopAllPlaying();
			if (m_Project != null)
			{
				if (onControllerRequest && m_Project.FileName.Equals(filePath, StringComparison.InvariantCultureIgnoreCase))
				{
					if (m_Network != null)
					{
						m_Network.InformClientOfProject(m_Project);
					}
					return;
				}
				Ares.Data.DataModule.ProjectManager.UnloadProject(m_Project);
				m_Project = null;
				m_TagLanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
			}
			try
			{
				Messages.AddMessage(Ares.Players.MessageType.Debug, "Opening project " + filePath);
				m_Project = Ares.Data.DataModule.ProjectManager.LoadProject(filePath);
				m_CurrentProjectPath = filePath;
				if (m_Project != null && m_Project.TagLanguageId != -1)
				{
					m_TagLanguageId = m_Project.TagLanguageId;
				}
				else
				{
					m_TagLanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
				}
				Ares.Settings.Settings.Instance.RecentFiles.AddFile(new Ares.Settings.RecentFiles.ProjectEntry(m_Project.FileName, m_Project.Title));
			}
			catch (Exception e)
			{
				ShowToast(String.Format(Resources.GetString(Resource.String.load_error, e.Message)));
				if (onControllerRequest)
				{
					m_Network.ErrorOccurred(-1, String.Format(Resources.GetString(Resource.String.load_error, e.Message)));
				}
				m_Project = null;
				m_CurrentProjectPath = String.Empty;
			}
			Ares.Playing.PlayingModule.ProjectPlayer.SetProject(m_Project);
			DoModelChecks();
			UpdateNotification(false);
			if (m_Network != null)
			{
				m_Network.InformClientOfProject(m_Project);
				if (m_Project != null)
				{
					m_Network.InformClientOfPossibleTags(m_TagLanguageId, m_Project);
				}
			}
		}

		class NotificationProgressMonitor : Ares.ModelInfo.IProgressMonitor
		{
			public bool Canceled
			{
				get { return false; }
			}

			public void IncreaseProgress(double percent)
			{
			}

			public void IncreaseProgress(double percent, string text)
			{
			}

			public void SetProgress(int percent, string text)
			{
				m_PlayerService.UpdateProgressNotification(percent, text);
			}

			public void SetIndeterminate(string text)
			{
			}

			public NotificationProgressMonitor(PlayerService service)
			{
				m_PlayerService= service;
			}

			private PlayerService m_PlayerService;
		}

		private void ImportProject(String fileName, bool controllerRequest)
		{
			Messages.AddMessage(Ares.Players.MessageType.Debug, "Importing project " + fileName);
			String defaultProjectName = fileName;
			if (defaultProjectName.EndsWith(".apkg"))
			{
				defaultProjectName = defaultProjectName.Substring(0, defaultProjectName.Length - 5);
			}
			defaultProjectName = defaultProjectName + ".ares";
			String projectFileName = defaultProjectName;

			Ares.ModelInfo.Importer.Import(new NotificationProgressMonitor(this), fileName, projectFileName, true, null, (error, cancelled) =>
				{
					if (error != null)
					{
						ShowToast(String.Format(Resources.GetString(Resource.String.import_error), error.Message));
						if (controllerRequest)
						{
							m_Network.ErrorOccurred(-1, String.Format(Resources.GetString(Resource.String.load_error, error.Message)));
						}
					}
					else if (!cancelled)
					{
						OpenProject(projectFileName, controllerRequest);
					}
				});
		}

		private void DoModelChecks()
		{
			Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
			foreach (Ares.ModelInfo.ModelError error in Ares.ModelInfo.ModelChecks.Instance.GetAllErrors())
			{
				Messages.AddMessage(error.Severity == ModelInfo.ModelError.ErrorSeverity.Error ? Ares.Players.MessageType.Error : Ares.Players.MessageType.Warning,
					error.Message);
			}
		}

		private void MessageReceived(Ares.Players.Message m)
		{
			if ((int)m.Type >= Ares.Settings.Settings.Instance.MessageFilterLevel)
			{
				String s;
				switch (m.Type)
				{
				case Ares.Players.MessageType.Debug:
					s = Resources.GetString(Resource.String.Debug);
					break;
				case Ares.Players.MessageType.Info:
					s = Resources.GetString(Resource.String.Info);
					break;
				case Ares.Players.MessageType.Warning:
					s = Resources.GetString(Resource.String.Warning);
					break;
				case Ares.Players.MessageType.Error:
				default:
					s = Resources.GetString(Resource.String.Error);
					break;
				}
				s += m.Text;
				ShowToast(s);
			}
		}

		private String m_CurrentProjectPath = String.Empty;

		#region INetworkClient implementation

		public void KeyReceived(int key)
		{
			m_PlayingControl.KeyReceived(key);
		}

		public void ChangeMode(String title)
		{
			m_PlayingControl.SetMode(title);
		}

		public void VolumeReceived(Playing.VolumeTarget target, int value)
		{
			switch (target)
			{
			case Ares.Playing.VolumeTarget.Both:
				Settings.Settings.Instance.GlobalVolume = value;
				m_PlayingControl.GlobalVolume = value;
				break;
			case Ares.Playing.VolumeTarget.Music:
				Settings.Settings.Instance.MusicVolume = value;
				m_PlayingControl.MusicVolume = value;
				break;
			case Ares.Playing.VolumeTarget.Sounds:
				Settings.Settings.Instance.SoundVolume = value;
				m_PlayingControl.SoundVolume = value;
				break;
			default:
				break;
			}

		}

		public void ClientConnected()
		{
			m_Network.StopUdpBroadcast();
			ClientDataChanged(true);
		}


		public void ClientDataChanged(bool listenAgainAfterDisconnect)
		{
			if (m_Network.ClientConnected)
			{
				Messages.AddMessage(Ares.Players.MessageType.Info, Resources.GetString(Resource.String.connected_with) + m_Network.ClientName);
				m_Network.InformClientOfEverything(m_PlayingControl.GlobalVolume, m_PlayingControl.MusicVolume,
					m_PlayingControl.SoundVolume, m_PlayingControl.CurrentMode, MusicInfo.GetInfo(m_PlayingControl.CurrentMusicElement),
					m_PlayingControl.CurrentModeElements, m_Project,
					m_PlayingControl.CurrentMusicList, m_PlayingControl.MusicRepeat,
					m_TagLanguageId, new System.Collections.Generic.List<int>(m_PlayingControl.GetCurrentMusicTags()), m_PlayingControl.GetMusicTagCategoriesCombination(),
					Settings.Settings.Instance.TagMusicFadeTime, Settings.Settings.Instance.TagMusicFadeOnlyOnChange,
					Settings.Settings.Instance.PlayMusicOnAllSpeakers,
					Settings.Settings.Instance.ButtonMusicFadeMode, Settings.Settings.Instance.ButtonMusicFadeTime);
				UpdateNotification(false);
			}
			else
			{
				Messages.AddMessage(Ares.Players.MessageType.Info, Resources.GetString(Resource.String.not_connected));
				if (listenAgainAfterDisconnect)
				{
					m_Network.StartUdpBroadcast();
					UpdateNotification(false);
				}
			}
		}

		public string GetProjectsDirectory()
		{
			String oldPath = m_Project != null ? m_Project.FileName : Settings.Settings.Instance.ProjectDirectory;
			if (m_Project != null)
			{
				oldPath = System.IO.Directory.GetParent(oldPath).FullName;
			}
			return oldPath;
		}

		public Ares.Settings.RecentFiles GetLastUsedProjects()
		{
			return Ares.Settings.Settings.Instance.RecentFiles;
		}

		public void ProjectShallChange(string newProjectFile)
		{
			OpenProjectFromController(newProjectFile);
		}

		public Data.IProject GetCurrentProject()
		{
			return m_Project;
		}

		public void PlayOtherMusic(int elementId)
		{
			m_PlayingControl.SelectMusicElement(elementId);
		}

		public void SwitchElement(int elementId)
		{
			m_PlayingControl.SwitchElement(elementId);
		}

		public void SetMusicRepeat(bool repeat)
		{
			m_PlayingControl.SetRepeatCurrentMusic(repeat);
		}

		public void SwitchTag(int categoryId, int tagId, bool tagIsActive)
		{
			if (tagIsActive)
			{
				m_PlayingControl.AddMusicTag(categoryId, tagId);
			}
			else
			{
				m_PlayingControl.RemoveMusicTag(categoryId, tagId);
			}
		}

		public void DeactivateAllTags()
		{
			m_PlayingControl.RemoveAllMusicTags();
		}

		public void SetTagCategoryCombination(Data.TagCategoryCombination categoryCombination)
		{
			m_PlayingControl.SetMusicTagCategoriesCombination(categoryCombination);
		}

		public void SetMusicTagsFading(int fadeTime, bool onlyOnChange)
		{
			m_PlayingControl.SetMusicTagFading(fadeTime, onlyOnChange);
			Ares.Settings.Settings.Instance.TagMusicFadeTime = fadeTime;
			Ares.Settings.Settings.Instance.TagMusicFadeOnlyOnChange = onlyOnChange;
		}

		public void SetPlayMusicOnAllSpeakers(bool onAllSpeakers)
		{
			m_PlayingControl.SetPlayMusicOnAllSpeakers(onAllSpeakers);
			Ares.Settings.Settings.Instance.PlayMusicOnAllSpeakers = onAllSpeakers;
		}

		public void SetFadingOnPreviousNext(int fadeMode, int fadeTime)
		{
			m_PlayingControl.SetFadingOnPreviousNext(fadeMode != 0, fadeMode == 2, fadeTime);
			Ares.Settings.Settings.Instance.ButtonMusicFadeMode = fadeMode;
			Ares.Settings.Settings.Instance.ButtonMusicFadeTime = fadeTime;
		}

		#endregion
	}
}

