/*
    Copyright (c) 2011 [Joerg Ruedenauer]
  
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
using System.Windows.Forms;
using System.ComponentModel;

namespace Ares.Online
{
    public class OnlineOperations
    {
        public static void ShowHelppage(String url, IWin32Window owner)
        {
            try
            {
               System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, String.Format(StringResources.OpenHelpError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CheckForUpdate(Form parent, bool verbose)
        {
            String url = GetUrlBase() + "ares_version.txt";
            FileDownloader<bool> downloader = new FileDownloader<bool>(parent, verbose);
            downloader.Download(url, StringResources.SearchingVersion, VersionDownloaded);
        }

        public static void DownloadSetup(Form parent, String version) 
        {
            String urlBase = "http://sourceforge.net/projects/aresrpg/files/";
            String urlAppendix = "/download";
            String windowsPart = "-Setup.exe";
            String linuxPart = "-Linux-x86-Install";
            bool isWindows = true;
            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    isWindows = false;
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    isWindows = true;
                    break;
                default:
                    MessageBox.Show(parent, StringResources.UnknownOS, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }
            String url = urlBase + version + "/Ares-" + version + (isWindows ? windowsPart : linuxPart) + urlAppendix;
            try
            {
                System.Diagnostics.Process.Start(url);
                if (MessageBox.Show(parent, StringResources.CloseAres, StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    parent.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(parent, String.Format(StringResources.DownloadError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ChangeLogDownloaded(System.Net.DownloadStringCompletedEventArgs result, Form parent, String version)
        {
            if (result.Cancelled)
                return;
            String text = result.Error != null ? String.Format(StringResources.ChangeLogError, result.Error.Message) : result.Result;
            ChangeLogDialog dialog = new ChangeLogDialog(text);
            DialogResult res = dialog.ShowDialog(parent);
            if (res == DialogResult.OK)
            {
                DownloadSetup(parent, version);
            }
        }

        private static String GetUrlBase()
        {
            return "http://aresrpg.sourceforge.net/";
        }
        
        private static void VersionDownloaded(System.Net.DownloadStringCompletedEventArgs result, Form parent, bool verbose)
        {
            Exception ex = result.Error;
            System.Version newVersion = null;
            if (ex == null)
            {
                String versionString = result.Result + ".0";
                try
                {
                    newVersion = new Version(versionString);
                }
                catch (Exception e)
                {
                    ex = e;
                }
            }
            if (ex != null)
            {
                MessageBox.Show(parent, String.Format(StringResources.DownloadError, result.Error.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (currentVersion < newVersion && (!verbose || !result.Cancelled))
            {
                NewVersionDialog dialog = new NewVersionDialog();
                DialogResult res = dialog.ShowDialog(parent);
                if (res == DialogResult.Cancel)
                    return;
                if (dialog.ShowChangeLog)
                {
                    String url = GetUrlBase() + StringResources.ChangeLogFile;
                    FileDownloader<String> downloader = new FileDownloader<String>(parent, result.Result);
                    downloader.Download(url, StringResources.GettingChangeLog, ChangeLogDownloaded);
                }
                else
                {
                    DownloadSetup(parent, result.Result);
                }
            }
            else if (verbose && !result.Cancelled)
            {
                MessageBox.Show(parent, StringResources.NoNewVersion, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private class FileDownloader<T>
        {
            private Form mParent;
            private String mUrl;
            private T mParam;
            private String mText;
            private Action<System.Net.DownloadStringCompletedEventArgs, Form, T> mCompletedFct;
            private ProgressMonitor mMonitor;
            private System.Net.WebClient mWebClient;

            public FileDownloader(Form parent, T t)
            {
                mParent = parent;
                mParam = t;
            }

            public void Download(String url, String text, Action<System.Net.DownloadStringCompletedEventArgs, Form, T> completedFct)
            {
                mUrl = url;
                mText = text;
                mCompletedFct = completedFct;

                mWebClient = new System.Net.WebClient();
                mWebClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                mWebClient.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                System.Uri uri;
                bool result = System.Uri.TryCreate(url, UriKind.Absolute, out uri);
                if (!result)
                {
                    MessageBox.Show(mParent, "Internal error: url \"" + url + "\" is invalid.", StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    mMonitor = new ProgressMonitor(mParent, text);
                    mWebClient.DownloadStringAsync(uri);
                }
            }

            void webClient_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
            {
                mMonitor.Close(new Action(() => { mCompletedFct(e, mParent, mParam); }));
            }

            void webClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
            {
                mMonitor.SetProgress(e.ProgressPercentage);
                if (mMonitor.Canceled)
                {
                    mWebClient.CancelAsync();
                }
            }

        }

        private class ProgressMonitor
        {
            private System.Timers.Timer timer;
            private bool canceled;
            private bool closed;
            private ProgressDialog dialog;
            private Form parent;

            private const int DELAY = 700;

            private Object syncObject = new object();

            public ProgressMonitor(Form parent, String text)
            {
                dialog = new ProgressDialog(text);
                closed = false;
                canceled = false;
                this.parent = parent;
                timer = new System.Timers.Timer(DELAY);
                timer.AutoReset = false;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }

            private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                lock (syncObject)
                {
                    if (closed)
                        return;
                }
                if (parent.InvokeRequired)
                {
                    parent.Invoke(new Action(() => { OpenDialog(); }));
                }
                else
                {
                    OpenDialog();
                }
            }

            private void OpenDialog()
            {
                lock (syncObject)
                {
                    if (closed)
                        return;
                }
                DialogClosed(dialog.ShowDialog(parent));
            }

            private void DialogClosed(DialogResult result)
            {
                lock (syncObject)
                {
                    if (!closed)
                        canceled = true;
                }
            }

            public bool Canceled
            {
                get
                {
                    lock (syncObject)
                    {
                        return canceled;
                    }
                }
            }

            public void Close(Action action)
            {
                lock (syncObject)
                {
                    closed = true;
                    if (timer.Enabled)
                    {
                        timer.Stop();
                    }
                }
                if (dialog != null && dialog.InvokeRequired)
                {
                    dialog.Invoke(new Action(() => { Close(action); }));
                }
                else
                {
                    if (dialog != null && dialog.Visible)
                    {
                        dialog.Visible = false;
                        dialog.Close();
                    }
                    timer.Dispose();
                    action();
                }
            }

            public void SetProgress(int progress)
            {
                if (dialog.InvokeRequired)
                {
                    dialog.Invoke(new Action(() => { dialog.SetProgress(progress); }));
                }
                else
                {
                    dialog.SetProgress(progress);
                }
            }
        }

    }
}
