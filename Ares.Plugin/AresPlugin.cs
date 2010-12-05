using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Ares.Players;
using Ares.Playing;

using System.Windows.Forms;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;


namespace Ares.Plugin
{
    public class AresPlugin : GUIWindow, ISetupForm, INetworkClient
    {
        public AresPlugin()
        {
        }

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
        protected GUILabelControl soundsDescLabel = null;

        [SkinControl(12)]
        protected GUILabelControl modeLabel = null;
        [SkinControl(14)]
        protected GUILabelControl elementsLabel = null;
        [SkinControl(16)]
        protected GUIFadeLabel musicLabel = null;
        [SkinControl(18)]
        protected GUIFadeLabel soundsLabel = null;

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
            soundsDescLabel.Label = StringResources.Sounds;
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

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            // m_PlayingControl.KeyReceived(e.Key);
            base.OnKeyDown(e);
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
            m_PlayingControl.GlobalVolume = overallVolumeBar.IntValue;
            m_Network.InformClientOfVolume(VolumeTarget.Both, overallVolumeBar.IntValue);
        }

        private void SoundsVolumeChanged()
        {
            m_PlayingControl.SoundVolume = soundsVolumeBar.IntValue;
            m_Network.InformClientOfVolume(VolumeTarget.Sounds, soundsVolumeBar.IntValue);
        }

        private void MusicVolumeChanged()
        {
            m_PlayingControl.MusicVolume = musicVolumeBar.IntValue;
            m_Network.InformClientOfVolume(VolumeTarget.Music, musicVolumeBar.IntValue);
        }

        private void StopClicked()
        {
            m_PlayingControl.KeyReceived(Keys.Escape);
        }

        private void PrevClicked()
        {
            m_PlayingControl.KeyReceived(Keys.Left); 
        }

        private void NextClicked()
        {
            m_PlayingControl.KeyReceived(Keys.Right);
        }

        private void DisconnectClicked()
        {
            m_Network.DisconnectClient(true); 
        }

        #endregion

        #region Main Control code

        private void ShowErrorDialog(string message)
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
        
        private Ares.Data.IProject m_Project;
        private Network m_Network;
        private PlayingControl m_PlayingControl;

        private void OpenProject(String filePath)
        {
            if (m_Project != null)
            {
                Ares.Data.DataModule.ProjectManager.UnloadProject(m_Project);
                m_Project = null;
            }
            try
            {
                m_Project = Ares.Data.DataModule.ProjectManager.LoadProject(filePath);
            }
            catch (Exception e)
            {
                ShowErrorDialog(String.Format(StringResources.LoadError, e.Message));
                m_Project = null;
            }
            PlayingModule.ProjectPlayer.SetProject(m_Project);
            DoModelChecks();
        }

        private void DoModelChecks()
        {
            Ares.ModelInfo.ModelChecks.Instance.Project = m_Project;
            Ares.ModelInfo.ModelChecks.Instance.CheckAll();
            String modelErrors = String.Empty;
            foreach (Ares.ModelInfo.ModelError error in Ares.ModelInfo.ModelChecks.Instance.GetAllErrors())
            {
                Messages.AddMessage(error.Severity == ModelInfo.ModelError.ErrorSeverity.Error ? MessageType.Error : MessageType.Warning,
                    error.Message);
                if (error.Severity == ModelInfo.ModelError.ErrorSeverity.Error)
                {
                    if (modelErrors.Length > 0)
                        modelErrors += "\n";
                    modelErrors += error.Message;
                }
            }
            if (modelErrors.Length > 0)
            {
                ShowErrorDialog(modelErrors);
            }
        }

        private int lastMusicElementId = -1;

        private void UpdateStatus()
        {
            PlayingControl control = m_PlayingControl;
            Ares.Data.IMode mode = control.CurrentMode;
            modeLabel.Label = mode != null ? mode.Title : String.Empty;
            StringBuilder modeElementsText = new StringBuilder();
            foreach (Ares.Data.IModeElement modeElement in control.CurrentModeElements)
            {
                if (modeElementsText.Length > 0)
                    modeElementsText.Append(", ");
                modeElementsText.Append(modeElement.Title);
            }
            elementsLabel.Label = modeElementsText.ToString();
            StringBuilder soundElementsText = new StringBuilder();
            foreach (int element in control.CurrentSoundElements)
            {
                if (soundElementsText.Length > 0)
                    soundElementsText.Append(", ");
                soundElementsText.Append(Ares.Data.DataModule.ElementRepository.GetElement(element).Title);
            }
            soundsLabel.Label = soundElementsText.ToString();
            int musicElementId = control.CurrentMusicElement;
            if (musicElementId != lastMusicElementId)
            {
                musicLabel.Label = MusicInfo.GetInfo(musicElementId);
                lastMusicElementId = musicElementId;
            }
            if (overallVolumeBar.IntValue != control.GlobalVolume)
            {
                overallVolumeBar.IntValue = control.GlobalVolume;
                m_Network.InformClientOfVolume(VolumeTarget.Both, control.GlobalVolume);
            }
            if (musicVolumeBar.IntValue != control.MusicVolume)
            {
                musicVolumeBar.IntValue = control.MusicVolume;
                m_Network.InformClientOfVolume(VolumeTarget.Music, control.MusicVolume);
            }
            if (soundsVolumeBar.IntValue != control.SoundVolume)
            {
                soundsVolumeBar.IntValue = control.SoundVolume;
                m_Network.InformClientOfVolume(VolumeTarget.Sounds, control.SoundVolume);
            }
        }

