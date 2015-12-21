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
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Host;
using ServiceStack.IO;
using ServiceStack.Logging;
using ServiceStack.Razor;
using ServiceStack.Razor.Managers;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using Ares.Data;
using Ares.Playing;
using Ares.Settings;

namespace Ares.Players.Web
{
    [Route("/Control", "GET")]
    public class ControlRequest
    {
    }

    [Route("/Modes", "GET")]
    public class ModesRequest
    {
    }

    [Route("/Elements", "GET")]
    public class ElementsRequest
    {
    }

    [Route("/Tags", "GET")]
    public class TagsRequest
    {
    }

    [Route("/GetProjects", "GET")]
    public class GetProjectsRequest
    {
    }

    [Route("/Playlist", "GET")]
    public class PlaylistRequest
    {
    }

    [Route("/Settings", "GET")]
    public class SettingsRequest
    {
        public String SourcePage { get; set; }
    }

    public class SettingsData
    {
        public bool PlayMusicOnAllSpeakers { get; set; }
        public int FadingOption { get; set; }
        public int FadingTime { get; set; }
        public String Language { get; set; }
    }

    [Route("/changeSettings")]
    public class ChangeSettingsRequest : SettingsData, IReturnVoid
    {
    }

    [Route("/openProject")]
    public class OpenProjectRequest : IReturnVoid
    {
        public String ProjectFileName { get; set; }
    }

    [Route("/changeVolume")]
    public class ChangeVolume : IReturnVoid
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    [Route("/selectMode")]
    public class SelectMode : IReturnVoid
    {
        public String Title { get; set; }
    }

    [Route("/triggerElement")]
    public class TriggerElement : IReturnVoid
    {
        public int Id { get; set; }
    }

    [Route("/mainControl")]
    public class MainControl : IReturnVoid
    {
        public String Command { get; set; }
    }

    [Route("/selectMusicElement")]
    public class SelectMusicElement : IReturnVoid
    {
        public int Id { get; set; }
    }

    [Route("/selectTagCategory")]
    public class SelectTagCategory : IReturnVoid
    {
        public int Id { get; set; }
    }

    [Route("/selectTagCombination")]
    public class SelectTagCombination : IReturnVoid
    {
        public int Option { get; set; }
    }

    [Route("/setTagFading")]
    public class SetTagFading : IReturnVoid
    {
        public int Time { get; set; }
        public bool OnlyOnChange { get; set; }
    }

    [Route("/switchTag")]
    public class SwitchTag : IReturnVoid
    {
        public int Id { get; set; }
    }

    [Route("/removeAllTags")]
    public class RemoveAllTags : IReturnVoid
    {

    }

    [Route("/resendInfo")]
    public class ResendInfo : IReturnVoid
    {
        public String InfoId { get; set; }
    }

    public class ElementTrigger
    {
        public String Name { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class VersionResponse
    {
        public String Version { get; set; }
        public String PageId { get { return mPageId; } set { mPageId = value; PageLink = Char.ToUpper(mPageId[0]) + mPageId.Substring(1); } }
        public String PageLink { get; set; }

        private String mPageId;
    }

    public class ControlResponse : VersionResponse
    {
    }

    public class ModesResponse : VersionResponse
    {
    }

    public class ElementsResponse : VersionResponse
    {
    }

    public class SettingsResponse : VersionResponse
    {
        public SettingsData Settings { get; set; }
        public String SourcePage { get; set; }
    }

    public class ProjectFile
    {
        public String FilePath { get; set; }
        public String ProjectName { get; set; }
    }

    public class ProjectsResponse : VersionResponse
    {
        public List<ProjectFile> Projects { get; set; }
        public String ActiveProject { get; set; }
    }

    public class TagsResponse : VersionResponse
    {
    }

    public class PlaylistResponse : VersionResponse
    {
    }

    [DefaultView("Modes")]
    public class ModesService : Service
    {
        public object Get(ModesRequest request)
        {
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();

            ModesResponse res = new ModesResponse() { Version = assemblyVersion, PageId = "modes" };

            return res;
        }
    }

    [DefaultView("Elements")]
    public class ElementsService : Service
    {
        public object Get(ElementsRequest request)
        {
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();

            ElementsResponse res = new ElementsResponse() { Version = assemblyVersion, PageId = "elements" };

            return res;
        }
    }

    [DefaultView("Settings")]
    public class SettingsService : Service
    {
        public object Get(SettingsRequest request)
        {
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();

            SettingsData settings = InfoSender.Settings;
            settings.Language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "de" : "en";

            SettingsResponse res = new SettingsResponse() { Version = assemblyVersion, PageId = "settings", Settings = settings, SourcePage = request.SourcePage };

            return res;
        }

        public object Any(ChangeSettingsRequest request)
        {
            NetworkClient.SetPlayMusicOnAllSpeakers(request.PlayMusicOnAllSpeakers);
            NetworkClient.SetFadingOnPreviousNext(request.FadingOption, request.FadingTime);

            var httpRequest = base.Request;
            var session = httpRequest.GetSession();
            if (session is AuthUserSession)
            {
                var auths = ((AuthUserSession)session);
                auths.Culture = request.Language;
                httpRequest.SaveSession(session);
                httpRequest.SetItem("Culture", auths.Culture);
            }

            return null;
        }

        public INetworkClient NetworkClient { get; set; }
        public InfoSender InfoSender { get;  set;}
    }

    [DefaultView("Tags")]
    public class TagsService : Service
    {
        public object Get(TagsRequest request)
        {
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();

            TagsResponse res = new TagsResponse() { Version = assemblyVersion, PageId = "tags" };

            return res;
        }

        public object Any(SelectTagCategory request)
        {
            InfoSender.SelectTagCategory(request.Id);
            return null;
        }

        public object Any(SelectTagCombination request)
        {
            NetworkClient.SetTagCategoryCombination((Data.TagCategoryCombination)request.Option);
            return null;
        }

        public object Any(SetTagFading request)
        {
            NetworkClient.SetMusicTagsFading(request.Time, request.OnlyOnChange);
            return null;
        }

        public object Any(SwitchTag request)
        {
            NetworkClient.SwitchTag(InfoSender.ActiveCategory, request.Id, !InfoSender.IsTagActive(request.Id));
            return null;
        }

        public object Any(RemoveAllTags request)
        {
            NetworkClient.DeactivateAllTags();
            return null;
        }

        public InfoSender InfoSender { get; set; }
        public INetworkClient NetworkClient { get; set; }
    }

    [DefaultView("Playlist")]
    public class PlaylistService : Service
    {
        public object Get(PlaylistRequest request)
        {
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();

            PlaylistResponse res = new PlaylistResponse() { Version = assemblyVersion, PageId = "playlist" };

            return res;
        }
    }

    [DefaultView("Projects")]
    public class ProjectsService : Service
    {
        public object Get(GetProjectsRequest request)
        {
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();
            String directory = NetworkClient.GetProjectsDirectory();
            String[] files1 = System.IO.Directory.GetFiles(directory, "*.ares");
            String[] files2 = System.IO.Directory.GetFiles(directory, "*.apkg");
            String[] files = new String[files1.Length + files2.Length];
            String[] paths = new String[files1.Length + files2.Length];
            for (int i = 0; i < files1.Length; ++i)
            {
                files[i] = System.IO.Path.GetFileName(files1[i]);
                paths[i] = files1[i];
            }
            for (int i = 0; i < files2.Length; ++i)
            {
                files[i + files1.Length] = System.IO.Path.GetFileName(files2[i]);
                paths[i + files1.Length] = files2[i];
            }
            Array.Sort(files, paths, StringComparer.CurrentCultureIgnoreCase);
            List<ProjectFile> projects = new List<ProjectFile>();
            if (files.Length == 0)
            {
                var lastProjects = NetworkClient.GetLastUsedProjects();
                foreach (var project in lastProjects.GetFiles())
                {
                    projects.Add(new ProjectFile() { FilePath = project.FilePath.Replace('\\', '/'), ProjectName = project.ProjectName });
                }
            }
            else
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    projects.Add(new ProjectFile() { FilePath = paths[i].Replace('\\', '/'), ProjectName = files[i] });
                }
            }
            var currentProject = NetworkClient.GetCurrentProject();
            String currentProjectName = currentProject != null ? currentProject.FileName : "";
            return new ProjectsResponse { Version = assemblyVersion, Projects = projects, ActiveProject = currentProjectName, PageId = "projects" };
        }

