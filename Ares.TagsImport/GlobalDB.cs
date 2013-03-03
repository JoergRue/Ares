/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
using System.Runtime.InteropServices;
using System.Xml;

using System.Threading;
using System.Threading.Tasks;


namespace Ares.TagsImport
{
    [Serializable]
    public class GlobalDbException : Exception
    {
        public GlobalDbException(String message)
            : base(message)
        {
        }

        public GlobalDbException(Exception inner)
            : base(inner.Message, inner)
        {
        }

        private GlobalDbException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    class GlobalDb
    {
        public static String BaseUrl { get { return Settings.Default.GlobalDbUrl; } }
        public static String UploadPath { get { return Settings.Default.UploadPath; } }
        public static String DownloadPath { get { return Settings.Default.DownloadPath; } }
    }

    public class GlobalDbUpload
    {
        private Ares.ModelInfo.IProgressMonitor m_Monitor;
        private CancellationTokenSource m_TokenSource;

        public static String UploadTags(Ares.ModelInfo.IProgressMonitor monitor, IList<String> files, String user, bool includeLog, CancellationTokenSource cancellationTokenSource)
        {
            GlobalDbUpload instance = new GlobalDbUpload(monitor, cancellationTokenSource);
            return instance.DoUploadTags(files, user, includeLog);
        }

        private GlobalDbUpload(Ares.ModelInfo.IProgressMonitor monitor, CancellationTokenSource tokenSource)
        {
            m_Monitor = monitor;
            m_TokenSource = tokenSource;
        }

        private static String ObfuscateUser(String user)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user));
            byte[] hashBytes = md5.Hash;
            return Convert.ToBase64String(hashBytes);
        }

        private String DoUploadTags(IList<String> files, String user, bool includeLog)
        {
            m_Monitor.SetIndeterminate(StringResources.UploadingTags);
            var exportedData = Ares.Tags.TagsModule.GetTagsDB().FilesInterface.ExportDatabaseForGlobalDB(files);

            var request = new RestSharp.RestRequest(GlobalDb.UploadPath, RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            String serializedTagsData = ServiceStack.Text.TypeSerializer.SerializeToString<Ares.Tags.TagsExportedData>(exportedData);
            request.AddParameter("TagsData", serializedTagsData);
            request.AddParameter("User", ObfuscateUser(user));
            request.AddParameter("IncludeLog", includeLog);
            if (Settings.Default.UseTestDB)
            {
                request.AddParameter("Test", true);
            }
            var client = new RestSharp.RestClient();
            client.BaseUrl = GlobalDb.BaseUrl;
            client.Timeout = 10 * 1000;
            var response = client.Execute<UploadResponse>(request);

            m_TokenSource.Token.ThrowIfCancellationRequested();
            if (response.ErrorException != null)
            {
                throw new GlobalDbException(response.ErrorException);
            }
            if (response.Data == null)
            {
                throw new GlobalDbException(String.IsNullOrEmpty(response.ErrorMessage) ? "No data received" : response.ErrorMessage);
            }
            if (response.Data.Status != 0)
            {
                throw new GlobalDbException(response.Data.ErrorMessage);
            }

            return response.Data.Log;
        }
    }

    public class GlobalDbDownload
    {
        private Ares.ModelInfo.IProgressMonitor m_Monitor;
        private CancellationTokenSource m_TokenSource;

        public static String DownloadTags(Ares.ModelInfo.IProgressMonitor monitor, IList<String> files, out int nrOfFoundFiles, bool includeLog, CancellationTokenSource cancellationTokenSource)
        {
            GlobalDbDownload instance = new GlobalDbDownload(monitor, cancellationTokenSource);
            return instance.DoDownloadTags(files, out nrOfFoundFiles, includeLog);
        }

        private GlobalDbDownload(Ares.ModelInfo.IProgressMonitor monitor, CancellationTokenSource tokenSource)
        {
            m_Monitor = monitor;
            m_TokenSource = tokenSource;
        }

        private String DoDownloadTags(IList<String> files, out int nrOfFoundFiles, bool includeLog)
        {
            nrOfFoundFiles = 0;
            m_Monitor.SetIndeterminate(StringResources.QueryingGlobalDB);
            IList<Ares.Tags.FileIdentification> ids = Ares.Tags.TagsModule.GetTagsDB().ReadInterface.GetIdentificationForFiles(files);
            List<Ares.Tags.FileIdentification> usableIds = new List<Tags.FileIdentification>();
            foreach (var id in ids)
            {
                if (!String.IsNullOrEmpty(id.AcoustId))
                    usableIds.Add(id);
                else if (!String.IsNullOrEmpty(id.Artist) && !String.IsNullOrEmpty(id.Title))
                    usableIds.Add(id);
            }
            var request = new RestSharp.RestRequest(GlobalDb.DownloadPath, RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            String serializedFileIds = ServiceStack.Text.TypeSerializer.SerializeToString<List<Ares.Tags.FileIdentification>>(usableIds);
            request.AddParameter("FileIdentification", serializedFileIds);
            if (Settings.Default.UseTestDB)
            {
                request.AddParameter("Test", true);
            }
            var client = new RestSharp.RestClient();
            client.BaseUrl = GlobalDb.BaseUrl;
            client.Timeout = 10 * 1000;
            var response = client.Execute<DownloadResponse>(request);
            m_TokenSource.Token.ThrowIfCancellationRequested();
            if (response.ErrorException != null)
            {
                throw new GlobalDbException(response.ErrorException);
            }
            if (response.Data == null)
            {
                throw new GlobalDbException(String.IsNullOrEmpty(response.ErrorMessage) ? "No data received" : response.ErrorMessage);
            }
            if (response.Data.Status != 0)
            {
                throw new GlobalDbException(response.Data.ErrorMessage);
            }
            if (response.Data.TagsData == null)
            {
                throw new GlobalDbException("No data received");
            }

            nrOfFoundFiles = response.Data.NrOfFoundFiles;
            m_Monitor.SetIndeterminate(StringResources.AddingTags);
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                Ares.Tags.TagsModule.GetTagsDB().FilesInterface.ImportDataFromGlobalDB(response.Data.TagsData, writer);
                return includeLog ? writer.ToString() : String.Empty;
            }
        }
    }

    class UploadResponse
    {
        public String ErrorMessage { get; set; }
        public int Status { get; set; }
        public String Log { get; set; }
    }

    class DownloadResponse
    {
        public int Status { get; set; }
        public String ErrorMessage { get; set; }
        public Ares.Tags.TagsExportedData TagsData { get; set; }
        public int NrOfFoundFiles { get; set; }
    }


}