/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
using System.Text;
using System.Data.Common;

#if ANDROID
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace Ares.Tags
{

    static class DbUtils
    {
        public static DbCommand CreateDbCommand(String commandString, DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandString;
            command.CommandType = System.Data.CommandType.Text;
            return command;
        }

        public static DbCommand CreateDbCommand(String commandString, DbConnection connection, DbTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = commandString;
            command.CommandType = System.Data.CommandType.Text;
            return command;
        }

        public static DbParameter AddParameter(this DbCommand command, String name, System.Data.DbType paramType)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = paramType;
            command.Parameters.Add(param);
            return param;
        }

        public static DbParameter AddParameterWithValue(this DbCommand command, String name, Int64 value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = System.Data.DbType.Int64;
            param.Value = value;
            command.Parameters.Add(param);
            return param;
        }

        public static DbParameter AddParameterWithValue(this DbCommand command, String name, String value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = System.Data.DbType.String;
            param.Value = value;
            command.Parameters.Add(param);
            return param;
        }

        public static DbParameter AddParameterWithValue(this DbCommand command, String name, System.DBNull value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = System.Data.DbType.Object;
            param.Value = value;
            command.Parameters.Add(param);
            return param;
        }

        public static String GetStringOrEmpty(this DbDataReader reader, int columnIndex)
        {
            return reader.IsDBNull(columnIndex) ? String.Empty : reader.GetString(columnIndex);
        }

        public static Int64 LastInsertRowId(this DbConnection connection)
        {
			#if ANDROID
			if (connection is SqliteConnection)
			{
				using (DbCommand command = CreateDbCommand("select last_insert_rowid()", connection))
				{
					return (Int64)command.ExecuteScalar();
				}
			}
			#else
            if (connection is SQLiteConnection)
            {
                return ((SQLiteConnection)connection).LastInsertRowId;
            }
			#endif
            else
                throw new NotImplementedException();
        }
    }

}