        public object Any(OpenProjectRequest request)
        {
            NetworkClient.ProjectShallChange(request.ProjectFileName.Replace('/', '\\'));

            return null;
        }

        public INetworkClient NetworkClient { get; set; }
    }

    [DefaultView("Control")]
    public class ControlService : Service
    {
        public object Get(ControlRequest request)
        {
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();

            ControlResponse res = new ControlResponse() { Version = assemblyVersion, PageId = "control" };

            return res;
        }

        public object Any(ChangeVolume request)
        {
            NetworkClient.VolumeReceived((Playing.VolumeTarget)request.Id, request.Value);
            return null;
        }

        public object Any(SelectMode request)
        {
            NetworkClient.ChangeMode(request.Title);
            return null;
        }

        public object Any(TriggerElement request)
        {
            NetworkClient.SwitchElement(request.Id);
            return null;
        }

        public object Any(MainControl request)
        {
            if (request.Command == "Stop")
                NetworkClient.KeyReceived((int)Ares.Data.Keys.Escape);
            else if (request.Command == "Back")
                NetworkClient.KeyReceived((int)Ares.Data.Keys.Left);
            else if (request.Command == "Forward")
                NetworkClient.KeyReceived((int)Ares.Data.Keys.Right);
            else if (request.Command == "RepeatOn")
                NetworkClient.SetMusicRepeat(true);
            else if (request.Command == "RepeatOff")
                NetworkClient.SetMusicRepeat(false);
            return null;
        }

        public object Any(SelectMusicElement request)
        {
            NetworkClient.PlayOtherMusic(request.Id);
            return null;
        }

        public object Any(ResendInfo request)
        {
            if (request.InfoId == "MusicList")
            {
                InfoSender.DoInformMusicList();
            }
            else if (request.InfoId == "Elements")
            {
                InfoSender.InformOfElements();
            }
            else if (request.InfoId == "Modes")
            {
                InfoSender.InformOfModes();
            }
            else if (request.InfoId == "Tags")
            {
                InfoSender.InformOfTags();
            }
            return null;
        }

        public InfoSender InfoSender { get; set; }
        public INetworkClient NetworkClient { get; set; }
    }

    public class ErrorInfo
    {
        public String ErrorMessage { get; set; }
        public int ElementId { get; set; }
    }

    public class NewProjectInfo
    {
        public String Name { get; set; }
        public List<String> Modes { get; set; }
    }

    public class VolumeInfo
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public class MusicInfo
    {
        public String LongTitle { get; set; }
        public String ShortTitle { get; set; }
    }

    public class ModeInfo
    {
        public String Title { get; set; }
        public List<ElementTrigger> Triggers { get; set; }
    }

    public class ActiveElementsInfo
    {
        public List<ElementTrigger> Triggers { get; set; }
    }