        private void UpdateClientData()
        {
            if (m_Network.ClientConnected)
            {
                networkLabel.Label = StringResources.ConnectedWith + m_Network.ClientName;
                m_Network.InformClientOfVolume(VolumeTarget.Both, m_PlayingControl.GlobalVolume);
                m_Network.InformClientOfVolume(VolumeTarget.Music, m_PlayingControl.MusicVolume);
                m_Network.InformClientOfVolume(VolumeTarget.Sounds, m_PlayingControl.SoundVolume);
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

        private void MessageReceived(Ares.Players.Message m)
        {
            switch (m.Type)
            {
                case MessageType.Debug:
                    Log.Debug(m.Text);
                    break;
                case MessageType.Error:
                    Log.Error(m.Text);
                    ShowErrorDialog(m.Text);
                    break;
                case MessageType.Info:
                    Log.Info(m.Text);
                    break;
                case MessageType.Warning:
                    Log.Warn(m.Text);
                    break;
                default:
                    break;
            }
        }

        private bool hasInitedBass = false;

        private void Startup()
        {
            Messages.Instance.MessageReceived += new MessageReceivedHandler(MessageReceived);
            AresSettings.ReadFromConfigFile();
            
            hasInitedBass = Un4seen.Bass.Bass.BASS_Init(-1, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            Un4seen.Bass.AddOn.Fx.BassFx.LoadMe();
            
            m_PlayingControl = new PlayingControl();
            m_PlayingControl.UpdateDirectories();

            overallVolumeBar.IntValue = Settings.Settings.Instance.GlobalVolume;
            musicVolumeBar.IntValue = Settings.Settings.Instance.MusicVolume;
            soundsVolumeBar.IntValue = Settings.Settings.Instance.SoundVolume;
            m_PlayingControl.GlobalVolume = Settings.Settings.Instance.GlobalVolume;
            m_PlayingControl.MusicVolume = Settings.Settings.Instance.MusicVolume;
            m_PlayingControl.SoundVolume = Settings.Settings.Instance.SoundVolume;

            m_Network = new Network(this);
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
            if (Settings.Settings.Instance.RecentFiles.GetFiles().Count > 0)
            {
                OpenProject(Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath);
            }
            m_Network.ListenForClient();
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

            Settings.Settings settings = Settings.Settings.Instance;
            settings.GlobalVolume = m_PlayingControl.GlobalVolume;
            settings.MusicVolume = m_PlayingControl.MusicVolume;
            settings.SoundVolume = m_PlayingControl.SoundVolume;
            AresSettings.SaveToConfigFile();

            m_PlayingControl.Dispose();
            if (hasInitedBass)
            {
                Un4seen.Bass.Bass.BASS_Free();
            }
        }

        private bool warnOnNetworkFail = true;

        void broadCastTimer_Tick(object sender, EventArgs e)
        {
            if (!m_Network.SendUdpPacket())
            {
                if (warnOnNetworkFail)
                {
                    warnOnNetworkFail = false;
                    ShowErrorDialog(StringResources.NoStatusInfoError);
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



        public void KeyReceived(Keys key)
        {
            m_PlayingControl.KeyReceived(key);
        }

        public void VolumeReceived(VolumeTarget target, int value)
        {
            switch (target)
            {
                case VolumeTarget.Both:
                    m_PlayingControl.GlobalVolume = value;
                    break;
                case VolumeTarget.Music:
                    m_PlayingControl.MusicVolume = value;
                    break;
                case VolumeTarget.Sounds:
                    m_PlayingControl.SoundVolume = value;
                    break;
                default:
                    break;
            }
        }

        private bool m_NeedsClientDataUpdate = false;

        public void ClientDataChanged()
        {
            m_NeedsClientDataUpdate = true;
        }
    }
}
