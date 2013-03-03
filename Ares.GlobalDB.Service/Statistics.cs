/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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

using ServiceStack.OrmLite;
using System.Data;
using ServiceStack.DataAnnotations;

namespace Ares.GlobalDB.Services
{
    class UploadStats
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int NrOfNewFiles { get; set; }
        public int NrOfNewTags { get; set; }
        public int NrOfUploads { get; set; }
        // public DateTime Date { get; set; } // would be stored as string, can't query with >=
        public long Date { get; set; }
    }

    class DownloadStats
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int NrOfDownloads { get; set; }
        public int NrOfRequestedFiles { get; set; }
        public int NrOfFoundFiles { get; set; }
        // public DateTime Date { get; set; }
        public long Date { get; set; }
    }

    class Version
    {
        public int Id { get; set; }
        public int Nr { get; set; }

        public static int Current { get { return 1; } }
    }

    public class UploadStatsPerTime
    {
        public int NrOfUploads { get; set; }
        public int NrOfNewFiles { get; set; }
        public int NrOfNewTags { get; set; }
    }

    public class DownloadStatsPerTime
    {
        public int NrOfDownloads { get; set; }
        public int NrOfRequestedFiles { get; set; }
        public int NrOfFoundFiles { get; set; }
        public double PercentFoundFiles { get { return NrOfFoundFiles == 0 ? 0.0 : (double)NrOfFoundFiles / (double)NrOfRequestedFiles * 100.0; } }
    }

    public class UploadStatistics
    {
        public UploadStatsPerTime Today { get; set; }
        public UploadStatsPerTime LastWeek { get; set; }
        public UploadStatsPerTime LastMonth { get; set; }
        public UploadStatsPerTime LastYear { get; set; }
    }

    public class DownloadStatistics
    {
        public DownloadStatsPerTime Today { get; set; }
        public DownloadStatsPerTime LastWeek { get; set; }
        public DownloadStatsPerTime LastMonth { get; set; }
        public DownloadStatsPerTime LastYear { get; set; }
    }

    class StatisticsDB
    {
        private bool m_TestDB;

        public static StatisticsDB GetStatisticsDB(bool test)
        {
            return new StatisticsDB(test);
        }

        private StatisticsDB(bool test)
        {
            m_TestDB = test;
        }

        public void InsertUpload(int nrOfNewFiles, int nrOfNewTags)
        {
            using (var conn = OpenStatisticsDB())
            {
                var stats = conn.Select<UploadStats>(s => s.Date == DateTime.Today.Ticks );
                if (stats != null && stats.Count > 0)
                {
                    stats[0].NrOfNewFiles += nrOfNewFiles;
                    stats[0].NrOfNewTags += nrOfNewTags;
                    ++stats[0].NrOfUploads;
                    conn.Update(stats[0]);
                }
                else
                {
                    var newStats = new UploadStats { NrOfUploads = 1, NrOfNewTags = nrOfNewTags, NrOfNewFiles = nrOfNewFiles, Date = DateTime.Today.Ticks };
                    conn.Insert(newStats);
                }
            }
        }

        public void InsertDownload(int nrOfRequestedFiles, int nrOfFoundFiles)
        {
            using (var conn = OpenStatisticsDB())
            {
                var stats = conn.Select<DownloadStats>(s => s.Date == DateTime.Today.Ticks );
                if (stats != null && stats.Count > 0)
                {
                    stats[0].NrOfRequestedFiles += nrOfRequestedFiles;
                    stats[0].NrOfFoundFiles += nrOfFoundFiles;
                    ++stats[0].NrOfDownloads;
                    conn.Update(stats[0]);
                }
                else
                {
                    var newStats = new DownloadStats { NrOfDownloads = 1, NrOfFoundFiles = nrOfFoundFiles, NrOfRequestedFiles = nrOfRequestedFiles, Date = DateTime.Today.Ticks };
                    conn.Insert(newStats);
                }
            }
        }

        public void GetStatistics(out UploadStatistics uploadStats, out DownloadStatistics downloadStats)
        {
            UploadStatistics stats = new UploadStatistics();
            DownloadStatistics stats2 = new DownloadStatistics();
            using (var conn = OpenStatisticsDB())
            {
                DateTime date = DateTime.Today;
                stats.Today = GetUploadStatsByTime(conn, date);
                stats2.Today = GetDownloadStatsByTime(conn, date);
                DateTime lastWeek = date.AddDays(-7);
                stats.LastWeek = GetUploadStatsByTime(conn, lastWeek);
                stats2.LastWeek = GetDownloadStatsByTime(conn, lastWeek);
                DateTime lastMonth = date.AddDays(-date.Day);
                stats.LastMonth = GetUploadStatsByTime(conn, lastMonth);
                stats2.LastMonth = GetDownloadStatsByTime(conn, lastMonth);
                DateTime lastYear = date.AddDays(-date.DayOfYear);
                stats.LastYear = GetUploadStatsByTime(conn, lastYear);
                stats2.LastYear = GetDownloadStatsByTime(conn, lastYear);
                PurgeOldStatistics(conn);
            }
            uploadStats = stats;
            downloadStats = stats2;
        }

        private UploadStatsPerTime GetUploadStatsByTime(IDbConnection conn, DateTime date)
        {
            return conn.QuerySingle<UploadStatsPerTime>("SELECT SUM(NrOfUploads) AS NrOfUploads, SUM(NrOfNewFiles) AS NrOfNewFiles, SUM(NrOfNewTags) AS NrOfNewTags FROM UploadStats WHERE Date >= @Date",
                new { Date = date.Ticks });
        }

        private DownloadStatsPerTime GetDownloadStatsByTime(IDbConnection conn, DateTime date)
        {
            return conn.QuerySingle<DownloadStatsPerTime>("SELECT SUM(NrOfDownloads) AS NrOfDownloads, SUM(NrOfRequestedFiles) AS NrOfRequestedFiles, SUM(NrOfFoundFiles) AS NrOfFoundFiles FROM DownloadStats WHERE Date >= @Date",
                new { Date = date.Ticks });
        }

        private void PurgeOldStatistics(IDbConnection conn)
        {
            // keep database small
            DateTime toPurge = DateTime.Today.AddYears(-2);
            using (var transaction = conn.BeginTransaction())
            {
                conn.Delete<UploadStats>(s => s.Date < toPurge.Ticks);
                conn.Delete<DownloadStats>(s => s.Date < toPurge.Ticks);
                transaction.Commit();
            }
        }

        static StatisticsDB()
        {
            OrmLiteConfig.DialectProvider = SqliteDialect.Provider;
        }

        private static String DB_FILE = "TagsDBStatistics.sqlite";
        private static String TEST_FILE = "TagsDBTestStatistics.sqlite";

        private static String DbFile
        {
            get
            {
                return System.Web.HttpContext.Current.Server.MapPath(@"~\App_Data\" + DB_FILE);
            }
        }

        private static String TestFile
        {
            get
            {
                return System.Web.HttpContext.Current.Server.MapPath(@"~\App_Data\" + TEST_FILE);
            }
        }

        private IDbConnection OpenStatisticsDB()
        {
            String fileName = m_TestDB ? TestFile : DbFile;
            if (System.IO.File.Exists(fileName))
            {
                return fileName.OpenDbConnection();
            }
            else
            {
                return CreateStatisticsDB(fileName);
            }
        }

        private IDbConnection CreateStatisticsDB(String fileName)
        {
            IDbConnection conn = fileName.OpenDbConnection();
            using (var transaction = conn.BeginTransaction())
            {
                conn.CreateTable<UploadStats>();
                conn.CreateTable<DownloadStats>();
                conn.CreateTable<Version>();
                conn.Insert(new Version { Nr = Version.Current });
                transaction.Commit();
            }
            return conn;
        }
    }
}
