using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;

namespace Server
{
    public static class DatabaseWrapper
    {
        public static int GetSpeedFromDb(long serialNumber)
        {
            var speed = -1;
            using (var dbConnection = new SQLiteConnection(Database.ConnectionString))
            {
                dbConnection.Open();
                using (
                    var sqlCommand =
                        new SQLiteCommand(
                            $"SELECT {Database.Speed} FROM {Database.TableName} WHERE {Database.SerialNumber} = {serialNumber};",
                            dbConnection))
                {
                    IDataReader reader = sqlCommand.ExecuteReader();

                    if (!reader.Read()) return speed;
                    if (!(reader[Database.Speed] is DBNull))
                    {
                        speed = (int)reader[Database.Speed];
                    }
                    else
                    {
                        // not found
                    }
                }
                dbConnection.Close();
            }

            return speed;
        }

        public static List<DatabaseEntry> LoadAllFromDatabase()
        {
            List<DatabaseEntry> allDatabaseEntries;
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand($"SELECT * FROM {Database.TableName}", connection))
                {
                    var reader = command.ExecuteReader();
                    allDatabaseEntries = ReadDataToList(reader);
                }
                connection.Close();
            }
            return allDatabaseEntries; // can be null?
        }

        public static List<DatabaseEntry> LoadZoneFromDb(int zone)
        {
            List<DatabaseEntry> entries;
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM {Database.TableName} WHERE {Database.Zone} = {zone} ";
                    var reader = cmd.ExecuteReader();
                    entries = ReadDataToList(reader);
                }
                connection.Close();
            }

            return entries; // can be null?
        }

        public static List<DatabaseEntry> LoadZoneAfterTimeStamp(int zone, long timestamp)
        {
            List<DatabaseEntry> entries;
            using (var con = new SQLiteConnection(Database.ConnectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM {Database.TableName} WHERE {Database.Zone} = {zone} " +
                                      $"AND {Database.Timestamp} > {timestamp} ";
                    var reader = cmd.ExecuteReader();
                    entries = ReadDataToList(reader);
                }
            }

            return entries;
        }

        public static bool AddEntry(DatabaseEntry entry)
        {
            if (entry == null || !entry.CheckData())
            {
                return false;
            }

            var retval = InsertData(new Rfid(entry.SerialNumber, entry.Speed), entry.Zone, entry.Timestamp);
            return retval;
        }

        public static bool UpdateEntry(DatabaseEntry entry)
        {
            if (entry == null || !entry.CheckData())
            {
                return false;
            }

            int updated;
            try
            {
                using (var cn = new SQLiteConnection(Database.ConnectionString))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText =
                            $"UPDATE {Database.TableName} " +
                            $"SET {Database.Speed} = {entry.Speed}, {Database.Timestamp} = {entry.Timestamp} " + // do not insert the entry timestamp here
                            $"WHERE {Database.SerialNumber} = {entry.SerialNumber}";
                        updated = cmd.ExecuteNonQuery();
                    }
                    cn.Close();
                }
            }
            catch (SQLiteException)
            {
                return false;
            }
            return updated > 0;
        }

        public static bool AddOrUpdateEntry(DatabaseEntry entry)
        {
            var allEntries = LoadAllFromDatabase();
            var update = allEntries.Any(databaseEntry => databaseEntry.SerialNumber == entry.SerialNumber);
            return update ? UpdateEntry(entry) : AddEntry(entry);
        }

        public static int AddOrUpdateEntries(List<DatabaseEntry> entries)
        {
            if (entries == null || entries.Count < 1 || !DatabaseEntry.CheckList(entries))
            {
                return -1;
            }

            var results = new List<int>();
            try
            {
                using (var cn = new SQLiteConnection(Database.ConnectionString))
                {
                    cn.Open();
                    using (SQLiteCommand insertCommand = cn.CreateCommand(), updateCommand = cn.CreateCommand())
                    {
                        using (var transaction = cn.BeginTransaction())
                        {
                            insertCommand.Transaction = transaction;
                            updateCommand.Transaction = transaction;
                            insertCommand.CommandText =
                                $"INSERT INTO {Database.TableName} " +
                                $"({Database.SerialNumber},{Database.Speed},{Database.Zone},{Database.Timestamp}) " +
                                "VALUES (@SerialNumber,@Speed,@Zone,@Timestamp)";
                            updateCommand.CommandText =
                                $"UPDATE {Database.TableName} " +
                                $"SET {Database.Speed} = @Speed, {Database.Timestamp} = @Timestamp, {Database.Zone} = @Zone " +
                                $"WHERE {Database.SerialNumber} = @SerialNumber";

                            insertCommand.Prepare();
                            updateCommand.Prepare();

                            var allEntries = LoadAllFromDatabase();
                            foreach (var databaseEntry in entries)
                            {
                                var update = allEntries.Any(entry => entry.SerialNumber == databaseEntry.SerialNumber);
                                if (update)
                                {
                                    updateCommand.Parameters.AddWithValue("@SerialNumber", databaseEntry.SerialNumber);
                                    updateCommand.Parameters.AddWithValue("@Speed", databaseEntry.Speed);
                                    updateCommand.Parameters.AddWithValue("@Zone", databaseEntry.Zone);
                                    updateCommand.Parameters.AddWithValue("@Timestamp", databaseEntry.Timestamp);
                                    results.Add(updateCommand.ExecuteNonQuery());
                                }
                                else
                                {
                                    insertCommand.Parameters.AddWithValue("@SerialNumber", databaseEntry.SerialNumber);
                                    insertCommand.Parameters.AddWithValue("@Speed", databaseEntry.Speed);
                                    insertCommand.Parameters.AddWithValue("@Zone", databaseEntry.Zone);
                                    insertCommand.Parameters.AddWithValue("@Timestamp", databaseEntry.Timestamp);
                                    results.Add(insertCommand.ExecuteNonQuery());
                                }

                            }
                            transaction.Commit();
                        }

                    }
                    cn.Close();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex);
            }
            return results.Sum();
        }

        public static long GetLatestTimestamp()
        {
            long timestamp = 0;
            var sqlQuery = $"SELECT MAX({Database.Timestamp}) FROM {Database.TableName};";
            try
            {
                using (var cn = new SQLiteConnection(Database.ConnectionString))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;
                        var obj = cmd.ExecuteScalar();
                        if (!(obj is DBNull))
                        {
                            timestamp = (long)obj;
                        }
                    }
                    cn.Close();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex);
            }

            return timestamp;

        }

        private static List<DatabaseEntry> ReadDataToList(IDataReader reader)
        {
            var databaseEntries = new List<DatabaseEntry>();
            while (reader.Read())
            {
                var serialNumber = (long)reader[Database.SerialNumber];
                var speed = (int)reader[Database.Speed];
                long timestamp = 0;
                var zone = (int)reader[Database.Zone];
                var timestampobj = reader[Database.Timestamp];
                if (timestampobj.GetType() != typeof(DBNull))
                {
                    timestamp = (long)timestampobj;
                }
                var entry = new DatabaseEntry(serialNumber, speed, zone, timestamp);
                databaseEntries.Add(entry);
            }
            return databaseEntries;
        }

        public static bool InsertData(Rfid rfid, int zone, long timestamp = -1)
        {
            try
            {
                using (var cn = new SQLiteConnection(Database.ConnectionString))
                {
                    cn.Open();
                    var longDate = timestamp < 0 ? Database.ConvertToTimestamp(DateTime.UtcNow) : timestamp;
                    using (var sqlCommand = cn.CreateCommand())
                    {
                        sqlCommand.CommandText =
                            $"INSERT INTO {Database.TableName} ({Database.SerialNumber}, {Database.Speed}, {Database.Zone}, {Database.Timestamp}) VALUES ({rfid.SerialNumber}, {rfid.Speed}, {zone}, {longDate})";
                        sqlCommand.ExecuteNonQuery();
                    }
                    cn.Close();
                }
            }
            catch (SQLiteException)
            {
                return false;
            }
            return true;
        }

        public static int DeleteAllEntries()
        {
            try
            {
                int deleted;
                using (var cn = new SQLiteConnection(Database.ConnectionString))
                {
                    cn.Open();
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = $"DELETE FROM {Database.TableName}; " + // deletes all entries
                                          $"VACUUM {Database.TableName}"; // cleans up database space
                        deleted = cmd.ExecuteNonQuery();
                    }
                    cn.Close();
                }
                return deleted / 2; // vacuum also reports how many rows it affected, I guess
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        // methods for dummy data
        private static long LongRandom(long min, long max, Random rand)
        {
            var buf = new byte[8];
            rand.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }

        public static void CreateDummyData()
        {
            var databaseEntries = new List<DatabaseEntry>();

            var random = new Random();
            const int amountOfRows = 1000;
            for (var i = 0; i < amountOfRows; i++)
            {
                var serialNumber = LongRandom(Rfid.MinHexSerialNumber, Rfid.MaxHexSerialNumber, random);
                var rfid = new Rfid(serialNumber, random.Next(Rfid.MinSpeed, Rfid.MaxSpeed));
                var zone = random.Next(1, 1000);
                var entry = new DatabaseEntry(rfid.SerialNumber, rfid.Speed, zone,
                    Database.ConvertToTimestamp(DateTime.Now));
                if (!databaseEntries.Contains(entry))
                {
                    databaseEntries.Add(entry);
                }
            }
            var addedEntries = AddOrUpdateEntries(databaseEntries);
            Console.WriteLine(addedEntries > 0 ? "Created dummy data" : "Dummy data creation failed");
        }
    }
}
