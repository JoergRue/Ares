﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Ares.Players;
using Ares.Playing;

using System.Windows.Forms;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;

[assembly: MediaPortal.Common.Utils.CompatibleVersion("1.1.6.27644")]
[assembly: MediaPortal.Common.Utils.UsesSubsystem("MP.Config")]
[assembly: MediaPortal.Common.Utils.UsesSubsystem("MP.Players.Music")]
[assembly: MediaPortal.Common.Utils.UsesSubsystem("MP.SkinEngine")]


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
            String projectTest = String.Empty;
            lock (syncObject)
            {
                projectTest = controllerFileName;
                controllerFileName = String.Empty;
            }
            if (!String.IsNullOrEmpty(projectTest))
            {
                OpenProjectFromController(projectTest);
            }
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
            m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Escape);
        }

        private void PrevClicked()
        {
            m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Left); 
        }

        private void NextClicked()
        {
            m_PlayingControl.KeyReceived((int)Ares.Data.Keys.Right);
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
        
        private Ares.Data.IProject m_Project;
        private Network m_Network;
        private PlayingControl m_PlayingControl;

        private String controllerFileName = String.Empty;

        private Object syncObject = new Object();

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
            if (System.IO.File.Exists(path))
            {
                if (path.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
                {
                    ImportProject(path);
                }
                else
                {
                    OpenProject(path, true);
                }
            }
        }

        private class DummyProgressMonitor : Ares.ModelInfo.IProgressMonitor
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

        private void ImportProject(String fileName)
        {
            String defaultProjectName = fileName;
            if (defaultProjectName.EndsWith(".apkg"))
            {
                defaultProjectName = defaultProjectName.Substring(0, defaultProjectName.Length - 5);
            }
            defaultProjectName = defaultProjectName + ".ares";
            String projectFileName = defaultProjectName;

            
            Ares.ModelInfo.Importer.Import(new DummyProgressMonitor(), fileName, projectFileName, true, (error, cancelled) =>
            {
                if (error != null)
                {
                    m_Network.ErrorOccurred(-1, String.Format(StringResources.LoadError, error.Message));
                }
                else if (!cancelled)
                {
                    OpenProject(projectFileName, true);
                }
            });
        }

        public void PlayOtherMusic(int musicId)
        {
            m_PlayingControl.SelectMusicElement(musicId);
        }

        public Ares.Data.IProject GetCurrentProject()
        {
            return m_Project;
        }

        public String GetProjectsDirectory()
        {
            String oldPath = m_Project != null ? m_Project.FileName : System.Environment.CurrentDirectory;
            if (m_Project != null)
            {
                oldPath = System.IO.Directory.GetParent(oldPath).FullName;
            }
            return oldPath;
        }

        private void OpenProject(String filePath, bool fromController)
        {
            if (m_Project != null)
            {
                if (fromController && m_Project.FileName.Equals(filePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (m_Network != null)
                    {
                        m_Network.InformClientOfProject(m_Project);
                    }
                    return;
                }
                Ares.Data.DataModule.ProjectManager.UnloadProject(m_Project);
                m_Project = null;
            }
            try
            {
                m_Project = Ares.Data.DataModule.ProjectManager.LoadProject(filePath);
                if (m_Project.TagLanguageId != -1)
                {
                    m_TagLanguageId = m_Project.TagLanguageId;
                }
                else
                    m_TagLanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
            }
            catch (Exception e)
            {
                if (fromController)
                {
                    m_Network.ErrorOccurred(-1, String.Format(StringResources.LoadError, e.Message));
                }
                else
                {
                    ShowErrorDialog(String.Format(StringResources.LoadError, e.Message), true);
                }
                m_Project = null;
            }
            PlayingModule.ProjectPlayer.SetProject(m_Project);
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

        private void DoModelChecks()
        {
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
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
                ShowErrorDialog(modelErrors, false);
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
            String sounds = soundElementsText.ToString();
            if (sounds.Length == 0)
                sounds = "  ";
            soundsLabel.Label = sounds;
            int musicElementId = control.CurrentMusicElement;
            if (musicElementId != lastMusicElementId)
            {
                String test = MusicInfo.GetInfo(musicElementId).LongTitle;
                if (test.Length == 0)
                    test = "  ";
                musicLabel.Label = test;
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
        }

        private int m_TagLanguageId = -1;

        private void UpdateClientData()
        {
            if (m_Network.ClientConnected)
            {
                networkLabel.Label = StringResources.ConnectedWith + m_Network.ClientName;
                m_Network.InformClientOfEverything(m_PlayingControl.GlobalVolume, m_PlayingControl.MusicVolume,
                    m_PlayingControl.SoundVolume, m_PlayingControl.CurrentMode, MusicInfo.GetInfo(m_PlayingControl.CurrentMusicElement),
                    m_PlayingControl.CurrentModeElements, m_Project,
                    m_PlayingControl.CurrentMusicList, m_PlayingControl.MusicRepeat, m_TagLanguageId, 
                    new List<int>(m_PlayingControl.GetCurrentMusicTags()), m_PlayingControl.IsMusicTagCategoriesOperatorAnd(), 
                    Settings.Settings.Instance.TagMusicFadeTime, Settings.Settings.Instance.TagMusicFadeOnlyOnChange);
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
                    ShowErrorDialog(m.Text, false);
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

            try
            {
                var tagsDBFiles = Ares.Tags.TagsModule.GetTagsDB().FilesInterface;
                String path = System.IO.Path.Combine(Ares.Settings.Settings.Instance.MusicDirectory, tagsDBFiles.DefaultFileName);
                tagsDBFiles.OpenOrCreateDatabase(path);
                m_TagLanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                ShowErrorDialog(ex.Message, false);
            }
            
            m_PlayingControl = new PlayingControl();
            m_PlayingControl.UpdateDirectories();

            overallVolumeBar.IntValue = Settings.Settings.Instance.GlobalVolume;
            musicVolumeBar.IntValue = Settings.Settings.Instance.MusicVolume;
            soundsVolumeBar.IntValue = Settings.Settings.Instance.SoundVolume;
            m_PlayingControl.GlobalVolume = Settings.Settings.Instance.GlobalVolume;
            m_PlayingControl.MusicVolume = Settings.Settings.Instance.MusicVolume;
            m_PlayingControl.SoundVolume = Settings.Settings.Instance.SoundVolume;

            m_PlayingControl.SetMusicTagFading(Settings.Settings.Instance.TagMusicFadeTime, Settings.Settings.Instance.TagMusicFadeOnlyOnChange);

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
                OpenProject(Settings.Settings.Instance.RecentFiles.GetFiles()[0].FilePath, false);
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

            Ares.Tags.TagsModule.GetTagsDB().FilesInterface.CloseDatabase();

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



        public void KeyReceived(int key)
        {
            m_PlayingControl.KeyReceived(key);
        }

        public void SwitchElement(Int32 id)
        {
            m_PlayingControl.SwitchElement(id);
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

        public void ProjectShallChange(string newProjectFile)
        {
            lock (syncObject)
            {
                controllerFileName = newProjectFile;
            }
        }

        public void SetMusicRepeat(bool repeat)
        {
            m_PlayingControl.SetRepeatCurrentMusic(repeat);
        }

        public void SetTagCategoryOperator(bool isAndOperator)
        {
            m_PlayingControl.SetMusicTagCategoriesOperator(isAndOperator);
        }

        public void SwitchTag(int categoryId, int tagId, bool isActive)
        {
            if (isActive)
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

        public void SetMusicTagsFading(int fadeTime, bool fadeOnlyOnChange)
        {
            m_PlayingControl.SetMusicTagFading(fadeTime, fadeOnlyOnChange);
        }
    }
}
