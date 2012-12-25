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

using System.Data.SQLite;

namespace Ares.Tags
{
    class TagsDBReading : ITagsDBRead, IConnectionClient
    {
        private SQLiteConnection m_Connection;

        internal TagsDBReading(SQLiteConnection connection)
        {
            m_Connection = connection;
        }

        public void ConnectionChanged(SQLiteConnection connection)
        {
            m_Connection = connection;
        }

        public IList<string> GetAllFilesWithAnyTag(IList<int> tagIds)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetAllFilesWithAnyTag(tagIds);
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (SQLiteException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        private IList<string> DoGetAllFilesWithAnyTag(IList<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                return new List<String>();
            }
            String queryString = String.Format("SELECT DISTINCT {0}.{1} FROM {0}, {2} WHERE {0}.{3}={2}.{4} AND {2}.{5} IN (",
                Schema.FILES_TABLE, Schema.PATH_COLUMN, Schema.FILETAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            for (int i = 0; i < tagIds.Count; ++i)
            {
                queryString += tagIds[i];
                if (i != tagIds.Count - 1)
                    queryString += ",";
            }
            queryString += ")";
            using (SQLiteCommand command = new SQLiteCommand(queryString, m_Connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                List<String> result = new List<string>();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
                return result;
            }
        }
    }
}