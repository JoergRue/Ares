using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Ares.Controllers;

using System.Windows.Forms;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;

//[assembly: MediaPortal.Common.Utils.CompatibleVersion("1.1.6.27644")]
[assembly: MediaPortal.Common.Utils.CompatibleVersion("1.4.0.0")]
[assembly: MediaPortal.Common.Utils.UsesSubsystem("MP.Config")]
[assembly: MediaPortal.Common.Utils.UsesSubsystem("MP.Players.Music")]
[assembly: MediaPortal.Common.Utils.UsesSubsystem("MP.SkinEngine")]


namespace Ares.MediaPortalPlugin
{
    public class AresPlugin : GUIWindow, ISetupForm, INetworkClient, IFromControllerNetworkClient, IUIThreadDispatcher
    {
        public AresPlugin()
        {
        }

        private Ares.Controllers.ServerSearch m_ServerSearch;
        private Controllers.ServerInfo m_Server;

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\Ares.xml");
        }

        #region Controls

        [SkinControl(1)]
        protected GUILabelControl aresTitle = null;

        [SkinControl(2)]
        protected GUIButtonControl stopButton = null;
        [SkinControl(3)]
        protected GUIButtonControl prevButton = null;
        [SkinControl(4)]
        protected GUIButtonControl nextButton = null;

        [SkinControl(5)]
        protected GUILabelControl overallVolumeLabel = null;
        [SkinControl(7)]
        protected GUILabelControl musicVolumeLabel = null;
        [SkinControl(9)]
        protected GUILabelControl soundsVolumeLabel = null;
        
        [SkinControl(6)]
        protected GUISliderControl overallVolumeBar = null;
        [SkinControl(8)]
        protected GUISliderControl musicVolumeBar = null;
        [SkinControl(10)]
        protected GUISliderControl soundsVolumeBar = null;

        [SkinControl(11)]
        protected GUILabelControl modeDescLabel = null;
        [SkinControl(13)]
        protected GUILabelControl elementsDescLabel = null;
        [SkinControl(15)]
        protected GUILabelControl musicDescLabel = null;
        [SkinControl(17)]
        protected GUILabelControl playerConnectionDescLabel = null;

        [SkinControl(12)]
        protected GUILabelControl modeLabel = null;
        [SkinControl(14)]
        protected GUILabelControl elementsLabel = null;
        [SkinControl(16)]
        protected GUIFadeLabel musicLabel = null;
        [SkinControl(18)]
        protected GUIFadeLabel playerConnectionLabel = null;

        [SkinControl(19)]
        protected GUILabelControl networkDescLabel = null;
        [SkinControl(20)]
        protected GUILabelControl networkLabel = null;
        [SkinControl(21)]
        protected GUIButtonControl disconnectButton = null;

#endregion

        #region Event Handlers

        protected override void OnPageLoad()
        {
            aresTitle.Label = StringResources.AresTitle;
            overallVolumeLabel.Label = StringResources.OverallVolume;
            musicVolumeLabel.Label = StringResources.MusicVolume;
            soundsVolumeLabel.Label = StringResources.SoundsVolume;
            modeDescLabel.Label = StringResources.Mode;
            elementsDescLabel.Label = StringResources.Elements;
            musicDescLabel.Label = StringResources.Music;
            playerConnectionDescLabel.Label = StringResources.PlayerConnection;
            networkDescLabel.Label = StringResources.NetworkState;

            disconnectButton.IsEnabled = false;
            overallVolumeBar.SetRange(0, 100);
            musicVolumeBar.SetRange(0, 100);
            soundsVolumeBar.SetRange(0, 100);
            Startup();
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            Shutdown();
            base.OnPageDestroy(new_windowId);
        }

