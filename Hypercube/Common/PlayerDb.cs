// -- Created by umby24
//#define MONO
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
#if MONO
using Mono.Data.Sqlite;
#else

#endif

namespace ZBase.Common {
	public class PlayerDb {
		const string DatabaseName = "Database.s3db";
#if MONO
        private SqliteConnection _dbConnection;
#else
	    private SQLiteConnection _dbConnection;
#endif
        readonly object _dbLock = new object();

		public PlayerDb () {
			if (!File.Exists (DatabaseName)) {
				CreateInitialDb ();
				return;
			}

			ConnectDb ();
		}

		/// <summary>
		/// Creates a new entry in the PlayerDB for a player.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="ip"></param>
		public void CreatePlayer(string name, string ip) {
			var myValues = new Dictionary<string, string>
			{
				{"Name", name},
				{"IP", ip},
				{"Rank", "65535"},
				{"Global", "1"},
				{"Banned", "0"},
				{"Stopped", "0"},
				{"Vanished", "0"},
				{"BoundBlock", "1"},
			    {"Time_Muted", "0"},
			    {"BannedUntil", "0"}
			};

			Insert("PlayerDB", myValues);
		}

		/// <summary>
		/// Checks to see if a player by this name already exists for this server.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool ContainsPlayer(string name) {
			DataTable dt = GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + name + "'");

			foreach (DataRow c in dt.Rows)
				if (((string) c["Name"]).ToLower() == name.ToLower()) return true;

			return false;
		}

	    public bool IsIpBanned(string ip) {
	        DataTable dt = GetDataTable("SELECT * FROM IPBanDB WHERE IP ='" + ip + "'");

	        return dt.Rows.Count > 0;
	    }

	    public void IpBan(string ip, string reason = "", string banner = "") {
	        var myValues = new Dictionary<string, string> {
	            {"IP", ip},
	            {"Reason", reason},
	            {"BannedBy", banner}
	        };

	        Insert("IPBanDB", myValues);
	    }

	    public void UnIpBan(string ip) {
	        Delete("IPBanDB", "IP = '" + ip + "'");
	    }
#if MONO
        private void CreateInitialDb() {
			SqliteConnection.CreateFile (DatabaseName);

			lock (_dbLock) {
				var connection = new SqliteConnection ($"Data Source={DatabaseName}");
				connection.Open ();

				var cmd = new SqliteCommand ("CREATE TABLE PlayerDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Rank INTEGER, BoundBlock INTEGER, RankChangedBy TEXT, LoginCounter INTEGER, KickCounter INTEGER, Ontime INTEGER, LastOnline INTEGER, IP TEXT, Stopped INTEGER, StoppedBy TEXT, Banned INTEGER, Vanished INTEGER, BannedBy STRING, BannedUntil INTEGER, Global INTEGER, Time_Muted INTEGER, BanMessage TEXT, KickMessage TEXT, MuteMessage TEXT, RankMessage TEXT, StopMessage TEXT)", connection);
				cmd.ExecuteNonQuery ();

				cmd.CommandText = "CREATE INDEX PlayerDB_Index ON PlayerDB (Name COLLATE NOCASE)";
				cmd.ExecuteNonQuery ();

				cmd.CommandText = "CREATE TABLE IPBanDB (Number INTEGER PRIMARY KEY, IP TEXT UNIQUE, Reason TEXT, BannedBy TEXT)";
				cmd.ExecuteNonQuery ();

				_dbConnection = connection;
			}
		}

		private void ConnectDb() {
			_dbConnection = new SqliteConnection ($"Data Source={DatabaseName}");
			_dbConnection.Open ();
		}
#else
        private void CreateInitialDb() {
            SQLiteConnection.CreateFile(DatabaseName);

            lock (_dbLock) {
                var connection = new SQLiteConnection($"Data Source={DatabaseName}");
                connection.Open();

                var cmd = new SQLiteCommand("CREATE TABLE PlayerDB (Number INTEGER PRIMARY KEY, Name TEXT UNIQUE, Rank INTEGER, BoundBlock INTEGER, RankChangedBy TEXT, LoginCounter INTEGER, KickCounter INTEGER, Ontime INTEGER, LastOnline INTEGER, IP TEXT, Stopped INTEGER, StoppedBy TEXT, Banned INTEGER, Vanished INTEGER, BannedBy STRING, BannedUntil INTEGER, Global INTEGER, Time_Muted INTEGER, BanMessage TEXT, KickMessage TEXT, MuteMessage TEXT, RankMessage TEXT, StopMessage TEXT)", connection);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE INDEX PlayerDB_Index ON PlayerDB (Name COLLATE NOCASE)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IPBanDB (Number INTEGER PRIMARY KEY, IP TEXT UNIQUE, Reason TEXT, BannedBy TEXT)";
                cmd.ExecuteNonQuery();

                _dbConnection = connection;
            }
        }

        private void ConnectDb() {
            _dbConnection = new SQLiteConnection($"Data Source={DatabaseName}");
            _dbConnection.Open();
        }