    public class MusicRepeatInfo
    {
        public bool Repeat { get; set; }
    }

    public class MusicListInfo
    {
        public List<int> Ids { get; set; }
        public List<String> Titles { get; set; }
    }

    public class TagInfo
    {
        public int ActiveCategory { get; set; }
        public List<ElementTrigger> Categories { get; set; }
        public Dictionary<int, List<ElementTrigger>> TagsPerCategory { get; set; }
    }

    public class ActiveTagInfo
    {
        public List<ElementTrigger> Tags { get; set; }
        public int CategoryCombination { get; set; }
        public int FadeTime { get; set; }
    }

    public class TagFadingInfo
    {
        public int FadeTime { get; set; }
        public bool FadeOnlyOnChange { get; set; }
    }

    public class EventsSender
    {
        private Object mSyncObject = new object();
        private System.Threading.AutoResetEvent mEvent = new System.Threading.AutoResetEvent(false);
        private System.Collections.Generic.List<System.Action> mActionQueue = new List<Action>();
        private bool mContinue = true;

        private System.Timers.Timer mWatchdogTimer;

        private void SetWatchdog()
        {
            mWatchdogTimer.Start();
        }

        private void CancelWatchdog()
        {
            mWatchdogTimer.Stop();
        }

        private void OnWatchdogTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            if (mSenderThread != null)
                mSenderThread.Abort();
        }

        private void ThreadFunction()
        {
            bool continueThread = true;
            Action nextAction = null;
            while (continueThread)
            {
                if (nextAction == null)
                {
                    mEvent.WaitOne(10);
                }
                lock (mSyncObject)
                {
                    continueThread = mContinue;
                    if (mActionQueue.Count > 0)
                    {
                        nextAction = mActionQueue[0];
                        mActionQueue.RemoveAt(0);
                    }
                    else
                    {
                        nextAction = null;
                    }
                }
                if (continueThread && nextAction != null)
                {
                    try
                    {
                        SetWatchdog();
                        nextAction();
                        CancelWatchdog();
                    }
                    catch (System.Threading.ThreadAbortException ex)
                    {
                        WriteExceptionTrace(ex);
                        System.Threading.Thread.ResetAbort();
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionTrace(ex);
                    }
                    nextAction = null;
                }
            }
        }

        private System.Threading.Thread mSenderThread = null;

