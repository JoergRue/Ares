/*
 Copyright (c) 2014 [Joerg Ruedenauer]
 
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
using System.Linq;
using System.Text;
using Ares.Players;
using Ares.Data;
using Ares.Settings;
using Ares.ModelInfo;

namespace Ares.CmdLinePlayer
{
    class PlayerOptions
    {
        public String Language { get; set; }
        public int UdpPort { get; set; }
        public int LegacyTcpPort { get; set; }
        public int WebTcpPort { get; set; }
        public String InitialProject { get; set; }
        public int MessageFilterLevel { get; set; }
        public bool NonInteractive { get; set; }
        public bool Daemon { get; set; }
        public int OutputDevice { get; set; }

        public PlayerOptions()
        {
            UdpPort = -1;
            LegacyTcpPort = -2;
            WebTcpPort = -1;
            InitialProject = String.Empty;
            Language = String.Empty;
            NonInteractive = false;
            Daemon = false;
            ShowHelp = false;
            MessageFilterLevel = -1;
            OutputDevice = 0;
        }

        private bool ShowHelp { get; set; }

        public bool Parse(String[] args)
        {
            var p = new NDesk.Options.OptionSet()
            {
                { "h|?|help", StringResources.CmdLineOptionHelp, var => ShowHelp = var != null },
                { "m|MessageLevel=", StringResources.CmdLineOptionMsgLevel, (int var) => MessageFilterLevel = var },
                { "UdpPort=", StringResources.CmdLineOptionUdpPort, (int var) => UdpPort = var },
                { "TcpPort=", StringResources.CmdLineOptionTcpPort, (int var) => LegacyTcpPort = var },
                { "WebTcpPort=", StringResources.CmdLineOptionWebTcpPort, (int var) => WebTcpPort = var },
                { "OutputDevice=", StringResources.CmdLineOutputDevice, (int var) => OutputDevice = var },
                { "Language=", StringResources.CmdLineOptionLanguage, var => Language = var },
                { "NonInteractive", StringResources.CmdLineOptionNonInteractive, var => NonInteractive = var != null },
                { "Daemon", StringResources.CmdLineOptionDaemon, var => { Daemon = var != null; if (Daemon) NonInteractive = true; } }
            };
            List<string> extra = null;
            try
            {
                extra = p.Parse(args);
            }
            catch (NDesk.Options.OptionException /*e*/)
            {
                ShowHelp = true;
            }
            if (UdpPort != -1 && UdpPort < 1)
            {
                Console.Error.WriteLine(StringResources.InvalidUdpPort);
                ShowHelp = true;
            }
            if (LegacyTcpPort != -1 && LegacyTcpPort < 1 && LegacyTcpPort != -2)
            {
                Console.Error.WriteLine(StringResources.InvalidTcpPort);
                ShowHelp = true;
            }
            if (WebTcpPort != -1 && WebTcpPort < 1)
            {
                Console.Error.WriteLine(StringResources.InvalidTcpPort);
                ShowHelp = true;
            }
            if (OutputDevice < -1 || OutputDevice > 10)
            {
                Console.Error.WriteLine(StringResources.InvalidOutputDevice);
                ShowHelp = true;
            }
            if (MessageFilterLevel != -1 && (MessageFilterLevel < 0 || MessageFilterLevel > 3))
            {
                Console.Error.WriteLine(StringResources.InvalidMsgLevel);
                ShowHelp = true;
            }
            if (extra != null && extra.Count > 1)
            {
                Console.Error.WriteLine(StringResources.MoreThanOneProject);
                ShowHelp = true;
            }
            if (ShowHelp)
            {
                Console.WriteLine();
                Console.WriteLine("Usage: Ares.CmdLinePlayer [Options] [ProjectFile]");
                Console.WriteLine("Options:");
                p.WriteOptionDescriptions(Console.Out);
                return false;
            }
            return true;
        }
    }

    class Player : INetworkClient
    {
        public Player(Ares.Playing.BassInit init)
        {
            m_PlayingControl = new PlayingControl();
            m_BassInit = init;
        }

        private INetworks m_Network;
        private PlayingControl m_PlayingControl;
        private Ares.Playing.BassInit m_BassInit;

        private BasicSettings m_BasicSettings;

        private IProject m_Project;
        private int m_TagLanguageId;

        private System.Timers.Timer m_BroadcastTimer;
        private bool warnOnNetworkFail = true;

        private System.Threading.AutoResetEvent m_NonInteractiveWaitEvent = null;
        private bool m_Shutdown = false;
        private bool m_IsDaemon = false;
        private Object m_LockObject = new Object();

        public int Run(PlayerOptions options)
        {
            int res = Initialize(options);
            if (res != 0)
                return res;

            if (!options.NonInteractive)
            {
                Console.WriteLine(StringResources.PressKeyToExit);
                Console.ReadKey();
            }
            else
            {
                m_IsDaemon = options.Daemon;
                m_NonInteractiveWaitEvent = new System.Threading.AutoResetEvent(false);
                bool shutdown = false;
                while (!shutdown)
                {
                    m_NonInteractiveWaitEvent.WaitOne(200);
                    lock (m_LockObject)
                    {
                        shutdown = m_Shutdown;
                    }
                }
            }

            Shutdown();
            return 0;
        }

        private int Initialize(PlayerOptions options)
        {
            m_BasicSettings = new BasicSettings();
            ReadSettings();
            if (options.MessageFilterLevel != -1)
            {
                Settings.Settings.Instance.MessageFilterLevel = options.MessageFilterLevel;
            }
            Messages.Instance.MessageReceived += new MessageReceivedHandler(MessageReceived);
            if (options.UdpPort != -1)
            {
                Settings.Settings.Instance.UdpPort = options.UdpPort;
            }
            if (options.LegacyTcpPort > 0)
            {
                Settings.Settings.Instance.TcpPort = options.LegacyTcpPort;
                Settings.Settings.Instance.UseLegacyNetwork = true;
            }
            else
            {
                Settings.Settings.Instance.UseLegacyNetwork = (options.LegacyTcpPort != -1);
            }
            if (options.WebTcpPort != -1)
            {
                Settings.Settings.Instance.WebTcpPort = options.WebTcpPort;
                Settings.Settings.Instance.UseWebNetwork = true;
            }
            else
            {
                Settings.Settings.Instance.UseWebNetwork = false;
            }
            if (options.OutputDevice == 0)
            {
                int outputDevice = Settings.Settings.Instance.OutputDeviceIndex;
                if (outputDevice != -1)
                {
                    try
                    {
                        m_BassInit.SwitchDevice(outputDevice);
                    }
                    catch (Ares.Playing.BassInitException)
                    {
                        Console.WriteLine(StringResources.DeviceInitError);
                    }
                }
            }
            else
            {
                Settings.Settings.Instance.OutputDeviceIndex = options.OutputDevice;
            }
            if (!String.IsNullOrEmpty(options.InitialProject))
            {
                if (options.InitialProject.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
                    ImportProject(options.InitialProject, false);
                else
                    OpenProject(options.InitialProject, false);
            }
            else if (Ares.Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
            {
                OpenProject(Ares.Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath, false);
            }
            else
            {
                Console.WriteLine(StringResources.NoOpenedProject);
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
                Console.WriteLine(StringResources.NoIpAddress);
                return 2;
            }
            m_Network = new Networks(this, Settings.Settings.Instance.UseLegacyNetwork, Settings.Settings.Instance.UseWebNetwork);
            m_Network.InitConnectionData();
            m_Network.StartUdpBroadcast();
            m_BroadcastTimer = new System.Timers.Timer(50);
            m_BroadcastTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_BroadcastTimer_Elapsed);
            m_BroadcastTimer.Enabled = true;
            m_Network.ListenForClient();
            return 0;
        }

        private void m_BroadcastTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!m_Network.SendUdpPacket())
            {
                if (warnOnNetworkFail)
                {
                    warnOnNetworkFail = false;
                    Console.WriteLine(StringResources.NoStatusInfoError);
                }
            }
        }

        private void Shutdown()
        {
            StopAllPlaying();
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
            m_PlayingControl.Dispose();
            WriteSettings();
            Settings.Settings.Instance.Shutdown();
        }


        private void WriteSettings()
        {
            try
            {
                Ares.Settings.Settings.Instance.WriteToFile(m_BasicSettings.GetSettingsDir());
                m_BasicSettings.WriteToFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format(StringResources.WriteSettingsError, ex.Message));
            }

        }

        private void ReadSettings()
        {
            bool foundSettings = m_BasicSettings.ReadFromFile();
            bool hasSettings = Ares.Settings.Settings.Instance.Initialize(Ares.Settings.Settings.PlayerID, foundSettings ? m_BasicSettings.GetSettingsDir() : null);
            if (!hasSettings)
            {
                Console.WriteLine(String.Format(StringResources.NoSettings, System.IO.Path.Combine(m_BasicSettings.GetSettingsDir(), Ares.Settings.Settings.Instance.settingsFileName)));
                SettingsChanged(true);
            }
            else
            {
                SettingsChanged(true);
            }
        }

        private void SettingsChanged(bool fundamentalChange)
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            if (fundamentalChange)
            {
                m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Escape);
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
                Console.WriteLine(String.Format(StringResources.TagsDbError, ex.Message));
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
                String oldPath = m_Project != null ? m_Project.FileName : System.Environment.CurrentDirectory;
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
                if (!onControllerRequest)
                {
                    Console.WriteLine(String.Format(StringResources.LoadError, e.Message));
                }
                else
                {
                    m_Network.ErrorOccurred(-1, String.Format(StringResources.LoadError, e.Message));
                }
                m_Project = null;
                m_CurrentProjectPath = String.Empty;
            }
            Ares.Playing.PlayingModule.ProjectPlayer.SetProject(m_Project);
            DoModelChecks();
            if (m_Network != null)
            {
                m_Network.InformClientOfProject(m_Project);
                if (m_Project != null)
                {
                    m_Network.InformClientOfPossibleTags(m_TagLanguageId, m_Project);
                }
            }
        }

        class NoProgressMonitor : IProgressMonitor
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
            }

            public void SetIndeterminate(string text)
            {
            }
        }

        private void ImportProject(String fileName, bool controllerRequest)
        {
            String defaultProjectName = fileName;
            if (defaultProjectName.EndsWith(".apkg"))
            {
                defaultProjectName = defaultProjectName.Substring(0, defaultProjectName.Length - 5);
            }
            defaultProjectName = defaultProjectName + ".ares";
            String projectFileName = defaultProjectName;

            Ares.ModelInfo.Importer.Import(new NoProgressMonitor(), fileName, projectFileName, true, null, (error, cancelled) =>
            {
                if (error != null)
                {
                    if (controllerRequest)
                    {
                        m_Network.ErrorOccurred(-1, String.Format(StringResources.LoadError, error.Message));
                    }
                    else
                    {
                        Console.WriteLine(String.Format(StringResources.ImportError, error.Message));
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
                Messages.AddMessage(error.Severity == ModelInfo.ModelError.ErrorSeverity.Error ? MessageType.Error : MessageType.Warning,
                    error.Message);
            }
        }

        private String m_CurrentProjectPath = String.Empty;

        private void MessageReceived(Ares.Players.Message m)
        {
            if ((int)m.Type >= Ares.Settings.Settings.Instance.MessageFilterLevel)
            {
                String s;
                switch (m.Type)
                {
                    case MessageType.Debug:
                        s = StringResources.Debug;
                        break;
                    case MessageType.Info:
                        s = StringResources.Info;
                        break;
                    case MessageType.Warning:
                        s = StringResources.Warning;
                        break;
                    case MessageType.Error:
                    default:
                        s = StringResources.Error;
                        break;
                }
                s += m.Text;
                Console.WriteLine(s);
            }
        }


        #region INetworkClient

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
                Console.WriteLine(StringResources.ConnectedWith + m_Network.ClientName);
                m_Network.InformClientOfEverything(m_PlayingControl.GlobalVolume, m_PlayingControl.MusicVolume,
                    m_PlayingControl.SoundVolume, m_PlayingControl.CurrentMode, MusicInfo.GetInfo(m_PlayingControl.CurrentMusicElement),
                    m_PlayingControl.CurrentModeElements, m_Project,
                    m_PlayingControl.CurrentMusicList, m_PlayingControl.MusicRepeat,
                    m_TagLanguageId, new List<int>(m_PlayingControl.GetCurrentMusicTags()), m_PlayingControl.GetMusicTagCategoriesCombination(),
                    Settings.Settings.Instance.TagMusicFadeTime, Settings.Settings.Instance.TagMusicFadeOnlyOnChange,
                    Settings.Settings.Instance.PlayMusicOnAllSpeakers,
                    Settings.Settings.Instance.ButtonMusicFadeMode, Settings.Settings.Instance.ButtonMusicFadeTime);
            }
            else
            {
                Console.WriteLine(StringResources.NotConnected);
                if (m_NonInteractiveWaitEvent != null)
                {
                    if (!m_IsDaemon)
                    {
                        lock (m_LockObject)
                        {
                            m_Shutdown = true;
                        }
                        m_NonInteractiveWaitEvent.Set();
                    }
                    else if (listenAgainAfterDisconnect)
                    {
                        m_Network.StartUdpBroadcast();
                    }
                }
            }
        }

        public string GetProjectsDirectory()
        {
            String oldPath = m_Project != null ? m_Project.FileName : System.Environment.CurrentDirectory;
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
