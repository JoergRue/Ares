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
        private CancellationToken m_Token;

        public static Task<String> UploadTagsAsync(Ares.ModelInfo.IProgressMonitor monitor, IList<String> files, String user, bool includeLog, CancellationToken cancellationToken)
        {
            GlobalDbUpload instance = new GlobalDbUpload(monitor, cancellationToken);
            return Task.Factory.StartNew(() =>
                {
                    return instance.DoUploadTags(files, user, includeLog);
                });
        }

        private GlobalDbUpload(Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token)
        {
            m_Monitor = monitor;
            m_Token = token;
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

            var proxy = System.Net.WebRequest.GetSystemWebProxy();

            // upload in chunks; otherwise request size gets too large
            const int MAX_FILES_PER_REQUEST = 50;
            int nrOfRequests = (files.Count + MAX_FILES_PER_REQUEST - 1) / MAX_FILES_PER_REQUEST;
            StringBuilder logBuilder = new StringBuilder();
            for (int i = 0; i < nrOfRequests; ++i)
            {
                List<String> filesInRequest = new List<string>();
                for (int j = i * MAX_FILES_PER_REQUEST; j < (i+1)*MAX_FILES_PER_REQUEST; ++j)
                {
                    if (j >= files.Count)
                        break;
                    filesInRequest.Add(files[j]);
                }
                var exportedData = Ares.Tags.TagsModule.GetTagsDB().FilesInterface.ExportDatabaseForGlobalDB(filesInRequest);

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
                client.Timeout = 20 * 1000;
                var response = client.Execute<UploadResponse>(request);

                m_Token.ThrowIfCancellationRequested();
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
                if (i > 0)
                {
                    logBuilder.AppendLine("---------------------------------------------------------");
                }
                logBuilder.Append(response.Data.Log);
            }
            return logBuilder.ToString();
        }
    }

    public class GlobalDbDownload
    {
        private Ares.ModelInfo.IProgressMonitor m_Monitor;
        private CancellationToken m_Token;

        public class DownloadResult
        {
            public String Result { get; set; }
            public int NrOfFoundFiles { get; set; }
        }

        public static Task<DownloadResult> DownloadTagsAsync(Ares.ModelInfo.IProgressMonitor monitor, IList<String> files, bool includeLog, CancellationToken cancellationToken)
        {
            GlobalDbDownload instance = new GlobalDbDownload(monitor, cancellationToken);
            return Task.Factory.StartNew(() =>
                {
                    int nrOfFoundFiles = 0;
                    String res = instance.DoDownloadTags(files, out nrOfFoundFiles, includeLog);
                    return new DownloadResult { Result = res, NrOfFoundFiles = nrOfFoundFiles };
                });
        }

        private GlobalDbDownload(Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token)
        {
            m_Monitor = monitor;
            m_Token = token;
        }

        private String DoDownloadTags(IList<String> files, out int nrOfFoundFiles, bool includeLog)
        {
            var proxy = System.Net.WebRequest.GetSystemWebProxy();

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
            // download in chunks; otherwise request size gets too large
            const int MAX_FILES_PER_REQUEST = 50;
            int nrOfRequests = (usableIds.Count + MAX_FILES_PER_REQUEST - 1) / MAX_FILES_PER_REQUEST;
            StringBuilder logBuilder = new StringBuilder();
            for (int i = 0; i < nrOfRequests; ++i)
            {
                m_Monitor.SetIndeterminate(StringResources.QueryingGlobalDB);
                List<Ares.Tags.FileIdentification> idsInRequest = new List<Tags.FileIdentification>();
                for (int j =  i * MAX_FILES_PER_REQUEST; j < (i+1)* MAX_FILES_PER_REQUEST; ++j)
                {
                    if (j >= usableIds.Count)
                        break;
                    idsInRequest.Add(usableIds[j]);
                }
                var request = new RestSharp.RestRequest(GlobalDb.DownloadPath, RestSharp.Method.POST);
                request.RequestFormat = RestSharp.DataFormat.Json;
                String serializedFileIds = ServiceStack.Text.TypeSerializer.SerializeToString<List<Ares.Tags.FileIdentification>>(idsInRequest);
                request.AddParameter("FileIdentification", serializedFileIds);
                if (Settings.Default.UseTestDB)
                {
                    request.AddParameter("Test", true);
                }
                var client = new RestSharp.RestClient();
                client.BaseUrl = GlobalDb.BaseUrl;
                client.Timeout = 20 * 1000;
                var response = client.Execute<DownloadResponse>(request);
                m_Token.ThrowIfCancellationRequested();
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

                nrOfFoundFiles += response.Data.NrOfFoundFiles;
                m_Monitor.SetIndeterminate(StringResources.AddingTags);
                using (System.IO.StringWriter writer = new System.IO.StringWriter())
                {
                    Ares.Tags.TagsModule.GetTagsDB().FilesInterface.ImportDataFromGlobalDB(response.Data.TagsData, files, writer);
                    if (includeLog)
                    {
                        if (i > 0)
                        {
                            logBuilder.AppendLine("-------------------------------------------------------");
                        }
                        logBuilder.Append(writer.ToString());
                    }
                }
            }
            return logBuilder.ToString();
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