        public override void Process()
        {
            base.Process();
            UpdateStatus();
            if (m_NeedsClientDataUpdate)
            {
                UpdateClientData();
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            base.OnClicked(controlId, control, actionType);
            if (control == stopButton)
            {
                StopClicked();
            }
            else if (control == prevButton)
            {
                PrevClicked();
            }
            else if (control == nextButton)
            {
                NextClicked();
            }
            else if (control == disconnectButton)
            {
                DisconnectClicked();
            }
            else if (control == overallVolumeBar)
            {
                OverallVolumeChanged();
            }
            else if (control == soundsVolumeBar)
            {
                SoundsVolumeChanged();
            }
            else if (control == musicVolumeBar)
            {
                MusicVolumeChanged();
            }
        }

        private void OverallVolumeChanged()
        {
            Controllers.Control.Instance.SetVolume(2, overallVolumeBar.IntValue);
            // m_Network.InformClientOfVolume(2, overallVolumeBar.IntValue);
        }

        private void SoundsVolumeChanged()
        {
            Controllers.Control.Instance.SetVolume(0, overallVolumeBar.IntValue);
            // m_Network.InformClientOfVolume(0, soundsVolumeBar.IntValue);
        }

        private void MusicVolumeChanged()
        {
            Controllers.Control.Instance.SetVolume(1, overallVolumeBar.IntValue);
            // m_Network.InformClientOfVolume(1, musicVolumeBar.IntValue);
        }

        private void StopClicked()
        {
            Controllers.Control.Instance.SendKey(Keys.Escape);
        }

        private void PrevClicked()
        {
            Controllers.Control.Instance.SendKey(Keys.Left);
        }

        private void NextClicked()
        {
            Controllers.Control.Instance.SendKey(Keys.Right);
        }

        private void DisconnectClicked()
        {
            m_Network.DisconnectClient(true); 
        }

        #endregion

        #region Main Control code

        private void ShowErrorDialog(string message, bool waitForOK)
        {
            if (waitForOK)
            {
                GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                if (dlgOK != null)
                {
                    dlgOK.SetHeading("Error" /* or Message */);
                    dlgOK.SetLine(1, message);
                    dlgOK.SetLine(2, "");
                    dlgOK.DoModal(PLUGIN_ID);
                }
            }
            else
            {
                GUIDialogNotify dlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                if (dlgNotify != null)
                {
                    dlgNotify.SetHeading("Error");
                    dlgNotify.SetText(message);
                    dlgNotify.DoModal(PLUGIN_ID);
                }
            }
        }
        
        private FromControllerNetwork m_Network;
        

        private Object syncObject = new Object();

        private String lastMusic = String.Empty;

        PlayingControl m_PlayingControl = new PlayingControl();

        private void UpdateStatus()
        {
            PlayingControl control = m_PlayingControl;
            String mode = control.CurrentMode;
            modeLabel.Label = mode != null ? mode : String.Empty;
            StringBuilder modeElementsText = new StringBuilder();
            foreach (String modeElement in control.CurrentModeElements)
            {
                if (modeElementsText.Length > 0)
                    modeElementsText.Append(", ");
                modeElementsText.Append(modeElement);
            }
            elementsLabel.Label = modeElementsText.ToString();
            String music = control.CurrentMusicElementLong;
            if (music != lastMusic)
            {
                musicLabel.Label = music;
                lastMusic = music;
            }
            if (overallVolumeBar.IntValue != control.GlobalVolume)
            {
                overallVolumeBar.IntValue = control.GlobalVolume;
                m_Network.InformClientOfVolume(2, control.GlobalVolume);
            }
            if (musicVolumeBar.IntValue != control.MusicVolume)
            {
                musicVolumeBar.IntValue = control.MusicVolume;
                m_Network.InformClientOfVolume(1, control.MusicVolume);
            }
            if (soundsVolumeBar.IntValue != control.SoundVolume)
            {
                soundsVolumeBar.IntValue = control.SoundVolume;
                m_Network.InformClientOfVolume(0, control.SoundVolume);
            }
            int oldTagFadeTime = Settings.Settings.Instance.TagMusicFadeTime;
            bool oldTagOnChange = Settings.Settings.Instance.TagMusicFadeOnlyOnChange;
            int newTagFadeTime; bool newTagOnChange;
            m_PlayingControl.GetMusicTagsFading(out newTagFadeTime, out newTagOnChange);
            bool change = false;
            if (oldTagFadeTime != newTagFadeTime)
            {
                Settings.Settings.Instance.TagMusicFadeTime = newTagFadeTime;
                change = true;
            }
            if (oldTagOnChange != newTagOnChange)
            {
                Settings.Settings.Instance.TagMusicFadeOnlyOnChange = newTagOnChange;
                change = true;
            }
            if (change)
            {
                m_Network.InformClientOfFading(newTagFadeTime, newTagOnChange);
            }
            bool oldOnAllSpeakers = Settings.Settings.Instance.PlayMusicOnAllSpeakers;
            if (m_PlayingControl.MusicOnAllSpeakers != oldOnAllSpeakers)
            {
                Settings.Settings.Instance.PlayMusicOnAllSpeakers = m_PlayingControl.MusicOnAllSpeakers;
            }
            int oldMusicFadingOption = Settings.Settings.Instance.ButtonMusicFadeMode;
            int oldMusicFadingTime = Settings.Settings.Instance.ButtonMusicFadeTime;
            change = false;
            if (oldMusicFadingOption != m_PlayingControl.MusicFadingOption)
            {
                Settings.Settings.Instance.ButtonMusicFadeMode = m_PlayingControl.MusicFadingOption;
                change = true;
            }
            if (oldMusicFadingTime != m_PlayingControl.MusicFadingTime)
            {
                Settings.Settings.Instance.ButtonMusicFadeTime = m_PlayingControl.MusicFadingTime;
                change = true;
            }
            if (change)
            {
                m_Network.InformClientOfMusicFading(m_PlayingControl.MusicFadingOption, m_PlayingControl.MusicFadingTime);
            }
        }

        private void UpdateClientData()
        {
            if (m_Network.ClientConnected)
            {
                networkLabel.Label = StringResources.ConnectedWith + m_Network.ClientName;
                m_Network.InformClientOfEverything(m_PlayingControl.GlobalVolume, m_PlayingControl.MusicVolume,
                    m_PlayingControl.SoundVolume, m_PlayingControl.CurrentMode, m_PlayingControl.CurrentMusicElementShort, m_PlayingControl.CurrentMusicElementLong,
                    m_PlayingControl.CurrentModeElementIds, m_PlayingControl.CurrentProject, m_PlayingControl.GetCurrentMusicList(),
                    m_PlayingControl.MusicRepeat, m_PlayingControl.TagCategories, m_PlayingControl.TagsPerCategory,
                    new List<int>(m_PlayingControl.GetCurrentMusicTags()), m_PlayingControl.GetMusicTagCategoriesCombination(), 
                    Settings.Settings.Instance.TagMusicFadeTime, Settings.Settings.Instance.TagMusicFadeOnlyOnChange,
                    Settings.Settings.Instance.PlayMusicOnAllSpeakers, Settings.Settings.Instance.ButtonMusicFadeMode, 
                    Settings.Settings.Instance.ButtonMusicFadeTime);
                disconnectButton.IsEnabled = true;
            }
            else
            {
                networkLabel.Label = StringResources.NotConnected;
                disconnectButton.IsEnabled = false;
            }
            m_NeedsClientDataUpdate = false;
        } 

        System.Timers.Timer broadCastTimer;

        void MessageAdded(object sender, Controllers.MessageEventArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.Debug:
                    Log.Debug(e.Message.Text);
                    break;
                case MessageType.Error:
                    Log.Error(e.Message.Text);
                    ShowErrorDialog(e.Message.Text, false);
                    break;
                case MessageType.Info:
                    Log.Info(e.Message.Text);
                    break;
                case MessageType.Warning:
                    Log.Warn(e.Message.Text);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Startup / Shutdown

        private void Startup()
        {
            Ares.Controllers.UIThreadDispatcher.Dispatcher = this;
            Ares.Controllers.Messages.Instance.MessageAdded += new EventHandler<Controllers.MessageEventArgs>(MessageAdded);
            AresSettings.ReadFromConfigFile();

            m_ServerSearch = new Controllers.ServerSearch(Settings.Settings.Instance.UdpPort + 1);
            m_ServerSearch.ServerFound += new EventHandler<Controllers.ServerEventArgs>(m_ServerSearch_ServerFound);
            m_ServerSearch.StartSearch();

            m_Network = new FromControllerNetwork(this);
            if (Settings.Settings.Instance.IPAddress.Length == 0)
            {
                System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(String.Empty);
                if (addresses.Length > 0)
                {
                    Settings.Settings.Instance.IPAddress = addresses[0].ToString();
                }
            }
            m_Network.InitConnectionData();
            m_Network.StartUdpBroadcast();

            broadCastTimer = new System.Timers.Timer(500);
            broadCastTimer.Elapsed += new System.Timers.ElapsedEventHandler(broadCastTimer_Tick);
            broadCastTimer.Enabled = true;
            m_Network.ListenForClient();

            MaybeStartPlayer();
        }

        void m_ServerSearch_ServerFound(object sender, Controllers.ServerEventArgs e)
        {
            if (e.Server.Name.EndsWith(FromControllerNetwork.MEDIA_PORTAL))
                // this is the mediaportal plugin itself
                return;
            MediaPortal.GUI.Library.Log.Info(String.Format(StringResources.ServerFound, e.Server.Name));
            m_Server = e.Server;
            ConnectOrDisconnectToPlayer();
        }

        private String FindLocalPlayer()
        {
            String path = Settings.Settings.Instance.LocalPlayerPath;
            if (String.IsNullOrEmpty(path))
            {
                // try to find via registry
                try
                {
                    path = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Ares", "PlayerPath", null);
                }
                catch (System.Security.SecurityException)
                {
                    path = null;
                }
                catch (System.IO.IOException)
                {
                    path = null;
                }
                if (String.IsNullOrEmpty(path))
                {
                    // try default location
                    path = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Ares\Player_Editor\Ares.Player.exe");
                    if (System.IO.File.Exists(path))
                    {
                        Settings.Settings.Instance.LocalPlayerPath = path;
                        return path;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Settings.Settings.Instance.LocalPlayerPath = path;
                }
            }
            return System.IO.File.Exists(path) ? path : null;
        }

        private void StartLocalPlayer()
        {
            String playerFile = FindLocalPlayer();
            if (playerFile == null)
                return;
            String arguments = "--minimized --udpPort=" + (Settings.Settings.Instance.UdpPort + 1) + " --tcpPort=" + (Settings.Settings.Instance.TcpPort + 1);
            try
            {
                System.Diagnostics.Process.Start(playerFile, arguments);
            }
            catch (Exception e)
            {
                String error = String.Format(StringResources.CouldNotStartPlayer, e.Message);
                Log.Error(error);
                ShowErrorDialog(error, true);
            }
        }

        private void MaybeStartPlayer()
        {
            if (Controllers.Control.Instance.IsConnected)
                return;
            String playerFile = FindLocalPlayer();
            if (playerFile == null)
            {
                Log.Error(StringResources.CouldNotFindPlayer);
                ShowErrorDialog(StringResources.CouldNotFindPlayer, true);
                return;
            }
            StartLocalPlayer();
        }

        private void ConnectOrDisconnectToPlayer()
        {
            if (Controllers.Control.Instance.IsConnected)
            {
                Controllers.Control.Instance.Disconnect(true);
            }
            else if (m_Server == null)
            {
                StartLocalPlayer();
            }
            else
            {
                if (m_Server != null)
                    Controllers.Control.Instance.Connect(m_Server, this, true);
            }
            UpdateNetworkState();
        }

        private void UpdateNetworkState()
        {
            if (Controllers.Control.Instance.IsConnected)
            {
                m_ServerSearch.StopSearch();
                playerConnectionLabel.Label = StringResources.PlayerConnectionEstablished;
            }
            else
            {
                m_Server = null;
                m_ServerSearch.StartSearch();
                playerConnectionLabel.Label = StringResources.PlayerConnectionDown;
            }
        }

        private void Shutdown()
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
            broadCastTimer.Enabled = false;
            m_Network.StopUdpBroadcast();

            Controllers.Control.Instance.Disconnect(true);
            Controllers.Messages.Instance.MessageAdded -= new EventHandler<Controllers.MessageEventArgs>(MessageAdded);
            m_ServerSearch.Dispose();

            AresSettings.SaveToConfigFile();
        }

        private bool warnOnNetworkFail = true;

        void broadCastTimer_Tick(object sender, EventArgs e)
        {
            if (!m_Network.SendUdpPacket())
            {
                if (warnOnNetworkFail)
                {
                    warnOnNetworkFail = false;
                    ShowErrorDialog(StringResources.NoStatusInfoError, true);
                }
            }
        }

        #endregion

        #region ISetupForm Members

        // Returns the name of the plugin which is shown in the plugin menu
        public string PluginName()
        {
            return "Ares";
        }

        // Returns the description of the plugin is shown in the plugin menu
        public string Description()
        {
            return "Ares Player Plugin";
        }

        // Returns the author of the plugin which is shown in the plugin menu
        public string Author()
        {
            return "Jörg Rüdenauer";
        }

        // show the setup dialog
        public void ShowPlugin()
        {
            SettingsDialog dialog = new SettingsDialog();
            dialog.ShowDialog();
        }

        // Indicates whether plugin can be enabled/disabled
        public bool CanEnable()
        {
            return true;
        }

        const int PLUGIN_ID = 7294;

        // Get Windows-ID
        public int GetWindowId()
        {
            // WindowID of windowplugin belonging to this setup
            // enter your own unique code
            return PLUGIN_ID;
        }

        // Indicates if plugin is enabled by default;
        public bool DefaultEnabled()
        {
            return true;
        }

        // indicates if a plugin has it's own setup screen
        public bool HasSetup()
        {
            return true;
        }

        /// <summary>
        /// If the plugin should have it's own button on the main menu of MediaPortal then it
        /// should return true to this method, otherwise if it should not be on home
        /// it should return false
        /// </summary>
        /// <param name="strButtonText">text the button should have</param>
        /// <param name="strButtonImage">image for the button, or empty for default</param>
        /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
        /// <param name="strPictureImage">subpicture for the button or empty for none</param>
        /// <returns>true : plugin needs it's own button on home
        /// false : plugin does not need it's own button on home</returns>

        public bool GetHome(out string strButtonText, out string strButtonImage,
          out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = "Ares";
            strButtonImage = "Ares";
            strButtonImageFocus = String.Empty;
            strPictureImage = String.Empty;
            return true;
        }

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID
        {
            get
            {
                return PLUGIN_ID;
            }

            set
            {
            }
        }

        #endregion

        #region Notifications from Controller

        public Ares.Controllers.Configuration FromControllerGetCurrentProject()
        {
            return m_PlayingControl.CurrentProject;
        }

        public String FromControllerGetProjectsDirectory()
        {
            String oldPath = !String.IsNullOrEmpty(Controllers.Control.Instance.FilePath) ? Controllers.Control.Instance.FilePath : System.Environment.CurrentDirectory;
            if (!String.IsNullOrEmpty(Controllers.Control.Instance.FilePath))
            {
                oldPath = System.IO.Directory.GetParent(oldPath).FullName;
            }
            return oldPath;
        }

        public void FromControllerPlayOtherMusic(int musicId)
        {
            Controllers.Control.Instance.SetMusicTitle(musicId);
        }

        public void FromControllerKeyReceived(byte[] key)
        {
            Ares.Controllers.Control.Instance.SendKey(key);
        }

        public void FromControllerSwitchElement(Int32 id)
        {
            Ares.Controllers.Control.Instance.SwitchElement(id);
        }

        public void FromControllerVolumeReceived(int target, int value)
        {
            Ares.Controllers.Control.Instance.SetVolume(target, value);
        }

        private bool m_NeedsClientDataUpdate = false;

        public void FromControllerClientDataChanged()
        {
            m_NeedsClientDataUpdate = true;
        }

        public void FromControllerProjectShallChange(string newProjectFile)
        {
            Ares.Controllers.Control.Instance.OpenFile(newProjectFile);
        }

        public void FromControllerSetMusicRepeat(bool repeat)
        {
            Ares.Controllers.Control.Instance.SetMusicRepeat(repeat);
        }

        public void FromControllerSetTagCategoryCombination(int categoryCombination)
        {
            Ares.Controllers.Control.Instance.SetTagCategoriesCombination(categoryCombination);
        }

        public void FromControllerSwitchTag(int categoryId, int tagId, bool isActive)
        {
            Ares.Controllers.Control.Instance.SwitchTag(categoryId, tagId, isActive);
        }

        public void FromControllerDeactivateAllTags()
        {
            Ares.Controllers.Control.Instance.RemoveAllTags();
        }

        public void FromControllerSetMusicTagsFading(int fadeTime, bool fadeOnlyOnChange)
        {
            Ares.Controllers.Control.Instance.SetTagFading(fadeTime, fadeOnlyOnChange);
        }

        public void FromControllerSetPlayMusicOnAllSpeakers(bool onAllSpeakers)
        {
            Ares.Controllers.Control.Instance.SetMusicOnAllSpeakers(onAllSpeakers);
        }

        public void FromControllerSetMusicFading(int fadingOption, int fadingTime)
        {
            Ares.Controllers.Control.Instance.SetMusicFading(fadingOption, fadingTime);
        }

        #endregion

        #region Dispatch to UI thread

        public void DispatchToUIThread(System.Action action)
        {
            MediaPortal.GUI.Library.GUIWindowManager.SendThreadCallback(new GUIWindowManager.Callback(ProcessAction), 0, 0, action);
        }

        private int ProcessAction(int x, int y, object action)
        {
            ((System.Action)action)();
            return 0;
        }

        #endregion

        #region Notifications from Player

        public void ModeChanged(string newMode)
        {
            m_PlayingControl.ModeChanged(newMode);
            DispatchToUIThread(() => m_Network.ModeChanged(newMode));
        }

        public void ModeElementStarted(int element)
        {
            m_PlayingControl.ModeElementStarted(element);
            DispatchToUIThread(() => m_Network.ModeElementStarted(element));
        }

        public void ModeElementStopped(int element)
        {
            m_PlayingControl.ModeElementFinished(element);
            DispatchToUIThread(() => m_Network.ModeElementFinished(element));
        }

        public void AllModeElementsStopped()
        {
            m_PlayingControl.AllModeElementsStopped();
        }

        public void VolumeChanged(int index, int volume)
        {
            m_PlayingControl.VolumeChanged(index, volume);
            DispatchToUIThread(() => m_Network.VolumeChanged(index, volume));
        }

        public void MusicChanged(string newMusic, string shortTitle)
        {
            m_PlayingControl.MusicStarted(shortTitle, newMusic);
            DispatchToUIThread(() => m_Network.MusicChanged(shortTitle, newMusic));
        }

        public void MusicListChanged(List<MusicListItem> newList)
        {
            m_PlayingControl.MusicListChanged(newList);
            DispatchToUIThread(() => m_Network.MusicPlaylistChanged(newList));
        }

        public void MusicRepeatChanged(bool repeat)
        {
            m_PlayingControl.MusicRepeatChanged(repeat);
            DispatchToUIThread(() => m_Network.MusicRepeatChanged(repeat));
        }

        public void MusicOnAllSpeakersChanged(bool onAllSpeakers)
        {
            m_PlayingControl.MusicOnAllSpeakersChanged(onAllSpeakers);
            DispatchToUIThread(() => m_Network.MusicOnAllSpeakersChanged(onAllSpeakers));
        }

        public void MusicFadingChanged(int fadingOption, int fadingTime)
        {
            m_PlayingControl.MusicFadingChanged(fadingOption, fadingTime);
            DispatchToUIThread(() => m_Network.MusicFadingChanged(fadingOption, fadingTime));
        }

        public void TagsChanged(List<MusicTagCategory> categories, Dictionary<int, List<MusicTag>> tagsPerCategory)
        {
            m_PlayingControl.TagsChanged(categories, tagsPerCategory);
            DispatchToUIThread(() => m_Network.InformClientOfPossibleTags(categories, tagsPerCategory));
        }

        public void ActiveTagsChanged(List<int> activeTags)
        {
            m_PlayingControl.ActiveTagsChanged(activeTags);
            DispatchToUIThread(() => m_Network.InformClientOfActiveTags(activeTags));
        }

        public void TagStateChanged(int tagId, bool isActive)
        {
            if (isActive)
            {
                m_PlayingControl.MusicTagAdded(tagId);
                DispatchToUIThread(() => m_Network.MusicTagAdded(tagId));
            }
            else
            {
                m_PlayingControl.MusicTagRemoved(tagId);
                DispatchToUIThread(() => m_Network.MusicTagRemoved(tagId));
            }
        }

        public void TagCategoryCombinationChanged(int categoryCombination)
        {
            m_PlayingControl.MusicTagCategoriesCombinationChanged(categoryCombination);
            DispatchToUIThread(() => m_Network.MusicTagCategoriesCombinationChanged(categoryCombination));
        }

        public void TagFadingChanged(int fadeTime, bool onlyOnChange)
        {
            m_PlayingControl.MusicTagsFadingChanged(fadeTime, onlyOnChange);
            DispatchToUIThread(() => m_Network.MusicTagsFadingChanged(fadeTime, onlyOnChange));
        }

        public void ProjectFilesRetrieved(List<string> files)
        {
            // not needed here; project file retrieval request is short-circuited since 
            // media-portal plugin and player are on the same computer
        }

        public void ConfigurationChanged(Configuration newConfiguration, string fileName)
        {
            m_PlayingControl.CurrentProject = newConfiguration;
            DispatchToUIThread(() => m_Network.InformClientOfProject(newConfiguration));
        }

        public void Disconnect()
        {
            DispatchToUIThread(() =>
            {
                Controllers.Control.Instance.Disconnect(false);
                UpdateNetworkState();
            }
            );
        }

        public void ConnectionFailed()
        {
            DispatchToUIThread(() =>
            {
                Controllers.Control.Instance.Disconnect(false);
                UpdateNetworkState();
            }
            );
        }

        #endregion

    }
}