#endif

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void SetDatabase(string name, string table, string field, bool value) {
            var values = new Dictionary<string, string> { { field, value.ToString() } };

            Update(table, values, "Name='" + name + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void SetDatabase(string name, string table, string field, int value) {
            var values = new Dictionary<string, string> { { field, value.ToString() } };
            Update(table, values, "Name='" + name + "'");
        }

        /// <summary>
        /// Sets a value on the player in the database.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void SetDatabase(string name, string table, string field, string value) {
            var values = new Dictionary<string, string> { { field, value } };
            Update(table, values, "Name='" + name + "'");
        }

        #region Basic DB Interaction
        // -- Taken / Inspired from.
        // -- http://www.dreamincode.net/forums/topic/157830-using-Sqlite-with-c%23/#/
#if MONO
        public DataTable GetDataTable(string sql) {
			var dt = new DataTable();

			try {
				lock (_dbLock) {
					var command = new SqliteCommand(_dbConnection) {CommandText = sql};

					var reader = command.ExecuteReader();
					dt.Load(reader);
					reader.Close();
				}
			} catch (SqliteException e) {
				throw new Exception(e.Message);
			}

			return dt;
		}

		public int ExecuteNonQuery(string sql) {
			lock (_dbLock) {
				var command = new SqliteCommand(_dbConnection) {CommandText = sql};

				var rowsUpdated = command.ExecuteNonQuery();

				return rowsUpdated;
			}
		}
#else
        public DataTable GetDataTable(string sql) {
            var dt = new DataTable();

            try {
                lock (_dbLock) {
                    var command = new SQLiteCommand(_dbConnection) { CommandText = sql };

                    SQLiteDataReader reader = command.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }
            } catch (SQLiteException e) {
                throw new Exception(e.Message);
            }

            return dt;
        }

        public int ExecuteNonQuery(string sql) {
            lock (_dbLock) {
                var command = new SQLiteCommand(_dbConnection) { CommandText = sql };

                int rowsUpdated = command.ExecuteNonQuery();

                return rowsUpdated;
            }
        }
#endif
        public bool Update(string tableName, Dictionary<string, string> data, string where) {
			var vals = "";
			var returnCode = true;

			if (data.Count >= 1) {
				foreach (KeyValuePair<string, string> val in data) 
					vals += $" {val.Key} = '{val.Value}',";

				vals = vals.Substring(0, vals.Length - 1);
			}

			try {
				ExecuteNonQuery($"update {tableName} set {vals} where {where};");
			} catch {
				returnCode = false;
			}

			return returnCode;
		}

		/// <summary>
		///     Allows the programmer to easily delete rows from the DB.
		/// </summary>
		/// <param name="tableName">The table from which to delete.</param>
		/// <param name="where">The where clause for the delete.</param>
		/// <returns>A boolean true or false to signify success or failure.</returns>
		public bool Delete(string tableName, string where) {
			var returnCode = true;

			try {
				ExecuteNonQuery($"delete from {tableName} where {where};");
			} catch {
				returnCode = false;
			}

			return returnCode;
		}

		/// <summary>
		///     Allows the programmer to easily insert into the DB
		/// </summary>
		/// <param name="tableName">The table into which we insert the data.</param>
		/// <param name="data">A dictionary containing the column names and data for the insert.</param>
		/// <returns>A boolean true or false to signify success or failure.</returns>
		public bool Insert(string tableName, Dictionary<string, string> data) {
			var columns = "";
			var values = "";
			var returnCode = true;

			foreach (KeyValuePair<string, string> val in data) {
				columns += $" {val.Key},";
				values += $" '{val.Value}',";
			}

			columns = columns.Substring(0, columns.Length - 1);
			values = values.Substring(0, values.Length - 1);

			try {
				ExecuteNonQuery($"insert into {tableName}({columns}) values({values});");
			} catch (Exception) {
				returnCode = false;
			}

			return returnCode;
		}

		/// <summary>
		///     Allows the programmer to easily delete all data from the DB.
		/// </summary>
		/// <returns>A boolean true or false to signify success or failure.</returns>
		public bool ClearDb() {
			try {
				DataTable tables = GetDataTable("select NAME from Sqlite_MASTER where type='table' order by NAME;");

				foreach (DataRow table in tables.Rows) 
					ClearTable(table["NAME"].ToString());

				return true;
			} catch {
				return false;
			}
		}

		public bool ClearTable(string table) {
			try {
				ExecuteNonQuery($"delete from {table};");
				return true;
			} catch {
				return false;
			}
		}

#endregion
	}
}