        public void StartSenderThread()
        {
            if (mSenderThread == null)
            {
                mWatchdogTimer = new System.Timers.Timer();
                mWatchdogTimer.AutoReset = false;
				#if MONO
                mWatchdogTimer.Interval = 5000;
				#else
				mWatchdogTimer.Interval = 1000;
				#endif
                mWatchdogTimer.Elapsed += OnWatchdogTimer;
                mSenderThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFunction));
                mContinue = true;
                mSenderThread.Start();
            }
        }

        public void StopSenderThread()
        {
            if (mSenderThread != null)
            {
                lock (mSyncObject)
                {
                    mContinue = false;
                }
                mEvent.Set();
                if (mSenderThread != System.Threading.Thread.CurrentThread)
                {
                    mSenderThread.Join();
                }
                mSenderThread = null;
                mWatchdogTimer.Dispose();
                mWatchdogTimer = null;
                lock (mSyncObject)
                {
                    mActionQueue.Clear();
                }
            }
        }

        public void Send(Action a)
        {
            if (mSenderThread == null)
                return;

            if (mSenderThread.ThreadState == System.Threading.ThreadState.Aborted)
            {
                mSenderThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFunction));
                mSenderThread.Start();
            }

            lock (mSyncObject)
            {
                mActionQueue.Add(a);
            }
            mEvent.Set();
        }

        public static EventsSender Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new EventsSender();
                return sInstance;
            }
        }

        private static EventsSender sInstance = null;

        private EventsSender()
        {
        }

		private static void DoWriteException(System.IO.TextWriter writer, Exception ex, String indent)
		{
			writer.WriteLine(indent + System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteLine(indent + ex.GetType().Name + ": " + ex.Message);
			writer.WriteLine(indent + "Stack Trace:");
			writer.WriteLine(indent + ex.StackTrace);
			if (ex.InnerException != null)
			{
				writer.WriteLine("INNER exception:");
				DoWriteException(writer, ex.InnerException, indent + "    ");
			}
			writer.WriteLine("--------------------------------------------------");
		}

        private static void WriteExceptionTrace(Exception ex)
        {
            try
            {
				#if !MONO
                String folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                String path = System.IO.Path.Combine(folder, "Ares_Errors.log");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true))
                {
				#else
				System.IO.TextWriter writer = System.Console.Error;
				#endif
				    DoWriteException(writer, ex, String.Empty);
				#if !MONO
					writer.Flush();
                }
				#endif
            }
            catch (Exception)
            {
				System.Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }


    public class InfoSender : Ares.Playing.IProjectPlayingCallbacks
    {
        public IServerEvents ServerEvents { get; set; } // public to enable IoC

        public INetworkClient NetworkClient { get; set; }

        public InfoSender()
        {

        }

        private Dictionary<int, int> mActiveElements = new Dictionary<int, int>();

        public void ErrorOccurred(int elementId, string errorMessage)
        {
           EventsSender.Instance.Send(() =>
           {
               ServerEvents.NotifyAll(new ErrorInfo() { ElementId = elementId, ErrorMessage = errorMessage });
           });
        }

        private void DoNotifyChannel(string channel, Object msg)
        {
            EventsSender.Instance.Send(() => ServerEvents.NotifyChannel(channel, msg));
        }

        private void DoNotifyAll(object msg)
        {
            EventsSender.Instance.Send(() => ServerEvents.NotifyAll(msg));
        }

        public void SelectTagCategory(int id)
        {
            mActiveCategory = id;
            InformClientOfPossibleTags(mTagLanguageId, mProject);
        }

        public void InformOfTags()
        {
            InformClientOfPossibleTags(mTagLanguageId, mProject);
            InformClientOfTagFading(mFadeTime, mFadeOnlyOnChange);
        }

        public void InformClientOfPossibleTags(int languageId, Data.IProject project)
        {
            mProject = project;
            try
            {
                var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId);
                var categories = dbRead.GetAllCategories();
                HashSet<int> hiddenCategories = project != null ? project.GetHiddenTagCategories() : new HashSet<int>();
                HashSet<int> hiddenTags = project != null ? project.GetHiddenTags() : new HashSet<int>();

                List<ElementTrigger> categoryList = new List<ElementTrigger>();
                Dictionary<int, List<ElementTrigger>> tags = new Dictionary<int, List<ElementTrigger>>();

                bool activeCategoryIsValid = false;
                int defaultCategory = 0;

                foreach (var category in categories)
                {
                    if (hiddenCategories.Contains(category.Id))
                        continue;
                    defaultCategory = category.Id;
                    categoryList.Add(new ElementTrigger { Name = category.Name, Id = category.Id, IsActive = true });
                    tags[category.Id] = new List<ElementTrigger>();
                    if (mActiveCategory == category.Id)
                        activeCategoryIsValid = true;
                    // all tags for the category
                    var dbTags = dbRead.GetAllTags(category.Id);
                    foreach (var tag in dbTags)
                    {
                        if (hiddenTags.Contains(tag.Id))
                            continue;
                        tags[category.Id].Add(new ElementTrigger { Name = tag.Name, Id = tag.Id, IsActive = mActiveTagIds.Contains(tag.Id) });
                    }
                }

                if (!activeCategoryIsValid)
                {
                    mActiveCategory = defaultCategory;
                }

                DoNotifyChannel("Tags", new TagInfo() { ActiveCategory = mActiveCategory, Categories = categoryList, TagsPerCategory = tags });
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                Messages.AddMessage(MessageType.Error, String.Format(StringResources.TagsDbError, ex.Message));
            }

        }

        private int mActiveCategory;
        public int ActiveCategory {  get { return mActiveCategory; } }
        private HashSet<int> mActiveTagIds = new HashSet<int>();
        public bool IsTagActive(int id) { return mActiveTagIds.Contains(id); }
        private Data.TagCategoryCombination mCategoryCombination;
        private int mFadeTime;
        private bool mFadeOnlyOnChange;
        private Data.IProject mProject;

        private void InformClientOfActiveTags(int tagLanguageId, ICollection<int> tagIds, Data.TagCategoryCombination categoryCombination, int fadeTime)
        {
            try
            {
                var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(tagLanguageId);
                var infoList = dbRead.GetTagInfos(tagIds);
                var activeTags = new List<ElementTrigger>();
                foreach (var info in infoList) activeTags.Add(new ElementTrigger { Id = info.Id, Name = info.Name, IsActive = true });
                DoNotifyChannel("Tags", new ActiveTagInfo() { Tags = activeTags, CategoryCombination = (int)categoryCombination, FadeTime = fadeTime });
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                Messages.AddMessage(MessageType.Error, String.Format(StringResources.TagsDbError, ex.Message));
            }
        }

        private void InformClientOfTagFading(int fadeTime, bool fadeOnlyOnChange)
        {
            mFadeTime = fadeTime;
            mFadeOnlyOnChange = fadeOnlyOnChange;
            DoNotifyChannel("Tags", new TagFadingInfo() { FadeOnlyOnChange = fadeOnlyOnChange, FadeTime = fadeTime });
        }

        private Data.IProject mActiveProject = null;

        public void InformClientOfProject(Data.IProject newProject)
        {
            mActiveProject = newProject;
            List<String> modes = new List<string>();
            if (newProject != null)
            {
                foreach (var mode in newProject.GetModes()) modes.Add(mode.Title);
                DoNotifyAll(new NewProjectInfo() { Name = newProject.Title, Modes = modes });
            }
            else
            {
                DoNotifyAll(new NewProjectInfo() { Name = "", Modes = modes });
            }
        }

        private void SetElementActive(int id)
        {
            if (mActiveElements.ContainsKey(id))
                mActiveElements[id] = mActiveElements[id] + 1;
            else
                mActiveElements[id] = 1;
        }

        private bool SetElementInactive(int id)
        {
            if (!mActiveElements.ContainsKey(id))
                return false;
            int value = mActiveElements[id];
            if (value > 1)
                mActiveElements[id] = value - 1;
            else
                mActiveElements.Remove(id);
            return value > 1;
        }

        public void InformOfActiveElements()
        {
            List<ElementTrigger> activeElements = new List<ElementTrigger>();
            foreach (int id in mActiveElements.Keys)
            {
                for (int i = 0; i < mActiveElements[id]; ++i)
                    activeElements.Add(new ElementTrigger { Id = id, IsActive = true, Name = Data.DataModule.ElementRepository.GetElement(id).Title });
            }
            DoNotifyChannel("Control", new ActiveElementsInfo { Triggers = activeElements });
            DoNotifyChannel("Elements", new ActiveElementsInfo { Triggers = activeElements });
        }

        private int mActiveMusicListId = -1;

        private void InformMusicList(int musicListId)
        {
            mActiveMusicListId = musicListId;
            DoInformMusicList();
        }

        public void DoInformMusicList()
        {
            List<int> ids = new List<int>();
            List<string> titles = new List<string>();
            Ares.Data.IElement element = mActiveMusicListId != -1 ? Ares.Data.DataModule.ElementRepository.GetElement(mActiveMusicListId) : null;
            Ares.Data.IMusicList musicList = (element != null && element is Ares.Data.IMusicList) ? element as Ares.Data.IMusicList : null;
            if (musicList != null)
            {
                foreach (Ares.Data.IFileElement fileElement in musicList.GetFileElements())
                {
                    ids.Add(fileElement.Id);
                    titles.Add(fileElement.Title);
                }
            }
            DoNotifyChannel("Playlist", new MusicListInfo { Ids = ids, Titles = titles });
            DoNotifyChannel("Control", new MusicListInfo { Ids = ids, Titles = titles });
        }

        private int mTagLanguageId;

        public void InformClientOfEverything(int overallVolume, int musicVolume, int soundVolume, Data.IMode mode, Ares.Players.MusicInfo music, IList<Data.IModeElement> elements, Data.IProject project, int musicListId, bool musicRepeat, int tagLanguageId, IList<int> activeTags, Data.TagCategoryCombination categoryCombination, int fadeTime, bool fadeOnlyOnChange, bool musicOnAllChannels, int fadeOnPreviousNextOption, int fadeOnPreviousNextTime)
        {
            InformClientOfProject(project);
            InformClientOfVolume(Playing.VolumeTarget.Both, overallVolume);
            InformClientOfVolume(Playing.VolumeTarget.Music, musicVolume);
            InformClientOfVolume(Playing.VolumeTarget.Sounds, soundVolume);
            DoNotifyChannel("Control", new MusicInfo { LongTitle = music.LongTitle, ShortTitle = music.ShortTitle });
            DoNotifyChannel("Playlist", new MusicInfo { LongTitle = music.LongTitle, ShortTitle = music.ShortTitle });
            mActiveElements.Clear();
            foreach (var element in elements)
                SetElementActive(element.Id);
            ModeChanged(mode);
            InformOfActiveElements();
            MusicRepeatChanged(musicRepeat);
            InformMusicList(musicListId);
            mTagLanguageId = tagLanguageId;
            mActiveTagIds.Clear();
            mActiveTagIds.UnionWith(activeTags);
            mCategoryCombination = categoryCombination;
            mFadeTime = fadeTime;
            InformClientOfPossibleTags(tagLanguageId, project);
            InformClientOfActiveTags(tagLanguageId, activeTags, categoryCombination, fadeTime);
            InformClientOfTagFading(fadeTime, fadeOnlyOnChange);
            MusicOnAllSpeakersChanged(musicOnAllChannels);
            Settings.FadingOption = fadeOnPreviousNextOption;
            Settings.FadingTime = fadeOnPreviousNextTime;
        }

        public void InformClientOfVolume(Ares.Playing.VolumeTarget target, int value)
        {
            DoNotifyChannel("Control", new VolumeInfo { Id = (int)target, Value = value });
        }

        public void InformClientOfFading(int fadeTime, bool fadeOnlyOnChange)
        {
            InformClientOfTagFading(fadeTime, fadeOnlyOnChange);
        }

        private Data.IMode mActiveMode = null;

        public void ModeChanged(Data.IMode newMode)
        {
            mActiveMode = newMode;
            InformOfElements(newMode);
        }

        public void InformOfModes()
        {
            InformClientOfProject(mActiveProject);
        }

        public void InformOfElements()
        {
            if (mActiveMode != null)
            {
                InformOfElements(mActiveMode);
            }
        }

        private void InformOfElements(Data.IMode mode)
        {
            List<ElementTrigger> triggers = new List<ElementTrigger>();
            foreach (var element in mode.GetElements())
            {
                if (element.IsVisibleInPlayer)
                    triggers.Add(new ElementTrigger { Id = element.Id, Name = element.Title, IsActive = mActiveElements.ContainsKey(element.Id) });
            }
            DoNotifyChannel("Control", new ModeInfo { Title = mode.Title, Triggers = triggers });
            DoNotifyChannel("Modes", new ModeInfo { Title = mode.Title, Triggers = triggers });
            DoNotifyChannel("Elements", new ModeInfo { Title = mode.Title, Triggers = triggers });
        }

        public void ModeElementStarted(Data.IModeElement element)
        {
            SetElementActive(element.Id);
            DoNotifyChannel("Control", new ElementTrigger { Id = element.Id, Name = element.Title, IsActive = true });
            DoNotifyChannel("Elements", new ElementTrigger { Id = element.Id, Name = element.Title, IsActive = true });
            InformOfActiveElements();
        }

        public void ModeElementFinished(Data.IModeElement element)
        {
            bool isActive = SetElementInactive(element.Id);
            DoNotifyChannel("Control", new ElementTrigger { Id = element.Id, Name = element.Title, IsActive = isActive });
            DoNotifyChannel("Elements", new ElementTrigger { Id = element.Id, Name = element.Title, IsActive = isActive });
            InformOfActiveElements();
        }

        public void SoundStarted(int elementId)
        {
        }

        public void SoundFinished(int elementId)
        {
        }

        public void MusicStarted(int elementId)
        {
            var info = Ares.Players.MusicInfo.GetInfo(elementId);
            DoNotifyChannel("Control", new MusicInfo { LongTitle = info.LongTitle, ShortTitle = info.ShortTitle });
            DoNotifyChannel("Playlist", new MusicInfo { LongTitle = info.LongTitle, ShortTitle = info.ShortTitle });
        }

        public void MusicFinished(int elementId)
        {
            DoNotifyChannel("Control", new MusicInfo { LongTitle = String.Empty, ShortTitle = String.Empty });
            DoNotifyChannel("Playlist", new MusicInfo { LongTitle = String.Empty, ShortTitle = String.Empty });
        }

        public void VolumeChanged(Playing.VolumeTarget target, int newValue)
        {
            // is informed seperately by the player when it updates itself
            // InformClientOfVolume(target, newValue);
        }

        public void MusicPlaylistStarted(int elementId)
        {
            InformMusicList(elementId);
        }

        public void MusicPlaylistFinished(int elementId)
        {
            if (mActiveMusicListId == elementId)
            {
                InformMusicList(-1);
            }
        }

        public void MusicRepeatChanged(bool repeat)
        {
            DoNotifyChannel("Control", new MusicRepeatInfo { Repeat = repeat });
            DoNotifyChannel("Playlist", new MusicRepeatInfo { Repeat = repeat });
        }

        private SettingsData mSettings = new SettingsData() { FadingOption = 0, FadingTime = 0, Language = "en", PlayMusicOnAllSpeakers = false };

        public SettingsData Settings { get { return mSettings; } }

        public void MusicOnAllSpeakersChanged(bool onAllSpeakers)
        {
            mSettings.PlayMusicOnAllSpeakers = onAllSpeakers;
        }

        public void PreviousNextFadingChanged(bool fade, bool crossFade, int fadeTime)
        {
            mSettings.FadingOption = (fade ? (crossFade ? 2 : 1) : 0);
            mSettings.FadingTime = fadeTime;
        }

        public void MusicTagAdded(int tagId)
        {
            mActiveTagIds.Add(tagId);
            InformClientOfActiveTags(mTagLanguageId, mActiveTagIds, mCategoryCombination, mFadeTime);
        }

        public void MusicTagRemoved(int tagId)
        {
            mActiveTagIds.Remove(tagId);
            InformClientOfActiveTags(mTagLanguageId, mActiveTagIds, mCategoryCombination, mFadeTime);
        }

        public void AllMusicTagsRemoved()
        {
            mActiveTagIds.Clear();
            InformClientOfActiveTags(mTagLanguageId, mActiveTagIds, mCategoryCombination, mFadeTime);
        }

        public void MusicTagCategoriesCombinationChanged(Data.TagCategoryCombination categoryCombination)
        {
            mCategoryCombination = categoryCombination;
            InformClientOfActiveTags(mTagLanguageId, mActiveTagIds, mCategoryCombination, mFadeTime);
        }

        public void MusicTagsChanged(ICollection<int> newTags, Data.TagCategoryCombination categoryCombination, int fadeTime)
        {
            mActiveTagIds.Clear();
            mActiveTagIds.UnionWith(newTags);
            mCategoryCombination = categoryCombination;
            mFadeTime = fadeTime;
            InformClientOfActiveTags(mTagLanguageId, newTags, categoryCombination, fadeTime);
        }

        public void MusicTagsFadingChanged(int fadeTime, bool fadeOnlyOnChange)
        {
            InformClientOfTagFading(fadeTime, fadeOnlyOnChange);
        }

        public void AddMessage(Playing.MessageType messageType, string message)
        {
            if (messageType == Ares.Playing.MessageType.Error)
                ErrorOccurred(-1, message);
        }
    }

    // needed because the Container in the AppHost disposes all its registered objects; 
    // but we don't want to dispose the original network client
    class NetworkClientProxy : INetworkClient
    {
        public INetworkClient Client { get; set; }

        public void ChangeMode(string title)
        {
            Client.ChangeMode(title);
        }

        public void ClientConnected()
        {
            Client.ClientConnected();
        }

        public void ClientDataChanged(bool listenAgainAfterDisconnect)
        {
            Client.ClientDataChanged(listenAgainAfterDisconnect);
        }

        public void DeactivateAllTags()
        {
            Client.DeactivateAllTags();
        }

        public IProject GetCurrentProject()
        {
            return Client.GetCurrentProject();
        }

        public RecentFiles GetLastUsedProjects()
        {
            return Client.GetLastUsedProjects();
        }

        public string GetProjectsDirectory()
        {
            return Client.GetProjectsDirectory();
        }

        public void KeyReceived(int key)
        {
            Client.KeyReceived(key);
        }

        public void PlayOtherMusic(int elementId)
        {
            Client.PlayOtherMusic(elementId);
        }

        public void ProjectShallChange(string newProjectFile)
        {
            Client.ProjectShallChange(newProjectFile);
        }

        public void SetFadingOnPreviousNext(int option, int fadeTime)
        {
            Client.SetFadingOnPreviousNext(option, fadeTime);
        }

        public void SetMusicRepeat(bool repeat)
        {
            Client.SetMusicRepeat(repeat);
        }

        public void SetMusicTagsFading(int fadeTime, bool onlyOnChange)
        {
            Client.SetMusicTagsFading(fadeTime, onlyOnChange);
        }

        public void SetPlayMusicOnAllSpeakers(bool onAllSpeakers)
        {
            Client.SetPlayMusicOnAllSpeakers(onAllSpeakers);
        }

        public void SetTagCategoryCombination(TagCategoryCombination categoryCombination)
        {
            Client.SetTagCategoryCombination(categoryCombination);
        }

        public void SwitchElement(int elementId)
        {
            Client.SwitchElement(elementId);
        }

        public void SwitchTag(int categoryId, int tagId, bool tagIsActive)
        {
            Client.SwitchTag(categoryId, tagId, tagIsActive);
        }

        public void VolumeReceived(VolumeTarget target, int value)
        {
            Client.VolumeReceived(target, value);
        }
    }

    class AppHost : AppHostHttpListenerBase
    {
        public InfoSender InfoSender { get; set; }

        private HashSet<IEventSubscription> mSubscriptions = new HashSet<IEventSubscription>();

        public bool ClientConnected
        {
            get { return mSubscriptions.Count > 0 && InfoSender != null; }
        }

        public string ClientName
        {
            get { return "Web Controller"; }
        }

        private void Connected(IEventSubscription es)
        {
        }

        private void Subscribed(IEventSubscription es)
        {
            mSubscriptions.Add(es);
            Messages.AddMessage(MessageType.Debug, "WebClient subscribed: " + es.DisplayName);
            EventsSender.Instance.StartSenderThread();
            if (InfoSender == null)
            {
                InfoSender = mContainer.Resolve<InfoSender>();
                Ares.Playing.PlayingModule.AddCallbacks(InfoSender);
            }
            mNetworkClient.ClientDataChanged(false);
        }

        private void Unsubscribed(IEventSubscription es)
        {
            mSubscriptions.Remove(es);
            Messages.AddMessage(MessageType.Debug, "WebClient unsubscribed: " + es.DisplayName);
            if (mSubscriptions.Count == 0)
            {
                Ares.Playing.PlayingModule.RemoveCallbacks(InfoSender);
                EventsSender.Instance.StopSenderThread();
                InfoSender = null;
                mNetworkClient.ClientDataChanged(false);
            }
        }

        private INetworkClient mNetworkClient;

        public AppHost(INetworkClient client) : base("Ares Player HttpListener", typeof(ControlService).Assembly) { mNetworkClient = new NetworkClientProxy() { Client = client }; }

        private Funq.Container mContainer;


        public override void Configure(Funq.Container container)
        {
            mContainer = container;

            container.RegisterAutoWired<InfoSender>();
            container.Register(mNetworkClient);

            Plugins.Add(new AuthFeature(() =>
                new AuthUserSession(),
                new IAuthProvider[] {
                    new BasicAuthProvider()
                }));
            Plugins.Add(new CultureAwareRazorFormat());

            PreRequestFilters.Add((httpReq, httpResp) =>
            {
                var session = httpReq.GetSession();
                if (session is AuthUserSession)
                {
                    var auths = ((AuthUserSession)session);
                    if (auths.Culture == null)
                    {
                        var languages = httpReq.Headers["Accept-Language"];
                        auths.Culture = GetBestAcceptLanguageMatch(languages);

                        httpReq.SaveSession(session);
                    }
                    httpReq.SetItem("Culture", auths.Culture);
                }
            });

            var userRep = new InMemoryAuthRepository();
            container.Register<IUserAuthRepository>(userRep);
            container.Register<ICacheClient>(c => new MemoryCacheClient());

            var sef = new ServerEventsFeature();
            Plugins.Add(sef);

            sef.OnConnect = (IEventSubscription es, Dictionary<string, string> dict) => { dict["retry"] = "10000"; Connected(es); };
            sef.OnSubscribe = (IEventSubscription es) => { Subscribed(es); };
            sef.OnUnsubscribe = (IEventSubscription es) => { Unsubscribed(es); };

            SetConfig(new HostConfig {
#if DEBUG
                DebugMode = true,
                WebHostPhysicalPath = "~/../../Ares.Players".MapServerPath(),
#endif
                DefaultRedirectPath = "/Control"
            });
        }

        private static string GetBestAcceptLanguageMatch(string langHeader)
        {
            String[] languageValues = langHeader.Split(',');
            if (languageValues.Length == 0) return "en";
            string[] languages = new string[languageValues.Length];
            double[] prefs = new double[languageValues.Length];
            for (int i = 0; i < languageValues.Length; ++i)
            {
                int index = languageValues[i].IndexOf(';');
                if (index == -1)
                {
                    languages[i] = languageValues[i];
                    prefs[i] = 1.0;
                }
                else if (index == languageValues[i].Length - 1)
                {
                    languages[i] = languageValues[i].Substring(0, index);
                    prefs[i] = 1.0;
                }
                else if (languageValues[i].Substring(index + 1).StartsWith("q="))
                {
                    double pref;
                    string qualString = languageValues[i].Substring(index + 3);
                    if (double.TryParse(qualString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out pref))
                    {
                        languages[i] = languageValues[i].Substring(0, index);
                        prefs[i] = pref;
                    }
                    else
                    {
                        languages[i] = languageValues[i].Substring(0, index);
                        prefs[i] = 1.0;
                    }
                }
                else
                {
                    languages[i] = languageValues[i].Substring(0, index);
                    prefs[i] = 1.0;
                }
            }
            System.Array.Sort(prefs, languages);
            for (int i = languages.Length; i > 0; --i)
            {
                if (languages[i-1].StartsWith("en"))
                    return "en";
                else if (languages[i-1].StartsWith("de"))
                    return "de";
            }
            return "en";
        }

        public override IServiceRunner<TRequest> CreateServiceRunner<TRequest>(ActionContext actionContext)
        {
            var runner = base.CreateServiceRunner<TRequest>(actionContext);
            return new CultureAwareServiceRunner<TRequest>(this, actionContext);
        }
    }

    public class WebNetwork : INetwork
    {
        public WebNetwork(INetworkClient networkClient)
        {
            mNetworkClient = networkClient;
            LogManager.LogFactory = new ConsoleLogFactory();
        }

        private String mListenAddress;

        private AppHost mAppHost;

        private INetworkClient mNetworkClient;

        public void ListenForClient()
        {
            int tcpPort = Settings.Settings.Instance.WebTcpPort;
            String ipAddress = Settings.Settings.Instance.IPAddress;
            try
            {
                mListenAddress = String.Format("http://+:{1}/", ipAddress, tcpPort);
                mAppHost = new AppHost(mNetworkClient);

                mAppHost.Init();
                mAppHost.Start(mListenAddress);

                Messages.AddMessage(MessageType.Info, "WebServer listening on " + mListenAddress);
            }
            catch (Exception e)
            {
                Messages.AddMessage(MessageType.Error, "Error starting WebServer: " + e.Message);
            }

        }


        public void StartUdpBroadcast()
        {
        }

        public bool SendUdpPacket()
        {
            return true;
        }

        public void StopUdpBroadcast()
        {
        }

        public void StopListenForClient()
        {
            mAppHost.Stop();
        }

        public bool ClientConnected
        {
            get { return mAppHost != null ? mAppHost.ClientConnected : false; }
        }

        public string ClientName
        {
            get { return mAppHost != null ? mAppHost.ClientName : String.Empty; }
        }

        public void DisconnectClient(bool listenAgain)
        {
        }

        public void Shutdown()
        {
            if (mAppHost != null)
                mAppHost.Dispose();
            EventsSender.Instance.StopSenderThread();
        }

        public void ErrorOccurred(int elementId, string errorMessage)
        {
            if (ClientConnected)
                mAppHost.InfoSender.ErrorOccurred(elementId, errorMessage);
        }

        public void InformClientOfPossibleTags(int languageId, Data.IProject project)
        {
            if (ClientConnected)
                mAppHost.InfoSender.InformClientOfPossibleTags(languageId, project);
        }

        public void InformClientOfProject(Data.IProject newProject)
        {
            if (ClientConnected)
                mAppHost.InfoSender.InformClientOfProject(newProject);
        }

        public void InformClientOfEverything(int overallVolume, int musicVolume, int soundVolume, Data.IMode mode, Ares.Players.MusicInfo music, IList<Data.IModeElement> elements, Data.IProject project, int musicListId, bool musicRepeat, int tagLanguageId, IList<int> activeTags, Data.TagCategoryCombination categoryCombination, int fadeTime, bool fadeOnlyOnChange, bool musicOnAllChannels, int fadeOnPreviousNextOption, int fadeOnPreviousNextTime)
        {
            if (ClientConnected)
                mAppHost.InfoSender.InformClientOfEverything(overallVolume, musicVolume, soundVolume, mode, music, elements, project, musicListId, musicRepeat, tagLanguageId, activeTags, categoryCombination, fadeTime, fadeOnlyOnChange, musicOnAllChannels, fadeOnPreviousNextOption, fadeOnPreviousNextTime);
        }

        public void InformClientOfVolume(Ares.Playing.VolumeTarget target, int value)
        {
            if (ClientConnected)
                mAppHost.InfoSender.InformClientOfVolume(target, value);
        }

        public void InformClientOfFading(int fadeTime, bool fadeOnlyOnChange)
        {
            if (ClientConnected)
                mAppHost.InfoSender.InformClientOfFading(fadeTime, fadeOnlyOnChange);
        }
    }

    class CultureAwareServiceRunner<T> : ServiceRunner<T>
    {

        public CultureAwareServiceRunner(AppHost appHost, ActionContext actionContext) :
            base(appHost, actionContext)
        { }

        public override void OnBeforeExecute(IRequest httpReq, T request)
        {
            httpReq.SetItem("PreviousCulture", System.Threading.Thread.CurrentThread.CurrentUICulture);
            string culture = httpReq.GetItem("Culture") as string;
            if (culture != null)
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(culture);
            base.OnBeforeExecute(httpReq, request);
        }

        public override object OnAfterExecute(IRequest httpReq, object response)
        {
            var prevCulture = httpReq.GetItem("PreviousCultureCulture") as System.Globalization.CultureInfo;
            if (prevCulture != null)
                System.Threading.Thread.CurrentThread.CurrentUICulture = prevCulture;
            return base.OnAfterExecute(httpReq, response);
        }
    }

    public class CultureAwareRazorFormat : RazorFormat
    {
        public override ServiceStack.Razor.Managers.RazorPageResolver CreatePageResolver()
        {
            return base.CreatePageResolver();
        }

        public override ServiceStack.Razor.Managers.RazorViewManager CreateViewManager()
        {
            return new CultureAwareRazorViewManager(this, VirtualPathProvider);
        }
    }


    public class CultureAwareRazorViewManager : RazorViewManager
    {
        public CultureAwareRazorViewManager(IRazorConfig viewConfig, IVirtualPathProvider virtualPathProvider) : base(viewConfig, virtualPathProvider)
        {

        }

        public override RazorPage GetPage(string absolutePath)
        {
            var extension = GetExtension(absolutePath);
            if (extension != null)
            {
                string path = absolutePath.Remove(absolutePath.IndexOf(extension, System.StringComparison.Ordinal));
                var localizedPage = TryGetLocalizedPage(path, extension, System.Threading.Thread.CurrentThread.CurrentUICulture);
                if (localizedPage != null)
                    return localizedPage;
            }

            return base.GetPage(absolutePath);
        }

        private static String GetExtension(String path)
        {
            int index = path.LastIndexOf('.');
            return index == -1 ? path : path.Substring(index);
        }

        private RazorPage TryGetLocalizedPage(string path, string extension, System.Globalization.CultureInfo culture)
        {
            var result = base.GetPage(path + "." + culture.ToString().ToLower() + extension);
            if (result != null) return result;

            result = base.GetPage(path + "." + culture.TwoLetterISOLanguageName + extension);
            if (result != null) return result;

            result = base.GetPage(path + "." + culture.ThreeLetterISOLanguageName + extension);
            if (result != null) return result;

            return null;
        }

    }
}
